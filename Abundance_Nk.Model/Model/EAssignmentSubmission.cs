using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class EAssignmentSubmission
    {
        public long Id { get; set; }
        public EAssignment EAssignment { get; set; }
        public Student Student { get; set; }
        public string AssignmentContent { get; set; }
        public string Remarks { get; set; }
        public decimal? Score { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string TextSubmission { get; set; }
    }
}
