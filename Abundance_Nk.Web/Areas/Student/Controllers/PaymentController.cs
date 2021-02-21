using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Transactions;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using System.Configuration;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Web.Areas.Common.Controllers;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
	[AllowAnonymous]
	public class PaymentController : BaseController
	{
		private PersonLogic personLogic;
		private PaymentLogic paymentLogic;
		private OnlinePaymentLogic onlinePaymentLogic;
		private StudentLevelLogic studentLevelLogic;
		private StudentLogic studentLogic;

		private PaymentViewModel viewModel;

		public PaymentController()
		{
			personLogic = new PersonLogic();
			paymentLogic = new PaymentLogic();
			onlinePaymentLogic = new OnlinePaymentLogic();
			studentLevelLogic = new StudentLevelLogic();
			studentLogic = new StudentLogic();

			viewModel = new PaymentViewModel();
		}

		public ActionResult Index(long sid)
		{
			try
			{
				SetFeeTypeDropDown(viewModel);
			    viewModel.Student = studentLogic.GetBy(sid);
                TempData["StudentMatricNumber"] = viewModel.Student.MatricNumber;
                TempData["StudentId"] = viewModel.Student.Id;
            }
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(viewModel);
		}

		private void SetFeeTypeDropDown(PaymentViewModel viewModel)
		{
			try
			{
				if (viewModel.FeeTypeSelectListItem != null && viewModel.FeeTypeSelectListItem.Count > 0)
				{
					viewModel.FeeType.Id = (int)FeeTypes.SchoolFees;
					ViewBag.FeeTypes = new SelectList(viewModel.FeeTypeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.FeeType.Id);
				}
				else
				{
					ViewBag.FeeTypes = new SelectList(new List<FeeType>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		[HttpPost]
		public ActionResult Index(PaymentViewModel viewModel)
		{
			try
			{
                var studentMatNo = TempData["StudentMatricNumber"] as string;
                var sid = TempData["StudentId"];
                var studentId = Convert.ToInt64(sid);
                viewModel.Student = studentLogic.GetBy(studentId);


                if (InvalidMatricNumber(viewModel.Student.MatricNumber))
				{
					SetFeeTypeDropDown(viewModel);
					return View(viewModel);
				}
				
				Model.Model.Student student = studentLogic.GetBy(viewModel.Student.MatricNumber);
				if (student != null && student.Id > 0)
				{
					return RedirectToAction("GenerateInvoice", "Payment", new { sid = student.Id });
				}
				else
				{
					return RedirectToAction("GenerateInvoice", "Payment", new { sid = 0 });
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			SetFeeTypeDropDown(viewModel);
			return View(viewModel);
		}



        public ActionResult OtherFees(long sid)
        {
            try
            {
                SetOtherFeeTypeDropDown(viewModel);
                viewModel.Student = studentLogic.GetBy(sid);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        private void SetOtherFeeTypeDropDown(PaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.OtherFeeTypeSelectListItem != null && viewModel.OtherFeeTypeSelectListItem.Count > 0)
                {
                    viewModel.FeeType.Id = (int)FeeTypes.LateSchoolFees;
                    ViewBag.FeeTypes = new SelectList(viewModel.OtherFeeTypeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.FeeType.Id);
                }
                else
                {
                    ViewBag.FeeTypes = new SelectList(new List<FeeType>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        [HttpPost]
        public ActionResult OtherFees(PaymentViewModel viewModel)
        {
            try
            {

                if (InvalidMatricNumber(viewModel.Student.MatricNumber))
                {
                    SetOtherFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }

                Model.Model.Student student = studentLogic.GetBy(viewModel.Student.MatricNumber);
                if (student != null && student.Id > 0)
                {
                    return RedirectToAction("GenerateOtherInvoice", "Payment", new { sid = student.Id });
                }
                else
                {
                    return RedirectToAction("GenerateOtherInvoice", "Payment", new { sid = 0 });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            SetOtherFeeTypeDropDown(viewModel);
            return View(viewModel);
        }
        public ActionResult GenerateOtherInvoice(long sid)
        {
            try
            {
                viewModel.FeeType = new FeeType() { Id = (int)FeeTypes.LateSchoolFees };
                viewModel.PaymentType = new PaymentType() { Id = 2 };

                ViewBag.States = viewModel.StateSelectListItem;
                ViewBag.Sessions = viewModel.SessionSelectListItem;
                ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
                ViewBag.PaymentModes = viewModel.PaymentModeSelectListItem;
                ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);

                if (sid > 0)
                {
                    viewModel.StudentAlreadyExist = true;
                    viewModel.Person = personLogic.GetModelBy(p => p.Person_Id == sid);
                    viewModel.Student = studentLogic.GetModelBy(s => s.Person_Id == sid);
                    viewModel.StudentLevel = studentLevelLogic.GetBy(sid);
                    if (viewModel.StudentLevel != null && viewModel.StudentLevel.Programme.Id > 0)
                    {
                        ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Programme.Id);
                    }
                    viewModel.LevelSelectListItem = Utility.PopulateLevelSelectListItem();
                    ViewBag.Levels = viewModel.LevelSelectListItem;

                    if (viewModel.Person != null && viewModel.Person.Id > 0)
                    {
                        if (viewModel.Person.State != null && !string.IsNullOrWhiteSpace(viewModel.Person.State.Id))
                        {
                            ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Person.State.Id);
                        }
                    }

                    if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0)
                    {
                        if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
                        {
                            //ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Level.Id);
                            //Commented because students weren't confirming level before generating invoice
                            ViewBag.Levels = viewModel.LevelSelectListItem;
                        }
                    }

                    SetDepartmentIfExist(viewModel);
                    SetDepartmentOptionIfExist(viewModel);
                }
                else
                {
                    ViewBag.Levels = new SelectList(new List<Level>(), Utility.ID, Utility.NAME);
                    //ViewBag.Sessions = Utility.PopulateSessionSelectListItem();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GenerateOtherInvoice(PaymentViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("Student.MobilePhone");
                ModelState.Remove("Student.SchoolContactAddress");
                ModelState.Remove("FeeType.Name");
                ModelState.Remove("Student.Email");

                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                var errors = from modelstate in ModelState.AsQueryable().Where(f => f.Value.Errors.Count > 0) select new { Title = modelstate.Key };

                if (ModelState.IsValid)
                {
                    if (InvalidDepartmentSelection(viewModel))
                    {
                        KeepInvoiceGenerationDropDownState(viewModel);
                        return View(viewModel);
                    }

                    if (InvalidMatricNumber(viewModel.Student.MatricNumber))
                    {
                        KeepInvoiceGenerationDropDownState(viewModel);
                        return View(viewModel);
                    }

                  

                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                    decimal Amt = 0M;
                    Amt = feeDetailLogic.GetFeeByDepartmentLevel(viewModel.StudentLevel.Department,
                                            viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme, viewModel.FeeType,
                                            viewModel.Session, viewModel.PaymentMode);

                  
                    Payment payment = null;
                    if (viewModel.StudentAlreadyExist == false)
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            CreatePerson(viewModel);
                            CreateStudent(viewModel);
                            payment = CreatePayment(viewModel);
                            CreateStudentLevel(viewModel);

                            if (payment != null)
                            {
                                Amt = payment.FeeDetails.Sum(p => p.Fee.Amount);

                                //Get Payment Specific Setting
                                RemitaSettings settings = new RemitaSettings();
                                RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                settings = settingsLogic.GetBy(1);

                             
                                
                                //Get BaseURL
                                string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                //string remitaBaseUrl = "http://www.remitademo.net/remita/ecomm/split/init.reg";
                                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                                viewModel.RemitaPayment = remitaProcessor.GenerateRRRCard(payment.InvoiceNumber, remitaBaseUrl, "LATE SCHOOL FEES",  settings, Amt);

                                viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                                if (viewModel.RemitaPayment != null)
                                {
                                    transaction.Complete();
                                }

                            }


                        }
                    }
                    else
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {

                            personLogic.Modify(viewModel.Person);


                            FeeType feeType = new FeeType() { Id = (int)FeeTypes.LateSchoolFees };
                            payment = paymentLogic.GetBy(feeType, viewModel.Person, viewModel.Session, viewModel.PaymentMode);

                            if (payment == null || payment.Id <= 0)
                            {
                                payment = CreatePayment(viewModel);
                            }
                            else if (payment.PaymentMode != null)
                            {
                              
                                    payment.FeeDetails = paymentLogic.SetFeeDetails(payment, viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id, payment.PaymentMode.Id, viewModel.StudentLevel.Department.Id, viewModel.Session.Id);
                                
                            }


                            RemitaPayment remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                            if (remitaPayment == null && Amt > 0M)
                            {
                                //Get Payment Specific Setting
                                RemitaSettings settings = new RemitaSettings();
                                RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                settings = settingsLogic.GetBy(1);
                                
                                //Get BaseURL
                                string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);

                                viewModel.RemitaPayment = remitaProcessor.GenerateRRRCard(payment.InvoiceNumber, remitaBaseUrl, "LATE SCHOOL FEES", settings, Amt);

                                viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                                if (viewModel.RemitaPayment != null)
                                {
                                    transaction.Complete();
                                }
                            }
                            else
                            {
                                viewModel.RemitaPayment = remitaPayment;

                                RemitaSettings settings = new RemitaSettings();
                                RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                settings = settingsLogic.GetBy(1);
                                viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                                transaction.Complete();
                            }
                        }
                    }
                    
                    TempData["PaymentViewModel"] = viewModel;
                    return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Abundance_Nk.Web.Models.Utility.Encrypt(payment.Id.ToString()), });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            KeepInvoiceGenerationDropDownState(viewModel);
            return View(viewModel);
        }
        
        public ActionResult GenerateLateFeeInvoiceApplicant(long applicationFormId)
        {
            try
            {

                if (applicationFormId > 0)
                {
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    var admissionList = admissionListLogic.GetBy(applicationFormId);
                    if (admissionList != null)
                    {
                        RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                        FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                        decimal Amt = 0M;

                        Level level = new Level() { Id = GetLevel(admissionList.Programme.Id) };
                        FeeType feeType = new FeeType() { Id = (int)FeeTypes.LateSchoolFees };
                        PaymentType paymentType = new PaymentType() { Id = (int)PaymentTypeEnum.OnlinePayment };
                        PaymentMode paymentMode = new PaymentMode() { Id = (int)PaymentModes.Full };

                        Amt = feeDetailLogic.GetFeeByDepartmentLevel(admissionList.Deprtment,
                                                level, admissionList.Programme, feeType,
                                                admissionList.Session, paymentMode);


                        PaymentLogic paymentLogic = new PaymentLogic();
                        Payment payment = null;
                        var existingPayment = paymentLogic.GetModelBy(s => s.Person_Id == admissionList.Form.Person.Id && s.Session_Id == admissionList.Session.Id && s.Fee_Type_Id == feeType.Id);
                        if (existingPayment == null)
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                PaymentViewModel newPaymentViewModel = new PaymentViewModel();
                                newPaymentViewModel.Session = admissionList.Session;
                                newPaymentViewModel.FeeType = feeType;
                                newPaymentViewModel.PaymentMode = paymentMode;
                                newPaymentViewModel.Person = admissionList.Form.Person;
                                newPaymentViewModel.PaymentType = paymentType;
                                payment = CreateApplicantPayment(newPaymentViewModel);

                                transaction.Complete();
                            }
                        }
                        else
                        {
                            payment = existingPayment;
                        }


                        if (payment != null)
                        {
                            RemitaPayment remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                            
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                if (remitaPayment == null && Amt > 0M)
                                {
                                    //Get Payment Specific Setting
                                    RemitaSettings settings = new RemitaSettings();
                                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                    settings = settingsLogic.GetBy(1);
                                    
                                    //Get BaseURL
                                    string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                    //string remitaBaseUrl = "http://www.remitademo.net/remita/ecomm/split/init.reg";
                                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                                    viewModel.RemitaPayment = remitaProcessor.GenerateRRRCard(payment.InvoiceNumber, remitaBaseUrl, "LATE SCHOOL FEES", settings, Amt);

                                    viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                                    if (viewModel.RemitaPayment != null)
                                    {
                                        transaction.Complete();
                                    }
                                }
                                else
                                {
                                    viewModel.RemitaPayment = remitaPayment;

                                    RemitaSettings settings = new RemitaSettings();
                                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                    settings = settingsLogic.GetBy(1);
                                    viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                                    transaction.Complete();
                                }
                            }

                            return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Abundance_Nk.Web.Models.Utility.Encrypt(payment.Id.ToString()) });
                        }
                        else
                        {
                            SetMessage("Error! Invalid Applicant Details Supplied", Message.Category.Error);
                            return View();
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();

        }


        private Int32 GetLevel(int ProgrammeId)
        {
            try
            {
                //set mode of study
                switch (ProgrammeId)
                {
                    case 1:
                        {
                            return 1;
                        }
                    case 2:
                        {
                            return 5;
                        }
                    case 3:
                        {
                            return 3;
                        }
                    case 4:
                        {
                            return 8;
                        }
                    case 5:
                        {
                            return 11;
                        }
                    case 6:
                        {
                            return 13;
                        }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return 0;
        }

        public ActionResult GenerateInvoice(long sid)
		{
			try
			{
				viewModel.FeeType = new FeeType() { Id = (int)FeeTypes.SchoolFees };
				viewModel.PaymentType = new PaymentType() { Id = 2 };

				ViewBag.States = viewModel.StateSelectListItem;
				ViewBag.Sessions = viewModel.SessionSelectListItem;
                ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
			    ViewBag.PaymentModes = viewModel.PaymentModeSelectListItem;
				ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
				ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);

				if (sid > 0)
				{
					viewModel.StudentAlreadyExist = true;
					viewModel.Person = personLogic.GetModelBy(p => p.Person_Id == sid);
					viewModel.Student = studentLogic.GetModelBy(s => s.Person_Id == sid);
					viewModel.StudentLevel = studentLevelLogic.GetBy(sid);
					if (viewModel.StudentLevel != null && viewModel.StudentLevel.Programme.Id > 0)
					{
						ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Programme.Id);
					}
					viewModel.LevelSelectListItem = Utility.PopulateLevelSelectListItem();
					ViewBag.Levels = viewModel.LevelSelectListItem;

					if (viewModel.Person != null && viewModel.Person.Id > 0)
					{
						if (viewModel.Person.State != null && !string.IsNullOrWhiteSpace(viewModel.Person.State.Id))
						{
							ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Person.State.Id);
						}
					}

					if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0)
					{
						if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
						{
							//ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Level.Id);
							//Commented because students weren't confirming level before generating invoice
							ViewBag.Levels = viewModel.LevelSelectListItem;
						}
					}

					SetDepartmentIfExist(viewModel);
					SetDepartmentOptionIfExist(viewModel);
				}
				else
				{
					ViewBag.Levels = new SelectList(new List<Level>(), Utility.ID, Utility.NAME);
					//ViewBag.Sessions = Utility.PopulateSessionSelectListItem();
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(viewModel);
		}

		private void SetDepartmentIfExist(PaymentViewModel viewModel)
		{
			try
			{
				if (viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
				{
					ProgrammeDepartmentLogic departmentLogic = new ProgrammeDepartmentLogic();
					List<Department> departments = departmentLogic.GetBy(viewModel.StudentLevel.Programme);
					if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
					{
						ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME, viewModel.StudentLevel.Department.Id);
					}
					else
					{
						ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME);
					}
				}
				else
				{
					ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetDepartmentOptionIfExist(PaymentViewModel viewModel)
		{
			try
			{
				if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0 && (viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 5 || viewModel.StudentLevel.Programme.Id == 4))
				{
					DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
					List<DepartmentOption> departmentOptions = departmentOptionLogic.GetModelsBy(l => l.Department_Id == viewModel.StudentLevel.Department.Id);
					if (viewModel.StudentLevel.DepartmentOption != null && viewModel.StudentLevel.DepartmentOption.Id > 0)
					{
						ViewBag.DepartmentOptions = new SelectList(departmentOptions, Utility.ID, Utility.NAME, viewModel.StudentLevel.DepartmentOption.Id);
					}
					else
					{
						ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
					}
				}
				else
				{
					ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

        [HttpPost]
        public ActionResult GenerateInvoice(PaymentViewModel viewModel)
        {
            try
            {
               

                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("Student.MobilePhone");
                ModelState.Remove("Student.SchoolContactAddress");
                ModelState.Remove("FeeType.Name");
                ModelState.Remove("Student.Email");

                Payment payment = new Payment();
                RemitaPayment remitaPayment = new RemitaPayment();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                FeeDetailLogic _feeDetailLogic = new FeeDetailLogic();
                payment = paymentLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id && p.Fee_Type_Id == viewModel.FeeType.Id).LastOrDefault();

                if(payment != null)
                {
                    remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == payment.Id);
                    var getStudentLevel = studentLevelLogic.GetBy(payment.Person.Id);



                    CredentialController credentialController = new CredentialController();
                    decimal amountToPay = _feeDetailLogic.GetFeeByDepartmentLevel(viewModel.StudentLevel.Department,
                                                viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme, viewModel.FeeType,
                                                payment.Session, viewModel.PaymentMode);

                    //Check that School fees payment for previous session has been fully made
                    var checkPreviousPayment = HasPaidSchoolFees(payment, viewModel.Session, amountToPay);
                    if (!checkPreviousPayment && (viewModel.StudentLevel.Level.Id != 1 || viewModel.StudentLevel.Level.Id != 3 || viewModel.StudentLevel.Level.Id != 5 || viewModel.StudentLevel.Level.Id != 8 || viewModel.StudentLevel.Level.Id != 11 || viewModel.StudentLevel.Level.Id != 13))
                    {
                        var msg = "You are not allowed to generate invoice for the selected session without completing payment for previous session";
                        SetMessage(msg, Message.Category.Warning);
                        KeepInvoiceGenerationDropDownState(viewModel);
                        return RedirectToAction("GenerateInvoice", "Payment", new { area = "Student", sid = getStudentLevel.Student.Id });

                    }
                }
  
                //end
                var errors = from modelstate in ModelState.AsQueryable().Where(f => f.Value.Errors.Count > 0) select new { Title = modelstate.Key };

				if (ModelState.IsValid)
				{
					if (InvalidDepartmentSelection(viewModel))
					{
						KeepInvoiceGenerationDropDownState(viewModel);
						return View(viewModel);
					}

					if (InvalidMatricNumber(viewModel.Student.MatricNumber))
					{
						KeepInvoiceGenerationDropDownState(viewModel);
						return View(viewModel);
					}

                    //check for e-wallet payment
                    EWalletPaymentLogic walletPaymentLogic = new EWalletPaymentLogic();
                    List<Payment> otherPaymentOptions = paymentLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id && p.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.Session_Id == viewModel.Session.Id);
                    for (int i = 0; i < otherPaymentOptions.Count; i++)
                    {
                        long currentPaymentId = otherPaymentOptions[i].Id;

                        EWalletPayment walletPayment = walletPaymentLogic.GetModelsBy(p => p.Payment_Id == currentPaymentId).LastOrDefault();
                        if (walletPayment != null)
                        {
                            SetMessage("You have already generated invoice for E-Wallet deposit, hence cannot use any other payment option!", Message.Category.Error);
                            KeepInvoiceGenerationDropDownState(viewModel);
                            return RedirectToAction("GenerateInvoice", "Payment", new { area = "Student", sid = viewModel.Person.Id });
                        }
                    }

                    //check late registration
                    SessionLogic sessionLogic = new SessionLogic();
                    Session lateRegSession = sessionLogic.GetModelBy(s => s.Session_Id == viewModel.Session.Id);
                    if (lateRegSession != null && lateRegSession.IsLateRegistration == true)
                    {
                        //check late reg payment
                        RemitaPayment remitaPaymentLateReg = remitaPaymentLogic.GetModelsBy(r => r.PAYMENT.Person_Id == viewModel.Person.Id && r.PAYMENT.Session_Id == viewModel.Session.Id &&
                                                                r.PAYMENT.Fee_Type_Id == (int)FeeTypes.LateSchoolFees && (r.Status.Contains("01") || r.Description.Contains("manual"))).LastOrDefault();

                        if (remitaPaymentLateReg == null)
                        {
                            SetMessage("Pay late registration before proceeding.", Message.Category.Information);
                            return RedirectToAction("OtherFees", "Payment", new { Area = "Student", sid = viewModel.Person.Id });
                        }
                    }

                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
					decimal Amt = 0M;
					Amt = feeDetailLogic.GetFeeByDepartmentLevel(viewModel.StudentLevel.Department,
											viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme, viewModel.FeeType,
											viewModel.Session, viewModel.PaymentMode);

				    if (viewModel.PaymentMode.Id == (int)PaymentModes.ThirdInstallment || viewModel.PaymentMode.Id == (int)PaymentModes.FourthInstallment)
				    {
                        ProgrammeFeeAmountLogic feeAmountLogic = new ProgrammeFeeAmountLogic();
                        ProgrammeFeeAmount programmeFeeAmount = feeAmountLogic.GetModelsBy(f => f.Level_Id == viewModel.StudentLevel.Level.Id && f.Payment_Mode_Id == viewModel.PaymentMode.Id && 
                                                                f.Programme_Id == viewModel.StudentLevel.Programme.Id && f.Session_Id == viewModel.Session.Id).LastOrDefault();

                        if (programmeFeeAmount != null)
                        {
                            Amt = programmeFeeAmount.Amount;
                        }
				    }
				    
					//Payment payment = null;
					if (viewModel.StudentAlreadyExist == false)
					{
						using (TransactionScope transaction = new TransactionScope())
						{
							CreatePerson(viewModel);
							CreateStudent(viewModel);
							payment = CreatePayment(viewModel);
							CreateStudentLevel(viewModel);

							if (payment != null)
							{
                                Amt = payment.FeeDetails.Sum(p => p.Fee.Amount);

                                //Get Payment Specific Setting
                                RemitaSettings settings = new RemitaSettings();
                                RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                settings = settingsLogic.GetBy(1);

                                decimal lloydantCommission = 0M;

                                //Get Split Specific details;
                                List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                                RemitaSplitItems singleItem = new RemitaSplitItems();
                                RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "1";

                                lloydantCommission = Convert.ToDecimal(singleItem.beneficiaryAmount);

                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(2);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - lloydantCommission);
                                splitItems.Add(singleItem);

                                //Get BaseURL
                                string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                //string remitaBaseUrl = "http://www.remitademo.net/remita/ecomm/split/init.reg";
                                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                                viewModel.RemitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", splitItems, settings, Amt);

                                viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                                if (viewModel.RemitaPayment != null)
                                {
                                    transaction.Complete();
                                }
                               
							}

							
						}
					}
					else
					{
						 using (TransactionScope transaction = new TransactionScope())
						{
							
							personLogic.Modify(viewModel.Person);
                            var studentLevel=studentLevelLogic.GetModelsBy(f => f.Person_Id == viewModel.Person.Id && f.Level_Id == viewModel.StudentLevel.Level.Id && f.Session_Id == viewModel.Session.Id).LastOrDefault();
                            if (studentLevel == null)
                            {
                                viewModel.Student = new Model.Model.Student { Id = viewModel.Person.Id };
                                CreateStudentLevel(viewModel);
                            }
							
							
							FeeType feeType = new FeeType() { Id = (int)FeeTypes.SchoolFees };
							payment = paymentLogic.GetBy(feeType, viewModel.Person, viewModel.Session, viewModel.PaymentMode);

							if (payment == null || payment.Id <= 0)
							{
								payment = CreatePayment(viewModel);
							}
							else if (payment.PaymentMode != null)
							{
							    if (payment.PaymentMode.Id != viewModel.PaymentMode.Id)
							    {
                                    SpilloverStudentLogic spilloverStudentLogic = new SpilloverStudentLogic();
                                    SpilloverStudent spilloverStudent = spilloverStudentLogic.GetModelsBy(s => s.Session_Id == payment.Session.Id && s.Student_Id == payment.Person.Id).LastOrDefault();

                                    if (spilloverStudent != null)
							        {
                                        if (viewModel.PaymentMode.Id == (int)PaymentModes.FourthInstallment)
							            {
                                            Payment thirdInstallmentPayment = paymentLogic.GetModelsBy(p => p.Person_Id == payment.Person.Id && p.Session_Id == viewModel.Session.Id &&
                                                                            p.Payment_Mode_Id == (int)PaymentModes.ThirdInstallment).LastOrDefault();
                                            if (thirdInstallmentPayment != null)
                                            {
                                                RemitaPayment paymentEtranzact = remitaPaymentLogic.GetBy(thirdInstallmentPayment);
                                                if (paymentEtranzact != null && !paymentEtranzact.Status.Contains("01"))
                                                {
                                                    SetMessage(
                                                        "Please generate an invoice for third installment / make payment before generating for the fourth installment",
                                                        Message.Category.Error);
                                                    KeepInvoiceGenerationDropDownState(viewModel);
                                                    transaction.Dispose();
                                                    return View(viewModel);
                                                }

                                                Payment secondInstallmentPayment = paymentLogic.GetSecondInstallment(payment);
                                                if (secondInstallmentPayment == null)
                                                {
                                                    payment = CreatePayment(viewModel);
                                                }
                                            }
                                            else
                                            {
                                                SetMessage(
                                                    "Please generate an invoice for third installment / make payment before generating for the fourth installment",
                                                    Message.Category.Error);
                                                KeepInvoiceGenerationDropDownState(viewModel);
                                                transaction.Dispose();
                                                return View(viewModel);
                                            }
							            }
                                        else
                                        {
                                            Payment existingPayment = paymentLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id && p.Person_Id == viewModel.Person.Id && 
                                                                        p.Payment_Mode_Id == viewModel.PaymentMode.Id).LastOrDefault();
                                            if (existingPayment == null)
                                            {
                                                payment = CreatePayment(viewModel);
                                            }
                                        }
							        }
							        else if (viewModel.PaymentMode.Id == (int)PaymentModes.SecondInstallment)
							        {
							            Payment firstInstallmentPayment = paymentLogic.GetFirstInstallment(payment);
							            if (firstInstallmentPayment != null)
							            {
							                RemitaPayment paymentEtranzact = remitaPaymentLogic.GetBy(firstInstallmentPayment);
							                if (paymentEtranzact != null && !paymentEtranzact.Status.Contains("01"))
							                {
							                    SetMessage(
							                        "Please generate an invoice for first installment / make payment before generating for the second installment",
							                        Message.Category.Error);
							                    KeepInvoiceGenerationDropDownState(viewModel);
							                    transaction.Dispose();
							                    return View(viewModel);
							                }

							                Payment secondInstallmentPayment = paymentLogic.GetSecondInstallment(payment);
							                if (secondInstallmentPayment == null)
							                {
							                    payment = CreatePayment(viewModel);
							                }
							            }
							            else
							            {
							                SetMessage(
							                    "Please generate an invoice for first installment / make payment before generating for the second installment",
							                    Message.Category.Error);
							                KeepInvoiceGenerationDropDownState(viewModel);
							                transaction.Dispose();
							                return View(viewModel);
							            }
							        }
                                    else if (viewModel.PaymentMode.Id == (int)PaymentModes.ThirdInstallment)
                                    {
                                        Payment firstInstallmentPayment = paymentLogic.GetFirstInstallment(payment);
                                        if (firstInstallmentPayment != null)
                                        {
                                            RemitaPayment paymentEtranzact = remitaPaymentLogic.GetBy(firstInstallmentPayment);
                                            if (paymentEtranzact != null && !paymentEtranzact.Status.Contains("01"))
                                            {
                                                SetMessage(
                                                    "Please generate an invoice for first installment / make payment before generating for the third installment",
                                                    Message.Category.Error);
                                                KeepInvoiceGenerationDropDownState(viewModel);
                                                transaction.Dispose();
                                                return View(viewModel);
                                            }

                                            Payment thirdInstallmentPayment = paymentLogic.GetThirdInstallment(payment);
                                            if (thirdInstallmentPayment == null)
                                            {
                                                payment = CreatePayment(viewModel);
                                            }
                                        }
                                        else
                                        {
                                            SetMessage(
                                                "Please generate an invoice for first installment / make payment before generating for the third installment",
                                                Message.Category.Error);
                                            KeepInvoiceGenerationDropDownState(viewModel);
                                            transaction.Dispose();
                                            return View(viewModel);
                                        }
                                    }
                                    else if (viewModel.PaymentMode.Id == (int)PaymentModes.FourthInstallment)
                                    {
                                        Payment firstInstallmentPayment = paymentLogic.GetFirstInstallment(payment);
                                        if (firstInstallmentPayment != null)
                                        {
                                            RemitaPayment paymentEtranzact = remitaPaymentLogic.GetBy(firstInstallmentPayment);
                                            if (paymentEtranzact != null && !paymentEtranzact.Status.Contains("01"))
                                            {
                                                SetMessage(
                                                    "Please generate an invoice for first installment / make payment before generating for the third installment",
                                                    Message.Category.Error);
                                                KeepInvoiceGenerationDropDownState(viewModel);
                                                transaction.Dispose();
                                                return View(viewModel);
                                            }

                                            Payment thirdInstallmentPayment = paymentLogic.GetThirdInstallment(payment);
                                            if (thirdInstallmentPayment != null)
                                            {
                                                RemitaPayment paymentEtranzactCheck = remitaPaymentLogic.GetBy(thirdInstallmentPayment);
                                                if (paymentEtranzactCheck != null && !paymentEtranzactCheck.Status.Contains("01"))
                                                {
                                                    SetMessage(
                                                        "Please generate an invoice for third installment / make payment before generating for the third installment",
                                                        Message.Category.Error);
                                                    KeepInvoiceGenerationDropDownState(viewModel);
                                                    transaction.Dispose();
                                                    return View(viewModel);
                                                }

                                                Payment fouthInstallmentPayment = paymentLogic.GetThirdInstallment(payment);
                                                if (fouthInstallmentPayment == null)
                                                {
                                                    payment = CreatePayment(viewModel);
                                                }
                                            }
                                            else
                                            {
                                                SetMessage(
                                                    "Please generate an invoice for third installment / make payment before generating for the fourth installment",
                                                    Message.Category.Error);
                                                KeepInvoiceGenerationDropDownState(viewModel);
                                                transaction.Dispose();
                                                return View(viewModel);
                                            }
                                        }
                                        else
                                        {
                                            SetMessage(
                                                "Please generate an invoice for first installment / make payment before generating for the fourth installment",
                                                Message.Category.Error);
                                            KeepInvoiceGenerationDropDownState(viewModel);
                                            transaction.Dispose();
                                            return View(viewModel);
                                        }
                                    }
							        else
							        {
                                        RemitaPayment remitaPaymentStatus = remitaPaymentLogic.GetBy(payment);
                                        if (remitaPaymentStatus != null && (remitaPaymentStatus.Status.Contains("025") || remitaPaymentStatus.Status.Contains("028")))
							            {
							                payment.PaymentMode = viewModel.PaymentMode;
                                            payment = paymentLogic.Update(payment);
							                payment.FeeDetails = paymentLogic.SetFeeDetails(payment,viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id,payment.PaymentMode.Id, viewModel.StudentLevel.Department.Id,viewModel.Session.Id);

							            }
							        }
							    }
							    else
							    {
							        payment.FeeDetails = paymentLogic.SetFeeDetails(payment,viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id,payment.PaymentMode.Id, viewModel.StudentLevel.Department.Id,viewModel.Session.Id);
							    }
							}


                             remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                            if (remitaPayment == null && (viewModel.PaymentMode.Id == (int)PaymentModes.Full || viewModel.PaymentMode.Id == (int)PaymentModes.FirstInstallment))
						    {
						        //Amt = payment.FeeDetails.Sum(p => p.Fee.Amount);

						        //Get Payment Specific Setting
						        RemitaSettings settings = new RemitaSettings();
						        RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
						        settings = settingsLogic.GetBy(1);

						        //Get Split Specific details;
						        List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
						        RemitaSplitItems singleItem = new RemitaSplitItems();
						        RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                                if (viewModel.PaymentMode.Id == (int)PaymentModes.FirstInstallment && viewModel.Session.Id == 8)
						        {
						            singleItem = splitItemLogic.GetBy(2);
                                    singleItem.beneficiaryAmount = "1200.00";
						            singleItem.deductFeeFrom = "1";
						            splitItems.Add(singleItem);
						            singleItem = splitItemLogic.GetBy(2);
						            singleItem.deductFeeFrom = "0";
						            singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
						            splitItems.Add(singleItem);
						        }
						        else
						        {
						            singleItem = splitItemLogic.GetBy(1);
						            singleItem.deductFeeFrom = "1";
						            splitItems.Add(singleItem);
						            singleItem = splitItemLogic.GetBy(2);
						            singleItem.deductFeeFrom = "0";
						            singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
						            splitItems.Add(singleItem);
						        }
						      

						        //Get BaseURL
						        string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                                if (viewModel.PaymentMode.Id == (int)PaymentModes.FirstInstallment && viewModel.Session.Id == 8)
                                {
                                    viewModel.RemitaPayment = remitaProcessor.GenerateRRRCard(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", settings, Amt);
                                }
                                else
                                {
                                    viewModel.RemitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", splitItems, settings, Amt);
                                }
                                viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

						        if (viewModel.RemitaPayment != null)
						        {
						            transaction.Complete();
						        }
						    }
                            else if (remitaPayment != null && remitaPayment.payment.Id > 0 && payment.PaymentMode != null && payment.PaymentMode.Id != remitaPayment.payment.PaymentMode.Id)
                            {
                                //Amt = payment.FeeDetails.Sum(p => p.Fee.Amount);

                                //Get Payment Specific Setting
                                RemitaSettings settings = new RemitaSettings();
                                RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                settings = settingsLogic.GetBy(1);

                                //Get Split Specific details;
                                List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                                RemitaSplitItems singleItem = new RemitaSplitItems();
                                RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                                if (viewModel.PaymentMode.Id == (int)PaymentModes.FirstInstallment && viewModel.Session.Id == 8)
                                {
                                    singleItem = splitItemLogic.GetBy(2);
                                    singleItem.beneficiaryAmount = "1200";
                                    singleItem.deductFeeFrom = "1";
                                    splitItems.Add(singleItem);
                                    singleItem = splitItemLogic.GetBy(2);
                                    singleItem.deductFeeFrom = "0";
                                    singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                    splitItems.Add(singleItem);
                                }
                                else if (viewModel.PaymentMode.Id == (int)PaymentModes.SecondInstallment)
                                {
                                    singleItem = splitItemLogic.GetBy(1);
                                    singleItem.deductFeeFrom = "1";
                                    splitItems.Add(singleItem);
                                    singleItem = splitItemLogic.GetBy(2);
                                    singleItem.deductFeeFrom = "0";
                                    singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                    splitItems.Add(singleItem);
                                }
                                else if (viewModel.PaymentMode.Id == (int)PaymentModes.ThirdInstallment)
                                {
                                    singleItem = splitItemLogic.GetBy(1);
                                    singleItem.deductFeeFrom = "1";
                                    singleItem.beneficiaryAmount = (Convert.ToDecimal(singleItem.beneficiaryAmount) / 2M).ToString();
                                    splitItems.Add(singleItem);
                                    singleItem = splitItemLogic.GetBy(2);
                                    singleItem.deductFeeFrom = "0";
                                    singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                    splitItems.Add(singleItem);
                                }
                                else if (viewModel.PaymentMode.Id == (int)PaymentModes.FourthInstallment)
                                {
                                    singleItem = splitItemLogic.GetBy(1);
                                    singleItem.deductFeeFrom = "1";
                                    singleItem.beneficiaryAmount = (Convert.ToDecimal(singleItem.beneficiaryAmount) / 2M).ToString();
                                    splitItems.Add(singleItem);
                                    singleItem = splitItemLogic.GetBy(2);
                                    singleItem.deductFeeFrom = "0";
                                    singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                    splitItems.Add(singleItem);
                                }
                                
                                //Get BaseURL
                                string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                                if (viewModel.PaymentMode.Id == (int)PaymentModes.FirstInstallment && viewModel.Session.Id == 8)
                                {
                                    viewModel.RemitaPayment = remitaProcessor.GenerateRRRCard(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", settings, Amt);
                                }
                                else
                                {
                                    viewModel.RemitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", splitItems, settings, Amt);
                               
                                }
                                viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                                if (viewModel.RemitaPayment != null)
                                {
                                    transaction.Complete();
                                }
                            }
                            else if (remitaPayment == null && viewModel.PaymentMode != null && (viewModel.PaymentMode.Id == (int)PaymentModes.SecondInstallment ||
                                viewModel.PaymentMode.Id == (int)PaymentModes.ThirdInstallment || viewModel.PaymentMode.Id == (int)PaymentModes.FourthInstallment))
						    {
                                //Get Payment Specific Setting
                                RemitaSettings settings = new RemitaSettings();
                                RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                settings = settingsLogic.GetBy(1);

                                //Get Split Specific details;
                                List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                                RemitaSplitItems singleItem = new RemitaSplitItems();
                                RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                                if (viewModel.PaymentMode.Id == (int)PaymentModes.SecondInstallment)
                                {
                                    singleItem = splitItemLogic.GetBy(1);
                                    singleItem.deductFeeFrom = "1";
                                    splitItems.Add(singleItem);
                                    singleItem = splitItemLogic.GetBy(2);
                                    singleItem.deductFeeFrom = "0";
                                    singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                    splitItems.Add(singleItem);
                                                    }
                                else if (viewModel.PaymentMode.Id == (int)PaymentModes.ThirdInstallment)
                                {
                                    singleItem = splitItemLogic.GetBy(1);
                                    singleItem.deductFeeFrom = "1";
                                    singleItem.beneficiaryAmount = (Convert.ToDecimal(singleItem.beneficiaryAmount) / 2M).ToString();
                                    splitItems.Add(singleItem);
                                    singleItem = splitItemLogic.GetBy(2);
                                    singleItem.deductFeeFrom = "0";
                                    singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                    splitItems.Add(singleItem);
                                }
                                else if (viewModel.PaymentMode.Id == (int)PaymentModes.FourthInstallment)
                                {
                                    singleItem = splitItemLogic.GetBy(1);
                                    singleItem.deductFeeFrom = "1";
                                    singleItem.beneficiaryAmount = (Convert.ToDecimal(singleItem.beneficiaryAmount) / 2M).ToString();
                                    splitItems.Add(singleItem);
                                    singleItem = splitItemLogic.GetBy(2);
                                    singleItem.deductFeeFrom = "0";
                                    singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                    splitItems.Add(singleItem);
                                }

                                //Get BaseURL
                                string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);

                                viewModel.RemitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", splitItems, settings, Amt);

                                viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);

                                if (viewModel.RemitaPayment != null)
                                {
                                    transaction.Complete();
                                }

						    }
                            else
                            {
                                viewModel.RemitaPayment = remitaPayment;

                                bool paymentModified = CheckPaymentAmount(viewModel, payment, remitaPayment, Amt);

                                if (!paymentModified)
                                {
                                    RemitaSettings settings = new RemitaSettings();
                                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                                    settings = settingsLogic.GetBy(1);
                                    viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);
                                }
                                
                                transaction.Complete();
                            }
                            //transaction.Complete();
                        }
					}

					//CheckAndUpdateStudentLevel(viewModel);

					TempData["PaymentViewModel"] = viewModel;
					return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Abundance_Nk.Web.Models.Utility.Encrypt(payment.Id.ToString()), });
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			KeepInvoiceGenerationDropDownState(viewModel);
			return View(viewModel);
		}
        public bool CheckPaymentAmount(PaymentViewModel viewModel, Payment payment, RemitaPayment remita, decimal amountToPay)
        {
            bool status = false;
            try
            {
                if (remita.TransactionAmount != amountToPay && !remita.Status.Contains("01"))
                {
                    RemitaSettings settings = new RemitaSettings();
                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                    settings = settingsLogic.GetBy(1);

                    if (payment.PaymentMode != null && payment.PaymentMode.Id == (int)PaymentModes.SecondInstallment)
                    {
                        string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                        RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                        viewModel.RemitaPayment = remitaProcessor.GenerateRRRCard(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", settings, amountToPay);
                        viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);
                    }
                    else
                    {
                        //Get Split Specific details;
                        List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                        RemitaSplitItems singleItem = new RemitaSplitItems();
                        RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                        singleItem = splitItemLogic.GetBy(1);
                        singleItem.deductFeeFrom = "1";
                        splitItems.Add(singleItem);
                        singleItem = splitItemLogic.GetBy(2);
                        singleItem.deductFeeFrom = "0";
                        singleItem.beneficiaryAmount = Convert.ToString(amountToPay - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                        splitItems.Add(singleItem);

                        //Get BaseURL
                        string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                        RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                        viewModel.RemitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "SCHOOL FEES", splitItems, settings, amountToPay);
                        viewModel.Hash = GenerateHash(settings.Api_key, viewModel.RemitaPayment);
                    }
                    
                    status = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return status;
        }
		public void CheckAndUpdateStudentLevel(PaymentViewModel viewModel)
		{
			try
			{
				StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
				List<StudentLevel> studentLevelList = studentLevelLogic.GetModelsBy(s => s.Person_Id == viewModel.Person.Id);
				//viewModel.StudentLevel = studentLevelLogic.GetBy(viewModel.Student, viewModel.Session);

				if (studentLevelList.Count != 0 && viewModel.StudentLevel != null)
				{
					StudentLevel currentSessionLevel = studentLevelList.LastOrDefault(s => s.Session.Id == viewModel.Session.Id);
					if (currentSessionLevel != null)
					{
						viewModel.StudentLevel = currentSessionLevel;
					}
					else
					{
						StudentLevel newStudentLevel = studentLevelList.LastOrDefault();
						newStudentLevel.Session = viewModel.Session;
						if (newStudentLevel.Level.Id == 1)
						{
							newStudentLevel.Level = new Level() { Id = 2 };
						}
						else if (newStudentLevel.Level.Id == 3)
						{
							newStudentLevel.Level = new Level() { Id = 4 };
						}
						else
						{
							newStudentLevel.Level = viewModel.StudentLevel.Level;
						}

						StudentLevel createdStudentLevel = studentLevelLogic.Create(newStudentLevel);
						viewModel.StudentLevel = studentLevelLogic.GetModelBy(s => s.Student_Level_Id == createdStudentLevel.Id);
					}
				}
			}
			catch (Exception)
			{   
				throw;
			} 
		}
		public ActionResult CarryIndex()
		{
			try
			{
				SetFeeTypeDropDown(viewModel);
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(viewModel);
		}

		[HttpPost]
		public ActionResult CarryIndex(PaymentViewModel viewModel)
		{
			try
			{

				if (InvalidMatricNumber(viewModel.Student.MatricNumber))
				{
					SetFeeTypeDropDown(viewModel);
					return View(viewModel);
				}

				Model.Model.Student student = studentLogic.GetModelsBy(m => m.Matric_Number == viewModel.Student.MatricNumber).LastOrDefault();             
				if (student != null && student.Id > 0)
				{
					return RedirectToAction("GenerateCarryOverInvoice", "Payment", new { sid = student.Id });
				}
				else
				{
					return RedirectToAction("GenerateCarryOverInvoice", "Payment", new { sid = 0 });
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			SetFeeTypeDropDown(viewModel);
			return View(viewModel);
		}

		public ActionResult GenerateCarryOverInvoice()
		{
			try
			{
				viewModel.FeeType = new FeeType() { Id = (int)FeeTypes.CarryOverSchoolFees };
				viewModel.PaymentMode = new PaymentMode() { Id = 1 };
				viewModel.PaymentType = new PaymentType() { Id = 2 };

				ViewBag.States = viewModel.StateSelectListItem;
				ViewBag.Sessions = viewModel.SessionSelectListItem;
				ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
				ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
				ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
				ViewBag.Levels = new SelectList(new List<Level>(), Utility.ID, Utility.NAME);
				
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(viewModel);
		}

		[HttpPost]
		public ActionResult GenerateCarryOverInvoice(PaymentViewModel viewModel)
		{
			try
			{
				ModelState.Remove("Student.LastName");
				ModelState.Remove("Student.FirstName");
				ModelState.Remove("Person.DateOfBirth");
				ModelState.Remove("Student.MobilePhone");
				ModelState.Remove("Student.SchoolContactAddress");
				ModelState.Remove("FeeType.Name");

				if (ModelState.IsValid)
				{
					if (InvalidDepartmentSelection(viewModel))
					{
						KeepInvoiceGenerationDropDownState(viewModel);
						return View(viewModel);
					}

					if (InvalidMatricNumber(viewModel.Student.MatricNumber))
					{
						KeepInvoiceGenerationDropDownState(viewModel);
						return View(viewModel);
					}

				  

					Payment payment = null;
					if (viewModel.StudentAlreadyExist == false)
					{
						using (TransactionScope transaction = new TransactionScope())
						{
							CreatePerson(viewModel);
							CreateStudent(viewModel);
							payment = CreatePayment(viewModel);
							CreateStudentLevel(viewModel);

							transaction.Complete();
						}
					}
					else
					{
						personLogic.Modify(viewModel.Person);
						FeeType feeType = new FeeType() { Id = (int)FeeTypes.CarryOverSchoolFees };
						payment = paymentLogic.GetBy(feeType, viewModel.Person, viewModel.Session);
					}


					if (payment == null || payment.Id <= 0)
					{
						payment = CreatePayment(viewModel);
					}

					TempData["PaymentViewModel"] = viewModel;
					return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Abundance_Nk.Web.Models.Utility.Encrypt(payment.Id.ToString()), });
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			KeepInvoiceGenerationDropDownState(viewModel);
			return View(viewModel);
		}

		private bool DoesMatricNumberExist(string matricNo)
		{
			try
			{
				Abundance_Nk.Model.Model.Student student = studentLogic.GetModelsBy(m => m.Matric_Number == matricNo).LastOrDefault();
				if (student == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				
				throw;
			}

		}

		private bool InvalidMatricNumber(string matricNo)
		{
			try
			{
				string baseMatricNo = null;
				string[] matricNoArray = matricNo.Split('/');
				
				if (matricNoArray.Length > 0)
				{
					string[] matricNoArrayCopy = new string[matricNoArray.Length - 1];
					for (int i = 0; i < matricNoArray.Length; i++)
					{
						if (i != matricNoArray.Length - 1)
						{
							matricNoArrayCopy[i] = matricNoArray[i];
						}
					}
					if (matricNoArrayCopy.Length > 0)
					{
						baseMatricNo = string.Join("/", matricNoArrayCopy); 
					}
				}
				else
				{
					SetMessage("Invalid Matric Number entered!", Message.Category.Error);
					return true;
				}

				if (!string.IsNullOrWhiteSpace(baseMatricNo))
				{
					//StudentMatricNumberAssignmentLogic studentMatricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
					//bool isInvalid = studentMatricNumberAssignmentLogic.IsInvalid(baseMatricNo);
					//if (isInvalid)
					//{
					//    SetMessage("Invalid Matric Number entered!", Message.Category.Error);
					//    return true;
					//}
				}
				else
				{
					SetMessage("Invalid Matric Number entered!", Message.Category.Error);
					return true;
				}
								
				return false;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private bool InvalidDepartmentSelection(PaymentViewModel viewModel)
		{
			try
			{
				if (viewModel.StudentLevel.Department == null || viewModel.StudentLevel.Department.Id <= 0)
				{
					SetMessage("Please select Department!", Message.Category.Error);
					return true;
				}
				else if ((viewModel.StudentLevel.DepartmentOption == null && viewModel.StudentLevel.Programme.Id > 2) || (viewModel.StudentLevel.DepartmentOption != null && viewModel.StudentLevel.DepartmentOption.Id <= 0 && viewModel.StudentLevel.Programme.Id > 2))
				{
					viewModel.DepartmentOptionSelectListItem = Utility.PopulateDepartmentOptionSelectListItem(viewModel.StudentLevel.Department, viewModel.StudentLevel.Programme);
					if (viewModel.DepartmentOptionSelectListItem != null && viewModel.DepartmentOptionSelectListItem.Count > 0)
					{
						SetMessage("Please select Department Option!", Message.Category.Error);
						return true;
					}
				}

				return false;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void KeepInvoiceGenerationDropDownState(PaymentViewModel viewModel)
		{
			try
			{
				if (viewModel.Session != null && viewModel.Session.Id > 0)
				{
					ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Session.Id);
				}
				else
				{
					ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT);
				}

				if (viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
				{
					ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Person.State.Id);
				}
				else
				{
					ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT);
				}

				if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
				{
					ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Level.Id);
				}
				else
				{
					ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT);
				}
			    if (viewModel.PaymentMode != null && viewModel.PaymentMode.Id > 0)
			    {
			        ViewBag.PaymentModes = new SelectList(viewModel.PaymentModeSelectListItem, Utility.VALUE,
			            Utility.TEXT, viewModel.PaymentMode.Id);
			    }
			    else
			    {
			        ViewBag.PaymentModes = new SelectList(new List<PaymentMode>(), Utility.VALUE, Utility.TEXT);
			    }
				if (viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
				{
					viewModel.DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(viewModel.StudentLevel.Programme);
					ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Programme.Id);

					if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
					{
						viewModel.DepartmentOptionSelectListItem = Utility.PopulateDepartmentOptionSelectListItem(viewModel.StudentLevel.Department, viewModel.StudentLevel.Programme);
						ViewBag.Departments = new SelectList(viewModel.DepartmentSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Department.Id);
						
						if (viewModel.StudentLevel.DepartmentOption != null && viewModel.StudentLevel.DepartmentOption.Id > 0)
						{
							ViewBag.DepartmentOptions = new SelectList(viewModel.DepartmentOptionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.DepartmentOption.Id);
						}
						else
						{
							if (viewModel.DepartmentOptionSelectListItem != null && viewModel.DepartmentOptionSelectListItem.Count > 0)
							{
								ViewBag.DepartmentOptions = new SelectList(viewModel.DepartmentOptionSelectListItem, Utility.VALUE, Utility.TEXT);
							}
							else
							{
								ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
							}
						}
					}
					else
					{
						ViewBag.Departments = new SelectList(viewModel.DepartmentSelectListItem, Utility.VALUE, Utility.TEXT);
						ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
					}
				}
				else
				{
					ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT);
					ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
					ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occured " + ex.Message, Message.Category.Error);
			}
		}

		private Person CreatePerson(PaymentViewModel viewModel)
		{
			try
			{
				Role role = new Role() { Id = 5 };
				//PersonType personType = new PersonType() { Id = viewModel.PersonType.Id };
				Nationality nationality = new Nationality() { Id = 1 };

				viewModel.Person.Role = role;
				viewModel.Person.Nationality = nationality;
				viewModel.Person.DateEntered = DateTime.Now;
				//viewModel.Person.PersonType = personType;

				Person person = personLogic.Create(viewModel.Person);
				if (person != null && person.Id > 0)
				{
					viewModel.Person.Id = person.Id;
				}

				return person;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public Model.Model.Student CreateStudent(PaymentViewModel viewModel)
		{
			try
			{
			   
				viewModel.Student.Number = 4;
				viewModel.Student.Category = new StudentCategory() { Id = viewModel.StudentLevel.Level.Id <= 2 ? 1 : 2 };
				viewModel.Student.Id = viewModel.Person.Id;
				
				return studentLogic.Create(viewModel.Student);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private Payment CreatePayment(PaymentViewModel viewModel)
		{
			Payment newPayment = new Payment();
			try
			{
				Payment payment = new Payment();
				payment.PaymentMode = viewModel.PaymentMode;
				payment.PaymentType = viewModel.PaymentType;
				payment.PersonType = viewModel.Person.Type;
				payment.FeeType = viewModel.FeeType;
				payment.DatePaid = DateTime.Now;
				payment.Person = viewModel.Person;
				payment.Session = viewModel.Session;

				OnlinePayment newOnlinePayment = null;
				newPayment = paymentLogic.Create(payment);
				newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id,viewModel.PaymentMode.Id, viewModel.StudentLevel.Department.Id, viewModel.Session.Id);
				Decimal Amt = newPayment.FeeDetails.Sum(p => p.Fee.Amount);

				if (newPayment != null)
				{
					PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
					OnlinePayment onlinePayment = new OnlinePayment();
					onlinePayment.Channel = channel;
					onlinePayment.Payment = newPayment;
					newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);

				}

					payment = newPayment;
					if (payment != null)
					{
					   // transaction.Complete();
					}
			   
				return newPayment;
			}
			catch (Exception)
			{
				throw;
			}
		}

        private Payment CreateApplicantPayment(PaymentViewModel viewModel)
        {
            Payment newPayment = new Payment();
            try
            {
                Payment payment = new Payment();
                payment.PaymentMode = viewModel.PaymentMode;
                payment.PaymentType = viewModel.PaymentType;
                payment.PersonType = viewModel.Person.Type;
                payment.FeeType = viewModel.FeeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = viewModel.Person;
                payment.Session = viewModel.Session;

                OnlinePayment newOnlinePayment = null;
                newPayment = paymentLogic.Create(payment);
                //newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id, viewModel.PaymentMode.Id, viewModel.StudentLevel.Department.Id, viewModel.Session.Id);
                //Decimal Amt = newPayment.FeeDetails.Sum(p => p.Fee.Amount);

                if (newPayment != null)
                {
                    PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                    OnlinePayment onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);

                }

                payment = newPayment;
                if (payment != null)
                {
                    // transaction.Complete();
                }

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private StudentPayment CreatePaymentLog(PaymentViewModel viewModel, Payment payment)
        {
            try
            {
                var student = new Model.Model.Student();
                var studentLogic = new StudentLogic();
                StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                student = studentLogic.GetBy(viewModel.Person.Id);
                var studentPayment = new StudentPayment();
                studentPayment.Id = payment.Id;
                studentPayment.Level = viewModel.StudentLevel.Level;
                studentPayment.Session = viewModel.Session;
                studentPayment.Student = student;
                studentPayment.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                studentPayment.Status = false;
                studentPayment = studentPaymentLogic.Create(studentPayment);
                return studentPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

		private StudentLevel CreateStudentLevel(PaymentViewModel viewModel)
		{
			try
			{
				//StudentLevel studentLevel = new StudentLevel();
				//studentLevel.Department = viewModel.StudentLevel.Department;
				//studentLevel.DepartmentOption = viewModel.StudentLevel.DepartmentOption;
				//studentLevel.Level = viewModel.StudentLevel.Level;
				//studentLevel.Programme = viewModel.Programme;

				viewModel.StudentLevel.Session = viewModel.Session;
				viewModel.StudentLevel.Student = viewModel.Student;
				return studentLevelLogic.Create(viewModel.StudentLevel);


				//StudentLevel studentLevel = new StudentLevel();
				//studentLevel.Department = viewModel.StudentLevel.Department;
				//studentLevel.DepartmentOption = viewModel.StudentLevel.DepartmentOption;
				//studentLevel.Session = viewModel.Session;
				//studentLevel.Level = viewModel.StudentLevel.Level;
				//studentLevel.Programme = viewModel.Programme;
				//studentLevel.Student = viewModel.Student;

				//return studentLevelLogic.Create(studentLevel);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public ActionResult GetDepartmentAndLevelByProgrammeId(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
				{
					return null;
				}

				List<Level> levels = null;
				List<Department> departments = null;
				Programme programme = new Programme() { Id = Convert.ToInt32(id) };
				if (programme.Id > 0)
				{
					DepartmentLogic departmentLogic = new DepartmentLogic();
					departments = departmentLogic.GetBy(programme);

					LevelLogic levelLogic = new LevelLogic();
                    levels = levelLogic.GetLevelsByProgramme(programme);
				}

				//return Json(new SelectList(departments, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
				//return Json(new { departments = departments, levels = levels }, "text/html", JsonRequestBehavior.AllowGet);
				return Json(new { Departments = departments, Levels = levels }, "json", JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	   
		public JsonResult GetDepartmentOptionByDepartment(string id, string programmeid)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
				{
					return null;
				}

				Department department = new Department() { Id = Convert.ToInt32(id) };
				Programme programme = new Programme() { Id = Convert.ToInt32(programmeid) };
				DepartmentOptionLogic departmentLogic = new DepartmentOptionLogic();
				List<DepartmentOption> departmentOptions = departmentLogic.GetBy(department, programme);

				return Json(new SelectList(departmentOptions, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public ActionResult GenerateShortFallInvoice()
		{
			try
			{
				viewModel = new PaymentViewModel();

				if (System.Web.HttpContext.Current.Session["student"] != null)
				{
					studentLogic = new StudentLogic();
					Model.Model.Student student = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
					student = studentLogic.GetBy(student.Id);

					viewModel.Student = student;
				}

				PopulateDropDown(viewModel);
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(viewModel);
		}

		[HttpPost]
		public ActionResult GenerateShortFallInvoice(PaymentViewModel viewModel)
		{
			try
			{
				if (viewModel.Student.MatricNumber != null && viewModel.PaymentEtranzact.ConfirmationNo != null && viewModel.FeeType.Id > 0 && viewModel.Session.Id > 0)
				{
					StudentLogic studentLogic = new StudentLogic();
					PaymentLogic paymentLogic = new PaymentLogic();
					PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
					PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
					FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
					StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
					ShortFallLogic shortFallLogic = new ShortFallLogic();
					OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();

					decimal invoiceAmount = 0M;
					double shortFallAmount = 0.0;

					Model.Model.Student student = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber).LastOrDefault();

					if (student != null)
					{
						StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == student.Id && s.Session_Id == viewModel.Session.Id).LastOrDefault();
						FeeDetail feeDetail = new FeeDetail();
						if (viewModel.FeeType.Id == (int)FeeTypes.AcceptanceFee || viewModel.FeeType.Id == (int)FeeTypes.CarryOverSchoolFees || viewModel.FeeType.Id == (int)FeeTypes.SchoolFees)
						{
							feeDetail = feeDetailLogic.GetModelsBy(f => f.Programme_Id == studentLevel.Programme.Id && f.Department_Id == studentLevel.Department.Id && f.Fee_Type_Id == viewModel.FeeType.Id && f.Level_Id == studentLevel.Level.Id && f.Session_Id == viewModel.Session.Id).LastOrDefault();
						}
						else
						{
							feeDetail = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == viewModel.FeeType.Id && f.Session_Id == viewModel.Session.Id).LastOrDefault();
						}

						invoiceAmount = feeDetail.Fee.Amount;

						Payment existingShortFallPayment = paymentLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id && p.Fee_Type_Id == (int)FeeTypes.ShortFall && p.Person_Id == student.Id).LastOrDefault();
						if (existingShortFallPayment != null)
						{
							ShortFall existingShortFall = shortFallLogic.GetModelsBy(s => s.Payment_Id == existingShortFallPayment.Id).LastOrDefault();
							if (existingShortFall != null)
							{
								return RedirectToAction("ShortFallInvoice", "Credential", new { area = "Common", pmid = existingShortFallPayment.Id, amount = existingShortFall.Amount });
							}
						}

						PaymentTerminal paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Session_Id == viewModel.Session.Id && p.Fee_Type_Id == viewModel.FeeType.Id);
						PaymentEtranzact etranzact = paymentEtranzactLogic.RetrieveEtranzactWebServicePinDetails(viewModel.PaymentEtranzact.ConfirmationNo, paymentTerminal);

						if (etranzact != null)
						{
							Payment existingPayment = paymentLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id && p.Fee_Type_Id == viewModel.FeeType.Id && p.Person_Id == student.Id).LastOrDefault();
							if (existingPayment != null)
							{
								if (etranzact.CustomerID.ToUpper().Trim() != existingPayment.InvoiceNumber.ToUpper().Trim())
								{
									SetMessage("Confirmation Order Number not valid for the selected payment type! ", Message.Category.Error);

									PopulateDropDown(viewModel);
									return View(viewModel);
								}

							}

							if (invoiceAmount > etranzact.TransactionAmount)
							{
								shortFallAmount = Convert.ToDouble(invoiceAmount - etranzact.TransactionAmount.Value);

								Payment createdPayment = new Payment();

								using (TransactionScope scope = new TransactionScope())
								{
									Payment payment = new Payment();
									payment.DatePaid = DateTime.Now;
									payment.FeeType = new FeeType() { Id = (int)FeeTypes.ShortFall };
									payment.PaymentMode = new PaymentMode() { Id = (int)PaymentModes.Full };
									payment.PaymentType = new PaymentType() { Id = (int)Paymenttypes.OnlinePayment };
									payment.Person = new Person() { Id = student.Id };
									payment.PersonType = new PersonType() { Id = (int)PersonTypes.Student };
									payment.Session = viewModel.Session;

									createdPayment = paymentLogic.Create(payment);

									if (createdPayment != null)
									{
										OnlinePayment onlinePaymentCheck = onlinePaymentLogic.GetModelBy(op => op.Payment_Id == createdPayment.Id);
										if (onlinePaymentCheck == null)
										{
											PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
											OnlinePayment onlinePayment = new OnlinePayment();
											onlinePayment.Channel = channel;
											onlinePayment.Payment = createdPayment;

											onlinePaymentLogic.Create(onlinePayment);
										}
									}

									ShortFall shortFall = new ShortFall();
									shortFall.Amount = shortFallAmount;
									shortFall.Payment = createdPayment;

									shortFallLogic.Create(shortFall);

									scope.Complete();
								}

								return RedirectToAction("ShortFallInvoice", "Credential", new { area = "Common", pmid = createdPayment.Id, amount = shortFallAmount });
							}
							else
							{
								SetMessage("No shortFall to generate! ", Message.Category.Error);
							}
						}
					}
					else
					{
						SetMessage("Matric Number not found! ", Message.Category.Error);
					}
				}
				else
				{
					SetMessage("Check the selected fields and try again! ", Message.Category.Error);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			PopulateDropDown(viewModel);

			return View(viewModel);
		}

		public void PopulateDropDown(PaymentViewModel viewModel)
		{
			try
			{
				int[] feeTypesToDisplay = { (int)FeeTypes.AcceptanceFee, (int)FeeTypes.CarryOverSchoolFees, (int)FeeTypes.SchoolFees, (int)FeeTypes.HostelFee };
				List<string> feeTypes = new List<string>();

				for (int i = 0; i < feeTypesToDisplay.Length; i++)
				{
					feeTypes.Add(feeTypesToDisplay[i].ToString());
				}

				viewModel.FeeTypeSelectListItem = viewModel.FeeTypeSelectListItem.Where(f => f.Value == "" || feeTypes.Contains(f.Value)).ToList();

				if (viewModel.Session != null && viewModel.Session.Id > 0 && viewModel.FeeType != null && viewModel.FeeType.Id > 0)
				{
					ViewBag.FeeTypes = new SelectList(viewModel.FeeTypeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.FeeType.Id);
					ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Session.Id);
				}
				else
				{
					ViewBag.FeeTypes = new SelectList(viewModel.FeeTypeSelectListItem, Utility.VALUE, Utility.TEXT);
					ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT);
				}
			}
			catch (Exception)
			{
				throw;
			}

		}

		public ActionResult PayShortFallFee()
		{
			try
			{
				viewModel = new PaymentViewModel();
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(viewModel);
		}
		[HttpPost]
		public ActionResult PayShortFallFee(PaymentViewModel viewModel)
		{
			try
			{
				if (viewModel.PaymentEtranzact.ConfirmationNo != null)
				{
					PaymentLogic paymentLogic = new PaymentLogic();

					if (viewModel.PaymentEtranzact.ConfirmationNo.Length > 12)
					{
						Model.Model.Session session = new Model.Model.Session() { Id = 7 };
						FeeType feetype = new FeeType() { Id = (int)FeeTypes.ShortFall };
						Payment payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.PaymentEtranzact.ConfirmationNo, feetype.Id);
						if (payment != null && payment.Id > 0)
						{
							if (payment.FeeType.Id != (int)FeeTypes.ShortFall)
							{
								SetMessage("Confirmation Order Number (" + viewModel.PaymentEtranzact.ConfirmationNo + ") entered is not for shortfall fee payment! Please enter your shortfall fee confirmation order number.", Message.Category.Error);
								return View(viewModel);
							}

							return RedirectToAction("Receipt", "Credential", new { area = "Common", pmid = payment.Id });
						}
					}
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(viewModel);
		}
        public ActionResult CardPayment()
        {
            PaymentViewModel viewModel = (PaymentViewModel)TempData["PaymentViewModel"];
            viewModel.ResponseUrl = ConfigurationManager.AppSettings["RemitaResponseUrl"].ToString();
            TempData.Keep("PaymentViewModel");

            return View(viewModel);
        }
        private string GenerateHash(string apiKey, RemitaPayment remitaPayment)
        {
            string hashConcatenate = null;
            try
            {
                if (remitaPayment != null)
                {
                    string hash = remitaPayment.MerchantCode + remitaPayment.RRR + apiKey;
                    RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(hash);
                    hashConcatenate = remitaProcessor.HashPaymentDetailToSHA512(hash);
                }
                
                return hashConcatenate;
            }
            catch (Exception)
            {
                throw;
            }
        }

	    public JsonResult GetPaymentModes(long studentId, int sessionId, int progId)
        {
            List<PaymentMode> paymentModes = new List<PaymentMode>();
	        try
	        {
	            if (studentId > 0 && sessionId > 0)
	            {
                    List<int> modeIds = new List<int>();

                    PaymentLogic paymentLogic = new PaymentLogic();
                    SpilloverStudentLogic spilloverStudentLogic = new SpilloverStudentLogic();

                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
	                List<Payment> sessionSchoolFeePayments = paymentLogic.GetModelsBy(p => p.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.Person_Id == studentId && p.Session_Id == sessionId);

                    SpilloverStudent spilloverStudent = spilloverStudentLogic.GetModelsBy(s => s.Student_Id == studentId && s.Session_Id == sessionId).LastOrDefault();

                    if (sessionId != (int)Sessions._20172018)
                    {
                        if (progId == (int)Programmes.HNDPartTime || progId == (int)Programmes.NDPartTime)
                        {
                            paymentModes = GetPartTimeModes(sessionSchoolFeePayments);
                        }
                        else
                        {
                            modeIds.Add((int)PaymentModes.Full);
                            paymentModes = LoadPaymentModes(modeIds);
                        }
                        
                        return Json(new SelectList(paymentModes, "Id", "Name"), JsonRequestBehavior.AllowGet);
                    }

                    if (spilloverStudent != null)
                    {
                        paymentModes = LoadPaymentModesForSpilloverStudent(sessionSchoolFeePayments, spilloverStudent);
                        return Json(new SelectList(paymentModes, "Id", "Name"), JsonRequestBehavior.AllowGet);
                    }

	                if (progId == (int)Programmes.NDFullTime || progId == (int)Programmes.HNDEvening || progId == (int)Programmes.HNDFullTime)
	                {
                        modeIds.Add(1);
                        modeIds.Add(2);
                        modeIds.Add(3);
                        paymentModes = LoadPaymentModes(modeIds);
                        return Json(new SelectList(paymentModes, "Id", "Name"), JsonRequestBehavior.AllowGet);
	                }

                    if (sessionSchoolFeePayments == null || sessionSchoolFeePayments.Count <= 0)
                    {
                        modeIds.Add(1);
                        modeIds.Add(2);
                        paymentModes = LoadPaymentModes(modeIds);
                    }
                    else
                    {
                        int paymentModeId = sessionSchoolFeePayments.Select(p => p.PaymentMode.Id).Max();
                        Payment payment = sessionSchoolFeePayments.LastOrDefault(p => p.PaymentMode.Id == paymentModeId);
                        bool paymentStatus = remitaPaymentLogic.GetModelBy(p => p.Payment_Id == payment.Id && p.Status.Contains("01")) != null ? true : false;

                        switch (paymentModeId)
                        {
                            case (int)PaymentModes.Full:
                                if (paymentStatus)
                                    paymentModes = LoadPaymentModes(modeIds);
                                else
                                {
                                    modeIds.Add(1);
                                    modeIds.Add(2);

                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                break;

                            case (int)PaymentModes.FirstInstallment:
                                if (paymentStatus)
                                {
                                    modeIds.Add(3);
                                    modeIds.Add(4);
                                    modeIds.Add(5);

                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                else
                                {
                                    modeIds.Add(1);
                                    modeIds.Add(2);

                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                break;
                            case (int)PaymentModes.SecondInstallment:
                                if (paymentStatus)
                                {
                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                else
                                {
                                    modeIds.Add(3);
                                    modeIds.Add(4);
                                    modeIds.Add(5);

                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                break;
                            case (int)PaymentModes.ThirdInstallment:
                                if (paymentStatus)
                                {
                                    modeIds.Add(5);
                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                else
                                {
                                    modeIds.Add(3);
                                    modeIds.Add(4);

                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                break;
                            case (int)PaymentModes.FourthInstallment:
                                if (paymentStatus)
                                {
                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                else
                                {
                                    modeIds.Add(5);

                                    paymentModes = LoadPaymentModes(modeIds);
                                }
                                break;
                        }
                    }
	            }
	        }
	        catch (Exception)
	        {
                return Json(null, JsonRequestBehavior.AllowGet);
	        }

	        return Json(new SelectList(paymentModes, "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        private List<PaymentMode> GetPartTimeModes(List<Payment> sessionSchoolFeePayments)
        {
            List<PaymentMode> paymentModes = new List<PaymentMode>();
            try
            {
                List<int> modeIds = new List<int>();

                if (sessionSchoolFeePayments == null || sessionSchoolFeePayments.Count <= 0)
                {
                    modeIds.Add(1);
                    modeIds.Add(2);
                    paymentModes = LoadPaymentModes(modeIds);
                }
                else
                {
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    int paymentModeId = sessionSchoolFeePayments.Select(p => p.PaymentMode.Id).Max();
                    Payment payment = sessionSchoolFeePayments.LastOrDefault(p => p.PaymentMode.Id == paymentModeId);
                    bool paymentStatus = remitaPaymentLogic.GetModelBy(p => p.Payment_Id == payment.Id && (p.Status.Contains("01") || p.Description.ToLower().Contains("manual"))) != null ? true : false;

                    switch (paymentModeId)
                    {
                        case (int)PaymentModes.Full:
                            if (paymentStatus)
                                paymentModes = LoadPaymentModes(modeIds);
                            else
                            {
                                modeIds.Add(1);
                                modeIds.Add(2);

                                paymentModes = LoadPaymentModes(modeIds);
                            }
                            break;

                        case (int)PaymentModes.FirstInstallment:
                            if (paymentStatus)
                            {
                                modeIds.Add(3);

                                paymentModes = LoadPaymentModes(modeIds);
                            }
                            else
                            {
                                modeIds.Add(1);
                                modeIds.Add(2);

                                paymentModes = LoadPaymentModes(modeIds);
                            }
                            break;

                        case (int)PaymentModes.SecondInstallment:
                            if (paymentStatus)
                            {
                                paymentModes = LoadPaymentModes(modeIds);
                            }
                            else
                            {
                                modeIds.Add(3);

                                paymentModes = LoadPaymentModes(modeIds);
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return paymentModes;
        }

        private List<PaymentMode> LoadPaymentModesForSpilloverStudent(List<Payment> sessionSchoolFeePayments, SpilloverStudent spilloverStudent)
        {
            List<PaymentMode> paymentModes = new List<PaymentMode>();
	        try
	        {
                List<int> modeIds = new List<int>();

                PaymentLogic paymentLogic = new PaymentLogic();

                if (sessionSchoolFeePayments == null || sessionSchoolFeePayments.Count == 0)
	            {
                    modeIds.Add(1);
                    modeIds.Add(2);
                    modeIds.Add(3);
                    modeIds.Add(4);
                    paymentModes = LoadPaymentModes(modeIds);
	            }
                else if (sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.FirstInstallment) != null && 
                    paymentLogic.IsPaid(sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.FirstInstallment).Id))
                {
                    modeIds.Add(3);
                    modeIds.Add(4);
                    paymentModes = LoadPaymentModes(modeIds);
                }
                else if (sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.FirstInstallment) != null &&
                    !paymentLogic.IsPaid(sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.FirstInstallment).Id))
                {
                    modeIds.Add(2);
                    modeIds.Add(3);
                    modeIds.Add(4);
                    paymentModes = LoadPaymentModes(modeIds);
                }
                else if (sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.SecondInstallment) != null &&
                    paymentLogic.IsPaid(sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.SecondInstallment).Id))
                {
                    paymentModes = LoadPaymentModes(modeIds);
                }
                else if (sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.SecondInstallment) != null &&
                    !paymentLogic.IsPaid(sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.SecondInstallment).Id))
                {
                    modeIds.Add(2);
                    modeIds.Add(3);
                    modeIds.Add(4);
                    paymentModes = LoadPaymentModes(modeIds);
                }
                else if (sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.ThirdInstallment) != null &&
                    paymentLogic.IsPaid(sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.ThirdInstallment).Id))
                {
                    modeIds.Add(5);
                    paymentModes = LoadPaymentModes(modeIds);
                }
                else if (sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.ThirdInstallment) != null &&
                    !paymentLogic.IsPaid(sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.ThirdInstallment).Id))
                {
                    modeIds.Add(3);
                    modeIds.Add(4);
                    paymentModes = LoadPaymentModes(modeIds);
                }
                else if (sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.FourthInstallment) != null &&
                    paymentLogic.IsPaid(sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.FourthInstallment).Id))
                {
                    paymentModes = LoadPaymentModes(modeIds);
                }
                else if (sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.FourthInstallment) != null &&
                    !paymentLogic.IsPaid(sessionSchoolFeePayments.LastOrDefault(s => s.PaymentMode.Id == (int)PaymentModes.FourthInstallment).Id))
                {
                    modeIds.Add(5);
                    paymentModes = LoadPaymentModes(modeIds);
                }
	        }
	        catch (Exception)
	        {
	            throw;
	        }

	        return paymentModes;
        }

	    private List<PaymentMode> LoadPaymentModes(List<int> modeIds)
        {
            try
            {
                PaymentModeLogic paymentModeLogic = new PaymentModeLogic();
                return paymentModeLogic.GetModelsBy(p => modeIds.Contains(p.Payment_Mode_Id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public decimal GetAmountToPay(Payment payment, Session session)
        {
            decimal amt = 0M;
            try
            {
                if (payment != null)
                {
                    long progId = 0;
                    long deptId = 0;
                    long levelId = 0;
                    long nextLevelId = 0;

                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id && s.Session_Id == payment.Session.Id).LastOrDefault();
                    if (studentLevel != null)
                    {
                        progId = studentLevel.Programme.Id;
                        deptId = studentLevel.Department.Id;
                        levelId = studentLevel.Level.Id;
                        nextLevelId = studentLevel.Level.Id + 1;
                    }
                    else
                    {
                        AdmissionListLogic listLogic = new AdmissionListLogic();
                        AdmissionList list = listLogic.GetModelsBy(s => s.APPLICATION_FORM.Person_Id == payment.Person.Id && s.Activated).LastOrDefault();
                        if (list != null)
                        {
                            progId = list.Programme.Id;
                            deptId = list.Deprtment.Id;
                            levelId = GetLevel(list.Programme.Id);
                        }
                    }

                    if (progId > 0 && deptId > 0 && levelId > 0)
                    {
                        FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                        List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(f => f.Department_Id == deptId && f.Fee_Type_Id == payment.FeeType.Id &&
                                                        f.Payment_Mode_Id == payment.PaymentMode.Id && f.Level_Id == nextLevelId && f.Programme_Id == progId && f.Session_Id == payment.Session.Id);

                        if (feeDetails != null && feeDetails.Count > 0)
                        {
                            return feeDetails.Sum(f => f.Fee.Amount);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return amt;
        }


        public bool HasPaidSchoolFees(Payment payment, Session session, decimal AmountToPay)
        {
            long progId = 0;
            //long deptId = 0;
            //long levelId = 0;
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == payment.Id && r.Status.Contains("021:"));
            StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id).LastOrDefault();

            if (studentLevel != null)
            {
                progId = studentLevel.Programme.Id;
                //deptId = studentLevel.Department.Id;
                //levelId = studentLevel.Level.Id;
            }
            if(remitaPayment != null)
            {
                if (progId == (int)Programmes.HNDPartTime || progId == (int)Programmes.NDPartTime)
                {
                    decimal eightyPercentOfFirstInstallment = AmountToPay * (80M / 100M);
                    if (remitaPayment != null && remitaPayment.TransactionAmount >= eightyPercentOfFirstInstallment)
                        return true;
                    return false;
                }
                else if (progId == (int)Programmes.HNDEvening || progId == (int)Programmes.NDEveningFullTime)
                {
                    decimal fiftyPercentOfFullPayment = AmountToPay * (45M / 100M);
                    if (remitaPayment != null && remitaPayment.TransactionAmount >= fiftyPercentOfFullPayment)
                        return true;
                    return false;
                }
                else
                {
                    if (remitaPayment.TransactionAmount >= AmountToPay)
                        return true;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }

}