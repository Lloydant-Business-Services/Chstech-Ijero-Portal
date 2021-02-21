using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ProgrammeDepartment
    {
        public int Id { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
    }



}
