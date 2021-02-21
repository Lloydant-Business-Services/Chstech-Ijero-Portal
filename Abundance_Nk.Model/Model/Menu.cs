using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class Menu
    {
        public long Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string DisplayName { get; set; }
        public bool Activated { get; set; }
        public MenuGroup MenuGroup { get; set; }
    }
}
