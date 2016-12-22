using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using getnet.Model;
using getnet.Model.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace getnet
{
    public static partial class Current
    {
        internal static void SetDbConfigurationState()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                bool configured, tested;

                Exception testException = null;

                tested = uow.TestDatabaseConnection(out testException);
                configured = uow.ConfigurationState == UnitOfWork.DatabaseConfigurationState.Configured;
                tested = uow.TestDatabaseConnection(out testException);

                ConfigurationRequired = !configured;
                DatabaseConnectionError = testException;
            }
        }

        public static bool ConfigurationRequired { get; private set; }

        public static Exception DatabaseConnectionError { get; private set; }
        

        public static IServiceProvider Services { get; internal set; }

        public static HttpContext Context => Services.GetRequiredService<IHttpContextAccessor>().HttpContext;
        
        public static string RequestIP => Context.Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

        public static SecurityProvider Security => Services.GetRequiredService<SecurityProvider>();

        public static User User => new User(Context.User);

        private static UnitOfWork unitOfWork;
        public static UnitOfWork uow => unitOfWork ?? (unitOfWork = new UnitOfWork());
    }
}
