using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class StudentSponsorTranslator : TranslatorBase<StudentSponsor, STUDENT_SPONSOR>
    {
        private StudentTranslator studentTranslator;
        private RelationshipTranslator relationshipTranslator;

        public StudentSponsorTranslator()
        {
            studentTranslator = new StudentTranslator();
            relationshipTranslator = new RelationshipTranslator();
        }

        public override StudentSponsor TranslateToModel(STUDENT_SPONSOR entity)
        {
            try
            {
                StudentSponsor model = null;
                if (entity != null)
                {
                    model = new StudentSponsor();
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Relationship = relationshipTranslator.Translate(entity.RELATIONSHIP);
                    model.Name = entity.Sponsor_Name;
                    model.ContactAddress = entity.Sponsor_Contact_Address;
                    model.MobilePhone = entity.Sponsor_Mobile_Phone;
                    model.Email = entity.Email;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_SPONSOR TranslateToEntity(StudentSponsor model)
        {
            try
            {
                STUDENT_SPONSOR entity = null;
                if (model != null)
                {
                    entity = new STUDENT_SPONSOR();
                    entity.Person_Id = model.Student.Id;
                    entity.Relationship_Id = model.Relationship.Id;
                    entity.Sponsor_Name = model.Name;
                    entity.Sponsor_Contact_Address = model.ContactAddress;
                    entity.Sponsor_Mobile_Phone = model.MobilePhone;
                    entity.Email = model.Email;
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
