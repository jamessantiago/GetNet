using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Nancy.Authentication.Stateless;

namespace getnet.service
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            if (!CoreCurrent.Configuration.GetSecure("Api:Keys:Default").HasValue())
            {
                Console.WriteLine("Created new default API key");
                CoreCurrent.Configuration.SetSecure("Api:Keys:Default", Convert.ToBase64String(Guid.NewGuid().ToByteArray()));
            }
            app.UseOwin(x => x.UseNancy());
        }
    }
}
