using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class OlevelResultdDetailsAudit:Audit
    {
        public long Id { get; set; }
        public OLevelResultDetail OLevelResultDetail { get; set; }
        public OLevelResult Header { get; set; }
        public OLevelSubject Subject { get; set; }
        public OLevelGrade Grade { get; set; }
    }
}
