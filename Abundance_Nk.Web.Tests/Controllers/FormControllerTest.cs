using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Areas.Applicant.Controllers;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Tests.Controllers
{
    [TestClass]
    public class FormControllerTest
    {
        [TestMethod]
        public void TestPostJambProgramme()
        {
            //
            PostJAMBProgrammeViewModel viewModel = new PostJAMBProgrammeViewModel();
            viewModel.ConfirmationOrderNumber = "03363454971438686629890";
            var controller = new FormController();
           // var result = controller.PostJambProgramme(viewModel) as ViewResult;
            var result = controller.PostJambProgramme(viewModel) as RedirectToRouteResult;
            result.RouteValues["Action"].Equals("PostJambForm");
        }
        [TestMethod]
        public void TestPostJambFormInvoiceGeneration()
        {
            PostJAMBFormPaymentViewModel viewModel = new PostJAMBFormPaymentViewModel();
            viewModel.Programme = new Programme() {Id = 1};
            viewModel.AppliedCourse.Department = new Department() {Id = 1};
            viewModel.Person = new Person();
            viewModel.Person.LastName = "Unit";
            viewModel.Person.FirstName = "Test";
            viewModel.Person.State = new State(){Id = "EN"};
            viewModel.Person.MobilePhone = "09021234578";
            viewModel.Person.Email = "me@gmail.com";
            viewModel.JambRegistrationNumber = "65367263HH";
            var controller = new FormController();
            var result = controller.PostJambFormInvoiceGeneration(viewModel) as RedirectToRouteResult;
            result.RouteValues["Action"].Equals("Index");
        }

        [TestMethod]
        public void TestModifyOlevelResult()
        {
            OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
            List<OLevelResultDetail> oLevelResultDetails = new  List<OLevelResultDetail>();
            OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
            OLevelResult oLevelResult = oLevelResultLogic.GetModelBy(a => a.Person_Id == 30350);
            if (oLevelResult != null)
            {
                oLevelResult.ExamYear = 2020;
                oLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(a => a.Applicant_O_Level_Result_Id == oLevelResult.Id);
                if (oLevelResultDetails.Count > 0)
                {
                    oLevelResultDetails.FirstOrDefault().Subject.Id = 2;
                    var controller = new FormController();
                    controller.ModifyOlevelResult(oLevelResult, oLevelResultDetails);

                   OLevelResult oLevelResultChanged = oLevelResultLogic.GetModelBy(a => a.Person_Id == 30350);
                   List<OLevelResultDetail> oLevelResultDetailsChanged = oLevelResultDetailLogic.GetModelsBy(a => a.Applicant_O_Level_Result_Id == oLevelResultChanged.Id);
               

                   Assert.AreEqual(oLevelResult.ExamYear,oLevelResultChanged.ExamYear);
                    var oLevelResultDetail = oLevelResultDetails.FirstOrDefault();
                    if (oLevelResultDetail != null)
                        Assert.AreEqual(oLevelResultDetail.Subject.Id,oLevelResultDetailsChanged.FirstOrDefault().Subject.Id);
                }
            }
        }
    }
}
