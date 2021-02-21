using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class Slug
    {
        public long AplicationNumber { get; set; }
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
        public long PaymentNumber { get; set; }
        public string JambNumber { get; set; }
        public Int16? JambScore { get; set; }
        public string ExamNumber { get; set; }
    }
}
