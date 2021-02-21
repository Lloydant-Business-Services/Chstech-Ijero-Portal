using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ProposeAdmission
    {
        public long Id { get; set; }

        public Person Person { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public User User { get; set; }
        public bool Active { get; set; }

    }
}
