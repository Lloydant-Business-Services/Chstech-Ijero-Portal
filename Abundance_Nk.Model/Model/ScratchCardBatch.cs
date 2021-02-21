using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ScratchCardBatch
    {
        public long Id { get; set; }
        public ScratchCardType CardType { get; set; }
        public User EnteredBy { get; set; }
        public DateTime DateGenerated { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public byte UsageCountLimit { get; set; }
        public decimal Price { get; set; }
    }


}
