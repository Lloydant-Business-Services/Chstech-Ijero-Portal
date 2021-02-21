using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{

    public class ElearningResult
    {
        public string Name { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string SessionName { get; set; }
        public decimal AverageScore { get; set; }
        public int NoOfTest { get; set; }
        public string MatricNo { get; set; }
        public string LecturerName { get; set; }
        public string LevelName { get; set; }
        public string Department { get; set; }
        public string Programme { get; set; }
        public string Semester { get; set; }
        public string School { get; set; }
    }
}
