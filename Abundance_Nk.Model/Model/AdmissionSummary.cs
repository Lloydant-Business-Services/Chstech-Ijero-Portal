using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionSummary
    {
        public int Programme_Id { get; set; }
        public string Programme_Name { get; set; }
        public string Department_Name { get; set; }
        public string Session_Name { get; set; }
        public int AdmissionCount { get; set; }
    }
}
