using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentAcademicInformation
    {
        public Student Student { get; set; }
        public ModeOfEntry ModeOfEntry { get; set; }
        public ModeOfStudy ModeOfStudy { get; set; }

        [Display(Name = "Year of Admission")]
        public int YearOfAdmission { get; set; }

        [Display(Name = "Possible Year of Graduation")]
        public int YearOfGraduation { get; set; }
        public Level Level { get; set; }
        public DateTime? GraduationDate { get; set; }
        public DateTime? TranscriptDate { get; set; }
    }


}
