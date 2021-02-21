using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StaffDepartmentTranslator : TranslatorBase<StaffDepartment, STAFF_DEPARTMENT>
    {
        private StaffTranslator staffTranslator;
        private DepartmentTranslator departmentTranslator;
        private SessionSemesterTranslator sessionSemstSemesterTranslator;

        public StaffDepartmentTranslator()
        {
            staffTranslator = new StaffTranslator();
            departmentTranslator = new DepartmentTranslator();
            sessionSemstSemesterTranslator = new SessionSemesterTranslator();
        }

        public override StaffDepartment TranslateToModel(STAFF_DEPARTMENT entity)
        {
            try
            {
                StaffDepartment model = null;
                if (entity != null)
                {
                    model = new StaffDepartment();

                    model.Id = entity.Staff_Department_Id;
                    model.DateEntered = entity.Date_Entered;
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.IsHead = entity.IsHead;
                    model.IsExamOfficer = entity.IsExamOfficer;
                    model.SessionSemester = sessionSemstSemesterTranslator.Translate(entity.SESSION_SEMESTER);
                    model.Staff = staffTranslator.Translate(entity.STAFF);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STAFF_DEPARTMENT TranslateToEntity(StaffDepartment model)
        {
            try
            {
                STAFF_DEPARTMENT entity = null;
                if (model != null)
                {
                    entity = new STAFF_DEPARTMENT();

                    entity.Date_Entered = model.DateEntered;
                    entity.IsHead = model.IsHead;
                    entity.Staff_Department_Id = model.Id;
                    entity.Department_Id = model.Department.Id;
                    if (model.SessionSemester?.Id > 0)
                    {
                        entity.Session_Semester_Id = model.SessionSemester.Id;
                    }
                    
                    entity.Staff_Id = model.Staff.Id;
                    entity.IsExamOfficer = model.IsExamOfficer;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
