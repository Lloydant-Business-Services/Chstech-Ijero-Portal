using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class RegistrationLogonViewModel
    {
        public RegistrationLogonViewModel()
        {
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
        }

        public string ConfirmationOrderNumber { get; set; }
        public Session Session { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
    }
}

