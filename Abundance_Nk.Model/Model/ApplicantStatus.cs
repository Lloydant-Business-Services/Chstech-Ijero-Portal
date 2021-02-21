using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantStatus : BasicSetup
    {
        [Display(Name = "Applicant Status")]
        public override int Id { get; set; }

        public enum Status
        {
            //SubmittedApplicationForm = 1,
            //OfferedAdmission = 2,
            //GeneratedAcceptanceInvoice = 3,
            //GeneratedAcceptanceReceipt = 4,
            //GeneratedSchoolFeesInvoice = 5,
            //GeneratedSchoolFeesReceipt = 6,
            //CompletedStudentInformationForm = 7,
            //Cleared = 8,


            SubmittedApplicationForm = 1,
            OfferedAdmission = 2,
            GeneratedAcceptanceInvoice = 3,
            GeneratedAcceptanceReceipt = 4,
            OLevelResultVerified = 5,
            ClearedAndAccepted = 6,
            ClearedAndRejected = 7,
            GeneratedSchoolFeesInvoice = 8,
            GeneratedSchoolFeesReceipt = 9,
            CompeledCourseRegistrationForm = 10,
            CompletedStudentInformationForm = 11,
        }



    }

}
