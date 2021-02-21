using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class Title : Setup
    {
        [Display(Name="Title")]
        public int Id { get; set; }
    }



}
