using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Model.Translator
{
    public class HostelRequestCountTranslator : TranslatorBase<HostelRequestCount, HOSTEL_REQUEST_COUNT>
    {
        private LevelTranslator levelTranslator;
        private SexTranslator sexTranslator;

        public HostelRequestCountTranslator()
        {
            levelTranslator = new LevelTranslator();
            sexTranslator = new SexTranslator();
        }
        public override HostelRequestCount TranslateToModel(HOSTEL_REQUEST_COUNT entity)
        {
            try
            {
                HostelRequestCount model = null;
                if (entity != null)
                {
                    model = new HostelRequestCount();
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.DateSet = entity.Date_Set;
                    model.Id = entity.Hostel_Request_Count_Id;
                    model.LastModified = entity.Last_Modified;
                    model.Sex = sexTranslator.Translate(entity.SEX);
                    model.TotalCount = entity.Total_Count;
                    model.Approved = entity.Approved;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override HOSTEL_REQUEST_COUNT TranslateToEntity(HostelRequestCount model)
        {
            try
            {
                HOSTEL_REQUEST_COUNT entity = null;
                if (model != null)
                {
                    entity = new HOSTEL_REQUEST_COUNT();

                    entity.Date_Set = model.DateSet;

                    entity.Hostel_Request_Count_Id = model.Id;
                    entity.Last_Modified = model.LastModified;
                    entity.Total_Count = model.TotalCount;
                    entity.Level_Id = model.Level.Id;
                    entity.Sex_Id = model.Sex.Id;
                    entity.Approved = model.Approved;
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
