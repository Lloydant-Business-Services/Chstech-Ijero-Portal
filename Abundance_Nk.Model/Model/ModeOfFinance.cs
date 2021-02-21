using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ModeOfFinance : BasicSetup
    {
        [Display(Name = "Mode of Finance")]
        public override int Id { get; set; }

        [Display(Name = "Mode of Finance")]
        public override string Name { get; set; }
    }



}
