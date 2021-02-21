using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class FeeSetupAudit
    {
        public int FeeSetupAuditId { get; set; }
        public FeeSetup FeeSetup { get; set; }
        public string FeeSetUpName { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public FeeType FeeType { get; set; }
        public Session Session { get; set; }
        public decimal Amount { get; set; }
        public bool Activated { get; set; }
        public User User { get; set; }
        public string Action { get; set; }
        public string Client { get; set; }
        public System.DateTime DateCreated { get; set; }
    }
}
