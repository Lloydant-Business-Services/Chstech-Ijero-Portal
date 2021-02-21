using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class CourseAllocationLogic : BusinessBaseLogic<CourseAllocation, COURSE_ALLOCATION>
    {
        public CourseAllocationLogic()
        {
            translator = new CourseAllocationTranslator();
        }

        public bool Modify(CourseAllocation courseAllocation)
        {
            try
            {
                Expression<Func<COURSE_ALLOCATION, bool>> selector = l => l.Course_Allocation_Id == courseAllocation.Id;
                COURSE_ALLOCATION entityCourseAllocation = GetEntityBy(selector);
                if (entityCourseAllocation == null)
                {
                    throw new Exception("Not Found");
                }

                if (courseAllocation.User != null)
                {
                    entityCourseAllocation.User_Id = courseAllocation.User.Id; 
                }
                if (courseAllocation.Course != null)
                {
                    entityCourseAllocation.Course_Id = courseAllocation.Course.Id;
                }
                entityCourseAllocation.Is_HOD = courseAllocation.IsHOD;
                entityCourseAllocation.Can_Upload = courseAllocation.CanUpload;
                if (courseAllocation.HodDepartment != null)
                {
                    entityCourseAllocation.HOD_Department_Id = courseAllocation.HodDepartment.Id; 
                } 

                int modofiedCount = Save();
                if (modofiedCount > 0)
                {
                    return true;
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
