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
    
    public partial class VW_PAYMENT_OLD_STUDENT_FULL
    {
        public long Person_Id { get; set; }
        public string Name { get; set; }
        public string Image_File_Url { get; set; }
        public string Signature_File_Url { get; set; }
        public string Matric_Number { get; set; }
        public int Department_Id { get; set; }
        public string Department_Name { get; set; }
        public int Programme_Id { get; set; }
        public string Programme_Name { get; set; }
        public int Session_Id { get; set; }
        public string Session_Name { get; set; }
        public Nullable<int> Admitted_Session_Id { get; set; }
        public string Admitted_Session_Name { get; set; }
        public int Payment_Mode_Id { get; set; }
        public Nullable<decimal> Transaction_Amount { get; set; }
        public string Confirmation_No { get; set; }
        public Nullable<System.DateTime> Transaction_Date { get; set; }
    }
}
