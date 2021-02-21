using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
   public class AdvancedAdmissionList
    {
        public long Person_Id { get; set; }
        public string FULL_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string PROGRAMME { get; set; }
        public int Programme_Id { get; set; }
        public int Department_Id { get; set; }
        public string APPLICATION_NUMBER { get; set; }
        public string Exam_Number { get; set; }
        public string JAMB_NUMBER { get; set; }
        public string TYPES { get; set; }
        public Nullable<System.DateTime> DATES { get; set; }
        public string Session_Name { get; set; }
        public int Session_Id { get; set; }
    }
}
