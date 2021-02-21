using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class DepartmentalSchoolFees
    {
        public Department department { get; set; }
        public Programme programme { get; set; }
        public Decimal Amount { get; set; }
        public FeeType feetype { get; set; }
        public Level level { get; set; }

    }
}
