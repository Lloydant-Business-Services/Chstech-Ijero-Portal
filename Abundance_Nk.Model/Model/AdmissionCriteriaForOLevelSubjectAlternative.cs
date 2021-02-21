using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionCriteriaForOLevelSubjectAlternative
    {
        public int Id { get; set; }
        public AdmissionCriteriaForOLevelSubject Alternative { get; set; }
        public OLevelSubject OLevelSubject { get; set; }
    }




}
