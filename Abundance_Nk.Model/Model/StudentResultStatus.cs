using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
   public class StudentResultStatus
    {
       public int Id { get; set; }
       public Department Department { get; set; }
       public Level Level { get; set; }
       public  Programme Programme { get; set; }
       public bool Activated { get; set; }
    }
}
