using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentMatricNumberGenerationAudit
    {
        public long Id { get; set; }
        public string StartingMatricNumber { get; set; }
        public string EndingMatricNumber { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int? StartNumber { get; set; }
        public int? NumberLength { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public User User { get; set; }
        public string Operation { get; set; }
        public string Action { get; set; }
        public System.DateTime Time { get; set; }
        public string Client { get; set; }
    }
}
