using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class TranscriptRequestTranslator : TranslatorBase<TranscriptRequest, TRANSCRIPT_REQUEST>
    {
        private StudentTranslator studentTranslator;
        private PaymentTranslator paymentTranslator;
        private CountryTranslator countryTranslator;
        private StateTranslator stateTranslator;
        private TranscriptClearanceStatusTranslator transcriptClearanceStatusTranslator;
        private TranscriptStatusTranslator transcriptStatusTranslator;
        private DeliveryServiceZoneTranslator deliveryServiceZoneTranslator;
        public TranscriptRequestTranslator()
        {
            studentTranslator = new StudentTranslator();
            paymentTranslator = new PaymentTranslator();
            countryTranslator = new CountryTranslator();
            stateTranslator = new StateTranslator();
            transcriptClearanceStatusTranslator = new TranscriptClearanceStatusTranslator();
            transcriptStatusTranslator = new TranscriptStatusTranslator();
            deliveryServiceZoneTranslator = new DeliveryServiceZoneTranslator();
        }

        public override TranscriptRequest TranslateToModel(TRANSCRIPT_REQUEST entity)
        {
            try
            {
                TranscriptRequest model = null;
                if (entity != null)
                {
                    model = new TranscriptRequest();
                    model.Id = entity.Transcript_Request_Id;
                    if (entity.PAYMENT != null)
                    {
                        model.payment = paymentTranslator.Translate(entity.PAYMENT);
                    }
                    model.student = studentTranslator.Translate(entity.STUDENT);
                    model.DateRequested = entity.Date_Requested;
                    model.DestinationAddress = entity.Destination_Address;
                    model.DestinationCountry = countryTranslator.Translate(entity.COUNTRY);
                    model.DestinationState = stateTranslator.Translate(entity.STATE);
                    model.transcriptClearanceStatus = transcriptClearanceStatusTranslator.Translate(entity.TRANSCRIPT_CLEARANCE_STATUS);
                    model.transcriptStatus = transcriptStatusTranslator.Translate(entity.TRANSCRIPT_STATUS);
                    model.RequestType = entity.Request_Type;
                    model.DeliveryServiceZone = deliveryServiceZoneTranslator.Translate(entity.DELIVERY_SERVICE_ZONE);
                    model.Reciever = entity.Reciever;
                    model.StatementOfResult = entity.StatementOfResultUrl;
                    model.Processed = entity.Processed;
                    model.Verified = entity.Verified;
                    model.Passport = entity.Passport_Url;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override TRANSCRIPT_REQUEST TranslateToEntity(TranscriptRequest model)
        {
            try
            {
                TRANSCRIPT_REQUEST entity = null;
                if (model != null)
                {
                    entity = new TRANSCRIPT_REQUEST();
                    entity.Transcript_Request_Id = model.Id;
                    if (model.payment!= null)
                    {
                        entity.Payment_Id = model.payment.Id;
                    }
                    
                    entity.Student_id = model.student.Id;
                    entity.Date_Requested = model.DateRequested;
                    entity.Destination_Address = model.DestinationAddress;
                    if (model.DestinationCountry != null && !string.IsNullOrEmpty(model.DestinationCountry.Id))
                    {
                        entity.Destination_Country_Id = model.DestinationCountry.Id;
                    }
                    if (model.DestinationState != null && !string.IsNullOrEmpty(model.DestinationState.Id))
                    {
                        entity.Destination_State_Id = model.DestinationState.Id;
                    }
                    
                    entity.Request_Type = model.RequestType;
                    entity.Transcript_clearance_Status_Id = model.transcriptClearanceStatus.TranscriptClearanceStatusId;
                    entity.Transcript_Status_Id = model.transcriptStatus.TranscriptStatusId;
                    if (model.DeliveryServiceZone != null)
                    {
                        entity.Delivery_Service_Zone_Id = model.DeliveryServiceZone.Id;
                    }
                    if (model.Reciever != null)
                    {
                        entity.Reciever = model.Reciever;
                    }
                    if (model.StatementOfResult != null)
                    {
                        entity.StatementOfResultUrl = model.StatementOfResult;
                    }
                    if (model.Passport != null)
                    {
                        entity.Passport_Url = model.Passport;
                    }
                    entity.Processed = model.Processed;
                    entity.Verified = model.Verified;


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
