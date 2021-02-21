using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abundance_Nk.Model.Model
{
   public class ScoreGrade
   {
       public int Id { get; set; }
       public int From { get; set; }
       public int To { get; set; }
       public string Grade { get; set; }
       public decimal GradePoint { get; set; }
       public string Performance { get; set; }
       public string Description { get; set; }
    }
}
