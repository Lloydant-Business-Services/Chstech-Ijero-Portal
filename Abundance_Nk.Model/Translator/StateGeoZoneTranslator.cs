using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StateGeoZoneTranslator : TranslatorBase<StateGeoZone, STATE_GEO_ZONE>
    {
        private GeoZoneTranslator geoZoneTranslator;
        private StateTranslator stateTranslator;

        public StateGeoZoneTranslator()
        {
            geoZoneTranslator = new GeoZoneTranslator();
            stateTranslator = new StateTranslator();
        }

        public override StateGeoZone TranslateToModel(STATE_GEO_ZONE entity)
        {
            try
            {
                StateGeoZone model = null;
                if (entity != null)
                {
                    model = new StateGeoZone();
                    model.Id = entity.State_Geo_Zone_Id;
                    model.Activated = entity.Activated;
                    model.GeoZone = geoZoneTranslator.Translate(entity.GEO_ZONE);
                    model.State = stateTranslator.Translate(entity.STATE);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STATE_GEO_ZONE TranslateToEntity(StateGeoZone model)
        {
            try
            {
                STATE_GEO_ZONE entity = null;
                if (model != null)
                {
                    entity = new STATE_GEO_ZONE();
                    entity.Activated = model.Activated;
                    entity.Geo_Zone_Id = model.GeoZone.Id;
                    entity.State_Geo_Zone_Id = model.Id;
                    entity.State_Id = model.State.Id;
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
