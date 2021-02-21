using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class HostelFee
    {
        public long Id { get; set; }
        public Payment Payment { get; set; }
        public Hostel Hostel { get; set; }
        public double Amount { get; set; }
    }
}
