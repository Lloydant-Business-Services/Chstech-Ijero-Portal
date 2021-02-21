using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class HostelRequestTranslator : TranslatorBase<HostelRequest, HOSTEL_REQUEST>
    {
        LevelTranslator levelTranslator = new LevelTranslator();
        ProgrammeTranslator programmeTranslator = new ProgrammeTranslator();
        DepartmentTranslator departmentTranslator = new DepartmentTranslator();
        StudentTranslator studentTranslator = new StudentTranslator();
        SessionTranslator sessionTranslator = new SessionTranslator();
        PersonTranslator personTranslator = new PersonTranslator();

        public override HostelRequest TranslateToModel(HOSTEL_REQUEST entity)
        {
            HostelRequest model = null;
            if (entity != null)
            {
                model = new HostelRequest();
                model.Id = entity.Hostel_Request_Id;
                model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
               
                if (entity.PERSON != null)
                {
                    model.Person = personTranslator.Translate(entity.PERSON);
                }
                model.Session = sessionTranslator.Translate(entity.SESSION);
                model.Level = levelTranslator.Translate(entity.LEVEL);
                model.Approved = entity.Approved;
                model.RequestDate = entity.Request_Date;
            }

            return model;
        }

        public override HOSTEL_REQUEST TranslateToEntity(HostelRequest model)
        {
            HOSTEL_REQUEST entity = null;

            if (model != null)
            {
                entity = new HOSTEL_REQUEST();
                entity.Hostel_Request_Id = model.Id;
                if (model.Student != null && model.Student.Id > 0)
                {
                    entity.Person_Id = model.Student.Id;
                }
                if (model.Person != null && model.Person.Id > 0)
                {
                    entity.Person_Id = model.Person.Id;
                }
                entity.Programme_Id = model.Programme.Id;
                entity.Department_Id = model.Department.Id;
                entity.Session_Id = model.Session.Id;
                entity.Level_Id = model.Level.Id;
                entity.Approved = model.Approved;
                entity.Request_Date = model.RequestDate;
            }
            return entity;
        }
    }
}
