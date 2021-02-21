
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class FeeTypeTranslator : TranslatorBase<FeeType, FEE_TYPE>
    {
        public override FeeType TranslateToModel(FEE_TYPE feeTypeEntity)
        {
            try
            {
                FeeType feeType = null;
                if (feeTypeEntity != null)
                {
                    feeType = new FeeType();
                    feeType.Id = feeTypeEntity.Fee_Type_Id;
                    feeType.Name = feeTypeEntity.Fee_Type_Name;
                    feeType.Description = feeTypeEntity.Fee_Type_Description;
                    //feeType.Active = feeTypeEntity.Active;
                }

                return feeType;
            }
            catch (Exception)
            {
                throw;
            };
        }

        public override FEE_TYPE TranslateToEntity(FeeType feeType)
        {
            try
            {
                FEE_TYPE feeTypeEntity = null;
                if (feeType != null)
                {
                    feeTypeEntity = new FEE_TYPE();
                    feeTypeEntity.Fee_Type_Id = feeType.Id;
                    feeTypeEntity.Fee_Type_Name = feeType.Name;
                    feeTypeEntity.Fee_Type_Description = feeType.Description;
                    //feeTypeEntity.Active = feeType.Active;
                }

                return feeTypeEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
      

    }
}
