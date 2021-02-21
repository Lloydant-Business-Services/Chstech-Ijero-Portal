using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Country
    {
         [Display(Name = "Destination Country")]
        public string Id { get; set; }
        [Display(Name = "Destination Country")]
        public string CountryName { get; set; }

    }
}
