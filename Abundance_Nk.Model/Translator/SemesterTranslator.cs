using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class SemesterTranslator : TranslatorBase<Semester, SEMESTER>
    {
        public override Semester TranslateToModel(SEMESTER entity)
        {
            try
            {
                Semester semester = null;
                if (entity != null)
                {
                    semester = new Semester();
                    semester.Id = entity.Semester_Id;
                    semester.Name = entity.Semester_Name;
                }

                return semester;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SEMESTER TranslateToEntity(Semester semester)
        {
            try
            {
                SEMESTER entity = null;
                if (semester != null)
                {
                    entity = new SEMESTER();
                    entity.Semester_Id = semester.Id;
                    entity.Semester_Name = semester.Name;
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
