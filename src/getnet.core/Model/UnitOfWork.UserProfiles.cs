using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model.Entities;

namespace getnet.core.Model
{
    public partial class UnitOfWork
    {
        //do this instead of custom repositories?
        public UserProfile GetUserProfile(string email)
        {
            var includes = "AlertRules,AlertRules.Site";
            if (!email.HasValue())
                return null;

            var profile = Repo<UserProfile>().Get(d => d.Email == email, includeProperties: includes).FirstOrDefault();
            if (profile != null)
                return profile;
            else
            {
                Repo<UserProfile>().Insert(new UserProfile()
                {
                    DisplayName = email,
                    Email = email
                });
                Save();
                return Repo<UserProfile>().Get(d => d.Email == email, includeProperties: includes).FirstOrDefault();
            }                
        }
    }
}
