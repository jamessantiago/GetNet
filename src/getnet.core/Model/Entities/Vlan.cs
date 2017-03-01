using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Net;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace getnet.core.Model.Entities
{
    public class Vlan
    {
        public int VlanId{ get; set; }

        [Required]
        public int VlanNumber { get; set; }

        [Required]
        public long RawVlanIP { get; set; }

        [Required]
        public long RawVlanSM { get; set; }


        [NotMapped]
        public IPAddress VlanIP => new IPAddress(RawVlanIP);

        [NotMapped]
        public IPAddress VlanSM => new IPAddress(RawVlanSM);

        [NotMapped]
        public IPNetwork IPNetwork => IPNetwork.Parse(VlanIP, VlanSM);

        [Required]
        public virtual NetworkDevice NetworkDevice { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual Tenant Tenant { get; set; }

        [Required]
        public virtual Site Site { get; set; }

        public static Expression<Func<Vlan, bool>> SearchPredicates(string text)
        {
            var predicates = PredicateBuilder.False<Vlan>();
            
            foreach (var term in text.Split(' '))
            {
                Regex ip = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
                IPAddress searchIp = null;
                if (ip.IsMatch(term) && IPAddress.TryParse(term, out searchIp))
                {
                    predicates = predicates.Or(d => searchIp.ToInt() >= d.RawVlanIP && searchIp.ToInt() <= d.RawVlanSM);
                }

                Regex network = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\/\d{1,2}$");
                IPNetwork searchNet = null;
                if (network.IsMatch(term) && IPNetwork.TryParse(term, out searchNet))
                {
                    predicates = predicates.Or(d => d.RawVlanIP >= searchNet.Network.ToInt() && d.RawVlanIP <= searchNet.Broadcast.ToInt());
                }
                int vlannum = 0;
                if (int.TryParse(term, out vlannum))
                {
                    predicates = predicates.And(d => d.VlanNumber == vlannum);
                }
            }

            return predicates;
        }
    }
}
