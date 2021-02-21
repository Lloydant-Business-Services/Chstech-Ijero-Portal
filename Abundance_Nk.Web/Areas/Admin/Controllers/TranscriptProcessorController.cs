using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Controllers;
using System.Transactions;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class TranscriptProcessorController : BaseController
    {
        TranscriptProcessorViewModel viewModel;
        // GET: Admin/TranscriptProcessor
        public ActionResult Index()
        {
            Person person;
            viewModel = new TranscriptProcessorViewModel();
            TranscriptRequestLogic requestLogic = new TranscriptRequestLogic();
            PersonLogic personLogic = new PersonLogic();
            viewModel.transcriptRequests = requestLogic.GetAll();
            PopulateDropDown(viewModel);

            try
            {
                for (int i = 0; i < viewModel.transcriptRequests.Count(); i++)
                {
                    person = personLogic.GetAll().Where(p => p.Id == viewModel.transcriptRequests[i].student.Id).FirstOrDefault();
                    viewModel.transcriptRequests[i].student.FirstName = person.FirstName;
                    viewModel.transcriptRequests[i].student.LastName = person.LastName;
                    viewModel.transcriptRequests[i].student.OtherName = person.OtherName;
                    viewModel.transcriptRequests[i].student.FullName = person.FullName;
                }
                return View(viewModel);
            }
            catch (Exception)
            {

                return View(viewModel);
            }
        }

        private void PopulateDropDown(TranscriptProcessorViewModel viewModel)
        {
            int i = 0;
            foreach (TranscriptRequest t in viewModel.transcriptRequests)
            {
                ViewData["status" + i] = new SelectList(viewModel.transcriptSelectList, Utility.VALUE, Utility.TEXT, t.transcriptStatus.TranscriptStatusId);
                ViewData["clearanceStatus" + i] = new SelectList(viewModel.transcriptClearanceSelectList, Utility.VALUE, Utility.TEXT, t.transcriptClearanceStatus.TranscriptClearanceStatusId);
                i++;
            }
        }
    
        public ActionResult UpdateStatus(long tid, long stat)
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                tRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = (int)stat };
                transcriptRequestLogic.Modify(tRequest);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }
    
        public ActionResult Clearance ()
        {
            try
            {
                 viewModel = new TranscriptProcessorViewModel();
                 TranscriptRequestLogic requestLogic = new TranscriptRequestLogic();
                 viewModel.transcriptRequests = requestLogic.GetAll();
                 PopulateDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(viewModel);

        }
        public ActionResult UpdateClearance(long tid, long stat)
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                tRequest.transcriptClearanceStatus = new TranscriptClearanceStatus { TranscriptClearanceStatusId = (int)stat };
                transcriptRequestLogic.Modify(tRequest);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Clearance");
        }
    
        public ActionResult ViewTranscriptDetails()
        {
            viewModel = new TranscriptProcessorViewModel();

            return View(viewModel); 
        }
        [HttpPost]
        public ActionResult ViewTranscriptDetails(TranscriptProcessorViewModel viewModel)
        {
            try
            {
                if (viewModel.transcriptRequest.student.MatricNumber != null)
                {
                    PersonLogic personLogic = new PersonLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();

                    Model.Model.Student student = studentLogic.GetModelBy(s => s.Matric_Number == viewModel.transcriptRequest.student.MatricNumber);
                    if (student != null)
                    {
                        Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                        List<TranscriptRequest> transcriptRequests = transcriptRequestLogic.GetModelsBy(tr => tr.Student_id == student.Id);
                        if (transcriptRequests == null)
                        {
                            SetMessage("The student has not made a transcript request", Message.Category.Error);
                        }
                        else
                        {
                            viewModel.RequestDateString = transcriptRequests.LastOrDefault().DateRequested.ToShortDateString();
                            viewModel.transcriptRequests = transcriptRequests;
                            viewModel.Person = person;
                        }
                    }
                    else
                    {
                        SetMessage("Matric Number is not valid, or the student has not made a transcript request", Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Enter Matric Number!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
                        
            return View(viewModel);
        }

        public ActionResult ViewAllTranscriptRequest()
        {
            TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
            TranscriptRequest transcriptRequest = new TranscriptRequest();
            PersonLogic personLogic = new PersonLogic();
            List<TranscriptRequest> requestList = new List<TranscriptRequest>();
            TranscriptViewModel vModel = new TranscriptViewModel();
            var tReqs = transcriptRequestLogic.GetModelsBy(x => x.Student_id > 0);
            foreach(var person in tReqs)
            {
                var getPerson = personLogic.GetModelBy(p => p.Person_Id == person.student.Id);
                if (!String.IsNullOrEmpty(getPerson.Email))
                {
                    person.Email = getPerson.Email;
                    requestList.Add(person);

                }
            }
            vModel.TranscriptRequests = requestList;
            return View(vModel);
        }
        public ActionResult EditTranscriptDetails(long Id)
        {
            try
            {
                viewModel = new TranscriptProcessorViewModel();
                if (Id > 0)
                {
                    PersonLogic personLogic = new PersonLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    TranscriptRequest transcriptRequest = transcriptRequestLogic.GetModelBy(a => a.Transcript_Request_Id == Id);
                    Model.Model.Student student = transcriptRequest.student;
                    if (student != null)
                    {
                        Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                        viewModel.RequestDateString = transcriptRequest.DateRequested.ToShortDateString();
                        viewModel.transcriptRequest = transcriptRequest;
                        viewModel.Person = person;
                       
                    }
                    else
                    {
                        SetMessage("Matric Number is not valid, or the student has not made a transcript request", Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Enter Matric Number!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveTranscriptDetails(TranscriptProcessorViewModel viewModel)
        {
            try
            {
                if (viewModel.transcriptRequest != null)
                {
                    PersonLogic personLogic = new PersonLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
 
                    Person person = new Person();
                    Model.Model.Student student = new Model.Model.Student();

                    person.Id = viewModel.transcriptRequest.student.Id;
                    person.LastName = viewModel.transcriptRequest.student.LastName;
                    person.FirstName = viewModel.transcriptRequest.student.FirstName;
                    person.OtherName = viewModel.transcriptRequest.student.OtherName;
                    bool isPersonModified = personLogic.Modify(person);

                    student.Id = viewModel.transcriptRequest.student.Id;
                    student.MatricNumber = viewModel.transcriptRequest.student.MatricNumber;
                    bool isStudentModified = studentLogic.Modify(student);

                    if (viewModel.transcriptRequest.DestinationCountry.Id == "OTH")
                    {
                        viewModel.transcriptRequest.DestinationState.Id = "OT"; 
                    }
                    bool isTranscriptRequestModified = transcriptRequestLogic.Modify(viewModel.transcriptRequest);

                    if (isTranscriptRequestModified && isStudentModified)
                    {
                        SetMessage("Operation Successful!", Message.Category.Information);
                        return RedirectToAction("ViewTranscriptDetails");
                    }
                    if (isTranscriptRequestModified && !isStudentModified)
                    {
                        SetMessage("Not all fields were modified!", Message.Category.Information);
                        return RedirectToAction("ViewTranscriptDetails");
                    }
                    if (!isTranscriptRequestModified && isStudentModified)
                    {
                        SetMessage("Not all fields were modified!", Message.Category.Information);
                        return RedirectToAction("ViewTranscriptDetails");
                    }
                    if (!isTranscriptRequestModified && !isStudentModified)
                    {
                        SetMessage("No item modified!", Message.Category.Information);
                        return RedirectToAction("ViewTranscriptDetails");
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("EditTranscriptDetails");
        }
        public ActionResult DeleteTranscriptDetails(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                    PaymentLogic paymentLogic = new PaymentLogic();

                    TranscriptRequest transcriptRequest = transcriptRequestLogic.GetModelBy(tr => tr.Transcript_Request_Id == Id);
                    TranscriptRequest transcriptRequestAlt = transcriptRequest;
                    if (transcriptRequest != null)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            transcriptRequestLogic.Delete(tr => tr.Transcript_Request_Id == transcriptRequest.Id);

                            //if (transcriptRequest.payment != null)
                            //{
                            //    OnlinePayment onlinePayment = onlinePaymentLogic.GetModelBy(op => op.Payment_Id == transcriptRequestAlt.payment.Id);
                            //    if (onlinePayment != null)
                            //    {
                            //        onlinePaymentLogic.Delete(op => op.Payment_Id == transcriptRequestAlt.payment.Id);
                            //    }

                            //    paymentLogic.Delete(p => p.Payment_Id == transcriptRequestAlt.payment.Id); 
                            //}
                            

                            SetMessage("Operation Successful!", Message.Category.Information);
                            scope.Complete();
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewTranscriptDetails");
        }
        private void RetainDropDownList(TranscriptProcessorViewModel viewModel)
        {
            try
            {
                if (viewModel.transcriptRequest != null)
                {
                    if (viewModel.transcriptRequest.DestinationCountry != null)
                    {
                        ViewBag.Country = new SelectList(viewModel.CountrySelectList, "Value", "Text", viewModel.transcriptRequest.DestinationCountry.Id);
                    }
                    else
                    {
                        ViewBag.Country = viewModel.CountrySelectList;
                    }

                    if (viewModel.transcriptRequest.DestinationCountry != null)
                    {
                        ViewBag.State = new SelectList(viewModel.StateSelectList, "Value", "Text", viewModel.transcriptRequest.DestinationState.Id);
                    }
                    else
                    {
                        ViewBag.State = viewModel.StateSelectList;
                    }

                    if (viewModel.transcriptRequest.transcriptClearanceStatus != null)
                    {
                        ViewBag.TranscriptClearanceStatus = new SelectList(viewModel.transcriptClearanceSelectList, "Value", "Text", viewModel.transcriptRequest.transcriptClearanceStatus.TranscriptClearanceStatusId);
                    }
                    else
                    {
                        ViewBag.TranscriptClearanceStatus = viewModel.transcriptClearanceSelectList;
                    }

                    if (viewModel.transcriptRequest.transcriptStatus != null)
                    {
                        ViewBag.TranscriptStatus = new SelectList(viewModel.transcriptSelectList, "Value", "Text", viewModel.transcriptRequest.transcriptStatus.TranscriptStatusId);
                    }
                    else
                    {
                        ViewBag.TranscriptStatus = viewModel.transcriptSelectList;
                    } 
                }
                else
                {
                    ViewBag.Country = viewModel.CountrySelectList;
                    ViewBag.State = viewModel.StateSelectList;
                    ViewBag.TranscriptClearanceStatus = viewModel.transcriptClearanceSelectList;
                    ViewBag.TranscriptStatus = viewModel.transcriptSelectList;
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        } 
    
    }
}
