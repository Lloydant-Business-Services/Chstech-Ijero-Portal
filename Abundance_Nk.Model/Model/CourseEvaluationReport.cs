using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Entity.Model
{
    public class CourseEvaluationReport
    {
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Level { get; set; }
        public string Session { get; set; }
        public string Semester { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }  
        public long? Score { get; set; }
        public int? NumberOfStudent { get; set; }
        public string LecturerName { get; set; }
        public long PersonId { get; set; }
    }
}
