using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
	[AllowAnonymous]
	public class ExtraYearController : BaseController
	{
		private const string ID = "Id";
		private const string NAME = "Name";
		private const string VALUE = "Value";
		private const string TEXT = "Text";
		private PersonLogic personLogic;
		private PaymentLogic paymentLogic;
		private OnlinePaymentLogic onlinePaymentLogic;
		private StudentLevelLogic studentLevelLogic;
		private StudentLogic studentLogic;
		private CourseLogic courseLogic;
		private PaymentViewModel viewModel;
		private CourseRegistrationViewModel courseRegViewModel;
		private StudentExtraYearLogic extraYearLogic;
		private RegistrationLogonViewModel logonViewModel;
		public ExtraYearController()
		{
			personLogic = new PersonLogic();
			paymentLogic = new PaymentLogic();
			onlinePaymentLogic = new OnlinePaymentLogic();
			studentLevelLogic = new StudentLevelLogic();
			studentLogic = new StudentLogic();
			courseRegViewModel = new CourseRegistrationViewModel();
			viewModel = new PaymentViewModel();
			courseLogic = new CourseLogic();
			extraYearLogic = new StudentExtraYearLogic();
			logonViewModel = new RegistrationLogonViewModel();
		}
		public ActionResult Index()
		{
			try
			{
				SetFeeTypeDropDown(viewModel);

				if (System.Web.HttpContext.Current.Session["student"] != null)
				{
					studentLogic = new StudentLogic();
					Model.Model.Student student = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
					student = studentLogic.GetBy(student.Id);

					viewModel.Student = student;
				}
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

				if (InvalidMatricNumber(viewModel.Student.MatricNumber))
				{
					SetFeeTypeDropDown(viewModel);
					return View(viewModel);
				}
				List<Model.Model.Student> duplicates = studentLogic.GetStudentsBy(viewModel.Student.MatricNumber);
				if (duplicates != null && duplicates.Count > 1)
				{
					//Fuck the duplicates
					studentLogic.DeleteDuplicateMatricNumber(viewModel.Student.MatricNumber);
				}

				Model.Model.Student student = studentLogic.GetBy(viewModel.Student.MatricNumber);
				if (student != null && student.Id > 0)
				{

					return RedirectToAction("Step_1", "ExtraYear", new { sid = student.Id });
				   
				}
				else
				{
					return RedirectToAction("Step_1", "ExtraYear", new { sid = 0 });
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			SetFeeTypeDropDown(viewModel);
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
				else if ((viewModel.StudentLevel.DepartmentOption == null && viewModel.StudentLevel.Programme.Id > 2) || (viewModel.StudentLevel.DepartmentOption.Id <= 0 && viewModel.StudentLevel.Programme.Id > 2))
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

			   
					if (viewModel.extraYear.Session != null && viewModel.extraYear.Session.Id > 0)
					{
						ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.extraYear.Session.Id);
						ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Session.Id);

					}
					else
					{
						ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT);

					}
					if (viewModel.extraYear.LastSessionRegistered != null && viewModel.extraYear.LastSessionRegistered.Id > 0)
					{
						ViewBag.LastSessions = new SelectList(viewModel.AllSessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.extraYear.LastSessionRegistered.Id);
					}
					else
					{
						ViewBag.LastSessions = new SelectList(viewModel.AllSessionSelectListItem, Utility.VALUE, Utility.TEXT);
				   
					}
					if (viewModel.extraYear.DeferementCommencedSession != null && viewModel.extraYear.DeferementCommencedSession.Id > 0)
					{
						ViewBag.DeferredSessions = new SelectList(viewModel.AllSessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.extraYear.DeferementCommencedSession.Id);

					}
					else
					{
						ViewBag.DeferredSessions = new SelectList(viewModel.AllSessionSelectListItem, Utility.VALUE, Utility.TEXT);

					}

					List<Value> NoOfSessions = Utility.CreateNumberListFrom(1, 4);
					NoOfSessions.Insert(0, new Value() { Name = "--Select--" });


					if (viewModel.extraYear.Sessions_Registered > 0)
					{
						ViewBag.SessionsRegistered = new SelectList(NoOfSessions, ID, NAME, viewModel.extraYear.Sessions_Registered);
					}
					else
					{
						ViewBag.SessionsRegistered = new SelectList(NoOfSessions, ID, NAME);
			   
					}

			   

			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred " + ex.Message, Message.Category.Error);
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
				//assign matric no to applicant

				//Department department = viewModel.Department;
				//Session session = viewModel.Session;
				//Level level = viewModel.Level;

				//StudentMatricNumberAssignment startMatricNo = studentMatricNumberAssignmentLogic.GetBy(faculty, department, level, session);
				//if (startMatricNo != null)
				//{
				//long studentNumber = 0;
				//string matricNumber = "";

				//if (startMatricNo.Used)
				//{
				//    string[] matricNoArray = startMatricNo.MatricNoStartFrom.Split('/');

				//    studentNumber = GetNextStudentNumber(faculty, department, level, session);
				//    matricNoArray[matricNoArray.Length - 1] = UtilityLogic.PaddNumber(studentNumber, 4);
				//    matricNumber = string.Join("/", matricNoArray);
				//}
				//else
				//{
				//    matricNumber = startMatricNo.MatricNoStartFrom;
				//    studentNumber = startMatricNo.MatricSerialNoStartFrom;
				//    bool markedAsUsed = studentMatricNumberAssignmentLogic.MarkAsUsed(startMatricNo);
				//}


				viewModel.Student.Number = 4;
				viewModel.Student.Category = new StudentCategory() { Id = viewModel.StudentLevel.Programme.Id <= 2 ? 1 : 2 };
				viewModel.Student.Id = viewModel.Person.Id;

				return studentLogic.Create(viewModel.Student);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private StudentExtraYearSession AddExtraYearDetails(PaymentViewModel viewModel)
		{
			StudentExtraYearSession extraYear = new StudentExtraYearSession();
			try
			{
				StudentExtraYearLogic extraYearLogic = new StudentExtraYearLogic();
				extraYear.Person = viewModel.Person;
				extraYear.Session = viewModel.Session;
				extraYear.LastSessionRegistered = viewModel.extraYear.LastSessionRegistered;
				extraYear.Sessions_Registered = viewModel.extraYear.Sessions_Registered;
				extraYear.DeferementCommencedSession = viewModel.extraYear.DeferementCommencedSession;
				extraYearLogic.Create(extraYear);

			}
			catch (Exception)
			{
				throw;
			}

			return extraYear;
		}

		private Payment CreatePayment(PaymentViewModel viewModel)
		{
			Payment newPayment = new Payment();
			try
			{
	
				Payment payment = new Payment();
				PaymentLogic paymentLogic = new PaymentLogic();
				payment.PaymentMode = new PaymentMode() { Id = 1 };
				payment.PaymentType = new PaymentType() { Id = 2 };
				payment.PersonType = new PersonType() { Id = 3 };
				payment.FeeType = new FeeType() { Id = 10 };
				payment.DatePaid = DateTime.Now;
				payment.Person = viewModel.Person;
				payment.Session = viewModel.Session;

				OnlinePayment newOnlinePayment = null;
				newPayment = paymentLogic.Create(payment);
				newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id, payment.PaymentMode.Id, viewModel.StudentLevel.Department.Id, viewModel.Session.Id);
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
					if (programme.Id <= 2)
					{
						levels = levelLogic.GetONDs();
					}
					else if (programme.Id > 2)
					{
						levels = levelLogic.GetHNDs();
					}
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
		public ActionResult Step_1(long sid)
		{
			try
			{
				List<Value> NoOfSessions = Utility.CreateNumberListFrom(1, 4);
				NoOfSessions.Insert(0, new Value() { Name = "--Select--" });

				viewModel.FeeType = new FeeType() { Id = (int)FeeTypes.SchoolFees };
				viewModel.PaymentMode = new PaymentMode() { Id = 1 };
				viewModel.PaymentType = new PaymentType() { Id = 2 };
			   
				ViewBag.States = viewModel.StateSelectListItem;
				ViewBag.Sessions = viewModel.SessionSelectListItem;
				ViewBag.LastSessions = viewModel.AllSessionSelectListItem;
				ViewBag.DeferredSessions = viewModel.AllSessionSelectListItem;
				ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
				ViewBag.SessionsRegistered = new SelectList(NoOfSessions, ID, NAME);
				ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
				ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);

				if (sid > 0)
				{
					viewModel.StudentAlreadyExist = true;
					viewModel.Person = personLogic.GetModelBy(p => p.Person_Id == sid);
					viewModel.Student = studentLogic.GetModelBy(s => s.Person_Id == sid);
					viewModel.StudentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == sid).LastOrDefault();
					viewModel.extraYear = extraYearLogic.GetModelsBy(s => s.Person_Id == sid).LastOrDefault();
					if (viewModel.StudentLevel != null && viewModel.StudentLevel.Programme.Id > 0)
					{
						ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Programme.Id);
					}
					viewModel.LevelSelectListItem = Utility.PopulateLevelSelectListItem();
					ViewBag.Levels = viewModel.LevelSelectListItem;
					viewModel.StudentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == sid).LastOrDefault();
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
					//else//Ugo
					//{
					//    ViewBag.Levels = new SelectList(new List<Level>(), Utility.ID, Utility.NAME);
					//}

					if (viewModel.extraYear != null && viewModel.extraYear.Id > 0)
					{
						if (viewModel.extraYear.Session != null && viewModel.extraYear.Session.Id > 0)
						{
							ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.extraYear.Session.Id);
					   
						}

						if (viewModel.extraYear.LastSessionRegistered != null && viewModel.extraYear.LastSessionRegistered.Id > 0)
						{
							ViewBag.LastSessions = new SelectList(viewModel.AllSessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.extraYear.LastSessionRegistered.Id);
						}

						if (viewModel.extraYear.DeferementCommencedSession != null && viewModel.extraYear.DeferementCommencedSession.Id > 0)
						{
							ViewBag.DeferredSessions = new SelectList(viewModel.AllSessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.extraYear.DeferementCommencedSession.Id);
					   
						}

						if (viewModel.extraYear.Sessions_Registered > 0)
						{
							ViewBag.SessionsRegistered = new SelectList(NoOfSessions, Utility.ID, Utility.NAME, viewModel.extraYear.Sessions_Registered);
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
				if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
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
		public ActionResult Step_1(PaymentViewModel viewModel)
		{
			try
			{
				Session session = new Session(){ Id = (int)Sessions._20172018 };

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

					//FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
					//decimal Amt = 0M;
					//Amt = feeDetailLogic.GetFeeByDepartmentLevel(viewModel.StudentLevel.Department,
					//                        viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme, viewModel.FeeType,
					//                        viewModel.Session, new PaymentMode() { Id = (int)PaymentModes.Full });

					Payment payment = null;
					StudentExtraYearSession extraYear = null;
					if (viewModel.StudentAlreadyExist == false)
					{
						using (TransactionScope transaction = new TransactionScope())
						{
							CreatePerson(viewModel);
							CreateStudent(viewModel);
							payment = CreatePayment(viewModel);
							CreateStudentLevel(viewModel);
							extraYear = AddExtraYearDetails(viewModel);
							transaction.Complete();

							if (payment != null && extraYear != null)
							{
								//payment.FeeDetails = paymentLogic.SetFeeDetails(payment, viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id, 1, viewModel.StudentLevel.Department.Id, viewModel.Session.Id);

								//int lastSession = Convert.ToInt32(extraYear.LastSessionRegistered.Name.Substring(0, 4));
								//int currentSession = Convert.ToInt32(payment.Session.Name.Substring(0, 4));
								//int NoOfOutstandingSession = currentSession - lastSession;
								//if (NoOfOutstandingSession == 0)
								//{
								//    NoOfOutstandingSession = 1;
								//}

								//decimal Amt = 0M;
								//Amt = payment.FeeDetails.Sum(p => p.Fee.Amount) * NoOfOutstandingSession;

								////Get Payment Specific Setting
								//RemitaSettings settings = new RemitaSettings();
								//RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
								//settings = settingsLogic.GetBy(1);

								////Get Split Specific details;
								//List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
								//RemitaSplitItems singleItem = new RemitaSplitItems();
								//RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
								//singleItem = splitItemLogic.GetBy(3);
								//singleItem.deductFeeFrom = "1";
								//singleItem.beneficiaryAmount = "5000";
								//splitItems.Add(singleItem);
								//singleItem = splitItemLogic.GetBy(1);
								//singleItem.deductFeeFrom = "0";
								//singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
								//splitItems.Add(singleItem);


								////Get BaseURL
								//string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
								//RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
								//viewModel.RemitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "CARRY OVER SCHOOL FEES", splitItems, settings, Amt);
								//if (viewModel.RemitaPayment != null)
								//{
								//    transaction.Complete();
								//}
								transaction.Complete();
							}
						}
					}
					else
					{
						using (TransactionScope transaction = new TransactionScope())
						{
							personLogic.Modify(viewModel.Person);
							FeeType feeType = new FeeType() { Id = (int)FeeTypes.CarryOverSchoolFees };
							payment = paymentLogic.GetBy(feeType, viewModel.Person, viewModel.Session);
							StudentLevel studentLevel = studentLevelLogic.GetModelBy(p => p.Session_Id == viewModel.Session.Id && p.Person_Id == viewModel.Person.Id);
							if (studentLevel == null)
							{
								viewModel.Student.Id = viewModel.Person.Id;
								 CreateStudentLevel(viewModel);
							}
							if (payment == null || payment.Id <= 0)
							{
								payment = CreatePayment(viewModel);
								extraYear = AddExtraYearDetails(viewModel);
							}

							extraYear = extraYearLogic.GetBy(viewModel.Person.Id, viewModel.Session.Id);
							if (extraYear == null || extraYear.Id <= 0)
							{
								extraYear = AddExtraYearDetails(viewModel);
							}
							else
							{
								extraYear.LastSessionRegistered = viewModel.extraYear.LastSessionRegistered;
								extraYear.Session = viewModel.Session;
								extraYear.Sessions_Registered = viewModel.extraYear.Sessions_Registered;
								extraYear.DeferementCommencedSession = viewModel.extraYear.DeferementCommencedSession;
								extraYearLogic.Modify(extraYear);
							}

							//if (payment != null && extraYear != null)
							//{
							//    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
							//    RemitaPayment remitaPayment = remitaPaymentLogic.GetBy(payment.Id);

							//    if (remitaPayment == null)
							//    {
							//        payment.FeeDetails = paymentLogic.SetFeeDetails(payment, viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id, 1, viewModel.StudentLevel.Department.Id, viewModel.Session.Id);

							//        int lastSession = Convert.ToInt32(extraYear.LastSessionRegistered.Name.Substring(0, 4));
							//        int currentSession = Convert.ToInt32(payment.Session.Name.Substring(0, 4));
							//        int NoOfOutstandingSession = currentSession - lastSession;
							//        if (NoOfOutstandingSession == 0)
							//        {
							//            NoOfOutstandingSession = 1;
							//        }

							//        decimal Amt = 0M;
							//        Amt = payment.FeeDetails.Sum(p => p.Fee.Amount) * NoOfOutstandingSession;

							//        //Get Payment Specific Setting
							//        RemitaSettings settings = new RemitaSettings();
							//        RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
							//        settings = settingsLogic.GetBy(1);

							//        //Get Split Specific details;
							//        List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
							//        RemitaSplitItems singleItem = new RemitaSplitItems();
							//        RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
							//        singleItem = splitItemLogic.GetBy(3);
							//        singleItem.deductFeeFrom = "1";
							//        singleItem.beneficiaryAmount = "5000";
							//        splitItems.Add(singleItem);
							//        singleItem = splitItemLogic.GetBy(1);
							//        singleItem.deductFeeFrom = "0";
							//        singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
							//        splitItems.Add(singleItem);


							//        //Get BaseURL
							//        string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
							//        RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
							//        viewModel.RemitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "CARRY OVER SCHOOL FEES", splitItems, settings, Amt);
							//        if (viewModel.RemitaPayment != null)
							//        {
							//            transaction.Complete();
							//        }
							//        //transaction.Complete();
							//    }
							//    else
							//    {
							//        transaction.Complete();
							//    }
							//}
							transaction.Complete();
						}
					}

					viewModel.Payment = payment;
					TempData["PaymentViewModel"] = viewModel;
					return RedirectToAction("Step_2", "ExtraYear", new { Area = "Student", sid = Abundance_Nk.Web.Models.Utility.Encrypt(viewModel.Person.Id.ToString()), sesid = viewModel.Session.Id });
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			KeepInvoiceGenerationDropDownState(viewModel);
			return View(viewModel);
		}
		public ActionResult Step_2(string sid, int sesid)
		{
			try
			{
				courseRegViewModel = new CourseRegistrationViewModel();
				long StudentId = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(sid));

				StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
				SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
				SessionSemester sessionSemester = null;
				if (sesid > 0)
				{
					sessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == sesid).LastOrDefault();
				}
				else
				{
					StudentLevel studentLevel = studentLevelLogic.GetBy(StudentId);
					sessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == studentLevel.Session.Id).LastOrDefault(); 
				}
				  
				PopulateCourseRegistrationForm(StudentId, sessionSemester);
				viewModel = new PaymentViewModel();
				viewModel = (PaymentViewModel)TempData["PaymentViewModel"];
				courseRegViewModel.invoiceNumber = viewModel.Payment.InvoiceNumber;
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			TempData["CourseRegistrationViewModel"] = courseRegViewModel;
			return View(courseRegViewModel);
		}
		private void PopulateCourseRegistrationForm(long sid, SessionSemester sessionSemester)
		{
			List<Course> firstSemesterCourses = null;
			List<Course> secondSemesterCourses = null;

			courseRegViewModel.Student = studentLogic.GetBy(sid);
			if (courseRegViewModel.Student != null && courseRegViewModel.Student.Id > 0)
			{
				CourseMode firstAttempt = new CourseMode() { Id = 1 };
				CourseMode carryOver = new CourseMode() { Id = 2 };
				Semester firstSemester = new Semester() { Id = 1 };
				Semester secondSemester = new Semester() { Id = 2 };

				//CurrentSessionSemesterLogic currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
				//courseRegViewModel.CurrentSessionSemester = currentSessionSemesterLogic.GetCurrentSessionSemester();
				courseRegViewModel.CurrentSessionSemester = new CurrentSessionSemester(){SessionSemester = sessionSemester};
			   
				StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
				//courseRegViewModel.StudentLevel = studentLevelLogic.GetExtraYearBy(courseRegViewModel.Student.Id);
				courseRegViewModel.StudentLevel = studentLevelLogic.GetModelBy(s => s.Person_Id == courseRegViewModel.Student.Id && s.Session_Id == courseRegViewModel.CurrentSessionSemester.SessionSemester.Session.Id);
				if (courseRegViewModel.StudentLevel != null && courseRegViewModel.StudentLevel.Id > 0 && courseRegViewModel.StudentLevel.DepartmentOption == null)
				{
					firstSemesterCourses = courseLogic.GetExtraYearBy(courseRegViewModel.StudentLevel.Department, courseRegViewModel.StudentLevel.Level, firstSemester, true);
					secondSemesterCourses = courseLogic.GetExtraYearBy(courseRegViewModel.StudentLevel.Department, courseRegViewModel.StudentLevel.Level, secondSemester, true);
					SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, courseRegViewModel.StudentLevel.Department, courseRegViewModel.StudentLevel.Level, courseRegViewModel.StudentLevel.DepartmentOption);
				}
				else if (courseRegViewModel.StudentLevel != null && courseRegViewModel.StudentLevel.Id > 0 && courseRegViewModel.StudentLevel.DepartmentOption != null && courseRegViewModel.StudentLevel.DepartmentOption.Id > 0)
				{
					firstSemesterCourses = courseLogic.GetExtraYearBy(courseRegViewModel.StudentLevel.Department, courseRegViewModel.StudentLevel.DepartmentOption, courseRegViewModel.StudentLevel.Level, firstSemester, true);
					secondSemesterCourses = courseLogic.GetExtraYearBy(courseRegViewModel.StudentLevel.Department, courseRegViewModel.StudentLevel.DepartmentOption, courseRegViewModel.StudentLevel.Level, secondSemester, true);
					SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, courseRegViewModel.StudentLevel.Level, courseRegViewModel.StudentLevel.DepartmentOption);
				}

				//get courses if already registered
				CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
				CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
				CourseRegistration courseRegistration = courseRegistrationLogic.GetBy(courseRegViewModel.Student, courseRegViewModel.StudentLevel.Level, courseRegViewModel.StudentLevel.Programme, courseRegViewModel.StudentLevel.Department, courseRegViewModel.CurrentSessionSemester.SessionSemester.Session);
				if (courseRegistration != null && courseRegistration.Id > 0)
				{
					courseRegViewModel.RegisteredCourse = courseRegistration;
					if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
					{
						//split registered courses by semester
						List<CourseRegistrationDetail> firstSemesterRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == firstSemester.Id).ToList();
						List<CourseRegistrationDetail> secondSemesterRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == secondSemester.Id).ToList();
						List<CourseRegistrationDetail> firstSemesterCarryOverRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == firstSemester.Id && rc.Mode.Id == carryOver.Id).ToList();
						List<CourseRegistrationDetail> secondSemesterCarryOverRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == secondSemester.Id && rc.Mode.Id == carryOver.Id).ToList();

						//get registered courses
						courseRegViewModel.FirstSemesterCourses = GetRegisteredCourse(courseRegistration, firstSemesterCourses, firstSemester, firstSemesterRegisteredCourseDetails, firstAttempt);
						courseRegViewModel.SecondSemesterCourses = GetRegisteredCourse(courseRegistration, secondSemesterCourses, secondSemester, secondSemesterRegisteredCourseDetails, firstAttempt);

						//get carry over courses
						List<Course> firstSemesterCarryOverCourses = courseRegistrationDetailLogic.GetCarryOverCoursesBy(courseRegistration, firstSemester);
						List<Course> secondSemesterCarryOverCourses = courseRegistrationDetailLogic.GetCarryOverCoursesBy(courseRegistration, secondSemester);
						courseRegViewModel.FirstSemesterCarryOverCourses = GetRegisteredCourse(courseRegistration, firstSemesterCarryOverCourses, firstSemester, firstSemesterCarryOverRegisteredCourseDetails, carryOver);
						courseRegViewModel.SecondSemesterCarryOverCourses = GetRegisteredCourse(courseRegistration, secondSemesterCarryOverCourses, secondSemester, secondSemesterCarryOverRegisteredCourseDetails, carryOver);

						if (courseRegViewModel.FirstSemesterCarryOverCourses != null && courseRegViewModel.FirstSemesterCarryOverCourses.Count > 0)
						{
							courseRegViewModel.CarryOverExist = true;
							courseRegViewModel.CarryOverCourses.AddRange(courseRegViewModel.FirstSemesterCarryOverCourses);
							courseRegViewModel.TotalFirstSemesterCarryOverCourseUnit = courseRegViewModel.FirstSemesterCarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
						}
						if (courseRegViewModel.SecondSemesterCarryOverCourses != null && courseRegViewModel.SecondSemesterCarryOverCourses.Count > 0)
						{
							courseRegViewModel.CarryOverExist = true;
							courseRegViewModel.CarryOverCourses.AddRange(courseRegViewModel.SecondSemesterCarryOverCourses);
							courseRegViewModel.TotalSecondSemesterCarryOverCourseUnit = courseRegViewModel.SecondSemesterCarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);
						}

						//set total selected course units
						courseRegViewModel.SumOfFirstSemesterSelectedCourseUnit = SumSemesterSelectedCourseUnit(firstSemesterRegisteredCourseDetails);
						courseRegViewModel.SumOfSecondSemesterSelectedCourseUnit = SumSemesterSelectedCourseUnit(secondSemesterRegisteredCourseDetails);
						courseRegViewModel.CourseAlreadyRegistered = true;
					}
					else
					{
						courseRegViewModel.FirstSemesterCourses = GetUnregisteredCourseDetail(firstSemesterCourses, firstSemester);
						courseRegViewModel.SecondSemesterCourses = GetUnregisteredCourseDetail(secondSemesterCourses, secondSemester);
						courseRegViewModel.CourseAlreadyRegistered = false;
						//get carry over courses
						courseRegViewModel.CarryOverCourses = courseRegistrationDetailLogic.GetCarryOverBy(courseRegViewModel.Student, courseRegViewModel.CurrentSessionSemester.SessionSemester.Session);


						if (courseRegViewModel.CarryOverCourses != null && courseRegViewModel.CarryOverCourses.Count > 0)
						{
							courseRegViewModel.CarryOverExist = true;
							courseRegViewModel.TotalFirstSemesterCarryOverCourseUnit = courseRegViewModel.CarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
							courseRegViewModel.TotalSecondSemesterCarryOverCourseUnit = courseRegViewModel.CarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);

							if (courseRegViewModel.TotalFirstSemesterCarryOverCourseUnit <= courseRegViewModel.FirstSemesterMaximumUnit && courseRegViewModel.TotalSecondSemesterCarryOverCourseUnit <= courseRegViewModel.SecondSemesterMaximumUnit)
							{
								foreach (CourseRegistrationDetail carryOverCourse in courseRegViewModel.CarryOverCourses)
								{
									carryOverCourse.Course.IsRegistered = true;
								}
							}
						}
					}
				}
				else
				{
					courseRegViewModel.FirstSemesterCourses = GetUnregisteredCourseDetail(firstSemesterCourses, firstSemester);
					courseRegViewModel.SecondSemesterCourses = GetUnregisteredCourseDetail(secondSemesterCourses, secondSemester);
					courseRegViewModel.CourseAlreadyRegistered = false;
					//get carry over courses
					courseRegViewModel.CarryOverCourses = courseRegistrationDetailLogic.GetCarryOverBy(courseRegViewModel.Student, courseRegViewModel.CurrentSessionSemester.SessionSemester.Session);
					

					if (courseRegViewModel.CarryOverCourses != null && courseRegViewModel.CarryOverCourses.Count > 0)
					{
						courseRegViewModel.CarryOverExist = true;
						courseRegViewModel.TotalFirstSemesterCarryOverCourseUnit = courseRegViewModel.CarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
						courseRegViewModel.TotalSecondSemesterCarryOverCourseUnit = courseRegViewModel.CarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);

						if (courseRegViewModel.TotalFirstSemesterCarryOverCourseUnit <= courseRegViewModel.FirstSemesterMaximumUnit && courseRegViewModel.TotalSecondSemesterCarryOverCourseUnit <= courseRegViewModel.SecondSemesterMaximumUnit)
						{
							foreach (CourseRegistrationDetail carryOverCourse in courseRegViewModel.CarryOverCourses)
							{
								carryOverCourse.Course.IsRegistered = true;
							}
						}
					}
				}


				//}
			}
		}
		private void SetMinimumAndMaximumCourseUnit(Semester firstSemester, Semester secondSemester, Department departmemt, Level level, DepartmentOption departmentOption)
		{
			try
			{
				CourseUnitLogic courseUnitLogic = new CourseUnitLogic();
				CourseUnit firstSemesterCourseUnit = courseUnitLogic.GetBy(departmemt, level, firstSemester, departmentOption);
				if (firstSemesterCourseUnit != null && firstSemesterCourseUnit.Id > 0)
				{
					courseRegViewModel.FirstSemesterMinimumUnit = firstSemesterCourseUnit.MinimumUnit;
					courseRegViewModel.FirstSemesterMaximumUnit = firstSemesterCourseUnit.MaximumUnit;
				}

				CourseUnit secondSemesterCourseUnit = courseUnitLogic.GetBy(departmemt, level, secondSemester, departmentOption);
				if (secondSemesterCourseUnit != null && secondSemesterCourseUnit.Id > 0)
				{
					courseRegViewModel.SecondSemesterMinimumUnit = secondSemesterCourseUnit.MinimumUnit;
					courseRegViewModel.SecondSemesterMaximumUnit = secondSemesterCourseUnit.MaximumUnit;
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}
		[HttpPost]
		public ActionResult Step_2(CourseRegistrationViewModel viewModel)
		{
			string message = null;

			try
			{
				//List<CourseRegistrationDetail> selectedFirstSemesterCourseRegistrationDetails = null;
				//List<CourseRegistrationDetail> selectedSecondSemesterCourseRegistrationDetails = null;
				//List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();

				//if (viewModel.CarryOverExist)
				//{
				//    List<CourseRegistrationDetail> selectedCarryOverCourseRegistrationDetails = new List<CourseRegistrationDetail>();
				//    selectedCarryOverCourseRegistrationDetails = GetSelectedCourses(viewModel.CarryOverCourses);
				//    courseRegistrationDetails.AddRange(selectedCarryOverCourseRegistrationDetails);
				//}

				//viewModel.RegisteredCourse.Student = viewModel.Student;

				//CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
				//if (viewModel.CourseAlreadyRegistered) //modify
				//{
				//    selectedFirstSemesterCourseRegistrationDetails = viewModel.FirstSemesterCourses;
				//    selectedSecondSemesterCourseRegistrationDetails = viewModel.SecondSemesterCourses;

				//    //courseRegistrationDetails = selectedFirstSemesterCourseRegistrationDetails;
				//    courseRegistrationDetails.AddRange(selectedFirstSemesterCourseRegistrationDetails);
				//    courseRegistrationDetails.AddRange(selectedSecondSemesterCourseRegistrationDetails);
				//    viewModel.RegisteredCourse.Details = courseRegistrationDetails;

				//    bool modified = courseRegistrationLogic.Modify(viewModel.RegisteredCourse);
				//    if (modified)
				//    {
				//        message = "Selected courses has been successfully modified.";
				//    }
				//    else
				//    {
				//        message = "Course Registration modification Failed! Please try again.";
				//    }
				//}
				//else //insert
				//{
				//    selectedFirstSemesterCourseRegistrationDetails = GetSelectedCourses(viewModel.FirstSemesterCourses);
				//    selectedSecondSemesterCourseRegistrationDetails = GetSelectedCourses(viewModel.SecondSemesterCourses);

				//    courseRegistrationDetails.AddRange(selectedFirstSemesterCourseRegistrationDetails);
				//    courseRegistrationDetails.AddRange(selectedSecondSemesterCourseRegistrationDetails);
				//    viewModel.RegisteredCourse.Details = courseRegistrationDetails;
					
				//    viewModel.RegisteredCourse.Level = new Level() { Id = viewModel.StudentLevel.Level.Id };
				//    viewModel.RegisteredCourse.Programme = new Programme() { Id = viewModel.StudentLevel.Programme.Id };
				//    viewModel.RegisteredCourse.Department = new Department() { Id = viewModel.StudentLevel.Department.Id };
				//    viewModel.RegisteredCourse.Session = new Session() { Id = viewModel.CurrentSessionSemester.SessionSemester.Session.Id };

				//    CourseRegistration courseRegistration = courseRegistrationLogic.Create(viewModel.RegisteredCourse);
				//    if (courseRegistration != null)
				//    {
				//        message = "Selected courses has been successfully registered.";
				//    }
				//    else
				//    {
				//        message = "Course Registration Failed! Please try again.";
				//    }
				//}
			}
			catch (Exception ex)
			{
				message = "Error Occurred! " + ex.Message + ". Please try again.";
			}

		   return Json(new { message = message }, "text/html", JsonRequestBehavior.AllowGet);
		  }
		private List<CourseRegistrationDetail> GetRegisteredCourse(CourseRegistration courseRegistration, List<Course> courses, Semester semester, List<CourseRegistrationDetail> registeredCourseDetails, CourseMode courseMode)
		{
			try
			{
				List<CourseRegistrationDetail> courseRegistrationDetails = null;
				if (registeredCourseDetails != null && registeredCourseDetails.Count > 0)
				{
					if (courses != null && courses.Count > 0)
					{
						courseRegistrationDetails = new List<CourseRegistrationDetail>();
						foreach (Course course in courses)
						{
							CourseRegistrationDetail registeredCourseDetail = registeredCourseDetails.LastOrDefault(c => c.Course.Id == course.Id && c.Mode.Id == courseMode.Id);
							if (registeredCourseDetail != null && registeredCourseDetail.Id > 0)
							{
								registeredCourseDetail.Course.IsRegistered = true;
								courseRegistrationDetails.Add(registeredCourseDetail);
							}
							else
							{
								CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();

								courseRegistrationDetail.Course = course;
								courseRegistrationDetail.Semester = semester;
								courseRegistrationDetail.Course.IsRegistered = false;
								//courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };

								courseRegistrationDetail.Mode = courseMode;
								courseRegistrationDetail.CourseRegistration = courseRegistration;

								courseRegistrationDetails.Add(courseRegistrationDetail);
							}
						}
					}
				}

				return courseRegistrationDetails;
			}
			catch (Exception)
			{
				throw;
			}
		}
		private List<CourseRegistrationDetail> GetUnregisteredCourseDetail(List<Course> courses, Semester semester)
		{
			try
			{
				List<CourseRegistrationDetail> courseRegistrationDetails = null;
				if (courses != null && courses.Count > 0)
				{
					courseRegistrationDetails = new List<CourseRegistrationDetail>();
					foreach (Course course in courses)
					{
						CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
						courseRegistrationDetail.Course = course;
						courseRegistrationDetail.Semester = semester;
						courseRegistrationDetail.Course.IsRegistered = false;
						courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };

						courseRegistrationDetails.Add(courseRegistrationDetail);
					}
				}

				return courseRegistrationDetails;
			}
			catch (Exception)
			{
				throw;
			}
		}
		private int SumSemesterSelectedCourseUnit(List<CourseRegistrationDetail> semesterRegisteredCourseDetails)
		{
			try
			{
				int totalRegisteredCourseUnit = 0;
				if (semesterRegisteredCourseDetails != null && semesterRegisteredCourseDetails.Count > 0)
				{
					totalRegisteredCourseUnit = semesterRegisteredCourseDetails.Sum(c => c.Course.Unit);
				}

				return totalRegisteredCourseUnit;
			}
			catch (Exception)
			{
				throw;
			}
		}
		private List<CourseRegistrationDetail> GetSelectedCourses(List<CourseRegistrationDetail> coursesToRegister)
		{
			try
			{
				List<CourseRegistrationDetail> selectedCourseDetails = null;

				if (coursesToRegister != null && coursesToRegister.Count > 0)
				{
					selectedCourseDetails = coursesToRegister.Where(c => c.Course.IsRegistered == true).ToList();
				}

				return selectedCourseDetails;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public ActionResult Invoice(string ivn)
		{
			Invoice invoice = new Model.Model.Invoice();
			   
			try
			{
				if (string.IsNullOrEmpty(ivn))
				{
					SetMessage("Invoice Not Found! Refresh and Try again ", Message.Category.Error);
				}

				invoice = GetInvoiceBy(ivn);
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(invoice);
		}
		public Invoice GetInvoiceBy(string invoiceNumber)
		{
			Invoice Invoice = new Invoice();
						   
			try
			{
				Payment payment = paymentLogic.GetBy(invoiceNumber);
				if (payment.FeeType.Id == (int)FeeTypes.CarryOverSchoolFees)
				{
					StudentLevel studentLevel = new StudentLevel();
					StudentLevelLogic levelLogic = new StudentLevelLogic();
					studentLevel = studentLevelLogic.GetModelsBy(sl => sl.Person_Id == payment.Person.Id && sl.Session_Id == payment.Session.Id).LastOrDefault();
					if(studentLevel == null)
					{
						studentLevel = levelLogic.GetExtraYearBy(payment.Person.Id);
					}                     
					SessionLogic sessionLogic = new SessionLogic();
					//Session session = sessionLogic.GetModelBy(s => s.Activated == true);
					Session session = payment.Session;
					if (studentLevel != null && studentLevel.Session.Id != session.Id)
					{
						CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
						List<CourseRegistrationDetail> courseRegistrationDetails = courseRegistrationDetailLogic.GetModelsBy(crd => crd.STUDENT_COURSE_REGISTRATION.Person_Id == studentLevel.Student.Id && crd.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id);
						if (courseRegistrationDetails.Count > 0)
						{
							studentLevel.Session = session;
						}
					}
					if (studentLevel != null)
					{

						payment.FeeDetails = paymentLogic.SetFeeDetails(payment, studentLevel.Programme.Id, studentLevel.Level.Id, 1, studentLevel.Department.Id, studentLevel.Session.Id);
					   
						PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
						PaymentEtranzactTypeLogic PaymentEtranzactTypeLogic = new Business.PaymentEtranzactTypeLogic();
						paymentEtranzactType = PaymentEtranzactTypeLogic.GetModelBy(p => p.Level_Id == studentLevel.Level.Id && p.Payment_Mode_Id == payment.PaymentMode.Id && p.Fee_Type_Id == payment.FeeType.Id && p.Programme_Id == studentLevel.Programme.Id && p.Session_Id == payment.Session.Id);
					   
						StudentExtraYearSession extraYear = new StudentExtraYearSession();
						StudentExtraYearLogic extraYearLogic = new StudentExtraYearLogic();
						extraYear = extraYearLogic.GetBy(payment.Person.Id, payment.Session.Id);
					
						if (extraYear != null)
						{
							int lastSession = Convert.ToInt32(extraYear.LastSessionRegistered.Name.Substring(0,4));
							int currentSession = Convert.ToInt32(payment.Session.Name.Substring(0,4));
							int NoOfOutstandingSession = currentSession - lastSession;
							if (NoOfOutstandingSession == 0)
							{
								NoOfOutstandingSession = 1;
							}

							Invoice.Payment = payment;
							Invoice.Person = payment.Person;
							Invoice.JambRegistrationNumber = "";
							Invoice.Amount = payment.FeeDetails.Sum(p => p.Fee.Amount) * NoOfOutstandingSession;
							Invoice.MatricNumber = studentLevel.Student.MatricNumber;
							Invoice.paymentEtranzactType = paymentEtranzactType;
						  
						}

					}

				}


				
			}
			catch (Exception)
			{
				throw;
			}

			return Invoice;
		}
		public ActionResult Step_3()
		{
			logonViewModel = new RegistrationLogonViewModel();
			ViewBag.Session = logonViewModel.SessionSelectListItem;
			return View(logonViewModel);
		}
		
		public ActionResult Step_4(string sid, int sesid)
		{
			try
			{
				long StudentId = Convert.ToInt64(sid);

				StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
				SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
				SessionSemester sessionSemester = null;
				if (sesid > 0)
				{
					sessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == sesid).LastOrDefault();
				}
				else
				{
					StudentLevel studentLevel = studentLevelLogic.GetBy(StudentId);
					sessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == studentLevel.Session.Id).LastOrDefault(); 
				} 

				courseRegViewModel = new CourseRegistrationViewModel();
				PopulateCourseRegistrationForm(StudentId, sessionSemester);
				
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(courseRegViewModel);
		}

	}
}