using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicationFormSetting
    {
        public int Id { get; set; }
        public PersonType PersonType { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public Session Session { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime DateEntered { get; set; }
        public User EnteredBy { get; set; }

        [Display(Name = "Exam Date")]
        public DateTime? ExamDate { get; set; }

        [Display(Name = "Exam Venue")]
        public string ExamVenue { get; set; }

        [Display(Name = "Exam Time")]
        public TimeSpan? ExamTime { get; set; }

        public DateTime? RegistrationEndDate { get; set; }

        public TimeSpan? RegistrationEndTime { get; set; }
        public string RegistrationEndTimeString { get; set; }

        [Display(Name = "Form Type")]
        public string Name { get; set; }

        public FeeType FeeType { get; set; }
    }
}