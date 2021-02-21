using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class MenuInRole
    {
        public int Id { get; set; }
        public Menu Menu { get; set; }
        public bool Activated { get; set; }
        public Role Role { get; set; }
    }
}
