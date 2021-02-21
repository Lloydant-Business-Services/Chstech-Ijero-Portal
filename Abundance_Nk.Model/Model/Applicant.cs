using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class Applicant
    {
        public ApplicationForm ApplicationForm { get; set; }
        public Person Person { get; set; }
        public Ability Ability { get; set; }
       
        [Display(Name = "If other Specify")]
        public string OtherAbility { get; set; }

        [Display(Name = "Extra-Curricullar Activities e.g. (Sports/Hobbies)")]
        public string ExtraCurricullarActivities { get; set; }

        public ApplicantStatus Status { get; set; }
    }



}
