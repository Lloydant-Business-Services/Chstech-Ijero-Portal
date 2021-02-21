using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AppliedCourse
    {
        public Person Person { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public DepartmentOption Option { get; set; }
        public DepartmentOption OptionSecondChoice { get; set; }
        //public bool Active { get; set; }

    }



}
