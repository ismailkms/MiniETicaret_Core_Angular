using ETicaretAPI.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class MailService : IMailService
    {
        readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendMailAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new();
            mail.IsBodyHtml = isBodyHtml;
            mail.Subject = subject;
            mail.Body = body;
            foreach (var to in tos)
                mail.To.Add(to);
            mail.From = new(_config["Mail:UserName"], "E-Ticaret", System.Text.Encoding.UTF8);

            SmtpClient smtp = new();
            smtp.Credentials = new NetworkCredential(_config["Mail:UserName"], _config["Mail:Password"]);
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Host = _config["Mail:Host"];
            await smtp.SendMailAsync(mail);
        }

        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            StringBuilder mail = new();
            mail.Append("Merhaba<br>Eğer yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirisniz.<br><strong><a target=\"_blank\" href=\"");
            mail.Append(_config["AngularClientUrl"]);
            mail.Append("/update-password/");
            mail.Append(userId);
            mail.Append("/");
            mail.Append(resetToken);
            mail.Append("\">Şifreyi değiştirmek için tıklayınız.</a></strong><br><br><span style=\"font-size: 12px;\">NOT: Eğer ki bu talep tarafınızca gerçekleştirilmemişse lütfen bu talebi ciddiye almayınız.</span><br><br>E-Ticaret");

            await SendMailAsync(to, "Şifre Yenileme Talebi", mail.ToString());
        }

        public async Task SendCompletedOrderMailAsync(string to, string orderCode, DateTime orderDate, string userNameSurname)
        {
            string mail = $"Sayın {userNameSurname} merhaba,<br>" +
                $"{orderDate} tarihinde vermiş olduğunuz {orderCode} sipariş numaralı siparişiniz tamamlanmış ve kargo firmasına verilmiştir.<br>" +
                $"İyi günler.";

            await SendMailAsync(to, $"{orderCode} Sipariş Numaralı Siparişiniz Tamamlandı", mail);
        }
    }
}
