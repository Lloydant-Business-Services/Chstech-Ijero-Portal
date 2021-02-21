using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ComplaintLog
    {
        public long Id { get; set; }
        [Required]
        [Display(Name="Full name")]
        public string Name { get; set; }
        [Required]
        [RegularExpression("^0[0-9]{10}$", ErrorMessage = "Phone number is not valid")]
        [Display(Name = "Phone Number")]
        public string MobileNumber { get; set; }
        [Display(Name = "Application Form Number")]
        public string ApplicationNumber { get; set; }
        public string RRR { get; set; }
        [Display(Name = "Exam Number")]
        public string ExamNumber { get; set; }
        [Display(Name = "Etranzact Confirmation Order Number or Scratch Card Pin")]
        public string ConfirmationNumber { get; set; }
        [Display(Name = "Describe your Problem")]
        [Required]
        public string Complain { get; set; }
        [Display(Name = "Resolved")]
        public bool Status { get; set; }
        public DateTime DateSubmitted { get; set; }
        [Display(Name = "Ticket ID")]
        public string TicketID { get; set; }
        public string Comment { get; set; }
    }
}
