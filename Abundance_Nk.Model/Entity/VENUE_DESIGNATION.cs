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
    
    public partial class VENUE_DESIGNATION
    {
        public int Id { get; set; }
        public int Department_id { get; set; }
        public int Programme_id { get; set; }
        public string Venue { get; set; }
        public System.DateTime Exam_Date { get; set; }
    
        public virtual DEPARTMENT DEPARTMENT { get; set; }
        public virtual PROGRAMME PROGRAMME { get; set; }
    }
}
