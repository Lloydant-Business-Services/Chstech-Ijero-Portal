using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentDetailFormat
    {
        public long PersonId { get; set; }
        public string Name { get; set; }
        public string MatricNumber { get; set; }
        public string ImageUrl { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public int GraduationYear { get; set; }
        public string BloodGroup { get; set; }
        public string SignatureUrl { get; set; }
        public string ApplicationNumber { get; set; }
        public string Level { get; set; }
        public int ProgrammeId { get; set; }
        public int DepartmentId { get; set; }
        public int NewStudentTotalCount { get; set; }
        public int ReturningStudentTotalCount { get; set; }
        public int TotalCount { get; set; }
        public string AdmittedSession { get; set; }
        public int AdmittedSessionId { get; set; }
        public string Session { get; set; }
        public int SessionId { get; set; }
        public int Count { get; set; }
        public int NewStudentTotalCountND { get; set; }
        public int NewStudentTotalCountHND { get; set; }
        public int OtherStudentTotalCount{ get; set; }
        public int ReturningStudentTotalCountND { get; set; }
        public int ReturningStudentTotalCountHND { get; set; }
        public string PassportUrl { get; set; }
        public int SN { get; set; }
        
    }
}
