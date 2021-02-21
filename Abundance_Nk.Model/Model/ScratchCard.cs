using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ScratchCard
    {
        public long Id { get; set; }
        public ScratchCardBatch Batch { get; set; }
        public string SerialNumber { get; set; }
        public string Pin { get; set; }
        public byte UsageCount { get; set; }
        public DateTime FirstUsedDate { get; set; }
        public Person person { get; set; }

    }




}
