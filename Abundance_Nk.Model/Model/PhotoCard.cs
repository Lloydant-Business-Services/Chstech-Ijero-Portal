using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PhotoCard
    {
        public long AplicationNumber { get; set; }
        public string MatriculationNumber { get; set; }
        public long? AplicationSerialNumber { get; set; }
        public string AplicationFormNumber { get; set; }
        public long PersonId { get; set; }
        public string Name { get; set; }
        public string FirstChoiceFaculty { get; set; }
        public string SecondChoiceFaculty { get; set; }
        public string FirstChoiceDepartment { get; set; }
        public string SecondChoiceDepartment { get; set; }
        public string AppliedProgrammeName { get; set; }
        public string MobilePhone { get; set; }
        public string PassportUrl { get; set; }
        public string SessionName { get; set; }
        public string DepartmentName { get; set; }
        public long PaymentNumber { get; set; }

        public int? ExamSerialNumber { get; set; }
        public string ExamNumber { get; set; }
        public decimal TrasactionAmount { get; set; }
    }







}
