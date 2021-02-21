using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentChannelTranslator : TranslatorBase<PaymentChannel, PAYMENT_CHANNEL>
    {
        public override PaymentChannel TranslateToModel(PAYMENT_CHANNEL entity)
        {
            try
            {
                PaymentChannel channel = null;
                if (channel != null)
                {
                    channel = new PaymentChannel();
                    channel.Id = entity.Payment_Channnel_Id;
                    channel.Name = entity.Payment_Channel_Name;
                    channel.Description = entity.Payment_Channel_Description;
                    channel.Status = entity.Payment_Channel_Status;
                }

                return channel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT_CHANNEL TranslateToEntity(PaymentChannel channel)
        {
            try
            {
                PAYMENT_CHANNEL entity = null;
                if (entity != null)
                {
                    entity = new PAYMENT_CHANNEL();
                    entity.Payment_Channnel_Id = channel.Id;
                    entity.Payment_Channel_Name = channel.Name;
                    entity.Payment_Channel_Description = channel.Description;
                    entity.Payment_Channel_Status = channel.Status;
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
