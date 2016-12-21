using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using getnet.Models;
using getnet.Model.Security;
using Microsoft.AspNetCore.Http;

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

        private static SecurityProvider _security;
        public static SecurityProvider Security => _security ?? (_security = GetSecurityProvider());

        private static SecurityProvider GetSecurityProvider()
        {
            if (!CoreCurrent.Configuration["Security:Provider"].HasValue())
                return new EveryonesReadOnlyProvider();
            switch (CoreCurrent.Configuration["Security:Provider"].ToLowerInvariant())
            {
                case "activedirectory":
                case "ad":
                    return new ActiveDirectoryProvider();
                case "alladmin":
                    return new EveryonesAnAdminProvider();
                //case "allview":
                default:
                    return new EveryonesReadOnlyProvider();
            }
        }

        public static IServiceProvider Services { get; internal set; }

        public static HttpContext Context => Services.GetRequiredService<HttpContextAccessor>().HttpContext;

        public static User User => Context.User as User;

        public static bool IsInRole(Roles roles)
        {
            if (User == null)
            {
                return RequestRoles.HasFlag(roles);
            }
            return ((User.Role | RequestRoles) & roles) != Roles.None || User.Role.HasFlag(Roles.GlobalAdmin);
        }

        public static Roles RequestRoles
        {
            get
            {
                var roles = Context.Items[nameof(RequestRoles)];
                if (roles != null) return (Roles)roles;

                var result = Roles.None;
                if (Context.Request.IsLocal()) result |= Roles.LocalRequest;
                if (Security.IsInternalIP(RequestIP)) result |= Roles.InternalRequest;

                Context.Items[nameof(RequestRoles)] = result;
                return result;
            }
        }

        public static string RequestIP => Context.Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
    }
}
