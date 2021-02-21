using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using Menu = Abundance_Nk.Model.Model.Menu;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class StudentResultController : BaseController
    {
        private StudentResultViewModel viewModel;
        public ActionResult CreateStudentResultStatus()
        {
            try
            {
                viewModel = new StudentResultViewModel();
                PopulateAllDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateStudentResultStatus(StudentResultViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    LevelLogic levelLogic = new LevelLogic();
                    ProgrammeLogic programmeLogic = new ProgrammeLogic();

                    Level level = levelLogic.GetModelBy(l => l.Level_Id == viewModel.Level.Id);
                    Programme programme = programmeLogic.GetModelBy(p => p.Programme_Id == viewModel.Programme.Id);

                    List<Department> departments = Utility.GetDepartmentByProgramme(programme);
                    Department castedDepartment = departments.Where(d => d.Name == "-- Select Department --").FirstOrDefault();
                    departments.Remove(castedDepartment);

                    viewModel.Programme = programme;
                    viewModel.Level = level;

                    List<StudentResultStatusFormat> studentResultStatusFormats = new List<StudentResultStatusFormat>();
                    for (int i = 0; i < departments.Count; i++)
                    {
                        StudentResultStatusFormat studentResultStatusFormat = new StudentResultStatusFormat();
                        studentResultStatusFormat.Department = departments[i];
                        studentResultStatusFormat.Approved = false;

                        studentResultStatusFormats.Add(studentResultStatusFormat);
                    }


                    viewModel.StudentResultStatusFormats = studentResultStatusFormats;

                    RetainDropDown(viewModel);
                    return View(viewModel);
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error!" + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveStudentResultStatus(StudentResultViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    Programme programme = viewModel.Programme;
                    Level level = viewModel.Level;

                    StudentResultStatusLogic studentResultStatusLogic = new StudentResultStatusLogic();

                    for (int i = 0; i < viewModel.StudentResultStatusFormats.Count; i++)
                    {
                        StudentResultStatusFormat currentStudentResultStatusFormat = viewModel.StudentResultStatusFormats[i];

                        List<StudentResultStatus> studentResultStatusList = studentResultStatusLogic.GetModelsBy(sr => sr.Department_Id == currentStudentResultStatusFormat.Department.Id && sr.Programme_Id == programme.Id && sr.Level_Id == level.Id);
                        if (studentResultStatusList.Count == 0)
                        {
                            if (currentStudentResultStatusFormat.Department.Id != 0)
                            {
                                StudentResultStatus studentResultStatus = new StudentResultStatus();
                                studentResultStatus.Programme = programme;
                                studentResultStatus.Level = level;
                                studentResultStatus.Department = currentStudentResultStatusFormat.Department;
                                studentResultStatus.Activated = currentStudentResultStatusFormat.Approved;

                                studentResultStatusLogic.Create(studentResultStatus); 
                            } 
                            
                        }
                    }

                    SetMessage("Operation Successful! ", Message.Category.Information);
                    return RedirectToAction("CreateStudentResultStatus");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error!" + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("CreateStudentResultStatus");
        }
        public ActionResult ViewStudentResultStatus()
        {
            try
            {
                viewModel = new StudentResultViewModel();
                PopulateAllDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewStudentResultStatus(StudentResultViewModel viewModel)
        {
            try
            {
                if (viewModel.Programme != null && viewModel.Programme.Id > 0 && viewModel.Level != null && viewModel.Level.Id > 0)
                {
                    StudentResultStatusLogic studentResultStatusLogic = new StudentResultStatusLogic();
                    viewModel.StudentResultStatusList = studentResultStatusLogic.GetModelsBy(m => m.Programme_Id == viewModel.Programme.Id && m.Level_Id == viewModel.Level.Id);

                    RetainDropDown(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }
        
        [HttpPost]
        public ActionResult SaveEditedStudentResultStatus(StudentResultViewModel viewModel)
        {
            try
            {
                StudentResultStatusLogic studentResultStatusLogic = new StudentResultStatusLogic();
                
                Programme programme = viewModel.Programme;
                Level level = viewModel.Level;

                for (int i = 0; i < viewModel.StudentResultStatusList.Count; i++)
                {   
                    StudentResultStatus currStudentResultStatus = viewModel.StudentResultStatusList[i];
                    if (currStudentResultStatus.Department != null && currStudentResultStatus.Department.Id != 0)
                    {
                        currStudentResultStatus.Programme = programme;
                        currStudentResultStatus.Level = level;
                        currStudentResultStatus.Department = currStudentResultStatus.Department;
                        currStudentResultStatus.Activated = currStudentResultStatus.Activated;

                        studentResultStatusLogic.Modify(currStudentResultStatus);
                    }  
                }
                
                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewStudentResultStatus");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View("ViewStudentResultStatus", viewModel);
        }

        public ActionResult ConfirmDeleteResultStatus(int rid)
        {
            try
            {
                viewModel = new StudentResultViewModel();
                if (rid > 0)
                {
                    StudentResultStatusLogic studentResultStatusLogic = new StudentResultStatusLogic();
                    viewModel.StudentResultStatus = studentResultStatusLogic.GetModelBy(x => x.Id == rid);
                    if (viewModel.StudentResultStatus != null)
                    {
                        viewModel.Programme = viewModel.StudentResultStatus.Programme;
                        viewModel.Level = viewModel.StudentResultStatus.Level;
                    }

                    RetainDropDown(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteResultStatus(StudentResultViewModel viewModel)
        {
            try
            {
                StudentResultStatusLogic studentResultStatusLogic = new StudentResultStatusLogic();
                studentResultStatusLogic.Delete(x => x.Id == viewModel.StudentResultStatus.Id);

                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewStudentResultStatus");

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ConfirmDeleteResultStatus", new { rid = viewModel.StudentResultStatus.Id });
        }

        public void PopulateAllDropDown(StudentResultViewModel viewModel)
        {
            try
            {
                ViewBag.Programme = viewModel.ProgrammeSelectListItem;
                ViewBag.Level = viewModel.LevelSelectListItem;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RetainDropDown(StudentResultViewModel viewModel)
        {
            try
            {
                if (viewModel.Programme != null)
                {
                    ViewBag.Programme = new SelectList(viewModel.ProgrammeSelectListItem, "Value", "Text", viewModel.Programme.Id);
                }
                else
                {
                    ViewBag.Programme = viewModel.ProgrammeSelectListItem;
                }
                if (viewModel.Level != null)
                {
                    ViewBag.Level = new SelectList(viewModel.LevelSelectListItem, "Value", "Text", viewModel.Level.Id);
                }
                else
                {
                    ViewBag.Level = viewModel.LevelSelectListItem;
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}