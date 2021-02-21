using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentView
    {
        public long PersonId { get; set; }
        public long PaymentId { get; set; }
        public string InvoiceNumber { get; set; }
        public string ReceiptNumber { get; set; }
        public string ConfirmationOrderNumber { get; set; }

        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        public DateTime InvoiceGenerationDate { get; set; }
        public string InvoiceGenerationDateStr { get; set; }
        public DateTime? PaymentDate { get; set; }

        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }

        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }

        public decimal? Amount { get; set; }

        public string TransactionDate { get; set; }

        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string MatricNumber { get; set; }
        public string MobilePhone { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public decimal? SecondInstallmentAmount { get; set; }
        public string SecondInstallmentDate { get; set; }
        public decimal? FirstInstallmentAmount { get; set; }
        public string FirstInstallmentDate { get; set; }
        public decimal? FullPaymentAmount { get; set; }
        public string FullPaymentDate { get; set; }
        public decimal? SumFirstInstallment { get; set; }
        public decimal? SumSecondInstallment { get; set; }
        public decimal? SumFullPayment { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public DateTime TransDate { get; set; }
        public DateTime? TransDateNullable { get; set; }

        public int PaymentCount { get; set; }
        public int NewStudentDebtorsCount { get; set; }
        public int OldStudentDebtorsCount { get; set; }
        public int TotalDebtorsCount { get; set; }
        public int NDCount { get; set; }
        public int HNDCount { get; set; }
        public int TotalCount { get; set; }
        public int OtherProgrammesCount { get; set; }
        


    }
}
