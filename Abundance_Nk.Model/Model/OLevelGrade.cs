using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class OLevelGrade : BasicSetup
    {
        //public int Id { get; set; }

        [Display(Name = "O-Level Grade")]
        public override string  Name { get; set; }

        //public string Description { get; set; }
    }


}
