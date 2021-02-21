using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Models.Result
{
    public abstract class ReportBase
    {
        protected string reportName;

        public abstract void SetPath();
        public abstract void GetData();
        public abstract void SetProperties();
        public abstract void DisplayHelper();
        public abstract void SetParameter();
        public abstract bool NoResultFound();

    }




}