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
    
    public partial class ABILITY
    {
        public ABILITY()
        {
            this.APPLICANT = new HashSet<APPLICANT>();
        }
    
        public int Ability_Id { get; set; }
        public string Ability_Name { get; set; }
        public string Ability_Description { get; set; }
    
        public virtual ICollection<APPLICANT> APPLICANT { get; set; }
    }
}
