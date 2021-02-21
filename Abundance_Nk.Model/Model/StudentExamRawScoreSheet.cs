using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentExamRawScoreSheet
    {
        public long Id { get; set; }
        public Student Student { get; set; }
        public Nullable<double> QU1 { get; set; }
        public Nullable<double> QU2 { get; set; }
        public Nullable<double> QU3 { get; set; }
        public Nullable<double> QU4 { get; set; }
        public Nullable<double> QU5 { get; set; }
        public Nullable<double> QU6 { get; set; }
        public Nullable<double> QU7 { get; set; }
        public Nullable<double> QU8 { get; set; }
        public Nullable<double> QU9 { get; set; }
        public Nullable<double> T_EX { get; set; }
        public Nullable<double> T_CA { get; set; }
        public Nullable<double> EX_CA { get; set; }
        public string Remark { get; set; }
        public string Special_Case { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public Level Level { get; set; }
        public User Uploader { get; set; }
        public string MatricNumber { get; set; }
        public string FileUploadURL { get; set; }
        public Programme Programme { get; set; }
    }
}
