using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class HostelAllocationTranslator:TranslatorBase<HostelAllocation,HOSTEL_ALLOCATION>
    {
        HostelTranslator hostelTranslator = new HostelTranslator();
        HostelRoomCornerTranslator cornerTranslator = new HostelRoomCornerTranslator();
        HostelRoomTranslator roomTranslator = new HostelRoomTranslator();
        HostelSeriesTranslator seriesTranslator = new HostelSeriesTranslator();
        SessionTranslator sessionTranslator = new SessionTranslator();
        StudentTranslator studentTranslator = new StudentTranslator();
        PaymentTranslator paymentTranslator = new PaymentTranslator();
        PersonTranslator personTranslator = new PersonTranslator();
        public override HostelAllocation TranslateToModel(HOSTEL_ALLOCATION entity)
        {
            HostelAllocation model = null;
            if (entity != null)
            {
                model = new HostelAllocation();
                model.Id = entity.Id;
                model.Hostel = hostelTranslator.Translate(entity.HOSTEL);
                model.Corner = cornerTranslator.Translate(entity.HOSTEL_ROOM_CORNER);
                model.Room = roomTranslator.Translate(entity.HOSTEL_ROOM);
                model.Series = seriesTranslator.Translate(entity.HOSTEL_SERIES);
                model.Session = sessionTranslator.Translate(entity.SESSION);
                
                if (entity.PERSON != null)
                {
                    model.Person = personTranslator.Translate(entity.PERSON);
                }
                
                model.Occupied = entity.Occupied;
                model.Payment = paymentTranslator.Translate(entity.PAYMENT);
            }
            return model;
        }

        public override HOSTEL_ALLOCATION TranslateToEntity(HostelAllocation model)
        {
            HOSTEL_ALLOCATION entity = null;
            if (model != null)
            {
                entity = new HOSTEL_ALLOCATION();
                entity.Id = model.Id;
                entity.Hostel_Id = model.Hostel.Id;
                entity.Corner_Id = model.Corner.Id;
                entity.Room_Id = model.Room.Id;
                entity.Series_Id = model.Series.Id;
                entity.Session_Id = model.Session.Id;
                if (model.Student != null)
                {
                    entity.Student_Id = model.Student.Id; 
                }
                if (model.Person != null)
                {
                    entity.Student_Id = model.Person.Id;
                }
                
                entity.Occupied = model.Occupied;
                entity.Payment_Id = model.Payment.Id;
            }
            return entity;
        }
    }
}
