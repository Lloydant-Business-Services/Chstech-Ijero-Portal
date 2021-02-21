using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class StudentExtraYearSession
    {
        public long Id { get; set; }
        public Person Person { get; set; }
        [Display(Name="Number of Sessions Registered")]
        public int Sessions_Registered { get; set; }
        [Display(Name = "Current Session")]
        public Session Session { get; set; }
        [Display(Name = "Last Session Registered")]
        public Session LastSessionRegistered { get; set; }
        [Display(Name = "Deferred Session")]
        public Session DeferementCommencedSession { get; set; }
    }
}
