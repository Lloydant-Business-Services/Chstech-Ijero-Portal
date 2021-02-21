using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class OLevelSubject
    {
        public int Id { get; set; }

        [Display(Name = "O-Level Subject")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int Rank { get; set; }

        public bool IsChecked { get; set; }
    }


}
