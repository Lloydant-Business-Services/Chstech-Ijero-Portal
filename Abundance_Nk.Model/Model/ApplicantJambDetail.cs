using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantJambDetail
    {
        public Person Person { get; set; }

        [Required]
        [Display(Name = "JAMB Registration No")]
        public string JambRegistrationNumber { get; set; }

        //[Required]
        [Display(Name = "JAMB Score")]
        public short? JambScore { get; set; }

        public InstitutionChoice InstitutionChoice { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        [Display(Name = "English")]
        public OLevelSubject Subject1 { get; set; }
        [Display(Name = "Second Subject")]
        public OLevelSubject Subject2 { get; set; }
        [Display(Name = "Third Subject")]
        public OLevelSubject Subject3 { get; set; }
        [Display(Name = "Fourth Subject")]
        public OLevelSubject Subject4 { get; set; }

        public int? Subject1Score { get; set; }
        public int? Subject2Score { get; set; }
        public int? Subject3Score { get; set; }
        public int? Subject4Score { get; set; }
    }



}
