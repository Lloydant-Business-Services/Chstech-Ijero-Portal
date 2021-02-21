using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Areas.Admin.ViewModels;


namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class PaymentRegistrationReportController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        public ActionResult Payment()
        {
            try
            {
                List<SelectListItem> FeeTypeList = Utility.PopulateFeeTypeSelectListItem();
                ViewBag.FeeType = new SelectList(FeeTypeList, "Id", "Name");
            }
            catch (Exception ex)
            {                
                SetMessage("Error: " + ex.Message, Message.Category.Error);
            }

            return View();
        }
        public ActionResult PaymentReport(FeeType model)
        {
            ViewBag.FeeType = model.Id;
            return View();
        }
        public ActionResult BursaryPaymentReport()
        {
            UserLogic userLogic = new UserLogic();
            User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
            if(user!=null && user.Role!=null && user.Role.Id > 0)
            {
                if(user.Role.Id == 26)
                {
                    ViewBag.ProgrammeId = Utility.PopulatePartTimeProgrammeSelectListItem();
                }
                else if (user.Role.Id == 27)
                {
                    ViewBag.ProgrammeId = Utility.PopulateFullTimeProgrammeSelectListItem();
                }
                else
                {
                    ViewBag.ProgrammeId = Utility.PopulateAllProgrammeSelectListItem();
                }
            }
            
            ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
            return View();
        }
        public JsonResult GetPaymentSummary(string gatewayString, string dateFromString, string dateToString, int programme, int dept )
        {
            PaymentJsonResult singleResult = new PaymentJsonResult();
            List<PaymentJsonResult> result = new List<PaymentJsonResult>();
            try
            {
                if (!string.IsNullOrEmpty(gatewayString) && !string.IsNullOrEmpty(dateFromString) && !string.IsNullOrEmpty(dateToString))
                {
                    DateTime DateFrom = new DateTime();
                    DateTime DateTo = new DateTime();

                    if (!DateTime.TryParse(dateFromString, out DateFrom))
                        DateFrom = DateTime.Now;
                    if (!DateTime.TryParse(dateToString, out DateTo))
                        DateTo = DateTime.Now;

                    DateTo = DateTo.AddHours(23.99);

                    PaymentLogic paymentLogic = new PaymentLogic();
                    ProgrammeLogic programmeLogic = new ProgrammeLogic();
                    UserLogic userLogic = new UserLogic();
                    List<int> programmeIdList = new List<int>();
                    List<PaymentSummary> paymentSummary = new List<PaymentSummary>();
                    User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                    if (user != null && user.Role != null && user.Role.Id > 0)
                    {
                        if (user.Role.Id == 27)
                        {
                            programmeIdList = programmeLogic.GetModelsBy(p => p.Activated == true && (!p.Programme_Name.Contains("Part Time") && !p.Programme_Name.Contains("Evening"))).Select(f=>f.Id).ToList();
                            paymentSummary = paymentLogic.GetPaymentSummary(DateFrom, DateTo, programmeIdList);
                        }
                        else if (user.Role.Id == 26)
                        {
                            programmeIdList = programmeLogic.GetModelsBy(p => p.Activated == true && (p.Programme_Name.Contains("Part Time") || p.Programme_Name.Contains("Evening"))).Select(f => f.Id).ToList();
                            paymentSummary = paymentLogic.GetPaymentSummary(DateFrom, DateTo, programmeIdList);
                        }
                        else
                        {
                            programmeIdList = programmeLogic.GetAll().Select(f => f.Id).ToList();
                            paymentSummary = paymentLogic.GetPaymentSummary(DateFrom, DateTo, programmeIdList);
                        }
                    }
                    
                    if (paymentSummary != null && paymentSummary.Count > 0)
                    {
                        if (gatewayString == "Etranzact")
                        {
                            paymentSummary = paymentSummary.Where(p => p.RRR == null && p.PaymentEtranzactId != null).ToList();
                            if (programme > 0 && dept==0)
                            {
                                paymentSummary = paymentSummary.Where(p => p.RRR == null && p.PaymentEtranzactId != null && p.ProgrammeId == programme).ToList();
                            }
                            if (programme>0 && dept > 0)
                            {
                                paymentSummary = paymentSummary.Where(p => p.RRR == null && p.PaymentEtranzactId != null && p.ProgrammeId==programme && p.DepartmentId==dept).ToList();
                            }
                        }
                        else
                        {
                            paymentSummary = paymentSummary.Where(p => p.RRR != null).ToList();
                            if (programme > 0 && dept == 0)
                            {
                                paymentSummary = paymentSummary.Where(p => p.RRR != null && p.ProgrammeId == programme).ToList();
                            }
                            if (programme > 0 && dept > 0)
                            {
                                paymentSummary = paymentSummary.Where(p => p.RRR != null && p.ProgrammeId == programme && p.DepartmentId == dept).ToList();
                            }
                        }
                        var uniquePaymentSummary=ReturnUniquePayment(paymentSummary);
                        decimal overallAmount = Convert.ToDecimal(uniquePaymentSummary.Sum(p => p.TransactionAmount));

                        List<int> distinctFeeTypeId = uniquePaymentSummary.Select(p => p.FeeTypeId).Distinct().ToList();
                        for (int i = 0; i < distinctFeeTypeId.Count; i++)
                        {
                            int currentFeeTypeId = distinctFeeTypeId[i];

                            List<PaymentSummary> feeTypeSummary = uniquePaymentSummary.Where(p => p.FeeTypeId == currentFeeTypeId).ToList();
                            decimal feeTypeTotalAmount = Convert.ToDecimal(feeTypeSummary.Sum(p => p.TransactionAmount));

                            PaymentJsonResult paymentJsonResult = new PaymentJsonResult();
                            paymentJsonResult.FeeTypeId = currentFeeTypeId;
                            paymentJsonResult.FeeTypeName = feeTypeSummary.FirstOrDefault().FeeTypeName;
                            paymentJsonResult.TotalAmount = String.Format("{0:N}", feeTypeTotalAmount);
                            paymentJsonResult.TotalCount = feeTypeSummary.Count();
                            paymentJsonResult.IsError = false;
                            paymentJsonResult.OverallAmount = String.Format("{0:N}", overallAmount);

                            result.Add(paymentJsonResult);
                        }
                    }

                    if (paymentSummary == null || paymentSummary.Count <= 0)
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "No records found! ";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    singleResult.IsError = true;
                    singleResult.Message = "Invalid parametr! ";
                }
            }
            catch (Exception ex)
            {
                singleResult.IsError = true;
                singleResult.Message = "Error! " + ex.Message;
            }

            return Json(singleResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPaymentBreakdown(string gatewayString, string dateFromString, string dateToString, int feeTypeId, int programme, int dept)
        {
            PaymentJsonResult singleResult = new PaymentJsonResult();
            List<PaymentJsonResult> result = new List<PaymentJsonResult>();
            try
            {
                if (!string.IsNullOrEmpty(gatewayString) && !string.IsNullOrEmpty(dateFromString) && !string.IsNullOrEmpty(dateToString) && feeTypeId > 0)
                {
                    DateTime DateFrom = new DateTime();
                    DateTime DateTo = new DateTime();

                    if (!DateTime.TryParse(dateFromString, out DateFrom))
                        DateFrom = DateTime.Now;
                    if (!DateTime.TryParse(dateToString, out DateTo))
                        DateTo = DateTime.Now;

                    DateTo = DateTo.AddHours(23.99);

                    PaymentLogic paymentLogic = new PaymentLogic();
                    ProgrammeLogic programmeLogic = new ProgrammeLogic();
                    UserLogic userLogic = new UserLogic();
                    List<int> programmeIdList = new List<int>();
                    List<PaymentSummary> paymentSummary = new List<PaymentSummary>();
                    User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                    if (user != null && user.Role != null && user.Role.Id > 0)
                    {
                        if (user.Role.Id == 27)
                        {
                            programmeIdList = programmeLogic.GetModelsBy(p => p.Activated == true && (!p.Programme_Name.Contains("Part Time") && !p.Programme_Name.Contains("Evening"))).Select(f => f.Id).ToList();
                            paymentSummary = paymentLogic.GetPaymentSummary(DateFrom, DateTo, programmeIdList);
                        }
                        else if (user.Role.Id == 26)
                        {
                            programmeIdList = programmeLogic.GetModelsBy(p => p.Activated == true && (p.Programme_Name.Contains("Part Time") || p.Programme_Name.Contains("Evening"))).Select(f => f.Id).ToList();
                            paymentSummary = paymentLogic.GetPaymentSummary(DateFrom, DateTo, programmeIdList);
                        }
                        else
                        {
                            programmeIdList = programmeLogic.GetAll().Select(f=>f.Id).ToList();
                            paymentSummary = paymentLogic.GetPaymentSummary(DateFrom, DateTo, programmeIdList);
                        }
                    }
                    if (paymentSummary != null && paymentSummary.Count > 0)
                    {
                        if (gatewayString == "Etranzact")
                        {
                            paymentSummary = paymentSummary.Where(p => p.RRR == null && p.PaymentEtranzactId != null && p.FeeTypeId == feeTypeId).ToList();
                            if (programme > 0 && dept == 0)
                            {
                                paymentSummary = paymentSummary.Where(p => p.RRR == null && p.PaymentEtranzactId != null && p.ProgrammeId == programme && p.FeeTypeId == feeTypeId).ToList();
                            }
                            if (programme > 0 && dept > 0)
                            {
                                paymentSummary = paymentSummary.Where(p => p.RRR == null && p.PaymentEtranzactId != null && p.ProgrammeId == programme && p.DepartmentId == dept && p.FeeTypeId == feeTypeId).ToList();
                            }
                        }
                        else
                        {
                            paymentSummary = paymentSummary.Where(p => p.RRR != null && p.FeeTypeId == feeTypeId).ToList();
                            if (programme > 0 && dept == 0)
                            {
                                paymentSummary = paymentSummary.Where(p => p.RRR != null && p.ProgrammeId == programme && p.FeeTypeId == feeTypeId).ToList();
                            }
                            if (programme > 0 && dept > 0)
                            {
                                paymentSummary = paymentSummary.Where(p => p.RRR != null && p.ProgrammeId == programme && p.DepartmentId == dept && p.FeeTypeId == feeTypeId).ToList();
                            }

                        }
                        var uniquePaymentSummary = ReturnUniquePayment(paymentSummary);

                        PaymentJsonResult paymentJsonResult;
                        for (int i = 0; i < uniquePaymentSummary.Count; i++)
                        {
                            paymentJsonResult = new PaymentJsonResult();
                            paymentJsonResult.TransactionDate = Convert.ToDateTime(uniquePaymentSummary[i].TransactionDate).ToString("dd/MM/yyyy/ hh:mm tt");
                            paymentJsonResult.MatricNumber = uniquePaymentSummary[i].MatricNumber;
                            paymentJsonResult.Name = uniquePaymentSummary[i].Name;
                            paymentJsonResult.Level = uniquePaymentSummary[i].LevelName;
                            paymentJsonResult.Department = uniquePaymentSummary[i].DepartmentName;
                            paymentJsonResult.Faculty = uniquePaymentSummary[i].FacultyName;
                            paymentJsonResult.Programme = uniquePaymentSummary[i].ProgrammeName;
                            paymentJsonResult.Session = uniquePaymentSummary[i].SessionName;
                            paymentJsonResult.InvoiceNumber = uniquePaymentSummary[i].InvoiceNumber;
                            paymentJsonResult.ConfirmationNumber = uniquePaymentSummary[i].ConfirmationNumber;
                            paymentJsonResult.FeeTypeName = uniquePaymentSummary[i].FeeTypeName;
                            paymentJsonResult.Amount = String.Format("{0:N}", uniquePaymentSummary[i].TransactionAmount);

                            result.Add(paymentJsonResult);
                        }
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    singleResult.IsError = true;
                    singleResult.Message = "Invalid parametr! ";
                }
            }
            catch (Exception ex)
            {
                singleResult.IsError = true;
                singleResult.Message = "Error! " + ex.Message;
            }

            return Json(singleResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartments(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };
                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<PaymentSummary> ReturnUniquePayment(List<PaymentSummary> list)
        {
            List<PaymentSummary> returnPayment = new List<PaymentSummary>();
            try
            {
                var uniqueInvoiceNumber=list.GroupBy(g => g.InvoiceNumber).ToList();
                for(int i=0; i<uniqueInvoiceNumber.Count; i++)
                {
                    var ivn = uniqueInvoiceNumber[i].Key;
                    var payment=list.Where(p => p.InvoiceNumber == ivn).LastOrDefault();
                    returnPayment.Add(payment);

                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return returnPayment;
        }
    }
}