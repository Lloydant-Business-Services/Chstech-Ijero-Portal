using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{

    public class ECourseContentDownload
    {
        public long ECourseDownloadId { get; set; }
        public ECourse ECourse { get; set; }
        public DateTime? DateViewed { get; set; }
        public Person Person { get; set; }
    }
}
