using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model.Entities;
using getnet.Helpers;
using getnet.Model;

namespace getnet.Controllers
{
    public class NetworkDeviceController : BaseController
    {
        public ActionResult NetworkDeviceHandler(int? id, string searchText, jQueryDataTableParamModel param)
        {
            var predicates = PredicateBuilder.True<NetworkDevice>();
            if (id.HasValue)
                predicates = predicates.And(d => d.Site.SiteId == id.Value);
            if (searchText.HasValue())
                predicates = predicates.And(NetworkDevice.SearchPredicates(searchText));

            var devices = uow.Repo<NetworkDevice>().Get(predicates, includeProperties: "Site");

            if (param.search["value"].HasValue())
                predicates = predicates.And(NetworkDevice.SearchPredicates(param.search["value"]));
            
            var queriedDevices = devices.AsQueryable().Where(predicates);

            Func<NetworkDevice, string> orderingFunction = (d =>
                param.order[0]["column"] == "0" ? d.Site.Name :
                param.order[0]["column"] == "1" ? d.Capabilities.ToString() :
                param.order[0]["column"] == "2" ? d.Hostname :
                param.order[0]["column"] == "3" ? d.Model :
                param.order[0]["column"] == "4" ? d.ChassisSerial :
                param.order[0]["column"] == "5" ? d.RawManagementIP.ToString() : d.Hostname);

            if (param.order[0]["dir"] == "asc")
                queriedDevices = queriedDevices.OrderBy(orderingFunction).AsQueryable();
            else
                queriedDevices = queriedDevices.OrderByDescending(orderingFunction).AsQueryable();

            var takecount = param.length;
            var displayedResults = queriedDevices.Skip(param.start).Take(takecount).ToArray();

            var results = from r in displayedResults
                          select new[] {
                              r.Site.Name,
                              r.Capabilities.ToString(),
                              r.Hostname,
                              r.Model,
                              r.ChassisSerial,
                              r.ManagementIP.ToString()
                          };

            return Json(new
            {
                recordsTotal = devices.Count(),
                recordsFiltered = devices.Count(),
                data = results
            });
        }

        public IActionResult SiteNetworkDevices(int id)
        {
            ViewData["SiteId"] = id;
            return PartialView("_sitenetworkdevices", uow.Repo<Vlan>().Get(d => d.Site.SiteId == id));
        }

        [Route("/networkdevices")]
        public IActionResult AllNetworkDevices()
        {
            return View();
        }

        public IActionResult AllNetworkDevicesPartial()
        {
            return PartialView("_networkdevices");
        }
    }
}
