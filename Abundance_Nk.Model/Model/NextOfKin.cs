using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class NextOfKin
    {
        public Person Person { get; set; }

        public Relationship Relationship { get; set; }
        //public Ability Ability { get; set; }

        [Required]
        [Display(Name = "Next of Kin")]
        public string Name { get; set; }
               
        [Required]
        [Display(Name = "Next of Kin's Address")]
        public string ContactAddress { get; set; }

        //[Display(Name = "If other Specify")]
        //public string OtherAbility { get; set; }

        //[Display(Name = "Extra-Curricullar Activities e.g. (Sports/Hobbies)")]
        //public string ExtraCurricullarActivities { get; set; }

        [Required]
        [Display(Name = "Mobile Phone")]
        public string MobilePhone { get; set; }

        public ApplicationForm ApplicationForm { get; set; }
        public PersonType PersonType { get; set; }
       
    }


}
