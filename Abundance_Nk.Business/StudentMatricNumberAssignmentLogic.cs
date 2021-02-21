using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Business
{
    public class StudentMatricNumberAssignmentLogic : BusinessBaseLogic<StudentMatricNumberAssignment, STUDENT_MATRIC_NUMBER_ASSIGNMENT>
    {
        public StudentMatricNumberAssignmentLogic()
        {
            translator = new StudentMatricNumberAssignmentTranslator();
        }

        public StudentMatricNumberAssignment GetBy(Faculty faculty, Department department,Programme programme, Level level, Session session)
        {
            try
            {
                Expression<Func<STUDENT_MATRIC_NUMBER_ASSIGNMENT, bool>> selector = s => s.Department_Id == department.Id && s.Programme_Id == programme.Id && s.Level_Id == level.Id && s.Session_Id == session.Id;
                return GetModelBy(selector);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public bool IsInvalid(string matricNo)
        {
            try
            {
                Expression<Func<STUDENT_MATRIC_NUMBER_ASSIGNMENT, bool>> selector = s => s.Matric_Number_Start_From.Contains(matricNo);
                List<StudentMatricNumberAssignment> studentMatricNos = GetModelsBy(selector);
                if (studentMatricNos == null || studentMatricNos.Count <= 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool MarkAsUsed(StudentMatricNumberAssignment studentMatricNumberAssignment)
        {
            try
            {
                Expression<Func<STUDENT_MATRIC_NUMBER_ASSIGNMENT, bool>> selector = s => s.Faculty_Id == studentMatricNumberAssignment.Faculty.Id && s.Department_Id == studentMatricNumberAssignment.Department.Id && s.Programme_Id == studentMatricNumberAssignment.Programme.Id && s.Level_Id == studentMatricNumberAssignment.Level.Id && s.Session_Id == studentMatricNumberAssignment.Session.Id;
                STUDENT_MATRIC_NUMBER_ASSIGNMENT entity = GetEntityBy(selector);
                
                entity.Used = true;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(StudentMatricNumberAssignment model)
        {
            try
            {
                Expression<Func<STUDENT_MATRIC_NUMBER_ASSIGNMENT, bool>> selector = a => a.Id == model.Id;
                STUDENT_MATRIC_NUMBER_ASSIGNMENT entity = GetEntityBy(selector);

                if (entity != null && entity.Id > 0)
                {
                    if (model.Department != null)
                    {
                        entity.Department_Id = model.Department.Id;
                    }
                    if (model.Programme != null)
                    {
                        entity.Programme_Id = model.Programme.Id;
                    }
                    if (model.Level != null)
                    {
                        entity.Level_Id = model.Level.Id;
                    }
                    if (model.Session != null)
                    {
                        entity.Session_Id = model.Session.Id;
                    }
                    if (model.Faculty != null)
                    {
                        entity.Faculty_Id = model.Faculty.Id;
                    }
                    if (model.DepartmentOptionId != null && model.DepartmentOptionId > 0)
                    {
                        entity.Department_Option_Id = model.DepartmentOptionId;
                    }
                    if (model.DepartmentOption != null && model.DepartmentOption.Id > 0)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
                    }
                    entity.Matric_Serial_Number_Start_From = model.MatricSerialNoStartFrom;
                    entity.Matric_Number_Start_From = model.MatricNoStartFrom;
                    entity.Used = model.Used;
                    entity.Department_Code = model.DepartmentCode;

                    int modifiedRecordCount = Save();

                    if (modifiedRecordCount > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentDetailsModel> GetMatricNumberList(Programme programme, Department department, Session session, DepartmentOption departmentOption)
        {
            List<StudentDetailsModel> list = new List<StudentDetailsModel>();
            List<StudentDetailsModel> masterList = new List<StudentDetailsModel>();
            try
            {
                list = (from x in  repository.GetBy<VW_MATRIC_NUMBER_GENERATION>( a => a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Session_Id == session.Id)
                        select new StudentDetailsModel
                        {
                            PersonId = x.Person_Id,
                            Name = x.Name,
                            MobilePhone = x.Mobile_Phone,
                            Email = x.Email,
                            SexName = x.Sex_Name,
                            MatricNumber = x.Matric_Number,
                            ApplicationNumber = x.Application_Form_Number,
                            ProgrammeName = x.Programme_Name,
                            DepartmentName = x.Department_Name,
                            SessionName = x.Session_Name
                        }).ToList();

                if (departmentOption != null)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    List<StudentLevel> studentLevels = studentLevelLogic.GetModelsBy(s => s.Programme_Id == programme.Id && s.Department_Id == department.Id && 
                                                                s.Department_Option_Id == departmentOption.Id && s.Session_Id == session.Id);
                    for (int i = 0; i < list.Count; i++)
                    {
                        StudentDetailsModel studentDetail = list[i];
                        if (studentLevels.Any(s => s.Student.Id == studentDetail.PersonId))
                        {
                            masterList.Add(studentDetail);
                        }
                    }
                }
                else
                {
                    masterList = list;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return masterList.OrderBy(s => s.Name).ToList();
        }
        public List<StudentDetailsModel> GetStudentList(Programme programme, Level level, Department department, DepartmentOption departmentOption, Session session)
        {
            List<StudentDetailsModel> list = new List<StudentDetailsModel>();
            try
            {
                if (departmentOption != null && departmentOption.Id > 0)
                {
                    list = (from x in repository.GetBy<VW_STUDENT_LIST>(a => a.Programme_Id == programme.Id && a.Level_Id == level.Id && a.Department_Id == department.Id && a.Department_Option_Id == departmentOption.Id && a.Session_Id == session.Id)
                            select new StudentDetailsModel
                            {
                                PersonId = x.Person_Id,
                                Name = x.Name,
                                MobilePhone = x.Mobile_Phone,
                                Email = x.Email,
                                SexName = x.Sex_Name,
                                MatricNumber = x.Matric_Number,
                                ProgrammeName = x.Programme_Name,
                                DepartmentName = x.Department_Name,
                                SessionName = x.Session_Name,
                                LevelName = x.Level_Name,
                                DepartmentOptionName = x.Department_Option_Name
                            }).ToList();
                }
                else
                {
                    list = (from x in repository.GetBy<VW_STUDENT_LIST>(a => a.Programme_Id == programme.Id && a.Level_Id == level.Id && a.Department_Id == department.Id && a.Session_Id == session.Id)
                            select new StudentDetailsModel
                            {
                                PersonId = x.Person_Id,
                                Name = x.Name,
                                MobilePhone = x.Mobile_Phone,
                                Email = x.Email,
                                SexName = x.Sex_Name,
                                MatricNumber = x.Matric_Number,
                                ProgrammeName = x.Programme_Name,
                                DepartmentName = x.Department_Name,
                                SessionName = x.Session_Name,
                                LevelName = x.Level_Name,
                                DepartmentOptionName = x.Department_Option_Name
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list.OrderBy(s => s.Name).ToList();
        } 
        public List<ExcelTemplateModel> GetStudentApplcationNoAndMatNo(Programme programme, Level level, Department department, Session session)
        {
            List<ExcelTemplateModel> list = new List<ExcelTemplateModel>();
            List<ExcelTemplateModel> masterList = new List<ExcelTemplateModel>();
            try
            {
                list = (from x in repository.GetBy<VW_MATRIC_NUMBER_GENERATION>(a => a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Session_Id == session.Id)
                        select new ExcelTemplateModel
                        {
                            FullName = x.Name,
                            ApplicationNo = x.Application_Form_Number,
                            MatricNo = x.Matric_Number
                            
                        }).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return masterList = list.OrderBy(s => s.FullName).ToList();
        }
    }
}
