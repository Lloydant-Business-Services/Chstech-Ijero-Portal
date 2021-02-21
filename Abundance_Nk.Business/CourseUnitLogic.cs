using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class CourseUnitLogic : BusinessBaseLogic<CourseUnit, COURSE_UNIT>
    {
        public CourseUnitLogic()
        {
            translator = new CourseUnitTranslator();
        }

        public CourseUnit GetBy(Department department, Level level, Semester semester, DepartmentOption departmentOption)
        {
            try
            {
                if (departmentOption != null && departmentOption.Id > 0)
                {
                    Expression<Func<COURSE_UNIT, bool>> selector = cu => cu.Department_Id == department.Id && cu.Level_Id == level.Id && cu.Semester_Id == semester.Id && cu.Department_Option_Id == departmentOption.Id;
                    return GetModelBy(selector);
                }
                else
                {
                    Expression<Func<COURSE_UNIT, bool>> selector = cu => cu.Department_Id == department.Id && cu.Level_Id == level.Id && cu.Semester_Id == semester.Id;
                    return GetModelBy(selector);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public CourseUnit GetBy(Session session, Programme programme, Department department, Level level, Semester semester, DepartmentOption departmentOption)
        {
            try
            {
                if (departmentOption != null && departmentOption.Id > 0)
                {
                    Expression<Func<COURSE_UNIT, bool>> selector = cu => cu.Programme_Id == programme.Id && cu.Session_Id == session.Id && cu.Department_Id == department.Id && cu.Level_Id == level.Id && cu.Semester_Id == semester.Id && cu.Department_Option_Id == departmentOption.Id;
                    return GetModelsBy(selector).LastOrDefault();
                }
                else
                {
                    Expression<Func<COURSE_UNIT, bool>> selector = cu => cu.Programme_Id == programme.Id && cu.Session_Id == session.Id && cu.Department_Id == department.Id && cu.Level_Id == level.Id && cu.Semester_Id == semester.Id;
                    return GetModelsBy(selector).LastOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }




}
