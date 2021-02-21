using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class NewResultFormat
    {
        public NewResultFormat()
        {
            ResultSpecialCaseMessages = new ResultSpecialCaseMessages();
        }
        public int SN { get; set; }
        public string MATRICNO { get; set; }
        public decimal T_EX { get; set; }
        public decimal T_CA { get; set; }
        //public decimal EX_CA { get; set; }
        //public string fileUploadUrl { get; set; }
        
        public ResultSpecialCaseMessages ResultSpecialCaseMessages { get; set; }

    }
}
