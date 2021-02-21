using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;


namespace Abundance_Nk.Model.Model
{
    public class AdmissionCriteria
    {
        [Display(Name = "Admission Criteria")]
        public int Id { get; set; }

        public Programme Programme { get; set; }
        public Department Department { get; set; }

        [Display(Name = "Minimum Required No of Subject")]
        public int MinimumRequiredNumberOfSubject { get; set; }

        [Display(Name = "Date Entered")]
        public DateTime DateEntered { get; set; }
    }



}
