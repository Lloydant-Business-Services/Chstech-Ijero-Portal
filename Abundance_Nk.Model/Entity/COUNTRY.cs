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
    
    public partial class COUNTRY
    {
        public COUNTRY()
        {
            this.DELIVERY_SERVICE_ZONE = new HashSet<DELIVERY_SERVICE_ZONE>();
            this.TRANSCRIPT_REQUEST = new HashSet<TRANSCRIPT_REQUEST>();
        }
    
        public string Country_Id { get; set; }
        public string Country_Name { get; set; }
    
        public virtual ICollection<DELIVERY_SERVICE_ZONE> DELIVERY_SERVICE_ZONE { get; set; }
        public virtual ICollection<TRANSCRIPT_REQUEST> TRANSCRIPT_REQUEST { get; set; }
    }
}
