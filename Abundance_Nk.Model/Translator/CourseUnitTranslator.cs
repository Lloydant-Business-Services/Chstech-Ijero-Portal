using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class CourseUnitTranslator : TranslatorBase<CourseUnit, COURSE_UNIT>
    {
        private LevelTranslator levelTranslator;
        private DepartmentTranslator departmentTranslator;
        private SemesterTranslator semesterTranslator;
        public DepartmentOptionTranslator departmentOptionTranslator;
        public SessionTranslator SessionTranslator;
        public ProgrammeTranslator ProgrammeTranslator;

        public CourseUnitTranslator()
        {
            levelTranslator = new LevelTranslator();
            departmentTranslator = new DepartmentTranslator();
            semesterTranslator = new SemesterTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
            SessionTranslator = new SessionTranslator();
            ProgrammeTranslator = new ProgrammeTranslator();
        }

        public override CourseUnit TranslateToModel(COURSE_UNIT entity)
        {
            try
            {
                CourseUnit model = null;
                if (entity != null)
                {
                    model = new CourseUnit();
                    model.Id = entity.Course_Unit_Id;
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.MinimumUnit = entity.Minimum_Unit;
                    model.MaximumUnit = entity.Maximum_Unit;
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                    model.Session = SessionTranslator.Translate(entity.SESSION);
                    model.Programme = ProgrammeTranslator.Translate(entity.PROGRAMME);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override COURSE_UNIT TranslateToEntity(CourseUnit model)
        {
            try
            {
                COURSE_UNIT entity = null;
                if (model != null)
                {
                    entity = new COURSE_UNIT();
                    entity.Course_Unit_Id = model.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Minimum_Unit = model.MinimumUnit;
                    entity.Maximum_Unit = model.MaximumUnit;
                    if (model.DepartmentOption != null)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
                    }
                    if (model.Session != null)
                    {
                        entity.Session_Id = model.Session.Id;
                    }
                    if (model.Programme != null)
                    {
                        entity.Programme_Id = model.Programme.Id;
                    }
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
