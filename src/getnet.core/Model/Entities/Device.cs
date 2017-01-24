using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace getnet.core.Model.Entities
{
    public class Device
    {
        public int DeviceId { get; set; }
        public DeviceType Type { get; set; }

        [Required]
        public long RawIP { get; set; }

        [NotMapped]
        public IPAddress IP => new IPAddress(RawIP);

        [Required, StringLength(50)]
        public string MAC { get; set; }

        [StringLength(255)]
        public string Hostname { get; set; }

        public DateTime LastSeenOnline { get; set; }
        
        public DateTime DiscoveryDate { get; set; } 

        [StringLength(50)]
        public string SerialNumber { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Details { get; set; }

        [StringLength(100)]
        public string Port { get; set; }

        [StringLength(500)]
        public string ReservationComment { get; set; }
        
        public virtual Tenant Tenant { get; set; }
        public virtual NetworkDevice NetworkDevice { get; set; }
        public virtual Vlan Vlan { get; set; }
        public virtual Site Site { get; set; }
        public virtual ICollection<DeviceHistory> DeviceHistories { get; set; }

        public static Expression<Func<Device, bool>> SearchPredicates(string text)
        {
            var predicates = PredicateBuilder.False<Device>();
            foreach (var term in text.Split(' '))
            {
                Regex ip = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
                IPAddress searchIp = null;
                if (ip.IsMatch(term) && IPAddress.TryParse(term, out searchIp))
                {
                    predicates = predicates.Or(d => d.RawIP == searchIp.ToInt());
                }

                Regex network = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\/\d{1,2}$");
                IPNetwork searchNet = null;
                if (network.IsMatch(term) && IPNetwork.TryParse(term, out searchNet))
                {
                    predicates = predicates.Or(d => d.RawIP >= searchNet.Network.ToInt() && d.RawIP <= searchNet.Broadcast.ToInt());
                }

                var st = term.ToLower();
                predicates = predicates.Or(d => d.Hostname.ToLower().Contains(st));
                predicates = predicates.Or(d => d.MAC.ToLower().Contains(st));
            }
            return predicates;
        }
    }

    public class DeviceBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>().HasIndex("MAC").IsUnique();
        }
    }
}
