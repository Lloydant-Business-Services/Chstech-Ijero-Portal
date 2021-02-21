using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class OLevelExamSitting : Setup
    {
        [Display(Name = "O-Level Exam Sitting")]
        public override int Id { get; set; }

        [Display(Name = "O-Level Exam Sitting")]
        public override string Name { get; set; }
    }


}
