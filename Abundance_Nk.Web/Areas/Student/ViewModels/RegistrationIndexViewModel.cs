using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class RegistrationIndexViewModel
    {
        public RegistrationIndexViewModel()
        {
            PaymentHistory = new PaymentHistory();
            Student = new Model.Model.Student();
            StudentLevel = new StudentLevel();
            Session = new Session();
            Payment = new Payment();
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            SemesterSelectListItem = Utility.PopulateSemesterSelectListItem();
        }

        public PaymentHistory PaymentHistory { get; set; }
        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public Session Session { get; set; }
        public Session CourseRegistrationSession { get; set; }
        public Payment Payment { get; set; }
        public bool isExtraYearStudent { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> SemesterSelectListItem { get; set; }
        public Semester Semester { get; set; }
    }
}