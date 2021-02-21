using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
   public class State
   {
       [Required]
       [Display(Name = "State")]
       public string Id { get; set; }

       [Display(Name="State")]
       public string Name { get; set; }
       public Nationality Nationality { get; set; }
    }



}
