using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Nancy.Authentication.Stateless;
using getnet.service.Api;

namespace getnet.service
{
    public static class Current
    {
        public static IScheduler Scheduler { get; internal set; }

        public static StatelessAuthenticationConfiguration StatelessConfig => new StatelessAuthenticationConfiguration(ctx => TokenValidator.GetUserFromToken(ctx.Request.Headers["PRIVATE_TOKEN"].FirstOrDefault()));
    }
}
