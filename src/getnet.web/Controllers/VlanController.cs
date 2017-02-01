using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model.Entities;
using getnet.Helpers;
using getnet.Model;
using System.Net;

namespace getnet.Controllers
{
    public class VlanController : BaseController
    {
        public IActionResult VlanUsage(int id)
        {
            var vlan = uow.Repo<Vlan>().Get(d => d.VlanId == id, includeProperties: "Devices,Site,Site.NetworkDevices").FirstOrDefault();
            return View(vlan);
        }

        //public ActionResult VlanHandler(int? id, string searchText, jQueryDataTableParamModel param)
        //{
        //    var predicates = PredicateBuilder.True<Vlan>();
        //    if (id.HasValue)
        //        predicates = predicates.And(d => d.Site.SiteId == id.Value);
        //    if (searchText.HasValue())
        //        predicates = predicates.And(Vlan.SearchPredicates(searchText));

        //    var devices = uow.Repo<Vlan>().Get(predicates);

        //    if (param.search["value"].HasValue())
        //        predicates = predicates.And(Vlan.SearchPredicates(param.search["value"]));

        //    var queriedDevices = devices.AsQueryable().Where(predicates);

        //    Func<Vlan, string> orderingFunction = (d =>
        //        param.order[0]["column"] == "0" ? d.Site.Name :
        //        param.order[0]["column"] == "1" ? d.VlanNumber.ToString() :
        //        param.order[0]["column"] == "2" ? d.RawVlanIP.ToString() :
        //        param.order[0]["column"] == "3" ? d.RawVlanIP.ToString() :
        //        param.order[0]["column"] == "4" ? d.RawVlanIP.ToString() : d.Site.Name);

        //    if (param.order[0]["dir"] == "asc")
        //        queriedDevices = queriedDevices.OrderBy(orderingFunction).AsQueryable();
        //    else
        //        queriedDevices = queriedDevices.OrderByDescending(orderingFunction).AsQueryable();

        //    var takecount = param.length;
        //    var displayedResults = queriedDevices.Skip(param.start).Take(takecount).ToArray();

        //    var results = from r in displayedResults
        //                  select new[] {
        //                      r.Site.Name,
        //                      r.VlanNumber.ToString(),
        //                      r.IPNetwork.Network.ToString() + "/" + r.IPNetwork.Cidr.ToString(),
        //                      r.IPNetwork.FirstUsable.ToString(),
        //                      r.IPNetwork.LastUsable.ToString(),
        //                      ((double)uow.Repo<Device>().Get(d => d.Vlan.VlanId == r.VlanId).Count() / (double)r.IPNetwork.Usable).ToString("P0") + " (" + uow.Repo<Device>().Get(d => d.Vlan.VlanId == r.VlanId).Count() + "/" + r.IPNetwork.Usable + ")"
        //                  };

        //    return Json(new
        //    {
        //        recordsTotal = devices.Count(),
        //        recordsFiltered = devices.Count(),
        //        data = results
        //    });
        //}
    }
}
