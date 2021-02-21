using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentResult
    {
        public long Id { get; set; }
        public StudentResultType Type { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public SessionSemester SessionSemester { get; set; }
        public User Uploader { get; set; }
        public DateTime DateUploaded { get; set; }
        public string UploadedFileUrl { get; set; }
        public int MaximumObtainableScore { get; set; }

        public List<StudentResultDetail> Results { get; set; }
    }


}
