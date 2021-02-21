using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Session : Setup
    {
        [Display(Name="Session")]
        public override int Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
            }
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? Activated { get; set; }
        public bool? ActiveForResult { get; set; }
        public bool? ActiveForAllocation { get; set; }
        public bool? ActiveForApplication { get; set; }
        public bool? ActiveForFees { get; set; }
        public bool? IsLateRegistration { get; set; }
    }
}
