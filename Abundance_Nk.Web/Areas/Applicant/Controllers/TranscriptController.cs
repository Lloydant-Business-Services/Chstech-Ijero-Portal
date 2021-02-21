using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Controllers;
using System.Configuration;
using System.Transactions;
using System.Web.Script.Serialization;
using Abundance_Nk.Model.Entity.Model;
using System.IO;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
    [AllowAnonymous]
    public class TranscriptController : BaseController
    {
        private string appRoot = ConfigurationManager.AppSettings["AppRoot"];
        TranscriptViewModel viewModel;
        public ActionResult Index(string type)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                ViewBag.StateId = viewModel.StateSelectList;
                ViewBag.CountryId = viewModel.CountrySelectList;

                if (type == "")
                {
                    type = null;
                }

                viewModel.RequestType = "Transcript_Request";
                TempData["RequestType"] = "Transcript_Request";
                TempData.Keep("RequestType");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(TranscriptViewModel transcriptViewModel)
        {
            try
            {

                if (transcriptViewModel.transcriptRequest.student.MatricNumber != null)
                {
                    TempData["Matric_Number"] = transcriptViewModel.transcriptRequest.student.MatricNumber;
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    TranscriptRequest tRequest = transcriptRequestLogic.GetModelsBy(t => t.STUDENT.Matric_Number == transcriptViewModel.transcriptRequest.student.MatricNumber).LastOrDefault();

                    if (tRequest != null)
                    {
                        PersonLogic personLogic = new PersonLogic();
                        Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(p => p.Person_Id == tRequest.student.Id);
                        tRequest.student.FirstName = person.FirstName;
                        tRequest.student.LastName = person.LastName;
                        tRequest.student.OtherName = person.OtherName;
                        transcriptViewModel.transcriptRequest = tRequest;

                        if (tRequest.payment != null && tRequest.payment.Id > 0)
                        {
                            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                            tRequest.remitaPayment = remitaPaymentLogic.GetBy(tRequest.payment.Id);
                            if (tRequest.remitaPayment != null && tRequest.remitaPayment.Status.Contains("01"))
                            {

                                GetStudentDetails(transcriptViewModel);
                                transcriptViewModel.transcriptRequest.payment = null;
                            }
                        }
                    }
                    else
                    {
                        GetStudentDetails(transcriptViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(transcriptViewModel);
        }
        public ActionResult IndexAlt(TranscriptViewModel transcriptViewModel)
        {
            string type = Convert.ToString(TempData["RequestType"]);
            if (type == "")
            {
                type = null;
            }
            TempData.Keep("RequestType");

            try
            {

                if (transcriptViewModel.transcriptRequest.student == null)
                {
                    SetMessage("Enter Your Matriculation Number", Message.Category.Error);
                    return RedirectToAction("Index", new { type = type });
                }

                if (transcriptViewModel.transcriptRequest.student.MatricNumber != null)
                {
                    TempData["Matric_Number"] = transcriptViewModel.transcriptRequest.student.MatricNumber;
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    List<TranscriptRequest> tRequests = transcriptRequestLogic.GetModelsBy(t => t.STUDENT.Matric_Number == transcriptViewModel.transcriptRequest.student.MatricNumber && t.Request_Type == type);

                    if (tRequests.Count > 0)
                    {
                        PersonLogic personLogic = new PersonLogic();
                        long sid = tRequests.FirstOrDefault().student.Id;
                        Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(p => p.Person_Id == sid);

                        for (int i = 0; i < tRequests.Count; i++)
                        {
                            tRequests[i].student.FirstName = person.FirstName;
                            tRequests[i].student.LastName = person.LastName;
                            tRequests[i].student.OtherName = person.OtherName;
                            //transcriptViewModel.transcriptRequest = tRequests[i];

                            if (tRequests[i].payment != null && tRequests[i].payment.Id > 0)
                            {
                                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                                tRequests[i].remitaPayment = remitaPaymentLogic.GetBy(tRequests[i].payment.Id);
                                if (tRequests[i].remitaPayment != null && tRequests[i].remitaPayment.Status.Contains("01"))
                                {
                                    GetStudentDetails(transcriptViewModel);
                                    tRequests[i].payment = null;
                                }
                            }
                        }

                        transcriptViewModel.TranscriptRequests = tRequests;
                        transcriptViewModel.RequestType = type;

                        return View(transcriptViewModel);
                    }
                    else
                    {
                        StudentLogic studentLogic = new StudentLogic();
                        //Model.Model.Student student = new Model.Model.Student();
                        var student = studentLogic.GetModelBy(s => s.Matric_Number == transcriptViewModel.transcriptRequest.student.MatricNumber);

                        if (student != null)
                        {
                            return RedirectToAction("Request", new { sid = student.Id });
                        }
                        else
                        {
                            return RedirectToAction("Request", new { sid = 0 });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                return RedirectToAction("Index", new { type = type });
            }

            transcriptViewModel.RequestType = type;
            return View(transcriptViewModel);
        }
        private static void GetStudentDetails(TranscriptViewModel transcriptViewModel)
        {
            StudentLogic studentLogic = new StudentLogic();
            Model.Model.Student student = new Model.Model.Student();
            string MatricNumber = transcriptViewModel.transcriptRequest.student.MatricNumber;
            student = studentLogic.GetBy(MatricNumber);
            if (student != null)
            {
                PersonLogic personLogic = new PersonLogic();
                Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                student.FirstName = person.FirstName;
                student.LastName = person.LastName;
                student.OtherName = person.OtherName;
                transcriptViewModel.transcriptRequest.student = student;

            }
        }

        public ActionResult Request(long sid)
        {
            string matric_no = Convert.ToString(TempData["Matric_Number"]);
            string type = "Transcript_Request";
            if (type == "")
            {
                type = null;
            }

            TempData.Keep("RequestType");
            TempData.Keep("Matric_Number");
          

            try
            {
                viewModel = new TranscriptViewModel();
                TranscriptRequest transcriptRequest = new TranscriptRequest();
                StudentLogic studentLogic = new StudentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                PersonLogic personLogic = new PersonLogic();
                Person person = new Person();
                if (sid > 0)
                {
                    // transcriptRequest = transcriptRequestLogic.GetBy(sid);
                    transcriptRequest = transcriptRequestLogic.GetModelsBy(tr => tr.Student_id == sid).FirstOrDefault();
                    person = personLogic.GetModelBy(p => p.Person_Id == sid);
                    if (transcriptRequest != null)
                    {
                        viewModel.transcriptRequest = transcriptRequest;
                    }
                    else
                    {
                        Abundance_Nk.Model.Model.Student student = studentLogic.GetBy(sid);
                        viewModel.transcriptRequest = new TranscriptRequest();
                        viewModel.transcriptRequest.student = student;
                        if(person != null && !String.IsNullOrEmpty(person.Email))
                        {
                            viewModel.transcriptRequest.Email = person.Email;
                        }
                    }
                }
                else
                {
                    viewModel.MatricNumber = matric_no;

                }

                viewModel.RequestType = type;
                if (type == "Transcript_Request")
                {
                    ViewBag.StateId = new SelectList(viewModel.StateSelectList, "Value", "Text", "ET");
                    ViewBag.CountryId = new SelectList(viewModel.CountrySelectList, "Value", "Text", "NIG");
                    ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
                    ViewBag.DepartmentId = viewModel.DepartmentSelectListItem;

                    if (sid > 0)
                    {
                        viewModel.transcriptRequest.DestinationAddress = "";
                        viewModel.transcriptRequest.DestinationState = new State() { Id = "ET" };
                        viewModel.transcriptRequest.DestinationCountry = new Country() { Id = "NIG" };
                        viewModel.StudentLevel = studentLevelLogic.GetBy(sid);
                    }
                    else
                    {
                        viewModel.transcriptRequest = new TranscriptRequest();
                        viewModel.transcriptRequest.DestinationAddress = "";
                        viewModel.transcriptRequest.DestinationState = new State() { Id = "ET" };
                        viewModel.transcriptRequest.DestinationCountry = new Country() { Id = "NIG" };
                    }
                }
                else
                {
                    ViewBag.StateId = viewModel.StateSelectList;
                    ViewBag.CountryId = viewModel.CountrySelectList;
                    if (sid > 0)
                    {
                        //Do nothing
                    }
                    else
                    {
                        viewModel.transcriptRequest = new TranscriptRequest();
                    }
                }

                ViewBag.DeliveryServices = viewModel.DeliveryServiceSelectList;
                ViewBag.DeliveryServiceZones = new SelectList(new List<DeliveryServiceZone>(), "Id", "Name");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            viewModel.RequestType = type;
            TempData["TranscriptViewModel"] = viewModel;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Request(TranscriptViewModel transcriptViewModel)
        {
            string type = Convert.ToString(TempData["RequestType"]);
            string result_url = Convert.ToString(TempData["Statement_of_result_url"]);
            string transcript_passport_url = Convert.ToString(TempData["transcript_passport_url"]);
            if (type == "")
            {
                type = null;
            }
         
            TempData.Keep("RequestType");           
            try
            {
                viewModel = new TranscriptViewModel();
                ReloadDropdown(transcriptViewModel);
                Model.Model.Student student = new Model.Model.Student();
                Person person = new Person();
                StudentLogic stduentLogic = new StudentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                PersonLogic personLogic = new PersonLogic();
                if (transcriptViewModel.transcriptRequest != null && transcriptViewModel.transcriptRequest.Id <= 0 && transcriptViewModel.transcriptRequest.student != null && transcriptViewModel.transcriptRequest.DestinationCountry != null)
                {
                    if (transcriptViewModel.transcriptRequest.student.Id < 1)
                    {
                       
                       

                        student = transcriptViewModel.transcriptRequest.student;
                        string matric_no = Convert.ToString(TempData["Matric_Number"]);

                        Role role = new Role() { Id = 5 };
                        Nationality nationality = new Nationality() { Id = 1 };

                        person.LastName = student.LastName;
                        person.FirstName = student.FirstName;
                        person.OtherName = student.OtherName;
                        person.Email = transcriptViewModel.transcriptRequest.Email;
                        person.State = new State() { Id = "OG" };
                        person.Role = role;
                        person.MobilePhone = student.MobilePhone;
                        person.Nationality = nationality;
                        person.DateEntered = DateTime.Now;
                        person.Type = new PersonType() { Id = 3 };
                        person = personLogic.Create(person);
                        if (person != null && person.Id > 0)
                        {
                            string Type = matric_no.Substring(0, 1);
                            if (Type == "H")
                            {
                                student.Type = new StudentType() { Id = 2 };
                            }
                            else
                            {
                                student.Type = new StudentType() { Id = 1 };
                            }
                            student.Id = person.Id;
                            student.MatricNumber = matric_no;
                            student.Category = new StudentCategory() { Id = 2 };
                            student.Status = new StudentStatus() { Id = 1 };
                            student = stduentLogic.Create(student);
                            
                        }

                    }
                    if (student?.Id > 0)
                    {
                        //create studentlevel record
                        var existStudentLevel = studentLevelLogic.GetModelsBy(f => f.Person_Id == student.Id).LastOrDefault();
                        if (existStudentLevel?.Id > 0)
                        {
                            existStudentLevel.Programme = new Programme { Id = transcriptViewModel.StudentLevel.Programme.Id };
                            existStudentLevel.Department = new Department { Id = transcriptViewModel.StudentLevel.Department.Id };
                            studentLevelLogic.Modify(existStudentLevel);
                        }
                        else
                        {
                            if (transcriptViewModel?.StudentLevel?.Programme?.Id > 0 && (transcriptViewModel.StudentLevel.Programme.Id == 1 ||
                                transcriptViewModel.StudentLevel.Programme.Id == 2 || transcriptViewModel.StudentLevel.Programme.Id == 6))
                            {
                                StudentLevel studentLevel = new StudentLevel()
                                {
                                    Active = true,
                                    Department = new Department { Id = transcriptViewModel.StudentLevel.Department.Id },
                                    Programme = new Programme { Id = transcriptViewModel.StudentLevel.Programme.Id },
                                    Level = new Level { Id = 2 },
                                    Student = student,
                                    Session = new Session { Id = 1 }

                                };
                                studentLevelLogic.Create(studentLevel);
                            }
                        }


                    }
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    TranscriptRequest transcriptRequest = new TranscriptRequest();
                    transcriptRequest = transcriptViewModel.transcriptRequest;
                    transcriptRequest.DateRequested = DateTime.Now;
                    transcriptRequest.Reciever = transcriptViewModel.Reciever;
                    transcriptRequest.StatementOfResult = result_url;
                    transcriptRequest.Passport = transcript_passport_url;
                   
                    //transcriptRequest.DestinationCountry = new Country { Id = "NIG" };
                    if (transcriptRequest.DestinationState.Id == null)
                    {
                        transcriptRequest.DestinationState = new State() { Id = "OT" };
                    }
                    transcriptRequest.transcriptClearanceStatus = new TranscriptClearanceStatus { TranscriptClearanceStatusId = 4 };
                    transcriptRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 1 };
                    transcriptRequest.RequestType = type;
                    if (transcriptRequest.DestinationCountry.Id == "OTH")
                    {
                        transcriptRequest.DestinationState = new State() { Id = "OT" };
                    }

                    if (string.IsNullOrEmpty(transcriptRequest.DestinationCountry.Id))
                    {
                        transcriptRequest.DestinationState.Id = null;
                    }

                    if (transcriptViewModel.DeliveryServiceZone != null)
                    {
                        transcriptRequest.DeliveryServiceZone = transcriptViewModel.DeliveryServiceZone;
                    }

                    transcriptRequest = transcriptRequestLogic.Create(transcriptRequest);

                    return RedirectToAction("TranscriptPayment", new { tid = transcriptRequest.Id });
                }
                else
                {
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();

                    if (transcriptViewModel.transcriptRequest.payment == null)
                    {
                        if (transcriptViewModel.transcriptRequest.student?.Id > 0)
                        {
                            //create studentlevel record
                            var existStudentLevel = studentLevelLogic.GetModelsBy(f => f.Person_Id == transcriptViewModel.transcriptRequest.student.Id).LastOrDefault();
                            if (existStudentLevel?.Id > 0)
                            {
                                existStudentLevel.Programme = new Programme { Id = transcriptViewModel.StudentLevel.Programme.Id };
                                existStudentLevel.Department = new Department { Id = transcriptViewModel.StudentLevel.Department.Id };
                                studentLevelLogic.Modify(existStudentLevel);
                            }
                            else
                            {
                                if (transcriptViewModel?.StudentLevel?.Programme?.Id > 0 && (transcriptViewModel.StudentLevel.Programme.Id == 1 ||
                                    transcriptViewModel.StudentLevel.Programme.Id == 2 || transcriptViewModel.StudentLevel.Programme.Id == 6))
                                {
                                    StudentLevel studentLevel = new StudentLevel()
                                    {
                                        Active = true,
                                        Department = new Department { Id = transcriptViewModel.StudentLevel.Department.Id },
                                        Programme = new Programme { Id = transcriptViewModel.StudentLevel.Programme.Id },
                                        Level = new Level { Id = 2 },
                                        Student = transcriptViewModel.transcriptRequest.student,
                                        Session = new Session { Id = 1 }

                                    };
                                    studentLevelLogic.Create(studentLevel);
                                }
                                else
                                {
                                    StudentLevel studentLevel = new StudentLevel()
                                    {
                                        Active = true,
                                        Department = new Department { Id = transcriptViewModel.StudentLevel.Department.Id },
                                        Programme = new Programme { Id = transcriptViewModel.StudentLevel.Programme.Id },
                                        Level = new Level { Id = 4 },
                                        Student = transcriptViewModel.transcriptRequest.student,
                                        Session = new Session { Id = 1 }

                                    };
                                    studentLevelLogic.Create(studentLevel);
                                }
                            }


                        }
                        TranscriptRequest transcriptRequest = new TranscriptRequest();
                        transcriptRequest = transcriptViewModel.transcriptRequest;
                        transcriptRequest.DateRequested = DateTime.Now;
                        transcriptRequest.Reciever = transcriptViewModel.Reciever;
                        transcriptRequest.StatementOfResult = result_url;
                        transcriptRequest.Passport = transcript_passport_url;
                        //transcriptRequest.DestinationCountry = new Country { Id = "NIG" };
                        if (transcriptRequest.DestinationState.Id == null)
                        {
                            transcriptRequest.DestinationState = new State() { Id = "OT" };
                        }
                        transcriptRequest.transcriptClearanceStatus = new TranscriptClearanceStatus { TranscriptClearanceStatusId = 4 };
                        transcriptRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 1 };
                        transcriptRequest.RequestType = type;
                        if (transcriptRequest.DestinationCountry.Id == "OTH")
                        {
                            transcriptRequest.DestinationState = new State() { Id = "OT" };
                        }

                        if (string.IsNullOrEmpty(transcriptRequest.DestinationCountry.Id))
                        {
                            transcriptRequest.DestinationState.Id = null;
                        }

                        if (transcriptViewModel.DeliveryServiceZone != null)
                        {
                            transcriptRequest.DeliveryServiceZone = transcriptViewModel.DeliveryServiceZone;
                        }
                        transcriptRequest = transcriptRequestLogic.Create(transcriptRequest);
                        return RedirectToAction("TranscriptPayment", new { tid = transcriptRequest.Id });

                    }
                    if (transcriptViewModel.transcriptRequest.DestinationCountry.Id == "OTH")
                    {
                        transcriptViewModel.transcriptRequest.DestinationState = new State() { Id = "OT" };
                    }
                    transcriptRequestLogic.Modify(transcriptViewModel.transcriptRequest);
                    return RedirectToAction("TranscriptPayment", new { tid = transcriptViewModel.transcriptRequest.Id });
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            viewModel.RequestType = type;
            TempData["TranscriptViewModel"] = viewModel;
            return View(viewModel);
        }

        public ActionResult TranscriptPayment(long tid)
        {
            viewModel = new TranscriptViewModel();
            string type = Convert.ToString(TempData["RequestType"]);
            if (type == "")
            {
                type = null;
            }
            TempData.Keep("RequestType");

            try
            {
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                Model.Model.Student student = new Model.Model.Student();
                StudentLogic studentLogic = new StudentLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                student = studentLogic.GetModelBy(s => s.Person_Id == tRequest.student.Id);
                PersonLogic personLogic = new PersonLogic();
                Person person = new Person();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

                person = personLogic.GetModelBy(p => p.Person_Id == tRequest.student.Id);
                if (person != null)
                {
                    tRequest.student.ImageFileUrl = person.ImageFileUrl;
                    tRequest.student.FirstName = person.FirstName;
                    tRequest.student.LastName = person.LastName;
                    tRequest.student.OtherName = person.OtherName;
                    tRequest.student.MatricNumber = student.MatricNumber;

                    if (tRequest.payment != null)
                    {
                        tRequest.remitaPayment = remitaPaymentLogic.GetBy(tRequest.payment.Id);

                    }
                    viewModel.transcriptRequest = tRequest;
                    viewModel.RequestType = type;
                }

            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            viewModel.RequestType = type;
            TempData["TranscriptViewModel"] = viewModel;
            return View(viewModel);
        }

        public ActionResult ProcessPayment(long tid)
        {
            string type = Convert.ToString(TempData["RequestType"]);
            if (type == "")
            {
                type = null;
            }
            TempData.Keep("RequestType");

            try
            {
                TranscriptViewModel viewModel = new TranscriptViewModel();
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                if (tRequest != null)
                {
                    Decimal Amt = 0;
                    Abundance_Nk.Model.Model.Student student = tRequest.student;
                    PersonLogic personLogic = new PersonLogic();
                    Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(t => t.Person_Id == tRequest.student.Id);
                    Payment payment = new Payment();

                    //if (type == "Transcript Verification" || type == "Convocation Fee")
                    //{
                    //    //singleItem = splitItemLogic.GetBy(1);
                    //    //singleItem.deductFeeFrom = "0";
                    //    //singleItem.beneficiaryAmount = Convert.ToString(Amt);
                    //    //splitItems.Add(singleItem);
                    //    Fee fee = new Fee();
                    //    if (type == "Transcript Verification")
                    //    {
                    //        if (tRequest.DestinationCountry.Id == "NIG")
                    //        {
                    //            fee = new Fee() { Id = 47 };
                    //        }
                    //        else
                    //        {
                    //            fee = new Fee() { Id = 46 };
                    //        }

                    //        return RedirectToAction("ProcessTranscriptVerificationPayment", new { feeId = fee.Id, personId = tRequest.student.Id, transcriptId = tid });
                    //    }
                    //    else
                    //    {
                    //        fee = new Fee() { Id = 60 };
                    //        return RedirectToAction("ProcessConvocationPayment", new { feeId = fee.Id, personId = tRequest.student.Id, transcriptId = tid });
                    //    }

                    //    //return RedirectToAction("ProcessCertificatePayment", new { feeId = fee.Id, personId = tRequest.student.Id, transcriptId = tid });
                    //}

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        FeeType feeType = new FeeType();

                        if (type == "Convocation Fee")
                        {
                            feeType = new FeeType() { Id = (int)FeeTypes.ConvocationFee };
                        }
                        else if (type == "Certificate Verification" || type == "Certificate Collection")
                        {
                            feeType = new FeeType() { Id = (int)FeeTypes.CerificateCollection };
                        }
                        else if (type == "Transcript_Request")
                        {
                            feeType = new FeeType() { Id = (int)FeeTypes.Transcript };
                            
                        }
                        else
                        {
                            feeType = new FeeType() { Id = (int)FeeTypes.Transcript };
                        }

                        payment = CreatePayment(student, feeType);

                        if (payment != null)
                        {
                            Fee fee = null;
                            PaymentLogic paymentLogic = new PaymentLogic();
                            //Get Payment Specific Setting
                            RemitaSettings settings = new RemitaSettings();
                            RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();

                            if (tRequest.DestinationCountry != null && tRequest.DestinationCountry.Id == "NIG")
                            {
                                
                                    fee = new Fee() { Id = 46 };
                                    settings = settingsLogic.GetBy(2);
                                    payment.FeeDetails = paymentLogic.SetFeeDetails(46);


                            }
                            else
                            {
                                
                                    fee = new Fee() { Id = 47 };
                                    settings = settingsLogic.GetBy(2);
                                    payment.FeeDetails = paymentLogic.SetFeeDetails(47);


                            }

                            SetSettingsDescription(settings, type);

                            Amt = payment.FeeDetails.Sum(a => a.Fee.Amount);
                            //Amt = 70000;

                            if (type == null || type.Contains("Transcript") && tRequest.DeliveryServiceZone != null)
                            {
                                //Amt = payment.FeeDetails.Sum(a => a.Fee.Amount) + tRequest.DeliveryServiceZone.Fee.Amount;
                                Amt = 45000;
                            }

                            //Get Split Specific details;
                            List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                            RemitaSplitItems singleItem = new RemitaSplitItems();
                            RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();
                            if (type == "Certificate Verification" || type == "Certificate Collection")
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "300";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);
                            }
                            else if (type == "Transcript Verification" && tRequest.DestinationCountry.Id != "NIG")
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "1000";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);
                            }
                            else if (type == "Transcript Verification" && tRequest.DestinationCountry.Id == "NIG")
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "500";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);
                            }
                            else if (type == "Transcript_Request" && tRequest.DestinationCountry.Id == "NIG")
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "500";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);
                            }
                            else if (type == "Transcript_Request" && tRequest.DestinationCountry.Id != "NIG")
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "1000";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);
                            }
                            else if (type == "Convocation Fee")
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "0";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);
                            }
                            else if (type == null && tRequest.DestinationCountry.Id != "NIG")
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "1000";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);
                            }
                            else if (type == null)
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "500";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);
                            }

                            //Get BaseURL
                            string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                            RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                            RemitaPayment remitaPayment = null;
                            if (type == null || type == "Transcript_Request")
                            {
                                remitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "TRANSCRIPT", splitItems, settings, Amt);
                            }
                            if (type == "Transcript Verification")
                            {
                                remitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "TRANSCRIPT VERIFICATION", splitItems, settings, Amt);
                            }
                            if (type == "Certificate Collection")
                            {
                                remitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "CERTIFICATE COLLECTION", splitItems, settings, Amt);
                            }
                            if (type == "Certificate Verification")
                            {
                                remitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "CERTIFICATE VERIFICATION", splitItems, settings, Amt);
                            }
                            if (type == "Convocation Fee")
                            {
                                remitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "CONVOCATION FEE", splitItems, settings, Amt);
                            }

                            if (remitaPayment != null)
                            {
                                transaction.Complete();
                            }
                            else
                            {
                                SetMessage("Error Occurred! Remita Response: 'Invalid Request' ", Message.Category.Error);
                                return RedirectToAction("TranscriptPayment", new { tid = tid });
                            }


                            viewModel.Hash = GenerateHash(settings.Api_key, remitaPayment);
                            viewModel.RemitaBaseUrl = remitaBaseUrl;
                            viewModel.RemitaPayment = remitaPayment;
                            //  viewModel.RemitaPayementProcessor = remitaProcessor;
                            TempData["TranscriptViewModel"] = viewModel;

                        }
                    }
                    tRequest.payment = payment;
                    tRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 1 };
                    transcriptRequestLogic.Modify(tRequest);

                    return RedirectToAction("TranscriptInvoice", new { controller = "Credential", area = "Common", pmid = payment.Id });
                    //move payment to invoiceGeneration
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index", new { type = type });
        }

        private void SetSettingsDescription(RemitaSettings settings, string type)
        {
            try
            {
                if (settings.Payment_SettingId == 4)
                {
                    if (type == null)
                    {
                        settings.Description = "TRANSCRIPT LOCAL";
                    }
                    if (type == "Transcript Verification")
                    {
                        settings.Description = "TRANSCRIPT VERIFICATION LOCAL";
                    }
                    if (type == "Certificate Collection")
                    {
                        settings.Description = "CERTIFICATE LOCAL";
                    }
                    if (type == "Certificate Verification")
                    {
                        settings.Description = "CERTIFICATE VERIFICATION LOCAL";
                    }
                    if (type == "Transcript_Request")
                    {
                        settings.Description = "TRANSCRIPT REQUEST";
                    }
                }
                if (settings.Payment_SettingId == 5)
                {
                    if (type == null)
                    {
                        settings.Description = "TRANSCRIPT INTERNATIONAL";
                    }
                    if (type == "Transcript Verification")
                    {
                        settings.Description = "TRANSCRIPT VERIFICATION INTERNATIONAL";
                    }
                    if (type == "Certificate Collection")
                    {
                        settings.Description = "CERTIFICATE INTERNATIONAL";
                    }
                    if (type == "Certificate Verification")
                    {
                        settings.Description = "CERTIFICATE VERIFICATION INTERNATIONAL";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GenerateHash(string apiKey, RemitaPayment remitaPayment)
        {
            try
            {
                string hash = remitaPayment.MerchantCode + remitaPayment.RRR + apiKey;
                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(hash);
                string hashConcatenate = remitaProcessor.HashPaymentDetailToSHA512(hash);
                return hashConcatenate;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult MakePayment()
        {

            return View();
        }
        [HttpPost]
        public ActionResult MakePayment(TranscriptViewModel viewModel)
        {
            try
            {
                Payment payment = new Payment();
                PaymentLogic paymentLogic = new PaymentLogic();
                PaymentEtranzact paymentEtranzact = new PaymentEtranzact();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

                Model.Model.Session session = new Model.Model.Session() { Id = 1 };
                FeeType feetype = new FeeType() { Id = (int)FeeTypes.Transcript };

                payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.PaymentEtranzact.ConfirmationNo, session, feetype);
                if (payment != null && payment.Id > 0)
                {
                    if (payment.FeeType.Id != (int)FeeTypes.Transcript)
                    {
                        paymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.Confirmation_No == viewModel.PaymentEtranzact.ConfirmationNo);
                        if (paymentEtranzact != null)
                        {
                            viewModel.PaymentEtranzact = paymentEtranzact;
                            viewModel.Paymentstatus = true;
                            TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                            TranscriptRequest transcriptRequest = new TranscriptRequest();
                            payment = paymentLogic.GetModelBy(p => p.Invoice_Number == paymentEtranzact.CustomerID);
                            transcriptRequest = transcriptRequestLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                            return RedirectToAction("TranscriptPayment", new { tid = transcriptRequest.Id });

                        }

                        else
                        {
                            viewModel.Paymentstatus = false;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);

            }
            return View(viewModel);
        }

        private Payment CreatePayment(Abundance_Nk.Model.Model.Student student, FeeType feeType)
        {
            try
            {
                Payment payment = new Payment();
                PaymentLogic paymentLogic = new PaymentLogic();
                payment.PaymentMode = new PaymentMode() { Id = 1 };
                payment.PaymentType = new PaymentType() { Id = 2 };
                payment.PersonType = new PersonType() { Id = 4 };
                payment.FeeType = feeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = student;
                payment.Session = new Session { Id = 7 };

                OnlinePayment newOnlinePayment = null;
                Payment newPayment = paymentLogic.Create(payment);
                if (newPayment != null)
                {
                    PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                    OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                    OnlinePayment onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult GetState(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var country = id;
                return Json(country, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ReloadDropdown(TranscriptViewModel transcriptViewModel)
        {
            if (transcriptViewModel.transcriptRequest.DestinationState != null && !string.IsNullOrEmpty(transcriptViewModel.transcriptRequest.DestinationState.Id))
            {
                ViewBag.StateId = new SelectList(viewModel.StateSelectList, Utility.VALUE, Utility.TEXT, transcriptViewModel.transcriptRequest.DestinationState.Id);
                ViewBag.CountryId = new SelectList(viewModel.CountrySelectList, Utility.VALUE, Utility.TEXT, transcriptViewModel.transcriptRequest.DestinationCountry.Id);

            }
            else
            {
                ViewBag.StateId = new SelectList(viewModel.StateSelectList, Utility.VALUE, Utility.TEXT);
                ViewBag.CountryId = new SelectList(viewModel.CountrySelectList, Utility.VALUE, Utility.TEXT, transcriptViewModel.transcriptRequest.DestinationCountry.Id);
            }

            if (viewModel.DeliveryService != null && viewModel.DeliveryService.Id > 0)
            {
                ViewBag.DeliveryServices = new SelectList(viewModel.DeliveryServiceSelectList, Utility.VALUE, Utility.TEXT, transcriptViewModel.DeliveryService.Id);
            }
            else
            {
                ViewBag.DeliveryServices = new SelectList(viewModel.DeliveryServiceSelectList, Utility.VALUE, Utility.TEXT);
            }

            ViewBag.DeliveryServiceZones = new SelectList(new List<DeliveryServiceZone>(), "Id", "Name");
        }

        public ActionResult VerificationFees(string feeTypeId)
        {
            try
            {
                int feeType = Convert.ToInt32(Utility.Decrypt(feeTypeId));
                viewModel = new TranscriptViewModel();
                ViewBag.FeeTypeId = viewModel.FeesTypeSelectList;
                FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                viewModel.FeeType = feeTypeLogic.GetModelBy(a => a.Fee_Type_Id == feeType);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(viewModel);

        }

        [HttpPost]
        public ActionResult VerificationFees(TranscriptViewModel transcriptViewModel)
        {
            try
            {
                if (transcriptViewModel.StudentVerification.Student.MatricNumber != null)
                {
                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                    transcriptViewModel.StudentVerification.RemitaPayment =
                        remitaPaymentLogic.GetModelBy(
                            r => r.PAYMENT.Person_Id == transcriptViewModel.StudentVerification.Student.Id);
                    if (transcriptViewModel.StudentVerification.RemitaPayment != null)
                    {
                        PersonLogic personLogic = new PersonLogic();
                        Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(p => p.Person_Id == transcriptViewModel.StudentVerification.Student.Id);
                        transcriptViewModel.StudentVerification.Student.FirstName = person.FirstName;
                        transcriptViewModel.StudentVerification.Student.LastName = person.LastName;
                        transcriptViewModel.StudentVerification.Student.OtherName = person.OtherName;
                    }
                    else
                    {
                        StudentLogic studentLogic = new StudentLogic();
                        Model.Model.Student student = new Model.Model.Student();
                        string MatricNumber = transcriptViewModel.StudentVerification.Student.MatricNumber;
                        student = studentLogic.GetBy(MatricNumber);
                        if (student != null)
                        {
                            PersonLogic personLogic = new PersonLogic();
                            Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                            student.FirstName = person.FirstName;
                            student.LastName = person.LastName;
                            student.OtherName = person.OtherName;
                            transcriptViewModel.StudentVerification.Student = student;
                        }

                    }
                    FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                    transcriptViewModel.StudentVerification.FeeType = feeTypeLogic.GetModelBy(a => a.Fee_Type_Id == transcriptViewModel.FeeType.Id);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(transcriptViewModel);
        }

        public ActionResult VerificationRequest(long sid, string feeTypeId)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                StudentLogic studentLogic = new StudentLogic();
                if (sid > 0)
                {
                    viewModel.StudentVerification.Student = studentLogic.GetBy(sid);
                    if (viewModel.StudentVerification.Student != null)
                    {
                        viewModel.StudentVerification.FeeType = new FeeType();
                        int feeType = Convert.ToInt32(Utility.Decrypt(feeTypeId));
                        viewModel.StudentVerification.FeeType.Id = feeType;
                    }


                }
                int feeType_Id = Convert.ToInt32(Utility.Decrypt(feeTypeId));
                ViewBag.FeeTypeId = new SelectList(viewModel.FeesTypeSelectList, "Value", "Text", feeType_Id);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            TempData["TranscriptViewModel"] = viewModel;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult VerificationRequest(TranscriptViewModel transcriptViewModel)
        {
            try
            {
                viewModel = new TranscriptViewModel();

                if (transcriptViewModel.StudentVerification != null && transcriptViewModel.StudentVerification.RemitaPayment == null)
                {
                    if (transcriptViewModel.StudentVerification.Student.Id < 1)
                    {
                        Model.Model.Student student = new Model.Model.Student();
                        Person person = new Person();
                        StudentLevel studentLevel = new StudentLevel();
                        StudentLogic stduentLogic = new StudentLogic();
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        PersonLogic personLogic = new PersonLogic();

                        student = transcriptViewModel.StudentVerification.Student;

                        Role role = new Role() { Id = 5 };
                        Nationality nationality = new Nationality() { Id = 1 };

                        person.LastName = student.LastName;
                        person.FirstName = student.FirstName;
                        person.OtherName = student.OtherName;
                        person.Email = student.Email;
                        person.MobilePhone = student.MobilePhone;
                        person.State = new State() { Id = "OG" };
                        person.Role = role;
                        person.Nationality = nationality;
                        person.DateEntered = DateTime.Now;
                        person.Type = new PersonType() { Id = 3 };
                        person = personLogic.Create(person);
                        if (person != null && person.Id > 0)
                        {
                            string StudentType = student.MatricNumber.Substring(0, 1);
                            if (StudentType == "H")
                            {
                                student.Type = new StudentType() { Id = 2 };
                            }
                            else
                            {
                                student.Type = new StudentType() { Id = 1 };
                            }
                            student.Id = person.Id;
                            student.Category = new StudentCategory() { Id = 2 };
                            student.Status = new StudentStatus() { Id = 1 };
                            student = stduentLogic.Create(student);
                            transcriptViewModel.StudentVerification.Student.Id = student.Id;
                            return RedirectToAction("ProcessVerificationPayment", new { sid = student.Id, feetypeId = 16 });
                        }

                    }
                    return RedirectToAction("ProcessVerificationPayment", new { sid = transcriptViewModel.StudentVerification.Student.Id, feetypeId = 16 });


                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            TempData["TranscriptViewModel"] = viewModel;
            return RedirectToAction("ProcessVerificationPayment", new { sid = transcriptViewModel.StudentVerification.Student.Id, feetypeId = transcriptViewModel.StudentVerification.FeeType.Id });

        }
        public ActionResult ProcessVerificationPayment(long sid, int feetypeId)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                viewModel.StudentVerification = new StudentVerification();
                StudentLogic studentLogic = new StudentLogic();
                viewModel.StudentVerification.Student = studentLogic.GetBy(sid);
                if (viewModel.StudentVerification.Student != null)
                {
                    Decimal Amt = 0;

                    PersonLogic personLogic = new PersonLogic();
                    Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(t => t.Person_Id == viewModel.StudentVerification.Student.Id);
                    Payment payment = new Payment();
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        payment = CreatePayment(viewModel.StudentVerification.Student, new FeeType() { Id = (int)FeeTypes.Transcript });
                        if (payment != null)
                        {
                            Fee fee = null;
                            PaymentLogic paymentLogic = new PaymentLogic();

                            //Get Payment Specific Setting
                            RemitaSettings settings = new RemitaSettings();
                            RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();

                            if (feetypeId == 14)
                            {
                                fee = new Fee() { Id = 47, Name = "CERTIFICATE COLLECTION" };
                                settings = settingsLogic.GetBy(8);
                            }
                            else if (feetypeId == 15)
                            {
                                fee = new Fee() { Id = 49, Name = "STUDENTSHIP VERIFICATION" };
                                settings = settingsLogic.GetBy(7);
                            }
                            else if (feetypeId == 16)
                            {
                                fee = new Fee() { Id = 48, Name = "WES VERIFICATION" };
                                settings = settingsLogic.GetBy(6);
                            }

                            //payment.FeeDetails = paymentLogic.SetFeeDetails(fee.Id);
                            //viewModel.StudentVerification.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);

                            payment.FeeDetails = paymentLogic.SetFeeDetails(fee.Id);
                            Amt = payment.FeeDetails.Sum(a => a.Fee.Amount);


                            //Get Split Specific details;
                            List<RemitaSplitItems> splitItems = new List<RemitaSplitItems>();
                            RemitaSplitItems singleItem = new RemitaSplitItems();
                            RemitaSplitItemLogic splitItemLogic = new RemitaSplitItemLogic();

                            if (feetypeId == 16)
                            {
                                singleItem = splitItemLogic.GetBy(5);
                                singleItem.deductFeeFrom = "1";
                                singleItem.beneficiaryAmount = "1000";
                                splitItems.Add(singleItem);
                                singleItem = splitItemLogic.GetBy(1);
                                singleItem.deductFeeFrom = "0";
                                singleItem.beneficiaryAmount = Convert.ToString(Amt - Convert.ToDecimal(splitItems[0].beneficiaryAmount));
                                splitItems.Add(singleItem);

                                //Get BaseURL
                                string remitaBaseUrl = ConfigurationManager.AppSettings["RemitaBaseUrl"].ToString();
                                RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
                                RemitaPayment remitaPayment = null;

                                remitaPayment = remitaProcessor.GenerateRRR(payment.InvoiceNumber, remitaBaseUrl, "WES VERIFICATION", splitItems, settings, Amt);
                                
                                if (remitaPayment != null)
                                {
                                    transaction.Complete();
                                }
                                else
                                {
                                    SetMessage("Error Occurred! Remita Response: 'Invalid Request' ", Message.Category.Error);
                                    return RedirectToAction("VerificationFees");
                                }


                                viewModel.Hash = GenerateHash(settings.Api_key, remitaPayment);
                                viewModel.RemitaBaseUrl = remitaBaseUrl;
                                viewModel.RemitaPayment = remitaPayment;
                                //  viewModel.RemitaPayementProcessor = remitaProcessor;
                                TempData["TranscriptViewModel"] = viewModel;

                                TempData["RequestType"] = "Wes";

                                return RedirectToAction("TranscriptInvoice", new { controller = "Credential", area = "Common", pmid = payment.Id });
                            }
                            else
                            {
                                if (viewModel.StudentVerification.Student.Email == null)
                                {
                                    viewModel.StudentVerification.Student.Email = viewModel.StudentVerification.Student.FullName + "@gmail.com";
                                }
                                Remita remitaObj = new Remita()
                                {
                                    merchantId = settings.MarchantId,
                                    serviceTypeId = settings.serviceTypeId,
                                    orderId = payment.InvoiceNumber,
                                    payerEmail = viewModel.StudentVerification.Student.Email,
                                    payerName = viewModel.StudentVerification.Student.FullName,
                                    payerPhone = viewModel.StudentVerification.Student.MobilePhone,
                                    paymenttype = fee.Name,
                                    responseurl = settings.Response_Url,
                                    totalAmount = viewModel.StudentVerification.Amount

                                };
                                string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId + remitaObj.totalAmount + remitaObj.responseurl + settings.Api_key;

                                RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                                remitaObj.hash = remitaPayementProcessor.HashPaymentDetailToSHA512(toHash);
                                viewModel.StudentVerification.Remita = remitaObj;
                                RemitaPayment remitaPayment = new RemitaPayment() { payment = payment, OrderId = payment.InvoiceNumber, RRR = payment.InvoiceNumber, Status = "025", Description = fee.Name, TransactionDate = DateTime.Now, TransactionAmount = viewModel.StudentVerification.Amount };
                                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                                remitaPaymentLogic.Create(remitaPayment);
                            }
                            
                        }

                        transaction.Complete();
                    }

                    viewModel.StudentVerification.Payment = payment;

                    return View(viewModel);
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("VerificationFees");
        }

        //public ActionResult ProcessVerificationPayment(long sid, int feetypeId)
        //{
        //    try
        //    {
        //        viewModel = new TranscriptViewModel();
        //        viewModel.StudentVerification = new StudentVerification();
        //        StudentLogic studentLogic = new StudentLogic();
        //        viewModel.StudentVerification.Student = studentLogic.GetBy(sid);
        //        if (viewModel.StudentVerification.Student != null)
        //        {
        //            PersonLogic personLogic = new PersonLogic();
        //            Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(t => t.Person_Id == viewModel.StudentVerification.Student.Id);
        //            Payment payment = new Payment();
        //            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
        //            {
        //                payment = CreatePayment(viewModel.StudentVerification.Student, new FeeType() { Id = (int)FeeTypes.Transcript });
        //                if (payment != null)
        //                {
        //                    Fee fee = null;
        //                    PaymentLogic paymentLogic = new PaymentLogic();

        //                    //Get Payment Specific Setting
        //                    RemitaSettings settings = new RemitaSettings();
        //                    RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();

        //                    if (feetypeId == 14)
        //                    {
        //                        fee = new Fee() { Id = 47, Name = "CERTIFICATE COLLECTION" };
        //                        settings = settingsLogic.GetBy(8);
        //                    }
        //                    else if (feetypeId == 15)
        //                    {
        //                        fee = new Fee() { Id = 49, Name = "STUDENTSHIP VERIFICATION" };
        //                        settings = settingsLogic.GetBy(7);
        //                    }
        //                    else if (feetypeId == 16)
        //                    {
        //                        fee = new Fee() { Id = 48, Name = "WES VERIFICATION" };
        //                        settings = settingsLogic.GetBy(6);
        //                    }
        //                    payment.FeeDetails = paymentLogic.SetFeeDetails(fee.Id);
        //                    viewModel.StudentVerification.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
        //                    if (viewModel.StudentVerification.Student.Email == null)
        //                    {
        //                        viewModel.StudentVerification.Student.Email = viewModel.StudentVerification.Student.FullName + "@gmail.com";
        //                    }
        //                    Remita remitaObj = new Remita()
        //                    {
        //                        merchantId = settings.MarchantId,
        //                        serviceTypeId = settings.serviceTypeId,
        //                        orderId = payment.InvoiceNumber,
        //                        payerEmail = viewModel.StudentVerification.Student.Email,
        //                        payerName = viewModel.StudentVerification.Student.FullName,
        //                        payerPhone = viewModel.StudentVerification.Student.MobilePhone,
        //                        paymenttype = fee.Name,
        //                        responseurl = settings.Response_Url,
        //                        totalAmount = viewModel.StudentVerification.Amount

        //                    };
        //                    string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId + remitaObj.totalAmount + remitaObj.responseurl + settings.Api_key;

        //                    RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
        //                    remitaObj.hash = remitaPayementProcessor.HashPaymentDetailToSHA512(toHash);
        //                    viewModel.StudentVerification.Remita = remitaObj;
        //                    RemitaPayment remitaPayment = new RemitaPayment() { payment = payment, OrderId = payment.InvoiceNumber, RRR = payment.InvoiceNumber, Status = "025", Description = fee.Name, TransactionDate = DateTime.Now, TransactionAmount = viewModel.StudentVerification.Amount };
        //                    RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
        //                    remitaPaymentLogic.Create(remitaPayment);
        //                }

        //                transaction.Complete();
        //            }
        //            viewModel.StudentVerification.Payment = payment;

        //            return View(viewModel);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
        //    }
        //    return RedirectToAction("VerificationFees");
        //}
        public ActionResult ProcessCertificatePayment(int feeId, long personId, long transcriptId)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                viewModel.StudentVerification = new StudentVerification();
                StudentLogic studentLogic = new StudentLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                //FeeLogic feeLogic = new FeeLogic();
                viewModel.StudentVerification.Student = studentLogic.GetBy(personId);
                if (viewModel.StudentVerification.Student != null)
                {
                    //PersonLogic personLogic = new PersonLogic();
                    //Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(t => t.Person_Id == viewModel.StudentVerification.Student.Id);
                    Payment payment = new Payment();

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        payment = CreatePayment(viewModel.StudentVerification.Student, new FeeType() { Id = (int)FeeTypes.CerificateCollection });
                        if (payment != null)
                        {
                            //Get Payment Specific Setting
                            RemitaSettings settings = new RemitaSettings();
                            RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();

                            Fee fee = new Fee() { Id = 46, Name = "CERTIFICATE COLLECTION" };
                            settings = settingsLogic.GetBy(8);

                            payment.FeeDetails = paymentLogic.SetFeeDetails(fee.Id);
                            viewModel.StudentVerification.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                            if (viewModel.StudentVerification.Student.Email == null)
                            {
                                viewModel.StudentVerification.Student.Email = viewModel.StudentVerification.Student.FullName + "@gmail.com";
                            }
                            Remita remitaObj = new Remita()
                            {
                                merchantId = settings.MarchantId,
                                serviceTypeId = settings.serviceTypeId,
                                orderId = payment.InvoiceNumber,
                                payerEmail = viewModel.StudentVerification.Student.Email,
                                payerName = viewModel.StudentVerification.Student.FullName,
                                payerPhone = viewModel.StudentVerification.Student.MobilePhone,
                                paymenttype = fee.Name,
                                responseurl = settings.Response_Url,
                                totalAmount = viewModel.StudentVerification.Amount

                            };
                            string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId + remitaObj.totalAmount + remitaObj.responseurl + settings.Api_key;

                            RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                            remitaObj.hash = remitaPayementProcessor.HashPaymentDetailToSHA512(toHash);
                            viewModel.StudentVerification.Remita = remitaObj;
                            RemitaPayment remitaPayment = new RemitaPayment() { payment = payment, OrderId = payment.InvoiceNumber, RRR = payment.InvoiceNumber, Status = "025", Description = fee.Name, TransactionDate = DateTime.Now, TransactionAmount = viewModel.StudentVerification.Amount };
                            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                            remitaPaymentLogic.Create(remitaPayment);

                            TranscriptRequest transcriptRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == transcriptId);

                            if (transcriptRequest != null)
                            {
                                transcriptRequest.payment = payment;
                                transcriptRequest.transcriptStatus = new TranscriptStatus() { TranscriptStatusId = (int)Model.Entity.Model.TranscriptStatusList.AwaitingPaymentConfirmation };
                                transcriptRequest.transcriptClearanceStatus = new TranscriptClearanceStatus() { TranscriptClearanceStatusId = (int)Model.Entity.Model.TranscriptClearanceStatusList.Completed };

                                transcriptRequestLogic.Modify(transcriptRequest);
                            }
                        }

                        transaction.Complete();
                    }
                    viewModel.StudentVerification.Payment = payment;

                    return View(viewModel);
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("VerificationFees");
        }
        public ActionResult ProcessTranscriptVerificationPayment(int feeId, long personId, long transcriptId)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                viewModel.StudentVerification = new StudentVerification();
                StudentLogic studentLogic = new StudentLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                //FeeLogic feeLogic = new FeeLogic();
                viewModel.StudentVerification.Student = studentLogic.GetBy(personId);
                if (viewModel.StudentVerification.Student != null)
                {
                    //PersonLogic personLogic = new PersonLogic();
                    //Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(t => t.Person_Id == viewModel.StudentVerification.Student.Id);
                    Payment payment = new Payment();

                    TranscriptRequest transcriptRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == transcriptId);

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        payment = CreatePayment(viewModel.StudentVerification.Student, new FeeType() { Id = (int)FeeTypes.Transcript });
                        if (payment != null)
                        {
                            //Get Payment Specific Setting
                            RemitaSettings settings = new RemitaSettings();
                            RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();

                            Fee fee = new Fee();
                            if (transcriptRequest.DestinationCountry.Id == "NIG")
                            {
                                fee = new Fee() { Id = 46, Name = "TRANSCRIPT VERIFICATION(LOCAL)" };
                                settings = settingsLogic.GetBy(4);
                            }
                            else
                            {
                                fee = new Fee() { Id = 47, Name = "TRANSCRIPT VERIFICATION(INTERNATIONAL)" };
                                settings = settingsLogic.GetBy(5);
                            }

                            payment.FeeDetails = paymentLogic.SetFeeDetails(fee.Id);
                            viewModel.StudentVerification.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                            if (viewModel.StudentVerification.Student.Email == null)
                            {
                                viewModel.StudentVerification.Student.Email = viewModel.StudentVerification.Student.FullName + "@gmail.com";
                            }
                            Remita remitaObj = new Remita()
                            {
                                merchantId = settings.MarchantId,
                                serviceTypeId = settings.serviceTypeId,
                                orderId = payment.InvoiceNumber,
                                payerEmail = viewModel.StudentVerification.Student.Email,
                                payerName = viewModel.StudentVerification.Student.FullName,
                                payerPhone = viewModel.StudentVerification.Student.MobilePhone,
                                paymenttype = fee.Name,
                                responseurl = settings.Response_Url,
                                totalAmount = viewModel.StudentVerification.Amount

                            };
                            string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId + remitaObj.totalAmount + remitaObj.responseurl + settings.Api_key;

                            RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                            remitaObj.hash = remitaPayementProcessor.HashPaymentDetailToSHA512(toHash);
                            viewModel.StudentVerification.Remita = remitaObj;
                            RemitaPayment remitaPayment = new RemitaPayment() { payment = payment, OrderId = payment.InvoiceNumber, RRR = payment.InvoiceNumber, Status = "025", Description = fee.Name, TransactionDate = DateTime.Now, TransactionAmount = viewModel.StudentVerification.Amount };
                            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                            remitaPaymentLogic.Create(remitaPayment);



                            if (transcriptRequest != null)
                            {
                                transcriptRequest.payment = payment;
                                transcriptRequest.transcriptStatus = new TranscriptStatus() { TranscriptStatusId = (int)Model.Entity.Model.TranscriptStatusList.AwaitingPaymentConfirmation };
                                transcriptRequest.transcriptClearanceStatus = new TranscriptClearanceStatus() { TranscriptClearanceStatusId = (int)Model.Entity.Model.TranscriptClearanceStatusList.Completed };

                                transcriptRequestLogic.Modify(transcriptRequest);
                            }
                        }

                        transaction.Complete();
                    }
                    viewModel.StudentVerification.Payment = payment;

                    return View(viewModel);
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("VerificationFees");
        }
        public ActionResult ProcessConvocationPayment(int feeId, long personId, long transcriptId)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                viewModel.StudentVerification = new StudentVerification();
                StudentLogic studentLogic = new StudentLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                //FeeLogic feeLogic = new FeeLogic();
                viewModel.StudentVerification.Student = studentLogic.GetBy(personId);
                if (viewModel.StudentVerification.Student != null)
                {
                    //PersonLogic personLogic = new PersonLogic();
                    //Abundance_Nk.Model.Model.Person person = personLogic.GetModelBy(t => t.Person_Id == viewModel.StudentVerification.Student.Id);
                    Payment payment = new Payment();

                    TranscriptRequest transcriptRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == transcriptId);

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        payment = CreatePayment(viewModel.StudentVerification.Student, new FeeType() { Id = (int)FeeTypes.ConvocationFee });
                        if (payment != null)
                        {
                            //Get Payment Specific Setting
                            RemitaSettings settings = new RemitaSettings();
                            RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();

                            Fee fee = new Fee() { Id = 60, Name = "CONVOCATION FEE" };

                            settings = settingsLogic.GetBy(4);

                            payment.FeeDetails = paymentLogic.SetFeeDetails(fee.Id);
                            viewModel.StudentVerification.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                            if (viewModel.StudentVerification.Student.Email == null)
                            {
                                viewModel.StudentVerification.Student.Email = viewModel.StudentVerification.Student.FullName + "@gmail.com";
                            }
                            Remita remitaObj = new Remita()
                            {
                                merchantId = settings.MarchantId,
                                serviceTypeId = settings.serviceTypeId,
                                orderId = payment.InvoiceNumber,
                                payerEmail = viewModel.StudentVerification.Student.Email,
                                payerName = viewModel.StudentVerification.Student.FullName,
                                payerPhone = viewModel.StudentVerification.Student.MobilePhone,
                                paymenttype = fee.Name,
                                responseurl = settings.Response_Url,
                                totalAmount = viewModel.StudentVerification.Amount

                            };
                            string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId + remitaObj.totalAmount + remitaObj.responseurl + settings.Api_key;

                            RemitaPayementProcessor remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                            remitaObj.hash = remitaPayementProcessor.HashPaymentDetailToSHA512(toHash);
                            viewModel.StudentVerification.Remita = remitaObj;
                            RemitaPayment remitaPayment = new RemitaPayment() { payment = payment, OrderId = payment.InvoiceNumber, RRR = payment.InvoiceNumber, Status = "025", Description = fee.Name, TransactionDate = DateTime.Now, TransactionAmount = viewModel.StudentVerification.Amount };
                            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                            remitaPaymentLogic.Create(remitaPayment);

                            if (transcriptRequest != null)
                            {
                                transcriptRequest.payment = payment;
                                transcriptRequest.transcriptStatus = new TranscriptStatus() { TranscriptStatusId = (int)Model.Entity.Model.TranscriptStatusList.AwaitingPaymentConfirmation };
                                transcriptRequest.transcriptClearanceStatus = new TranscriptClearanceStatus() { TranscriptClearanceStatusId = (int)Model.Entity.Model.TranscriptClearanceStatusList.Completed };

                                transcriptRequestLogic.Modify(transcriptRequest);
                            }
                        }

                        transaction.Complete();
                    }
                    viewModel.StudentVerification.Payment = payment;

                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("VerificationFees");
        }

        public ActionResult ConvocationFee()
        {
            viewModel = new TranscriptViewModel();
            try
            {
                SetFeeTypeDropDown(viewModel);

                ViewBag.States = viewModel.StateSelectListItem;
                ViewBag.Sessions = viewModel.AllSessionSelectListItem;
                ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
                ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                ViewBag.Levels = new SelectList(new List<Level>(), Utility.ID, Utility.NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        private void SetFeeTypeDropDown(TranscriptViewModel viewModel)
        {
            try
            {
                if (viewModel.FeeTypeSelectListItem != null && viewModel.FeeTypeSelectListItem.Count > 0)
                {
                    viewModel.FeeType = new FeeType();
                    viewModel.FeeType.Id = (int)FeeTypes.ConvocationFee;
                    ViewBag.FeeTypes = new SelectList(viewModel.FeeTypeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.FeeType.Id);
                }
                else
                {
                    ViewBag.FeeTypes = new SelectList(new List<FeeType>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        [HttpPost]
        public ActionResult ConvocationFee(TranscriptViewModel viewModel)
        {
            try
            {
                PersonLogic personLogic = new PersonLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                viewModel.FeeType = new FeeType() { Id = (int)FeeTypes.ConvocationFee };
                viewModel.PaymentMode = new PaymentMode() { Id = 1 };
                viewModel.PaymentType = new PaymentType() { Id = 2 };

                ViewBag.States = viewModel.StateSelectListItem;
                ViewBag.Sessions = viewModel.AllSessionSelectListItem;
                ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
                ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);

                Model.Model.Student student = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber.Trim()).LastOrDefault();

                if (student != null)
                {
                    viewModel.StudentAlreadyExist = true;
                    viewModel.Person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                    viewModel.Student = student;
                    viewModel.StudentLevel = studentLevelLogic.GetModelsBy(l => l.Person_Id == student.Id).LastOrDefault();


                    if (viewModel.StudentLevel != null && viewModel.StudentLevel.Programme.Id > 0)
                    {
                        ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Programme.Id);
                    }
                    viewModel.LevelSelectListItem = Utility.PopulateLevelSelectListItem();
                    ViewBag.Levels = viewModel.LevelSelectListItem;

                    if (viewModel.Person != null && viewModel.Person.Id > 0)
                    {
                        if (viewModel.Person.State != null && !string.IsNullOrWhiteSpace(viewModel.Person.State.Id))
                        {
                            ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Person.State.Id);
                        }
                    }

                    if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0)
                    {
                        if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
                        {
                            //ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Level.Id);
                            //Commented because students weren't confirming level before generating invoice
                            ViewBag.Levels = viewModel.LevelSelectListItem;
                        }
                    }

                    SetDepartmentIfExist(viewModel);
                    SetDepartmentOptionIfExist(viewModel);
                }
                else
                {
                    ViewBag.Levels = new SelectList(new List<Level>(), Utility.ID, Utility.NAME);
                    //ViewBag.Sessions = Utility.PopulateSessionSelectListItem();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            viewModel.ShowInvoicePage = true;
            SetFeeTypeDropDown(viewModel);

            if (viewModel.StudentLevel == null)
            {
                viewModel.StudentAlreadyExist = false;
            }

            return View(viewModel);
        }
        public void CheckAndUpdateStudentLevel(TranscriptViewModel viewModel)
        {
            try
            {
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                List<StudentLevel> studentLevelList = studentLevelLogic.GetModelsBy(s => s.Person_Id == viewModel.Student.Id);
                //viewModel.StudentLevel = studentLevelLogic.GetBy(viewModel.Student, viewModel.Session);

                if (studentLevelList.Count != 0 && viewModel.StudentLevel != null)
                {
                    StudentLevel currentSessionLevel = studentLevelList.LastOrDefault(s => s.Session.Id == viewModel.Session.Id);
                    if (currentSessionLevel != null)
                    {
                        viewModel.StudentLevel = currentSessionLevel;
                    }
                    else
                    {
                        StudentLevel newStudentLevel = studentLevelList.LastOrDefault();
                        newStudentLevel.Session = viewModel.Session;
                        if (newStudentLevel.Level.Id == 1)
                        {
                            newStudentLevel.Level = new Level() { Id = 2 };
                        }
                        else if (newStudentLevel.Level.Id == 3)
                        {
                            newStudentLevel.Level = new Level() { Id = 4 };
                        }
                        else
                        {
                            newStudentLevel.Level = viewModel.StudentLevel.Level;
                        }

                        StudentLevel createdStudentLevel = studentLevelLogic.Create(newStudentLevel);
                        viewModel.StudentLevel = studentLevelLogic.GetModelBy(s => s.Student_Level_Id == createdStudentLevel.Id);
                    }
                }
                else if (viewModel.StudentLevel == null)
                {
                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void SetDepartmentIfExist(TranscriptViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel != null && viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
                {
                    ProgrammeDepartmentLogic departmentLogic = new ProgrammeDepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(viewModel.StudentLevel.Programme);
                    if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                    {
                        ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME, viewModel.StudentLevel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDepartmentOptionIfExist(TranscriptViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel != null && viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                {
                    DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                    List<DepartmentOption> departmentOptions = departmentOptionLogic.GetModelsBy(l => l.Department_Id == viewModel.StudentLevel.Department.Id);
                    if (viewModel.StudentLevel.DepartmentOption != null && viewModel.StudentLevel.DepartmentOption.Id > 0)
                    {
                        ViewBag.DepartmentOptions = new SelectList(departmentOptions, Utility.ID, Utility.NAME, viewModel.StudentLevel.DepartmentOption.Id);
                    }
                    else
                    {
                        ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        [HttpPost]
        public ActionResult ConvocationFeeInvocie(TranscriptViewModel viewModel)
        {
            try
            {
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                PersonLogic personLogic = new PersonLogic();

                    if (InvalidDepartmentSelection(viewModel))
                    {
                        KeepInvoiceGenerationDropDownState(viewModel);
                        return RedirectToAction("ConvocationFee");
                    }

                    if (InvalidMatricNumber(viewModel.Student.MatricNumber))
                    {
                        KeepInvoiceGenerationDropDownState(viewModel);
                        return RedirectToAction("ConvocationFee");
                    }

                    Payment payment = null;
                    if (viewModel.StudentAlreadyExist == false)
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            CreatePerson(viewModel);
                            if (viewModel.Student != null && viewModel.Student.Id > 0)
                            {
                                //do nothing   
                            }
                            else
                            {
                                CreateStudent(viewModel);
                            }
                            
                            payment = CreatePayment(viewModel);
                            CreateStudentLevel(viewModel);

                            transaction.Complete();
                        }
                    }
                    else
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            FeeType feeType = new FeeType() { Id = (int)FeeTypes.ConvocationFee };
                            payment = paymentLogic.GetBy(feeType, viewModel.Person, viewModel.Session);
                            if (payment == null || payment.Id <= 0)
                            {
                                payment = CreatePayment(viewModel);
                            }

                            transaction.Complete();
                        }
                    }

                    TempData["PaymentViewModel"] = viewModel;
                    return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Abundance_Nk.Web.Models.Utility.Encrypt(payment.Id.ToString()), });
                
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            KeepInvoiceGenerationDropDownState(viewModel);
            return RedirectToAction("ConvocationFee");
        }
        private bool InvalidMatricNumber(string matricNo)
        {
            try
            {
                string baseMatricNo = null;
                string[] matricNoArray = matricNo.Split('/');

                if (matricNoArray.Length > 0)
                {
                    string[] matricNoArrayCopy = new string[matricNoArray.Length - 1];
                    for (int i = 0; i < matricNoArray.Length; i++)
                    {
                        if (i != matricNoArray.Length - 1)
                        {
                            matricNoArrayCopy[i] = matricNoArray[i];
                        }
                    }
                    if (matricNoArrayCopy.Length > 0)
                    {
                        baseMatricNo = string.Join("/", matricNoArrayCopy);
                    }
                }
                else
                {
                    SetMessage("Invalid Matric Number entered!", Message.Category.Error);
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(baseMatricNo))
                {
                    
                }
                else
                {
                    SetMessage("Invalid Matric Number entered!", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidDepartmentSelection(TranscriptViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel.Department == null || viewModel.StudentLevel.Department.Id <= 0)
                {
                    SetMessage("Please select Department!", Message.Category.Error);
                    return true;
                }
                else if ((viewModel.StudentLevel.DepartmentOption == null && viewModel.StudentLevel.Programme.Id > 2) || (viewModel.StudentLevel.DepartmentOption != null && viewModel.StudentLevel.DepartmentOption.Id <= 0 && viewModel.StudentLevel.Programme.Id > 2))
                {
                    viewModel.DepartmentOptionSelectListItem = Utility.PopulateDepartmentOptionSelectListItem(viewModel.StudentLevel.Department, viewModel.StudentLevel.Programme);
                    if (viewModel.DepartmentOptionSelectListItem != null && viewModel.DepartmentOptionSelectListItem.Count > 0)
                    {
                        SetMessage("Please select Department Option!", Message.Category.Error);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void KeepInvoiceGenerationDropDownState(TranscriptViewModel viewModel)
        {
            try
            {
                if (viewModel.Session != null && viewModel.Session.Id > 0)
                {
                    ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Session.Id);
                }
                else
                {
                    ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT);
                }

                if (viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
                {
                    ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.Person.State.Id);
                }
                else
                {
                    ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT);
                }

                if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
                {
                    ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Level.Id);
                }
                else
                {
                    ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT);
                }

                if (viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
                {
                    viewModel.DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(viewModel.StudentLevel.Programme);
                    ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Programme.Id);

                    if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                    {
                        viewModel.DepartmentOptionSelectListItem = Utility.PopulateDepartmentOptionSelectListItem(viewModel.StudentLevel.Department, viewModel.StudentLevel.Programme);
                        ViewBag.Departments = new SelectList(viewModel.DepartmentSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Department.Id);

                        if (viewModel.StudentLevel.DepartmentOption != null && viewModel.StudentLevel.DepartmentOption.Id > 0)
                        {
                            ViewBag.DepartmentOptions = new SelectList(viewModel.DepartmentOptionSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.DepartmentOption.Id);
                        }
                        else
                        {
                            if (viewModel.DepartmentOptionSelectListItem != null && viewModel.DepartmentOptionSelectListItem.Count > 0)
                            {
                                ViewBag.DepartmentOptions = new SelectList(viewModel.DepartmentOptionSelectListItem, Utility.VALUE, Utility.TEXT);
                            }
                            else
                            {
                                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(viewModel.DepartmentSelectListItem, Utility.VALUE, Utility.TEXT);
                        ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT);
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                    ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured " + ex.Message, Message.Category.Error);
            }
        }

        private Person CreatePerson(TranscriptViewModel viewModel)
        {
            try
            {
                PersonLogic personLogic = new PersonLogic();

                Role role = new Role() { Id = 5 };
                //PersonType personType = new PersonType() { Id = viewModel.PersonType.Id };
                Nationality nationality = new Nationality() { Id = 1 };

                viewModel.Person.Role = role;
                viewModel.Person.Nationality = nationality;
                viewModel.Person.DateEntered = DateTime.Now;
                viewModel.Person.Type = new PersonType() { Id = (int)PersonTypes.Student };

                Person person = personLogic.Create(viewModel.Person);
                if (person != null && person.Id > 0)
                {
                    viewModel.Person.Id = person.Id;
                }

                return person;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Model.Model.Student CreateStudent(TranscriptViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();

                viewModel.Student.Number = 4;
                viewModel.Student.Category = new StudentCategory() { Id = viewModel.StudentLevel.Level.Id <= 2 ? 1 : 2 };
                viewModel.Student.Type = new StudentType() { Id = viewModel.StudentLevel.Programme.Id <= 2 ? 1 : 2 };
                viewModel.Student.Id = viewModel.Person.Id;
                viewModel.Student.Status = new StudentStatus() {Id = 1};

                return studentLogic.Create(viewModel.Student);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Payment CreatePayment(TranscriptViewModel viewModel)
        {
            Payment newPayment = new Payment();
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();

                Payment payment = new Payment();
                payment.PaymentMode = viewModel.PaymentMode;
                payment.PaymentType = viewModel.PaymentType;
                payment.PersonType = viewModel.Person.Type;
                payment.FeeType = viewModel.FeeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = viewModel.Person;
                payment.Session = viewModel.Session;

                PaymentMode pyamentMode = new PaymentMode() { Id = 1 };
                OnlinePayment newOnlinePayment = null;
                newPayment = paymentLogic.Create(payment);
                newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id, pyamentMode.Id, viewModel.StudentLevel.Department.Id, viewModel.Session.Id);
                Decimal Amt = newPayment.FeeDetails.Sum(p => p.Fee.Amount);

                if (newPayment != null)
                {
                    PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                    OnlinePayment onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);

                }

                payment = newPayment;
                if (payment != null)
                {
                    // transaction.Complete();
                }

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private StudentLevel CreateStudentLevel(TranscriptViewModel viewModel)
        {
            try
            {
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                viewModel.StudentLevel.Session = viewModel.Session;
                viewModel.StudentLevel.Student = viewModel.Student;
                return studentLevelLogic.Create(viewModel.StudentLevel);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult PayConvocationFee()
        {
            try
            {
                viewModel = new TranscriptViewModel();
                ViewBag.Sessions = viewModel.SessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult PayConvocationFee(TranscriptViewModel viewModel)
        {
            try
            {
                if (viewModel.ConfirmationNumber != null)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();

                    if (viewModel.ConfirmationNumber.Length > 12)
                    {
                        Model.Model.Session session = viewModel.Session;
                        FeeType feetype = new FeeType() { Id = (int)FeeTypes.ConvocationFee };

                        Payment payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationNumber, session, feetype);

                        if (payment != null && payment.Id > 0)
                        {
                            if (payment.FeeType.Id != (int)FeeTypes.ConvocationFee)
                            {
                                SetMessage("Confirmation Order Number (" + viewModel.ConfirmationNumber + ") entered is not for Convocation Fee payment! Please enter your Convocation Fee Confirmation Order Number.", Message.Category.Error);
                                ViewBag.Sessions = viewModel.SessionSelectListItem;
                                return View(viewModel);
                            }

                            return RedirectToAction("Receipt", "Credential", new { area = "Common", pmid = payment.Id });
                        }
                    }
                    else
                    {
                        SetMessage("Invalid Confirmation Order Number!", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Sessions = viewModel.SessionSelectListItem;
            return View(viewModel);
        }

        public JsonResult GetDeliveryServices(string countryId, string stateId)
        {
            try
            {
                List<DeliveryService> deliveryServicesInStateCountry = new List<DeliveryService>();

                if (!string.IsNullOrEmpty(countryId) && !string.IsNullOrEmpty(stateId))
                {
                    DeliveryServiceZoneLogic deliveryServiceZoneLogic = new DeliveryServiceZoneLogic();
                    DeliveryServiceLogic deliveryServiceLogic = new DeliveryServiceLogic();
                    StateGeoZoneLogic stateGeoZoneLogic = new StateGeoZoneLogic();

                    List<StateGeoZone> stateGeoZones = stateGeoZoneLogic.GetModelsBy(s => s.State_Id == stateId && s.Activated);

                    for (int i = 0; i < stateGeoZones.Count; i++)
                    {
                        StateGeoZone stateGeoZone = stateGeoZones[i];

                        List<DeliveryServiceZone> deliveryServiceZones = deliveryServiceZoneLogic.GetModelsBy(s => s.Country_Id == countryId && s.Geo_Zone_Id == stateGeoZone.GeoZone.Id && s.Activated);

                        List<DeliveryService> deliveryServices = deliveryServiceLogic.GetModelsBy(s => s.Activated);

                        for (int j = 0; j < deliveryServices.Count; j++)
                        {
                            DeliveryService deliveryService = deliveryServices[j];
                            if (deliveryServiceZones.Count(s => s.DeliveryService.Id == deliveryService.Id) > 0)
                            {
                                if (deliveryServicesInStateCountry.Count(s => s.Id == deliveryService.Id) <= 0)
                                {

                                    deliveryServicesInStateCountry.Add(deliveryService);
                                }

                            }
                        }
                    }
                }

                return Json(new SelectList(deliveryServicesInStateCountry, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public JsonResult GetDeliveryServiceZones(string countryId, string stateId, int deliveryServiceId)
        {
            try
            {
                List<DeliveryServiceZone> deliveryServiceZones = new List<DeliveryServiceZone>();

                if (!string.IsNullOrEmpty(countryId) && !string.IsNullOrEmpty(stateId) && deliveryServiceId > 0)
                {
                    DeliveryServiceZoneLogic deliveryServiceZoneLogic = new DeliveryServiceZoneLogic();
                    StateGeoZoneLogic stateGeoZoneLogic = new StateGeoZoneLogic();

                    List<StateGeoZone> stateGeoZones = stateGeoZoneLogic.GetModelsBy(s => s.State_Id == stateId && s.Activated);

                    for (int i = 0; i < stateGeoZones.Count; i++)
                    {
                        StateGeoZone stateGeoZone = stateGeoZones[i];

                        List<DeliveryServiceZone>  currentDeliveryServiceZones = deliveryServiceZoneLogic.GetModelsBy(s => s.Country_Id == countryId && s.Geo_Zone_Id == stateGeoZone.GeoZone.Id && s.Delivery_Service_Id == deliveryServiceId && s.Activated);

                        for (int j = 0; j < currentDeliveryServiceZones.Count; j++)
                        {
                            currentDeliveryServiceZones[j].Name = currentDeliveryServiceZones[j].GeoZone.Name + " - " + currentDeliveryServiceZones[j].Fee.Amount;

                            DeliveryServiceZone serviceZone = currentDeliveryServiceZones[j];

                            if (deliveryServiceZones.Count(s => s.Id == serviceZone.Id) <= 0)
                            {
                                deliveryServiceZones.Add(currentDeliveryServiceZones[j]);
                            }
                            
                        }
                    }
                }

                return Json(new SelectList(deliveryServiceZones, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public virtual ActionResult UploadStatementOfResult(FileUpload form)
        {
            //HttpPostedFileBase file = Request.Files["MyFile"];
            string matric_number = Convert.ToString(TempData["Matric_Number"]);
            TempData.Keep("Matric_Number");
            JsonResultObject jsonResultObject = new JsonResultObject();
            PersonLogic personLogic = new PersonLogic();
            Person person = new Person();
            TranscriptRequest transcriptRequest = new TranscriptRequest();
            TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();

            var myfile = form.file;
            bool isUploaded = false;
            string personName = form.personName;
            long studentId = form.studentId;
            string passportUrl = form.personName;
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;
            string newFileName = "";
            string returnName = "";

            try
            {
                if (form != null && form.file.ContentLength != 0)
                {
                    FileInfo fileInfo = new FileInfo(form.file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personName +"__statement_of_result__";
                    newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;
                    returnName = newFileName;

                    bool invalidFileMessage = InvalidFile(form.file.ContentLength, fileExtension);
                    if (!invalidFileMessage)
                    {
                        //isUploaded = false;
                        //TempData["imageUrl"] = null;
                        jsonResultObject.Message = "File Size exceeds the required";
                        return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
                    }
                    //string junkPath = imageFileUrl.Split('?').FirstOrDefault();
                    //string studentPath = "/Content/Student/" + junkPath.Split('/').LastOrDefault();

                    string pathForSaving = Server.MapPath("~/Content/StatementOfResult");
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, studentId.ToString());

                        form.file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "Statement of Result was saved!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/StatementOfResult/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            TempData["imageUrl"] = imageUrl;
                            TempData["Statement_of_result_url"] = imageUrl;
                            person = personLogic.GetModelBy(p => p.Person_Id == studentId);
                            //transcriptRequest = transcriptRequestLogic.GetModelBy
                            if (person != null)
                            {
                                person.ImageFileUrl = imageUrl;
                                personLogic.Modify(person);

                                jsonResultObject.Message = message;
                                //jsonResultObject.FileName = imageUrl;
                                return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
                            }


                        }
                    }
                }
                else
                {
                    jsonResultObject.Message = "No file Selected";
                    return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            //return Json(new { isUploaded = isUploaded, message = message, imageUrl = returnName }, "text/html", JsonRequestBehavior.AllowGet);
            jsonResultObject.Message = message;
            return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public virtual ActionResult UploadPassportCredential(FileUpload form)
        {
            //HttpPostedFileBase file = Request.Files["MyFile"];
            string matric_number = Convert.ToString(TempData["Matric_Number"]);
            TempData.Keep("Matric_Number");
            JsonResultObject jsonResultObject = new JsonResultObject();
            PersonLogic personLogic = new PersonLogic();
            Person person = new Person();
            TranscriptRequest transcriptRequest = new TranscriptRequest();
            TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();

            var myfile = form.file;
            bool isUploaded = false;
            string personName = form.personName;
            long studentId = form.studentId;
            string passportUrl = form.personName;
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;
            string newFileName = "";
            string returnName = "";

            try
            {
                if (form != null && form.file.ContentLength != 0)
                {
                    FileInfo fileInfo = new FileInfo(form.file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = "__passport_credential__" + personName;
                    newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;
                    returnName = newFileName;

                    bool invalidFileMessage = InvalidFilePassport(form.file.ContentLength, fileExtension);
                    if (!invalidFileMessage)
                    {
                        //isUploaded = false;
                        //TempData["imageUrl"] = null;
                        jsonResultObject.Message = "File Size exceeds the required";
                        return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
                    }
                    //string junkPath = imageFileUrl.Split('?').FirstOrDefault();
                    //string studentPath = "/Content/Student/" + junkPath.Split('/').LastOrDefault();

                    string pathForSaving = Server.MapPath("~/Content/StatementOfResult");
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, studentId.ToString());

                        form.file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "Passport was saved!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/StatementOfResult/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            TempData["transcript_passport_url"] = imageUrl;
                            person = personLogic.GetModelBy(p => p.Person_Id == studentId);
                            //transcriptRequest = transcriptRequestLogic.GetModelBy
                            if (person != null)
                            {
                                person.ImageFileUrl = imageUrl;
                                personLogic.Modify(person);

                                jsonResultObject.Message = message;
                                //jsonResultObject.FileName = imageUrl;
                                return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
                            }


                        }
                    }
                }
                else
                {
                    jsonResultObject.Message = "No file Selected";
                    return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            //return Json(new { isUploaded = isUploaded, message = message, imageUrl = returnName }, "text/html", JsonRequestBehavior.AllowGet);
            jsonResultObject.Message = message;
            return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
        }

        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return false;
                case ".png":
                    return false;
                case ".gif":
                    return false;
                case ".jpeg":
                    return false;
                default:
                    return true;
            }
        }

        private void DeleteFileIfExist(string folderPath, string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool InvalidFilePassport(decimal uploadedFileSize, string fileExtension)
        {
            try
            {
                JsonResultObject jsonResultObject = new JsonResultObject();
                string message = null;
                decimal oneKiloByte = 100;
                //decimal oneKiloByte = 200;
                decimal maximumFileSize = 20 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte, 1);
                if (InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension +
                              "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if (actualFileSizeToUpload > (maximumFileSize))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") +
                              " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                    jsonResultObject.Message = message;
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool InvalidFile(decimal uploadedFileSize, string fileExtension)
        {
            try
            {
                JsonResultObject jsonResultObject = new JsonResultObject();
                string message = null;
                //decimal oneKiloByte = 10;
                decimal oneKiloByte = 200;
                //decimal maximumFileSize = 20 * oneKiloByte;
                decimal maximumFileSize = 20 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte, 1);
                if (InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension +
                              "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if (actualFileSizeToUpload > (maximumFileSize))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") +
                              " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                    jsonResultObject.Message = message;
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult PrintTranscriptReciept()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PrintTranscriptReciept(string rrr)
        {
            if (!string.IsNullOrEmpty(rrr))
            {
                RemitaPayment remitaPayment = new RemitaPayment();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                RemitaPayementProcessor r = new RemitaPayementProcessor("521096");
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                remitaPayment = remitaPaymentLogic.GetModelsBy(a => a.RRR == rrr).FirstOrDefault();
                if (remitaPayment?.payment?.Id > 0)
                {
                    remitaPayment = r.GetStatus(remitaPayment.OrderId);
                    if (remitaPayment.Status.Contains("01") || remitaPayment.Status.Contains("00") || remitaPayment.Description.Contains("manual"))
                    {
                        var transcriptRequest = transcriptRequestLogic.GetModelsBy(f => f.Payment_Id == remitaPayment.payment.Id).FirstOrDefault();
                        if (transcriptRequest?.Id > 0)
                        {
                            transcriptRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 2 };
                            transcriptRequestLogic.Modify(transcriptRequest);
                            return RedirectToAction("TranscriptReciept", "Credential", new { area = "Common", pmid = remitaPayment.payment.Id });
                        }
                    }
                    else
                    {
                        SetMessage("This RRR has not been paid for!", Message.Category.Information);
                    }
                }
                else
                {
                    SetMessage("This RRR does not exist!", Message.Category.Information);
                }
               
            }
            return View();
        }
     
    }

    public class FileUpload
    {
        public HttpPostedFileBase file { get; set; }
        //public int my { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string personName { get; set; }
        public long personId { get; set; }
        public long studentId { get; set; }
    }
}