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
    public class Subnet
    {
        public int SubnetId { get; set; }

        [Required]
        public long RawSubnetIP { get; set; }

        [Required]
        public long RawSubnetSM { get; set; }

        public SubnetTypes Type { get; set; }

        [NotMapped]
        public IPAddress SubnetIP => new IPAddress(RawSubnetIP);

        [NotMapped]
        public IPAddress SubnetSM => new IPAddress(RawSubnetSM);

        [NotMapped]
        public IPNetwork IPNetwork => IPNetwork.Parse(SubnetIP, SubnetSM);

        public virtual Site Site { get; set; }

        public static Expression<Func<Subnet, bool>> SearchPredicates(string text)
        {
            var predicates = PredicateBuilder.False<Subnet>();
            foreach (var term in text.Split(' '))
            {
                Regex ip = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
                IPAddress searchIp = null;
                if (ip.IsMatch(term) && IPAddress.TryParse(term, out searchIp))
                {
                    predicates = predicates.Or(d => d.RawSubnetIP == searchIp.ToInt());
                }

                Regex network = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\/\d{1,2}$");
                IPNetwork searchNet = null;
                if (network.IsMatch(term) && IPNetwork.TryParse(term, out searchNet))
                {
                    predicates = predicates.Or(d => d.RawSubnetIP >= searchNet.Network.ToInt() && d.RawSubnetIP <= searchNet.Broadcast.ToInt());
                }
                SubnetTypes searchType = 0;
                if (Enum.TryParse<SubnetTypes>(term, out searchType))
                {
                    predicates = predicates.Or(d => d.Type == searchType);
                }
                int searchCidr = 0;
                if (int.TryParse(term, out searchCidr) && searchCidr > 0 && searchCidr < 33)
                {
                    var netmask = IPNetwork.ToNetmask((byte)searchCidr, System.Net.Sockets.AddressFamily.InterNetwork);
                    predicates = predicates.Or(d => d.RawSubnetSM == netmask.ToInt());
                }
            }
            return predicates;
        }
    }
}
