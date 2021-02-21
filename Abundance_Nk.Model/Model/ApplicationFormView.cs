using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicationFormView
    {
        public long FormId { get; set; }
        public long SerialNumber { get; set; }
        public string FormNumber { get; set; }
        public int SessionId { get; set; }
        public long PersonId { get; set; }
        public string Name { get; set; }
        public int? ExamSerialNumber { get; set; }
        public string ExamNumber { get; set; }
        public string RejectReason { get; set; }
        public string Remarks { get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }

        public long StudentNumber { get; set; }
        public string MatricNumber { get; set; }

        public ApplicantJambDetail JambDetail { get; set; }
        public bool IsSelected { get; set; }
    }



}
