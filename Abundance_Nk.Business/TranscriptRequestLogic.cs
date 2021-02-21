using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Web.ModelBinding;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class TranscriptRequestLogic : BusinessBaseLogic<TranscriptRequest,TRANSCRIPT_REQUEST>
    {
        public TranscriptRequestLogic()
        {
            translator = new TranscriptRequestTranslator();
        }

        public TranscriptRequest GetBy(long Id)
        {
            TranscriptRequest request = null;
            try
            {
                request = GetModelBy(a => a.Student_id == Id && a.Transcript_Status_Id < 5);
            }
            catch (Exception)
            {
                
                throw;
            }
            return request;
        }

        public List<TranscriptRequest> GetBy(Student student)
        {
            List<TranscriptRequest> request = null;
            try
            {
                request = GetModelsBy(a => a.Student_id == student.Id);
            }
            catch (Exception)
            {

                throw;
            }
            return request;
        }

        public bool Modify (TranscriptRequest model)
        {
            try
            {
                Expression<Func<TRANSCRIPT_REQUEST, bool>> selector = af => af.Transcript_Request_Id == model.Id;
                TRANSCRIPT_REQUEST entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.payment != null)
                {
                    entity.Payment_Id = model.payment.Id;
                }

                entity.Destination_Address = model.DestinationAddress;

                if (model.DestinationState != null && !string.IsNullOrEmpty(model.DestinationState.Id))
                {
                    entity.Destination_State_Id = model.DestinationState.Id;
                }

                if (model.DestinationCountry != null && !string.IsNullOrEmpty(model.DestinationCountry.Id))
                {
                    entity.Destination_Country_Id = model.DestinationCountry.Id;
                }
                
                entity.Transcript_clearance_Status_Id = model.transcriptClearanceStatus.TranscriptClearanceStatusId;
                entity.Transcript_Status_Id = model.transcriptStatus.TranscriptStatusId;

                if (model.DeliveryServiceZone != null)
                {
                    entity.Delivery_Service_Zone_Id = model.DeliveryServiceZone.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
  
    }
}
