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
    
    public partial class SHORT_FALL
    {
        public long Short_Fall_Id { get; set; }
        public long Payment_Id { get; set; }
        public double Amount { get; set; }
        public Nullable<int> Fee_Type_Id { get; set; }
        public Nullable<long> User_Id { get; set; }
        public string Fee_Reference { get; set; }
    
        public virtual FEE_TYPE FEE_TYPE { get; set; }
        public virtual PAYMENT PAYMENT { get; set; }
        public virtual USER USER { get; set; }
    }
}
