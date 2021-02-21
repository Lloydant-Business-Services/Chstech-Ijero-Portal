using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class HostelTranslator:TranslatorBase<Hostel,HOSTEL>
    {
        HostelTypeTranslator hostelTypeTranslator = new HostelTypeTranslator();
        
        public override Hostel TranslateToModel(HOSTEL entity)
        {
            Hostel model = null;
            if (entity != null)
            {
                model = new Hostel();
                model.Id = entity.Hostel_Id;
                model.Name = entity.Hostel_Name;
                model.HostelType = hostelTypeTranslator.Translate(entity.HOSTEL_TYPE);
                model.Capacity = entity.Capacity;
                model.Description = entity.Description;
                model.Activated = entity.Activated;
                model.DateEntered = entity.Date_Entered;

            }
            return model;
        }

        public override HOSTEL TranslateToEntity(Hostel model)
        {
            HOSTEL entity = null;
            if (model != null)
            {
                entity = new HOSTEL();
                entity.Hostel_Id = model.Id;
                entity.Hostel_Name = model.Name;
                entity.Hostel_Type_Id = model.HostelType.Hostel_Type_Id;
                entity.Capacity = model.Capacity;
                entity.Description = model.Description;
                entity.Activated = model.Activated;
                entity.Date_Entered = model.DateEntered;
            }
            return entity;
        }
    }
}
