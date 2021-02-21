using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class FeeSetup
    {
        public int FeeSetupId { get; set; }
        public string FeeSetUpName { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public FeeType FeeType { get; set; }
        public Session Session { get; set; }
        public decimal Amount { get; set; }
        public bool Activated { get; set; }
        public User User { get; set; }
        public System.DateTime DateCreated { get; set; }
    }
}
