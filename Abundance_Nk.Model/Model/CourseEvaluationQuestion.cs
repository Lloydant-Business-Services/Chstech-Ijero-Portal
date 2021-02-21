using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseEvaluationQuestion
    {
        public int Id { get; set; }
        public int Section { get; set; }
        public string Question { get; set; }
        public int Score { get; set; }
        public bool Activated { get; set; }
       
    }
}
