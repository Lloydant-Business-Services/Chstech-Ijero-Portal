using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseRegistrationReportModel
    {
        public string Session { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
        public string Semester { get; set; }
        public string CourseCode { get; set; }
        public int CourseUnit { get; set; }
        public string CourseName { get; set; }
    }
}
