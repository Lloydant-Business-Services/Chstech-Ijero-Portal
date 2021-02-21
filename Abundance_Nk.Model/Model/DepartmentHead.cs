using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class DepartmentHead
    {
        public int DepartmentHeadId { get; set; }
        public long PersonId { get; set; }
        public DateTime Date { get; set; }
        public int Department_Id { get; set; }

        public virtual Department Department { get; set; }
        public virtual Person Person { get; set; }
    }
}
