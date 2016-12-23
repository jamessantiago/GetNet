using System;
using getnet.core;
using System.Collections.Generic;

namespace getnet.Model
{
    public static class Roles
    {
        public const string GlobalViewers = "GlobalViewers";
        public const string GlobalAdmin = "GlobalAdmins";

        private static Dictionary<string, string> configRoles;
        public static Dictionary<string, string> ConfigRoles => configRoles ?? (configRoles = LoadConfigRoles());

        private static Dictionary<string, string> LoadConfigRoles()
        {
            var roles = new Dictionary<string, string>();
            //ldap
            roles.Add(GlobalAdmin, CoreCurrent.Configuration["Security:Ldap:Roles:" + GlobalAdmin]);
            roles.Add(GlobalViewers, CoreCurrent.Configuration["Security:Ldap:Roles" + GlobalViewers]);
            return roles;
        }
    }
}