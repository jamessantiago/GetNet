using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace getnet.core.Model.Entities
{
    public class NetworkDevice
    {
        public int NetworkDeviceId { get; set; }

        [StringLength(100)]
        public string ChassisSerial { get; set; }
        
        [StringLength(500)]
        public string Details { get; set; }

        [StringLength(100)]
        public string Hostname { get; set; }

        [NotMapped]
        public IPAddress ManagementIP => new IPAddress(RawManagementIP);

        [StringLength(150)]
        public string Model { get; set; }

        public NetworkCapabilities Capabilities { get; set; }

        [Required]
        public long RawManagementIP { get; set; }

        public virtual Tenant Tenant { get; set; }

        public virtual ICollection<NetworkDeviceConnection> RemoteNetworkDeviceConnections { get; set; }

        public virtual ICollection<NetworkDeviceConnection> LocalNetworkDeviceConnections { get; set; }

        public virtual ICollection<Vlan> Vlans { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
        
        [Required]
        public virtual Site Site { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.Hostname);
            sb.AppendLine(this.ManagementIP.ToString());
            sb.AppendLine(this.Model);
            sb.AppendLine(this.ChassisSerial);
            return sb.ToString();
        }

        public static Expression<Func<NetworkDevice, bool>> SearchPredicates(string text)
        {
            var predicates = PredicateBuilder.False<NetworkDevice>();
            foreach (var term in text.Split(' '))
            {
                Regex ip = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
                IPAddress searchIp = null;
                if (ip.IsMatch(term) && IPAddress.TryParse(term, out searchIp))
                {
                    predicates = predicates.Or(d => d.RawManagementIP == searchIp.ToInt());
                }

                Regex network = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\/\d{1,2}$");
                IPNetwork searchNet = null;
                if (network.IsMatch(term) && IPNetwork.TryParse(term, out searchNet))
                {
                    predicates = predicates.Or(d => d.RawManagementIP >= searchNet.Network.ToInt() && d.RawManagementIP <= searchNet.Broadcast.ToInt());
                }

                var st = term.ToLower();
                predicates = predicates.Or(d => d.Hostname.ToLower().Contains(st));
                predicates = predicates.Or(d => d.Model.ToLower().Contains(st));
            }

            return predicates;
        }
    }

    public class NetworkDeviceBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NetworkDevice>().HasIndex("Model");
            modelBuilder.Entity<NetworkDevice>().HasIndex("RawManagementIP")
                .IsUnique();
        }
    }
}