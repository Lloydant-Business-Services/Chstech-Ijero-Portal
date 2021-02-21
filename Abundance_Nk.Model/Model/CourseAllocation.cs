using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseAllocation
    {
        public long Id { get; set; }
        public User User { get; set; }
        public Course Course { get; set; }
        public Department Department { get; set; }
        public Level Level { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Programme Programme { get; set; }
        public bool? IsHOD { get; set; }
        public bool? IsDean { get; set; }
        public Department HodDepartment { get; set; }
        public bool? CanUpload { get; set; }
        public bool StaffCanUpload { get; set; }
        public bool Uploaded { get; set; }
    }
}
