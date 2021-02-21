using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ResultGrade 
    {
        [Display(Name = "Result Grade")]
        public int Id { get; set; }

        public decimal CGPAFrom { get; set; }
        public decimal CGPATo { get; set; }
        public string LevelOfPassCode { get; set; }

        [Display(Name = "Level of Pass")]
        public string LevelOfPass { get; set; }
    }



}
