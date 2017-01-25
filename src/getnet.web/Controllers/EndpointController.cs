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
    public class EndpointController : BaseController
    {
        private Whistler logger = new Whistler(typeof(EndpointController).FullName);

        [RedirectOnDbIssue]
        [Authorize(Roles =Roles.GlobalViewers)]
        public ActionResult EndpointHandler(int? siteid, string text, jQueryDataTableParamModel param)
        {
            var predicates = PredicateBuilder.True<Device>();
            if (siteid.HasValue)
                predicates = predicates.And(d => d.Site.SiteId == siteid.Value);
            if (text.HasValue())
                predicates = predicates.And(Device.SearchPredicates(text));
            
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

            if (siteid.HasValue)
                results = results.Select(d => d.SubArray(1, 5));

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
            ViewData["Vlans"] = uow.Repo<Vlan>().Get(d => d.Site.SiteId == id);
            return PartialView("_endpoints");
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

        public IActionResult EndpointsPartial(string id)
        {
            ViewData["SearchText"] = id;
            return PartialView("_endpoints");
        }

        [Route("/e/{id}")]
        public IActionResult Details(string id)
        {
            int devid = 0;
            Device device = null;
            if (int.TryParse(id, out devid))
                device = uow.Repo<Device>().Get(d => d.DeviceId == devid,
                    includeProperties: "Vlan,NetworkDevice,Site,Vlan.NetworkDevice,DeviceHistories").FirstOrDefault();
            else
                device = uow.Repo<Device>().Get(d => d.MAC.ToLower() == id.ToLower(),
                    includeProperties: "Vlan,NetworkDevice,Site,Vlan.NetworkDevice,DeviceHistories").FirstOrDefault();

            return View(device);
        }

        [HttpPost]
        public void Reserve(IFormCollection collection)
        {
            long ip = 0;
            if (long.TryParse(collection["dialog-ip"].FirstOrDefault(), out ip))
            {
                try
                {
                    uow.Transaction(() =>
                    {
                        var thisVlan = uow.Repo<Vlan>().Get(d => d.VlanId == int.Parse(collection["vlanid"].FirstOrDefault()), includeProperties: "Devices,Site").FirstOrDefault();
                        var thisSite = uow.Repo<Site>().Get(d => d.SiteId == thisVlan.Site.SiteId, includeProperties: "Devices").FirstOrDefault();
                        var newdevice = new Device
                        {
                            Type = DeviceType.Reservation,
                            RawIP = ip,
                            MAC = "IPR" + ip.ToString(),
                            Site = thisSite
                        };
                        if (collection["ReservationComment"].FirstOrDefault() != null)
                            newdevice.ReservationComment = collection["ReservationComment"].FirstOrDefault();
                        uow.Repo<Device>().Insert(newdevice);
                        uow.Save();
                        thisVlan.Devices.AddOrNew(newdevice);
                        thisSite.Devices.AddOrNew(newdevice);
                        uow.Save();
                        HttpContext.Session.AddSnackMessage("Successfully added reservation.  Refresh to view updated listing.");
                    });
                }catch (Exception ex)
                {
                    logger.Error(ex, WhistlerTypes.UnhandledException);
                    HttpContext.Session.AddSnackMessage("Failed to add reservation: " + ex.Message);
                }
            } else
            {
                HttpContext.Session.AddSnackMessage("Failed to reserve ip, missing ip from dialog");
            }
        }
    }
}
