using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ResultFormat
    {
        public ResultFormat()
        {
            ResultSpecialCaseMessages = new ResultSpecialCaseMessages();
        }
        public int SN { get; set; }
        public string MATRICNO { get; set; }
        public decimal QU1 { get; set; }
        public decimal QU2 { get; set; }
        public decimal QU3 { get; set; }
        public decimal QU4 { get; set; }
        public decimal QU5 { get; set; }
        public decimal QU6 { get; set; }
        public decimal QU7 { get; set; }
        public decimal QU8 { get; set; }
        public decimal QU9 { get; set; }
        public decimal T_EX { get; set; }
        public decimal T_CA { get; set; }
        public decimal EX_CA { get; set; }
        public ResultSpecialCaseMessages ResultSpecialCaseMessages { get; set; }
        public string fileUploadUrl { get; set; }
    }

    
    public class CBTResultFormat
    {
        public int SN { get; set; }
        public string MATRICNO { get; set; }
        public string STUDENTNAME { get; set; }
        public decimal SCORE { get; set; }
    }



}
