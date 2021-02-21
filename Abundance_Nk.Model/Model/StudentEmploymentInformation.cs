using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentEmploymentInformation
    {
        public int Id { get; set; }
        public Student Student { get; set; }

        [Required]
        [Display(Name = "Place of Last Employment")]
        public string PlaceOfLastEmployment { get; set; }

        [Display(Name = "Last Employment Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Last Employment End Date")]
        public DateTime EndDate { get; set; }

        public Value StartDay { get; set; }
        public Value StartMonth { get; set; }
        public Value StartYear { get; set; }

        public Value EndDay { get; set; }
        public Value EndMonth { get; set; }
        public Value EndYear { get; set; }


    }



}
