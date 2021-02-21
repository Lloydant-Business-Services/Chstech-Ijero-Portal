using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicationProgrammeFee
    {
        public int Id { get; set; }
        public Programme Programme { get; set; }
        public FeeType FeeType { get; set; }
        public Session Session { get; set; }
        public DateTime DateEntered { get; set; }
    }




}
