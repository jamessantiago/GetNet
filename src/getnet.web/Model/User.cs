using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using getnet.Model.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using getnet.core.Model.Entities;
using System.Linq;

namespace getnet.Model
{
    public class User : ClaimsPrincipal
    {

        public string AccountName { get; }
        public bool IsAnonymous { get; }
        public bool IsAdmin => this.Identity.IsAuthenticated && this.InRoles(Roles.GlobalAdmins);
        public UserProfile UserProfile => getnet.Current.uow.GetUserProfile(AccountName);
        public List<string> UserRoles => Claims.Where(d => d.Type == ClaimTypes.Role).Select(d => d.Value).ToList();

        public User(ClaimsPrincipal principal)
        {
            AccountName = principal.Identity.Name;
            this.AddIdentity(new Identity((ClaimsIdentity)principal.Identity));
        }

        public User(string email, params string[] roles)
        {
            AccountName = email;
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, AccountName));
            claims.Add(new Claim(ClaimTypes.Email, AccountName));
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            this.AddIdentity(new Identity(AccountName, claims));
        }

        public bool InRoles(string roles)
        {
            foreach (var role in roles.Split(','))
            {
                if (UserRoles.Any(d => d.ToLower() == role.Trim().ToLower()))
                    return true;
            }
            return false;
        }
    }

    public class Identity : ClaimsIdentity
    {
        public Identity(ClaimsIdentity identity)
        {
            Name = identity.Name;
            Claims = identity.Claims;
        }

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