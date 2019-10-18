using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Routes.Models;

[assembly: HostingStartup(typeof(Routes.Areas.Identity.IdentityHostingStartup))]
namespace Routes.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<RouteContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("RoutesContextConnection")));

                /*services.AddDefaultIdentity<User>()
                        .AddEntityFrameworkStores<RouteContext>()
                        .AddDefaultUI();*/
            });
        }
    }
}