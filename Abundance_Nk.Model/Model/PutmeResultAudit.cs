using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PutmeResultAudit : Audit
    {
        public long Id { get; set; }
        public int Result_Id { get; set; }
        public string Old_RegNo { get; set; }
        public string New_RegNo { get; set; }
       

    }
}
