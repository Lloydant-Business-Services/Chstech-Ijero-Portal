using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseEvaluation
    {
      public List<CourseEvaluationQuestion> CourseEvaluationQuestion { get; set; }
      public List<CourseEvaluationQuestion> CourseEvaluationQuestionSectionTwo { get; set; }
      public Course Course { get; set; }
      public bool Selected { get; set; }
      public int QuestioinId { get; set; }
      public int Score { get; set; }
       
    }

}
