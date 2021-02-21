using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class PersonLogic : BusinessBaseLogic<Person, PERSON>
    {
        private PersonAuditLogic personAuditLogic;

        public PersonLogic()
        {
            translator = new PersonTranslator();
        }

        public bool Modify(Person person)
        {
            try
            {
                Expression<Func<PERSON, bool>> selector = p => p.Person_Id == person.Id;
                PERSON personEntity = GetEntityBy(selector);

                if (personEntity == null || personEntity.Person_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                    personEntity.First_Name = person.FirstName;               
                    personEntity.Last_Name = person.LastName;
                    personEntity.Other_Name = person.OtherName;
               
                if (person.ContactAddress != null)
                {
                    personEntity.Contact_Address = person.ContactAddress;
                }
                if (person.Email != null)
                {
                    personEntity.Email = person.Email;
                }
                if (person.MobilePhone != null)
                {
                    personEntity.Mobile_Phone = person.MobilePhone;
                }
                if (person.SignatureFileUrl != null)
                {
                    personEntity.Signature_File_Url = person.SignatureFileUrl;
                }
                if (person.ImageFileUrl != null)
                {
                    personEntity.Image_File_Url = person.ImageFileUrl;
                }
                if (person.DateOfBirth != null)
                {
                    personEntity.Date_Of_Birth = person.DateOfBirth;
                }
                if (person.HomeTown != null)
                {
                    personEntity.Home_Town = person.HomeTown;
                }
                if (person.HomeAddress != null)
                {
                    personEntity.Home_Address = person.HomeAddress;
                }
                if (person.DateEntered != null)
                {
                    personEntity.Date_Entered = personEntity.Date_Entered;
                }
                if (person.Initial != null)
                {
                    personEntity.Initial = person.Initial;
                }
                if (person.Title != null)
                {
                    personEntity.Title = person.Title;
                }

                if (person.Role != null && person.Role.Id > 0)
                {
                    personEntity.Role_Id = person.Role.Id;
                }
                if (person.Nationality != null && person.Nationality.Id > 0)
                {
                    personEntity.Nationality_Id = person.Nationality.Id;
                }
                if (person.State != null && !string.IsNullOrEmpty(person.State.Id))
                {
                    personEntity.State_Id = person.State.Id;
                }
                if (person.Type != null && person.Type.Id > 0)
                {
                    personEntity.Person_Type_Id = person.Type.Id;
                }
                if (person.Religion != null)
                {
                    personEntity.Religion_Id = person.Religion.Id;
                }
                if (person.LocalGovernment != null)
                {
                    personEntity.Local_Government_Id = person.LocalGovernment.Id;
                }
                if (person.Sex != null)
                {
                    personEntity.Sex_Id = person.Sex.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool Modify(Person person, PersonAudit audit)
        {
            try
            {
                Expression<Func<PERSON, bool>> selector = p => p.Person_Id == person.Id;
                PERSON personEntity = GetEntityBy(selector);

                bool audited = CreateAudit(person, audit, personEntity);
                if (audited)
                {
                    if (personEntity == null || personEntity.Person_Id <= 0)
                    {
                        throw new Exception(NoItemFound);
                    }

                    if (person.FirstName != null)
                    {
                        personEntity.First_Name = person.FirstName;
                    }
                    if (person.LastName != null)
                    {
                        personEntity.Last_Name = person.LastName;
                    }
                    if (person.OtherName != null)
                    {
                        personEntity.Other_Name = person.OtherName;
                    }
                    if (person.ContactAddress != null)
                    {
                        personEntity.Contact_Address = person.ContactAddress;
                    }
                    if (person.Email != null)
                    {
                        personEntity.Email = person.Email;
                    }
                    if (person.MobilePhone != null)
                    {
                        personEntity.Mobile_Phone = person.MobilePhone;
                    }
                    if (person.SignatureFileUrl != null)
                    {
                        personEntity.Signature_File_Url = person.SignatureFileUrl;
                    }
                    if (person.ImageFileUrl != null)
                    {
                        personEntity.Image_File_Url = person.ImageFileUrl;
                    }
                    if (person.DateOfBirth != null)
                    {
                        personEntity.Date_Of_Birth = person.DateOfBirth;
                    }
                    if (person.HomeTown != null)
                    {
                        personEntity.Home_Town = person.HomeTown;
                    }
                    if (person.HomeAddress != null)
                    {
                        personEntity.Home_Address = person.HomeAddress;
                    }
                    if (person.DateEntered != null)
                    {
                        personEntity.Date_Entered = personEntity.Date_Entered;
                    }
                    if (person.Initial != null)
                    {
                        personEntity.Initial = person.Initial;
                    }
                    if (person.Title != null)
                    {
                        personEntity.Title = person.Title;
                    }

                    if (person.Role != null && person.Role.Id > 0)
                    {
                        personEntity.Role_Id = person.Role.Id;
                    }
                    if (person.Nationality != null && person.Nationality.Id > 0)
                    {
                        personEntity.Nationality_Id = person.Nationality.Id;
                    }
                    if (person.State != null && !string.IsNullOrEmpty(person.State.Id))
                    {
                        personEntity.State_Id = person.State.Id;
                    }
                    if (person.Type != null && person.Type.Id > 0)
                    {
                        personEntity.Person_Type_Id = person.Type.Id;
                    }
                    if (person.Religion != null && person.Religion.Id > 0)
                    {
                        personEntity.Religion_Id = person.Religion.Id;
                    }
                    if (person.LocalGovernment != null && person.LocalGovernment.Id > 0)
                    {
                        personEntity.Local_Government_Id = person.LocalGovernment.Id;
                    }
                    if (person.Sex != null && person.Sex.Id > 0)
                    {
                        personEntity.Sex_Id = person.Sex.Id;
                    }

                    int modifiedRecordCount = Save();
                    if (modifiedRecordCount <= 0)
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateAudit(Person person, PersonAudit audit, PERSON personEntity)
        {
            try
            {
                audit.Person = new Person() { Id = person.Id };
                audit.FirstName = person.FirstName;
                audit.LastName = person.LastName;
                audit.PersonType = person.Type;
                audit.OtherName = person.OtherName;
                audit.Sex = person.Sex;
                audit.ContactAddress = person.ContactAddress;
                audit.Email = person.Email;
                audit.MobilePhone = person.MobilePhone;
                audit.SignatureFileUrl = person.SignatureFileUrl;
                audit.ImageFileUrl = person.ImageFileUrl;
                audit.DateOfBirth = person.DateOfBirth;
                audit.State = person.State;
                audit.LocalGovernment = person.LocalGovernment;
                audit.HomeTown = person.HomeTown;
                audit.HomeAddress = person.HomeAddress;
                audit.Nationality = person.Nationality;
                audit.DateEntered = person.DateEntered;
                audit.Role = person.Role;
                audit.Initial = person.Initial;
                audit.Title = person.Title;
                audit.Religion = person.Religion;


                Person oldPerson = translator.Translate(personEntity);
                audit.OldFirstName = oldPerson.FirstName;
                audit.OldLastName = oldPerson.LastName;
                audit.OldPersonType = oldPerson.Type;
                audit.OldOtherName = oldPerson.OtherName;
                audit.OldSex = oldPerson.Sex;
                audit.OldContactAddress = oldPerson.ContactAddress;
                audit.OldEmail = oldPerson.Email;
                audit.OldMobilePhone = oldPerson.MobilePhone;
                audit.OldSignatureFileUrl = oldPerson.SignatureFileUrl;
                audit.OldImageFileUrl = oldPerson.ImageFileUrl;
                audit.OldDateOfBirth = oldPerson.DateOfBirth;
                audit.OldState = oldPerson.State;
                audit.OldLocalGovernment = oldPerson.LocalGovernment;
                audit.OldHomeTown = oldPerson.HomeTown;
                audit.OldHomeAddress = oldPerson.HomeAddress;
                audit.OldNationality = oldPerson.Nationality;
                audit.OldDateEntered = oldPerson.DateEntered;
                audit.OldRole = oldPerson.Role;
                audit.OldInitial = oldPerson.Initial;
                audit.OldTitle = oldPerson.Title;
                audit.OldReligion = oldPerson.Religion;

                personAuditLogic = new PersonAuditLogic();
                PersonAudit personAudit = personAuditLogic.Create(audit);
                if (personAudit == null || personAudit.Id <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Person GetPersonByMatricNumber(string matricNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(matricNumber))
                {
                    StudentLogic studentLogic = new StudentLogic();
                    List<Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == matricNumber);

                    if (students != null && students.Count > 0)
                        return students[0];

                    ApplicationFormLogic formLogic = new ApplicationFormLogic();
                    List<ApplicationForm> form = formLogic.GetModelsBy(a => a.Application_Form_Number == matricNumber);

                    if (form != null && form.Count > 0)
                        return form[0].Person;
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }



}
