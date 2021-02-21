using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class HostelAmount
    {
        public int Id { get; set; }
        public Hostel Hostel { get; set; }
        public bool Activated { get; set; }
        public decimal Amount { get; set; }
    }
}
