using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;

namespace getnet.core
{
    public static partial class Discovery
    {
        public static async Task FindNetworkDevices(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                uow.DumpChanges();
                foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)).ToList())
                {
                    RecurseDevices(router, new List<NetworkDevice>());
                }
            });
        }

        private static List<NetworkDevice> RecurseDevices(NetworkDevice device, List<NetworkDevice> devices)
        {
            IEnumerable<CdpNeighbor> neighbors = null;
            try
            {
                neighbors = device.ManagementIP.Ssh().Execute<CdpNeighbor>().Where(d => d.Capabilities.GetCaps().HasFlag(NetworkCapabilities.Switch));
            } catch (Exception ex)
            {
                logger.Error(ex, WhistlerTypes.NetworkDiscovery, "Failed to retrieve cdp table from device " + device.Hostname);
                return devices;
            }

            foreach (var nei in neighbors)
            {
                var existingDevice = uow.Repo<NetworkDevice>().Get(d => d.RawManagementIP == nei.IP.ToInt()).FirstOrDefault();
                if (!devices.Any(d => d.RawManagementIP == nei.IP.ToInt() || d.Hostname == nei.Hostname) && existingDevice == null)
                {
                    var newDevice = new NetworkDevice
                    {
                        Hostname = nei.Hostname,
                        Model = nei.Model,
                        RawManagementIP = nei.IP.ToInt(),
                        Capabilities = nei.Capabilities.GetCaps(),
                        Site = device.Site
                    };
                    var newDeviceChanges = uow.Repo<NetworkDevice>().Insert(newDevice);
                    uow.Save();
                    newDevice = uow.Repo<NetworkDevice>().GetByID((int)newDeviceChanges.CurrentValues["NetworkDeviceId"]);
                    uow.Repo<Site>().Get(d => d.SiteId == device.Site.SiteId, includeProperties: "NetworkDevices").First().NetworkDevices.Add(newDevice);
                    uow.Save();
                    var ndc = new NetworkDeviceConnection()
                    {
                        NetworkDeviceId = device.NetworkDeviceId,
                        NetworkDevice = device,
                        ConnectedNetworkDeviceId = newDevice.NetworkDeviceId,
                        ConnectedNetworkDevice = newDevice,
                        DevicePort = nei.OutPort,
                        ConnectedDevicePort = nei.InPort
                    };
                    uow.Repo<NetworkDeviceConnection>().Insert(ndc);
                    uow.Save();
                    UpdateDevice(newDevice);
                    devices.Add(newDevice);
                    devices = RecurseDevices(newDevice, devices);
                }
                else if (!devices.Any(d => d.RawManagementIP == nei.IP.ToInt() || d.Hostname == nei.Hostname) && existingDevice != null)
                {
                    existingDevice.Hostname = nei.Hostname;
                    existingDevice.Model = nei.Model;
                    //existingDevice.Site = device.Site;
                    existingDevice.Capabilities = nei.Capabilities.GetCaps();
                    uow.Save();
                    UpdateDevice(existingDevice);
                    devices.Add(existingDevice);
                    devices = RecurseDevices(existingDevice, devices);
                    uow.Save();
                    //todo do something for determining connections for existing devices
                }
            }
            return devices;
        }

        private static void UpdateDevice(NetworkDevice device)
        {
            try
            {
                var version = device.ManagementIP.Ssh().Execute<DeviceVersion>().First();
                device.ChassisSerial = version.Serial;
                uow.Save();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to update device " + device.Hostname, ex, WhistlerTypes.NetworkDiscovery);
            }
        }
    }
}
