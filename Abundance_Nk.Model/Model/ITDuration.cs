using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ITDuration : BasicSetup
    {
        [Required]
        [Display(Name = "IT Duration")]
        public override int Id { get; set; }
    }



}
