using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class HostelBlacklistTranslator : TranslatorBase<HostelBlacklist, HOSTEL_BLACKLIST>
    {
        LevelTranslator levelTranslator = new LevelTranslator();
        ProgrammeTranslator programmeTranslator = new ProgrammeTranslator();
        DepartmentTranslator departmentTranslator = new DepartmentTranslator();
        StudentTranslator studentTranslator = new StudentTranslator();
        SessionTranslator sessionTranslator = new SessionTranslator();

        public override HostelBlacklist TranslateToModel(HOSTEL_BLACKLIST entity)
        {
            HostelBlacklist model = null;
            if (entity != null)
            {
                model = new HostelBlacklist();
                model.Id = entity.Hostel_Blacklist_Id;
                model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                model.Student = studentTranslator.Translate(entity.STUDENT);
                model.Session = sessionTranslator.Translate(entity.SESSION);
                model.Level = levelTranslator.Translate(entity.LEVEL);
                model.Reason = entity.Reason;
            }

            return model;
        }

        public override HOSTEL_BLACKLIST TranslateToEntity(HostelBlacklist model)
        {
            HOSTEL_BLACKLIST entity = null;

            if (model != null)
            {
                entity = new HOSTEL_BLACKLIST();
                entity.Hostel_Blacklist_Id = model.Id;
                entity.Person_Id = model.Student.Id;
                entity.Programme_Id = model.Programme.Id;
                entity.Department_Id = model.Department.Id;
                entity.Session_Id = model.Session.Id;
                entity.Level_Id = model.Level.Id;
                entity.Reason = model.Reason;
            }

            return entity;
        }
    }
}
