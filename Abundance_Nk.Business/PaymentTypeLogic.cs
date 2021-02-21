
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
    public class PaymentTypeLogic : BusinessBaseLogic<PaymentType, PAYMENT_TYPE>
    {
        public PaymentTypeLogic()
        {
            translator = new PaymentTypeTranslator();
        }

        public bool Modify(PaymentType paymentType)
        {
            try
            {
                Expression<Func<PAYMENT_TYPE, bool>> selector = p => p.Payment_Type_Id == paymentType.Id;
                PAYMENT_TYPE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Payment_Type_Name = paymentType.Name;
                entity.Payment_Type_Description = paymentType.Description;

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



    }





}
