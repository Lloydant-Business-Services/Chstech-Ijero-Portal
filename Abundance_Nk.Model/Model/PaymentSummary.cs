using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentSummary
    {
        public long PersonId { get; set; }
        public string Name { get; set; }
        public string MatricNumber { get; set; }
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public int? LevelId { get; set; }
        public string LevelName { get; set; }
        public int? ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? FacultyId { get; set; }
        public string FacultyName { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string ConfirmationNumber { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string RRR { get; set; }
        public string Status { get; set; }
        public long? PaymentEtranzactId { get; set; }
    }
}
