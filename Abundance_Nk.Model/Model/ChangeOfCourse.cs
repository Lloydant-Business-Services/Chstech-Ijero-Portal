using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class ChangeOfCourse
    {
        public int Id { get; set; }
        public string JambRegistrationNumber { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public Person OldPerson{ get; set; }
        public Person NewPerson { get; set; }
        public Session Session { get; set; }
    }
}
