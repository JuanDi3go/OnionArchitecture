
using Application.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class ServicExtensions
    {
        public static void AddSharedInfraEstructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService,DateTimeService>();
        }
    }
}
