using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicationForm
    {
        [Display(Name = "Form No")]
        public long Id { get; set; }

        [Display(Name = "Serial No")]
        public long? SerialNumber { get; set; }

        [Display(Name = "Application Form Number")]
        public string Number { get; set; }
        public ApplicationFormSetting Setting { get; set; }
        public ApplicationProgrammeFee ProgrammeFee { get; set; }
        public Person Person { get; set; }
        public Payment Payment { get; set; }
        public Session Session { get; set; }
        public DateTime DateSubmitted { get; set; }
        public bool IsAwaitingResult { get; set; }
        public bool Release { get; set; }
        public bool Rejected { get; set; }

        public int? ExamSerialNumber { get; set; }
        public string ExamNumber { get; set; }

        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }
        public string Remarks { get; set; }
        public User VerificationOfficer { get; set; }
        public string VerificationComment { get; set; }
        public bool? VerificationStatus { get; set; }
        public string VerificationStatusStr { get; set; }
        public bool? CleardForAcceptance { get; set; }
    }
}
