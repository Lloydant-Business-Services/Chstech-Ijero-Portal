using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Entity.Model
{
    public class ResultUpdateModel
    {
        public string MatricNumber { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
        public string Session { get; set; }
        public string Semester { get; set; }
        public string LastModifiedDate { get; set; }
        public string StaffName { get; set; }
        public long UserId { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
    }
}
