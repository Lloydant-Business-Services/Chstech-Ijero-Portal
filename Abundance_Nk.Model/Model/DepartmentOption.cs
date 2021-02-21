using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class DepartmentOption : Setup
    {
        [Display(Name = "Course Option")]
        public override int Id { get; set; }

        [Display(Name = "Course Option")]
        public override string Name { get; set; }

        [Display(Name = "Course Option Status")]
        public bool Activated { get; set; }
        public Department Department { get; set; }
    }



}
