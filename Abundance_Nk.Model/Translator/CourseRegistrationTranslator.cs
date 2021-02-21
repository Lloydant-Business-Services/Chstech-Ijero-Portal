using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class CourseRegistrationTranslator : TranslatorBase<CourseRegistration, STUDENT_COURSE_REGISTRATION>
    {
        private LevelTranslator levelTranslator;
        private StudentTranslator studentTranslator;
        private ProgrammeTranslator programmeTranslator;
        private DepartmentTranslator departmentTranslator;
        private SessionTranslator sessionTranslator;
        private StaffTranslator staffTranslator;
        
        public CourseRegistrationTranslator()
        {
            levelTranslator = new LevelTranslator();
            studentTranslator = new StudentTranslator();
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            sessionTranslator = new SessionTranslator();
            staffTranslator = new StaffTranslator();
        }

        public override CourseRegistration TranslateToModel(STUDENT_COURSE_REGISTRATION entity)
        {
            try
            {
                CourseRegistration model = null;
                if (entity != null)
                {
                    model = new CourseRegistration();
                    model.Id = entity.Student_Course_Registration_Id;
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Approved = entity.Approved;
                    model.Approver = staffTranslator.Translate(entity.STAFF);
                    model.DateApproved = entity.Date_Approved;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_COURSE_REGISTRATION TranslateToEntity(CourseRegistration model)
        {
            try
            {
                STUDENT_COURSE_REGISTRATION entity = null;
                if (model != null)
                {
                    entity = new STUDENT_COURSE_REGISTRATION();
                    entity.Student_Course_Registration_Id = model.Id;
                    entity.Person_Id = model.Student.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Approved = model.Approved;
                    entity.Date_Approved = model.DateApproved;

                    if (model.Approver != null && model.Approver.Id > 0)
                    {
                        entity.Approver_Id = model.Approver.Id;
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
