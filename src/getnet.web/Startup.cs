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
using getnet.Model.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace getnet
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Current.SetDbConfigurationState();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //CoreCurrent.Configuration.SetSecure("Security:Provider", "ldap");
            //CoreCurrent.Configuration.Set("Security:Ldap:Host", "192.168.157.131");
            //CoreCurrent.Configuration.SetSecure("Security:Ldap:LoginDN", "CN=ldapuser,CN=Users,DC=getnet,DC=local");
            //CoreCurrent.Configuration.SetSecure("Security:Ldap:Password", "TestPassword123");
            //CoreCurrent.Configuration.Set("Security:Ldap:Roles:" + Roles.GlobalAdmins, "Domain Users");
            //CoreCurrent.Configuration.SetSecure("SSH:Username", "admin");
            //CoreCurrent.Configuration.SetSecure("SSH:Password", "password");
            switch (CoreCurrent.Configuration.GetSecure("Security:Provider"))
            {
                case "ldap":
                    services.AddIdentity<User, Role>()
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
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(150);
                options.Cookies.ApplicationCookie.LoginPath = "/login";
                options.Cookies.ApplicationCookie.LogoutPath = "/logoff";
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
