using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionListReportFormat
    {
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public string FacultyName { get; set; }
        public string FacultyCode { get; set; }
        public string ProgrammeName { get; set; }
        public string ProgrammeCode { get; set; }
        public string ApplicationNumber { get; set; }
        public string ExamNumber { get; set; }
        public string Sex { get; set; }
        public string Session { get; set; }
        public int TotalCount { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
