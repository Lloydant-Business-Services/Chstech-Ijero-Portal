using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentInformation
    {
        public long Person_Id { get; set; }
        public string Name { get; set; }
        public string ContactAddress { get; set; }
        public DateTime DateEntered { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string HomeAddress { get; set; }
        public string HomeTown { get; set; }
        public string ImageFileUrl { get; set; }
        public string MobilePhone { get; set; }
        public string Title { get; set; }
        public bool? Activated { get; set; }
        public string MatricNumber { get; set; }
        public string Reason { get; set; }
        public string SchoolContactAddress { get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public int? DepartmentOptionId { get; set; }
        public string DepartmentOptionName { get; set; }
        public int StudentStatusId { get; set; }
        public string StudentStatusName { get; set; }
        public byte? SexId { get; set; }
        public string SexName { get; set; }
        public string StateId { get; set; }
        public string StateName { get; set; }
        public int? LocalGovernmentId { get; set; }
        public string LocalGovernmentName { get; set; }
        public int? NationalityId { get; set; }
        public string NationalityName { get; set; }
        public DateTime? GraduationDate { get; set; }
        public int? YearOfAdmission { get; set; }
        public int? YearOfGraduation { get; set; }
        public int? ModeOfEntryId { get; set; }
        public string ModeOfEntryName { get; set; }
        public int? ModeOfStudyId { get; set; }
        public string ModeOfStudyName { get; set; }
        public string ApplicationExamNumber { get; set; }
        public string ApplicationFormNumber { get; set; }
        public int? AdmittedDepartmentId { get; set; }
        public string AdmittedDepartmentName { get; set; }
        public int? AdmittedOptionId { get; set; }
        public string AdmittedOptionName { get; set; }
        public int? AdmittedProgrammeId { get; set; }
        public string AdmittedProgrammeName { get; set; }
        public int? AdmittedSessionId { get; set; }
        public string AdmittedSessionName { get; set; }
        public string ApplicantJambRegistrationNumber { get; set; }
        public short? ApplicantJambScore { get; set; }
    }
}
