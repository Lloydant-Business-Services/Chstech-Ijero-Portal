using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class HostelRoomCornerTranslator:TranslatorBase<HostelRoomCorner,HOSTEL_ROOM_CORNER>
    {
        HostelRoomTranslator roomTranslator = new HostelRoomTranslator();
        public override HostelRoomCorner TranslateToModel(HOSTEL_ROOM_CORNER entity)
        {
            HostelRoomCorner model = null;

            if (entity != null)
            {
                model = new HostelRoomCorner();
                model.Id = entity.Corner_Id;
                model.Name = entity.Corner_Name;
                model.Room = roomTranslator.Translate(entity.HOSTEL_ROOM);
                model.Activated = entity.Activated;
            }
            return model;
        }

        public override HOSTEL_ROOM_CORNER TranslateToEntity(HostelRoomCorner model)
        {
            HOSTEL_ROOM_CORNER entity = null;

            if (model != null)
            {
                entity = new HOSTEL_ROOM_CORNER();
                entity.Corner_Id = model.Id;
                entity.Corner_Name = model.Name;
                entity.Room_Id = model.Room.Id;
                entity.Activated = model.Activated;

            }
            return entity;
        }
    }
}
