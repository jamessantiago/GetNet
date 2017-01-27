using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Responses;
using Nancy.ModelBinding;

namespace getnet.service.Api
{
    public class SetupModule : NancyModule
    {
        public SetupModule() : base("/setup")
        {
            StatelessAuthentication.Enable(this, Current.StatelessConfig);
            Before += ctx =>
            {
                return (this.Context.CurrentUser == null) ? new HtmlResponse(HttpStatusCode.Unauthorized) : null;
            };

            Post("SetKeys", args => SetKeys(this.Bind<Keys>()));
        }

        public dynamic SetKeys(Keys keys)
        {
            if (Context.CurrentUser.IsDefault())
            {
                if (CoreCurrent.Configuration.GetSecure("Api:Keys:Read").HasValue() || CoreCurrent.Configuration.GetSecure("Api:Keys:Admin").HasValue())
                {
                    return new { Status = "Error", Message = "The admin API key must be used to change keys" };
                }
                else if (!keys.AdminKey.HasValue() || !keys.ReadKey.HasValue())
                {
                    return new { Status = "Error", Message = "Keys missing" };
                } else
                {
                    CoreCurrent.Configuration.SetSecure("Api:Keys:Admin", keys.AdminKey);
                    CoreCurrent.Configuration.SetSecure("Api:Keys:Read", keys.ReadKey);
                    return new { Status = "Success", Message = "Admin and read keys set.  They now may be used for API calls to GetNet service." };
                }
            } else if (Context.CurrentUser.IsAdmin())
            {
                string message = "";
                if (keys.AdminKey.HasValue() && keys.ReadKey.HasValue())
                {
                    return new { Status = "Error", Message = "No keys specified" };
                }

                if (keys.AdminKey.HasValue())
                {
                    CoreCurrent.Configuration.SetSecure("Api:Keys:Admin", keys.AdminKey);
                    message += "Admin key set.  ";
                }
                if (keys.ReadKey.HasValue())
                {
                    CoreCurrent.Configuration.SetSecure("Api:Keys:Read", keys.ReadKey);
                    message += "Read key set.";
                }

                return new { Status = "Success", Message = message };
            }
            return new { Status = "Error", Message = "Incorrect API token privileges" };
        }
    }
}
