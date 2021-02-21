using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Web.Areas.Admin.ViewModels;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
	public class ReportController : Controller
	{
		public ReportViewModel ReportViewModel;
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult ApplicationFormSummary()
		{
			return PartialView();
		}

		//
		// GET: /Admin/Report/
		public ActionResult ApplicationSummary()
		{
			return PartialView();
		}

		public ActionResult ListOfApplications()
		{
			return View();
		}

		public ActionResult PhotoCard()
		{
			return View();
		}
		public ActionResult PhotoCardZip()
		{
			return View();
		}

        public ActionResult StudentExamPermit()
        {
            return View();
        }
        public ActionResult AdmissionProcessing()
		{
			AppliedCourseLogic appliedCourseLogic = new Business.AppliedCourseLogic();
			AdmissionCriteriaLogic admissionCriteriaLogic = new AdmissionCriteriaLogic();

			AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(m => m.Person_Id == 152);


			string rejectReason = admissionCriteriaLogic.EvaluateApplication(appliedCourse);
			ViewBag.RejectReason = rejectReason;
			return View();
		}

		public ActionResult AcceptanceReport()
		{
			return View();
		}

		public ActionResult MatriculationReport()
		{
			return View();
		}

		public ActionResult BiodatatReport()
		{
			return View();
		}
		public ActionResult SchoolFeesPayment()
		{
			return View();
		}
		public ActionResult StudentCourseRegistration()
		{
			return View();
		}
		public ActionResult FeesPaymentReport()
		{
			return View();
		}
		public ActionResult ExamRawScoreSheetReport()
		{
			return View();
		}
		public ActionResult CourseRegistrationReport()
		{
			return View();
		}
		public ActionResult Transcript()
		{
			return View();
		}
		public ActionResult StatementOfResult()
		{
			return View();
		}
		public ActionResult AttendanceReportBulk()
		{
			return View();
		}

		public ActionResult NotificationOfResult(long personId, int semesterId, int sessionId, int programmeId, int departmentId, int levelId)
		{
			ViewBag.personId = personId;
			ViewBag.semesterId = semesterId;
			ViewBag.sessionId = sessionId;
			ViewBag.programmeId = programmeId;
			ViewBag.departmentId = departmentId;
			ViewBag.semesterId = semesterId;
			ViewBag.levelId = levelId;
			return View();
		}
		public ActionResult NotificationOfResultBulk()
		{
			return View();
		}
		public ActionResult StatementOfResultBulk()
		{
			return View();
		}
		public ActionResult MasterGradeSheet()
		{
			return View();
		}

		public ActionResult ResultSheet()
		{
			ReportViewModel = new ReportViewModel();
			if (TempData["ReportViewModel"] != null)
			{
				ReportViewModel = (ReportViewModel) TempData["ReportViewModel"];
				ViewBag.levelId = ReportViewModel.Level.Id;
				ViewBag.semesterId = ReportViewModel.Semester.Id;
				ViewBag.progId = ReportViewModel.Programme.Id;
				ViewBag.deptId = ReportViewModel.Department.Id;
				ViewBag.sessionId = ReportViewModel.Session.Id;
				ViewBag.courseId = ReportViewModel.Course.Id;
			}
		   
			return View();
		}
		public ActionResult CAResultSheet()
		{
			ReportViewModel = new ReportViewModel();
			if (TempData["ReportViewModel"] != null)
			{
				ReportViewModel = (ReportViewModel)TempData["ReportViewModel"];
				ViewBag.levelId = ReportViewModel.Level.Id;
				ViewBag.semesterId = ReportViewModel.Semester.Id;
				ViewBag.progId = ReportViewModel.Programme.Id;
				ViewBag.deptId = ReportViewModel.Department.Id;
				ViewBag.sessionId = ReportViewModel.Session.Id;
				ViewBag.courseId = ReportViewModel.Course.Id;
			}

			return View();
		}

		public ActionResult UnregisteredStudentResultSheet(string levelId,string semesterId,string progId,string deptId,string sessionId,string courseId)
		{
			ViewBag.levelId = levelId;
			ViewBag.semesterId = semesterId;
			ViewBag.progId = progId;
			ViewBag.deptId = deptId;
			ViewBag.sessionId = sessionId;
			ViewBag.courseId = courseId;
			return View();
		}
		public ActionResult ApplicantResult()
		{
			return View();
		}
		public ActionResult ApplicantsByChoice()
		{
			return View();
		}
		public ActionResult HNDApplicantReport()
		{
			return View();
		}
        public ActionResult NDPApplicantReport()
        {
            return View();
        }
        public ActionResult HNDRejectedApplicantReport()
		{
			return View();
		}

		public ActionResult ResultSheetAlt()
		{
			try
			{
				if (TempData["viewModel"] != null)
				{
					StaffViewModel viewModel = (StaffViewModel)TempData["viewModel"];

					ViewBag.levelId = viewModel.CourseAllocation.Level.Id;
					ViewBag.semesterId = viewModel.CourseAllocation.Semester.Id;
					ViewBag.progId = viewModel.CourseAllocation.Programme.Id;
					ViewBag.deptId = viewModel.CourseAllocation.Department.Id;
					ViewBag.sessionId = viewModel.CourseAllocation.Session.Id;
					ViewBag.courseId = viewModel.CourseAllocation.Course.Id;
					ViewBag.courseModeId = viewModel.courseModeId;
				}
			}
			catch (Exception)
			{   
				throw;
			}

			return View();
		}
		public ActionResult MasterGradeSheetAlt()
		{
			return View();
		}
		public ActionResult StudentsWithoutResult()
		{
			return View();
		}
		public ActionResult ApplicantReport()
		{
			return View();
		}
		public ActionResult RejectedApplicantReport()
		{
			return View();
		}
		public ActionResult CourseEvaluationReport(string downloadPath, string downloadName )
		{
			try
			{
				if (downloadPath != null && downloadName != null)
				{
					return File(Server.MapPath(downloadPath), "application/zip", downloadName + ".zip"); 
				}
			}
			catch (Exception)
			{   
				throw;
			}

			return View();
		}
		public ActionResult StudentInformationReport()
		{
			return View();
		}
		public ActionResult VerificationReport()
		{
			return View();
		}
		public ActionResult GraduationReport()
		{
			return View();
		}
        public ActionResult DiplomaClassReport()
        {
            return View();
        }

        //New Reports from Abiapoly
        public ActionResult SchoolFeesPaymentReport()
        {
            return View();
        }
        public ActionResult AdmissionListReport()
        {
            return View();
        }
        public ActionResult AdmissionListCountReport()
        {
            return View();
        }
        public ActionResult AcceptanceFeeReport()
        {
            return View();
        }
        public ActionResult AcceptanceFeeReportCount()
        {
            return View();
        }
        public ActionResult DebtorsReport()
        {
            return View();
        }
        public ActionResult StudentCount()
        {
            return View();
        }
        public ActionResult OldStudentCount()
        {
            return View();
        }
        public ActionResult FullPaymentDebtorsReportCount()
        {
            return View();
        }
        public ActionResult SecondInstallmentDebtorsReportCount()
        {
            return View();
        }
        public ActionResult PaymentCount()
        {
            return View();
        }
        public ActionResult CourseRegistrationBulk()
        {
            return View();
        }


	    public ActionResult CBEResultSheet()
	    {
            ReportViewModel = new ReportViewModel();
            if (TempData["ReportViewModel"] != null)
            {
                ReportViewModel = (ReportViewModel)TempData["ReportViewModel"];
                ViewBag.levelId = ReportViewModel.Level.Id;
                ViewBag.semesterId = ReportViewModel.Semester.Id;
                ViewBag.progId = ReportViewModel.Programme.Id;
                ViewBag.deptId = ReportViewModel.Department.Id;
                ViewBag.sessionId = ReportViewModel.Session.Id;
                ViewBag.courseId = ReportViewModel.Course.Id;
            }

            return View();
	    }
        public ActionResult ExamResultSummary()
        {
            return View();
        }
        public ActionResult ExamScoreBroadSheet()
        {
            return View();
        }
        public ActionResult AdmissionCountSummary()
        {
            return View();
        }
        public ActionResult CourseRegistrationCountSummary()
        {
            return View();
        }
    }
}