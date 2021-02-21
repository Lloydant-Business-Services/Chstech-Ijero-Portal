using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abundance_Nk.Model.Model
{
    public class Staff : Person
    {
        public StaffType StaffType { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string ProfileDescription { get; set; }
        public User User { get; set; }

    }


}
