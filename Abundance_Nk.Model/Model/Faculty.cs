using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Faculty : BasicSetup
    {
        [Display(Name = "School")]
        public override int Id { get; set; }

        [Display(Name = "School")]
        public override string Name { get; set; }
    }



}
