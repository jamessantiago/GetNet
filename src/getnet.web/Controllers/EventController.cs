using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.Model;
using getnet.core.Model;
using getnet.core.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using getnet.Helpers;
using Microsoft.AspNetCore.Http;

namespace getnet.Controllers
{
    [RedirectOnDbIssue]
    [Authorize(Roles = Roles.GlobalViewers)]
    public class EventController : BaseController
    {
        public ActionResult EventHandler(int? siteid, string text, jQueryDataTableParamModel param)
        {
            var predicates = PredicateBuilder.True<Event>();
            if (siteid.HasValue)
                predicates = predicates.And(d => d.SiteId == siteid.Value);
            if (text.HasValue())
                predicates = predicates.And(Event.SearchPredicates(text));

            var events = uow.Repo<Event>().Get(predicates);

            if (param.search["value"].HasValue())
                predicates = predicates.And(Event.SearchPredicates(param.search["value"]));
            
            var queriedEvents = events.AsQueryable().Where(predicates);

            Func<Event, string> orderingFunction = (d =>
                param.order[0]["column"] == "0" ? d.TimeStamp.ToLocalTimeString("s") : 
                param.order[0]["column"] == "1" ? d.Level :
                param.order[0]["column"] == "2" ? d.Type :
                param.order[0]["column"] == "3" ? d.Source :
                param.order[0]["column"] == "4" ? d.Message : d.TimeStamp.ToLocalTimeString("s"));

            if (param.order[0]["dir"] == "asc")
                queriedEvents = queriedEvents.OrderBy(orderingFunction).AsQueryable();
            else
                queriedEvents = queriedEvents.OrderByDescending(orderingFunction).AsQueryable();

            var takecount = param.length;
            var displayedResults = queriedEvents.Skip(param.start).Take(takecount).ToArray();

            var results = from r in displayedResults
                          select new[] {
                              r.TimeStamp.ToLocalTimeString(),
                              r.Level,
                              r.Type,
                              r.Source,
                              r.Message,
                              r.EventId.ToString()
                          };
            
            return Json(new
            {
                recordsTotal = events.Count(),
                recordsFiltered = events.Count(),
                data = results
            });
        }

        public IActionResult SiteEvents(int id)
        {
            ViewData["SiteId"] = id;
            return PartialView("_events");
        }

        [Route("/events")]
        public IActionResult AllEvents()
        {
            return View();
        }

        public IActionResult AllEventsPartial()
        {
            return PartialView("_events");
        }

        public IActionResult EventsPartial(string id)
        {
            ViewData["SearchText"] = id;
            return PartialView("_events");
        }

        public IActionResult Details(int id)
        {
            var ev = uow.Repo<Event>().GetByID(id);
            if (!ev.SiteId.HasValue || ev.SiteId.Value == 0) return PartialView("_details", ev);
            var site = uow.Repo<Site>().GetByID(ev.SiteId.Value);
            if (site != null)
                ViewData["Site"] = site.Name;
            return PartialView("_details", ev);
        }
    }
}
