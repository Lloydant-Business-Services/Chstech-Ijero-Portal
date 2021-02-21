using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class FeeDetailTranslator : TranslatorBase<FeeDetail, FEE_DETAIL>
    {
        private FeeTranslator feeTranslator;
        private FeeTypeTranslator feeTypeTranslator;
        private ProgrammeTranslator programmeTranslator;
        private LevelTranslator levelTranslator;
        private PaymentModeTranslator paymentModeTranslator;
        private DepartmentTranslator departmentTranslator;
        private SessionTranslator sessionTranslator;

        public FeeDetailTranslator()
        {
            feeTranslator = new FeeTranslator();
            feeTypeTranslator = new FeeTypeTranslator();
            programmeTranslator = new ProgrammeTranslator();
            levelTranslator = new LevelTranslator();
            paymentModeTranslator = new PaymentModeTranslator();
            departmentTranslator = new DepartmentTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override FeeDetail TranslateToModel(FEE_DETAIL entity)
        {
            try
            {
                FeeDetail model = null;
                if (entity != null)
                {
                    model = new FeeDetail();
                    model.Id = entity.Fee_Detail_Id;
                    model.Fee = feeTranslator.Translate(entity.FEE);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    
                    if (entity.PROGRAMME != null)
                    {
                        model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    }
                    if (entity.PAYMENT_MODE != null)
                    {
                        model.PaymentMode = paymentModeTranslator.Translate(entity.PAYMENT_MODE);
                    }
                    if (entity.LEVEL != null)
                    {
                        model.Level = levelTranslator.Translate(entity.LEVEL);
                    }
                    if (entity.DEPARTMENT != null)
                    {
                        model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    }
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override FEE_DETAIL TranslateToEntity(FeeDetail model)
        {
            try
            {
                FEE_DETAIL entity = null;
                if (model != null)
                {
                    entity = new FEE_DETAIL();
                    entity.Fee_Detail_Id = model.Id;
                    entity.Fee_Id = model.Fee.Id;
                    entity.Fee_Type_Id = model.FeeType.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Payment_Mode_Id = model.PaymentMode.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Programme_Id = model.Programme.Id;
                    
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
