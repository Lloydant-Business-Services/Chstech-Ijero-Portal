using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class StudentType : BasicSetup
    {
        [Display(Name="Student Type")]
        public override int Id { get; set; }

        [Display(Name = "Student Type")]
        public override string Name { get; set; }

        public enum EnumName
        {
            New = 1,
            Returning = 2,
            Prospective = 3
        }

       
    }

}
