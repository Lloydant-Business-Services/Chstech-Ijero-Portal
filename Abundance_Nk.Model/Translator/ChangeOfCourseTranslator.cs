using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ChangeOfCourseTranslator : TranslatorBase<ChangeOfCourse, CHANGE_OF_COURSE>
    {
        private ApplicationFormTranslator applicationFormTranslator;
        private PersonTranslator personTranslator;
        private SessionTranslator sessionTranslator;

        public ChangeOfCourseTranslator()
        {
            applicationFormTranslator = new ApplicationFormTranslator();
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override ChangeOfCourse TranslateToModel(CHANGE_OF_COURSE entity)
        {
            try
            {
                ChangeOfCourse model = null;
                if (entity != null)
                {
                    model = new ChangeOfCourse();
                    model.Id = entity.Change_Of_Course_Id;
                    model.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.JambRegistrationNumber = entity.Jamb_Registration_Number;
                    model.NewPerson = personTranslator.TranslateToModel(entity.PERSON1);
                    model.OldPerson = personTranslator.Translate(entity.PERSON);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                }

                return model;
            }
            catch (Exception)
            {   
                throw;
            }
        }

        public override CHANGE_OF_COURSE TranslateToEntity(ChangeOfCourse model)
        {
            try
            {
                CHANGE_OF_COURSE entity = null;
                if (model != null)
                {
                    entity = new CHANGE_OF_COURSE();
                    entity.Change_Of_Course_Id = model.Id;
                    entity.Jamb_Registration_Number = model.JambRegistrationNumber;
                    entity.Session_Id = model.Session.Id;
                    entity.Application_Form_Id = model.ApplicationForm.Id;
                    entity.New_Person_Id = model.NewPerson.Id;
                    entity.Old_Person_Id = model.OldPerson.Id;
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
