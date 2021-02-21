using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class HostelAmountTranslator : TranslatorBase<HostelAmount, HOSTEL_AMOUNT>
    {
        private HostelTranslator hostelTranslator;

        public HostelAmountTranslator()
        {
            hostelTranslator = new HostelTranslator();
        }

        public override HostelAmount TranslateToModel(HOSTEL_AMOUNT entity)
        {
            try
            {
                HostelAmount model = null;
                if (entity != null)
                {
                    model = new HostelAmount();
                    model.Id = entity.Hostel_Amount_Id;
                    model.Amount = entity.Amount;
                    model.Hostel = hostelTranslator.Translate(entity.HOSTEL);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override HOSTEL_AMOUNT TranslateToEntity(HostelAmount model)
        {
            try
            {
                HOSTEL_AMOUNT entity = null;
                if (model != null)
                {
                    entity = new HOSTEL_AMOUNT();
                    entity.Hostel_Amount_Id = model.Id;
                    entity.Hostel_Id = model.Hostel.Id;
                    entity.Amount = model.Amount;
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
