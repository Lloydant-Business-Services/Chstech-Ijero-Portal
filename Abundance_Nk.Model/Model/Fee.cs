using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace Abundance_Nk.Model.Model
{
    public class Fee : BasicSetup
    {
        [Display(Name = "Fee Type")]
        public override int Id { get; set; }

        [Required]
        [Display(Name = "Fee Type")]
        public override string Name { get; set; }

        [Required]
        public decimal Amount { get; set; }

        //[Required]
        //[DataType(DataType.Date)]
        //[DisplayName("Date Entered")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        //public DateTime DateEntered { get; set; }
    }





}
