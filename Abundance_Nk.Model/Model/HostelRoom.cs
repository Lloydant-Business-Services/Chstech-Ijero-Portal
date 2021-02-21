using Abundance_Nk.Model.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class HostelRoom
    {
        [Display(Name = "Room Number")]
        public long Id { get; set; }
        [Display(Name = "Room Number")]
        public string Number { get; set; }
        public bool Reserved { get; set; }
        public bool Activated { get; set; }
        public HostelSeries Series { get; set; }
        public Hostel Hostel { get; set; }     
        public int RoomCapacity { get; set; }
        public int Corners { get; set; }
        public bool Allocated { get; set; }
    }
}
