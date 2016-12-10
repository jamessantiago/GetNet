using System.ComponentModel.DataAnnotations;

namespace getnet.core.Model.Entities
{
    public class Site
    {
        [MaxLength(100)]
        public string Building { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Owner { get; set; }

        public int SiteID { get; set; }
        public SiteStatus Status { get; set; }
    }
}