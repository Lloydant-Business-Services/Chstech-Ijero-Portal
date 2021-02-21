using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class HostelSeriesTranslator:TranslatorBase<HostelSeries,HOSTEL_SERIES>
    {
        HostelTranslator hostelTranslator = new HostelTranslator();
        public override HostelSeries TranslateToModel(HOSTEL_SERIES entity)
        {
            HostelSeries model = null;
            if (entity != null)
            {
                model = new HostelSeries();
                model.Id = entity.Series_Id;
                model.Name = entity.Series_Name;
                model.Hostel = hostelTranslator.Translate(entity.HOSTEL);
                model.Activated = entity.Activated;
               
            }
            return model;
        }

        public override HOSTEL_SERIES TranslateToEntity(HostelSeries model)
        {
            HOSTEL_SERIES entity = null;
            if (model != null)
            {
                entity = new HOSTEL_SERIES();
                entity.Series_Id = model.Id;
                entity.Series_Name = model.Name;
                entity.Hostel_Id = model.Hostel.Id;
                entity.Activated = model.Activated;
            }
            return entity;
        }
    }
}
