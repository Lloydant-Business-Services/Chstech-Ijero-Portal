using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
   public class FeeSetupFee
    {
        public int FeeSetUpFeeId { get; set; }
        public FeeSetup FeeSetUp { get; set; }
        public Fee Fee { get; set; }
        public bool Activated { get; set; }
    }
}
