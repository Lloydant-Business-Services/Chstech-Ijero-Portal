using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class RightLogic : BusinessBaseLogic<Right, RIGHT>
    {
        public RightLogic()
        {
            base.translator = new RightTranslator();
        }

        public bool Modify(Right right)
        {
            try
            {
                Expression<Func<RIGHT, bool>> selector = r => r.Right_Id == right.Id;
                RIGHT rightEntity = GetEntityBy(selector);
                rightEntity.Right_Name = right.Name;
                rightEntity.Right_Description = right.Description;

                int rowsAffected = repository.Save();

                if (rowsAffected > 0)
                {
                    return true;
                }
                else
                {
                    throw new Exception(NoItemModified);
                }
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException(ArgumentNullException);
            }
            //catch (UpdateException)
            //{
            //    throw new UpdateException(UpdateException);
            //}
            catch (Exception)
            {
                throw;
            }
        }

        public bool Remove(Right right)
        {
            try
            {
                Func<RIGHT, bool> selector = r => r.Right_Id == right.Id;
                bool suceeded = base.Delete(selector);
                repository.Save();

                //base.ContextManager.SummitChanges();
                return suceeded;
            }
            catch (Exception)
            {
                throw;
            }
        }




    }

}
