
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PersonTranslator : TranslatorBase<Person, PERSON>
    {
        private PersonTypeTranslator personTypeTranslator;
        private SexTranslator sexTranslator = new SexTranslator();
        private StateTranslator stateTranslator = new StateTranslator();
        private LocalGovernmentTranslator localGovernmentTranslator = new LocalGovernmentTranslator();
        private NationalityTranslator countryTranslator = new NationalityTranslator();
        private NationalityTranslator nationalityTranslator = new NationalityTranslator();
        private RoleTranslator roleTranslator;
        private ReligionTranslator religionTranslator;

        public PersonTranslator()
        {
            roleTranslator = new RoleTranslator();
            personTypeTranslator = new PersonTypeTranslator();
            religionTranslator = new ReligionTranslator();
        }

        public override Person TranslateToModel(PERSON personEntity)
        {
            try
            {
                Person person = null;
                if (personEntity != null)
                {
                    person = new Person();
                    person.Id = personEntity.Person_Id;
                    person.FirstName = personEntity.First_Name;
                    person.LastName = personEntity.Last_Name;
                    person.Type = personTypeTranslator.TranslateToModel(personEntity.PERSON_TYPE);
                    person.OtherName = personEntity.Other_Name;
                    person.Sex = sexTranslator.TranslateToModel(personEntity.SEX);
                    person.ContactAddress = personEntity.Contact_Address;
                    person.Email = personEntity.Email;
                    person.MobilePhone = personEntity.Mobile_Phone;

                    person.SignatureFileUrl = personEntity.Signature_File_Url;
                    person.ImageFileUrl = personEntity.Image_File_Url;
                    person.DateOfBirth = personEntity.Date_Of_Birth;
                    person.State = stateTranslator.TranslateToModel(personEntity.STATE);
                    person.LocalGovernment = localGovernmentTranslator.Translate(personEntity.LOCAL_GOVERNMENT);

                    person.HomeTown = personEntity.Home_Town;
                    person.HomeAddress = personEntity.Home_Address;
                    
                    person.Nationality = nationalityTranslator.TranslateToModel(personEntity.NATIONALITY);
                    person.DateEntered = personEntity.Date_Entered;

                    //person.FullName = personEntity.First_Name + " " + personEntity.Last_Name + " " + personEntity.Other_Name;
                    //person.Name = personEntity.First_Name + " " + personEntity.Last_Name;
                    
                    person.Role = roleTranslator.Translate(personEntity.ROLE);
                    person.Initial = personEntity.Initial;
                    person.Title = personEntity.Title;

                    person.Religion = religionTranslator.Translate(personEntity.RELIGION);

                    if (personEntity.Date_Of_Birth.HasValue)
                    {
                        person.DayOfBirth = new Value() { Id = personEntity.Date_Of_Birth.Value.Day };
                        person.MonthOfBirth = new Value() { Id = personEntity.Date_Of_Birth.Value.Month };
                        person.YearOfBirth = new Value() { Id = personEntity.Date_Of_Birth.Value.Year };
                    }

                    //person.Initial = person.GetInitial(personEntity.Last_Name, personEntity.First_Name, personEntity.Other_Name);
                }

                return person;
            }
            catch (Exception)
            {
                throw;
            }
        }

        

        public override PERSON TranslateToEntity(Person person)
        {
            try
            {
                PERSON personEntity = null;
                if (person != null)
                {
                    personEntity = new PERSON();
                    personEntity.First_Name = person.FirstName;
                    personEntity.Last_Name = person.LastName;
                    personEntity.Person_Type_Id = person.Type.Id;
                    personEntity.Other_Name = person.OtherName;

                    if (person.Sex != null)
                    {
                        personEntity.Sex_Id = person.Sex.Id;
                    }

                    personEntity.Contact_Address = person.ContactAddress;
                    personEntity.Email = person.Email;
                    personEntity.Mobile_Phone = person.MobilePhone;

                    personEntity.Signature_File_Url = person.SignatureFileUrl;
                    personEntity.Image_File_Url = person.ImageFileUrl;
                    personEntity.Date_Of_Birth = person.DateOfBirth;
                    personEntity.State_Id = person.State.Id;

                    if (person.LocalGovernment != null)
                    {
                        personEntity.Local_Government_Id = person.LocalGovernment.Id;
                    }

                    personEntity.Home_Town = person.HomeTown;
                    personEntity.Home_Address = person.HomeAddress;

                    //personEntity.Country_Id = person.Country.Id;

                    personEntity.Nationality_Id = person.Nationality.Id;
                    personEntity.Date_Entered = person.DateEntered;

                    personEntity.Role_Id = person.Role.Id;

                    personEntity.Initial = person.Initial;
                    personEntity.Title = person.Title;

                    if (person.Religion != null)
                    {
                        personEntity.Religion_Id = person.Religion.Id;
                    }
                }

                return personEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
