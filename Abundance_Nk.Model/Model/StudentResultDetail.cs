using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentResultDetail
    {
        public StudentResult Header { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }
        public decimal? Score { get; set; }
        public string SpecialCaseMessage { get; set; }
    }


}
