using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class EAssignment
    {
        public long Id { get; set; }
        public Course Course { get; set; }
        public string Assignment { get; set; }
        public string URL { get; set; }
        public string Instructions { get; set; }
        public DateTime DateSet { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }
        public string AssignmentinText { get; set; }
        public bool IsDelete { get; set; }
        public CourseAllocation CourseAllocation { get; set; }
        public bool Publish { get; set; }
    }
}
