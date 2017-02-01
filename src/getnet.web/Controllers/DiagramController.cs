using System.Linq;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model.Entities;
using System.Text;
using getnet.core.ssh;
using getnet.Helpers;
using Microsoft.AspNetCore.Authorization;
using getnet.Model;


namespace getnet.Controllers
{
    [Authorize(Roles=Roles.GlobalViewers)]
    [RedirectOnDbIssue]
    public class DiagramController : BaseController
    {
        [Route("/diagrams")]
        public IActionResult Index()
        {
            return View(uow.Repo<Site>().Get().Select(d => d.Name).OrderBy(d => d));
        }

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

        public IActionResult Viz(string id)
        {
            int siteId;
            Site site = null;
            if (int.TryParse(id, out siteId))
                site = uow.Repo<Site>().Get(d => d.SiteId == siteId, includeProperties: "HotPaths,NetworkDevices,NetworkDevices.RemoteNetworkDeviceConnections").FirstOrDefault();
            else
                site = uow.Repo<Site>().Get(d => d.Name == id, includeProperties: "HotPaths,NetworkDevices,NetworkDevices.RemoteNetworkDeviceConnections").FirstOrDefault();
            var sb = new StringBuilder();
            sb.Append(@"graph """ + site.Name + @""" { ");
            sb.Append(@"subgraph cluster_0 { label=""Hotpath Connections""; ");
            foreach (var hp in site.HotPaths)
                sb.Append(string.Format(@"""{0}""; ", hp.Name + " (" + hp.MonitorDeviceHostname.ReverseTruncateWithEllipsis(11) + ")"));
            sb.Append(@"} subgraph cluster_1 { margin=30; label=""Routing""; ");
            foreach (var dev in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                sb.Append(string.Format(@"""{0}"" [label=""{1}"", shape=box]; ",
                    dev.RawManagementIP,
                    $"{dev.Hostname.ReverseTruncateWithEllipsis(11)}\\n{dev.Model}\\n{dev.ManagementIP.ToString()}"));
            sb.Append(@"} subgraph cluster_2 { margin=30; label=""Switching""; ");
            foreach (var dev in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Switch)))
                sb.Append(string.Format(@"""{0}"" [label=""{1}"", shape=box]; ", 
                    dev.RawManagementIP, 
                    $"{dev.Hostname.ReverseTruncateWithEllipsis(11)}\\n{dev.Model}\\n{dev.ManagementIP.ToString()}" ));
            sb.Append("} ");

            sb.Append(string.Format(@"""{0}"" [shape=Msquare];", site.Name));

            foreach (var hp in site.HotPaths)
                sb.Append(string.Format(@"""{0}"" -- ""{1}""; ", hp.Name + " (" + hp.MonitorDeviceHostname.ReverseTruncateWithEllipsis(11) + ")", site.Name));
            
            foreach (var con in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)).SelectMany(d => d.RemoteNetworkDeviceConnections))
            {
                sb.Append(string.Format(@"""{0}"" -- ""{1}""; ", site.Name, con.NetworkDevice.RawManagementIP));
            }
            foreach (var con in site.NetworkDevices.SelectMany(d => d.RemoteNetworkDeviceConnections))
                sb.Append(string.Format(@"""{0}"" -- ""{1}"" [headlabel=""{2}"";taillabel=""{3}""]; ", 
                    con.NetworkDevice.RawManagementIP, 
                    con.ConnectedNetworkDevice.RawManagementIP, 
                    con.DevicePort.ShortIntName(), 
                    con.ConnectedDevicePort.ShortIntName()));

            sb.Append("}");
            return PartialView("_viz", sb.ToString());
        }
    }
}
