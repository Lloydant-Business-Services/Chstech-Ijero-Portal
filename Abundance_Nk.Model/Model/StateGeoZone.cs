using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class StateGeoZone
    {
        public int Id { get; set; }
        public State State { get; set; }
        public GeoZone GeoZone { get; set; }
        public bool Activated { get; set; }
    }
}
