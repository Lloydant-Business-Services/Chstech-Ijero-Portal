using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                DisplayMessageLogic messageLogic = new DisplayMessageLogic();
                List<DisplayMessage> displaymessage = new List<DisplayMessage>();

                displaymessage = messageLogic.GetModelsBy(b => b.Activated);

                List<string> messages = new List<string>();
                foreach (var message in displaymessage)
                {
                    messages.Add(message.Message);

                }

                ViewBag.message = messages;

            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        public ActionResult About()
        {
            AdmissionCriteriaLogic admissionCriteriaLogic = new AdmissionCriteriaLogic();
            AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
            RankingDataLogic rankingDataLogic = new RankingDataLogic();
            var unranked = rankingDataLogic.GetModelsBy(a => a.Total == 0 && a.Jamb_Raw_Score == 0).Take(1000);
            foreach (RankingData rank in unranked)
            {
                var applicant = appliedCourseLogic.GetModelsBy(a => a.Person_Id == rank.Person.Id).FirstOrDefault();
                admissionCriteriaLogic.EvaluateApplication(applicant);
            }
            ViewBag.Message = "Done";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}