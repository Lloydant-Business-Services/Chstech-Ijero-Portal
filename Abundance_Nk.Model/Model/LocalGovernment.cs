using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
   public class LocalGovernment
    {
       [Display(Name = "Local Goverment")]
       public int Id { get; set; }

       [Display(Name = "Local Goverment")]
       public string Name { get; set; }
       public State State { get; set; }

    }


}
