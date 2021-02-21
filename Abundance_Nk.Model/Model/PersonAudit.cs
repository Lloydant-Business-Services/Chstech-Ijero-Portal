using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class PersonAudit : Audit
    {
        private string name;
        private string fullName;

        public long Id { get; set; }
        public Person Person { get; set; }
        public PersonType PersonType { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string LastName { get; set; }

        [Display(Name = "Other Name")]
        public string OtherName { get; set; }

        public string ImageFileUrl { get; set; }
        public string SignatureFileUrl { get; set; }

        public Sex Sex { get; set; }

        [Display(Name = "Contact Address")]
        public string ContactAddress { get; set; }


        [Display(Name = "Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mobile Phone")]
        [RegularExpression("^0[0-9]{10}$", ErrorMessage = "Phone number is not valid")]
        public string MobilePhone { get; set; }
        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        public Value DayOfBirth { get; set; }
        public Value MonthOfBirth { get; set; }
        public Value YearOfBirth { get; set; }

        public State State { get; set; }
        public string Title { get; set; }
        public string Initial { get; set; }
        public string HomeTown { get; set; }

        [Display(Name = "Permanent Address")]
        public string HomeAddress { get; set; }
        public Nationality Nationality { get; set; }
        public DateTime? DateEntered { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get { return LastName + " " + FirstName + " " + OtherName; }
            set { fullName = value; }
        }
        [Display(Name = "Name")]
        public string Name
        {
            get { return LastName + " " + FirstName; }
            set { name = value; }
        }
        public Role Role { get; set; }
        public LocalGovernment LocalGovernment { get; set; }
        public Religion Religion { get; set; }

        public PersonType OldPersonType { get; set; }
        public string OldFirstName { get; set; }
        public string OldLastName { get; set; }
        public string OldOtherName { get; set; }
        public string OldImageFileUrl { get; set; }
        public string OldSignatureFileUrl { get; set; }
        public Sex OldSex { get; set; }
        public string OldContactAddress { get; set; }
        public string OldEmail { get; set; }
        public string OldMobilePhone { get; set; }
        public DateTime? OldDateOfBirth { get; set; }
        public Value OldDayOfBirth { get; set; }
        public Value OldMonthOfBirth { get; set; }
        public Value OldYearOfBirth { get; set; }
        public State OldState { get; set; }
        public string OldTitle { get; set; }
        public string OldInitial { get; set; }
        public string OldHomeTown { get; set; }
        public string OldHomeAddress { get; set; }
        public Nationality OldNationality { get; set; }
        public DateTime? OldDateEntered { get; set; }
        public Role OldRole { get; set; }
        public LocalGovernment OldLocalGovernment { get; set; }
        public Religion OldReligion { get; set; }

       
    }
}
