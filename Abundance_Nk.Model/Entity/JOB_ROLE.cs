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
    
    public partial class JOB_ROLE
    {
        public JOB_ROLE()
        {
            this.STAFF_JOB_ROLE = new HashSet<STAFF_JOB_ROLE>();
        }
    
        public int Job_Role_Id { get; set; }
        public string Job_Role_Name { get; set; }
    
        public virtual ICollection<STAFF_JOB_ROLE> STAFF_JOB_ROLE { get; set; }
    }
}
