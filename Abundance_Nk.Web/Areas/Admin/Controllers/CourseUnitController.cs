using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class CourseUnitController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/CourseUnit
        public ActionResult Index()
        {
            var cOURSE_UNIT = db.COURSE_UNIT.Include(c => c.DEPARTMENT).Include(c => c.LEVEL).Include(c => c.SEMESTER).Include(c => c.DEPARTMENT_OPTION).Include(c => c.PROGRAMME).Include(c => c.SESSION);
            return View(cOURSE_UNIT.OrderBy(c => c.Session_Id).ThenBy(c => c.Programme_Id).ThenBy(c => c.Level_Id).ThenBy(c => c.Department_Id).ThenBy(c => c.Semester_Id).ToList());
        }

        // GET: Admin/CourseUnit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COURSE_UNIT cOURSE_UNIT = db.COURSE_UNIT.Find(id);
            if (cOURSE_UNIT == null)
            {
                return HttpNotFound();
            }
            return View(cOURSE_UNIT);
        }

        // GET: Admin/CourseUnit/Create
        public ActionResult Create()
        {
            CourseUnitViewModel vModel = new CourseUnitViewModel();
            ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
            ViewBag.SessionSelectList = vModel.SessionSelectList;
            ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
            ViewBag.SemesterSelectList = vModel.SemesterSelectList;
            ViewBag.LevelSelectList = vModel.LevelSelectList;

            //db.DEPARTMENT_OPTION.Add(new DEPARTMENT_OPTION(){ Department_Option_Id = 0, Department_Option_Name = "-- No Option --"});

            //ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name");
            //ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name");
            //ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name");
            //ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name");
            //ViewBag.Semester_Id = new SelectList(db.SEMESTER, "Semester_Id", "Semester_Name");
            //ViewBag.Department_Option_Id = new SelectList(db.DEPARTMENT_OPTION, "Department_Option_Id", "Department_Option_Name");
            return View();
        }
        [HttpPost]
        public ActionResult Create(CourseUnitViewModel vModel)
        {
            ProgrammeDepartment programmeDepartment = new ProgrammeDepartment();
            ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();
            CourseUnit courseUnit = new CourseUnit();
            CourseUnitLogic courseUnitLogic = new CourseUnitLogic();
            if(vModel != null)
            {
                var programmeDept = programmeDepartmentLogic.GetModelsBy(p => p.Programme_Id == vModel.Programme.Id);
                if(programmeDept != null)
                {
                    
                    foreach(var item in programmeDept)
                    {
                        courseUnit.Department = item.Department;
                        courseUnit.Session = vModel.Session;
                        courseUnit.Programme = vModel.Programme;
                        courseUnit.Semester = vModel.Semester;
                        courseUnit.Level = vModel.Level;
                        courseUnit.MaximumUnit = vModel.Maximum_Unit;
                        courseUnit.MinimumUnit = vModel.Minimum_Unit;

                        courseUnitLogic.Create(courseUnit);

                    }

                    ViewBag.ProgrammeSelectList = vModel.ProgrammeSelectList;
                    ViewBag.SessionSelectList = vModel.SessionSelectList;
                    ViewBag.DepartmentSelectList = vModel.DepartmentSelectList;
                    ViewBag.SemesterSelectList = vModel.SemesterSelectList;
                    ViewBag.LevelSelectList = vModel.LevelSelectList;

                    //ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name");
                    //ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name");
                    //ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name");
                    //ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name");
                    //ViewBag.Semester_Id = new SelectList(db.SEMESTER, "Semester_Id", "Semester_Name");
                    //ViewBag.Department_Option_Id = new SelectList(db.DEPARTMENT_OPTION, "Department_Option_Id", "Department_Option_Name");
                }

                ViewBag.SuccessMsg = "Operation Successful";
            }

            return View(vModel);
        }
        // POST: Admin/CourseUnit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Course_Unit_Id,Department_Id,Programme_Id,Level_Id,Semester_Id,Minimum_Unit,Maximum_Unit,Department_Option_Id,Session_Id")] COURSE_UNIT cOURSE_UNIT)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (cOURSE_UNIT.Department_Option_Id <= 0)
        //        {
        //            cOURSE_UNIT.DEPARTMENT_OPTION = null;
        //            cOURSE_UNIT.Department_Option_Id = null;
        //        }

        //        db.COURSE_UNIT.Add(cOURSE_UNIT);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", cOURSE_UNIT.Department_Id);
        //    ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", cOURSE_UNIT.Programme_Id);
        //    ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name", cOURSE_UNIT.Session_Id);
        //    ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name", cOURSE_UNIT.Level_Id);
        //    ViewBag.Semester_Id = new SelectList(db.SEMESTER, "Semester_Id", "Semester_Name", cOURSE_UNIT.Semester_Id);
        //    ViewBag.Department_Option_Id = new SelectList(db.DEPARTMENT_OPTION, "Department_Option_Id", "Department_Option_Name", cOURSE_UNIT.Department_Option_Id);
        //    return View(cOURSE_UNIT);
        //}

        // GET: Admin/CourseUnit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COURSE_UNIT cOURSE_UNIT = db.COURSE_UNIT.Find(id);
            if (cOURSE_UNIT == null)
            {
                return HttpNotFound();
            }

            ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name", cOURSE_UNIT.Session_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", cOURSE_UNIT.Programme_Id);
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", cOURSE_UNIT.Department_Id);
            ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name", cOURSE_UNIT.Level_Id);
            ViewBag.Semester_Id = new SelectList(db.SEMESTER, "Semester_Id", "Semester_Name", cOURSE_UNIT.Semester_Id);
            ViewBag.Department_Option_Id = new SelectList(db.DEPARTMENT_OPTION, "Department_Option_Id", "Department_Option_Name", cOURSE_UNIT.Department_Option_Id);
            return View(cOURSE_UNIT);
        }

        // POST: Admin/CourseUnit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Course_Unit_Id,Department_Id,Programme_Id,Level_Id,Semester_Id,Minimum_Unit,Maximum_Unit,Department_Option_Id,Session_Id")] COURSE_UNIT cOURSE_UNIT)
        {
            if (ModelState.IsValid)
            {
                if (cOURSE_UNIT.Department_Option_Id <= 0)
                {
                    cOURSE_UNIT.DEPARTMENT_OPTION = null;
                    cOURSE_UNIT.Department_Option_Id = null;
                }

                db.Entry(cOURSE_UNIT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name", cOURSE_UNIT.Session_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", cOURSE_UNIT.Programme_Id);
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", cOURSE_UNIT.Department_Id);
            ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name", cOURSE_UNIT.Level_Id);
            ViewBag.Semester_Id = new SelectList(db.SEMESTER, "Semester_Id", "Semester_Name", cOURSE_UNIT.Semester_Id);
            ViewBag.Department_Option_Id = new SelectList(db.DEPARTMENT_OPTION, "Department_Option_Id", "Department_Option_Name", cOURSE_UNIT.Department_Option_Id);
            return View(cOURSE_UNIT);
        }

        // GET: Admin/CourseUnit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COURSE_UNIT cOURSE_UNIT = db.COURSE_UNIT.Find(id);
            if (cOURSE_UNIT == null)
            {
                return HttpNotFound();
            }
            return View(cOURSE_UNIT);
        }

        // POST: Admin/CourseUnit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            COURSE_UNIT cOURSE_UNIT = db.COURSE_UNIT.Find(id);
            db.COURSE_UNIT.Remove(cOURSE_UNIT);
            db.SaveChanges();
            return RedirectToAction("Index");
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

                return Json(new SelectList(departmentOptions, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                return Json(new SelectList(departments, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetLevels(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };
                LevelLogic levelLogic = new LevelLogic();
                List<Level> levels = levelLogic.GetBy(programme);

                return Json(new SelectList(levels, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
