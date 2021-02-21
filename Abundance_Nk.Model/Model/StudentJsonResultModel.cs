using Abundance_Nk.Model.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentJsonResultModel : JsonResultModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string ApplicationNumber { get; set; }
        public string MatricNumber { get; set; }
        public string Status { get; set; }
        public bool Select { get; set; }
    }
}
