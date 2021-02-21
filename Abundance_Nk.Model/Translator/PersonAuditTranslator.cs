using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class PersonAuditTranslator : TranslatorBase<PersonAudit, PERSON_AUDIT>
    {
        private PersonTypeTranslator personTypeTranslator;
        private SexTranslator sexTranslator = new SexTranslator();
        private StateTranslator stateTranslator = new StateTranslator();
        private LocalGovernmentTranslator localGovernmentTranslator = new LocalGovernmentTranslator();
        private NationalityTranslator countryTranslator = new NationalityTranslator();
        private NationalityTranslator nationalityTranslator = new NationalityTranslator();
        private RoleTranslator roleTranslator;
        private ReligionTranslator religionTranslator;
        private UserTranslator userTranslator;
        private PersonTranslator personTranslator;

        public PersonAuditTranslator()
        {
            roleTranslator = new RoleTranslator();
            personTypeTranslator = new PersonTypeTranslator();
            religionTranslator = new ReligionTranslator();
            userTranslator = new UserTranslator();
            personTranslator = new PersonTranslator();
        }

        public override PersonAudit TranslateToModel(PERSON_AUDIT personEntity)
        {
            try
            {
                PersonAudit person = null;
                if (personEntity != null)
                {
                    person = new PersonAudit();
                    person.Id = personEntity.Person_Audit_Id;
                    person.Person = personTranslator.Translate(personEntity.PERSON);
                    person.FirstName = personEntity.First_Name;
                    person.LastName = personEntity.Last_Name;
                    person.PersonType = personTypeTranslator.Translate(personEntity.PERSON_TYPE);
                    person.OtherName = personEntity.Other_Name;
                    person.Sex = sexTranslator.Translate(personEntity.SEX);
                    person.ContactAddress = personEntity.Contact_Address;
                    person.Email = personEntity.Email;
                    person.MobilePhone = personEntity.Mobile_Phone;
                    person.SignatureFileUrl = personEntity.Signature_File_Url;
                    person.ImageFileUrl = personEntity.Image_File_Url;
                    person.DateOfBirth = personEntity.Date_Of_Birth;
                    person.State = stateTranslator.Translate(personEntity.STATE);
                    person.LocalGovernment = localGovernmentTranslator.Translate(personEntity.LOCAL_GOVERNMENT);
                    person.HomeTown = personEntity.Home_Town;
                    person.HomeAddress = personEntity.Home_Address;
                    person.Nationality = nationalityTranslator.Translate(personEntity.NATIONALITY);
                    person.DateEntered = personEntity.Date_Entered;
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

                    person.OldLastName = personEntity.Old_Last_Name;
                    person.OldFirstName = personEntity.Old_First_Name;
                    person.OldOtherName = personEntity.Old_Other_Name;
                    person.OldPersonType = personTypeTranslator.Translate(personEntity.PERSON_TYPE1);
                    person.OldSex = sexTranslator.Translate(personEntity.SEX1);
                    person.OldContactAddress = personEntity.Old_Contact_Address;
                    person.OldEmail = personEntity.Old_Email;
                    person.OldMobilePhone = personEntity.Old_Mobile_Phone;
                    person.OldSignatureFileUrl = personEntity.Old_Signature_File_Url;
                    person.OldImageFileUrl = personEntity.Old_Image_File_Url;
                    person.OldDateOfBirth = personEntity.Old_Date_Of_Birth;
                    person.OldState = stateTranslator.Translate(personEntity.STATE1);
                    person.OldLocalGovernment = localGovernmentTranslator.Translate(personEntity.LOCAL_GOVERNMENT1);
                    person.OldHomeTown = personEntity.Old_Home_Town;
                    person.OldHomeAddress = personEntity.Old_Home_Address;
                    person.OldNationality = nationalityTranslator.Translate(personEntity.NATIONALITY1);
                    person.OldDateEntered = personEntity.Old_Date_Entered;
                    person.OldRole = roleTranslator.Translate(personEntity.ROLE1);
                    person.OldInitial = personEntity.Old_Initial;
                    person.OldTitle = personEntity.Old_Title;
                    person.OldReligion = religionTranslator.Translate(personEntity.RELIGION1);

                    person.User = userTranslator.Translate(personEntity.USER);
                    person.Operation = personEntity.Operation;
                    person.Action = personEntity.Action;
                    person.Time = personEntity.Time;
                    person.Client = personEntity.Client;
                }

                return person;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PERSON_AUDIT TranslateToEntity(PersonAudit person)
        {
            try
            {
                PERSON_AUDIT personEntity = null;
                if (person != null)
                {
                    personEntity = new PERSON_AUDIT();
                    personEntity.Person_Audit_Id = person.Id;
                    personEntity.Person_Id = person.Person.Id;
                    personEntity.First_Name = person.FirstName;
                    personEntity.Last_Name = person.LastName;
                    personEntity.Other_Name = person.OtherName;
                    if (person.PersonType != null && person.PersonType.Id > 0)
                    {
                        personEntity.Person_Type_Id = person.PersonType.Id;
                    }

                    if (person.Sex != null && person.Sex.Id > 0)
                    {
                        personEntity.Sex_Id = person.Sex.Id;
                    }

                    personEntity.Contact_Address = person.ContactAddress;
                    personEntity.Email = person.Email;
                    personEntity.Mobile_Phone = person.MobilePhone;
                    personEntity.Signature_File_Url = person.SignatureFileUrl;
                    personEntity.Image_File_Url = person.ImageFileUrl;
                    personEntity.Date_Of_Birth = person.DateOfBirth;

                    if (person.State != null && !string.IsNullOrWhiteSpace(person.State.Id))
                    {
                        personEntity.State_Id = person.State.Id;
                    }
                    if (person.LocalGovernment != null && person.LocalGovernment.Id > 0)
                    {
                        personEntity.Local_Government_Id = person.LocalGovernment.Id;
                    }

                    personEntity.Home_Town = person.HomeTown;
                    personEntity.Home_Address = person.HomeAddress;
                    if (person.Nationality != null && person.Nationality.Id > 0)
                    {
                        personEntity.Nationality_Id = person.Nationality.Id;
                    }
                    if (person.DateEntered != null)
                    {
                        personEntity.Date_Entered = person.DateEntered;
                    }

                    if (person.Role != null && person.Role.Id > 0)
                    {
                        personEntity.Role_Id = person.Role.Id;
                    }

                    personEntity.Initial = person.Initial;
                    personEntity.Title = person.Title;
                    if (person.Religion != null && person.Religion.Id > 0)
                    {
                        personEntity.Religion_Id = person.Religion.Id;
                    }

                    //old
                    personEntity.Old_First_Name = person.OldFirstName;
                    personEntity.Old_Last_Name = person.OldLastName;
                    personEntity.Old_Person_Type_Id = person.OldPersonType.Id;
                    personEntity.Old_Other_Name = person.OldOtherName;

                    if (person.OldSex != null && person.OldSex.Id > 0)
                    {
                        personEntity.Old_Sex_Id = person.OldSex.Id;
                    }

                    personEntity.Old_Contact_Address = person.OldContactAddress;
                    personEntity.Old_Email = person.OldEmail;
                    personEntity.Old_Mobile_Phone = person.OldMobilePhone;
                    personEntity.Old_Signature_File_Url = person.OldSignatureFileUrl;
                    personEntity.Old_Image_File_Url = person.OldImageFileUrl;
                    personEntity.Old_Date_Of_Birth = person.OldDateOfBirth;

                    if (person.OldState != null && !string.IsNullOrWhiteSpace(person.OldState.Id))
                    {
                        personEntity.Old_State_Id = person.OldState.Id;
                    }

                    if (person.OldLocalGovernment != null && person.OldLocalGovernment.Id > 0)
                    {
                        personEntity.Old_Local_Government_Id = person.OldLocalGovernment.Id;
                    }

                    personEntity.Old_Home_Town = person.OldHomeTown;
                    personEntity.Old_Home_Address = person.OldHomeAddress;
                    if (person.OldNationality != null && person.OldNationality.Id > 0)
                    {
                        personEntity.Old_Nationality_Id = person.OldNationality.Id;
                    }

                    personEntity.Old_Date_Entered = person.OldDateEntered;
                    if (person.OldRole != null && person.OldRole.Id > 0)
                    {
                        personEntity.Old_Role_Id = person.OldRole.Id;
                    }

                    personEntity.Old_Initial = person.OldInitial;
                    personEntity.Old_Title = person.OldTitle;
                    if (person.OldReligion != null && person.OldReligion.Id > 0)
                    {
                        personEntity.Old_Religion_Id = person.OldReligion.Id;
                    }

                    personEntity.User_Id = person.User.Id;
                    personEntity.Operation = person.Operation;
                    personEntity.Action = person.Action;
                    personEntity.Time = person.Time;
                    personEntity.Client = person.Client;
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
