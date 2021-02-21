using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StaffTranslator : TranslatorBase<Staff, STAFF>
    {
        private StaffTypeTranslator staffTypeTranslator;
        private MaritalStatusTranslator maritalStatusTranslator;
        private UserTranslator userTranslator;
        private StateTranslator stateTranslator;
        private ReligionTranslator religionTranslator;
        public StaffTranslator()
        {
            staffTypeTranslator = new StaffTypeTranslator();
            maritalStatusTranslator = new MaritalStatusTranslator();
            userTranslator = new UserTranslator();
            stateTranslator = new StateTranslator();
            religionTranslator = new ReligionTranslator();
        }

        public override Staff TranslateToModel(STAFF entity)
        {
            try
            {
                Staff model = null;
                if (entity != null)
                {
                    model = new Staff();
                    model.Id = entity.Staff_Id;
                    model.StaffType = staffTypeTranslator.Translate(entity.STAFF_TYPE);
                    model.MaritalStatus = maritalStatusTranslator.Translate(entity.MARITAL_STATUS);
                    model.ProfileDescription = entity.Profile_Description;
                    model.User = userTranslator.Translate(entity.USER);
                    if (entity.PERSON != null)
                    {
                        model.LastName = entity.PERSON.Last_Name;
                        model.FirstName = entity.PERSON.First_Name;
                        model.OtherName = entity.PERSON.Other_Name;
                        model.MobilePhone = entity.PERSON.Mobile_Phone;
                        model.Email = entity.PERSON.Email;
                        model.State = stateTranslator.Translate(entity.PERSON.STATE);
                        model.Religion = religionTranslator.Translate(entity.PERSON.RELIGION); 
                    }
                    
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STAFF TranslateToEntity(Staff model)
        {
            try
            {
                STAFF entity = null;
                if (model != null)
                {
                    entity = new STAFF();
                    entity.Staff_Id = model.Id;
                    entity.Staff_Type_Id = model.StaffType.Id;
                    entity.Marital_Status_Id = model.MaritalStatus.Id;
                    entity.Profile_Description = model.ProfileDescription;
                    entity.User_Id = model.User.Id;
                    
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
