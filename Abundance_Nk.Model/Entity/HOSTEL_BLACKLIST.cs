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
    
    public partial class HOSTEL_BLACKLIST
    {
        public int Hostel_Blacklist_Id { get; set; }
        public long Person_Id { get; set; }
        public int Programme_Id { get; set; }
        public int Department_Id { get; set; }
        public int Level_Id { get; set; }
        public int Session_Id { get; set; }
        public string Reason { get; set; }
    
        public virtual DEPARTMENT DEPARTMENT { get; set; }
        public virtual LEVEL LEVEL { get; set; }
        public virtual SESSION SESSION { get; set; }
        public virtual STUDENT STUDENT { get; set; }
        public virtual PROGRAMME PROGRAMME { get; set; }
    }
}
