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
    public class ScratchCardLogic : BusinessBaseLogic<ScratchCard, SCRATCH_CARD>
    {
        public ScratchCardLogic()
        {
            base.translator = new ScratchCardTranslator();
        }

        public ScratchCard GetBy(string pin)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector = s => s.Pin == pin;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ValidatePin(string pin, FeeType fee_type)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector = s => s.Pin == pin && s.SCRATCH_CARD_BATCH.SCRATCH_CARD_TYPE.Fee_Type_Id == fee_type.Id ;
                List<ScratchCard> scratchCardPayments = GetModelsBy(selector);
                if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsPinUsed(string pin, long personId)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector = s => s.Pin == pin && s.Person_Id != null;
                List<ScratchCard> scratchCardPayments = GetModelsBy(selector);
                if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                {
                     Expression<Func<SCRATCH_CARD, bool>> expression = s => s.Pin == pin && s.Person_Id == personId;
                     scratchCardPayments = GetModelsBy(expression);
                     if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                     {
                         return false;
                     }
                     return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public bool UpdatePin(string pin, Person person)
        {
            try
            {

                Expression<Func<SCRATCH_CARD, bool>> selector = p => p.Pin == pin;
                SCRATCH_CARD scratchCardEntity = GetEntityBy(selector);

                if (scratchCardEntity == null || scratchCardEntity.Scratch_Card_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                if (scratchCardEntity.First_Used_Date == null)
                {
                    scratchCardEntity.First_Used_Date = DateTime.Now;
                }
                if (scratchCardEntity.Person_Id == null)
                {
                    scratchCardEntity.Person_Id = person.Id;
                }

                scratchCardEntity.Usage_Count = scratchCardEntity.Usage_Count ++;
                

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
