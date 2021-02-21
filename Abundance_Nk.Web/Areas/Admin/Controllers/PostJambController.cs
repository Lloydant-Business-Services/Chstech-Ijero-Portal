using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Models;
using System.Transactions;
using System.Linq.Expressions;
using System.IO;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class PostJambController : BaseController
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();       
        //
        // GET: /Admin/PostJamb/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PostjambResultSupportViewModel vModel, FormCollection f)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vModel.JambNumber != null)
                    {
                        PutmeResult result = new PutmeResult();
                        PutmeResultLogic PostUtmeResultLogic = new PutmeResultLogic();
                        result = PostUtmeResultLogic.GetModelsBy(m => m.EXAMNO == vModel.JambNumber && m.Session_Id == 7).FirstOrDefault();
                        if (result == null || result.Id <= 0)
                        {
                            SetMessage("Registration Number / Jamb No was not found! Please check that you have typed in the correct detail", Message.Category.Error);
                            return View(vModel);
                        }
                        else
                        {
                            vModel.putmeResult = result; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            TempData["ResultViewModel"] = vModel;
            return View(vModel);
        }
	
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateResult(PostjambResultSupportViewModel vModel, FormCollection f)
        {
            try
            {
               // TempData["ResultViewModel"] = vModel;
                string dd = f.AllKeys[0].ToString();
                if (ModelState.IsValid && f.AllKeys[0].ToString() != null)
                {
                    if (vModel.putmeResult != null && vModel.putmeResult.Id > 0)
                    {
                        PutmeResult putme = new PutmeResult();
                        PutmeResultLogic putmeLogic = new PutmeResultLogic();
                        putme = vModel.putmeResult;
                        
                        string operation = "UPDATE";
                        string action = "MODIFY APPLICANT JAMB RESULT";
                        string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                        PutmeResultAudit putmeAudit = new PutmeResultAudit();
                        UserLogic loggeduser = new UserLogic();
                        putmeAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                        putmeAudit.Operation = operation;
                        putmeAudit.Action = action;
                        putmeAudit.Time = DateTime.Now;
                        putmeAudit.Client = client;

                        putmeLogic.Modify(putme, putmeAudit);
                        TempData["Message"] = "Record was successfully updated";

                        
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "System Message :" + ex.Message;
                return RedirectToAction("index");
            }

           return RedirectToAction("index");
        }

        public ActionResult CorrectExamNumber()
        {
            PostjambResultSupportViewModel viewModel = null;
            try
            {
                viewModel = new PostjambResultSupportViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel); 
        }
        [HttpPost]
        public ActionResult CorrectExamNumber(PostjambResultSupportViewModel viewModel)
        {
            try
            {
                if (viewModel.ExamNumber != null)
                {
                    List<PutmeResult> results = new List<PutmeResult>();
                    PutmeResultLogic PostUtmeResultLogic = new PutmeResultLogic();
                    results = PostUtmeResultLogic.GetModelsBy(m => m.EXAMNO.Trim().Replace(" ", "") == viewModel.ExamNumber.Trim().Replace(" ", "") && m.Session_Id == 7);
                    viewModel.PutmeResults = results;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult EditExamNumber(long rid)
        {
            PostjambResultSupportViewModel viewModel = new PostjambResultSupportViewModel();
            try
            {
                if (rid > 0)
                {
                    PutmeResult result = new PutmeResult();
                    PutmeResultLogic PostUtmeResultLogic = new PutmeResultLogic();
                    result = PostUtmeResultLogic.GetModelBy(m => m.ID == rid);

                    viewModel.putmeResult = result;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditExamNumber(PostjambResultSupportViewModel viewModel)
        {
            try
            {
                if (viewModel.putmeResult != null && viewModel.putmeResult.Id > 0)
                {
                    PutmeResult putme = new PutmeResult();
                    PutmeResultLogic putmeLogic = new PutmeResultLogic();
                    putme = viewModel.putmeResult;

                    string operation = "UPDATE";
                    string action = "MODIFY APPLICANT JAMB RESULT";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                    PutmeResultAudit putmeAudit = new PutmeResultAudit();
                    UserLogic loggeduser = new UserLogic();
                    putmeAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                    putmeAudit.Operation = operation;
                    putmeAudit.Action = action;
                    putmeAudit.Time = DateTime.Now;
                    putmeAudit.Client = client;

                    putmeLogic.Modify(putme, putmeAudit);
                    SetMessage("Operation Successful! ", Message.Category.Information); 
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("CorrectExamNumber");
        }
        public JsonResult AutoCompleteSearch(string term)
        {
            try
            {
                PutmeResultLogic putmeLogic = new PutmeResultLogic();

                List<string> searchResult = new List<string>();

                List<PUTME_RESULT> results = putmeLogic.GetEntitiesBy(i => i.EXAMNO.Contains(term));
                foreach (PUTME_RESULT result in results)
                {
                    searchResult.Add(result.EXAMNO.ToUpper() + ", " + result.FULLNAME.ToUpper());
                }

                return Json(searchResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}