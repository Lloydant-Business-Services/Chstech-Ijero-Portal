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
    
    public partial class STUDENT_RESULT
    {
        public STUDENT_RESULT()
        {
            this.STUDENT_RESULT_DETAIL = new HashSet<STUDENT_RESULT_DETAIL>();
        }
    
        public long Student_Result_Id { get; set; }
        public int Student_Result_Type_Id { get; set; }
        public int Level_Id { get; set; }
        public int Programme_Id { get; set; }
        public int Department_Id { get; set; }
        public int Session_Semester_Id { get; set; }
        public Nullable<int> Maximum_Score_Obtainable { get; set; }
        public long Uploader_Id { get; set; }
        public System.DateTime Date_Uploaded { get; set; }
        public string Uploaded_File_Url { get; set; }
    
        public virtual CLASS_ROOM CLASS_ROOM { get; set; }
        public virtual DEPARTMENT DEPARTMENT { get; set; }
        public virtual LEVEL LEVEL { get; set; }
        public virtual ICollection<STUDENT_RESULT_DETAIL> STUDENT_RESULT_DETAIL { get; set; }
        public virtual STUDENT_RESULT_TYPE STUDENT_RESULT_TYPE { get; set; }
        public virtual USER USER { get; set; }
        public virtual SESSION_SEMESTER SESSION_SEMESTER { get; set; }
        public virtual PROGRAMME PROGRAMME { get; set; }
    }
}
