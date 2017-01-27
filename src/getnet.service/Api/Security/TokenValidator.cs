using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace getnet.service.Api
{
    public static class TokenValidator
    {
        public static ClaimsPrincipal GetUserFromToken(string apikey)
        {
            if (!apikey.HasValue())
                return null;

            switch (GetTokenType(apikey))
            {
                case TokenType.Default:
                    return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("ApiRole", "Default") }, "API"));
                case TokenType.Read:
                    return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("ApiRole", "Read") }, "API"));
                case TokenType.Admin:
                    return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("ApiRole", "Admin") }, "API"));
                case TokenType.None:
                default:
                    return null;
            }
        }

        private static TokenType GetTokenType(string apikey)
        {
            if (CoreCurrent.Configuration.GetSecure("Api:Keys:Default") == apikey)
                return TokenType.Default;
            else if (CoreCurrent.Configuration.GetSecure("Api:Keys:Read") == apikey)
                return TokenType.Read;
            else if (CoreCurrent.Configuration.GetSecure("Api:Keys:Admin") == apikey)
                return TokenType.Admin;
            else
                return TokenType.None;
        }

        public static bool IsDefault(this ClaimsPrincipal user)
        {
            return user != null ? user.Claims.Any(d => d.Type == "ApiRole" && d.Value == "Default") : false;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user != null ? user.Claims.Any(d => d.Type == "ApiRole" && d.Value == "Admin") : false;
        }

        public static bool IsReader(this ClaimsPrincipal user)
        {
            return user != null ? user.Claims.Any(d => d.Type == "ApiRole" && d.Value == "Read") : false;
        }
    }

    public enum TokenType
    {
        None,
        Default,
        Read,
        Admin
    }
}
