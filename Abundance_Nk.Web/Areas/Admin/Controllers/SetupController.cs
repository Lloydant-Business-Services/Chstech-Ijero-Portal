using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class SetupController : BaseController
    {
        private SetupViewModel setupViewModel;
        private SessionLogic sessionLogic;
        private GeneralAuditLogic generalAuditLogic;
        public SetupController()
        {
            setupViewModel = new SetupViewModel();
            sessionLogic = new SessionLogic();
            generalAuditLogic = new GeneralAuditLogic();
        }
        // GET: Admin/Setup
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SessionManagement()
        {
            setupViewModel.SessionList= sessionLogic.GetAll();
            return View(setupViewModel);
        }
        public JsonResult ActivateDeactivateSession(string id, bool status,int type)
        {
            JsonResultModel result = new JsonResultModel();

            try
            {
                if (!String.IsNullOrEmpty(id))
                {

                    var sessionId = Convert.ToInt32(id);
                    var session=sessionLogic.GetModelBy(f => f.Session_Id == sessionId);
                    if (session?.Id > 0)
                    {
                        switch (type)
                        {
                            case 1:
                                DeactivateSessions(type);
                                session.Activated = status;
                                break;
                            case 2:
                                DeactivateSessions(type);
                                session.ActiveForApplication = status;
                                break;
                            case 3:
                                session.ActiveForFees = status;
                                break;
                            default:
                                break;
                        }
                        sessionLogic.Modify(session);
                        result.IsError = false;
                        result.Message = "Operation Successful";
                        return Json(result, JsonRequestBehavior.AllowGet);

                    }
                        
                    



                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void DeactivateSessions(int type)
        {
            var sessions=sessionLogic.GetAll();
            if (sessions?.Count > 0)
            {
                switch (type)
                {
                    case 1:
                        foreach(var item in sessions)
                        {
                            item.Activated = false;
                            sessionLogic.Modify(item);
                        }
                        break;
                    case 2:
                        foreach (var item in sessions)
                        {
                            item.ActiveForApplication = false;
                            sessionLogic.Modify(item);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public ActionResult ViewGeneralAudit()
        {
            
            return View(setupViewModel);
        }
        [HttpPost]
        public ActionResult ViewGeneralAudit(DateTime dateTo, TimeSpan timeTo, DateTime dateFrom, TimeSpan timeFrom)
        {
            setupViewModel.ShowTable = true;
            DateTime to = dateTo.Add(timeTo);
            DateTime from = dateFrom.Add(timeFrom);
            if (to > from)
            {
                setupViewModel.GeneralAuditList=generalAuditLogic.GetModelsBy(f => f.Time > from && f.Time < to);
            }
            return View(setupViewModel);
        }

    }
}