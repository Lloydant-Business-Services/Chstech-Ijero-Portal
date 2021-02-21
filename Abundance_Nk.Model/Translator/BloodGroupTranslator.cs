using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class BloodGroupTranslator : TranslatorBase<BloodGroup, BLOOD_GROUP>
    {
        public override BloodGroup TranslateToModel(BLOOD_GROUP entity)
        {
            try
            {
                BloodGroup model = null;
                if (entity != null)
                {
                    model = new BloodGroup();
                    model.Id = entity.Blood_Group_Id;
                    model.Name = entity.Blood_Group_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override BLOOD_GROUP TranslateToEntity(BloodGroup model)
        {
            try
            {
                BLOOD_GROUP entity = null;
                if (model != null)
                {
                    entity = new BLOOD_GROUP();
                    entity.Blood_Group_Id = model.Id;
                    entity.Blood_Group_Name = model.Name;
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
