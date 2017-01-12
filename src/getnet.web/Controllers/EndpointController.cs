using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.Model;
using getnet.core.Model;
using getnet.core.Model.Entities;

namespace getnet.Controllers
{
    public class EndpointController : BaseController
    {
        public ActionResult EndpointHandler(int? id, string searchText, jQueryDataTableParamModel param)
        {
            var predicates = PredicateBuilder.True<Device>();
            if (id.HasValue)
                predicates = predicates.And(d => d.Site.SiteId == id.Value);
            if (searchText.HasValue())
                predicates = predicates.And(Device.SearchPredicates(searchText));
            
            var devices = uow.Repo<Device>().Get(predicates, includeProperties: "Vlan,Site");

            if (param.search["value"].HasValue())
                    predicates = predicates.And(Device.SearchPredicates(param.search["value"]));
            
            var filter = param.columns[0]["search"]["value"];
            if (filter.HasValue() && filter != "none")
                foreach (var vn in ((string)filter).Split(','))
                    predicates = predicates.And(d => d.Vlan.VlanNumber == int.Parse(vn));
            else if (filter == "none")
                predicates = predicates.And(d => false);

            var queriedDevices = devices.AsQueryable().Where(predicates);

            Func<Device, string> orderingFunction = (d =>
                param.order[0]["column"] == "0" ? d.Site.Name :
                param.order[0]["column"] == "1" ? d.Vlan.VlanNumber.ToString() :
                param.order[0]["column"] == "2" ? d.Hostname :
                param.order[0]["column"] == "3" ? d.RawIP.ToString() :
                param.order[0]["column"] == "4" ? d.MAC :
                param.order[0]["column"] == "5" ? d.LastSeenOnline.ToString() : d.Hostname);

            if (param.order[0]["dir"] == "asc")
                queriedDevices = queriedDevices.OrderBy(orderingFunction).AsQueryable();
            else
                queriedDevices = queriedDevices.OrderByDescending(orderingFunction).AsQueryable();

            var takecount = param.length;
            var displayedResults = queriedDevices.Skip(param.start).Take(takecount).ToArray();

            var results = from r in displayedResults
                          select new[] {
                              r.Site.Name,
                              r.Vlan.VlanNumber.ToString(),
                              r.Hostname,
                              r.IP.ToString(),
                              r.MAC,
                              r.LastSeenOnline.ToLocalTimeString()
                          };

            return Json(new
            {
                recordsTotal = devices.Count(),
                recordsFiltered = devices.Count(),
                data = results
            });
        }

        public IActionResult SiteEndpoints(int id)
        {
            ViewData["SiteId"] = id;
            return PartialView("_siteendpoints", uow.Repo<Vlan>().Get(d => d.Site.SiteId == id));
        }

        [Route("/endpoints")]
        public IActionResult AllEndpoints()
        {
            return View();
        }

        public IActionResult AllEndpointsPartial()
        {
            return PartialView("_endpoints");
        }
    }
}
