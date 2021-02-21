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
    public class EAssignmentLogic : BusinessBaseLogic<EAssignment, E_ASSIGNMENT>
    {
        public EAssignmentLogic()
        {
            translator = new EAssignmentTranslator();
        }

        public List<EAssignment> getBy(long CourseId)
        {
            try
            {
                return GetModelsBy(a => a.Course_Id == CourseId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public EAssignment GetAssignment(long Id)
        {
            try
            {
                return GetModelBy(a => a.Id == Id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool Modify(EAssignment model)
        {
            try
            {

                Expression<Func<E_ASSIGNMENT, bool>> selector = c => c.Id == model.Id;
                E_ASSIGNMENT entity = GetEntityBy(selector);
                if (entity?.Id > 0)
                {
                    entity.Instructions = model.Instructions;
                    entity.Assignment = model.Assignment;
                    entity.Date_Set = model.DateSet;
                    entity.Due_Date = model.DueDate;
                    entity.IsDelete = model.IsDelete;
                    entity.Publish = model.Publish;
                    if (model.CourseAllocation?.Id > 0)
                    {
                        entity.Course_Allocation_Id = model.CourseAllocation.Id;
                    }
                    if (model.Course?.Id > 0)
                    {
                        entity.Course_Id = model.Course.Id;
                    }

                    int modifiedRecordCount = Save();
                    if (modifiedRecordCount <= 0)
                    {
                        return false;
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public List<ElearningResult> GetEAssignmentResult(CourseAllocation courseAllocation)
        {
            List<ElearningResult> result = new List<ElearningResult>();
            try
            {
                if (courseAllocation?.Id > 0)
                {
                    EAssignmentLogic eAssignmentLogic = new EAssignmentLogic();
                    EAssignmentSubmissionLogic assignmentSubmissionLogic = new EAssignmentSubmissionLogic();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    var classList = courseRegistrationDetailLogic.GetModelsBy(f => f.Course_Id == courseAllocation.Course.Id && f.STUDENT_COURSE_REGISTRATION.Department_Id == courseAllocation.Department.Id
                      && f.STUDENT_COURSE_REGISTRATION.Programme_Id == courseAllocation.Programme.Id && f.STUDENT_COURSE_REGISTRATION.Level_Id == courseAllocation.Level.Id && f.STUDENT_COURSE_REGISTRATION.Session_Id == courseAllocation.Session.Id).Select(f => f.CourseRegistration.Student);
                    var eAssignment = eAssignmentLogic.GetModelsBy(d => d.Course_Allocation_Id == courseAllocation.Id && d.IsDelete == false);
                    if (classList?.Count() > 0 && eAssignment.Count > 0)
                    {
                        var assignmentCount = eAssignment.Count;
                        foreach (var item in classList)
                        {
                            ElearningResult elearningResult = new ElearningResult();
                            var score = ((assignmentSubmissionLogic.GetModelsBy(f => f.E_ASSIGNMENT.Course_Allocation_Id == courseAllocation.Id && f.E_ASSIGNMENT.IsDelete == false && f.Student_Id == item.Id).Sum(f => f.Score)) / assignmentCount);
                            elearningResult.AverageScore = (decimal)score;
                            elearningResult.CourseCode = courseAllocation.Course.Code;
                            elearningResult.CourseName = courseAllocation.Course.Name;
                            elearningResult.Name = item.FullName;
                            elearningResult.NoOfTest = assignmentCount;
                            elearningResult.SessionName = courseAllocation.Session.Name;
                            elearningResult.MatricNo = item.MatricNumber;
                            elearningResult.LecturerName = courseAllocation.User.Email;
                            elearningResult.Programme = courseAllocation.Programme.Name;
                            elearningResult.Department = courseAllocation.Department.Name;
                            elearningResult.Semester = courseAllocation.Semester.Name;
                            elearningResult.School = courseAllocation.Department.Faculty.Name;
                            elearningResult.LevelName = courseAllocation.Level.Name;
                            result.Add(elearningResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }
    }
}
