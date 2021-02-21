using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ExamRawScoreSheetReport
    {
        public string MatricNumber { get; set; }
        public string Fullname { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Semester { get; set; }
        public string Session { get; set; }
        public string Programme { get; set; }
        public int CourseUnit { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string Remark { get; set; }
        public double QU1 { get; set; }
        public double QU2 { get; set; }
        public double QU3 { get; set; }
        public double QU4 { get; set; }
        public double QU5 { get; set; }
        public double QU6 { get; set; }
        public double QU7 { get; set; }
        public double QU8 { get; set; }
        public double QU9 { get; set; }
        public double T_EX { get; set; }
        public double T_CA { get; set; }
        public double EX_CA { get; set; }
        public double SuccessRate { get; set; }
        public string Date { get; set; }
        public string Level { get; set; }
        public string Identifier { get; set; }
        public int PassedStudentCount { get; set; }
        public int CourseModeId { get; set; }
        public string CourseModeName { get; set; }
        public long UserId { get; set; }
        public string StaffName { get; set; }
        public long CourseId { get; set; }
        public int SessionId { get; set; }
        public int SemesterId { get; set; }
        public int ProgrammeId { get; set; }
        public int LevelId { get; set; }
        public string SpecialCase { get; set; }
        public long StudentResultId { get; set; }
        public string UploadDate { get; set; }
    }
}
