using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Entity.Model
{
    public class StudentDetailsModel
    {
        public int SN { get; set; }
        public long PersonId { get; set; }
        public string Name { get; set; }
        public string ImageFileUrl { get; set; }
        public string SexName { get; set; }
        public string ContactAddress { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string DateOfBirth { get; set; }
        public string StateName { get; set; }
        public string LocalGovernmentName { get; set; }
        public string NationalityName { get; set; }
        public string HomeTown { get; set; }
        public string HomeAddress { get; set; }
        public string ReligionName { get; set; }
        public string MatricNumber { get; set; }
        public string ApplicationNumber { get; set; }
        public string SchoolContactAddress { get; set; }
        public string Genotype { get; set; }
        public string BloodGroup{ get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
        public string ProgrammeShortName { get; set; }
        public string DepartmentCode { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public string SessionName { get; set; }
        public string FacultyName { get; set; }
        public int DepartmentOptionId { get; set; }
        public string DepartmentOptionName { get; set; }
    }
    public class StudentDetailsDisplay
    {
        public int SN { get; set; }
        public string Name { get; set; }
        public string SexName { get; set; }
        public string Genotype { get; set; }
        public string HomeTown { get; set; }
        
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string StateName { get; set; }
        public string LocalGovernmentName { get; set; }
        public string NationalityName { get; set; }
        public string ProgrammeName { get; set; }
        public string DepartmentName { get; set; }

        public string MatricNumber { get; set; }

        public string DateOfBirth { get; set; }
    }

}
