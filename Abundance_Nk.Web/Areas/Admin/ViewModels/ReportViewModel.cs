using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
            Department = new Department();
            Person = new Person();
            Programme = new Programme();
            Faculty = new Faculty();
            Session = new Session();
            Semester = new Semester();
            Course = new Course();
            Level = new Level();
        }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Faculty Faculty { get; set; }
        public Person Person { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public Level Level { get; set; }
    }
}