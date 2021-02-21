using Abundance_Nk.Model.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class Hostel
    {
        [Display(Name = "Hostel")]
        public int Id { get; set; }
        [Display(Name = "Hostel")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public bool Activated { get; set; }
        public DateTime DateEntered { get; set; }
        public virtual HostelType HostelType { get; set; }
    }
}
