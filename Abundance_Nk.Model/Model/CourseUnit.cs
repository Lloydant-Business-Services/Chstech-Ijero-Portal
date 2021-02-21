using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseUnit
    {
        public int Id { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public Level Level { get; set; }
        public Semester Semester { get; set; }
        public byte MinimumUnit { get; set; }
        public byte MaximumUnit { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
    }


}
