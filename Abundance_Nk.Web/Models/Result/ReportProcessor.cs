using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Web.Models.Intefaces;

namespace Abundance_Nk.Web.Models.Result
{
    public class ReportProcessor
    {
        private IReport view;
        private ReportBase report;

        public ReportProcessor(IReport _view, ReportBase _report)
        {
            view = _view;
            report = _report;
        }

        public void DisplayResult()
        {
            try
            {
                report.GetData();
               
                if (report.NoResultFound())
                {
                    return;
                }

                report.SetPath();
                report.SetProperties();
                report.SetParameter();
                report.DisplayHelper();
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}