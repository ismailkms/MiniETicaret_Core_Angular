using ETicaretAPI.API.Configurations.ColumnWriters;
using ETicaretAPI.API.Extensions;
using ETicaretAPI.API.Filters;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using ETicaretAPI.SignalR;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();//Client'tan gelen request neticesinde oluþturulan HttpContext nesnesine katmanlardaki class'lar üzerinden(busineess logic) eriþebilmemizi saðlayan bir servistir. HttpContext üzerinden User.Identity.Name'e eriþebiliyoruz.

builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddSignalRServices();

//builder.Services.AddStorage<LocalStorage>();
builder.Services.AddStorage<LocalStorage>();
//builder.Services.AddStorage(ETicaretAPI.Infrastructure.Enums.StorageType.Local);

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));
//Cors Politikasý

Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt")
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs", needAutoCreateTable: true,
        columnOptions: new Dictionary<string, ColumnWriterBase>
        {
            {"message", new RenderedMessageColumnWriter()},
            {"message_template", new MessageTemplateColumnWriter()},
            {"level", new LevelColumnWriter()},
            {"time_stamp", new TimestampColumnWriter()},
            {"exception", new ExceptionColumnWriter()},
            {"log_event", new LogEventSerializedColumnWriter()},
            {"user_name", new UsernameColumnWriter()}
        })
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"])
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
//Logger'ýn Serilog.Core'dan gelmesi gerekiyor.
builder.Host.UseSerilog(log);
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<RolePermissionFilter>(); //Yetkilendirme için ekledik
})
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
//RegisterValidatorsFromAssemblyContaining'in içerisine Application katmanýnda oluþturduðumuz her hangi bir validator class'ý verebiliriz. Buradan sonra Assembly'deki yani Application katmanýndaki tüm validator'ler çalýþacaktýr. Burada önemli olan dosyanýn yolunu(assembly) almak.
//ConfigureApiBehaviorOptions'un üstündeki iþlemi yaptýðýmýz zaman otomatik filtreleme yapýp proje daha controller'a gelmeden hataya geri dönüþ yapýlacaktýr. Biz ConfigureApiBehaviorOptions ve devamýný yazarak controllerde iþlemi manuel yapmak için otomatik filtrelemeyi devre dýþý býrakýyoruz. Manuel filtreleme iþlemi yapacaðýmýzdan dolayý otomatik filtrelemeyi kullanmamak biraz kompleks bir yapýdýr. Bu nedenle devre dýþý býrakýlmadan kullanýlabilir.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, //Oluþturulacak token deðerini kimlerin/hangi originlerin/sitelerin kullanacaðýný belirlediðimiz deðerdir. -> www.bilmemne.com
            ValidateIssuer = true, //Oluþturulacak token deðerini kimin daðýttýðýný ifade edeceðimiz alandýr. -> www.myapi.com
            ValidateLifetime = true, //Oluþturulan token deðerinin süresini kontrol edecek olan doðrulamadýr.
            ValidateIssuerSigningKey = true, //Üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden security key verisinin doðrulanmasýdýr.

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false, //Jwt expires geçerlilik süresini belirtir. Token'in oluþturulduðu zamana istenilen saniye/dakika/saat eklenir ve expires'te tutulur. Eðer þu anki zaman(Datetime.UtcNow/Now) expires'teki zamaný geçerse token'in yetkisi biter.
            NameClaimType = ClaimTypes.Name //JWT üzerinde Name claimine karþýlýk gelen deðeri User.Identity.Name propertysinden elde edebiliriz.
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

app.UseStaticFiles();

app.UseSerilogRequestLogging(); //Loglanmasýný istediklerimizin üstüne yazmalýyýz. UseStaticFiles'ý istemiyoruz mesela

app.UseHttpLogging();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});

app.MapControllers();

app.MapHubs();

app.Run();
