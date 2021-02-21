using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentResultStatusTranslator:TranslatorBase<StudentResultStatus,STUDENT_RESULT_STATUS>
    {
        private DepartmentTranslator departmentTranslator;
        private ProgrammeTranslator programmeTranslator;
        private LevelTranslator levelTranslator;

        public StudentResultStatusTranslator()
        {
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            levelTranslator = new LevelTranslator();
        }
        public override StudentResultStatus TranslateToModel(STUDENT_RESULT_STATUS entity)
        {
            try
            {
                StudentResultStatus model = null;
                if (entity != null)
                {
                    model = new StudentResultStatus();
                    model.Id = entity.Id;
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Activated = entity.Activated;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_RESULT_STATUS TranslateToEntity(StudentResultStatus model)
        {
            try
            {
                STUDENT_RESULT_STATUS entity = null;
                if (model != null)
                {
                    entity = new STUDENT_RESULT_STATUS();
                    entity.Id = model.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Activated = model.Activated;
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
