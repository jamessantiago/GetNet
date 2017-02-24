using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;

namespace getnet.core
{
    public static partial class EndpointAnalysis
    {
        private static UnitOfWork uow = new UnitOfWork();

        public static void FullAnalysis(int deviceId)
        {
            SetHostname(deviceId);
            //SetType(deviceId);
        }

        public static void SetHostname(int deviceId)
        {

            var device = uow.Repo<Device>().Get(d => d.DeviceId == deviceId, includeProperties: "NetworkDevice").FirstOrDefault();
            if (device == null) return;
            try
            {
                var cts = new CancellationTokenSource();
                IPHostEntry hostname = null;
                cts.CancelAfter(TimeSpan.FromSeconds(1));
                Task.Run(async () => hostname = await System.Net.Dns.GetHostEntryAsync(device.IP.ToString()), cts.Token);
                device.Hostname = hostname.HostName;
                uow.Save();
                return;
            }
            catch
            {
                // ignored
            }

            var cdp = device.NetworkDevice.ManagementIP.Ssh().Execute<CdpNeighbor>();
            var cdpEntry = cdp.FirstOrDefault(d => d.IP.ToInt() == device.RawIP);
            if (cdpEntry == null || !cdpEntry.Capabilities.Contains("Phone")) return;
            device.Type = DeviceType.Phone;
            device.Hostname = cdpEntry.Hostname;
            uow.Save();
        }

        public static void SetType(int deviceId)
        {
            var device = uow.Repo<Device>().Get(d => d.DeviceId == deviceId, includeProperties: "NetworkDevice").FirstOrDefault();
            if (device == null || device.Type == DeviceType.Phone) return;

            //probably windows
            if (device.Hostname.HasValue())
            {
                //do something
            }
        }
    }
}