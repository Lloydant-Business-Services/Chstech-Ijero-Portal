using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{

    public class EChatResponse
    {
        public long EChatResponseId { get; set; }
        public string Response { get; set; }
        public bool Active { get; set; }
        public System.DateTime Response_Time { get; set; }
        public string Upload { get; set; }

        public EChatTopic EChatTopic { get; set; }
        public User User { get; set; }
        public Student Student { get; set; }
    }
}
