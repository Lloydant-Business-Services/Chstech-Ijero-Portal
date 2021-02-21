using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class DeliveryServiceZone
    {
        public int Id { get; set; }
        public DeliveryService DeliveryService { get; set; }
        public GeoZone GeoZone { get; set; }
        public Fee Fee { get; set; }
        public Country Country { get; set; }
        public bool Activated { get; set; }
        public string Name { get; set; }
    }
}
