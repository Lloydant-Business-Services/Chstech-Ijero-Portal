using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class HostelRoomTranslator:TranslatorBase<HostelRoom,HOSTEL_ROOM>
    {
        HostelSeriesTranslator seriesTranslator = new HostelSeriesTranslator();
        private HostelTranslator hostelTranslator = new HostelTranslator();
        public override HostelRoom TranslateToModel(HOSTEL_ROOM entity)
        {
            HostelRoom model = null;
            if (entity != null)
            {
                model = new HostelRoom();
                model.Id = entity.Room_Id;
                model.Number = entity.Room_Number;
                model.Series = seriesTranslator.Translate(entity.HOSTEL_SERIES);
                model.Hostel = hostelTranslator.Translate(entity.HOSTEL);
                model.Activated = entity.Activated;
                model.Reserved = entity.Reserved;
            }
            return model;
        }

        public override HOSTEL_ROOM TranslateToEntity(HostelRoom model)
        {
            HOSTEL_ROOM entity = null;
            if (model != null)
            {
                entity = new HOSTEL_ROOM();
                entity.Room_Id = model.Id;
                entity.Room_Number = model.Number;
                entity.Series_Id = model.Series.Id;
                entity.Activated = model.Activated;
                entity.Hostel_Id = model.Hostel.Id;
                entity.Reserved = model.Reserved;
            }
            return entity;
        }
    }
}
