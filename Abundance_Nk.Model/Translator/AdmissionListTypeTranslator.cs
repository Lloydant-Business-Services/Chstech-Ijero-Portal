using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class AdmissionListTypeTranslator : TranslatorBase<AdmissionListType, ADMISSION_LIST_TYPE>
    {
        public override AdmissionListType TranslateToModel(ADMISSION_LIST_TYPE entity)
        {
            try
            {
                AdmissionListType model = null;
                if (entity != null)
                {
                    model = new AdmissionListType();
                    model.Id = entity.Admission_List_Type_Id;
                    model.Name = entity.Admission_List_Type_Name;
                    //model.Description = entity.Ability_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_LIST_TYPE TranslateToEntity(AdmissionListType model)
        {
            try
            {
                ADMISSION_LIST_TYPE entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_LIST_TYPE();
                    entity.Admission_List_Type_Id = model.Id;
                    entity.Admission_List_Type_Name = model.Name;
                    //entity.Ability_Description = model.Description;
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
