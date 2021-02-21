using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
   public class TranscriptStatusTranslator : TranslatorBase<TranscriptStatus, TRANSCRIPT_STATUS>
    {
        public override TranscriptStatus TranslateToModel(TRANSCRIPT_STATUS entity)
        {
            try
            {
                TranscriptStatus model = null;
                if (entity != null)
                {
                    model = new TranscriptStatus();
                    model.TranscriptStatusId = entity.Transcript_Status_Id;
                    model.TranscriptStatusName = entity.Transcript_Status_Name;
                    model.TranscriptStatusDescription = entity.Transcript_Status_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override TRANSCRIPT_STATUS TranslateToEntity(TranscriptStatus model)
        {
            try
            {
                TRANSCRIPT_STATUS entity = null;
                if (model != null)
                {
                    entity = new TRANSCRIPT_STATUS();
                    entity.Transcript_Status_Id = model.TranscriptStatusId;
                    entity.Transcript_Status_Name = model.TranscriptStatusName;
                    entity.Transcript_Status_Description = model.TranscriptStatusDescription;
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
