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
    
    public partial class STUDENT_EMPLOYMENT_INFORMATION
    {
        public int Student_Employment_Information_Id { get; set; }
        public long Person_Id { get; set; }
        public string Place_Of_Last_Employment { get; set; }
        public System.DateTime Start_Date { get; set; }
        public System.DateTime End_Date { get; set; }
    
        public virtual STUDENT STUDENT { get; set; }
    }
}
