
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class FacultyLogic : BusinessBaseLogic<Faculty, FACULTY>
    {
        public FacultyLogic()
        {
            translator = new FacultyTranslator();
        }

        public bool Modify(Faculty faculty)
        {
            try
            {
                Expression<Func<FACULTY, bool>> selector = f => f.Faculty_Id == faculty.Id;
                FACULTY entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Faculty_Name = faculty.Name;
                entity.Faculty_Description = faculty.Description;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }


        //protected override FACULTY ModifyHelper(Faculty faculty)
        //{
        //    try
        //    {
        //        Expression<Func<FACULTY, bool>> selector = f => f.Faculty_Id == faculty.Id;
        //        FACULTY entity = GetEntityBy(selector);

        //        entity.Faculty_Name = faculty.Name;
        //        entity.Faculty_Description = faculty.Description;

        //        return entity;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

       




    }




}
