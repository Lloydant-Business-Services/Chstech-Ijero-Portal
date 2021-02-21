using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class Notifications
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool Active { get; set; }
        public bool IsDelete { get; set; }
    }
}
