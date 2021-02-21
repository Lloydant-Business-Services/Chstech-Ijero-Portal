using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentLevel
    {
        public long Id { get; set; }
        public Student Student { get; set; }
        public Person Person { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public Session Session { get; set; }
        public bool Active { get; set; }
        public string GraduationDate { get; set; }
        public string TranscriptDate { get; set; }

        //public SessionSemester SessionSemester { get; set; }
    }



}
