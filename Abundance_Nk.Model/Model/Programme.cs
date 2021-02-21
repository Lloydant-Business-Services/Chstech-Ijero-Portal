using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Programme : BasicSetup
    {
        [Display(Name="Programme")]
        public override int Id { get; set; }

        [Display(Name = "Programme")]
        public override string Name { get; set; }

        public string ShortName { get; set; }

        public bool? Activated { get; set; }
        public bool? IsActiveForApplication { get; set; }
    }



}
