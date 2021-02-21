using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class PutmeResult
    {
        public int Id { get; set; }

        [RegularExpression("^(5)([0-9]{7}[A-Z]{2})$", ErrorMessage = "JAMB Registration No is not valid")]
        public string RegNo { get; set; }

        public string ExamNo { get; set; }

        public string FullName { get; set; }

        public string Jambscore { get; set; }

        public string Course { get; set; }

        public double? RawScore { get; set; }

        public double? Total { get; set; }

        public Person Person { get; set; }
        public Session Session { get; set; }
        public string Programme { get; set; }

    }
}
