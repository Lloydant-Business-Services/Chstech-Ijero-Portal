using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PostUtmeResult 
    {
        public long Id { get; set; }
        public string Regno { get; set; }
        public string Examno { get; set; }
        public string Fullname { get; set; }
        public string Sex { get; set; } 
        public string JambScore { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public string Course { get; set; }
        public long? Eng { get; set; }
        public string Sub2 { get; set; }
        public long? Scr2 { get; set; }
        public string Sub3 { get; set; }
        public long? Scr3 { get; set; }
        public string Sub4 { get; set; }
        public long? Scr4 { get; set; }
        public long? Total { get; set; }
        public long? Average { get; set; }

    }
}
