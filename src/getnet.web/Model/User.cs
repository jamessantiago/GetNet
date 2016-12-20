using System;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using getnet.Models.Security;

namespace getnet.Models
{
    public class User : ClaimsPrincipal
    {
        public override IIdentity Identity { get; }

        public string AccountName { get; private set; }
        public bool IsAnonymous { get; }

        public User(IIdentity identity)
        {
            Identity = identity;
            var i = identity as ClaimsIdentity;
            if (i == null)
            {
                IsAnonymous = true;
                return;
            }

            IsAnonymous = !i.IsAuthenticated;
            if (i.IsAuthenticated)
                AccountName = i.Name;
        }

        public override bool IsInRole(string role)
        {
            Roles r;
            return Enum.TryParse(role, out r) && getnet.Current.IsInRole(r);
        }

        private Roles? _role;
        public Roles? RawRoles => _role;

        /// <summary>
        /// Returns this user's role on the current site.
        /// </summary>
        public Roles Role
        {
            get
            {
                if (_role == null)
                {
                    if (IsAnonymous)
                    {
                        _role = Roles.Anonymous;
                    }
                    else
                    {
                        var result = Roles.Authenticated;

                        if (getnet.Current.Security.IsAdmin) result |= Roles.GlobalAdmin;

                        _role = result;
                    }
                }

                return _role.Value;
            }
        }

        public Roles GetRoles(ISecurableModule module, Roles user, Roles admin)
        {
            if (module.IsAdmin()) return admin | user;
            if (module.HasAccess()) return user;
            return Roles.None;
        }        
    }
}