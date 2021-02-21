using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class SetupViewModel
    {
        public List<Session> SessionList { get; set; }
        public List<GeneralAudit> GeneralAuditList { get; set; }
        public bool ShowTable { get; set; }
    }
}