using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Common.Controllers
{
    public class TranscriptsController : BaseController
    {
        // GET: Common/Transcripts
        private TranscriptViewModel _transcriptViewModel;
        private string appRoot = ConfigurationManager.AppSettings["AppRoot"];
        public TranscriptsController()
        {
            _transcriptViewModel = new TranscriptViewModel();
        }
        public ActionResult Index()
        {
            try
            {

            }
            catch (Exception ex)
            {
                SetMessage("The following error Occurred" + ex.Message, Message.Category.Error);
            }
            return View(_transcriptViewModel);
        }
    }
}