using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentPaymentAudit 
    {
        public long Id { get; set; }
        public StudentPayment StudentPayment { get; set; }
        public Person Person { get; set; }
        public Session Session { get; set; }
        public Level Level { get; set; }
        public decimal Amount { get; set; }
        public bool Status { get; set; }
        public Person OldPerson { get; set; }
        public Level OldLevel { get; set; }
        public Session OldSession { get; set; }
        public decimal OldAmount { get; set; }
        public bool OldStatus {get; set;}
        public User User { get; set; }
        public string Operation { get; set; }
        public string Action { get; set; }
        public DateTime Time { get; set; }
        public string Client { get; set; }
    }
}
