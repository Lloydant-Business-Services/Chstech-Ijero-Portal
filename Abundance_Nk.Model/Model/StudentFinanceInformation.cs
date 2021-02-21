using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentFinanceInformation
    {
        public Student Student { get; set; }
        public ModeOfFinance Mode { get; set; }

        [Display(Name = "Scholarship Title")]
        public string ScholarshipTitle { get; set; }
    }


}
