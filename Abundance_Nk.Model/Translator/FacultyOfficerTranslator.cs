using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class FacultyOfficerTranslator : TranslatorBase<FacultyOfficer, FACULTY_OFFICER>
    {
        private UserTranslator userTranslator;
        private FacultyTranslator facultyTranslator;

        public FacultyOfficerTranslator()
        {
            userTranslator = new UserTranslator();
            facultyTranslator = new FacultyTranslator();
        }

        public override FacultyOfficer TranslateToModel(FACULTY_OFFICER entity)
        {
            try
            {
                FacultyOfficer model = null;
                if (entity != null)
                {
                    model = new FacultyOfficer();
                    model.Officer = userTranslator.Translate(entity.USER);
                    model.Faculty = facultyTranslator.Translate(entity.FACULTY);
                    model.Description = entity.Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override FACULTY_OFFICER TranslateToEntity(FacultyOfficer model)
        {
            try
            {
                FACULTY_OFFICER entity = null;
                if (model != null)
                {
                    entity = new FACULTY_OFFICER();
                    entity.User_Id = model.Officer.Id;
                    entity.Faculty_Id = model.Faculty.Id;
                    entity.Description = model.Description;
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
