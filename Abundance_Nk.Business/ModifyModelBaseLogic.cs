
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Abundance_Nk.Business
{
    public abstract class ModifyModelBaseLogic<T, E>
        where T : class
        where E : class
    {
        //private IRepository repository;

        //protected const string NoItemModified = "No item modified!";
        //protected const string NoItemFound = "No item found to modified!";

        //public ModifyModelBaseLogic(IRepository _repository)
        //{
        //    repository = _repository;
        //}

        //public virtual bool Modify(T model)
        //{
        //    try
        //    {
        //        E entity = ModifyHelper(model);
        //        if (entity == null)
        //        {
        //            throw new Exception(NoItemFound);
        //        }

        //        int modifiedRecordCount = repository.Save();
        //        if (modifiedRecordCount <= 0)
        //        {
        //            throw new Exception(NoItemModified);
        //        }

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //protected abstract E ModifyHelper(T model);


    }
}
