using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Business;
using System.Transactions;
using Abundance_Nk.Model.Entity.Model;
using System.IO;
using System.Configuration;

namespace Abundance_Nk.Web.Areas.Student.Controllers 
{
	[AllowAnonymous]
	public class InformationController : BaseController
	{
        //private const string ID = "Id";
        //private const string NAME = "Name";
        //private const string VALUE = "Value";
        //private const string TEXT = "Text";
        //private const string DEFAULT_PASSPORT = "/Content/Images/default_avatar.png";
        private string appRoot = ConfigurationManager.AppSettings["AppRoot"];
        private FormViewModel viewModel;

		public InformationController()
		{
			viewModel = new FormViewModel();
		}

		public ActionResult Form(long? fid, int? pid)
		{
			FormViewModel existingViewModel = (FormViewModel)TempData["FormViewModel"];

			try
			{
				PopulateAllDropDowns((int)pid);

				if (existingViewModel != null)
				{
					viewModel = existingViewModel;
					SetStudentUploadedPassport(viewModel);
				}
				SetDateOfBirth();
				SetLgaIfExist(viewModel);

				ApplicationForm applicationform = viewModel.GetApplicationFormBy((long)fid);
				if ((applicationform != null && applicationform.Id > 0) && viewModel.ApplicationAlreadyExist == false)
				{
					viewModel.ApplicationAlreadyExist = true;
					viewModel.LoadApplicantionFormBy((long)fid);

					SetSelectedSittingSubjectAndGrade(viewModel);
					SetLgaIfExist(viewModel);
					SetDepartmentIfExist(viewModel);
					SetDepartmentOptionIfExist(viewModel);

					SetDateOfBirth();

					SetLevel(viewModel);
					SetEntryAndStudyMode(viewModel);
					

					viewModel.Student.Type = new StudentType() { Id = 3 };
					if (viewModel.AppliedCourse.Programme.Id == 3 || viewModel.AppliedCourse.Programme.Id == 4)
					{
						SetPreviousEducationStartDate();
						SetPreviousEducationEndDate();

						viewModel.Student.Category = new StudentCategory() { Id = 2 };
					}
					else
					{
						viewModel.Student.Category = new StudentCategory() { Id = 1 };
					}

					Model.Model.Student student = viewModel.GetStudentBy(applicationform.Person.Id);
					if (student != null && student.Id > 0)
					{
						viewModel.StudentAlreadyExist = true;
						viewModel.LoadStudentInformationFormBy(applicationform.Person.Id);

						SetLastEmploymentStartDate();
						SetLastEmploymentEndDate();
						SetNdResultDateAwarded();
					}
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			TempData["FormViewModel"] = viewModel;
			TempData["imageUrl"] = viewModel.Person.ImageFileUrl;

			return View(viewModel);
		}

		private void SetNdResultDateAwarded()
		{
			try
			{
				if (viewModel.StudentNdResult != null && viewModel.StudentNdResult.DateAwarded != null)
				{
					if (viewModel.StudentNdResult.YearAwarded.Id > 0 && viewModel.StudentNdResult.MonthAwarded.Id > 0)
					{
						int noOfDays = DateTime.DaysInMonth(viewModel.StudentNdResult.YearAwarded.Id, viewModel.StudentNdResult.MonthAwarded.Id);
						List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
						if (days != null && days.Count > 0)
						{
							days.Insert(0, new Value() { Name = "--DD--" });
						}

						if (viewModel.StudentNdResult.DayAwarded != null && viewModel.StudentNdResult.DayAwarded.Id > 0)
						{
							ViewBag.StudentNdResultDayAwardedId = new SelectList(days, Utility.ID, Utility.NAME, viewModel.StudentNdResult.DayAwarded.Id);
						}
						else
						{
							ViewBag.StudentNdResultDayAwardedId = new SelectList(days, Utility.ID, Utility.NAME);
						}
					}
				}
				else
				{
					ViewBag.StudentNdResultDayAwardedId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}
	   
		private void SetLastEmploymentStartDate()
		{
			try
			{
				if (viewModel.StudentEmploymentInformation != null && viewModel.StudentEmploymentInformation.StartDate != null)
				{
					if (viewModel.StudentEmploymentInformation.StartYear.Id > 0 && viewModel.StudentEmploymentInformation.StartMonth.Id > 0)
					{
						int noOfDays = DateTime.DaysInMonth(viewModel.StudentEmploymentInformation.StartYear.Id, viewModel.StudentEmploymentInformation.StartMonth.Id);
						List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
						if (days != null && days.Count > 0)
						{
							days.Insert(0, new Value() { Name = "--DD--" });
						}

						if (viewModel.StudentEmploymentInformation.StartDay != null && viewModel.StudentEmploymentInformation.StartDay.Id > 0)
						{
							ViewBag.StudentLastEmploymentStartDayId = new SelectList(days, Utility.ID, Utility.NAME, viewModel.StudentEmploymentInformation.StartDay.Id);
						}
						else
						{
							ViewBag.StudentLastEmploymentStartDayId = new SelectList(days, Utility.ID, Utility.NAME);
						}
					}
				}
				else
				{
					ViewBag.StudentLastEmploymentStartDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetLastEmploymentEndDate()
		{
			try
			{
				if (viewModel.StudentEmploymentInformation != null && viewModel.StudentEmploymentInformation.EndDate != null)
				{
					if (viewModel.StudentEmploymentInformation.EndYear.Id > 0 && viewModel.StudentEmploymentInformation.EndMonth.Id > 0)
					{
						int noOfDays = DateTime.DaysInMonth(viewModel.StudentEmploymentInformation.EndYear.Id, viewModel.StudentEmploymentInformation.EndMonth.Id);
						List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
						if (days != null && days.Count > 0)
						{
							days.Insert(0, new Value() { Name = "--DD--" });
						}

						if (viewModel.StudentEmploymentInformation.EndDay != null && viewModel.StudentEmploymentInformation.EndDay.Id > 0)
						{
							ViewBag.StudentLastEmploymentEndDayId = new SelectList(days, Utility.ID, Utility.NAME, viewModel.StudentEmploymentInformation.EndDay.Id);
						}
						else
						{
							ViewBag.StudentLastEmploymentEndDayId = new SelectList(days, Utility.ID, Utility.NAME);
						}
					}
				}
				else
				{
					ViewBag.StudentLastEmploymentEndDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetLevel(FormViewModel vModel)
		{
			try
			{
				//set mode of entry
				vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry() { Id = vModel.AppliedCourse.Programme.Id };

				//set mode of study
				switch (vModel.AppliedCourse.Programme.Id)
				{
					case 1:
					case 2:
						{
							vModel.StudentAcademicInformation.Level = new Level() { Id = 1 };
							break;
						}
					case 3:
					case 4:
						{
							vModel.StudentAcademicInformation.Level = new Level() { Id = 3 };
							break;
						}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SetEntryAndStudyMode(FormViewModel vModel)
		{
			try
			{
				//set mode of entry
				vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry() { Id = vModel.AppliedCourse.Programme.Id };

				//set mode of study
				switch (vModel.AppliedCourse.Programme.Id)
				{
					case 1:
					case 3:
						{
							vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy() { Id = 1 };
							
							break;
						}
					case 2:
					case 4:
						{
							vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy() { Id = 2 };
							break;
						}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpPost]
		public ActionResult Form(FormViewModel viewModel)
		{
			try
			{

				SetStudentUploadedPassport(viewModel);
				ModelState.Remove("Student.FirstName");
				ModelState.Remove("Student.LastName");
				ModelState.Remove("Student.MobilePhone");

				if (viewModel.AppliedCourse.Programme.Id == 2 || viewModel.AppliedCourse.Programme.Id == 3 || viewModel.AppliedCourse.Programme.Id == 4)
				{
					ModelState.Remove("ApplicantJambDetail.JambRegistrationNumber");
				}
								
			   // if (ModelState.IsValid)
//{
				if (string.IsNullOrEmpty(viewModel.Person.ImageFileUrl) || viewModel.Person.ImageFileUrl == Utility.DEFAULT_AVATAR)
				{
					SetMessage("No Passport uploaded! Please upload your passport to continue.", Message.Category.Error);
					SetPostJAMBStateVariables(viewModel);
					return View(viewModel);
				}

					TempData["FormViewModel"] = viewModel;
					return RedirectToAction("FormPreview", "Information");
			   // }
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			SetPostJAMBStateVariables(viewModel);
			return View(viewModel);
		}

		private void SetPostJAMBStateVariables(FormViewModel viewModel)
		{
			try
			{
				TempData["FormViewModel"] = viewModel;
				TempData["imageUrl"] = viewModel.Person.ImageFileUrl;

				PopulateAllDropDowns(viewModel.AppliedCourse.Programme.Id);
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}
		
		public ActionResult FormPreview()
		{
			FormViewModel viewModel = (FormViewModel)TempData["FormViewModel"];

			try
			{
				if (viewModel != null)
				{
					viewModel.Person.DateOfBirth = new DateTime(viewModel.Person.YearOfBirth.Id, viewModel.Person.MonthOfBirth.Id,viewModel.Person.DayOfBirth.Id);
					viewModel.Person.State = viewModel.States.Where(m => m.Id == viewModel.Person.State.Id).SingleOrDefault();
					viewModel.Person.LocalGovernment = viewModel.Lgas.Where(m => m.Id == viewModel.Person.LocalGovernment.Id).SingleOrDefault();
					viewModel.Person.Sex = viewModel.Genders.Where(m => m.Id == viewModel.Person.Sex.Id).SingleOrDefault();
					viewModel.Sponsor.Relationship = viewModel.Relationships.Where(m => m.Id == viewModel.Sponsor.Relationship.Id).SingleOrDefault();
					viewModel.Person.Religion = viewModel.Religions.Where(m => m.Id == viewModel.Person.Religion.Id).SingleOrDefault();
					viewModel.Student.Title = viewModel.Titles.Where(m => m.Id == viewModel.Student.Title.Id).SingleOrDefault();
					viewModel.Student.MaritalStatus = viewModel.MaritalStatuses.Where(m => m.Id == viewModel.Student.MaritalStatus.Id).SingleOrDefault();

					if (viewModel.Student.BloodGroup != null && viewModel.Student.BloodGroup.Id > 0)
					{
						viewModel.Student.BloodGroup = viewModel.BloodGroups.Where(m => m.Id == viewModel.Student.BloodGroup.Id).SingleOrDefault();
					}
					if (viewModel.Student.Genotype != null && viewModel.Student.Genotype.Id > 0)
					{
						viewModel.Student.Genotype = viewModel.Genotypes.Where(m => m.Id == viewModel.Student.Genotype.Id).SingleOrDefault();
					}

					viewModel.StudentAcademicInformation.ModeOfEntry = viewModel.ModeOfEntries.Where(m => m.Id == viewModel.StudentAcademicInformation.ModeOfEntry.Id).SingleOrDefault();
					viewModel.StudentAcademicInformation.ModeOfStudy = viewModel.ModeOfStudies.Where(m => m.Id == viewModel.StudentAcademicInformation.ModeOfStudy.Id).SingleOrDefault();
					viewModel.Student.Category = viewModel.StudentCategories.Where(m => m.Id == viewModel.Student.Category.Id).SingleOrDefault();
					viewModel.Student.Type = viewModel.StudentTypes.Where(m => m.Id == viewModel.Student.Type.Id).SingleOrDefault();
					viewModel.StudentAcademicInformation.Level = viewModel.Levels.Where(m => m.Id == viewModel.StudentAcademicInformation.Level.Id).SingleOrDefault();
					viewModel.StudentFinanceInformation.Mode = viewModel.ModeOfFinances.Where(m => m.Id == viewModel.StudentFinanceInformation.Mode.Id).SingleOrDefault();
					viewModel.StudentSponsor.Relationship = viewModel.Relationships.Where(m => m.Id == viewModel.StudentSponsor.Relationship.Id).SingleOrDefault();
					
					viewModel.FirstSittingOLevelResult.Type = viewModel.OLevelTypes.Where(m => m.Id == viewModel.FirstSittingOLevelResult.Type.Id).SingleOrDefault();
					if (viewModel.SecondSittingOLevelResult.Type != null)
					{
						viewModel.SecondSittingOLevelResult.Type = viewModel.OLevelTypes.Where(m => m.Id == viewModel.SecondSittingOLevelResult.Type.Id).SingleOrDefault();
					}

					//if (viewModel.AppliedCourse.Option == null || viewModel.AppliedCourse.Option.Id <= 0)
					//{
					//    viewModel.AppliedCourse.Option = new DepartmentOption();
					//    viewModel.AppliedCourse.Option.Name = viewModel.AppliedCourse.Department.Name;
					//}
 
					if (viewModel.AppliedCourse.Programme.Id == 3 || viewModel.AppliedCourse.Programme.Id == 4)
					{
						viewModel.ApplicantPreviousEducation.StartDate = new DateTime(viewModel.ApplicantPreviousEducation.StartYear.Id, viewModel.ApplicantPreviousEducation.StartMonth.Id, viewModel.ApplicantPreviousEducation.StartDay.Id);
						viewModel.ApplicantPreviousEducation.EndDate = new DateTime(viewModel.ApplicantPreviousEducation.EndYear.Id, viewModel.ApplicantPreviousEducation.EndMonth.Id, viewModel.ApplicantPreviousEducation.EndDay.Id);
						viewModel.StudentEmploymentInformation.StartDate = new DateTime(viewModel.StudentEmploymentInformation.StartYear.Id, viewModel.StudentEmploymentInformation.StartMonth.Id, viewModel.StudentEmploymentInformation.StartDay.Id);
						viewModel.StudentEmploymentInformation.EndDate = new DateTime(viewModel.StudentEmploymentInformation.EndYear.Id, viewModel.StudentEmploymentInformation.EndMonth.Id, viewModel.StudentEmploymentInformation.EndDay.Id);
						viewModel.StudentNdResult.DateAwarded = new DateTime(viewModel.StudentNdResult.YearAwarded.Id, viewModel.StudentNdResult.MonthAwarded.Id, viewModel.StudentNdResult.DayAwarded.Id);
						viewModel.ApplicantPreviousEducation.ResultGrade = viewModel.ResultGrades.Where(m => m.Id == viewModel.ApplicantPreviousEducation.ResultGrade.Id).SingleOrDefault();
					}
				  
					UpdateOLevelResultDetail(viewModel);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			TempData["FormViewModel"] = viewModel;
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult FormPreview(FormViewModel vm)
		{
			Abundance_Nk.Model.Model.Student newStudent = null;
			FormViewModel viewModel = (FormViewModel)TempData["FormViewModel"];

			try
			{
				if (viewModel.Applicant.Status.Id != (int)ApplicantStatus.Status.CompletedStudentInformationForm)
				{
					using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
					{
						viewModel.Student.ApplicationForm = viewModel.ApplicationForm;
						viewModel.Student.Id = viewModel.Person.Id;
						viewModel.Student.Status = new StudentStatus() { Id = 1 };
						StudentLogic studentLogic = new StudentLogic();
						PersonLogic personLogic = new PersonLogic(); //ugo 2/4/2016
						personLogic.Modify(viewModel.Person);

						newStudent = viewModel.Student;
						studentLogic.Modify(viewModel.Student);
						//newStudent = studentLogic.Create(viewModel.Student);


						viewModel.StudentSponsor.Student = newStudent;
						StudentSponsorLogic sponsorLogic = new StudentSponsorLogic();
						if (sponsorLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
						{
							sponsorLogic.Modify(viewModel.StudentSponsor);
						}
						else
						{
							sponsorLogic.Create(viewModel.StudentSponsor);
						}

						viewModel.StudentAcademicInformation.Student = newStudent;
						StudentAcademicInformationLogic academicInformationLogic = new StudentAcademicInformationLogic();
						if (academicInformationLogic.GetModelBy(a => a.Person_Id == newStudent.Id) != null)
						{
							academicInformationLogic.Modify(viewModel.StudentAcademicInformation);
						}
						else
						{
							academicInformationLogic.Create(viewModel.StudentAcademicInformation);
						}


						viewModel.StudentFinanceInformation.Student = newStudent;
						StudentFinanceInformationLogic financeInformationLogic = new StudentFinanceInformationLogic();
						if (financeInformationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
						{
							financeInformationLogic.Modify(viewModel.StudentFinanceInformation);
						}
						else
						{
							financeInformationLogic.Create(viewModel.StudentFinanceInformation);
						}


						if (viewModel.AppliedCourse.Programme.Id == 3 || viewModel.AppliedCourse.Programme.Id == 4)
						{
							viewModel.StudentEmploymentInformation.Student = newStudent;
							StudentEmploymentInformationLogic employmentInformationLogic = new StudentEmploymentInformationLogic();
							StudentEmploymentInformation studentEmploymentInformation = new StudentEmploymentInformation();
							studentEmploymentInformation = employmentInformationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault();
							if (studentEmploymentInformation != null)
							{
								//ugo 2/4/2016

								employmentInformationLogic.Modify(studentEmploymentInformation);
							}
							else
							{

								employmentInformationLogic.Create(viewModel.StudentEmploymentInformation);

							}


							viewModel.StudentNdResult.Student = newStudent;
							StudentNdResultLogic ndResultLogic = new StudentNdResultLogic();
							if (ndResultLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
							{
								ndResultLogic.Modify(viewModel.StudentNdResult);
							}
							else
							{
								ndResultLogic.Create(viewModel.StudentNdResult);
							}
						}

						//update applicant status
						ApplicantLogic applicantLogic = new ApplicantLogic();
						applicantLogic.UpdateStatus(viewModel.ApplicationForm, ApplicantStatus.Status.CompletedStudentInformationForm);

						transaction.Complete();
					}
				}

				TempData["FormViewModel"] = viewModel;
				return RedirectToAction("AcknowledgementSlip", "Information");
			}
			catch (Exception ex)
			{
				newStudent = null;
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			TempData["FormViewModel"] = viewModel;
			return View(viewModel);
		}

		public ActionResult AcknowledgementSlip()
		{
			FormViewModel existingViewModel = (FormViewModel)TempData["FormViewModel"];

			TempData["FormViewModel"] = existingViewModel;
			return View(existingViewModel);
		}

		private void UpdateOLevelResultDetail(FormViewModel viewModel)
		{
			try
			{
				if (viewModel != null && viewModel.FirstSittingOLevelResultDetails != null && viewModel.FirstSittingOLevelResultDetails.Count > 0)
				{
					foreach (OLevelResultDetail firstSittingOLevelResultDetail in viewModel.FirstSittingOLevelResultDetails)
					{
						if (firstSittingOLevelResultDetail.Subject != null)
						{
							firstSittingOLevelResultDetail.Subject = viewModel.OLevelSubjects.Where(m => m.Id == firstSittingOLevelResultDetail.Subject.Id).SingleOrDefault();
						}
						if (firstSittingOLevelResultDetail.Grade != null)
						{
							firstSittingOLevelResultDetail.Grade = viewModel.OLevelGrades.Where(m => m.Id == firstSittingOLevelResultDetail.Grade.Id).SingleOrDefault();
						}
					}
				}

				if (viewModel != null && viewModel.SecondSittingOLevelResultDetails != null && viewModel.SecondSittingOLevelResultDetails.Count > 0)
				{
					foreach (OLevelResultDetail secondSittingOLevelResultDetail in viewModel.SecondSittingOLevelResultDetails)
					{
						if (secondSittingOLevelResultDetail.Subject != null)
						{
							secondSittingOLevelResultDetail.Subject = viewModel.OLevelSubjects.Where(m => m.Id == secondSittingOLevelResultDetail.Subject.Id).SingleOrDefault();
						}
						if (secondSittingOLevelResultDetail.Grade != null)
						{
							secondSittingOLevelResultDetail.Grade = viewModel.OLevelGrades.Where(m => m.Id == secondSittingOLevelResultDetail.Grade.Id).SingleOrDefault();
						}
					}
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetStudentUploadedPassport(FormViewModel viewModel)
		{
			if (viewModel != null && viewModel.Person != null && !string.IsNullOrEmpty((string)TempData["imageUrl"]))
			{
				viewModel.Person.ImageFileUrl = (string)TempData["imageUrl"];
			}
			else
			{
				viewModel.Person.ImageFileUrl = Utility.DEFAULT_AVATAR;
			}
		}

		private void PopulateAllDropDowns(int programmeId)
		{
			FormViewModel existingViewModel = (FormViewModel)TempData["FormViewModel"];

			try
			{
				if (existingViewModel == null)
				{
					viewModel = new FormViewModel();

					ViewBag.StateId = viewModel.StateSelectList;
					ViewBag.SexId = viewModel.SexSelectList;
					ViewBag.FirstChoiceFacultyId = viewModel.FacultySelectList;
					ViewBag.SecondChoiceFacultyId = viewModel.FacultySelectList;
					ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), Utility.ID, Utility.NAME);
					ViewBag.RelationshipId = viewModel.RelationshipSelectList;
					ViewBag.FirstSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
					ViewBag.SecondSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
					ViewBag.FirstSittingExamYearId = viewModel.ExamYearSelectList;
					ViewBag.SecondSittingExamYearId = viewModel.ExamYearSelectList;
					ViewBag.ReligionId = viewModel.ReligionSelectList;
					ViewBag.AbilityId = viewModel.AbilitySelectList;
					ViewBag.DayOfBirthId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
					ViewBag.MonthOfBirthId = viewModel.MonthOfBirthSelectList;
					ViewBag.YearOfBirthId = viewModel.YearOfBirthSelectList;
					ViewBag.TitleId = viewModel.TitleSelectList;
					ViewBag.MaritalStatusId = viewModel.MaritalStatusSelectList;
					ViewBag.BloodGroupId = viewModel.BloodGroupSelectList;
					ViewBag.GenotypeId = viewModel.GenotypeSelectList;
					ViewBag.ModeOfEntryId = viewModel.ModeOfEntrySelectList;
					ViewBag.ModeOfStudyId = viewModel.ModeOfStudySelectList;

					//ViewBag.StudentTypeId = viewModel.StudentTypeSelectList;
					//ViewBag.StudentStatusId = viewModel.StudentStatusSelectList;

					ViewBag.StudentCategoryId = viewModel.StudentCategorySelectList;
					ViewBag.StudentTypeId = viewModel.StudentTypeSelectList;

					ViewBag.LevelId = viewModel.LevelSelectList;
					ViewBag.ModeOfFinanceId = viewModel.ModeOfFinanceSelectList;
					ViewBag.RelationshipId = viewModel.RelationshipSelectList;
					ViewBag.FacultyId = viewModel.FacultySelectList;
					ViewBag.AdmissionYearId = viewModel.AdmissionYearSelectList;
					ViewBag.GraduationYearId = viewModel.GraduationYearSelectList;
					ViewBag.ProgrammeId = viewModel.ProgrammeSelectList;

					if (viewModel.DepartmentSelectList != null)
					{
						ViewBag.DepartmentId = viewModel.DepartmentSelectList;
					}
					else
					{
						ViewBag.DepartmentId = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
					}

					if (programmeId == 3 || programmeId == 4)
					{
						ViewBag.StudentNdResultDayAwardedId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
						ViewBag.StudentNdResultMonthAwardedId = viewModel.StudentNdResultMonthAwardedSelectList;
						ViewBag.StudentNdResultYearAwardedId = viewModel.StudentNdResultYearAwardedSelectList;

						ViewBag.StudentLastEmploymentStartDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
						ViewBag.StudentLastEmploymentStartMonthId = viewModel.StudentLastEmploymentStartMonthSelectList;
						ViewBag.StudentLastEmploymentStartYearId = viewModel.StudentLastEmploymentStartYearSelectList;

						ViewBag.StudentLastEmploymentEndDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
						ViewBag.StudentLastEmploymentEndMonthId = viewModel.StudentLastEmploymentEndMonthSelectList;
						ViewBag.StudentLastEmploymentEndYearId = viewModel.StudentLastEmploymentEndYearSelectList;

						ViewBag.PreviousEducationStartDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
						ViewBag.PreviousEducationStartMonthId = viewModel.PreviousEducationStartMonthSelectList;
						ViewBag.PreviousEducationStartYearId = viewModel.PreviousEducationStartYearSelectList;

						ViewBag.PreviousEducationEndDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
						ViewBag.PreviousEducationEndMonthId = viewModel.PreviousEducationEndMonthSelectList;
						ViewBag.PreviousEducationEndYearId = viewModel.PreviousEducationEndYearSelectList;

						ViewBag.ResultGradeId = viewModel.ResultGradeSelectList;
					}

					SetDefaultSelectedSittingSubjectAndGrade(viewModel);
				}
				else
				{
					if (existingViewModel.Student.Title == null) { existingViewModel.Student.Title = new Title(); }
					if (existingViewModel.Person.Sex == null) { existingViewModel.Person.Sex = new Sex(); }
					if (existingViewModel.Student.MaritalStatus == null) { existingViewModel.Student.MaritalStatus = new MaritalStatus(); }
					if (existingViewModel.Person.Religion == null) { existingViewModel.Person.Religion = new Religion(); }
					if (existingViewModel.Person.State == null) { existingViewModel.Person.State = new State(); }
					if (existingViewModel.AppliedCourse.Programme == null) { existingViewModel.AppliedCourse.Programme = new Programme(); }
					if (existingViewModel.Sponsor.Relationship == null) { existingViewModel.Sponsor.Relationship = new Relationship(); }
					if (existingViewModel.FirstSittingOLevelResult.Type == null) { existingViewModel.FirstSittingOLevelResult.Type = new OLevelType(); }
					if (existingViewModel.Applicant.Ability == null) { existingViewModel.Applicant.Ability = new Ability(); }
					if (existingViewModel.Person.YearOfBirth == null) { existingViewModel.Person.YearOfBirth = new Value(); }
					if (existingViewModel.Person.MonthOfBirth == null) { existingViewModel.Person.MonthOfBirth = new Value(); }
					if (existingViewModel.Person.DayOfBirth == null) { existingViewModel.Person.DayOfBirth = new Value(); }
					if (existingViewModel.AppliedCourse.Department == null) { existingViewModel.AppliedCourse.Department = new Department(); }
					if (existingViewModel.Student.BloodGroup == null) { existingViewModel.Student.BloodGroup = new BloodGroup(); }
					if (existingViewModel.Student.Genotype == null) { existingViewModel.Student.Genotype = new Genotype(); }

					// PERSONAL INFORMATION
					ViewBag.TitleId = new SelectList(existingViewModel.TitleSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Student.Title.Id);
					ViewBag.SexId = new SelectList(existingViewModel.SexSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Person.Sex.Id);
					ViewBag.MaritalStatusId = new SelectList(existingViewModel.MaritalStatusSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Student.MaritalStatus.Id);
					SetDateOfBirthDropDown(existingViewModel);
					ViewBag.ReligionId = new SelectList(existingViewModel.ReligionSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Person.Religion.Id);
					ViewBag.StateId = new SelectList(existingViewModel.StateSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Person.State.Id);

					if (existingViewModel.Person.LocalGovernment != null && existingViewModel.Person.LocalGovernment.Id > 0)
					{
						ViewBag.LgaId = new SelectList(existingViewModel.LocalGovernmentSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Person.LocalGovernment.Id);
					}
					else
					{
						ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), Utility.VALUE, Utility.TEXT);
					}
					ViewBag.BloodGroupId = new SelectList(existingViewModel.BloodGroupSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Student.BloodGroup.Id);
					ViewBag.GenotypeId = new SelectList(existingViewModel.GenotypeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Student.Genotype.Id);



					// ACADEMIC DETAILS
					ViewBag.ModeOfEntryId = new SelectList(existingViewModel.ModeOfEntrySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentAcademicInformation.ModeOfEntry.Id);
					ViewBag.ModeOfStudyId = new SelectList(existingViewModel.ModeOfStudySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentAcademicInformation.ModeOfStudy.Id);
					ViewBag.ProgrammeId = new SelectList(existingViewModel.ProgrammeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.AppliedCourse.Programme.Id);
					ViewBag.FacultyId = new SelectList(existingViewModel.FacultySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.AppliedCourse.Department.Faculty.Id);
					//ViewBag.ProgrammeId = new SelectList(existingViewModel.FacultySelectList, VALUE, TEXT, existingViewModel.AppliedCourse.Programme.Id);

					SetDepartmentIfExist(existingViewModel);
					SetDepartmentOptionIfExist(existingViewModel);

					ViewBag.StudentTypeId = new SelectList(viewModel.StudentTypeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Student.Type.Id);
					ViewBag.StudentCategoryId = new SelectList(viewModel.StudentCategorySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Student.Category.Id);
					ViewBag.AdmissionYearId = new SelectList(existingViewModel.AdmissionYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentAcademicInformation.YearOfAdmission);
					ViewBag.GraduationYearId = new SelectList(existingViewModel.GraduationYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentAcademicInformation.YearOfGraduation);
					ViewBag.LevelId = new SelectList(existingViewModel.LevelSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentAcademicInformation.Level.Id);

					// FINANCE DETAILS
					ViewBag.ModeOfFinanceId = new SelectList(existingViewModel.ModeOfFinanceSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentFinanceInformation.Mode.Id);

					// NEXT OF KIN
					ViewBag.RelationshipId = new SelectList(existingViewModel.RelationshipSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentSponsor.Relationship.Id);
				   
					//SPONSOR
					ViewBag.RelationshipId = new SelectList(existingViewModel.RelationshipSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Sponsor.Relationship.Id);


					ViewBag.FirstSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.FirstSittingOLevelResult.Type.Id);
					ViewBag.FirstSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.FirstSittingOLevelResult.ExamYear);
					ViewBag.SecondSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.SecondSittingOLevelResult.ExamYear);
					//ViewBag.AbilityId = new SelectList(existingViewModel.AbilitySelectList, VALUE, TEXT, existingViewModel.Applicant.Ability.Id);


					if (programmeId == 3 || programmeId == 4)
					{
						SetStudentNdResultDateAwardedDropDown(existingViewModel);
						SetStudentLastEmploymentEndDateDropDown(existingViewModel);
						SetStudentLastEmploymentStartDateDropDown(existingViewModel);
						SetPreviousEducationEndDateDropDowns(existingViewModel);
						SetPreviousEducationStartDateDropDowns(existingViewModel);

						ViewBag.ResultGradeId = new SelectList(existingViewModel.ResultGradeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.ResultGrade.Id);
					}
					
					if (existingViewModel.SecondSittingOLevelResult.Type != null)
					{
						ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.SecondSittingOLevelResult.Type.Id);
					}
					else
					{
						existingViewModel.SecondSittingOLevelResult.Type = new OLevelType() { Id = 0 };
						ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, Utility.VALUE, Utility.TEXT, 0);
					}

					SetSelectedSittingSubjectAndGrade(existingViewModel);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}
		
		private void SetStudentLastEmploymentStartDateDropDown(FormViewModel existingViewModel)
		{
			try
			{
				ViewBag.StudentLastEmploymentStartMonthId = new SelectList(existingViewModel.StudentLastEmploymentStartMonthSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartMonth.Id);
				ViewBag.StudentLastEmploymentStartYearId = new SelectList(existingViewModel.StudentLastEmploymentStartYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartYear.Id);
				
				if ((existingViewModel.StudentLastEmploymentStartDaySelectList == null || existingViewModel.StudentLastEmploymentStartDaySelectList.Count == 0) && (existingViewModel.StudentEmploymentInformation.StartMonth.Id > 0 && existingViewModel.StudentEmploymentInformation.StartYear.Id > 0))
				{
					existingViewModel.StudentLastEmploymentStartDaySelectList = Utility.PopulateDaySelectListItem(existingViewModel.StudentEmploymentInformation.StartMonth, existingViewModel.StudentEmploymentInformation.StartYear);
				}
				else
				{
					if (existingViewModel.StudentLastEmploymentStartDaySelectList != null && existingViewModel.StudentEmploymentInformation.StartDay.Id > 0)
					{
						ViewBag.StudentLastEmploymentStartDayId = new SelectList(existingViewModel.StudentLastEmploymentStartDaySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartDay.Id);
					}
					else if (existingViewModel.StudentLastEmploymentStartDaySelectList != null && existingViewModel.StudentEmploymentInformation.StartDay.Id <= 0)
					{
						ViewBag.StudentLastEmploymentStartDayId = existingViewModel.StudentLastEmploymentStartDaySelectList;
					}
					else if (existingViewModel.StudentLastEmploymentStartDaySelectList == null)
					{
						existingViewModel.StudentLastEmploymentStartDaySelectList = new List<SelectListItem>();
						ViewBag.StudentLastEmploymentStartDayId = new List<SelectListItem>();
					}
				}

				if (existingViewModel.StudentEmploymentInformation.StartDay != null && existingViewModel.StudentEmploymentInformation.StartDay.Id > 0)
				{
					ViewBag.StudentLastEmploymentStartDayId = new SelectList(existingViewModel.StudentLastEmploymentStartDaySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartDay.Id);
				}
				else
				{
					ViewBag.StudentLastEmploymentStartDayId = existingViewModel.StudentLastEmploymentStartDaySelectList;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SetStudentLastEmploymentEndDateDropDown(FormViewModel existingViewModel)
		{
			try
			{
				ViewBag.StudentLastEmploymentEndMonthId = new SelectList(existingViewModel.StudentLastEmploymentEndMonthSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndMonth.Id);
				ViewBag.StudentLastEmploymentEndYearId = new SelectList(existingViewModel.StudentLastEmploymentEndYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndYear.Id);

				if ((existingViewModel.StudentLastEmploymentEndDaySelectList == null || existingViewModel.StudentLastEmploymentEndDaySelectList.Count == 0) && (existingViewModel.StudentEmploymentInformation.EndMonth.Id > 0 && existingViewModel.StudentEmploymentInformation.EndYear.Id > 0))
				{
					existingViewModel.StudentLastEmploymentEndDaySelectList = Utility.PopulateDaySelectListItem(existingViewModel.StudentEmploymentInformation.EndMonth, existingViewModel.StudentEmploymentInformation.EndYear);
				}
				else
				{
					if (existingViewModel.StudentLastEmploymentEndDaySelectList != null && existingViewModel.StudentEmploymentInformation.EndDay.Id > 0)
					{
						ViewBag.StudentLastEmploymentEndDayId = new SelectList(existingViewModel.StudentLastEmploymentEndDaySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndDay.Id);
					}
					else if (existingViewModel.StudentLastEmploymentEndDaySelectList != null && existingViewModel.StudentEmploymentInformation.EndDay.Id <= 0)
					{
						ViewBag.StudentLastEmploymentEndDayId = existingViewModel.StudentLastEmploymentEndDaySelectList;
					}
					else if (existingViewModel.StudentLastEmploymentEndDaySelectList == null)
					{
						existingViewModel.StudentLastEmploymentEndDaySelectList = new List<SelectListItem>();
						ViewBag.StudentLastEmploymentEndDayId = new List<SelectListItem>();
					}
				}

				if (existingViewModel.StudentEmploymentInformation.EndDay != null && existingViewModel.StudentEmploymentInformation.EndDay.Id > 0)
				{
					ViewBag.StudentLastEmploymentEndDayId = new SelectList(existingViewModel.StudentLastEmploymentEndDaySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndDay.Id);
				}
				else
				{
					ViewBag.StudentLastEmploymentEndDayId = existingViewModel.StudentLastEmploymentEndDaySelectList;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
		
		private void SetStudentNdResultDateAwardedDropDown(FormViewModel existingViewModel)
		{
			try
			{
				ViewBag.StudentNdResultMonthAwardedId = new SelectList(existingViewModel.StudentNdResultMonthAwardedSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentNdResult.MonthAwarded.Id);
				ViewBag.StudentNdResultYearAwardedId = new SelectList(existingViewModel.StudentNdResultYearAwardedSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentNdResult.YearAwarded.Id);
				if ((existingViewModel.StudentNdResultDayAwardedSelectList == null || existingViewModel.StudentNdResultDayAwardedSelectList.Count == 0) && (existingViewModel.StudentNdResult.MonthAwarded.Id > 0 && existingViewModel.StudentNdResult.YearAwarded.Id > 0))
				{
					existingViewModel.StudentNdResultDayAwardedSelectList = Utility.PopulateDaySelectListItem(existingViewModel.StudentNdResult.MonthAwarded, existingViewModel.StudentNdResult.YearAwarded);
				}
				else
				{
					if (existingViewModel.StudentNdResultDayAwardedSelectList != null && existingViewModel.StudentNdResult.DayAwarded.Id > 0)
					{
						ViewBag.StudentNdResultDayAwardedId = new SelectList(existingViewModel.StudentNdResultDayAwardedSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentNdResult.DayAwarded.Id);
					}
					else if (existingViewModel.StudentNdResultDayAwardedSelectList != null && existingViewModel.StudentNdResult.DayAwarded.Id <= 0)
					{
						ViewBag.StudentNdResultDayAwardedId = existingViewModel.StudentNdResultDayAwardedSelectList;
					}
					else if (existingViewModel.StudentNdResultDayAwardedSelectList == null)
					{
						existingViewModel.StudentNdResultDayAwardedSelectList = new List<SelectListItem>();
						ViewBag.StudentNdResultDayAwardedId = new List<SelectListItem>();
					}
				}

				if (existingViewModel.StudentNdResult.DayAwarded != null && existingViewModel.StudentNdResult.DayAwarded.Id > 0)
				{
					ViewBag.StudentNdResultDayAwardedId = new SelectList(existingViewModel.StudentNdResultDayAwardedSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.StudentNdResult.DayAwarded.Id);
				}
				else
				{
					ViewBag.StudentNdResultDayAwardedId = existingViewModel.StudentNdResultDayAwardedSelectList;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SetDateOfBirthDropDown(FormViewModel existingViewModel)
		{
			try
			{
				ViewBag.MonthOfBirthId = new SelectList(existingViewModel.MonthOfBirthSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Person.MonthOfBirth.Id);
				ViewBag.YearOfBirthId = new SelectList(existingViewModel.YearOfBirthSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Person.YearOfBirth.Id);
				if ((existingViewModel.DayOfBirthSelectList == null || existingViewModel.DayOfBirthSelectList.Count == 0) && (existingViewModel.Person.MonthOfBirth.Id > 0 && existingViewModel.Person.YearOfBirth.Id > 0))
				{
					existingViewModel.DayOfBirthSelectList = Utility.PopulateDaySelectListItem(existingViewModel.Person.MonthOfBirth, existingViewModel.Person.YearOfBirth);
				}
				else
				{
					if (existingViewModel.DayOfBirthSelectList != null && existingViewModel.Person.DayOfBirth.Id > 0)
					{
						ViewBag.DayOfBirthId = new SelectList(existingViewModel.DayOfBirthSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Person.DayOfBirth.Id);
					}
					else if (existingViewModel.DayOfBirthSelectList != null && existingViewModel.Person.DayOfBirth.Id <= 0)
					{
						ViewBag.DayOfBirthId = existingViewModel.DayOfBirthSelectList;
					}
					else if (existingViewModel.DayOfBirthSelectList == null)
					{
						existingViewModel.DayOfBirthSelectList = new List<SelectListItem>();
						ViewBag.DayOfBirthId = new List<SelectListItem>();
					}
				}

				if (existingViewModel.Person.DayOfBirth != null && existingViewModel.Person.DayOfBirth.Id > 0)
				{
					ViewBag.DayOfBirthId = new SelectList(existingViewModel.DayOfBirthSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.Person.DayOfBirth.Id);
				}
				else
				{
					ViewBag.DayOfBirthId = existingViewModel.DayOfBirthSelectList;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SetPreviousEducationStartDateDropDowns(FormViewModel existingViewModel)
		{
			try
			{
				ViewBag.PreviousEducationStartMonthId = new SelectList(existingViewModel.PreviousEducationStartMonthSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.StartMonth.Id);
				ViewBag.PreviousEducationStartYearId = new SelectList(existingViewModel.PreviousEducationStartYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.StartYear.Id);
				if ((existingViewModel.PreviousEducationStartDaySelectList == null || existingViewModel.PreviousEducationStartDaySelectList.Count == 0) && (existingViewModel.ApplicantPreviousEducation.StartMonth.Id > 0 && existingViewModel.ApplicantPreviousEducation.StartYear.Id > 0))
				{
					existingViewModel.PreviousEducationStartDaySelectList = Utility.PopulateDaySelectListItem(existingViewModel.ApplicantPreviousEducation.StartMonth, existingViewModel.ApplicantPreviousEducation.StartYear);
				}
				else
				{
					if (existingViewModel.PreviousEducationStartDaySelectList != null && existingViewModel.ApplicantPreviousEducation.StartDay.Id > 0)
					{
						ViewBag.PreviousEducationStartDayId = new SelectList(existingViewModel.PreviousEducationStartDaySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.StartDay.Id);
					}
					else if (existingViewModel.PreviousEducationStartDaySelectList != null && existingViewModel.ApplicantPreviousEducation.StartDay.Id <= 0)
					{
						ViewBag.PreviousEducationStartDayId = existingViewModel.PreviousEducationStartDaySelectList;
					}
					else if (existingViewModel.PreviousEducationStartDaySelectList == null)
					{
						existingViewModel.PreviousEducationStartDaySelectList = new List<SelectListItem>();
						ViewBag.PreviousEducationStartDayId = new List<SelectListItem>();
					}
				}

				if (existingViewModel.ApplicantPreviousEducation.StartDay != null && existingViewModel.ApplicantPreviousEducation.StartDay.Id > 0)
				{
					ViewBag.PreviousEducationStartDayId = new SelectList(existingViewModel.PreviousEducationStartDaySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.StartDay.Id);
				}
				else
				{
					ViewBag.PreviousEducationStartDayId = existingViewModel.PreviousEducationStartDaySelectList;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SetPreviousEducationEndDateDropDowns(FormViewModel existingViewModel)
		{
			try
			{
				ViewBag.PreviousEducationEndMonthId = new SelectList(existingViewModel.PreviousEducationEndMonthSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.EndMonth.Id);
				ViewBag.PreviousEducationEndYearId = new SelectList(existingViewModel.PreviousEducationEndYearSelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.EndYear.Id);
				if ((existingViewModel.PreviousEducationEndDaySelectList == null || existingViewModel.PreviousEducationEndDaySelectList.Count == 0) && (existingViewModel.ApplicantPreviousEducation.EndMonth.Id > 0 && existingViewModel.ApplicantPreviousEducation.EndYear.Id > 0))
				{
					existingViewModel.PreviousEducationEndDaySelectList = Utility.PopulateDaySelectListItem(existingViewModel.ApplicantPreviousEducation.EndMonth, existingViewModel.ApplicantPreviousEducation.EndYear);
				}
				else
				{
					if (existingViewModel.PreviousEducationEndDaySelectList != null && existingViewModel.ApplicantPreviousEducation.EndDay.Id > 0)
					{
						ViewBag.PreviousEducationEndDayId = new SelectList(existingViewModel.PreviousEducationEndDaySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.EndDay.Id);
					}
					else if (existingViewModel.PreviousEducationEndDaySelectList != null && existingViewModel.ApplicantPreviousEducation.EndDay.Id <= 0)
					{
						ViewBag.PreviousEducationEndDayId = existingViewModel.PreviousEducationEndDaySelectList;
					}
					else if (existingViewModel.PreviousEducationEndDaySelectList == null)
					{
						existingViewModel.PreviousEducationEndDaySelectList = new List<SelectListItem>();
						ViewBag.PreviousEducationEndDayId = new List<SelectListItem>();
					}
				}

				if (existingViewModel.ApplicantPreviousEducation.EndDay != null && existingViewModel.ApplicantPreviousEducation.EndDay.Id > 0)
				{
					ViewBag.PreviousEducationEndDayId = new SelectList(existingViewModel.PreviousEducationEndDaySelectList, Utility.VALUE, Utility.TEXT, existingViewModel.ApplicantPreviousEducation.EndDay.Id);
				}
				else
				{
					ViewBag.PreviousEducationEndDayId = existingViewModel.PreviousEducationEndDaySelectList;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SetDefaultSelectedSittingSubjectAndGrade(FormViewModel viewModel)
		{
			try
			{
				if (viewModel != null && viewModel.FirstSittingOLevelResultDetails != null)
				{
					for (int i = 0; i < 9; i++)
					{
						ViewData["FirstSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
						ViewData["FirstSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
					}
				}

				if (viewModel != null && viewModel.SecondSittingOLevelResultDetails != null)
				{
					for (int i = 0; i < 9; i++)
					{
						ViewData["SecondSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
						ViewData["SecondSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
					}
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetLgaIfExist(FormViewModel viewModel)
		{
			try
			{
				if (viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
				{
					LocalGovernmentLogic localGovernmentLogic = new LocalGovernmentLogic();
					List<LocalGovernment> lgas = localGovernmentLogic.GetModelsBy(l => l.State_Id == viewModel.Person.State.Id);
					if (viewModel.Person.LocalGovernment != null && viewModel.Person.LocalGovernment.Id > 0)
					{
						ViewBag.LgaId = new SelectList(lgas, Utility.ID, Utility.NAME, viewModel.Person.LocalGovernment.Id);
					}
					else
					{
						ViewBag.LgaId = new SelectList(lgas, Utility.ID, Utility.NAME);
					}
				}
				else
				{
					ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetDepartmentIfExist(FormViewModel viewModel)
		{
			try
			{
				if (viewModel.AppliedCourse.Programme != null && viewModel.AppliedCourse.Programme.Id > 0)
				{
					ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
					ProgrammeDepartmentLogic departmentLogic = new ProgrammeDepartmentLogic();
					AdmissionList admissionList = new AdmissionList();//Ugo 2/4/2016
					AdmissionListLogic admissionListLogic = new AdmissionListLogic();
					if ( viewModel.AppliedCourse.ApplicationForm != null)
					{
						admissionList = admissionListLogic.GetModelBy(p => p.Application_Form_Id == viewModel.AppliedCourse.ApplicationForm.Id); 
					}
					else
					{
						ApplicationForm applicationForm = new ApplicationForm();
						applicationForm = applicationFormLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id).FirstOrDefault();
						admissionList = admissionListLogic.GetModelBy(p => p.Application_Form_Id == applicationForm.Id);
					}
				   
					List<Department> departments = departmentLogic.GetBy(viewModel.AppliedCourse.Programme);
					if (viewModel.AppliedCourse.Department != null && viewModel.AppliedCourse.Department.Id > 0)
					{
						//ViewBag.DepartmentId = new SelectList(departments, Utility.ID, Utility.NAME, viewModel.AppliedCourse.Department.Id);
						ViewBag.DepartmentId = new SelectList(departments, Utility.ID, Utility.NAME, admissionList.Deprtment.Id);
						viewModel.AdmissionList = admissionList;
					}
					else
					{
						ViewBag.DepartmentId = new SelectList(departments, Utility.ID, Utility.NAME);
					}
				}
				else
				{
					ViewBag.DepartmentId = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetDepartmentOptionIfExist(FormViewModel viewModel)
		{
			try
			{
				if (viewModel.AppliedCourse.Department != null && viewModel.AppliedCourse.Department.Id > 0)
				{
					ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
					DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
					AdmissionList admissionList = new AdmissionList();//Ugo 2/4/2016
					AdmissionListLogic admissionListLogic = new AdmissionListLogic();
					if (viewModel.AppliedCourse.ApplicationForm != null)
					{
						admissionList = admissionListLogic.GetModelBy(p => p.Application_Form_Id == viewModel.AppliedCourse.ApplicationForm.Id);
					}
					else
					{
						ApplicationForm applicationForm = new ApplicationForm();
						applicationForm = applicationFormLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id).FirstOrDefault();
						admissionList = admissionListLogic.GetModelBy(p => p.Application_Form_Id == applicationForm.Id);
					}
				   
					List<DepartmentOption> departmentOptions = departmentOptionLogic.GetModelsBy(l => l.Department_Id == viewModel.AppliedCourse.Department.Id);
					if (viewModel.AppliedCourse.Option != null && viewModel.AppliedCourse.Option.Id > 0)
					{
						ViewBag.DepartmentOptionId = new SelectList(departmentOptions, Utility.ID, Utility.NAME, viewModel.AppliedCourse.Option.Id);
					  
					}
					else
					{
						List<DepartmentOption> options = new List<DepartmentOption>();
						//DepartmentOption option = new DepartmentOption() { Id = 0, Name = viewModel.AppliedCourse.Department.Name };
						DepartmentOption option = new DepartmentOption() { Id = 0, Name = admissionList.Deprtment.Name };
						options.Add(option);

						ViewBag.DepartmentOptionId = new SelectList(options, Utility.ID, Utility.NAME);
					}
				}
				else
				{
					ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}
		
		private void SetPreviousEducationStartDate()
		{
			try
			{
				if (viewModel.ApplicantPreviousEducation != null && viewModel.ApplicantPreviousEducation.StartDate != null)
				{
					int noOfDays = DateTime.DaysInMonth(viewModel.ApplicantPreviousEducation.StartYear.Id, viewModel.ApplicantPreviousEducation.StartMonth.Id);
					List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
					if (days != null && days.Count > 0)
					{
						days.Insert(0, new Value() { Name = "--DD--" });
					}

					if (viewModel.ApplicantPreviousEducation.StartDay != null && viewModel.ApplicantPreviousEducation.StartDay.Id > 0)
					{
						ViewBag.PreviousEducationStartDayId = new SelectList(days, Utility.ID, Utility.NAME, viewModel.ApplicantPreviousEducation.StartDay.Id);
					}
					else
					{
						ViewBag.PreviousEducationStartDayId = new SelectList(days, Utility.ID, Utility.NAME);
					}
				}
				else
				{
					ViewBag.PreviousEducationStartDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetPreviousEducationEndDate()
		{
			try
			{
				if (viewModel.ApplicantPreviousEducation != null && viewModel.ApplicantPreviousEducation.EndDate != null)
				{
					int noOfDays = DateTime.DaysInMonth(viewModel.ApplicantPreviousEducation.EndYear.Id, viewModel.ApplicantPreviousEducation.EndMonth.Id);
					List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
					if (days != null && days.Count > 0)
					{
						days.Insert(0, new Value() { Name = "--DD--" });
					}

					if (viewModel.ApplicantPreviousEducation.EndDay != null && viewModel.ApplicantPreviousEducation.EndDay.Id > 0)
					{
						ViewBag.PreviousEducationEndDayId = new SelectList(days, Utility.ID, Utility.NAME, viewModel.ApplicantPreviousEducation.EndDay.Id);
					}
					else
					{
						ViewBag.PreviousEducationEndDayId = new SelectList(days, Utility.ID, Utility.NAME);
					}
				}
				else
				{
					ViewBag.PreviousEducationEndDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetDateOfBirth()
		{
			try
			{
				if (viewModel.Person.DateOfBirth.HasValue)
				{
					if (viewModel.Person.YearOfBirth.Id > 0 && viewModel.Person.MonthOfBirth.Id > 0)
					{
						int noOfDays = DateTime.DaysInMonth(viewModel.Person.YearOfBirth.Id, viewModel.Person.MonthOfBirth.Id);
						List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
						if (days != null && days.Count > 0)
						{
							days.Insert(0, new Value() { Name = "--DD--" });
						}

						if (viewModel.Person.DayOfBirth != null && viewModel.Person.DayOfBirth.Id > 0)
						{
							ViewBag.DayOfBirthId = new SelectList(days, Utility.ID, Utility.NAME, viewModel.Person.DayOfBirth.Id);
						}
						else
						{
							ViewBag.DayOfBirthId = new SelectList(days, Utility.ID, Utility.NAME);
						}
					}
				}
				else
				{
					ViewBag.DayOfBirthId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		private void SetSelectedSittingSubjectAndGrade(FormViewModel existingViewModel)
		{
			try
			{
				if (existingViewModel != null && existingViewModel.FirstSittingOLevelResultDetails != null && existingViewModel.FirstSittingOLevelResultDetails.Count > 0)
				{
					int i = 0;
					foreach (OLevelResultDetail firstSittingOLevelResultDetail in existingViewModel.FirstSittingOLevelResultDetails)
					{
						if (firstSittingOLevelResultDetail.Subject != null && firstSittingOLevelResultDetail.Grade != null)
						{
							ViewData["FirstSittingOLevelSubjectId" + i] = new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, firstSittingOLevelResultDetail.Subject.Id);
							ViewData["FirstSittingOLevelGradeId" + i] = new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, firstSittingOLevelResultDetail.Grade.Id);
						}
						else
						{
							firstSittingOLevelResultDetail.Subject = new OLevelSubject() { Id = 0 };
							firstSittingOLevelResultDetail.Grade = new OLevelGrade() { Id = 0 };

							ViewData["FirstSittingOLevelSubjectId" + i] = new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
							ViewData["FirstSittingOLevelGradeId" + i] = new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
						}

						i++;
					}
				}

				if (existingViewModel != null && existingViewModel.SecondSittingOLevelResultDetails != null && existingViewModel.SecondSittingOLevelResultDetails.Count > 0)
				{
					int i = 0;
					foreach (OLevelResultDetail secondSittingOLevelResultDetail in existingViewModel.SecondSittingOLevelResultDetails)
					{
						if (secondSittingOLevelResultDetail.Subject != null && secondSittingOLevelResultDetail.Grade != null)
						{
							ViewData["SecondSittingOLevelSubjectId" + i] = new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, secondSittingOLevelResultDetail.Subject.Id);
							ViewData["SecondSittingOLevelGradeId" + i] = new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, secondSittingOLevelResultDetail.Grade.Id);
						}
						else
						{
							secondSittingOLevelResultDetail.Subject = new OLevelSubject() { Id = 0 };
							secondSittingOLevelResultDetail.Grade = new OLevelGrade() { Id = 0 };

							ViewData["SecondSittingOLevelSubjectId" + i] = new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
							ViewData["SecondSittingOLevelGradeId" + i] = new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
						}

						i++;
					}
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}

		public JsonResult GetLocalGovernmentsByState(string id)
		{
			try
			{
				LocalGovernmentLogic lgaLogic = new LocalGovernmentLogic();
				List<LocalGovernment> lgas = lgaLogic.GetModelsBy(l => l.State_Id == id);

				return Json(new SelectList(lgas, "Id", "Name"), JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public JsonResult GetDayOfBirthBy(string monthId, string yearId)
		{
			try
			{
				if (string.IsNullOrEmpty(monthId) || string.IsNullOrEmpty(yearId))
				{
					return null;
				}

				Value month = new Value() { Id = Convert.ToInt32(monthId) };
				Value year = new Value() { Id = Convert.ToInt32(yearId) };
				List<Value> days = Utility.GetNumberOfDaysInMonth(month, year);

				return Json(new SelectList(days, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public JsonResult GetDepartmentByProgrammeId(string id)
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

				return Json(new SelectList(departments, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
        public JsonResult SavePassport(string imageFileUrl)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!string.IsNullOrEmpty(imageFileUrl))
                {
                    Model.Model.Student student = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;

                    if (student == null)
                    {
                        System.Web.Security.FormsAuthentication.SignOut();
                        System.Web.HttpContext.Current.Response.Redirect("/Security/Account/Login");
                    }

                    string junkPath = imageFileUrl.Split('?').FirstOrDefault();
                    string studentPath = "/Content/Student/" + junkPath.Split('/').LastOrDefault();

                    if (System.IO.File.Exists(Server.MapPath(junkPath)))
                    {
                        System.IO.File.Move(Server.MapPath(junkPath), Server.MapPath(studentPath));

                        PersonLogic personLogic = new PersonLogic();
                        Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);

                        person.ImageFileUrl = studentPath;

                        personLogic.Modify(person);

                        result.IsError = false;
                        result.Message = "Passport has been saved.";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Initial filepath does not exist.";
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Image Url not set.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public virtual ActionResult UploadFile(FormCollection form)
        {
            HttpPostedFileBase file = Request.Files["PassportFile"];

            bool isUploaded = false;
            string personId = form["Person.Id"].ToString();
            string passportUrl = form["Person.ImageFileUrl"].ToString();
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;

            try
            {
                if (file != null && file.ContentLength != 0)
                {
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__";
                    string newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;

                    string invalidFileMessage = InvalidFile(file.ContentLength, fileExtension);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["imageUrl"] = null;
                        return Json(new { isUploaded = isUploaded, message = invalidFileMessage, imageUrl = passportUrl }, "text/html", JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk");
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId);

                        file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "File uploaded successfully!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/Junk/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            //imageUrlDisplay = "/ilaropoly" + imageUrl + "?t=" + DateTime.Now;
                            TempData["imageUrl"] = imageUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            return Json(new { isUploaded = isUploaded, message = message, imageUrl = imageUrlDisplay }, "text/html", JsonRequestBehavior.AllowGet);
        }

        private string InvalidFile(decimal uploadedFileSize, string fileExtension)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = 20 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte, 1);
                if (InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension + "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if (actualFileSizeToUpload > (maximumFileSize / oneKiloByte))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") + " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                }

                return message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return false;
                case ".png":
                    return false;
                case ".gif":
                    return false;
                case ".jpeg":
                    return false;
                default:
                    return true;
            }
        }

        private void DeleteFileIfExist(string folderPath, string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}