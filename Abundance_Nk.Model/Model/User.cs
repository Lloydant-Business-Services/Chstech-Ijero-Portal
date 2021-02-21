using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public SecurityQuestion SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public Role Role { get; set; }
        public DateTime LastLoginDate { get; set; }
    }


}
