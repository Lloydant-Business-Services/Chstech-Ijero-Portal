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
    
    public partial class VW_PAYMENT_SUMMARY
    {
        public long Person_Id { get; set; }
        public string Name { get; set; }
        public string Matric_Number { get; set; }
        public int Session_Id { get; set; }
        public string Session_Name { get; set; }
        public int Fee_Type_Id { get; set; }
        public string Fee_Type_Name { get; set; }
        public Nullable<int> Level_Id { get; set; }
        public string Level_Name { get; set; }
        public Nullable<int> Programme_Id { get; set; }
        public string Programme_Name { get; set; }
        public Nullable<int> Department_Id { get; set; }
        public string Department_Name { get; set; }
        public Nullable<int> Faculty_Id { get; set; }
        public string Faculty_Name { get; set; }
        public Nullable<System.DateTime> Transaction_Date { get; set; }
        public string Invoice_Number { get; set; }
        public string Confirmation_Number { get; set; }
        public Nullable<decimal> Transaction_Amount { get; set; }
        public string RRR { get; set; }
        public string Status { get; set; }
        public Nullable<long> Payment_Etranzact_Id { get; set; }
    }
}
