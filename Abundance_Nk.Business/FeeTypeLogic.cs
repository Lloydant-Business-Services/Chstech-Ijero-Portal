
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class FeeTypeLogic : BusinessBaseLogic<FeeType, FEE_TYPE>
    {
        public FeeTypeLogic()
        {
            translator = new FeeTypeTranslator();
        }

        public bool Modify(FeeType feeType)
        {
            try
            {
                Expression<Func<FEE_TYPE, bool>> selector = f => f.Fee_Type_Id == feeType.Id;
                FEE_TYPE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Fee_Type_Name = feeType.Name;
                entity.Fee_Type_Description = feeType.Description;
                                
                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public bool Delete(FeeType feeType)
        //{
        //    try
        //    {
        //        Func<FEE_TYPE, bool> selector = f => f.Fee_Type_Id == feeType.Id;
               
        //        repository.Delete(selector);
        //        return Save() > 0 ? true : false;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}





    }
}
