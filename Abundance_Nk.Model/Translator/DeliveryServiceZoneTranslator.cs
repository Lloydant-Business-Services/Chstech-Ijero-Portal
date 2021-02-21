using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class DeliveryServiceZoneTranslator : TranslatorBase<DeliveryServiceZone, DELIVERY_SERVICE_ZONE>
    {
        private DeliveryServiceTranslator deliveryServiceTranslator;
        private GeoZoneTranslator geoZoneTranslator;
        private FeeTranslator feeTranslator;
        private CountryTranslator countryTranslator;

        public DeliveryServiceZoneTranslator()
        {
            deliveryServiceTranslator = new DeliveryServiceTranslator();
            geoZoneTranslator = new GeoZoneTranslator();
            feeTranslator = new FeeTranslator();
            countryTranslator = new CountryTranslator();

        }

        public override DeliveryServiceZone TranslateToModel(DELIVERY_SERVICE_ZONE entity)
        {
            try
            {
                DeliveryServiceZone model = null;
                if (entity != null)
                {
                    model = new DeliveryServiceZone();
                    model.Id = entity.Delivery_Service_Zone_Id;
                    model.Activated = entity.Activated;
                    model.Country = countryTranslator.TranslateToModel(entity.COUNTRY);
                    model.DeliveryService = deliveryServiceTranslator.Translate(entity.DELIVERY_SERVICE);
                    model.Fee = feeTranslator.Translate(entity.FEE);
                    model.GeoZone = geoZoneTranslator.Translate(entity.GEO_ZONE);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override DELIVERY_SERVICE_ZONE TranslateToEntity(DeliveryServiceZone model)
        {
            try
            {
                DELIVERY_SERVICE_ZONE entity = null;
                if (model != null)
                {
                    entity = new DELIVERY_SERVICE_ZONE();
                    entity.Activated = model.Activated;
                    entity.Country_Id = model.Country.Id;
                    entity.Delivery_Service_Zone_Id = model.Id;
                    entity.Delivery_Service_Id = model.DeliveryService.Id;
                    entity.Fee_Id = model.Fee.Id;
                    entity.Geo_Zone_Id = model.GeoZone.Id;
                    
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
