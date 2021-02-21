using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class OlevelResultAudit:Audit
    {
        public long Id { get; set; }
        public OLevelResult OLevelResult { get; set; }
        public Person Person { get; set; }
        public PersonType PersonType { get; set; }
        [Display(Name="Exam No")]
        public string ExamNumber { get; set; }
        [Required]
        [Display(Name = "Exam Year")]
        public int ExamYear { get; set; }
        public OLevelExamSitting Sitting { get; set; }
        public OLevelType Type { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        [Display(Name = "Scanned Copy")]
        public string ScannedCopyUrl { get; set; }
        public string OldExamNumber { get; set; }
        public int OldExamYear { get; set; }
        public string OldScannedCopyUrl { get; set; }
    }
}
