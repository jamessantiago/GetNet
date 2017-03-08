using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model.Entities;
using getnet.Model.Security;

namespace getnet.core.Model
{
    public partial class UnitOfWork
    {
        //do this instead of custom repositories?
        public UserProfile GetUserProfile(string username)
        {
            const string includes = "AlertRules,AlertRules.Site";
            if (!username.HasValue())
                return null;

            var profile = Repo<UserProfile>().Get(d => d.Username == username, includeProperties: includes).FirstOrDefault();
            if (profile != null)
                return profile;
            else
            {
                var user = Retry.Do(() => LdapServer.Current.FindUser(username), TimeSpan.FromSeconds(1));
                Repo<UserProfile>().Insert(new UserProfile()
                {
                    Username = username.ToLower(),
                    DisplayName = user.getAttribute("displayName").StringValue,
                    Email = user.getAttribute("mail").StringValue
                });
                Save();
                return Repo<UserProfile>().Get(d => d.Username == username, includeProperties: includes).FirstOrDefault();
            }                
        }
    }
}

