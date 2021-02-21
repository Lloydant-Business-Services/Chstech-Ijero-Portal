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
    public class StudentLevelLogic : BusinessBaseLogic<StudentLevel, STUDENT_LEVEL>
    {
        public StudentLevelLogic()
        {
            translator = new StudentLevelTranslator();
        }

        public StudentLevel GetBy(long studentId)
        {
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Person_Id == studentId ;
                List<StudentLevel> studentLevels = base.GetModelsBy(selector);
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                if (studentLevels != null && studentLevels.Count > 0)
                {
                    int maxLevel =  studentLevels.Max(p => p.Level.Id);
                    Expression<Func<STUDENT_LEVEL, bool>> selector2 = sl => sl.Person_Id == studentId && sl.Level_Id == maxLevel && sl.Session_Id == session.Id;
                    StudentLevel CurrentLevel = base.GetModelBy(selector2);
                    if (CurrentLevel == null)
                    {
                        int minLevel = studentLevels.Min(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector3 = sl => sl.Person_Id == studentId && sl.Level_Id == minLevel;
                        StudentLevel CurrentLevelAlt = base.GetModelsBy(selector3).LastOrDefault();
                        CurrentLevel = CurrentLevelAlt;
                    }
                    return CurrentLevel;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public StudentLevel GetExtraYearBy(long studentId)
        {
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Person_Id == studentId;
                List<StudentLevel> studentLevels = base.GetModelsBy(selector);
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                if (studentLevels != null && studentLevels.Count > 0)
                {
                    int maxLevel = studentLevels.Max(p => p.Level.Id);
                    Expression<Func<STUDENT_LEVEL, bool>> selector2 = sl => sl.Person_Id == studentId && sl.Level_Id == maxLevel && sl.Session_Id == session.Id;
                    StudentLevel CurrentLevel = base.GetModelBy(selector2);
                    if (CurrentLevel == null)
                    {
                        int minLevel = studentLevels.Min(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector3 = sl => sl.Person_Id == studentId && sl.Level_Id == minLevel && sl.Session_Id == session.Id;
                        StudentLevel CurrentLevelAlt = base.GetModelBy(selector3);
                        CurrentLevel = CurrentLevelAlt;
                    }
                    if (CurrentLevel == null)
                    {
                        int maxLevel2 = studentLevels.Max(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector4 = sl => sl.Person_Id == studentId && sl.Level_Id == maxLevel;
                        StudentLevel CurrentLevel2 = base.GetModelBy(selector4);
                        CurrentLevel = CurrentLevel2;
                        if (CurrentLevel2 == null)
                        {
                            int minLevel = studentLevels.Min(p => p.Level.Id);
                            Expression<Func<STUDENT_LEVEL, bool>> selector5 = sl => sl.Person_Id == studentId && sl.Level_Id == minLevel;
                            StudentLevel CurrentLevelAlt = base.GetModelBy(selector5);
                            CurrentLevel = CurrentLevelAlt;
                        }
                    }
                    return CurrentLevel;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public StudentLevel GetBy(string MatricNumber)
        {
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.STUDENT.Matric_Number == MatricNumber;
                List<StudentLevel> studentLevels = base.GetModelsBy(selector);
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                if (studentLevels != null && studentLevels.Count > 0)
                {
                    int maxLevel = studentLevels.Max(p => p.Level.Id);
                    Expression<Func<STUDENT_LEVEL, bool>> selector2 = sl => sl.STUDENT.Matric_Number == MatricNumber && sl.Level_Id == maxLevel && sl.Session_Id == session.Id;
                    StudentLevel CurrentLevel = base.GetModelBy(selector2);
                    if (CurrentLevel == null)
                    {
                        int minLevel = studentLevels.Min(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector3 = sl => sl.STUDENT.Matric_Number == MatricNumber && sl.Level_Id == minLevel && sl.Session_Id == session.Id;
                        StudentLevel CurrentLevelAlt = base.GetModelBy(selector3);
                        CurrentLevel = CurrentLevelAlt;
                    }
                    return CurrentLevel;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public StudentLevel GetBy(Student student, Session session)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Person_Id == student.Id && sl.Session_Id == session.Id;
                return base.GetModelsBy(selector).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentLevel> GetBy(Level level, Session session)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Level_Id == level.Id && sl.Session_Id == session.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentLevel> GetBy(Level level, Programme programme, Department department, Session session)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Level_Id == level.Id && sl.Programme_Id == programme.Id && sl.Department_Id == department.Id && sl.Session_Id == session.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(StudentLevel student)
        {
            try
            {
                StudentLevel model = GetBy(student.Student.Id);
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Level_Id == model.Level.Id && sl.Person_Id == model.Student.Id;
                STUDENT_LEVEL entity = GetEntityBy(selector);
                entity.Level_Id = student.Level.Id;
                if (student.Department != null)
                {
                    entity.Department_Id = student.Department.Id;
                }
                if (student.Programme != null)
                {
                    entity.Programme_Id = student.Programme.Id;
                }
                if (student.Session != null)
                {
                    entity.Session_Id = student.Session.Id;
                }
                int modifiedRecordCount = Save();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ModifyById(StudentLevel model)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Student_Level_Id == model.Id;
                STUDENT_LEVEL entity = GetEntityBy(selector);

                if (entity == null)
                {
                    return false;
                }

                entity.Level_Id = model.Level.Id;

                if (model.Department != null)
                {
                    entity.Department_Id = model.Department.Id;
                }
                if (model.Programme != null)
                {
                    entity.Programme_Id = model.Programme.Id;
                }
                if (model.Session != null)
                {
                    entity.Session_Id = model.Session.Id;
                }

                int modifiedRecordCount = Save();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(StudentLevel student, Person person)
        {
            try
            {
                StudentLevel model = GetBy(person.Id);
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Level_Id == model.Level.Id && sl.Person_Id == model.Student.Id && sl.Session_Id == student.Session.Id;
                STUDENT_LEVEL entity = GetEntityBy(selector);

                entity.Level_Id = student.Level.Id;
                entity.Department_Id = student.Department.Id;
                if (student.DepartmentOption != null)
                {
                    entity.Department_Option_Id = student.DepartmentOption.Id;
                }
               

                int modifiedRecordCount = Save();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(StudentLevel student, long studentLevelId)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Student_Level_Id == studentLevelId && sl.Person_Id == student.Student.Id;
                STUDENT_LEVEL entity = GetEntityBy(selector);
                if (student.Level != null)
                {
                    entity.Level_Id = student.Level.Id; 
                }
                if (student.Session != null)
                {
                    entity.Session_Id = student.Session.Id;
                } 
                if (student.Department != null)
                {
                    entity.Department_Id = student.Department.Id;
                }
                if (student.DepartmentOption != null)
                {
                    entity.Department_Option_Id = student.DepartmentOption.Id;
                }
                if (student.Programme != null)
                {
                    entity.Programme_Id = student.Programme.Id;
                }
                int modifiedRecordCount = Save();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ModifyByStudentLevelId(StudentLevel student)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Student_Level_Id == student.Id;
                STUDENT_LEVEL entity = GetEntityBy(selector);
                if (student.Level != null)
                {
                    entity.Level_Id = student.Level.Id;
                }
                if (student.Session != null)
                {
                    entity.Session_Id = student.Session.Id;
                }
                if (student.Department != null)
                {
                    entity.Department_Id = student.Department.Id;
                }
                if (student.DepartmentOption != null)
                {
                    entity.Department_Option_Id = student.DepartmentOption.Id;
                }
                if (student.Programme != null)
                {
                    entity.Programme_Id = student.Programme.Id;
                }
                if (student.Student != null)
                {
                    entity.Person_Id = student.Student.Id;
                }
                int modifiedRecordCount = Save();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
      
    }




}
