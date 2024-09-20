using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ServiceRegistration));
            //AddMediatR Assembly'deki tüm handler sınıflarını bulacak ve IoC'ye ekleyecek. Ondan dolayı içine ServiceRegistration'ı verdik. Bu Assembly'de bulunan herhangi bir şeyi verebilirdik.
            services.AddHttpClient();
        }
    }
}
