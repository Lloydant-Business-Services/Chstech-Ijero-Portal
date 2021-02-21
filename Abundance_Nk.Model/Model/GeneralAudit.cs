using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class GeneralAudit
    {
        public long Id { get; set; }
        public string TableNames { get; set; }
        public string InitialValues { get; set; }
        public string CurrentValues { get; set; }
        public User User { get; set; }
        public string Operation { get; set; }
        public string Action { get; set; }
        public System.DateTime Time { get; set; }
        public string Client { get; set; }
    }
}
