using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class HostelFeeTranslator : TranslatorBase<HostelFee, HOSTEL_FEE>
    {
        private PaymentTranslator paymentTranslator;
        private HostelTranslator hostelTranslator;

        public HostelFeeTranslator()
        {
            paymentTranslator = new PaymentTranslator();
            hostelTranslator = new HostelTranslator();
        }

        public override HostelFee TranslateToModel(HOSTEL_FEE entity)
        {
            try
            {
                HostelFee model = null;
                if (entity != null)
                {
                    model = new HostelFee();
                    model.Id = entity.Hostel_Fee_Id;
                    model.Amount = entity.Amount;
                    model.Hostel = hostelTranslator.Translate(entity.HOSTEL);
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                }

                return model;
            }
            catch (Exception)
            {   
                throw;
            }
        }

        public override HOSTEL_FEE TranslateToEntity(HostelFee model)
        {
            try
            {
                HOSTEL_FEE entity = null;
                if (model != null)
                {
                    entity = new HOSTEL_FEE();
                    entity.Hostel_Fee_Id = model.Id;
                    entity.Hostel_Id = model.Hostel.Id;
                    entity.Payment_Id = model.Payment.Id;
                    entity.Amount = model.Amount;
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
