using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Transactions;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class TranscriptProcessorViewModel
    {
        public TranscriptProcessorViewModel()
        {
            transcriptSelectList = Utility.PopulateTranscriptStatusSelectListItem();
            transcriptClearanceSelectList = Utility.PopulateTranscriptClearanceStatusSelectListItem();
            transcriptRequest = new TranscriptRequest(); 
            StateSelectList = Utility.PopulateStateSelectListItem();
            CountrySelectList = Utility.PopulateCountrySelectListItem();
        }
        public List<SelectListItem> transcriptSelectList { get; set; }
        public List<SelectListItem> transcriptClearanceSelectList { get; set; }
        public List<TranscriptRequest> transcriptRequests { get; set; }
        public TranscriptRequest transcriptRequest { get; set; }
        public TranscriptStatus transcriptStatus { get; set; }
        public TranscriptClearanceStatus transcriptClearanceStatus { get; set; }
        public Person Person { get; set; }
        public Model.Model.Student Student { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> CountrySelectList { get; set; }
        public String RequestDateString { get; set; }
    }
}