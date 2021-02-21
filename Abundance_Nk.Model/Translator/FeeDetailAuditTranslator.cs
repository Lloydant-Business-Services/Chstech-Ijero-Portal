using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class FeeDetailAuditTranslator : TranslatorBase<FeeDetailAudit, FEE_DETAIL_AUDIT>
    {
        private readonly FeeTranslator feeTranslator;
        private readonly FeeTypeTranslator feeTypeTranslator;
        private readonly SessionTranslator sessionTranslator;
        private readonly DepartmentTranslator departmentTranslator;
        private readonly ProgrammeTranslator programmeTranslator;
        private readonly LevelTranslator levelTranslator;
        private readonly UserTranslator userTranslator;
        private readonly FeeDetailTranslator feeDetailTranslator;
        public FeeDetailAuditTranslator()
        {
            feeTranslator = new FeeTranslator();
            feeTypeTranslator = new FeeTypeTranslator();
            sessionTranslator = new SessionTranslator();
            departmentTranslator = new DepartmentTranslator();
            levelTranslator = new LevelTranslator();
            programmeTranslator = new ProgrammeTranslator();
            userTranslator = new UserTranslator();
            feeDetailTranslator = new FeeDetailTranslator();
        }
        public override FeeDetailAudit TranslateToModel(FEE_DETAIL_AUDIT entity)
        {
            try
            {
                FeeDetailAudit model = null;
                if (entity != null)
                {
                    model = new FeeDetailAudit();
                    model.Id = entity.Fee_Detail_Audit_Id;
                    model.FeeDetail = feeDetailTranslator.Translate(entity.FEE_DETAIL);
                    model.Fee = feeTranslator.Translate(entity.FEE);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.DateUploaded = entity.Date_Uploaded;
                    model.Operatiion = entity.Operation;
                }
                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public override FEE_DETAIL_AUDIT TranslateToEntity(FeeDetailAudit model)
        {
            try
            {
                FEE_DETAIL_AUDIT entity = null;
                if (model != null)
                {
                    entity = new FEE_DETAIL_AUDIT();
                    entity.Fee_Detail_Audit_Id = model.Id;
                    entity.Fee_Detail_Id = model.FeeDetail.Id;
                    entity.Fee_Id = model.Fee.Id;
                    entity.Fee_Type_Id = model.FeeType.Id;
                    if (model.Department != null && model.Department.Id > 0)
                    {
                        entity.Department_Id = model.Department.Id;
                    }
                    if (model.Programme != null && model.Programme.Id > 0)
                    {
                        entity.Programme_Id = model.Programme.Id;
                    }
                    if (model.Session != null && model.Session.Id > 0)
                    {
                        entity.Session_Id = model.Session.Id;
                    }
                    if (model.Level != null && model.Level.Id > 0)
                    {
                        entity.Level_Id = model.Level.Id;
                    }

                    entity.User_Id = model.User.Id;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Date_Uploaded = model.DateUploaded;
                    entity.Operation = model.Operatiion;
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
