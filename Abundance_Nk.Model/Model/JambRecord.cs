using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class JambRecord
    {
        public long Id { get; set; }
        public string JambRegistrationNumber { get; set; }
        public string CandidateName { get; set; }
        public string FirstChoiceInstitution { get; set; }
        public string Subject1 { get; set; }
        public string Subject2 { get; set; }
        public string Subject3 { get; set; }
        public string Subject4 { get; set; }
        public short? TotalJambScore { get; set; }
    }
}
