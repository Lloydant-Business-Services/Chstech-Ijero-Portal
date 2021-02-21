using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Model
{
    public class TranscriptRequest
    {
        public Int64 Id { get; set; }
        public Student student { get; set; }
        public Person person { get; set; }
        public Payment payment {get; set;}
        [Display(Name = "Date of Request")]
        public DateTime DateRequested {get; set;}
        [Display(Name="Destination Address")]
        public string DestinationAddress {get; set;}
        [Display(Name = "Destination Address State")]
        public State DestinationState {get; set;}
        [Display(Name = "Destination Address Country")]
        public Country DestinationCountry { get; set; }

        [Display(Name = "Reciever")]
        public string Reciever { get; set; }

        public TranscriptClearanceStatus transcriptClearanceStatus {get; set;}
        [Display(Name = "Statement Of Result")]
        public string StatementOfResult { get; set; }
        public string Passport { get; set; }
        public string Email { get; set; }
        public bool? Processed { get; set; }
        public bool? Verified { get; set; }

        public TranscriptStatus transcriptStatus {get; set;}
        public string ConfirmationOrderNumber { get; set; }

        public string Amount { get; set; }
        public RemitaPayment remitaPayment { get; set; }
        public string RequestType { get; set; }
        public DeliveryServiceZone DeliveryServiceZone { get; set; }
    }
}
