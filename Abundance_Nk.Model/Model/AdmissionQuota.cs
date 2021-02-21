using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionQuota
    {
        public int Id { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public User User { get; set; }
        public long Quota { get; set; }
        public long UnusedQuota { get; set; }
        public bool Active { get; set; }
    }
}
