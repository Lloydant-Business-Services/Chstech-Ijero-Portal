using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class OLevelType : BasicSetup
    {
        [Display(Name = "O-Level Type")]
        public override int Id { get; set; }

        [Display(Name = "O-Level Type")]
        public override string Name { get; set; }
               
        public string ShortName { get; set; }
    }



}
