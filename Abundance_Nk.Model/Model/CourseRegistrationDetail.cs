using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseRegistrationDetail
    {
        public long Id { get; set; }
        public CourseRegistration CourseRegistration { get; set; }
        public Course Course { get; set; }
        public CourseMode Mode { get; set; }
        public Semester Semester { get; set; }
        public decimal? TestScore { get; set; }
        public decimal? ExamScore { get; set; }
        public int? CourseUnit { get; set; }
        public string SpecialCase { get; set; }
    }


}
