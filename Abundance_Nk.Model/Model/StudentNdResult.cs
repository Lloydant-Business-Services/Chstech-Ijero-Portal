using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentNdResult
    {
        public Student Student { get; set; }

        [Display(Name = "Date Awarded")]
        public DateTime DateAwarded { get; set; }

        public Value DayAwarded { get; set; }
        public Value MonthAwarded { get; set; }
        public Value YearAwarded { get; set; }
    }




}
