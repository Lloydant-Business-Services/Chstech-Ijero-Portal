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
    
    public partial class PAYMENT_VERIFICATION
    {
        public long Payment_Id { get; set; }
        public long User_Id { get; set; }
        public System.DateTime DateVerified { get; set; }
        public string Comments { get; set; }
    
        public virtual PAYMENT PAYMENT { get; set; }
        public virtual USER USER { get; set; }
    }
}
