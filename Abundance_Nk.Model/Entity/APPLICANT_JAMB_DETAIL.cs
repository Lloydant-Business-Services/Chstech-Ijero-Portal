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
    
    public partial class APPLICANT_JAMB_DETAIL
    {
        public long Person_Id { get; set; }
        public string Applicant_Jamb_Registration_Number { get; set; }
        public Nullable<short> Applicant_Jamb_Score { get; set; }
        public Nullable<int> Institution_Choice_Id { get; set; }
        public Nullable<long> Application_Form_Id { get; set; }
        public Nullable<int> Subject1 { get; set; }
        public Nullable<int> Subject2 { get; set; }
        public Nullable<int> Subject3 { get; set; }
        public Nullable<int> Subject4 { get; set; }
        public Nullable<int> Subject1_Score { get; set; }
        public Nullable<int> Subject2_Score { get; set; }
        public Nullable<int> Subject3_Score { get; set; }
        public Nullable<int> Subject4_Score { get; set; }
    
        public virtual O_LEVEL_SUBJECT O_LEVEL_SUBJECT { get; set; }
        public virtual O_LEVEL_SUBJECT O_LEVEL_SUBJECT1 { get; set; }
        public virtual O_LEVEL_SUBJECT O_LEVEL_SUBJECT2 { get; set; }
        public virtual O_LEVEL_SUBJECT O_LEVEL_SUBJECT3 { get; set; }
        public virtual APPLICATION_FORM APPLICATION_FORM { get; set; }
        public virtual PERSON PERSON { get; set; }
        public virtual INSTITUTION_CHOICE INSTITUTION_CHOICE { get; set; }
    }
}
