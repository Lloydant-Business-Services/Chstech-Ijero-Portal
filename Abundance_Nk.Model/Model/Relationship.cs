using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Relationship : Setup
    {
        [Display(Name="Relationship")]
        public override int Id { get; set; }

        [Display(Name = "Relationship")]
        public override string Name { get; set; }
    }




}
