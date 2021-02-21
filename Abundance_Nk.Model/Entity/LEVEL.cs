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
    
    public partial class LEVEL
    {
        public LEVEL()
        {
            this.APPLICANT_LEVEL = new HashSet<APPLICANT_LEVEL>();
            this.COURSE = new HashSet<COURSE>();
            this.COURSE_ALLOCATION = new HashSet<COURSE_ALLOCATION>();
            this.COURSE_UNIT = new HashSet<COURSE_UNIT>();
            this.FEE_DETAIL = new HashSet<FEE_DETAIL>();
            this.FEE_DETAIL_AUDIT = new HashSet<FEE_DETAIL_AUDIT>();
            this.HOSTEL_ALLOCATION_COUNT = new HashSet<HOSTEL_ALLOCATION_COUNT>();
            this.HOSTEL_ALLOCATION_CRITERIA = new HashSet<HOSTEL_ALLOCATION_CRITERIA>();
            this.HOSTEL_BLACKLIST = new HashSet<HOSTEL_BLACKLIST>();
            this.HOSTEL_REQUEST = new HashSet<HOSTEL_REQUEST>();
            this.HOSTEL_REQUEST_COUNT = new HashSet<HOSTEL_REQUEST_COUNT>();
            this.PAYMENT_ETRANZACT_TYPE = new HashSet<PAYMENT_ETRANZACT_TYPE>();
            this.PROGRAMME_FEE_AMOUNT = new HashSet<PROGRAMME_FEE_AMOUNT>();
            this.PROGRAMME_LEVEL = new HashSet<PROGRAMME_LEVEL>();
            this.STUDENT_ACADEMIC_INFORMATION = new HashSet<STUDENT_ACADEMIC_INFORMATION>();
            this.STUDENT_COURSE_REGISTRATION = new HashSet<STUDENT_COURSE_REGISTRATION>();
            this.STUDENT_EXAM_RAW_SCORE_SHEET_RESULT = new HashSet<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT>();
            this.STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED = new HashSet<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED>();
            this.STUDENT_LEVEL = new HashSet<STUDENT_LEVEL>();
            this.STUDENT_MATRIC_NUMBER_ASSIGNMENT = new HashSet<STUDENT_MATRIC_NUMBER_ASSIGNMENT>();
            this.STUDENT_PAYMENT_AUDIT = new HashSet<STUDENT_PAYMENT_AUDIT>();
            this.STUDENT_PAYMENT_AUDIT1 = new HashSet<STUDENT_PAYMENT_AUDIT>();
            this.STUDENT_PAYMENT = new HashSet<STUDENT_PAYMENT>();
            this.STUDENT_RESULT = new HashSet<STUDENT_RESULT>();
            this.STUDENT_RESULT_STATUS = new HashSet<STUDENT_RESULT_STATUS>();
        }
    
        public int Level_Id { get; set; }
        public string Level_Name { get; set; }
        public string Level_Description { get; set; }
    
        public virtual ICollection<APPLICANT_LEVEL> APPLICANT_LEVEL { get; set; }
        public virtual ICollection<COURSE> COURSE { get; set; }
        public virtual ICollection<COURSE_ALLOCATION> COURSE_ALLOCATION { get; set; }
        public virtual ICollection<COURSE_UNIT> COURSE_UNIT { get; set; }
        public virtual ICollection<FEE_DETAIL> FEE_DETAIL { get; set; }
        public virtual ICollection<FEE_DETAIL_AUDIT> FEE_DETAIL_AUDIT { get; set; }
        public virtual ICollection<HOSTEL_ALLOCATION_COUNT> HOSTEL_ALLOCATION_COUNT { get; set; }
        public virtual ICollection<HOSTEL_ALLOCATION_CRITERIA> HOSTEL_ALLOCATION_CRITERIA { get; set; }
        public virtual ICollection<HOSTEL_BLACKLIST> HOSTEL_BLACKLIST { get; set; }
        public virtual ICollection<HOSTEL_REQUEST> HOSTEL_REQUEST { get; set; }
        public virtual ICollection<HOSTEL_REQUEST_COUNT> HOSTEL_REQUEST_COUNT { get; set; }
        public virtual ICollection<PAYMENT_ETRANZACT_TYPE> PAYMENT_ETRANZACT_TYPE { get; set; }
        public virtual ICollection<PROGRAMME_FEE_AMOUNT> PROGRAMME_FEE_AMOUNT { get; set; }
        public virtual ICollection<PROGRAMME_LEVEL> PROGRAMME_LEVEL { get; set; }
        public virtual ICollection<STUDENT_ACADEMIC_INFORMATION> STUDENT_ACADEMIC_INFORMATION { get; set; }
        public virtual ICollection<STUDENT_COURSE_REGISTRATION> STUDENT_COURSE_REGISTRATION { get; set; }
        public virtual ICollection<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT> STUDENT_EXAM_RAW_SCORE_SHEET_RESULT { get; set; }
        public virtual ICollection<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED> STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED { get; set; }
        public virtual ICollection<STUDENT_LEVEL> STUDENT_LEVEL { get; set; }
        public virtual ICollection<STUDENT_MATRIC_NUMBER_ASSIGNMENT> STUDENT_MATRIC_NUMBER_ASSIGNMENT { get; set; }
        public virtual ICollection<STUDENT_PAYMENT_AUDIT> STUDENT_PAYMENT_AUDIT { get; set; }
        public virtual ICollection<STUDENT_PAYMENT_AUDIT> STUDENT_PAYMENT_AUDIT1 { get; set; }
        public virtual ICollection<STUDENT_PAYMENT> STUDENT_PAYMENT { get; set; }
        public virtual ICollection<STUDENT_RESULT> STUDENT_RESULT { get; set; }
        public virtual ICollection<STUDENT_RESULT_STATUS> STUDENT_RESULT_STATUS { get; set; }
    }
}
