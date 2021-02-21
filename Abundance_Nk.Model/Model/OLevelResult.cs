using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class OLevelResult
    {
        public long Id { get; set; }
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
        [Display(Name = "Scratch Card Pin/Token")]
        public string ScratchCard { get; set; }
        [Display(Name = "Scratch Card Serial Number")]
        public string ScratchCardSerialNo { get; set; }
    }


}
