using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class HostelAllocationCriteriaTranslator : TranslatorBase<HostelAllocationCriteria, HOSTEL_ALLOCATION_CRITERIA>
    {
        HostelTranslator hostelTranslator = new HostelTranslator();
        HostelRoomCornerTranslator cornerTranslator = new HostelRoomCornerTranslator();
        HostelRoomTranslator roomTranslator = new HostelRoomTranslator();
        HostelSeriesTranslator seriesTranslator = new HostelSeriesTranslator();
        LevelTranslator levelTranslator = new LevelTranslator();

        public override HostelAllocationCriteria TranslateToModel(HOSTEL_ALLOCATION_CRITERIA entity)
        {
            HostelAllocationCriteria model = null;
            if (entity != null)
            {
                model = new HostelAllocationCriteria();
                model.Id = entity.Id;
                model.Hostel = hostelTranslator.Translate(entity.HOSTEL);
                model.Corner = cornerTranslator.Translate(entity.HOSTEL_ROOM_CORNER);
                model.Room = roomTranslator.Translate(entity.HOSTEL_ROOM);
                model.Series = seriesTranslator.Translate(entity.HOSTEL_SERIES);
                model.Level = levelTranslator.Translate(entity.LEVEL);

            }
            return model;
        }

        public override HOSTEL_ALLOCATION_CRITERIA TranslateToEntity(HostelAllocationCriteria model)
        {
            HOSTEL_ALLOCATION_CRITERIA entity = null;

            if (model != null)
            {
                entity = new HOSTEL_ALLOCATION_CRITERIA();
                entity.Id = model.Id;
                entity.Hostel_Id = model.Hostel.Id;
                entity.Corner_Id = model.Corner.Id;
                entity.Room_Id = model.Room.Id;
                entity.Series_Id = model.Series.Id;
                entity.Level_Id = model.Level.Id;
            }
            return entity;
        }
    }
}
