using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class HostelController : BaseController
    {
        private HostelViewModel viewModel;
        public ActionResult CreateHostelRequest()
        {
            viewModel = new HostelViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult CreateHostelRequest(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.Student.MatricNumber != null)
                {
                    HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                    SessionLogic sessionLogic = new SessionLogic();
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    PersonLogic personLogic = new PersonLogic();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    HostelBlacklistLogic hostelBlacklistLogic = new HostelBlacklistLogic();

                    Model.Model.Student student = new Model.Model.Student();
                    Person person = new Person();
                    StudentLevel studentLevel = new StudentLevel();
                    Programme programme = new Programme();
                    Department department = new Department();
                    Level level = new Level();

                    List<StudentLevel> studentLevels = new List<StudentLevel>();

                    Session session = new Session(){ Id = 8 };
                    List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                    if (students.Count != 1 && viewModel.Student.MatricNumber.Length < 20)
                    {
                        SetMessage("Student with this Matriculation Number does not exist Or Matric Number is Duplicate!", Message.Category.Error);
                        return View(viewModel);
                    }

                    if (students.Count == 0 && viewModel.Student.MatricNumber.Length > 20)
                    {
                        PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.Confirmation_No == viewModel.Student.MatricNumber && (p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.AcceptanceFee || p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.HNDAcceptance) && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id).LastOrDefault();
                        if (paymentEtranzact == null)
                        {
                            SetMessage("Confirmation Order Number is not for Current session's Acceptance Fee!", Message.Category.Error);
                            return View(viewModel); 
                        }

                        person = paymentEtranzact.Payment.Payment.Person;
                        AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);
                        if (appliedCourse == null)
                        {
                            SetMessage("No Applied course record!", Message.Category.Error);
                            return View(viewModel);
                        }

                        programme = appliedCourse.Programme;
                        department = appliedCourse.Department;
                        level = new Level(){Id = 1};
                        if (programme.Id == 3)
                        {
                           level = new Level(){Id = 3}; 
                        }
                    }
                    else
                    {
                        student = students.FirstOrDefault();
                        person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                        studentLevels = studentLevelLogic.GetModelsBy(sl => sl.STUDENT.Person_Id == student.Id);
                        if (studentLevels.Count == 0)
                        {
                            SetMessage("You have not registered for this session!", Message.Category.Error);
                            return View(viewModel);
                        }

                        int maxLevelId = studentLevels.Max(sl => sl.Level.Id);
                        studentLevel = studentLevels.Where(sl => sl.Level.Id == maxLevelId).LastOrDefault();
                        programme = studentLevel.Programme;
                        department = studentLevel.Department;
                        level = studentLevel.Level;
                    }
                    
                    //check blacklist

                    HostelBlacklist hostelBlacklist = hostelBlacklistLogic.GetModelsBy(h => h.Person_Id == person.Id && h.Session_Id == session.Id).LastOrDefault();

                    if (hostelBlacklist != null)
                    {
                        SetMessage("You cannot request for hostel allocation because of " + hostelBlacklist.Reason, Message.Category.Error);
                        return View(viewModel);
                    }

                    //List<PaymentEtranzact> paymentEtranzacts = paymentEtranzactLogic.GetModelsBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == student.Id && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees);

                    //if (paymentEtranzacts.Count == 0)
                    //{
                    //    SetMessage("Pay School Fees before making hostel request!", Message.Category.Error);
                    //    return View(viewModel);
                    //}

                    HostelRequest hostelRequest = hostelRequestLogic.GetModelBy(h => h.Person_Id == person.Id && h.Session_Id == session.Id);
                    if (hostelRequest == null)
                    {
                        if (student != null && student.Id > 0)
                        {
                            hostelRequest = new HostelRequest();
                            hostelRequest.Approved = false;
                            hostelRequest.Department = studentLevel.Department;
                            hostelRequest.Programme = studentLevel.Programme;
                            hostelRequest.RequestDate = DateTime.Now;
                            hostelRequest.Session = session;
                            hostelRequest.Student = student;
                            hostelRequest.Person = person;
                            SetLevel(student, studentLevel, hostelRequest);

                            hostelRequestLogic.Create(hostelRequest);
                             
                        }
                        else
                        {
                            hostelRequest = new HostelRequest();
                            hostelRequest.Approved = false;
                            hostelRequest.Department = department;
                            hostelRequest.Programme = programme;
                            hostelRequest.RequestDate = DateTime.Now;
                            hostelRequest.Session = session;
                            hostelRequest.Student = student;
                            hostelRequest.Person = person;
                            hostelRequest.Level = level;

                            hostelRequestLogic.Create(hostelRequest);
                        }

                        SetMessage("Your request has been submitted!", Message.Category.Information);
                        return View(viewModel);
                    }
                    if (hostelRequest != null && hostelRequest.Approved)
                    {
                        SetMessage("Your request has been approved proceed to generate invoice!", Message.Category.Information);
                        return View(viewModel);
                    }
                    if (hostelRequest != null && !hostelRequest.Approved)
                    {
                        SetMessage("Your request has not been approved!", Message.Category.Error);
                        return View(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        private static void SetLevel(Model.Model.Student student, StudentLevel studentLevel, HostelRequest hostelRequest)
        {
            try
            {
                if (student.MatricNumber.Contains("/12/") || student.MatricNumber.Contains("/13/") || student.MatricNumber.Contains("/14/") || student.MatricNumber.Contains("/15/") || student.MatricNumber.Contains("/16/"))
                {
                    if (studentLevel.Programme.Id == 1 || studentLevel.Programme.Id == 2)
                    {
                        hostelRequest.Level = new Level() { Id = 2 };
                    }
                    else if (studentLevel.Programme.Id == 3)
                    {
                        hostelRequest.Level = new Level() { Id = 4 };
                    }
                }
                else
                {
                    if (studentLevel.Programme.Id == 1 || studentLevel.Programme.Id == 2)
                    {
                        hostelRequest.Level = new Level() { Id = 1 };
                    }
                    else if (studentLevel.Programme.Id == 3)
                    {
                        hostelRequest.Level = new Level() { Id = 3 };
                    }
                }
            }
            catch (Exception)
            {   
                throw;
            }    
        }

        //public ActionResult ModifyRequest()
        //{
        //    try
        //    {
        //        HostelRequestLogic requestLogic = new HostelRequestLogic();
        //        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

        //        List<HostelRequest> requests = requestLogic.GetModelsBy(h => h.Session_Id == (int)Sessions._20172018 && h.STUDENT.Matric_Number.Contains("/16/"));
        //        for (int i = 0; i < requests.Count; i++)
        //        {
        //            HostelRequest request = requests[i];
        //            if (request.Student != null && request.Student.MatricNumber.Contains("/16/"))
        //            {
        //                StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == request.Student.Id).LastOrDefault();
        //                if (studentLevel.Programme.Id == 1 || studentLevel.Programme.Id == 2)
        //                {
        //                    request.Level = new Level() { Id = 2 };
        //                }
        //                else if (studentLevel.Programme.Id == 3)
        //                {
        //                    request.Level = new Level() { Id = 4 };
        //                }

        //                requestLogic.Modify(request);
        //            }
        //        }

        //        SetMessage("Successful!", Message.Category.Information);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return RedirectToAction("CreateHostelRequest");
        //}

        public ActionResult GenerateHostelInvoice()
        {
            viewModel = new HostelViewModel();
            try
            {
                SetFeeTypeDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        private void SetFeeTypeDropDown(HostelViewModel viewModel)
        {
            try
            {
                FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                if (viewModel.FeeTypeSelectListItem != null && viewModel.FeeTypeSelectListItem.Count > 0)
                {                       
                    viewModel.FeeType = feeTypeLogic.GetModelBy(ft => ft.Fee_Type_Id == (int)FeeTypes.HostelFee);
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
        public ActionResult GenerateHostelInvoice(HostelViewModel viewModel)
        {
            try
            { 
                StudentLogic studentLogic = new StudentLogic();
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                HostelAllocationCriteriaLogic hostelAllocationCriteriaLogic = new HostelAllocationCriteriaLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PersonLogic personLogic = new PersonLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                SessionLogic sessionLogic = new SessionLogic();
                HostelAllocationCountLogic hostelAllocationCountLogic = new HostelAllocationCountLogic();
                HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();

                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                Programme programme = new Programme();
                Department department = new Department();
                Level level = new Level();

                Model.Model.Student student = new Model.Model.Student();
                Person person = new Person();
                Payment payment = new Payment();
                StudentLevel studentLevel = new StudentLevel();
                HostelAllocation hostelAllocation = new HostelAllocation();
                HostelAllocation existingHostelAllocation = new HostelAllocation();

                List<StudentLevel> studentLevels = new List<StudentLevel>();
                viewModel.Session = new Session() { Id = 8 };
                List<HostelAllocationCriteria> hostelAllocationCriteriaList = new List<HostelAllocationCriteria>();

                //Remove due unoccupied bedspaces.
                //CheckAndRemoveDueUnoccupiedBedspaces(viewModel.Session);

                List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                if (students.Count != 1 && viewModel.Student.MatricNumber.Length < 20)
                {
                    SetMessage("Student with this Matriculation Number does not exist Or Matric Number is Duplicate!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }
                
                if (students.Count == 0 && viewModel.Student.MatricNumber.Length > 20)
                {
                    PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.Confirmation_No == viewModel.Student.MatricNumber && (p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.AcceptanceFee || p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.HNDAcceptance) && p.ONLINE_PAYMENT.PAYMENT.Session_Id == viewModel.Session.Id).LastOrDefault();
                    if (paymentEtranzact == null)
                    {
                        SetMessage("Confirmation Order Number is not for Current session's Acceptance Fee!", Message.Category.Error);
                        SetFeeTypeDropDown(viewModel);
                        return View(viewModel);
                    }

                    person = paymentEtranzact.Payment.Payment.Person;
                    AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);
                    if (appliedCourse == null)
                    {
                        SetMessage("No Applied course record!", Message.Category.Error);
                        SetFeeTypeDropDown(viewModel);
                        return View(viewModel);
                    }

                    programme = appliedCourse.Programme;
                    department = appliedCourse.Department;
                    level = new Level() { Id = 1 };
                    if (programme.Id == 3)
                    {
                        level = new Level() { Id = 3 };
                    }
                }
                else
                {
                    student = students.FirstOrDefault();
                    person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                    studentLevels = studentLevelLogic.GetModelsBy(sl => sl.STUDENT.Person_Id == student.Id);
                    if (studentLevels.Count == 0)
                    {
                        SetMessage("You have not registered for this session!", Message.Category.Error);
                        SetFeeTypeDropDown(viewModel);
                        return View(viewModel);
                    }

                    int maxLevelId = studentLevels.Max(sl => sl.Level.Id);
                    studentLevel = studentLevels.LastOrDefault(sl => sl.Level.Id == maxLevelId);
                    viewModel.StudentLevel = studentLevel;
                    programme = studentLevel.Programme;
                    department = studentLevel.Department;
                    level = studentLevel.Level;
                }

                //student = students.FirstOrDefault();
                //person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                viewModel.Person = person;
                

                HostelRequest hostelRequest = hostelRequestLogic.GetModelBy(h => h.Person_Id == person.Id && h.Session_Id == viewModel.Session.Id);
                if (hostelRequest == null)
                {
                    SetMessage("Make a request for hostel allocation before generating invoice!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel); 
                }
                if(hostelRequest != null && !hostelRequest.Approved)
                {
                    SetMessage("Your request for hostel allocation has not been approved!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }

                //studentLevels = studentLevelLogic.GetModelsBy(sl => sl.STUDENT.Person_Id == student.Id);
                //if (studentLevels.Count == 0)
                //{
                //    SetMessage("No StudentLevel Record!", Message.Category.Error);
                //    SetFeeTypeDropDown(viewModel);
                //    return View(viewModel);
                //}
                //int maxLevelId = studentLevels.Max(sl => sl.Level.Id);
                //studentLevel = studentLevels.Where(sl => sl.Level.Id == maxLevelId).LastOrDefault();
                //viewModel.StudentLevel = studentLevel;  

                //PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.ONLINE_PAYMENT.PAYMENT.Session_Id == viewModel.Session.Id && p.ONLINE_PAYMENT.PAYMENT.Person_Id == person.Id && (p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 || p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 10));
                //if (paymentEtranzact == null)
                //{
                //    SetMessage("You have to pay school fees before making payment for hostel allocation!", Message.Category.Error);
                //    SetFeeTypeDropDown(viewModel);
                //    return View(viewModel);
                //}  

                existingHostelAllocation = hostelAllocationLogic.GetModelBy(ha => ha.Session_Id == viewModel.Session.Id && ha.Student_Id == person.Id);
                if (existingHostelAllocation != null)
                {
                    if (existingHostelAllocation.Occupied)
                    {
                        payment = paymentLogic.GetModelBy(p => p.Person_Id == person.Id && p.Fee_Type_Id == existingHostelAllocation.Payment.FeeType.Id && p.Session_Id == existingHostelAllocation.Session.Id);
                        return RedirectToAction("HostelReceipt", new { pmid = payment.Id });
                    }
                    else
                    {
                        payment = paymentLogic.GetModelBy(p => p.Person_Id == person.Id && p.Fee_Type_Id == existingHostelAllocation.Payment.FeeType.Id && p.Session_Id == existingHostelAllocation.Session.Id);
                        viewModel.Payment = payment;
                        TempData["ViewModel"] = viewModel;
                        return RedirectToAction("Invoice"); 
                    }  
                }

                if (person.Sex == null)
                {
                    SetMessage("Error! Ensure that your student profile(Sex) is completely filled", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel); 
                }

                HostelAllocationCount hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Sex_Id == person.Sex.Id && h.Level_Id == level.Id);
                if (hostelAllocationCount.Free == 0)
                {
                    SetMessage("Error! The Set Number for free Bed Spaces for your level has been exausted!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }

                hostelAllocationCriteriaList = hostelAllocationCriteriaLogic.GetModelsBy(hac => hac.Level_Id == level.Id && hac.HOSTEL.HOSTEL_TYPE.Hostel_Type_Name == person.Sex.Name && hac.HOSTEL_ROOM.Reserved == false && hac.HOSTEL_ROOM.Activated && hac.HOSTEL.Activated && hac.HOSTEL_SERIES.Activated && hac.HOSTEL_ROOM_CORNER.Activated);

                if (hostelAllocationCriteriaList.Count == 0)
                {
                    SetMessage("Hostel Allocation Criteria for your Level has not been set!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }

                for (int i = 0; i < hostelAllocationCriteriaList.Count; i++)
                {
                    hostelAllocation.Corner = hostelAllocationCriteriaList[i].Corner;
                    hostelAllocation.Hostel = hostelAllocationCriteriaList[i].Hostel;
                    hostelAllocation.Occupied = false;
                    hostelAllocation.Room = hostelAllocationCriteriaList[i].Room;
                    hostelAllocation.Series = hostelAllocationCriteriaList[i].Series;
                    hostelAllocation.Session = viewModel.Session;
                    hostelAllocation.Student = student;
                    hostelAllocation.Person = person;

                    HostelAllocation allocationCheck = hostelAllocationLogic.GetModelBy(h => h.Corner_Id == hostelAllocation.Corner.Id && h.Hostel_Id == hostelAllocation.Hostel.Id && h.Room_Id == hostelAllocation.Room.Id && h.Series_Id == hostelAllocation.Series.Id && h.Session_Id == hostelAllocation.Session.Id);
                    if (allocationCheck != null)
                    {
                        continue;
                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                        
                        payment = CreatePayment(viewModel, hostelAllocationCriteriaList[i].Hostel);
                        hostelAllocation.Payment = payment;

                        HostelAllocation newHostelAllocation = hostelAllocationLogic.Create(hostelAllocation);
                        
                        hostelAllocationCount.Free -= 1;
                        hostelAllocationCount.TotalCount -= 1;
                        hostelAllocationCount.LastModified = DateTime.Now;
                        hostelAllocationCountLogic.Modify(hostelAllocationCount);

                        scope.Complete();   
                    }

                    viewModel.Student = student;
                    viewModel.StudentLevel = studentLevel;
                    viewModel.Payment = payment;
                    TempData["ViewModel"] = viewModel;

                    return RedirectToAction("Invoice");
                }

                SetMessage("No free bedspace! Contact your system administrator.", Message.Category.Error);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            SetFeeTypeDropDown(viewModel);
            return View(viewModel);
        }

        //private void CheckAndRemoveDueUnoccupiedBedspaces(Model.Model.Session session)
        //{
        //    try
        //    {
        //        //start date
        //        DateTime startDate = new DateTime(2017, 10, 26);

        //        HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();

        //        List<HostelAllocation> hostelAllocations = hostelAllocationLogic.GetModelsBy(h => h.Session_Id == session.Id && h.PAYMENT.Date_Paid > startDate && (DateTime.Now - h.PAYMENT.Date_Paid) > 3)

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        private Payment CreatePayment(HostelViewModel viewModel, Hostel hostel)
        {
            
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();

                Payment newPayment = new Payment();

                PaymentMode paymentMode = new PaymentMode(){Id = 1};
                PaymentType paymentType = new PaymentType(){Id = 2};
                PersonType personType = viewModel.Person.Type;
                FeeType feeType = new FeeType() { Id = (int)FeeTypes.HostelFee };

                Payment payment = new Payment();
                payment.PaymentMode = paymentMode;
                payment.PaymentType = paymentType;
                payment.PersonType = personType;
                payment.FeeType = feeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = viewModel.Person;
                payment.Session = viewModel.Session;

                Payment checkPayment = paymentLogic.GetModelBy(p => p.Person_Id == viewModel.Person.Id && p.Fee_Type_Id == feeType.Id && p.Session_Id == viewModel.Session.Id);
                if (checkPayment != null)
                {
                    newPayment = checkPayment;
                }
                else
                {
                    newPayment = paymentLogic.Create(payment); 
                }

                OnlinePayment newOnlinePayment = null;
                
                if (newPayment != null)
                {
                    OnlinePayment onlinePaymentCheck = onlinePaymentLogic.GetModelBy(op => op.Payment_Id == newPayment.Id);
                    if (onlinePaymentCheck == null)
                    {
                        PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                        OnlinePayment onlinePayment = new OnlinePayment();
                        onlinePayment.Channel = channel;
                        onlinePayment.Payment = newPayment;
                        newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                    }
                    
                }

                HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                HostelFee hostelFee = new HostelFee();

                HostelFee existingHostelFee = hostelFeeLogic.GetModelsBy(h => h.Payment_Id == newPayment.Id).LastOrDefault();

                if (existingHostelFee == null)
                {
                    hostelFee.Hostel = hostel;
                    hostelFee.Payment = newPayment;
                    hostelFee.Amount = GetHostelFee(hostel);

                    hostelFeeLogic.Create(hostelFee);
                }

                newPayment.Amount = GetHostelFee(hostel).ToString(); 

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private double GetHostelFee(Hostel hostel)
        //{
        //    double amount = 0;
        //    try
        //    {
        //        string[] firstHostelGroup = {"KINGS PALACE", "KINGS ANNEX(A)", "KINGS ANNEX(B)", "ALUTA BASE", "ALUTA BASE(ANNEX)", "QUEENS PALACE(ANNEX)"};
        //        string[] secondHostelGroup = {"QUEENS PALACE I", "QUEENS PALACE II", "QUEENS PALACE III"};

        //        if (firstHostelGroup.Contains(hostel.Name))
        //        {
        //            amount = 13000;
        //        }
        //        if (secondHostelGroup.Contains(hostel.Name))
        //        {
        //            amount = 11500; 
        //        }
        //    }
        //    catch (Exception)
        //    {   
        //        throw;
        //    }

        //    return amount;
        //}
        private double GetHostelFee(Hostel hostel)
        {
            double amount = 0;
            HostelAmountLogic hostelAmountLogic = new HostelAmountLogic();
            HostelAmount hostelAmount = new HostelAmount();
            try
            {
                hostelAmount = hostelAmountLogic.GetModelBy(p => p.Hostel_Id == hostel.Id);

                amount = Convert.ToDouble(hostelAmount.Amount);
            }
            catch (Exception)
            {
                throw;
            }

            return amount;
        }
        public ActionResult Invoice()
        {
            viewModel = (HostelViewModel)TempData["ViewModel"];
            try
            {
                //Int64 paymentid = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(pmid));
                PaymentLogic paymentLogic = new PaymentLogic();
                Payment payment = paymentLogic.GetModelBy(p => p.Payment_Id == viewModel.Payment.Id);
                if (payment.FeeType.Id == (int)FeeTypes.HostelFee)
                {
                    Invoice invoice = new Invoice();
                    invoice.Person = payment.Person;
                    invoice.Payment = payment;

                    Model.Model.Student student = new Model.Model.Student();
                    StudentLogic studentLogic = new StudentLogic();
                    student = studentLogic.GetBy(payment.Person.Id);

                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                    PaymentEtranzactTypeLogic PaymentEtranzactTypeLogic = new Business.PaymentEtranzactTypeLogic();

                    paymentEtranzactType = PaymentEtranzactTypeLogic.GetModelBy(p => p.Fee_Type_Id == payment.FeeType.Id && p.Session_Id == payment.Session.Id);

                    if (student != null)
                    {
                        invoice.MatricNumber = student.MatricNumber; 
                    } 

                    invoice.paymentEtranzactType = paymentEtranzactType;

                    PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(payment);
                    if (paymentEtranzact != null)
                    {
                        invoice.Paid = true;
                    }

                    HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                    HostelFee hostelFee = hostelFeeLogic.GetModelBy(h => h.Payment_Id == payment.Id);

                    invoice.Amount = Convert.ToDecimal(hostelFee.Amount);

                    invoice.Payment.FeeDetails = null;

                    return View(invoice);
                } 
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        public ActionResult PayHostelFee()
        {
            try
            {
                 viewModel = new HostelViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult PayHostelFee(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.ConfirmationOrder != null)
                {
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    PaymentLogic paymentLogic = new PaymentLogic();
                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();

                    if (viewModel.ConfirmationOrder.Length > 12)
                    {
                        Model.Model.Session session = new Model.Model.Session() { Id = (int)Sessions._20172018 };
                        FeeType feetype = new FeeType() { Id = (int)FeeTypes.HostelFee };
                        Payment payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrder, feetype.Id);
                        if (payment != null && payment.Id > 0)
                        {
                            if (payment.FeeType.Id != (int)FeeTypes.HostelFee)
                            {
                                SetMessage("Confirmation Order Number (" + viewModel.ConfirmationOrder + ") entered is not for Hostel Fee payment! Please enter your Hostel Fee Confirmation Order Number.", Message.Category.Error);
                                return View(viewModel);
                            }

                            HostelAllocation hostelAllocation = hostelAllocationLogic.GetModelBy(ha => ha.Student_Id == payment.Person.Id && ha.Session_Id == payment.Session.Id);

                            if (hostelAllocation != null)
                            {
                                hostelAllocation.Occupied = true;
                                hostelAllocationLogic.Modify(hostelAllocation);
                            }
                            else
                            {
                                SetMessage("Allocation does not exist, this could be because you didn't pay within the specified time. Contact your administrator.", Message.Category.Error);
                                return RedirectToAction("HostelReceipt");
                            }

                            return RedirectToAction("HostelReceipt", new { pmid = payment.Id });
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
        public ActionResult HostelReceipt(long pmid)
        {
            try
            {
                viewModel = new HostelViewModel();
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                HostelFee hostelFee = new HostelFee();
                Payment payment = new Payment();

                payment = paymentLogic.GetModelBy(p => p.Payment_Id == pmid);
                hostelFee = hostelFeeLogic.GetModelBy(h => h.Payment_Id == pmid);
                HostelAllocation hostelAllocation = hostelAllocationLogic.GetModelBy(ha => ha.Payment_Id == pmid && ha.Session_Id == payment.Session.Id && ha.Student_Id == payment.Person.Id);

                if (hostelAllocation != null)
                {
                    viewModel.HostelAllocation = hostelAllocation;
                    viewModel.HostelFee = hostelFee;
                    return View(viewModel);
                }    
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
    }
}