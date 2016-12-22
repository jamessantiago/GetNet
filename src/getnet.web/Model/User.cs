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
    public class User : ClaimsPrincipal
    {

        public string AccountName { get; }
        public bool IsAnonymous { get; }
        public UserProfile UserProfile => profile ?? (profile = getnet.Current.uow.GetUserProfile(AccountName));
        private UserProfile profile;
        public List<Role> UserRoles { get; set; }

        public User(ClaimsPrincipal principal)
        {
            AccountName = principal.Identity.Name;
            this.AddIdentity(new Identity((ClaimsIdentity)principal.Identity));
        }

        public User(string email, UserProfile profile)
        {
            AccountName = email;
            this.profile = profile;
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, AccountName));
            claims.Add(new Claim(ClaimTypes.Email, AccountName));
            this.AddIdentity(new Identity(AccountName, claims));
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