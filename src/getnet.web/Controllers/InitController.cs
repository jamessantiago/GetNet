﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model;
using getnet.core.Model.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using getnet.Model;
using System.Threading;

namespace getnet.Controllers
{
    public class InitController : BaseController
    {

        [Route("/configure")]
        public IActionResult Index()
        {
            try {
                ViewData["DbExists"] = uow.CheckIfDabaseExists();
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
            uow = new UnitOfWork();
            return PartialView("_success", "Successfully configured the SQL connection string");
        }

        [HttpPost]
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
    }
}
