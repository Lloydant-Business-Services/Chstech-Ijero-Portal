
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abundance_Nk.Model.Model
{
    public class Role : BasicSetup
    {
        public enum EnumName
        {
            Admin = 1,
            Guest = 2,
            Teacher = 3,
            Parent = 4,
            Student = 5,
            Applicant = 6
        }
               
        public PersonRight UserRight { get; set; }
        public List<Right> Rights { get; set; }
        public bool HasUser { get; set; }
    }




}
