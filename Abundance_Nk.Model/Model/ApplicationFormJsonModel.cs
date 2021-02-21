using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicationFormJsonModel
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string PersonId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public string SexId { get; set; }
        public string Sex { get; set; }
        public string TitleId { get; set; }
        public string Title { get; set; }
        public string MaritalStatusId { get; set; }
        public string MaritalStatus { get; set; }
        public string GenotypeId { get; set; }
        public string Genotype { get; set; }
        public string BloodGroupId { get; set; }
        public string BloodGroup { get; set; }
        public string YearOfBirthId { get; set; }
        public string MonthOfBirthId { get; set; }
        public string DayOfBirthId { get; set; }
        public string StateId { get; set; }
        public string State { get; set; }
        public string LocalGovernmentId { get; set; }
        public string LocalGovernment { get; set; }
        public string HomeTown { get; set; }
        public string HomeAddress { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string ReligionId { get; set; }
        public string Religion { get; set; }
        public string ContactAddress { get; set; }
        public string SponsorId { get; set; }
        public string SponsorName { get; set; }
        public string SponsorContactAddress { get; set; }
        public string SponsorRelationshipId { get; set; }
        public string SponsorRelationship { get; set; }
        public string SponsorMobilePhone { get; set; }
        public string SponsorEmail { get; set; }
        public string FirstSittingOLevelResultId { get; set; }
        public string FirstSittingOLevelResultTypeId { get; set; }
        public string FirstSittingOLevelResultType { get; set; }
        public string FirstSittingOLevelResultExamNumber { get; set; }
        public string FirstSittingOLevelResultExamYear { get; set; }
        public string SecondSittingOLevelResultId { get; set; }
        public string SecondSittingOLevelResultTypeId { get; set; }
        public string SecondSittingOLevelResultType { get; set; }
        public string SecondSittingOLevelResultExamNumber { get; set; }
        public string SecondSittingOLevelResultExamYear { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string ProgrammeId { get; set; }
        public string DepartmentId { get; set; }
        public string Faculty { get; set; }
        public string FacultyId { get; set; }
        public string Level { get; set; }
        public string LevelId { get; set; }
        public string MatricNumber { get; set; }
        public string ModeOfEntry { get; set; }
        public string ModeOfEntryId { get; set; }
        public string ModeOfFinanceId { get; set; }
        public string ModeOfFinance { get; set; }
        public string ModeOfStudy { get; set; }
        public string ModeOfStudyId { get; set; }
        public string StudentTypeId { get; set; }
        public string StudentType { get; set; }
        public string StudentCategoryId { get; set; }
        public string StudentCategory { get; set; }
        public string SessionId { get; set; }
        public string Session { get; set; }
        public string YearOfAdmission { get; set; }
        public string YearOfGraduation { get; set; }
        public string ModeOfAdmission { get; set; }
        public string ScholarshipTitle { get; set; }
        public string ScholarshipTitleId { get; set; }
        public string NextOfKinMobilePhone { get; set; }
        public string NextOfKinContactAddress { get; set; }
        public string NextOfKinRelationshipId { get; set; }
        public string NextOfKinRelationship { get; set; }
        public string NextOfKinName { get; set; }
        public string DateOfBirth { get; set; }
        public string ImageFileUrl { get; set; }
        public string ApplicantId { get; set; }
        public string AbilityId { get; set; }
        public string OtherAbility { get; set; }
        public string ExtraCurricullarActivities { get; set; }

        public string ApplicationFormId { get; set; }

        public string ApplicationFormNumber { get; set; }

        public string ApplicationFormSettingId { get; set; }

        public string PaymentTypeId { get; set; }

        public string ProgrammeFeeId { get; set; }

        public string FeeTypeId { get; set; }

        public string PaymentId { get; set; }

        public string ApplicationAlreadyExist { get; set; }
        public List<OLevelResultDetailJsonModel> FirstSittingOLevelJsonData { get; set; }
        public List<OLevelResultDetailJsonModel> SecondSittingOLevelJsonData { get; set; }
        public string FirstSittingOLevelResultScratchCard { get; set; }
        public string SecondSittingOLevelResultScratchCard { get; set; }
        public string FirstSittingOLevelResultScratchCardSerialNo { get; set; }
        public string SecondSittingOLevelResultScratchCardSerialNo { get; set; }
    }

    public class OLevelResultDetailJsonModel
    {
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string GradeId { get; set; }
        public string GradeName { get; set; }
    }
}
