using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class RemitaSettingsTranslator: TranslatorBase<RemitaSettings,REMITA_PAYMENT_SETTINGS>
    {
        public override RemitaSettings TranslateToModel(REMITA_PAYMENT_SETTINGS entity)
        {
            try
            {
                RemitaSettings model = null;
                if (entity != null)
                {
                    model = new RemitaSettings();
                    model.Api_key = entity.Api_Key;
                    model.MarchantId = entity.MarchantId;
                    model.Payment_SettingId = entity.Payment_SettingId;
                    model.Response_Url = entity.Response_Url;
                    model.serviceTypeId = entity.serviceTypeId;
                    model.Description = entity.Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override REMITA_PAYMENT_SETTINGS TranslateToEntity(RemitaSettings model)
        {
            try
            {
                REMITA_PAYMENT_SETTINGS entity = null;
                if (model != null)
                {
                    entity = new REMITA_PAYMENT_SETTINGS();
                    entity.Api_Key = model.Api_key;
                    entity.MarchantId = model.MarchantId;
                    entity.Payment_SettingId = model.Payment_SettingId;
                    entity.Response_Url = model.Response_Url;
                    entity.serviceTypeId = model.serviceTypeId;
                    entity.Description = model.Description;
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
