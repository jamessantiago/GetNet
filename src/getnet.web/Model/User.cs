using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using getnet.Model.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using getnet.core.Model.Entities;

namespace getnet.Model
{
    public class User : IdentityUser
    {

        public string AccountName { get; }
        public bool IsAnonymous { get; }
        public UserProfile UserProfile { get; }
        //public List<string> Roles { get; }

        public User(string email, UserProfile profile)
        {
            AccountName = email;
            UserProfile = profile;
        }

        public ClaimsPrincipal CreatePrincipal()
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, AccountName));
            claims.Add(new Claim(ClaimTypes.Email, AccountName));
            var principal = new ClaimsPrincipal(new Identity(AccountName, claims));
            return principal;
        }

        public bool IsInRole(string role)
        {
            return true;
            //Roles r;
            //return Enum.TryParse(role, out r) && getnet.Current.IsInRole(r);
        }

        private Roles? _role;
        public Roles? RawRoles => _role;

        /// <summary>
        /// Returns this user's role on the current site.
        /// </summary>
        //public Roles Role
        //{
        //    get
        //    {
        //        if (_role == null)
        //        {
        //            if (IsAnonymous)
        //            {
        //                _role = Roles.Anonymous;
        //            }
        //            else
        //            {
        //                var result = Roles.Authenticated;

        //                //if (getnet.Current.Security.IsAdmin) result |= Roles.GlobalAdmin;

        //                _role = result;
        //            }
        //        }

        //        return _role.Value;
        //    }
        //}

        //public Roles GetRoles(ISecurableModule module, Roles user, Roles admin)
        //{
        //    //if (module.IsAdmin()) return admin | user;
        //    //if (module.HasAccess()) return user;
        //    return Roles.None;
        //}        
    }

    public class Identity : ClaimsIdentity
    {
        public Identity(string name, IEnumerable<Claim> claims)
        {
            Name = name;
            Claims = claims;
        }

        public override string AuthenticationType => "GetNet";

        public override bool IsAuthenticated => true;

        public override string Name { get; }

        public override IEnumerable<Claim> Claims { get; }
    }
}