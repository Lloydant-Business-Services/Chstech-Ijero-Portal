using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class TranscriptClearanceStatusTranslator : TranslatorBase<TranscriptClearanceStatus, TRANSCRIPT_CLEARANCE_STATUS>
    {

        public override TranscriptClearanceStatus TranslateToModel(TRANSCRIPT_CLEARANCE_STATUS entity)
        {
            try
            {
                TranscriptClearanceStatus model = null;
                if (entity != null)
                {
                    model = new TranscriptClearanceStatus();
                    model.TranscriptClearanceStatusId = entity.Transcript_clearance_Status_Id;
                    model.TranscriptClearanceStatusName = entity.Transcript_clearance_Status_Name;
                    model.TranscriptClearanceStatusDescription = entity.Transcript_clearance_Status_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override TRANSCRIPT_CLEARANCE_STATUS TranslateToEntity(TranscriptClearanceStatus model)
        {
            try
            {
                TRANSCRIPT_CLEARANCE_STATUS entity = null;
                if (model != null)
                {
                    entity = new TRANSCRIPT_CLEARANCE_STATUS();
                    entity.Transcript_clearance_Status_Id = model.TranscriptClearanceStatusId;
                    entity.Transcript_clearance_Status_Name = model.TranscriptClearanceStatusName;
                    entity.Transcript_clearance_Status_Description = model.TranscriptClearanceStatusDescription;
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
