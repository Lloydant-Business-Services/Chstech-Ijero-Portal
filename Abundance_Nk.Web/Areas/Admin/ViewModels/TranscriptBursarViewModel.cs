using System.ComponentModel.DataAnnotations;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class TranscriptBursarViewModel
    {
         public TranscriptBursarViewModel()
        {
            transcriptRequest = new TranscriptRequest();
        }
        public List<TranscriptRequest> transcriptRequests { get; set; }
        public TranscriptRequest transcriptRequest { get; set; }
        public TranscriptStatus transcriptStatus { get; set; }
        public TranscriptClearanceStatus transcriptClearanceStatus { get; set; }
        public List<RemitaPayment> remitaPayments { get; set; }
        [DataType(DataType.Date)]
        public DateTime FromDateTime { get; set; }
        [DataType(DataType.Date)]
        public DateTime ToDateTime { get; set; }
    }
   
}