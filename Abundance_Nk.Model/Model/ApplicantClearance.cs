using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantClearance
    {
        public ApplicationForm ApplicationForm { get; set; }
        public bool Cleared { get; set; }
        public DateTime DateCleared { get; set; }
        public User User { get; set; }
    }


}
