using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model;
using getnet.core.Model.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using getnet.Model;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace getnet.Controllers
{
    [RequireHttps]
    [Authorize(Roles = Roles.GlobalAdmins)]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class InitController : BaseController
    {
        Whistler logger = new Whistler();

        [Route("/configure")]
        public IActionResult Index()
        {
            try {
                ViewData["DbExists"] = uow.CheckIfDabaseExists();
                Current.SetDbConfigurationState();
            } catch
            {
                ViewData["DbExists"] = false;
            }
            ViewData["DbError"] = Current.DatabaseConnectionError;
            ViewData["MSSQLString"] = CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString");
            ViewData["PostgresString"] = CoreCurrent.Configuration.GetSecure("Data:PostgresConnectionString");
            ViewData["DbChoice"] = CoreCurrent.Configuration["Data:DataStore"] == "Postgres" ? "UsePostgres" : "UseMSSQL";

            return View();
        }

        [HttpPost]
        public IActionResult Configure(string SqlConfigurationString, string PostgresConfigurationString, string DbChoice)
        {
            if (SqlConfigurationString.HasValue())
                CoreCurrent.Configuration.SetSecure("Data:SqlServerConnectionString", SqlConfigurationString);
            if (PostgresConfigurationString.HasValue())
                CoreCurrent.Configuration.SetSecure("Data:PostgresConnectionString", PostgresConfigurationString);
            if (DbChoice == "UseMSSQL")
                CoreCurrent.Configuration.Set("Data:DataStore", "MSSQL");
            else
                CoreCurrent.Configuration.Set("Data:DataStore", "Postgres");
            Current.SetDbConfigurationState();
            _uow = new UnitOfWork();
            return PartialView("_success", "Successfully configured the SQL connection string");
        }
        
        public IActionResult CreateDb()
        {
            try
            {
                Exception e;
                uow.TestDatabaseConnection(out e);
                uow.EnsureDatabaseExists();
                Current.SetDbConfigurationState();
                HttpContext.Session.AddSnackMessage("Successfully ensured the creation of the GetNet database");
                return RedirectToAction("index", "a");
            } catch (Exception ex)
            {
                HttpContext.Session.AddSnackMessage("Failed to ensure database creation: {0}", ex.Message);
                return RedirectToAction("index", "init");
            }
        }

        [HttpPost]
        public IActionResult AuthConfig(IFormCollection collection)
        {
            var oldAuth = CoreCurrent.Configuration["Security:Provider"];
            CoreCurrent.Configuration.Set("Security:Provider", collection["AuthChoice"]);
            bool success = true;
            if (collection["AuthChoice"] == "ldap")
            {
                var oldHost = CoreCurrent.Configuration["Security:Ldap:Host"];
                var oldLoginDn = CoreCurrent.Configuration.GetSecure("Security:Ldap:LoginDN");
                var oldPass = CoreCurrent.Configuration.GetSecure("Security:Ldap:Password");
                var oldAdmins = CoreCurrent.Configuration["Security:Ldap:GlobalAdmins"];
                var oldUsers = CoreCurrent.Configuration["Security:Ldap:GlobalViewers"];

                try
                {
                    CoreCurrent.Configuration.Set("Security:Ldap:Host", collection["Host"]);
                    CoreCurrent.Configuration.SetSecure("Security:Ldap:LoginDN", collection["LoginDN"]);
                    if (collection["Password"].Any())
                        CoreCurrent.Configuration.SetSecure("Security:Ldap:Password", collection["Password"]);
                    CoreCurrent.Configuration.Set("Security:Ldap:Roles:GlobalAdmins", collection["GlobalAdmins"]);
                    CoreCurrent.Configuration.Set("Security:Ldap:Roles:GlobalViewers", collection["GlobalViewers"]);
                    getnet.Model.Security.LdapServer.Current.EnsureBind(true);
                    HttpContext.Session.AddSnackMessage("GetNet will reinitialize in roughly 1 second.  Existing sessions will need to be logged out of or wait until the session expires in 5 days.");
                    Current.AppCancellationSource.CancelAfter(1000);
                } catch (Exception ex)
                {
                    success = false;
                    HttpContext.Session.AddSnackMessage("Ldap configuration failed (changes reverted): " + ex.Message);
                    logger.Error(ex, WhistlerTypes.Configuration);
                    CoreCurrent.Configuration.Set("Security:Ldap:Host", oldHost);
                    CoreCurrent.Configuration.SetSecure("Security:Ldap:LoginDN", oldLoginDn);
                    CoreCurrent.Configuration.SetSecure("Security:Ldap:Password", oldPass);
                    CoreCurrent.Configuration.Set("Security:Ldap:Roles:GlobalAdmins", oldAdmins);
                    CoreCurrent.Configuration.Set("Security:Ldap:Roles:GlobalViewers", oldUsers);
                    CoreCurrent.Configuration.Set("Security:Provider", oldAuth);
                }
            }
            
            if (success)
                return PartialView("_success", "Successfully configured authentication");
            else
                return PartialView("_error", "Configuration failed");
        }

        [HttpPost]
        public IActionResult SshConfig(IFormCollection collection)
        {
            CoreCurrent.Configuration.SetSecure("SSH:Username", collection["username"]);
            CoreCurrent.Configuration.SetSecure("SSH:Password", collection["password"]);
            CoreCurrent.Configuration.Set("SSH:Port", collection["port"]);
            return PartialView("_success", "Successfully configured ssh");
        }

        [HttpPost]
        public IActionResult LogConfig(IFormCollection collection)
        {
            CoreCurrent.Configuration.Set("Whistler:Console:Enabled", collection["consoleenabled"] == "on" ? "true" : "false");
            CoreCurrent.Configuration.Set("Whistler:Console:Layout", collection["consolelayout"]);
            CoreCurrent.Configuration.Set("Whistler:File:Enabled", collection["fileenabled"] == "on" ? "true" : "false");
            CoreCurrent.Configuration.Set("Whistler:File:Layout", collection["filelayout"]);
            CoreCurrent.Configuration.Set("Whistler:File:FileName", collection["filename"]);
            CoreCurrent.Configuration.Set("Whistler:Smtp:Enabled", collection["smtpenabled"] == "on" ? "true" : "false");
            CoreCurrent.Configuration.Set("Whistler:Smtp:Server", collection["smtpserver"]);
            CoreCurrent.Configuration.Set("Whistler:Smtp:From", collection["smtpfrom"]);
            CoreCurrent.Configuration.Set("Whistler:Smtp:SubjectLayout", collection["smtpsubject"]);
            CoreCurrent.Configuration.Set("Whistler:Db:Enabled", collection["databaseenabled"] == "on" ? "true" : "false");
            return PartialView("_success", "Successfully configured logging");
        }

        private string GetLocations()
        {
            var locs = uow.Repo<Location>().Get().ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var loc in locs.OrderBy(d => d.Name))
            {
                sb.AppendLine(loc.Name);
            }
            return sb.ToString();
        }

        public IActionResult Fields()
        {
            ViewData["Locations"] = GetLocations();
            return View();
        }

        [HttpPost]
        public IActionResult AddLocations(string fields)
        {
            var repo = uow.Repo<Location>();
            int count = 0;
            int removed = 0;
            var newfields = fields.Split(Environment.NewLine.ToCharArray());
            foreach (var field in newfields)
            {
                var thisfield = field.Trim();
                if (!thisfield.HasValue() || repo.Get(d => d.Name == thisfield).Any())
                    continue;

                repo.Insert(new Location { Name = thisfield });
                count++;
            }
            foreach (var field in repo.Get())
            {
                if (!newfields.Any(d => d == field.Name.Trim()))
                {
                    repo.Delete(field);
                    removed++;
                }
            }
            uow.Save();
            HttpContext.Session.AddSnackMessage("Added {0} new locations and removed {1}", count, removed);
            ViewData["Locations"] = GetLocations();
            return RedirectToAction("Fields");
        }

    }
}
