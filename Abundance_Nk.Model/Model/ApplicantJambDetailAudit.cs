using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantJambDetailAudit
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string ApplicantJambRegistrationNumber { get; set; }
        public short? ApplicantJambScore { get; set; }
        public int? InstitutionChoiceId { get; set; }
        public long? ApplicationFormId { get; set; }
        public int? Subject1 { get; set; }
        public int? Subject2 { get; set; }
        public int? Subject3 { get; set; }
        public int? Subject4 { get; set; } 
        public int? Subject1Score { get; set; }
        public int? Subject2Score { get; set; }
        public int? Subject3Score { get; set; }
        public int? Subject4Score { get; set; }
        public long UserId { get; set; }
        public string Operation { get; set; }
        public string Action { get; set; }
        public System.DateTime Time { get; set; }
        public string Client { get; set; }
    }
}
