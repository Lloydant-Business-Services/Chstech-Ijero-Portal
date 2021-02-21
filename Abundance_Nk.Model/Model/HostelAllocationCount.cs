using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class HostelAllocationCount
    {
        public int Id { get; set; }
        public Level Level { get; set; }
        public Sex Sex { get; set; }
        public long TotalCount { get; set; }
        public long Free { get; set; }
        public long Reserved { get; set; }
        public DateTime LastModified { get; set; }
        public bool Activated { get; set; }
        public DateTime DateSet { get; set; }
    }
}
