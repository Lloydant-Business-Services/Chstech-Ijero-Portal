using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{

    public class ECourse
    {
        public long Id { get; set; }
        public Course Course { get; set; }
        public EContentType EContentType { get; set; }
        public string Url { get; set; }
        public int? views { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Active { get; set; }
        public string VideoUrl { get; set; }
        public bool IsDelete { get; set; }
        public string LiveStreamLink { get; set; }




    }
}
