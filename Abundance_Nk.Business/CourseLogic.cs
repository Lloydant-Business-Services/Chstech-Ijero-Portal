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
    public class CourseLogic : BusinessBaseLogic<Course, COURSE>
    {
        public CourseLogic()
        {
            translator = new CourseTranslator();
        }

        public List<Course> GetBy(Department department, Level level, Semester semester)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Activated;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetBy(Programme programme, Department department, Level level, Semester semester)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Programme_Id == programme.Id && c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Activated;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetBy(Department department, DepartmentOption departmentOption, Level level, Semester semester)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Department_Option_Id == departmentOption.Id && c.Activated;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetBy(Programme programme, Department department, DepartmentOption departmentOption, Level level, Semester semester)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Programme_Id == programme.Id && c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Department_Option_Id == departmentOption.Id && c.Activated;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetBy(Department department, Level level, Semester semester, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetBy(Department department, DepartmentOption departmentOption, Semester semester, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id && c.Semester_Id == semester.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetBy(Department department, Semester semester, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Semester_Id == semester.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetBy(Programme programme, Department department, Level level, Semester semester, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Programme_Id == programme.Id && c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetBy(Programme programme, Department department, DepartmentOption departmentOption, Semester semester, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Programme_Id == programme.Id && c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id && c.Semester_Id == semester.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetBy(Programme programme, Department department, Semester semester, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Programme_Id == programme.Id && c.Department_Id == department.Id && c.Semester_Id == semester.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetBy(Department department, DepartmentOption departmentOption, Level level, Semester semester, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetBy(Programme programme, Department department, DepartmentOption departmentOption, Level level, Semester semester, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Programme_Id == programme.Id && c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetExtraYearBy(Department department, Level level, Semester semester, bool status)
        {
            try
            {
                if (level.Id <= 2)
                {
                    Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Level_Id <= level.Id && c.Semester_Id == semester.Id && c.Activated == status;
                    return base.GetModelsBy(selector);
                }
                else
                {
                    Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Level_Id > 2 && c.Semester_Id == semester.Id && c.Activated == status;
                    return base.GetModelsBy(selector);
                }

              
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetExtraYearBy(Department department, DepartmentOption departmentOption, Level level, Semester semester, bool status)
        {
            try
            {
                if (level.Id <= 2)
                {
                    Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id && c.Level_Id <= level.Id && c.Semester_Id == semester.Id && c.Activated == status;
                    return base.GetModelsBy(selector);
                }
                else
                {
                    Expression<Func<COURSE, bool>> selector = c => c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id && c.Level_Id > 2 && c.Semester_Id == semester.Id && c.Activated == status;
                    return base.GetModelsBy(selector);
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }

   
        public Course GetBy(long id)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Course_Id == id;
                return base.GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(List<Course> courses)
        {
            try
            {
                foreach(Course course in courses)
                {
                    int modified = 0;
                    Expression<Func<COURSE, bool>> selector = c => c.Course_Id == course.Id;
                    COURSE courseEntity = GetEntityBy(selector);
                    if (courseEntity == null && course.Code != null && course.Name != null && !string.IsNullOrEmpty(course.Code) && !string.IsNullOrEmpty(course.Name))
                    {
                        Course newCourse = new Course();
                        newCourse.Name = course.Name;
                        newCourse.Code = course.Code;
                        newCourse.Department = course.Department;
                        newCourse.Programme = course.Programme;
                        if (course.DepartmentOption.Id > 0)
                        {
                            newCourse.DepartmentOption = course.DepartmentOption;
                        
                        }
                        newCourse.IsRegistered = false;
                        newCourse.Level = course.Level;
                        newCourse.Semester = course.Semester;
                        newCourse.Type = new CourseType() { Id = 1};
                        newCourse.Unit = course.Unit;
                        newCourse.Activated = false;
                        Create(newCourse);
                    }
                    else
                    {
                        if (course.Code == null && course.Unit <= 0 && course.Name == null && String.IsNullOrEmpty(course.Name) && String.IsNullOrEmpty(course.Code))
                        {
                            courseEntity.Activated = false;
                            modified = Save();

                            //Delete(selector);
                        }
                        else
                        {
                            courseEntity.Course_Code = course.Code;
                            courseEntity.Course_Unit = course.Unit;
                            courseEntity.Course_Name = course.Name;
                            modified = Save();
                        }
                        
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool Modify(Course course)
        {
            try
            {
                
                    int modified = 0;
                    Expression<Func<COURSE, bool>> selector = c => c.Course_Id == course.Id;
                    COURSE courseEntity = GetEntityBy(selector);
                    if (courseEntity == null)
                    {
                        Course newCourse = new Course();
                        newCourse.Name = course.Name;
                        newCourse.Code = course.Code;
                        newCourse.Department = course.Department;
                        newCourse.Programme = course.Programme;
                        newCourse.IsRegistered = false;
                        newCourse.Level = course.Level;
                        newCourse.Semester = course.Semester;
                        newCourse.Type = new CourseType() { Id= 1};
                        newCourse.Unit = course.Unit;
                        Create(newCourse);
                        return true;
                    }
                    else
                    {
                        courseEntity.Course_Code = course.Code;
                        courseEntity.Course_Unit = course.Unit;
                        courseEntity.Course_Name = course.Name;
                        courseEntity.Activated = course.Activated;
                        modified = Save();
                    }
              
                
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }
     
  
    
    
    }


}
