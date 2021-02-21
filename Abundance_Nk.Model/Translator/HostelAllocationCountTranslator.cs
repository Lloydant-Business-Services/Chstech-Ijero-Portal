using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class HostelAllocationCountTranslator : TranslatorBase<HostelAllocationCount, HOSTEL_ALLOCATION_COUNT>
    {
        private LevelTranslator levelTranslator;
        private SexTranslator sexTranslator;

        public HostelAllocationCountTranslator()
        {
            levelTranslator = new LevelTranslator();
            sexTranslator = new SexTranslator();
        }
        public override HostelAllocationCount TranslateToModel(HOSTEL_ALLOCATION_COUNT entity)
        {
            try
            {
                HostelAllocationCount model = null;
                if (entity != null)
                {
                    model = new HostelAllocationCount();
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Activated = entity.Activated;
                    model.DateSet = entity.Date_Set;
                    model.Free = entity.Free;
                    model.Id = entity.Hostel_Allocation_Count_Id;
                    model.LastModified = entity.Last_Modified;
                    model.Reserved = entity.Reserved;
                    model.Sex = sexTranslator.Translate(entity.SEX);
                    model.TotalCount = entity.Total_Count;
                    
                }

                return model;
            }
            catch (Exception)
            {   
                throw;
            }
        }

        public override HOSTEL_ALLOCATION_COUNT TranslateToEntity(HostelAllocationCount model)
        {
            try
            {
                HOSTEL_ALLOCATION_COUNT entity = null;
                if (model != null)
                {
                    entity = new HOSTEL_ALLOCATION_COUNT();
                    entity.Activated = model.Activated;
                    entity.Date_Set = model.DateSet;
                    entity.Free = model.Free;
                    entity.Hostel_Allocation_Count_Id = model.Id;
                    entity.Last_Modified = model.LastModified;
                    entity.Reserved = model.Reserved;
                    entity.Total_Count = model.TotalCount;
                    entity.Level_Id = model.Level.Id;
                    entity.Sex_Id = model.Sex.Id;
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
