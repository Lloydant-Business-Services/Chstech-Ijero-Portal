using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
   public class RemitaRequestState
    {
        public Remita Remita { get; set; }
        public HttpWebRequest Request { get; set; }
        public Payment Payment { get; set; }
    }
}
