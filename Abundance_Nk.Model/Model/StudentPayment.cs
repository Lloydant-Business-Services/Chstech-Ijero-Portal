using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentPayment : Payment
    {
        public Student Student { get; set; }
        public Level Level { get; set; }
        public decimal Amount { get; set; }
        public bool Status { get; set; }
    }
}