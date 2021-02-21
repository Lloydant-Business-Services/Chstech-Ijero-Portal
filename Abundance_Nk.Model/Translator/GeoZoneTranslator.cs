using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class GeoZoneTranslator : TranslatorBase<GeoZone, GEO_ZONE>
    {
        public override GeoZone TranslateToModel(GEO_ZONE entity)
        {
            try
            {
                GeoZone model = null;
                if (entity != null)
                {
                    model = new GeoZone();
                    model.Id = entity.Geo_Zone_Id;
                    model.Activated = entity.Activated;
                    model.Description = entity.Description;
                    model.Name = entity.Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override GEO_ZONE TranslateToEntity(GeoZone model)
        {
            try
            {
                GEO_ZONE entity = null;
                if (model != null)
                {
                    entity = new GEO_ZONE();
                    entity.Activated = model.Activated;
                    entity.Description = model.Description;
                    entity.Geo_Zone_Id = model.Id;
                    entity.Name = model.Name;
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
