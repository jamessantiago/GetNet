using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Responses;

namespace getnet.service.Api
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            StatelessAuthentication.Enable(this, Current.StatelessConfig);
            Before += ctx => {
                return (this.Context.CurrentUser == null) ? new HtmlResponse(HttpStatusCode.Unauthorized) : null;
            };
            Get("/", args => "GetNet Service");
            Post("/restart", args => Restart());
        }

        public dynamic Restart()
        {
            if (Context.CurrentUser.IsAdmin())
            {
                Runner.Restart();
                return new { Status = "Success", Message = "GetNet Service Restarted" };
            }
            else
                return new { Status = "Error", Message = "Incorrect API token privileges" };
        }
    }
}
