using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AppliedCourseAudit : Audit
    {
        public long Id { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public DepartmentOption Option { get; set; }

        public Programme OldProgramme { get; set; }
        public Department OldDepartment { get; set; }
        public ApplicationForm OldApplicationForm { get; set; }
        public DepartmentOption OldOption { get; set; }
       
    }




}
