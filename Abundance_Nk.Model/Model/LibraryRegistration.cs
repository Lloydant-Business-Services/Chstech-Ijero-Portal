using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class LibraryRegistration
    {
        public int Id { get; set; }
        public long Person_Id { get; set; }
        public long User_Id { get; set; }
        public int Session_Id { get; set; }
        public string Guardian_Name { get; set; }
        public string Previous_School_Name { get; set; }
        public DateTime Date { get; set; }

        public Person Person { get; set; }
        public User User { get; set; }
        public Session Session { get; set; }
    }
}
