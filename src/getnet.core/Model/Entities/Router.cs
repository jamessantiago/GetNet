using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace getnet.core.Model.Entities
{
    public class Router
    {
        [MaxLength(100)]
        public string ChassisSerial { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }

        public string Hostname { get; set; }

        [NotMapped]
        public IPAddress ManagementIP => new IPAddress((long)RawManagementIP);

        [MaxLength(50)]
        public string Model { get; set; }

        public int RawManagementIP { get; set; }
        public int RouterID { get; set; }
    }
}