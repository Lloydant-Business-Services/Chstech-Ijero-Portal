using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentUpdateAudit
    {
        public long Id { get; set; }
        public Model.Student Student { get; set; }
        public User User { get; set; }
        public System.DateTime Date { get; set; }

    }
}
