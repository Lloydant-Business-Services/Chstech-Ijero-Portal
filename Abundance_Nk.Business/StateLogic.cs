using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;


namespace Abundance_Nk.Business
{
    public class StateLogic : BusinessBaseLogic<State, STATE>
    {
        public StateLogic()
        {
            base.translator = new StateTranslator();
        }

        public bool Modify(State state)
        {
            try
            {
                Expression<Func<STATE, bool>> selector = s => s.State_Id == state.Id;
                STATE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.State_Name = state.Name;
                entity.Nationality_Id = state.Nationality.Id;

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


        //public List<State> GetBy(int nationalityId)
        //{
        //    try
        //    {
        //        Func<STATE, bool> selector = s => s.Nationality_Id == nationalityId;
        //        return GetModelsBy(selector);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}





    }
}
