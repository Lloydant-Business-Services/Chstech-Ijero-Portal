using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionCriteriaForOLevelType
    {
        //[Display(Name = "O-Level Type Admission Criteria")]
        public int Id { get; set; }
        public AdmissionCriteria MainCriteria { get; set; }
        public OLevelType OLevelType { get; set; }
    }



}
