using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CourseTranslator : TranslatorBase<Course, COURSE>
    {
        private readonly CourseTypeTranslator courseTypeTranslator;
        private readonly DepartmentOptionTranslator departmentOptionTranslator;
        private readonly DepartmentTranslator departmentTranslator;
        private readonly LevelTranslator levelTranslator;
        private readonly ProgrammeTranslator programmeTranslator;
        private readonly SemesterTranslator semesterTranslator;

        public CourseTranslator()
        {
            levelTranslator = new LevelTranslator();
            semesterTranslator = new SemesterTranslator();
            courseTypeTranslator = new CourseTypeTranslator();
            departmentTranslator = new DepartmentTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
            programmeTranslator = new ProgrammeTranslator();
        }

        public override Course TranslateToModel(COURSE entity)
        {
            try
            {
                Course model = null;
                if (entity != null)
                {
                    model = new Course();
                    model.Id = entity.Course_Id;
                    model.Name = entity.Course_Name;
                    model.Type = courseTypeTranslator.Translate(entity.COURSE_TYPE);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                    model.Unit = entity.Course_Unit;
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.Code = entity.Course_Code;
                    model.Activated = entity.Activated;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override COURSE TranslateToEntity(Course model)
        {
            try
            {
                COURSE entity = null;
                if (model != null)
                {
                    entity = new COURSE();
                    entity.Course_Id = model.Id;
                    entity.Course_Name = model.Name;
                    entity.Course_Type_Id = model.Type.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Department_Id = model.Department.Id;
                    if (model.DepartmentOption != null && model.DepartmentOption.Id > 0)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
                    }
                    entity.Course_Unit = model.Unit;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Course_Code = model.Code;
                    entity.Programme_Id = model.Programme.Id;
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