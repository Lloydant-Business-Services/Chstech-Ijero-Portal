using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class PaymentScholarship
    {
        public long Id { get; set; }
        public Person person { get; set; }
        public Session session { get; set; }
        public Decimal Amount { get; set; }

        [Display(Name="Scholarship Name")]
        public string ScholarshipName { get; set; }
    }
}
