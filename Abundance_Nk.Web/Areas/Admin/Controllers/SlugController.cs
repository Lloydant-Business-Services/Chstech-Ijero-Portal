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
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class SlugController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private SlugViewModel viewmodel;
        public ActionResult Index()
        {
            viewmodel = new SlugViewModel();
            
            try
            {
                viewmodel.ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);

            }
            catch (Exception ex)
            {
                
               SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SlugViewModel Slugviewmodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Model.Model.Session currentSession = new Model.Model.Session() { Id = 7 };
                    ApplicationFormLogic appFormLogic = new ApplicationFormLogic();
                    List<Slug> appliedCourse = appFormLogic.GetPostJAMBSlugDataBy(currentSession, Slugviewmodel.appliedCourse.Programme, Slugviewmodel.appliedCourse.Department);
                    Slugviewmodel.applicantDetails = appliedCourse;

                    ViewBag.ProgrammeId = new SelectList(Slugviewmodel.ProgrammeSelectListItem, VALUE, TEXT);
                    ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);


                    StringWriter sw = new StringWriter();

                    //First line for column names
                    sw.WriteLine("\"EXAMNO1\",\"EXAMNO2\",\"JAMB1\",\"JAMB2\",\"JAMBSCORE\",\"DEPT1\",\"DEPT2\",\"TYPE\",\"NAME\",\"PHOTO\"");

                    foreach (Slug item in appliedCourse)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",
                                                   item.ExamNumber,
                                                   item.ExamNumber,
                                                   item.JambNumber,
                                                   item.JambNumber,
                                                   item.JambScore,
                                                   item.FirstChoiceDepartment,
                                                   item.SecondChoiceDepartment,
                                                   "H",
                                                   item.Name.ToUpper(),
                                                   item.PassportUrl));
                    }

                    Response.AddHeader("Content-Disposition", "attachment; filename=test.csv");
                    Response.ContentType = "text/csv";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                    Response.Write(sw);
                    Response.End(); 



                    return View(Slugviewmodel);
                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            viewmodel.ProgrammeSelectListItem = Utility.PopulateProgrammeSelectListItem();
            ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
            ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);

            return View(Slugviewmodel);
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

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetSlugData(string programmeId, string departmentId)
        {
            try
            {
                if (string.IsNullOrEmpty(programmeId) || string.IsNullOrEmpty(departmentId))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(programmeId) };
                Department department = new Department() { Id = Convert.ToInt32(departmentId) };

                AppliedCourseLogic appliedcourseLogic = new AppliedCourseLogic();
                var appliedCourse = appliedcourseLogic.GetModelsBy(a => a.Programme_Id == programme.Id && a.Department_Id == department.Id);

                return Json(new { iCount = appliedCourse.Count , aaData = appliedCourse}, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    
    }
}