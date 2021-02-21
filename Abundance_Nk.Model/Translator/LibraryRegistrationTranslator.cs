using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class LibraryRegistrationTranslator: TranslatorBase<LibraryRegistration, LIBRARY_REGISTRATION>
    {
        private PersonTranslator PersonTranslator;
        private UserTranslator UserTranslator;
        private SessionTranslator SessionTranslator;

        public LibraryRegistrationTranslator()
        {
            PersonTranslator = new PersonTranslator();
            UserTranslator = new UserTranslator();
            SessionTranslator = new SessionTranslator();
        }

        public override LIBRARY_REGISTRATION TranslateToEntity(LibraryRegistration model)
        {
            try
            {
                LIBRARY_REGISTRATION entity = null;
                if (model != null)
                {
                    entity = new LIBRARY_REGISTRATION();
                    entity.Date = model.Date;
                    entity.Guardian_Name = model.Guardian_Name;
                    entity.Id = model.Id;
                    entity.Person_Id = model.Person.Id;
                    entity.User_Id = model.User.Id;
                    entity.Previous_School_Name = model.Previous_School_Name;
                    if (entity.Session_Id > 0)
                    {
                        entity.Session_Id = model.Session_Id;
                    }
                }

                return entity;
            }
            catch(Exception ex) { throw ex; }
        }

        public override LibraryRegistration TranslateToModel(LIBRARY_REGISTRATION entity)
        {
            try
            {
                LibraryRegistration model = null;
                if (entity != null)
                {
                    model = new LibraryRegistration();
                    model.Id = entity.Id;
                    model.Guardian_Name = entity.Guardian_Name;
                    model.Previous_School_Name = entity.Previous_School_Name;
                    model.Person = PersonTranslator.Translate(entity.PERSON);
                    model.User = UserTranslator.Translate(entity.USER);
                    model.Date = entity.Date;
                    if (entity.SESSION != null)
                    {
                        model.Session = SessionTranslator.Translate(entity.SESSION);
                    }
                }

                return model;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
