using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public abstract class Setup
    {
        [Display(Name ="Fee Type")]
        [Required]
        public virtual int Id { get; set; }
                
        public virtual string Name { get; set; }
    }




}
