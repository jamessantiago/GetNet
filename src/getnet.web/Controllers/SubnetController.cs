using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model.Entities;
using getnet.Helpers;
using getnet.Model;
using Microsoft.AspNetCore.Authorization;

namespace getnet.Controllers
{
    [RedirectOnDbIssue]
    [Authorize(Roles=Roles.GlobalViewers)]
    public class SubnetController : BaseController
    {
        public ActionResult SubnetHandler(int? siteid, string searchText, jQueryDataTableParamModel param)
        {
            var predicates = PredicateBuilder.True<Subnet>();
            if (siteid.HasValue)
                predicates = predicates.And(d => d.Site.SiteId == siteid.Value);
            if (searchText.HasValue())
                predicates = predicates.And(Subnet.SearchPredicates(searchText));

            var devices = uow.Repo<Subnet>().Get(predicates, includeProperties: "Site");

            if (param.search["value"].HasValue())
                predicates = predicates.And(Subnet.SearchPredicates(param.search["value"]));

            var queriedDevices = devices.AsQueryable().Where(predicates);

            Func<Subnet, string> orderingFunction = (d =>
                param.order[0]["column"] == "0" ? d.Site.Name :
                param.order[0]["column"] == "1" ? d.Type.ToString() :
                param.order[0]["column"] == "2" ? d.RawSubnetIP.ToString() : d.Site.Name);

            if (param.order[0]["dir"] == "asc")
                queriedDevices = queriedDevices.OrderBy(orderingFunction).AsQueryable();
            else
                queriedDevices = queriedDevices.OrderByDescending(orderingFunction).AsQueryable();

            var takecount = param.length;
            var displayedResults = queriedDevices.Skip(param.start).Take(takecount).ToArray();

            var results = from r in displayedResults
                          select new[] {
                              r.Site.Name,
                              r.Type.ToString(),
                              r.IPNetwork.Network.ToString() + "/" + r.IPNetwork.Cidr.ToString()
                          };

            return Json(new
            {
                recordsTotal = devices.Count(),
                recordsFiltered = devices.Count(),
                data = results
            });
        }

        public IActionResult SiteSubnets(int id)
        {
            ViewData["SiteId"] = id;
            return PartialView("_sitesubnets");
        }

        [Route("/subnets")]
        public IActionResult AllSubnets()
        {
            return View();
        }

        public IActionResult AllSubnetsPartial()
        {
            return PartialView("_subnets");
        }
    }
}
