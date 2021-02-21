using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Entity.Model
{
    public class StudentDeferementLog
    {
        public long Id { get; set; }
        public Student Student { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public bool Rusticated { get; set; }
    }
}
