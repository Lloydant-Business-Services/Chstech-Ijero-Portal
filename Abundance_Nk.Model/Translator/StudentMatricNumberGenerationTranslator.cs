using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class StudentMatricNumberGenerationAuditTranslator : TranslatorBase<StudentMatricNumberGenerationAudit, STUDENT_MATRIC_NUMBER_GENERATION_AUDIT>
    {
        private UserTranslator userTranslator;
        private ProgrammeTranslator programmeTranslator;
        private DepartmentTranslator departmentTranslator;
        private SessionTranslator sessionTranslator;

        public StudentMatricNumberGenerationAuditTranslator()
        {
            userTranslator = new UserTranslator();
            programmeTranslator = new ProgrammeTranslator();
            departmentTranslator = new DepartmentTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override StudentMatricNumberGenerationAudit TranslateToModel(STUDENT_MATRIC_NUMBER_GENERATION_AUDIT entity)
        {
            try
            {
                StudentMatricNumberGenerationAudit model = null;
                if (entity != null)
                {
                    model = new StudentMatricNumberGenerationAudit();
                    model.User = userTranslator.Translate(entity.USER);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Id = entity.Id;
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.EndingMatricNumber = entity.Ending_Matric_Number;
                    model.NumberLength = entity.Number_Length;
                    model.Operation = entity.Operation;
                    model.Prefix = entity.Prefix;
                    model.StartNumber = entity.Start_Number;
                    model.StartingMatricNumber = entity.Starting_Matric_Number;
                    model.Suffix = entity.Suffix;
                    model.Time = entity.Time;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_MATRIC_NUMBER_GENERATION_AUDIT TranslateToEntity(StudentMatricNumberGenerationAudit model)
        {
            try
            {
                STUDENT_MATRIC_NUMBER_GENERATION_AUDIT entity = null;
                if (model != null)
                {
                    entity = new STUDENT_MATRIC_NUMBER_GENERATION_AUDIT();
                    if (model.Programme != null)
                    {
                        entity.Programme_Id = model.Programme.Id; 
                    }
                    if (model.Department != null)
                    {
                        entity.Department_Id = model.Department.Id;
                    }
                    if (model.Session != null)
                    {
                        entity.Session_Id = model.Session.Id;
                    }
                    
                    entity.User_Id = model.User.Id;
                    entity.Id = model.Id;
                    entity.Prefix = model.Prefix;
                    entity.Suffix = model.Suffix;
                    entity.Starting_Matric_Number = model.StartingMatricNumber;
                    entity.Ending_Matric_Number = model.EndingMatricNumber;
                    entity.Start_Number = model.StartNumber;
                    entity.Number_Length = model.NumberLength;
                    entity.Action = model.Action;
                    entity.Operation = model.Operation;
                    entity.Time = model.Time;
                    entity.Client = model.Client;
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
