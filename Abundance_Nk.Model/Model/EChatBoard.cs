using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class EChatBoard
    {
        public string Sender { get; set; }
        public string DateSent { get; set; }
        public string Response { get; set; }
        public bool ActiveSender { get; set; }
        public string Topic { get; set; }
        public string FilePath { get; set; }
    }
}
