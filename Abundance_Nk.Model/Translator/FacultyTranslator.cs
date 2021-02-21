
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class FacultyTranslator : TranslatorBase<Faculty, FACULTY>
    {
        public override Faculty TranslateToModel(FACULTY entity)
        {
            try
            {
                Faculty faculty = null;
                if (entity != null)
                {
                    faculty = new Faculty();
                    faculty.Id = entity.Faculty_Id;
                    faculty.Name = entity.Faculty_Name;
                    faculty.Description = entity.Faculty_Description;
                }

                return faculty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override FACULTY TranslateToEntity(Faculty faculty)
        {
            try
            {
                FACULTY entity = null;
                if (faculty != null)
                {
                    entity = new FACULTY();
                    entity.Faculty_Id = faculty.Id;
                    entity.Faculty_Name = faculty.Name;
                    entity.Faculty_Description = faculty.Description;
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
