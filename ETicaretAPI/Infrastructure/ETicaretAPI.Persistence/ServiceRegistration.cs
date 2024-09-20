using Microsoft.EntityFrameworkCore;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;
using ETicaretAPI.Application.Repositories.CustomerRepository;
using ETicaretAPI.Persistence.Repositories.CustomerRepository;
using ETicaretAPI.Application.Repositories.OrderRepository;
using ETicaretAPI.Persistence.Repositories.OrderRepository;
using ETicaretAPI.Persistence.Repositories.ProductRepository;
using ETicaretAPI.Application.Repositories.ProductRepository;
using ETicaretAPI.Application.Repositories.FileRepository;
using ETicaretAPI.Persistence.Repositories.FileRepository;
using ETicaretAPI.Application.Repositories.InvoiceFileRepository;
using ETicaretAPI.Persistence.Repositories.InvoiceFileRepository;
using ETicaretAPI.Application.Repositories.ProductImageFileRepository;
using ETicaretAPI.Persistence.Repositories.ProductImageFileRepository;
using ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Persistence.Services;
using ETicaretAPI.Application.Abstractions.Services.Authentications;
using ETicaretAPI.Persistence.Repositories.BasketRepository;
using ETicaretAPI.Application.Repositories.BasketRepository;
using ETicaretAPI.Application.Repositories.BasketItemRepository;
using ETicaretAPI.Persistence.Repositories.BasketItemRepository;
using Microsoft.AspNetCore.Identity;
using ETicaretAPI.Application.Repositories.CompletedOrderRepository;
using ETicaretAPI.Persistence.Repositories.CompletedOrderRepository;
using ETicaretAPI.Application.Repositories.MenuRepository;
using ETicaretAPI.Persistence.Repositories.MenuRepository;
using ETicaretAPI.Persistence.Repositories.EndpointRepository;
using ETicaretAPI.Application.Repositories.EndpointRepository;

namespace ETicaretAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql(Configuration.ConnectionString));
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 2;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ETicaretAPIDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IFileReadRepository, FileReadRepository>();
            services.AddScoped<IFileWriteRepository, FileWriteRepository>();
            services.AddScoped<IInvoiceFileReadRepository, InvoiceFileReadRepository>();
            services.AddScoped<IInvoiceFileWriteRepository, InvoiceFileWriteRepository>();
            services.AddScoped<IProductImageFileReadRepository, ProductImageFileReadRepository>();
            services.AddScoped<IProductImageFileWriteRepository, ProductImageFileWriteRepository>();
            services.AddScoped<IBasketReadRepository, BasketReadRepository>();
            services.AddScoped<IBasketWriteRepository, BasketWriteRepository>();
            services.AddScoped<IBasketItemReadRepository, BasketItemReadRepository>();
            services.AddScoped<IBasketItemWriteRepository, BasketItemWriteRepository>();
            services.AddScoped<ICompletedOrderReadRepository, CompletedOrderReadRepository>();
            services.AddScoped<ICompletedOrderWriteRepository, CompletedOrderWriteRepository>();
            services.AddScoped<IMenuReadRepository, MenuReadRepository>();
            services.AddScoped<IMenuWriteRepository, MenuWriteRepository>();
            services.AddScoped<IEndpointReadRepository, EndpointReadRepository>();
            services.AddScoped<IEndpointWriteRepository, EndpointWriteRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExternalAuthentication, AuthService>();
            services.AddScoped<IInternalAuthentication, AuthService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthorizationEndpointService, AuthorizationEndpointService>();
            services.AddScoped<IProductService, ProductService>();

        }
    }
}
