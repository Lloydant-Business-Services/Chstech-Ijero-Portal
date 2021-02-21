using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class HostelBlacklist
    {
        public int Id { get; set; }
        public Student Student { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Level Level { get; set; }
        public Session Session { get; set; }
        public string Reason { get; set; }
        
    }
}
