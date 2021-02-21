using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CardPayment
    {
        public Payment Payment { get; set; }
        public ScratchCard Card { get; set; }
        public DateTime? TransactionDate { get; set; }
    }



}
