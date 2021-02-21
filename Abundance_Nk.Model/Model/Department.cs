using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Department : Setup
    {
        [Display(Name = "Course of Study")]
        public override int Id { get; set; }

        [Display(Name = "Course of Study")]
        public override string Name { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        public Faculty Faculty { get; set; }
        public bool? Active { get; set; }
    }




}
