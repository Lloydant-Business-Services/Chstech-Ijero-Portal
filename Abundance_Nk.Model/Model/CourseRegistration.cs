using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseRegistration
    {
        public long Id { get; set; }
        public Student Student { get; set; }
        public string Name { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public bool? Approved { get; set; }
        public Staff Approver { get; set; }
        public DateTime? DateApproved { get; set; }
        public List<CourseRegistrationDetail> Details { get; set; }
    }




}
