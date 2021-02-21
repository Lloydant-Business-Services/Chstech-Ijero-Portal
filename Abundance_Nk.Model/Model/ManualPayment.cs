using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ManualPayment
    {
        public long ManualPayment_Id { get; set; }
        public long PersonId { get; set; }
        public int FeeTypeId { get; set; }
        public int SessionId { get; set; }
        public decimal Amount { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime DateApproved { get; set; }
        public long ApproverOfficerId { get; set; }

        public FeeType FeeType { get; set; }
        public Person Person { get; set; }
        public User User { get; set; }
        public Session Session { get; set; }
    }
}
