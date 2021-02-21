
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
    public class ProgrammeLogic : BusinessBaseLogic<Programme, PROGRAMME>
    {
        public ProgrammeLogic()
        {
            translator = new ProgrammeTranslator();
        }

        public bool Modify(Programme programme)
        {
            try
            {
                Expression<Func<PROGRAMME, bool>> selector = p => p.Programme_Id == programme.Id;
                PROGRAMME entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Programme_Name = programme.Name;
                entity.Programme_Description = programme.Description;
                entity.Activated = programme.Activated;
                entity.IsActiveForApplication = programme.IsActiveForApplication;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }




}
