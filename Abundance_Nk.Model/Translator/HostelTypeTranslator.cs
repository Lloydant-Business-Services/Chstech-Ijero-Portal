using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class HostelTypeTranslator:TranslatorBase<HostelType,HOSTEL_TYPE>
    {
        public override HostelType TranslateToModel(HOSTEL_TYPE entity)
        {
            HostelType model = null;
            if (entity != null)
            {
                model = new HostelType();
                model.Hostel_Type_Id = entity.Hostel_Type_Id;
                model.Hostel_Type_Name = entity.Hostel_Type_Name;
                model.Hostel_Type_Description = entity.Hostel_Type_Description;
            }
            return model;
        }

        public override HOSTEL_TYPE TranslateToEntity(HostelType model)
        {
            HOSTEL_TYPE entity = null;

            if (model != null)
            {
                entity = new HOSTEL_TYPE();
                entity.Hostel_Type_Id = model.Hostel_Type_Id;
                entity.Hostel_Type_Name = model.Hostel_Type_Name;
                entity.Hostel_Type_Description = model.Hostel_Type_Description;
            }
            return entity;
        }
    }
}
