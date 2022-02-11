using System;
using BradyTask.Areas.Identity.Data;
using BradyTask.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BradyTask.Areas.Identity.IdentityHostingStartup))]
namespace BradyTask.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<BradyTaskDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("BradyTaskDbContextConnection")));

                services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<BradyTaskDbContext>();
            });
        }
    }
}