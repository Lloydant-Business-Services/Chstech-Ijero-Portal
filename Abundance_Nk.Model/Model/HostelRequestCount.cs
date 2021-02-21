using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class HostelRequestCount
    {
        public int Id { get; set; }
        public Level Level { get; set; }
        public Sex Sex { get; set; }
        public long TotalCount { get; set; }
        public DateTime LastModified { get; set; }
        public bool Approved { get; set; }
        public DateTime DateSet { get; set; }
    }
}
