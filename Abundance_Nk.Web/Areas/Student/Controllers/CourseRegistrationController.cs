using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Web.Script.Serialization;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
	[AllowAnonymous]
	public class CourseRegistrationController : BaseController
	{
		private CourseLogic courseLogic;
		private StudentLogic studentLogic;
		private CourseRegistrationViewModel viewModel;

		public CourseRegistrationController()
		{
			courseLogic = new CourseLogic();
			studentLogic = new StudentLogic();
			viewModel = new CourseRegistrationViewModel();
		}

		public ActionResult Logon()
		{
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult Logon(CourseRegistrationViewModel vModel)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Model.Model.Student student = studentLogic.GetBy(vModel.MatricNumber);
					if (student != null && student.Id > 0)
					{
						return RedirectToAction("Form", new { sid = student.Id });
					}

					SetMessage("Invalid Matric Number or PIN!", Message.Category.Error);
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(vModel);
		}

		public ActionResult Form(string sid, int ssid)
		{
			try
			{
				//SetMessage("Registration has closed! ", Message.Category.Error);
				//return RedirectToAction("CheckStatus", "Admission", new { area = "Applicant"});

				long StudentId = Convert.ToInt64(Utility.Decrypt(sid));
				SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
				SessionSemester sessionSemester = null;
				if (ssid > 0)
				{
					sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Semester_Id == ssid); 
				}
				else
				{
					StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
					StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == StudentId).LastOrDefault();
					if (studentLevel == null)
					{
					   SetMessage("No Student Level record for this session! ", Message.Category.Error);
					   return View("Form", viewModel);
					}

					sessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == studentLevel.Session.Id).LastOrDefault();
				}

				PopulateCourseRegistrationForm(StudentId, sessionSemester);

			    viewModel.SessionSemester = sessionSemester;

				if (sessionSemester != null)
				{
					if (sessionSemester.Semester.Id == (int) Semesters.FirstSemester)
					{
						viewModel.SecondSemesterCourses = null;
					}
					else
					{
						viewModel.FirstSemesterCourses = null;
					}
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			TempData["CourseRegistrationViewModel"] = viewModel;
			return View("Form", viewModel);
		}

		public ActionResult ELearning(string sid, int ssid)
		{
			try
			{
				//SetMessage("Registration has closed! ", Message.Category.Error);
				//return RedirectToAction("CheckStatus", "Admission", new { area = "Applicant"});

				long StudentId = Convert.ToInt64(Utility.Decrypt(sid));
				SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
				SessionSemester sessionSemester = null;
				if (ssid > 0)
				{
					sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Semester_Id == ssid);
				}
				else
				{
					StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
					StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == StudentId).LastOrDefault();
					if (studentLevel == null)
					{
						SetMessage("No Student Level record for this session! ", Message.Category.Error);
						return View("Form", viewModel);
					}

					sessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == studentLevel.Session.Id).LastOrDefault();
				}

				PopulateCourseRegistrationForm(StudentId, sessionSemester);
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			TempData["CourseRegistrationViewModel"] = viewModel;
			return View("ELearning", viewModel);
		}

		private void PopulateCourseRegistrationForm(long sid, SessionSemester sessionSemester)
		{
			try
			{
				List<Course> firstSemesterCourses = null;
				List<Course> secondSemesterCourses = null;

				viewModel.Student = studentLogic.GetBy(sid);
				if(viewModel.Student != null && viewModel.Student.Id > 0)
				{
					CourseMode firstAttempt = new CourseMode() { Id = 1 };
					CourseMode carryOver = new CourseMode() { Id = 2 };
					Semester firstSemester = new Semester() { Id = 1 };
					Semester secondSemester = new Semester() { Id = 2 };

					//CurrentSessionSemesterLogic currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
					//viewModel.CurrentSessionSemester = currentSessionSemesterLogic.GetCurrentSessionSemester();
					viewModel.CurrentSessionSemester = new CurrentSessionSemester(){ SessionSemester = sessionSemester};

					StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
					//viewModel.StudentLevel = studentLevelLogic.GetBy(viewModel.Student,viewModel.CurrentSessionSemester.SessionSemester.Session);
					viewModel.StudentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == viewModel.Student.Id).LastOrDefault();
                    SetMinimumAndMaximumCourseUnit(viewModel.CurrentSessionSemester.SessionSemester.Semester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption, viewModel.StudentLevel.Programme, sessionSemester);
					
                    //if(viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0 && viewModel.StudentLevel.DepartmentOption == null)
                    //{
                    //    int[] partTimeProgrammes = { (int)Programmes.NDPartTime, (int)Programmes.HNDPartTime,(int)Programmes.HNDEvening };
                    //    if (partTimeProgrammes.Contains(viewModel.StudentLevel.Programme.Id))
                    //    {
                    //        if (viewModel.StudentLevel.Level.Id == (int)Levels.HNDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.NDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.HNDEI)
                    //        {
                    //            if (viewModel.StudentLevel.Level.Id == (int)Levels.HNDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.HNDEI)
                    //            {
                    //                firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, new Level(){Id = (int)Levels.HNDI} , firstSemester, true);
                    //                secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, new Level() { Id = (int)Levels.HNDI }, secondSemester, true);
                    //            }
                    //            if (viewModel.StudentLevel.Level.Id == (int)Levels.NDYRI)
                    //            {
                    //                firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, new Level() { Id = (int)Levels.NDI }, firstSemester, true);
                    //                secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, new Level() { Id = (int)Levels.NDI }, secondSemester, true);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (viewModel.StudentLevel.Programme.Id == (int)Programmes.HNDPartTime || viewModel.StudentLevel.Programme.Id == (int)Programmes.HNDEvening)
                    //            {
                    //                firstSemesterCourses = courseLogic.GetBy(new Programme(){Id = (int)Programmes.HNDFullTime}, viewModel.StudentLevel.Department, firstSemester, true);
                    //                secondSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.HNDFullTime }, viewModel.StudentLevel.Department, secondSemester, true);
                    //            }
                    //            if (viewModel.StudentLevel.Programme.Id == (int)Programmes.NDPartTime)
                    //            {
                    //                firstSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.NDFullTime }, viewModel.StudentLevel.Department, firstSemester, true);
                    //                secondSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.NDFullTime }, viewModel.StudentLevel.Department, secondSemester, true);
                    //            }
                               
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (viewModel.StudentLevel.Level.Id == (int)Levels.NDI || viewModel.StudentLevel.Level.Id == (int)Levels.HNDI)
                    //        {
                    //            firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, firstSemester, true);
                    //            secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, secondSemester, true);
                                
                    //        }
                    //        else
                    //        {
                    //            firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, firstSemester, true);
                    //            secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, secondSemester, true);
                    //        }
                    //    }

                    //    //SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption);
                    //    SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption, viewModel.StudentLevel.Programme, sessionSemester);
					
                    //}
                    //else if(viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0 && viewModel.StudentLevel.DepartmentOption != null && viewModel.StudentLevel.DepartmentOption.Id > 0)
                    //{
                    //    int[] partTimeProgrammes = { (int)Programmes.NDPartTime, (int)Programmes.HNDPartTime, (int)Programmes.HNDEvening };
                    //    if (partTimeProgrammes.Contains(viewModel.StudentLevel.Programme.Id))
                    //    {
                    //        if (viewModel.StudentLevel.Level.Id == (int)Levels.HNDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.NDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.HNDEI)
                    //        {
                    //            if (viewModel.StudentLevel.Level.Id == (int)Levels.HNDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.HNDEI)
                    //            {
                    //                firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, new Level(){Id = (int)Levels.HNDI}, firstSemester, true);
                    //                secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, new Level() { Id = (int)Levels.HNDI }, secondSemester, true);
                    //            }
                    //            if (viewModel.StudentLevel.Level.Id == (int)Levels.NDYRI)
                    //            {
                    //                firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, new Level() { Id = (int)Levels.NDI }, firstSemester, true);
                    //                secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, new Level() { Id = (int)Levels.NDII }, secondSemester, true);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (viewModel.StudentLevel.Programme.Id == (int)Programmes.HNDPartTime || viewModel.StudentLevel.Programme.Id == (int)Programmes.HNDEvening)
                    //            {
                    //                firstSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.HNDFullTime }, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, firstSemester, true);
                    //                secondSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.HNDFullTime }, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, secondSemester, true);
                    //            }
                    //            if (viewModel.StudentLevel.Programme.Id == (int)Programmes.NDPartTime)
                    //            {
                    //                firstSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.NDFullTime }, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, firstSemester, true);
                    //                secondSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.NDFullTime }, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, secondSemester, true);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (viewModel.StudentLevel.Level.Id == (int)Levels.HNDI || viewModel.StudentLevel.Level.Id == (int)Levels.NDI)
                    //        {
                    //            firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, viewModel.StudentLevel.Level, firstSemester, true);
                    //            secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, viewModel.StudentLevel.Level, secondSemester, true);
                                
                    //        }
                    //        else
                    //        {
                    //            firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, firstSemester, true);
                    //            secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, secondSemester, true);   
                    //        }
                    //    }
                    //    //firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department,viewModel.StudentLevel.DepartmentOption,viewModel.StudentLevel.Level,firstSemester,true);
                    //    //secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department,viewModel.StudentLevel.DepartmentOption,viewModel.StudentLevel.Level,secondSemester,true);
                    //    //SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption);
                    //    SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption, viewModel.StudentLevel.Programme, sessionSemester);
                    //}

					//get courses if already registered
					CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
					CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
					CourseRegistration courseRegistration = courseRegistrationLogic.GetBy(viewModel.Student,viewModel.StudentLevel.Level,viewModel.StudentLevel.Programme,viewModel.StudentLevel.Department,viewModel.CurrentSessionSemester.SessionSemester.Session);
					if(courseRegistration != null && courseRegistration.Id > 0)
					{
						viewModel.RegisteredCourse = courseRegistration;
						if(courseRegistration.Details != null && courseRegistration.Details.Count > 0)
						{
							//split registered courses by semester
                            List<CourseRegistrationDetail> firstSemesterRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == firstSemester.Id).ToList();
                            List<CourseRegistrationDetail> secondSemesterRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == secondSemester.Id).ToList();
                            List<CourseRegistrationDetail> firstSemesterCarryOverRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == firstSemester.Id && rc.Mode.Id == carryOver.Id).ToList();
                            List<CourseRegistrationDetail> secondSemesterCarryOverRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == secondSemester.Id && rc.Mode.Id == carryOver.Id).ToList();

                            //get registered courses
                            viewModel.FirstSemesterCourses = GetRegisteredCourse(courseRegistration, firstSemesterCourses, firstSemester, firstSemesterRegisteredCourseDetails, firstAttempt);
                            viewModel.SecondSemesterCourses = GetRegisteredCourse(courseRegistration, secondSemesterCourses, secondSemester, secondSemesterRegisteredCourseDetails, firstAttempt);

							//get carry over courses
							List<Course> firstSemesterCarryOverCourses = courseRegistrationDetailLogic.GetCarryOverCoursesBy(courseRegistration,firstSemester);
							List<Course> secondSemesterCarryOverCourses = courseRegistrationDetailLogic.GetCarryOverCoursesBy(courseRegistration,secondSemester);
							viewModel.FirstSemesterCarryOverCourses = GetRegisteredCourse(courseRegistration,firstSemesterCarryOverCourses,firstSemester,firstSemesterCarryOverRegisteredCourseDetails,carryOver);
							viewModel.SecondSemesterCarryOverCourses = GetRegisteredCourse(courseRegistration,secondSemesterCarryOverCourses,secondSemester,secondSemesterCarryOverRegisteredCourseDetails,carryOver);

							if(viewModel.FirstSemesterCarryOverCourses != null && viewModel.FirstSemesterCarryOverCourses.Count > 0)
							{
								viewModel.CarryOverExist = true;
								viewModel.CarryOverCourses.AddRange(viewModel.FirstSemesterCarryOverCourses);
								viewModel.TotalFirstSemesterCarryOverCourseUnit = viewModel.FirstSemesterCarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
							}
							if(viewModel.SecondSemesterCarryOverCourses != null && viewModel.SecondSemesterCarryOverCourses.Count > 0)
							{
								viewModel.CarryOverExist = true;
								viewModel.CarryOverCourses.AddRange(viewModel.SecondSemesterCarryOverCourses);
								viewModel.TotalSecondSemesterCarryOverCourseUnit = viewModel.SecondSemesterCarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);
							}

							//set total selected course units
							viewModel.SumOfFirstSemesterSelectedCourseUnit = SumSemesterSelectedCourseUnit(firstSemesterRegisteredCourseDetails);
							viewModel.SumOfSecondSemesterSelectedCourseUnit = SumSemesterSelectedCourseUnit(secondSemesterRegisteredCourseDetails);
							viewModel.CourseAlreadyRegistered = true;
						}
						else
						{
							viewModel.FirstSemesterCourses = GetUnregisteredCourseDetail(firstSemesterCourses,firstSemester);
							viewModel.SecondSemesterCourses = GetUnregisteredCourseDetail(secondSemesterCourses,secondSemester);
							viewModel.CourseAlreadyRegistered = false;
							//get carry over courses
							viewModel.CarryOverCourses = courseRegistrationDetailLogic.GetCarryOverBy(viewModel.Student,viewModel.CurrentSessionSemester.SessionSemester.Session);
							if(viewModel.CarryOverCourses != null && viewModel.CarryOverCourses.Count > 0)
							{
								viewModel.CarryOverExist = true;
								viewModel.TotalFirstSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
								viewModel.TotalSecondSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);

								if(viewModel.TotalFirstSemesterCarryOverCourseUnit <= viewModel.FirstSemesterMaximumUnit && viewModel.TotalSecondSemesterCarryOverCourseUnit <= viewModel.SecondSemesterMaximumUnit)
								{
									foreach(CourseRegistrationDetail carryOverCourse in viewModel.CarryOverCourses)
									{
										carryOverCourse.Course.IsRegistered = true;
									}
								}
							}
						}
					}
					else
					{
						viewModel.FirstSemesterCourses = GetUnregisteredCourseDetail(firstSemesterCourses,firstSemester);
						viewModel.SecondSemesterCourses = GetUnregisteredCourseDetail(secondSemesterCourses,secondSemester);
						viewModel.CourseAlreadyRegistered = false;
						//get carry over courses
						viewModel.CarryOverCourses = courseRegistrationDetailLogic.GetCarryOverBy(viewModel.Student,viewModel.CurrentSessionSemester.SessionSemester.Session);
						if(viewModel.CarryOverCourses != null && viewModel.CarryOverCourses.Count > 0)
						{
							viewModel.CarryOverExist = true;
							viewModel.TotalFirstSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
							viewModel.TotalSecondSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);

							if(viewModel.TotalFirstSemesterCarryOverCourseUnit <= viewModel.FirstSemesterMaximumUnit && viewModel.TotalSecondSemesterCarryOverCourseUnit <= viewModel.SecondSemesterMaximumUnit)
							{
								foreach(CourseRegistrationDetail carryOverCourse in viewModel.CarryOverCourses)
								{
									carryOverCourse.Course.IsRegistered = true;
								}
							}
						}
					}


					//}
				}
			}
			catch(Exception)
			{
				throw;
			}
		}
        private void PopulateCourseRegistrationFormPrintOut(long sid, SessionSemester sessionSemester)
        {
            try
            {
                List<Course> firstSemesterCourses = null;
                List<Course> secondSemesterCourses = null;

                viewModel.Student = studentLogic.GetBy(sid);
                if (viewModel.Student != null && viewModel.Student.Id > 0)
                {
                    CourseMode firstAttempt = new CourseMode() { Id = 1 };
                    CourseMode carryOver = new CourseMode() { Id = 2 };
                    Semester firstSemester = new Semester() { Id = 1 };
                    Semester secondSemester = new Semester() { Id = 2 };

                    //CurrentSessionSemesterLogic currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
                    //viewModel.CurrentSessionSemester = currentSessionSemesterLogic.GetCurrentSessionSemester();
                    viewModel.CurrentSessionSemester = new CurrentSessionSemester() { SessionSemester = sessionSemester };

                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    //viewModel.StudentLevel = studentLevelLogic.GetBy(viewModel.Student,viewModel.CurrentSessionSemester.SessionSemester.Session);
                    viewModel.StudentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == viewModel.Student.Id).LastOrDefault();
                    //SetMinimumAndMaximumCourseUnit(viewModel.CurrentSessionSemester.SessionSemester.Semester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption, viewModel.StudentLevel.Programme, sessionSemester);

                    int[] partTimeProgrammes = { (int)Programmes.NDPartTime, (int)Programmes.HNDPartTime, (int)Programmes.HNDEvening, (int)Programmes.NDEveningFullTime };
                    int[] ndProgrammes = { (int)Programmes.NDFullTime, (int)Programmes.NDPartTime, (int)Programmes.NDEveningFullTime };
                    int[] hndProgrammes = { (int)Programmes.HNDFullTime, (int)Programmes.HNDPartTime, (int)Programmes.HNDEvening };

                    if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0 && viewModel.StudentLevel.DepartmentOption == null)
                    {
                        if (partTimeProgrammes.Contains(viewModel.StudentLevel.Programme.Id))
                        {
                            if (viewModel.StudentLevel.Level.Id == (int)Levels.HNDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.NDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.HNDEI || 
                                viewModel.StudentLevel.Level.Id == (int)Levels.NDEI)
                            {
                                firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, firstSemester, true);
                                secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, secondSemester, true);
                            }
                            else
                            {
                                if (ndProgrammes.Contains(viewModel.StudentLevel.Programme.Id))
                                {
                                    firstSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.NDFullTime }, viewModel.StudentLevel.Department, firstSemester, true);
                                    secondSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.NDFullTime }, viewModel.StudentLevel.Department, secondSemester, true);
                                }
                                else
                                {
                                    firstSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.HNDFullTime }, viewModel.StudentLevel.Department, firstSemester, true);
                                    secondSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.HNDFullTime }, viewModel.StudentLevel.Department, secondSemester, true);
                                }
                            }
                        }
                        else
                        {
                            firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, firstSemester, true);
                            secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, secondSemester, true);
                        }

                        //SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption);
                        SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption, viewModel.StudentLevel.Programme, sessionSemester);
                    }
                    else if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0 && viewModel.StudentLevel.DepartmentOption != null && viewModel.StudentLevel.DepartmentOption.Id > 0)
                    {
                        if (partTimeProgrammes.Contains(viewModel.StudentLevel.Programme.Id))
                        {
                            if (viewModel.StudentLevel.Level.Id == (int)Levels.HNDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.NDYRI || viewModel.StudentLevel.Level.Id == (int)Levels.HNDEI ||
                                viewModel.StudentLevel.Level.Id == (int)Levels.NDEI)
                            {
                                firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, firstSemester, true);
                                secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, secondSemester, true);
                            }
                            else
                            {
                                if (viewModel.StudentLevel.Programme.Id == (int)Programmes.HNDPartTime || viewModel.StudentLevel.Programme.Id == (int)Programmes.HNDEvening)
                                {
                                    firstSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.HNDFullTime }, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, firstSemester, true);
                                    secondSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.HNDFullTime }, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, secondSemester, true);
                                }
                                if (viewModel.StudentLevel.Programme.Id == (int)Programmes.NDPartTime || viewModel.StudentLevel.Programme.Id == (int)Programmes.NDEveningFullTime)
                                {
                                    firstSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.NDFullTime }, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, firstSemester, true);
                                    secondSemesterCourses = courseLogic.GetBy(new Programme() { Id = (int)Programmes.NDFullTime }, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, secondSemester, true);
                                }
                            }
                        }
                        else
                        {
                            if (viewModel.StudentLevel.Level.Id == (int)Levels.HNDI || viewModel.StudentLevel.Level.Id == (int)Levels.NDI)
                            {
                                firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, firstSemester, true);
                                secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, viewModel.StudentLevel.DepartmentOption, secondSemester, true);

                            }
                            else
                            {
                                firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, firstSemester, true);
                                secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, secondSemester, true);
                            }
                        }
                        //firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department,viewModel.StudentLevel.DepartmentOption,viewModel.StudentLevel.Level,firstSemester,true);
                        //secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department,viewModel.StudentLevel.DepartmentOption,viewModel.StudentLevel.Level,secondSemester,true);
                        //SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption);
                        SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.DepartmentOption, viewModel.StudentLevel.Programme, sessionSemester);
                    }

                    //Merge all courses
                    List<Course> allCourses = new List<Course>();
                    allCourses.AddRange(firstSemesterCourses);
                    allCourses.AddRange(secondSemesterCourses);

                    //get courses if already registered
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    CourseRegistration courseRegistration = courseRegistrationLogic.GetBy(viewModel.Student, viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, viewModel.CurrentSessionSemester.SessionSemester.Session);
                    if (courseRegistration != null && courseRegistration.Id > 0)
                    {
                        viewModel.RegisteredCourse = courseRegistration;
                        if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
                        {
                            //split registered courses by semester
                            List<CourseRegistrationDetail> firstSemesterRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == firstSemester.Id).ToList();
                            List<CourseRegistrationDetail> secondSemesterRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == secondSemester.Id).ToList();
                            List<CourseRegistrationDetail> firstSemesterCarryOverRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == firstSemester.Id && rc.Mode.Id == carryOver.Id).ToList();
                            List<CourseRegistrationDetail> secondSemesterCarryOverRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == secondSemester.Id && rc.Mode.Id == carryOver.Id).ToList();

                            //get registered courses
                            viewModel.FirstSemesterCourses = GetRegisteredCourse(courseRegistration, allCourses, firstSemester, firstSemesterRegisteredCourseDetails, firstAttempt);
                            viewModel.SecondSemesterCourses = GetRegisteredCourse(courseRegistration, allCourses, secondSemester, secondSemesterRegisteredCourseDetails, firstAttempt);

                            //get carry over courses
                            List<Course> firstSemesterCarryOverCourses = courseRegistrationDetailLogic.GetCarryOverCoursesBy(courseRegistration, firstSemester);
                            List<Course> secondSemesterCarryOverCourses = courseRegistrationDetailLogic.GetCarryOverCoursesBy(courseRegistration, secondSemester);
                            viewModel.FirstSemesterCarryOverCourses = GetRegisteredCourse(courseRegistration, firstSemesterCarryOverCourses, firstSemester, firstSemesterCarryOverRegisteredCourseDetails, carryOver);
                            viewModel.SecondSemesterCarryOverCourses = GetRegisteredCourse(courseRegistration, secondSemesterCarryOverCourses, secondSemester, secondSemesterCarryOverRegisteredCourseDetails, carryOver);

                            if (viewModel.FirstSemesterCarryOverCourses != null && viewModel.FirstSemesterCarryOverCourses.Count > 0)
                            {
                                viewModel.CarryOverExist = true;
                                viewModel.CarryOverCourses.AddRange(viewModel.FirstSemesterCarryOverCourses);
                                viewModel.TotalFirstSemesterCarryOverCourseUnit = viewModel.FirstSemesterCarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
                            }
                            if (viewModel.SecondSemesterCarryOverCourses != null && viewModel.SecondSemesterCarryOverCourses.Count > 0)
                            {
                                viewModel.CarryOverExist = true;
                                viewModel.CarryOverCourses.AddRange(viewModel.SecondSemesterCarryOverCourses);
                                viewModel.TotalSecondSemesterCarryOverCourseUnit = viewModel.SecondSemesterCarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);
                            }

                            //set total selected course units
                            viewModel.SumOfFirstSemesterSelectedCourseUnit = SumSemesterSelectedCourseUnit(firstSemesterRegisteredCourseDetails);
                            viewModel.SumOfSecondSemesterSelectedCourseUnit = SumSemesterSelectedCourseUnit(secondSemesterRegisteredCourseDetails);
                            viewModel.CourseAlreadyRegistered = true;
                        }
                        else
                        {
                            viewModel.FirstSemesterCourses = GetUnregisteredCourseDetail(firstSemesterCourses, firstSemester);
                            viewModel.SecondSemesterCourses = GetUnregisteredCourseDetail(secondSemesterCourses, secondSemester);
                            viewModel.CourseAlreadyRegistered = false;
                            //get carry over courses
                            viewModel.CarryOverCourses = courseRegistrationDetailLogic.GetCarryOverBy(viewModel.Student, viewModel.CurrentSessionSemester.SessionSemester.Session);
                            if (viewModel.CarryOverCourses != null && viewModel.CarryOverCourses.Count > 0)
                            {
                                viewModel.CarryOverExist = true;
                                viewModel.TotalFirstSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
                                viewModel.TotalSecondSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);

                                if (viewModel.TotalFirstSemesterCarryOverCourseUnit <= viewModel.FirstSemesterMaximumUnit && viewModel.TotalSecondSemesterCarryOverCourseUnit <= viewModel.SecondSemesterMaximumUnit)
                                {
                                    foreach (CourseRegistrationDetail carryOverCourse in viewModel.CarryOverCourses)
                                    {
                                        carryOverCourse.Course.IsRegistered = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        viewModel.FirstSemesterCourses = GetUnregisteredCourseDetail(firstSemesterCourses, firstSemester);
                        viewModel.SecondSemesterCourses = GetUnregisteredCourseDetail(secondSemesterCourses, secondSemester);
                        viewModel.CourseAlreadyRegistered = false;
                        //get carry over courses
                        viewModel.CarryOverCourses = courseRegistrationDetailLogic.GetCarryOverBy(viewModel.Student, viewModel.CurrentSessionSemester.SessionSemester.Session);
                        if (viewModel.CarryOverCourses != null && viewModel.CarryOverCourses.Count > 0)
                        {
                            viewModel.CarryOverExist = true;
                            viewModel.TotalFirstSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
                            viewModel.TotalSecondSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);

                            if (viewModel.TotalFirstSemesterCarryOverCourseUnit <= viewModel.FirstSemesterMaximumUnit && viewModel.TotalSecondSemesterCarryOverCourseUnit <= viewModel.SecondSemesterMaximumUnit)
                            {
                                foreach (CourseRegistrationDetail carryOverCourse in viewModel.CarryOverCourses)
                                {
                                    carryOverCourse.Course.IsRegistered = true;
                                }
                            }
                        }
                    }


                    //}
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

		[HttpPost]
		public ActionResult Form(CourseRegistrationViewModel viewModel)
		{
			string message = null;

			try
			{
				CourseLogic courseLogic = new CourseLogic();

				string operation = "INSERT";
				string action = "REGISTRATION :COURSE FORM";
				string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
				var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
				courseRegistrationDetailAudit.Action = action;
				courseRegistrationDetailAudit.Operation = operation;
				courseRegistrationDetailAudit.Client = client;
				 UserLogic loggeduser = new UserLogic();
				courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Id == 1);

				List<CourseRegistrationDetail> selectedFirstSemesterCourseRegistrationDetails = null;
				List<CourseRegistrationDetail> selectedSecondSemesterCourseRegistrationDetails = null;
				List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();

				if (viewModel.CarryOverExist)
				{
					List<CourseRegistrationDetail> selectedCarryOverCourseRegistrationDetails = new List<CourseRegistrationDetail>();
					selectedCarryOverCourseRegistrationDetails = GetSelectedCourses(viewModel.CarryOverCourses);
					courseRegistrationDetails.AddRange(selectedCarryOverCourseRegistrationDetails);
				}

				viewModel.RegisteredCourse.Student = viewModel.Student;

				CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
				if (viewModel.CourseAlreadyRegistered) //modify
				{
					selectedFirstSemesterCourseRegistrationDetails = viewModel.FirstSemesterCourses;
					selectedSecondSemesterCourseRegistrationDetails = viewModel.SecondSemesterCourses;

					//courseRegistrationDetails = selectedFirstSemesterCourseRegistrationDetails;
					if (selectedFirstSemesterCourseRegistrationDetails != null && selectedFirstSemesterCourseRegistrationDetails.Count > 0)
					{
						courseRegistrationDetails.AddRange(selectedFirstSemesterCourseRegistrationDetails);
					}
					if (selectedSecondSemesterCourseRegistrationDetails != null && selectedSecondSemesterCourseRegistrationDetails.Count > 0)
					{
						courseRegistrationDetails.AddRange(selectedSecondSemesterCourseRegistrationDetails);
					}

					for (int i = 0; i < courseRegistrationDetails.Count; i++)
					{
						CourseRegistrationDetail courseRegistrationDetail = courseRegistrationDetails[i];
						courseRegistrationDetails[i].CourseUnit = courseLogic.GetModelBy(c => c.Course_Id == courseRegistrationDetail.Course.Id).Unit;
					}

					viewModel.RegisteredCourse.Details = courseRegistrationDetails;

					for (int i = 0; i < viewModel.RegisteredCourse.Details.Count; i++)
					{
						viewModel.RegisteredCourse.Details[i].ExamScore = null;
						viewModel.RegisteredCourse.Details[i].TestScore = null;
					}

					courseRegistrationDetailAudit.Operation = "MODIFY: COURSE FORM";
					bool modified = courseRegistrationLogic.Modify(viewModel.RegisteredCourse, courseRegistrationDetailAudit);
					if (modified)
					{
						message = "Selected courses has been successfully modified.";
					}
					else
					{
						message = "Course Registration modification Failed! Please try again.";
					}
				}
				else //insert
				{
					selectedFirstSemesterCourseRegistrationDetails = GetSelectedCourses(viewModel.FirstSemesterCourses);
					selectedSecondSemesterCourseRegistrationDetails = GetSelectedCourses(viewModel.SecondSemesterCourses);

					//courseRegistrationDetails = selectedFirstSemesterCourseRegistrationDetails;
					if (selectedFirstSemesterCourseRegistrationDetails != null && selectedFirstSemesterCourseRegistrationDetails.Count > 0)
					{
						courseRegistrationDetails.AddRange(selectedFirstSemesterCourseRegistrationDetails);
					}
					if (selectedSecondSemesterCourseRegistrationDetails != null && selectedSecondSemesterCourseRegistrationDetails.Count > 0)
					{
						courseRegistrationDetails.AddRange(selectedSecondSemesterCourseRegistrationDetails);
					}
					
					for (int i = 0; i < courseRegistrationDetails.Count; i++)
					{
						CourseRegistrationDetail courseRegistrationDetail = courseRegistrationDetails[i];
						courseRegistrationDetails[i].CourseUnit = courseLogic.GetModelBy(c => c.Course_Id == courseRegistrationDetail.Course.Id).Unit;
					}

					viewModel.RegisteredCourse.Details = courseRegistrationDetails;

					for (int i = 0; i < viewModel.RegisteredCourse.Details.Count; i++)
					{
						viewModel.RegisteredCourse.Details[i].ExamScore = null;
						viewModel.RegisteredCourse.Details[i].TestScore = null;
					}

					//viewModel.RegisteredCourse.Student = new Model.Model.Student() { Id = viewModel.Student.Id };

					
					viewModel.RegisteredCourse.Level = new Level() { Id = viewModel.StudentLevel.Level.Id };
					viewModel.RegisteredCourse.Programme = new Programme() { Id = viewModel.StudentLevel.Programme.Id };
					viewModel.RegisteredCourse.Department = new Department() { Id = viewModel.StudentLevel.Department.Id };
					viewModel.RegisteredCourse.Session = new Session() { Id = viewModel.CurrentSessionSemester.SessionSemester.Session.Id };
					CourseRegistration oldCourseRegistration = courseRegistrationLogic.GetBy(viewModel.RegisteredCourse.Student, viewModel.RegisteredCourse.Level, viewModel.RegisteredCourse.Programme,
							viewModel.RegisteredCourse.Department, viewModel.RegisteredCourse.Session);

					CourseRegistration courseRegistration = new CourseRegistration();

					if (oldCourseRegistration != null)
					{
						CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

						courseRegistrationDetailLogic.Delete(c => c.Student_Course_Registration_Id == oldCourseRegistration.Id);
						//courseRegistrationLogic.Delete(a => a.Student_Course_Registration_Id == oldCourseRegistration.Id);

						for (int i = 0; i < viewModel.RegisteredCourse.Details.Count; i++)
						{
							viewModel.RegisteredCourse.Details[i].CourseRegistration = oldCourseRegistration;
						}

						courseRegistrationLogic.Modify(viewModel.RegisteredCourse, courseRegistrationDetailAudit);

						courseRegistration = oldCourseRegistration;
					}
					else
					{
						if (viewModel.RegisteredCourse != null && viewModel.RegisteredCourse.Details != null && viewModel.RegisteredCourse.Details.Count > 0)
						{
							courseRegistration = courseRegistrationLogic.Create(viewModel.RegisteredCourse, courseRegistrationDetailAudit);
						}
						else
						{
							//do nothing
						}
					}
				 
					
					if (courseRegistration != null)
					{
						message = "Selected courses has been successfully registered.";
					}
					else
					{
						message = "Course Registration Failed! Please try again.";
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					message = "Error Occurred! " + ex.Message + ". Please try again.";
				}
				else
				{
					if (ex.InnerException != null)
					{
						message = "Error Occurred! " + ex.Message + ". Please try again." + ex.InnerException.ToString();  
					}
					else
					{
						message = "Error Occurred! " + ex.Message + ". Please try again.";
					}    
				}
			}

			return Json(new { message = message }, "text/html", JsonRequestBehavior.AllowGet);
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

		private List<CourseRegistrationDetail> GetRegisteredCourse(CourseRegistration courseRegistration, List<Course> courses, Semester semester, List<CourseRegistrationDetail> registeredCourseDetails, CourseMode courseMode)
		{
			try
			{
				List<CourseRegistrationDetail> courseRegistrationDetails = null;
				if ((registeredCourseDetails != null && registeredCourseDetails.Count > 0) || (courses != null && courses.Count > 0))
				{
					if (courses != null && courses.Count > 0)
					{
						courseRegistrationDetails = new List<CourseRegistrationDetail>();
						foreach (Course course in courses)
						{
							CourseRegistrationDetail registeredCourseDetail = registeredCourseDetails.Where(c => c.Course.Id == course.Id && c.Mode.Id == courseMode.Id).SingleOrDefault();
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

		private void SetMinimumAndMaximumCourseUnit(Semester firstSemester, Semester secondSemester, Department departmemt, Level level, DepartmentOption departmentOption)
		{
			try
			{
				CourseUnitLogic courseUnitLogic = new CourseUnitLogic();
				CourseUnit firstSemesterCourseUnit = courseUnitLogic.GetBy(departmemt, level, firstSemester, departmentOption);
				if (firstSemesterCourseUnit != null && firstSemesterCourseUnit.Id > 0)
				{
					viewModel.FirstSemesterMinimumUnit = firstSemesterCourseUnit.MinimumUnit;
					viewModel.FirstSemesterMaximumUnit = firstSemesterCourseUnit.MaximumUnit;
				}

				CourseUnit secondSemesterCourseUnit = courseUnitLogic.GetBy(departmemt, level, secondSemester, departmentOption);
				if (secondSemesterCourseUnit != null && secondSemesterCourseUnit.Id > 0)
				{
					viewModel.SecondSemesterMinimumUnit = secondSemesterCourseUnit.MinimumUnit;
					viewModel.SecondSemesterMaximumUnit = secondSemesterCourseUnit.MaximumUnit;
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}
		}
        private void SetMinimumAndMaximumCourseUnit(Semester firstSemester, Semester secondSemester, Department departmemt, Level level, DepartmentOption departmentOption, Programme programme, SessionSemester sessionSemester)
        {
            try
            {
                CourseUnitLogic courseUnitLogic = new CourseUnitLogic();
                CourseUnit firstSemesterCourseUnit = courseUnitLogic.GetBy(sessionSemester.Session, programme, departmemt, level, firstSemester, departmentOption);
                if (firstSemesterCourseUnit != null && firstSemesterCourseUnit.Id > 0)
                {
                    viewModel.FirstSemesterMinimumUnit = firstSemesterCourseUnit.MinimumUnit;
                    viewModel.FirstSemesterMaximumUnit = firstSemesterCourseUnit.MaximumUnit;
                }

                CourseUnit secondSemesterCourseUnit = courseUnitLogic.GetBy(sessionSemester.Session, programme, departmemt, level, secondSemester, departmentOption);
                if (secondSemesterCourseUnit != null && secondSemesterCourseUnit.Id > 0)
                {
                    viewModel.SecondSemesterMinimumUnit = secondSemesterCourseUnit.MinimumUnit;
                    viewModel.SecondSemesterMaximumUnit = secondSemesterCourseUnit.MaximumUnit;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        private void SetMinimumAndMaximumCourseUnit(Semester semester, Department departmemt, Level level, DepartmentOption departmentOption, Programme programme, SessionSemester sessionSemester)
        {
            try
            {
                CourseUnitLogic courseUnitLogic = new CourseUnitLogic();
                CourseUnit semesterCourseUnit = courseUnitLogic.GetBy(sessionSemester.Session, programme, departmemt, level, semester, departmentOption);
                if (semesterCourseUnit != null && semesterCourseUnit.Id > 0)
                {
                    viewModel.MinimumUnit = semesterCourseUnit.MinimumUnit;
                    viewModel.MaximumUnit = semesterCourseUnit.MaximumUnit;
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

		public ActionResult CourseFormPrintOut(string sesid, long sid, int smid)
		{
			try
			{
				SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
				StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                SessionSemester sessionSemester = null;

                StudentLevel studentLevel = studentLevelLogic.GetBy(sid);

				if (sesid != null && smid > 0)
				{
					int sessionId = Convert.ToInt32(sesid);
					sessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == sessionId && s.Semester_Id == smid).LastOrDefault();
				}
				else
				{
					sessionSemester = sessionSemesterLogic.GetModelsBy(s => s.Session_Id == studentLevel.Session.Id).LastOrDefault();
				}

				PopulateCourseRegistrationFormPrintOut(sid, sessionSemester);

                if (sessionSemester != null)
                {
                    if (sessionSemester.Semester.Id == (int)Semesters.FirstSemester)
                    {
                        viewModel.SecondSemesterCourses = null;
                    }
                    else
                    {
                        viewModel.FirstSemesterCourses = null;
                    }
                }

			    if (studentLevel != null)
			    {
                    viewModel.QRVerification = studentLevel.Student.Name + " Course Form " + sessionSemester.Semester.Name + " " + sessionSemester.Session.Name;
			    }
			    else
			    {
                    viewModel.QRVerification = sid + " Course Form " + sessionSemester.Semester.Name + " " + sessionSemester.Session.Name;
			    }
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			return View(viewModel);
		}



		
		//[HttpPost]
		//public ActionResult RegisterCourse(List<int> firstSemesterCourseIds, List<int> secondSemesterCourseIds, long studentId, int programmeId, int departmentId, int sessionId, int levelId)
		//{
		//    try
		//    {
		//        CourseRegistration courseRegistration = new CourseRegistration();
		//        List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();

		//        if (firstSemesterCourseIds != null && firstSemesterCourseIds.Count > 0 && secondSemesterCourseIds != null && secondSemesterCourseIds.Count > 0)
		//        {
		//            courseRegistration.Student = new Model.Model.Student() { Id = studentId };
		//            courseRegistration.Level = new Level() { Id = levelId };
		//            courseRegistration.Programme = new Programme() { Id = programmeId };
		//            courseRegistration.Department = new Department() { Id = departmentId };
		//            courseRegistration.Session = new Session() { Id = sessionId };
					
		//            foreach (int id in firstSemesterCourseIds)
		//            {
		//                CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
		//                courseRegistrationDetail.Course = new Course() { Id = id };
		//                courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };
		//                courseRegistrationDetail.Semester = new Semester() { Id = 1 };
		//                courseRegistrationDetails.Add(courseRegistrationDetail);
		//            }

		//            foreach (int id in secondSemesterCourseIds)
		//            {
		//                CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
		//                courseRegistrationDetail.Course = new Course() { Id = id };
		//                courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };
		//                courseRegistrationDetail.Semester = new Semester() { Id = 2 };
		//                courseRegistrationDetails.Add(courseRegistrationDetail);
		//            }
		//        }

		//        courseRegistration.Details = courseRegistrationDetails;
		//        CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
		//        CourseRegistration CourseRegistration = courseRegistrationLogic.Create(courseRegistration);
		//        if (CourseRegistration != null)
		//        {
		//            SetMessage("Selected Course has been successfully registered", Message.Category.Error);
		//        }
		//        else
		//        {
		//            SetMessage("Selected Course Registration failed!", Message.Category.Error);
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
		//    }

		//    return View(new CourseRegistrationViewModel());
		//    //return PartialView("_ApplicationFormsGrid", null);
		//}


	    public JsonResult LoadStudentRegisteredCourses(long studentId, int sessionSemesterId)
	    {
            RegistrationModel result = new RegistrationModel();
            List<CourseModel> courseModels = new List<CourseModel>();
	        int totalCarryOverUnit = 0;
	        try
	        {
	            if (studentId > 0 && sessionSemesterId > 0)
	            {
                    SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                    SessionSemester sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Semester_Id == sessionSemesterId);
	                if (sessionSemester != null)
	                {
	                    CourseRegistrationDetailLogic registrationDetailLogic = new CourseRegistrationDetailLogic();
	                    List<CourseRegistrationDetail> registrationDetails =
	                        registrationDetailLogic.GetModelsBy(
	                            r =>
	                                r.STUDENT_COURSE_REGISTRATION.Person_Id == studentId &&
	                                r.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id &&
	                                r.Semester_Id == sessionSemester.Semester.Id);

	                    if (registrationDetails != null && registrationDetails.Count > 0)
	                    {
	                        totalCarryOverUnit = registrationDetails.Count(r => r.Mode.Id == (int) CourseModes.CarryOver);
	                        List<CourseRegistrationDetail> firstAttemptRegistrations =
	                            registrationDetails.Where(r => r.Mode.Id == (int) CourseModes.FirstAttempt).ToList();
	                        firstAttemptRegistrations.ForEach(r =>
	                        {
	                            CourseModel courseModel = new CourseModel();
	                            courseModel.CourseCode = r.Course.Code;
	                            courseModel.CourseId = r.Course.Id;
	                            courseModel.CourseRegistrationId = r.Id;
	                            courseModel.CourseTitle = r.Course.Name;
	                            courseModel.CourseType = r.Course.Type.Name;
	                            courseModel.CourseUnit = r.CourseUnit ?? r.Course.Unit;
	                            courseModel.IsRegistered = true;

	                            courseModels.Add(courseModel);
	                        });
	                    }


	                    result.CourseModels = courseModels;
	                    result.CarryOverTotalUnit = totalCarryOverUnit;

	                    result.IsError = false;
	                }
	                else
	                {
                        result.IsError = true;
                        result.Message = "Session not set.";
	                }
	            }
                else
                {
                    result.IsError = true;
                    result.Message = "Invalid Parameters.";
                }
	        }
	        catch (Exception ex)
	        {
                return Json(null, JsonRequestBehavior.AllowGet); ;
	        }

            return Json(result, JsonRequestBehavior.AllowGet);
	    }

        public JsonResult RegisteredCourses(string courses, long studentId, int sessionSemesterId, bool courseAlreadyRegistered, bool carryOverExist)
	    {
            RegistrationModel result = new RegistrationModel();
            string message = null;
            bool isError = false;

            try
            {
                CourseLogic courseLogic = new CourseLogic();
                StudentLogic studentLogic = new StudentLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();

                Model.Model.Student student = studentLogic.GetModelBy(s => s.Person_Id == studentId);
                SessionSemester sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Semester_Id == sessionSemesterId);

                string operation = "INSERT";
                string action = "REGISTRATION :COURSE FORM";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                UserLogic loggeduser = new UserLogic();
                courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Id == 1);

                List<CourseRegistrationDetail> selectedCourseRegistrationDetails = null;
                List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();
                CourseRegistration RegisteredCourse = new CourseRegistration();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<CourseModel> courseModels = serializer.Deserialize<List<CourseModel>>(courses);

                if (carryOverExist)
                {
                    //to be implemented later
                    //List<CourseRegistrationDetail> selectedCarryOverCourseRegistrationDetails = new List<CourseRegistrationDetail>();
                    //selectedCarryOverCourseRegistrationDetails = GetSelectedCourses(viewModel.CarryOverCourses);
                    //courseRegistrationDetails.AddRange(selectedCarryOverCourseRegistrationDetails);
                }

                RegisteredCourse.Student = student;

                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                if (courseAlreadyRegistered) //modify
                {
                    RegisteredCourse = courseRegistrationLogic.GetModelsBy(c => c.Person_Id == studentId && c.Session_Id == sessionSemester.Session.Id).LastOrDefault();

                    selectedCourseRegistrationDetails = GetCourseRegistration(courseModels, sessionSemester);

                    //courseRegistrationDetails = selectedFirstSemesterCourseRegistrationDetails;
                    if (selectedCourseRegistrationDetails != null && selectedCourseRegistrationDetails.Count > 0)
                    {
                        courseRegistrationDetails.AddRange(selectedCourseRegistrationDetails);
                    }

                    for (int i = 0; i < courseRegistrationDetails.Count; i++)
                    {
                        CourseRegistrationDetail courseRegistrationDetail = courseRegistrationDetails[i];
                        courseRegistrationDetails[i].CourseUnit = courseLogic.GetModelBy(c => c.Course_Id == courseRegistrationDetail.Course.Id).Unit;
                    }

                    RegisteredCourse.Details = courseRegistrationDetails;

                    for (int i = 0; i < RegisteredCourse.Details.Count; i++)
                    {
                        RegisteredCourse.Details[i].ExamScore = null;
                        RegisteredCourse.Details[i].TestScore = null;
                        RegisteredCourse.Details[i].CourseRegistration = RegisteredCourse;
                    }

                    courseRegistrationDetailAudit.Operation = "MODIFY: COURSE FORM";
                    bool modified = courseRegistrationLogic.Modify(RegisteredCourse, courseRegistrationDetailAudit);
                    if (modified)
                    {
                        isError = false;
                        message = "Selected courses has been successfully modified.";
                    }
                    else
                    {
                        isError = true;
                        message = "Course Registration modification Failed! Please try again.";
                    }
                }
                else //insert
                {
                    selectedCourseRegistrationDetails = GetCourseRegistration(courseModels, sessionSemester);

                    if (selectedCourseRegistrationDetails != null && selectedCourseRegistrationDetails.Count > 0)
                    {
                        courseRegistrationDetails.AddRange(selectedCourseRegistrationDetails);
                    }

                    for (int i = 0; i < courseRegistrationDetails.Count; i++)
                    {
                        CourseRegistrationDetail courseRegistrationDetail = courseRegistrationDetails[i];
                        courseRegistrationDetails[i].CourseUnit = courseLogic.GetModelBy(c => c.Course_Id == courseRegistrationDetail.Course.Id).Unit;
                    }

                    RegisteredCourse.Details = courseRegistrationDetails;

                    for (int i = 0; i < RegisteredCourse.Details.Count; i++)
                    {
                        RegisteredCourse.Details[i].ExamScore = null;
                        RegisteredCourse.Details[i].TestScore = null;
                    }

                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == student.Id).LastOrDefault();

                    RegisteredCourse.Level = new Level() { Id = studentLevel.Level.Id };
                    RegisteredCourse.Programme = new Programme() { Id = studentLevel.Programme.Id };
                    RegisteredCourse.Department = new Department() { Id = studentLevel.Department.Id };
                    RegisteredCourse.Session = new Session() { Id = sessionSemester.Session.Id };
                    CourseRegistration oldCourseRegistration = courseRegistrationLogic.GetBy(student, RegisteredCourse.Level, RegisteredCourse.Programme, RegisteredCourse.Department, RegisteredCourse.Session);

                    CourseRegistration courseRegistration = new CourseRegistration();

                    if (oldCourseRegistration != null)
                    {
                        CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                        courseRegistrationDetailLogic.Delete(c => c.Student_Course_Registration_Id == oldCourseRegistration.Id);

                        for (int i = 0; i < RegisteredCourse.Details.Count; i++)
                        {
                            RegisteredCourse.Details[i].CourseRegistration = oldCourseRegistration;
                        }

                        courseRegistrationLogic.Modify(RegisteredCourse, courseRegistrationDetailAudit);

                        courseRegistration = oldCourseRegistration;
                    }
                    else
                    {
                        if (RegisteredCourse != null && RegisteredCourse.Details != null && RegisteredCourse.Details.Count > 0)
                            courseRegistration = courseRegistrationLogic.Create(RegisteredCourse, courseRegistrationDetailAudit);
                    }

                    message = courseRegistration != null ? "Selected courses has been successfully registered." : "Course Registration Failed! Please try again.";
                    isError = courseRegistration == null;
                }
            }
            catch (Exception ex)
            {
                isError = true;
                if (ex.InnerException != null)
                {
                    message = "Error Occurred! " + ex.Message + ". Please try again.";
                }
                else
                {
                    if (ex.InnerException != null)
                    {
                        message = "Error Occurred! " + ex.Message + ". Please try again." + ex.InnerException.ToString();
                    }
                    else
                    {
                        message = "Error Occurred! " + ex.Message + ". Please try again.";
                    }
                }
            }

            result.IsError = isError;
            result.Message = message;

            return Json(result, JsonRequestBehavior.AllowGet);
	    }

        private List<CourseRegistrationDetail> GetCourseRegistration(List<CourseModel> courseModels, SessionSemester sessionSemester)
        {
            List<CourseRegistrationDetail> regDetails = new List<CourseRegistrationDetail>();
            try
            {
                for (int i = 0; i < courseModels.Count; i++)
                {
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    courseRegistrationDetail.Course = new Course { Id = courseModels[i].CourseId, IsRegistered = courseModels[i].IsRegistered };
                    courseRegistrationDetail.CourseUnit = courseModels[i].CourseUnit;
                    courseRegistrationDetail.Id = courseModels[i].CourseRegistrationId;
                    courseRegistrationDetail.Semester = sessionSemester.Semester;
                    courseRegistrationDetail.Mode = new CourseMode(){ Id = (int)CourseModes.FirstAttempt };
                    
                    regDetails.Add(courseRegistrationDetail);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regDetails;
        }

        [HttpPost]
        public JsonResult SearchCourse(string prefix, int deptId, int progId, int optionId)
        {
            try
            {
                if (!string.IsNullOrEmpty(prefix) && deptId > 0 && progId > 0)
                {
                    progId = progId == 2 || progId == 6 ? 1 : progId == 4 || progId == 5 ? 3 : progId;
                    CourseLogic coursLogic = new CourseLogic();
                    
                    List<string> courseNames = new List<string>();
                    //List<COURSE> courses = new List<COURSE>();
                    if (optionId > 0)
                    {
                        coursLogic.GetEntitiesBy(
                           c =>
                               c.Department_Id == deptId && c.Programme_Id == progId && c.Department_Option_Id == optionId && c.Activated &&
                               (c.Course_Name.Contains(prefix) || c.Course_Code.Contains(prefix)))
                               .ForEach(c =>
                               {
                                   courseNames.Add(c.Course_Code + ": " + c.Course_Unit + " unit(s) " + c.Course_Name);
                               });
                    }
                    else
                    {
                        coursLogic.GetEntitiesBy(
                           c =>
                               c.Department_Id == deptId && c.Programme_Id == progId && c.Activated &&
                               (c.Course_Name.Contains(prefix) || c.Course_Code.Contains(prefix)))
                               .ForEach(c =>
                               {
                                   courseNames.Add(c.Course_Code + ": " + c.Course_Unit + " unit(s) " + c.Course_Name);
                                   var check = c.Course_Id;
                                   TempData["Active_CourseId"] = c.Course_Id;
                                   TempData.Keep("Active_CourseId");
                               });
                    }
                   
                    //ForEach(c =>
                    //{
                    //    courseNames.Add(c.Course_Code + ": " + c.Course_Unit + " unit(s) " + c.LEVEL.Level_Description);
                    //});

                    return Json(courseNames, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new List<COURSE>{new COURSE{Course_Code= ex.Message}}, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourse(string courseValue, int deptId, int progId)
	    {
            long course_Id = 0;
            RegistrationModel result = new RegistrationModel();
            List<CourseModel> courseModels = new List<CourseModel>();
            
            int totalCarryOverUnit = 0;
            try
            {
                if (!string.IsNullOrEmpty(courseValue) && deptId > 0 && progId > 0)
                    {
                    char[] separator = { ':'};
                    String[] strlist = courseValue.Split(separator);
                    var extract_course_code = strlist[0];

                    progId = progId == 2 || progId == 6 ? 1 : progId == 4 || progId == 5 ? 3 : progId;
                    CourseLogic courseLogic = new CourseLogic();
                    var course = courseLogic.GetModelBy(c => c.Course_Code == extract_course_code && c.Programme_Id == progId && c.Department_Id == deptId);

                    //var course = courseLogic.GetModelsBy(c => c.Course_Code + ": " + c.Course_Unit + " unit(s) " + c.Course_Name == courseValue && c.Department_Id == deptId && c.Programme_Id == progId && c.Activated).LastOrDefault();

                    if (course != null)
                    {
                        CourseModel courseModel = new CourseModel();
                        courseModel.CourseCode = course.Code;
                        courseModel.CourseId = course.Id;
                        courseModel.CourseRegistrationId = 0;
                        courseModel.CourseTitle = course.Name;
                        courseModel.CourseType = course.Type.Name;
                        courseModel.CourseUnit = course.Unit;
                        courseModel.IsRegistered = true;

                        courseModels.Add(courseModel);
                    }

                    result.CourseModels = courseModels;
                    result.CarryOverTotalUnit = totalCarryOverUnit;

                    result.IsError = false;
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Invalid Parameters.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Invalid Parameters.";
                return Json(result, JsonRequestBehavior.AllowGet); ;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
	    }
	}


}