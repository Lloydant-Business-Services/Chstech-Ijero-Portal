using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class NotificationsViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool Active { get; set; }
        public bool IsDelete { get; set; }
        public List<Notifications> GetNotifications { get; set; }
        public Notifications Notifications { get; set; }
    }
}