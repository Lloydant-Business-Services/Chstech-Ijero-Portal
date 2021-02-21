using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantSponsorTranslator: TranslatorBase<ApplicantSponsor, APPLICANT_SPONSOR>
    {
        private ApplicationFormTranslator ApplicationFormTranslator;
        private PersonTranslator PersonTranslator;
        private RelationshipTranslator RelationshipTranslator;

        public ApplicantSponsorTranslator()
        {
            ApplicationFormTranslator = new ApplicationFormTranslator();
            PersonTranslator = new PersonTranslator();
            RelationshipTranslator = new RelationshipTranslator();
        }

        public override APPLICANT_SPONSOR TranslateToEntity(ApplicantSponsor model)
        {
            try
            {
                APPLICANT_SPONSOR entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_SPONSOR();
                    entity.Sponsor_Name = model.Sponsor_Name;
                    entity.Person_Id = model.Person.Id;
                    entity.Relationship_Id = model.Relationship.Id;
                    entity.Sponsor_Mobile_Phone = model.Sponsor_Mobile_Phone;
                    entity.Sponsor_Contact_Address = model.Sponsor_Contact_Address;
                    entity.Application_Form_Id = model.ApplicationForm.Id;
                }

                return entity;
            }
            catch (Exception ex) { throw ex; }
        }

        public override ApplicantSponsor TranslateToModel(APPLICANT_SPONSOR entity)
        {
            try
            {
                ApplicantSponsor model = null;
                if (entity != null)
                {
                    model = new ApplicantSponsor();
                    model.ApplicationForm = ApplicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Relationship = RelationshipTranslator.Translate(entity.RELATIONSHIP);
                    model.Person = PersonTranslator.Translate(entity.PERSON);
                    model.Sponsor_Name = entity.Sponsor_Name;
                    model.Sponsor_Mobile_Phone = entity.Sponsor_Mobile_Phone;
                    model.Sponsor_Contact_Address = entity.Sponsor_Contact_Address;
                }

                return model;
            }
            catch(Exception ex) { throw ex; }
        }
    }
}
