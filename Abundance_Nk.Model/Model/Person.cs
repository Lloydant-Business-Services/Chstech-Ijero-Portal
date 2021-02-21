using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Person
    {
        private string name;
        private string fullName;

        public long Id { get; set; }
        public PersonType Type { get; set; }

        [Required]
        [Display(Name ="First Name")]
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

       [Required]
        [Display(Name = "Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mobile Phone")]
        [RegularExpression("^0[0-9]{10}$", ErrorMessage = "Phone number is not valid")]
        public string MobilePhone { get; set; }

        //[Required]
        //[DataType(DataType.Date)]
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
        public DateTime  DateEntered { get; set; }
        
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

        //public string StateId { get; set; }
        //public string SexId { get; set; }
        //public string LgaId { get; set; }
        //public LoginDetail LoginDetail { get; set; }
        //public StaffQualification Qualification { get; set; }



        //public string Initial { get; set; }
        //public string GetInitial(string lastName, string firstName, string otherName)
        //{
        //    try
        //    {
        //        string initial = "";
        //        if (!string.IsNullOrWhiteSpace(lastName))
        //        {
        //            initial += lastName.Substring(0, 1).ToUpper();
        //        }

        //        if (!string.IsNullOrWhiteSpace(firstName))
        //        {
        //            if (initial.Length > 0)
        //            {
        //                initial += ". " + firstName.Substring(0, 1).ToUpper();
        //            }
        //            else
        //            {
        //                initial += firstName.Substring(0, 1).ToUpper();
        //            }
        //        }

        //        if (!string.IsNullOrWhiteSpace(otherName))
        //        {
        //            if (initial.Length > 0)
        //            {
        //                initial += ". " + otherName.Substring(0, 1).ToUpper();
        //            }
        //            else
        //            {
        //                initial += otherName.Substring(0, 1).ToUpper();
        //            }
        //        }

        //        return initial;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}



    }
}
