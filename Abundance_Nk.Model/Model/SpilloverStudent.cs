using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class SpilloverStudent
    {
        public long Id { get; set; }
        public Student Student { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public User UploadedBy { get; set; }
    }
}
