using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ProgrammeLevel
    {
        public int Id { get; set; }
        public Programme Programme { get; set; }
        public Level Level { get; set; }
        public bool Active { get; set; }
    }
}
