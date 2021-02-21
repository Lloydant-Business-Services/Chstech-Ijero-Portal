using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentMatricNumberAssignment
    {
        public Faculty Faculty { get; set; }
        public Level Level { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Session Session { get; set; }
        public int MatricSerialNoStartFrom { get; set; }
        public string MatricNoStartFrom { get; set; }
        public string DepartmentCode { get; set; }
        public bool Used { get; set; }
        public int Id { get; set; }
        public int? DepartmentOptionId { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
    }


}
