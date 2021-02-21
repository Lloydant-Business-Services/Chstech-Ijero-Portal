using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class StaffDepartment
    {
        public long Id { get; set; }
        public Staff Staff  { get; set; }
        public Department Department { get; set; }
        public SessionSemester SessionSemester { get; set; }
        public System.DateTime DateEntered { get; set; }
        public bool IsHead { get; set; }
        public bool IsExamOfficer { get; set; }

    }
}
