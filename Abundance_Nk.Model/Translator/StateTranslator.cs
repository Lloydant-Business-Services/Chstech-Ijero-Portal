
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
   public class StateTranslator : TranslatorBase<State, STATE>
    {
       private NationalityTranslator nationalityTranslator;

       public StateTranslator()
       {
           nationalityTranslator = new NationalityTranslator();
       }

       public override State TranslateToModel(STATE stateEntity)
        {
            try
            {
                State state = null;
                if (stateEntity != null)
                {
                    state = new State();
                    state.Id = stateEntity.State_Id;
                    state.Name = stateEntity.State_Name;
                    state.Nationality = nationalityTranslator.TranslateToModel(stateEntity.NATIONALITY);
                }

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STATE TranslateToEntity(State state)
        {
             try
            {
                STATE stateEntity = null;
                if (state != null)
                {
                     stateEntity = new STATE();
                     stateEntity.State_Id = state.Id;
                     stateEntity.State_Name = state.Name;
                     stateEntity.Nationality_Id = state.Nationality.Id;
                }

                return stateEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
