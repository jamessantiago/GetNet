using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using getnet.core.Model.Entities;
using getnet.core.Model;

namespace getnet
{
    public static class ContextAlerts
    {
        public static List<SnackMessage> GetRefererSnacks(this HttpRequest request, RouteData routeData)
        {
            var snacks = new List<SnackMessage>();
            var referer = request.Headers["Referer"].FirstOrDefault();
            if (!referer.HasValue())
                return snacks;

            if (Current.DatabaseConnectionError != null)
                return snacks;

            var route = routeData.Routers.OfType<Route>().FirstOrDefault(p => p.Name == "default");
            var template = route.ParsedTemplate;
            var matcher = new TemplateMatcher(template, route.Defaults);
            var routeValues = new RouteValueDictionary();
            var localPath = (new Uri(referer)).LocalPath;
            if (matcher.TryMatch(localPath, routeValues))
            {
                if (routeValues["controller"] as string == "s"
                    && (routeValues["action"] as string).HasValue())
                {
                    using (UnitOfWork uow = new UnitOfWork())
                    {
                        int siteId;
                        Site site = null;
                        if (int.TryParse(routeValues["action"] as string, out siteId))
                            site = uow.Repo<Site>().Get(filter: d => d.SiteId == siteId).FirstOrDefault();
                        else
                            site = uow.Repo<Site>().Get(filter: d => d.Name == routeValues["action"] as string).FirstOrDefault();

                        if (site != null)
                            foreach (var ev in uow.Repo<Event>().Get(d => d.TimeStamp > DateTime.UtcNow.AddSeconds(-15) && d.SiteId == site.SiteId))
                                snacks.Add(new SnackMessage
                                {
                                    message = ev.Message
                                });
                    }
                }
            }

            if (snacks.Count > 1)
                for (int i = 0; i < snacks.Count; i++)
                    snacks[i].timeout = 10000 / snacks.Count;

            return snacks;
        }
    }
}
