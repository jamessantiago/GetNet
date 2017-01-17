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

namespace getnet.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Roles = Roles.GlobalAdmins)]
    public class InitController : BaseController
    {

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
            CoreCurrent.Configuration.Set("Security:Provider", collection["AuthChoice"]);
            if (collection["AuthChoice"] == "ldap")
            {
                CoreCurrent.Configuration.Set("Security:Ldap:Host", collection["Host"]);
                CoreCurrent.Configuration.SetSecure("Security:Ldap:LoginDN", collection["LoginDN"]);
                if (collection["Password"].Any())
                    CoreCurrent.Configuration.SetSecure("Security:Ldap:Password", collection["Password"]);
                CoreCurrent.Configuration.Set("Security:Ldap:Roles:GlobalAdmins", collection["GlobalAdmins"]);
                CoreCurrent.Configuration.Set("Security:Ldap:Roles:GlobalViewers", collection["GlobalViewers"]);
            }
            HttpContext.Session.AddSnackMessage("GetNet will reinitialize in roughly 1 second.  Existing sessions will need to be logged out of or wait until the session expires in 5 days.");
            Current.AppCancellationSource.CancelAfter(1000);
            return PartialView("_success", "Successfully configured authentication");
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

        [AllowAnonymous]
        [Route("/dbissue")]
        public IActionResult Anon()
        {
            return View();
        }
    }
}
