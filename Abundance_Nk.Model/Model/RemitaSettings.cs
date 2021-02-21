using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class RemitaSettings
    {
        public int Payment_SettingId { get; set; }
        public string MarchantId { get; set; }
        public string Api_key { get; set; }
        public string serviceTypeId { get; set; }
        public string Response_Url { get; set; }
        public string Description { get; set; }
        
    }
}
