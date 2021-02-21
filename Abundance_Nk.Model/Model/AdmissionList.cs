using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionList
    {
        public long Id { get; set; }
        public ApplicationForm Form { get; set; }
        public AdmissionListBatch Batch { get; set; }
        public Department Deprtment { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public bool Activated { get; set; }
        public bool Deactivated { get; set; }
        public bool ActivateAlt { get; set; }
    }

    public class AdmissionEmail
    {
        public string Name { get; set; }
        public string message { get; set; }
        public string header { get; set; }
        public string footer { get; set; }
    }



}
