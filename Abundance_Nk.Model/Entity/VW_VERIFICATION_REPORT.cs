//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Abundance_Nk.Model.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class VW_VERIFICATION_REPORT
    {
        public long Person_Id { get; set; }
        public string Name { get; set; }
        public string Application_Form_Number { get; set; }
        public string Application_Exam_Number { get; set; }
        public string Verification_Comment { get; set; }
        public string Verification_Officer { get; set; }
        public Nullable<bool> Verification_Status { get; set; }
        public int Session_Id { get; set; }
        public string Session_Name { get; set; }
        public int Department_Id { get; set; }
        public string Department_Name { get; set; }
        public int Programme_Id { get; set; }
        public string Programme_Name { get; set; }
        public int O_Level_Exam_Sitting_Id { get; set; }
        public string Exam_Number { get; set; }
        public int Exam_Year { get; set; }
        public string O_Level_Type_Name { get; set; }
        public string O_Level_Subject_Name { get; set; }
        public string O_Level_Grade_Name { get; set; }
        public string Applicant_Jamb_Registration_Number { get; set; }
        public Nullable<short> Applicant_Jamb_Score { get; set; }
    }
}
