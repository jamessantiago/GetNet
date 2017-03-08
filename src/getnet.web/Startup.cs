using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using getnet.core.Model;
using Microsoft.AspNetCore.Mvc;
using getnet.Model;
using System.IO;
using getnet.Model.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

namespace getnet
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string pathToCryptoKeys = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                    + "dp_keys" + Path.DirectorySeparatorChar;
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(pathToCryptoKeys))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(3650))
                .SetApplicationName("getnet");
            
            switch (CoreCurrent.Configuration["Security:Provider"])
            {
                case "ldap":
                    services.AddIdentity<User, Role>(options =>
                        {
                            options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(7);
                            options.Cookies.ApplicationCookie.LoginPath = "/login";
                            options.Cookies.ApplicationCookie.LogoutPath = "/logoff";
                            options.Cookies.ApplicationCookie.SlidingExpiration = true;
                        })
                        .AddClaimsPrincipalFactory<ActiveDirectoryProvider>()
                        .AddUserStore<ActiveDirectoryProvider>()
                        .AddRoleStore<ActiveDirectoryProvider>();
                    services.AddSingleton<SecurityProvider, ActiveDirectoryProvider>();
                    break;
                case "view":
                    services.AddIdentity<User, Role>()
                        .AddClaimsPrincipalFactory<EveryonesReadOnlyProvider>()
                        .AddUserStore<EveryonesReadOnlyProvider>()
                        .AddRoleStore<EveryonesReadOnlyProvider>();
                    services.AddSingleton<SecurityProvider, EveryonesReadOnlyProvider>();
                    break;
                case "admin":
                default:
                    services.AddIdentity<User, Role>()
                        .AddClaimsPrincipalFactory<EveryonesAnAdminProvider>()
                        .AddUserStore<EveryonesAnAdminProvider>()
                        .AddRoleStore<EveryonesAnAdminProvider>();
                    services.AddSingleton<SecurityProvider, EveryonesAnAdminProvider>();
                    break;
            }
            services.AddMvc();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookies.ApplicationCookie.LoginPath = "/login";
                options.Cookies.ApplicationCookie.LogoutPath = "/logoff";
                options.Cookies.ApplicationCookie.SlidingExpiration = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(CoreCurrent.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentity();
            app.UseSession();
            app.UseCookieAuthentication();
            

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }

            app.UseExceptionHandler("/error");
            
            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=a}/{action=Index}/{id?}");
            });
        }
    }
}
