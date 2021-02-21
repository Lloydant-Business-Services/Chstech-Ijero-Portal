using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Entity.Model
{
    public class DuplicateApplicationNumber
    {
        public long ApplicationId { get; set; }
        public long? SerialNumber { get; set; }
        public string Number { get; set; }
        public int ApplicationFormSetting { get; set; }
        public int ApplicationProgrammeFee { get; set; }
        public long PersonId { get; set; }
        public long PaymentId { get; set; }
        public bool Rejected { get; set; } 
        public int? ExamSerialNumber { get; set; }
        public string ExamNumber { get; set; }
        public string RejectReason { get; set; }
        public long? AdmissionListId { get; set; }
        public long? AdmissionListBatchId { get; set; }
        public long? DeprtmentId { get; set; }
        public long? DepartmentOptionId { get; set; }
        public int? SessionId { get; set; }
        public bool? Activated { get; set; }
    }
}
