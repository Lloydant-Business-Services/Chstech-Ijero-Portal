using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionListAudit: Audit
    {
        public long Id { get; set; }
        public AdmissionList AdmissionList { get; set; }
        public ApplicationForm Form { get; set; }
        public Department Deprtment { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
      
    }
}
