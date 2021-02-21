using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantPreviousEducation
    {
        public long Applicant_Previous_Education_Id { get; set; }
        public long Person_Id { get; set; }
        public string Previous_School_Name { get; set; }
        public string Previous_Course { get; set; }
        public DateTime? Previous_Education_Start_Date { get; set; }
        public DateTime Previous_Education_End_Date { get; set; }
        public int Educational_Qualification_Id { get; set; }
        public int Result_Grade_Id { get; set; }
        public int IT_Duration_Id { get; set; }
        public long? Application_Form_Id { get; set; }
        public int? Previous_School_Id { get; set; }
        public DateTime? IT_Start_Date { get; set; }
        public DateTime? IT_End_Date { get; set; }

        public ApplicationForm ApplicationForm { get; set; }
        public Person Person { get; set; }
        public ResultGrade ResultGrade { get; set; }
        public TertiaryInstitution TertiaryInstitution { get; set; }
        public EducationalQualification EducationalQualification { get; set; }
        public ITDuration ITDuration { get; set; }
    }
}
