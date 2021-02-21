using Abundance_Nk.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class HostelAllocationCriteria
    {
        public long Id { get; set; }
        public  HostelRoomCorner Corner { get; set; }
        public Hostel Hostel { get; set; }
        public Level Level { get; set; }
        public HostelRoom Room{ get; set; }
        public HostelSeries Series{ get; set; }
        public bool EditAll { get; set; }
    }
}
