using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model.Entities;
using System.Text;

namespace getnet.Controllers
{
    public class DiagramController : BaseController
    {
        public IActionResult Sigma(int id)
        {
            var site = uow.Repo<Site>().GetByID(id);
            return View(site);
        }

        public IActionResult SigmaJSON(int id)
        {
            var site = uow.Repo<Site>().Get(d => d.SiteId == id, includeProperties: "NetworkDevices,NetworkDevices.RemoteNetworkDeviceConnections").FirstOrDefault();
            var nodes = site.NetworkDevices.Select(d => new { id = d.Hostname, label = d.ToString(), x= 0, y = d.NetworkDeviceId, size = 3 });
            var edges = site.NetworkDevices.SelectMany(d => d.RemoteNetworkDeviceConnections).Select(d => new { id = d.NetworkDevice.Hostname + "_" + d.ConnectedNetworkDevice.Hostname, source = d.NetworkDevice.Hostname, target = d.ConnectedNetworkDevice.Hostname });
            return Json(new { nodes = nodes, edges = edges });
        }

        public IActionResult Viz(int id)
        {
            var site = uow.Repo<Site>().Get(d => d.SiteId == id, includeProperties: "NetworkDevices,NetworkDevices.RemoteNetworkDeviceConnections").FirstOrDefault();
            var sb = new StringBuilder();
            sb.Append("graph viz { ");
            foreach (var con in site.NetworkDevices.SelectMany(d => d.RemoteNetworkDeviceConnections))
                sb.Append(string.Format(@"""{0}"" -- ""{1}""; ", con.NetworkDevice.Hostname, con.ConnectedNetworkDevice.Hostname));
            sb.Append("}");
            return PartialView("_viz", sb.ToString());
        }
    }
}
