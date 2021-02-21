using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class MaritalStatus : Setup
    {
        [Display(Name = "Marital Status")]
        public override int Id { get; set; }

        [Display(Name = "Marital Status")]
        public override string Name { get; set; }
    }
}
