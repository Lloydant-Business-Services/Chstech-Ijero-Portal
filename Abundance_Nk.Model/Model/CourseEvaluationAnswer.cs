using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseEvaluationAnswer
    {
        public long Id { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public int Score { get; set; }
        public CourseEvaluationQuestion CourseEvaluationQuestion { get; set; }
    }
}
