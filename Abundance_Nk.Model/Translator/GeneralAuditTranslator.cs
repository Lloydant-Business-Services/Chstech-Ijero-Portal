using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class GeneralAuditTranslator : TranslatorBase<GeneralAudit, GENERAL_AUDIT>
    {
        private UserTranslator userTranslator;

        public GeneralAuditTranslator()
        {
            userTranslator = new UserTranslator();
        }
        public override GeneralAudit TranslateToModel(GENERAL_AUDIT entity)
        {
            try
            {
                GeneralAudit model = null;
                if (entity != null)
                {
                    model = new GeneralAudit();
                    model.Id = entity.General_Audit_Id;
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.CurrentValues = entity.Current_Values;
                    model.InitialValues = entity.Initial_Values;
                    model.Operation = entity.Operation;
                    model.TableNames = entity.Table_Names;
                    model.Time = entity.Time;
                    model.User = userTranslator.Translate(entity.USER);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override GENERAL_AUDIT TranslateToEntity(GeneralAudit model)
        {
            try
            {
                GENERAL_AUDIT entity = null;
                if (model != null)
                {
                    entity = new GENERAL_AUDIT();
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Current_Values = model.CurrentValues;
                    entity.General_Audit_Id = model.Id;
                    entity.Initial_Values = model.InitialValues;
                    entity.Operation = model.Operation;
                    entity.Table_Names = model.TableNames;
                    entity.Time = model.Time;
                    if (model.User != null && model.User.Id > 0)
                    {
                        entity.User_Id = model.User.Id;
                    }
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
