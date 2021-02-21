using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;


namespace Abundance_Nk.Model.Translator
{
    public class StaffTypeTranslator : TranslatorBase<StaffType, STAFF_TYPE>
    {
        public override StaffType TranslateToModel(STAFF_TYPE entity)
        {
            try
            {
                StaffType model = null;
                if (entity != null)
                {
                    model = new StaffType();
                    model.Id = entity.Staff_Type_Id;
                    model.Name = entity.Staff_Type_Name;
                    model.Description = entity.Staff_Type_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STAFF_TYPE TranslateToEntity(StaffType model)
        {
            try
            {
                STAFF_TYPE entity = null;
                if (model != null)
                {
                    entity = new STAFF_TYPE();
                    entity.Staff_Type_Id = model.Id;
                    entity.Staff_Type_Name = model.Name;
                    entity.Staff_Type_Description = model.Description;
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
