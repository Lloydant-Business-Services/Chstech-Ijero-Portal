using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class EChatTopic
    {
        public long EChatTopicId { get; set; }
        public bool Active { get; set; }
        public CourseAllocation CourseAllocation { get; set; }
    }
}
