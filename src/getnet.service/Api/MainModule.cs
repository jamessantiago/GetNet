using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        }
    }
}
