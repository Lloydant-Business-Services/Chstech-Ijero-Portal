using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Abundance_Nk.Model.Model
{
    public class Student : Person
    {
        public StudentType Type { get; set; }
        public StudentCategory Category { get; set; }
        public StudentStatus Status { get; set; }
        public Person Person { get; set; }

        public long? Number { get; set; }

        [Display(Name="Matric Number")]
        public string MatricNumber { get; set; }
        public ApplicationForm ApplicationForm { get; set; }

        [Display(Name = "Title")]
        public Title Title { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public Genotype Genotype { get; set; }

        [Display(Name = "Hall/Off Campus Address")]
        public string SchoolContactAddress { get; set; }
        public bool? Activated { get; set; }
        public string RejectCategory { get; set; }
        public string Reason { get; set; }

        public string PasswordHash { get; set; }
        public bool Select { get; set; }

    }
}
