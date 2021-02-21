using Abundance_Nk.Model.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class HostelRoomCorner
    {
        [Display(Name = "Corner")]
        public int Id { get; set; }
        [Display(Name="Corner")]
        public string Name { get; set; }
        public bool Activated { get; set; }
        public virtual HostelRoom Room { get; set; }
    }
}
