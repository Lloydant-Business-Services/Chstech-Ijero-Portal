using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionListBatch
    {
        public int Id { get; set; }
        public AdmissionListType Type { get; set; }
        public DateTime DateUploaded { get; set; }
        public string IUploadedFilePath { get; set; }
    }



}
