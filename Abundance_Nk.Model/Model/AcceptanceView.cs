using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AcceptanceView
    {
        public long Person_Id { get; set; }
        public string Name { get; set; }
        public string First_Choice_Department_Name { get; set; }
        public string Programme_Name { get; set; }

        public string Application_Form_Number { get; set; }
        public string Application_Exam_Number { get; set; }
        public string Invoice_Number { get; set; }
        public string RRR { get; set; }

        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public string Status { get; set; }
        public string DepartmentOption { get; set; }
        public string InvoiceDate { get; set; }
        public int Count { get; set; }
        public int HNDCount { get; set; }
        public int NDCount { get; set; }
        public int OTHERCount { get; set; }
        public int TotalCount { get; set; }
    }
}
