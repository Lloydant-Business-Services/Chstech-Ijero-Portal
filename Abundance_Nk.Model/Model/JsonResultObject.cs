using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class JsonResultObject
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string UnusedQuota { get; set; }
    }
}
