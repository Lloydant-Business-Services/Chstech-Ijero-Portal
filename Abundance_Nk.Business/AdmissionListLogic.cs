using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class AdmissionListLogic : BusinessBaseLogic<AdmissionList, ADMISSION_LIST>
    {
        public AdmissionListLogic()
        {
            translator = new AdmissionListTranslator();
        }

        public AdmissionList GetBy(long applicationFormId)
        {
            try
            {
                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.Application_Form_Id == applicationFormId && a.Activated == true;

                AdmissionList admission = GetModelsBy(selector).FirstOrDefault();
                if (admission != null && admission.Id > 0)
                {

                }

                return admission;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public AdmissionList GetBy(Person person)
        {
            try
            {
                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.APPLICATION_FORM.Person_Id == person.Id;

                AdmissionList admission = GetModelsBy(selector).FirstOrDefault();
                if (admission != null && admission.Id > 0)
                {

                }

                return admission;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public bool IsAdmitted(ApplicationForm applicationForm)
        {
            try
            {
                bool admitted = false;

                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.Application_Form_Id == applicationForm.Id && a.Activated == true;
                AdmissionList admissionList = GetModelBy(selector);
                if (admissionList != null && admissionList.Id > 0)
                {
                    admitted = true;
                }

                return admitted;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsAdmitted(string ApplicationNumber)
        {
            try
            {
                bool admitted = false;

                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.APPLICATION_FORM.Application_Form_Number == ApplicationNumber && a.Activated == true;
                AdmissionList admissionList = GetModelBy(selector);
                if (admissionList != null && admissionList.Id > 0)
                {
                    admitted = true;
                }

                return admitted;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AdmissionList Create(AdmissionList admissionlist, AdmissionListBatch batch, AdmissionListAudit Audit)
        {
            AdmissionList List = new AdmissionList();
            try
            {
                AdmissionListAuditLogic auditLogic = new AdmissionListAuditLogic();
                admissionlist.Batch = batch;
                if (!IsAdmitted(admissionlist.Form))
                {
                    List = base.Create(admissionlist);
                    Audit.AdmissionList = admissionlist;
                    Audit.AdmissionList.Id = List.Id;
                    Audit.Form = admissionlist.Form;
                    Audit.Deprtment = admissionlist.Deprtment;
                    Audit.DepartmentOption = admissionlist.DepartmentOption;
                    auditLogic.Create(Audit);
                }
            }
            catch (Exception e)
            {
                
                throw;
            }
          
            return List;
            
        }

        public bool IsValidApplicationNumberAndPin(string applicationNumber, string pin)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = af => af.Application_Form_Number == applicationNumber;
                ApplicationForm applicationForm = new ApplicationForm();
                ApplicationFormLogic formLogic = new ApplicationFormLogic();
                applicationForm = formLogic.GetModelBy(selector);
                if (applicationForm == null)
                {
                    return false;
                }

                if (!IsAdmitted(applicationForm))
                {
                    return false;
                }
                FeeType feetype;
                if (applicationForm.ProgrammeFee.Programme.Id == 1)
                {
                    feetype  = new FeeType() { Id = 7};
                }
                else 
                {
                    feetype = new FeeType() { Id = 8 };
                }
                ScratchCardLogic paymentLogic = new ScratchCardLogic();
              ;
              if (!paymentLogic.ValidatePin(pin, feetype))
                {
                    return false;
                }
              bool pinUseStatus = paymentLogic.IsPinUsed(pin, applicationForm.Person.Id);
                  if (!pinUseStatus)
                  {
                      return true;
                  }


                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public bool HasStudentCheckedStatus(long fid)
        {
            try
            {
                ApplicationForm appForm = new ApplicationForm();
                ApplicationFormLogic appFormLogic = new ApplicationFormLogic();
                appForm = appFormLogic.GetModelBy(a => a.Application_Form_Id == fid);
                if (appForm != null)
                {
                    ScratchCardLogic paymentLogic = new ScratchCardLogic();
                    ScratchCard payment = new ScratchCard();
                   payment = paymentLogic.GetModelsBy(s => s.Person_Id == appForm.Person.Id).FirstOrDefault();
                   if (payment != null)
                    {
                        return true;
                    }
                       
                }
               
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return false;
        }
        public bool Update(AdmissionList admissionlist, AdmissionListAudit Audit)
        {
           
            try
            {
                AdmissionListAuditLogic auditLogic = new AdmissionListAuditLogic();
                if (IsAdmitted(admissionlist.Form))
                {
                    Expression<Func<ADMISSION_LIST, bool>> selector = a => a.Admission_List_Id == admissionlist.Id;
                    ADMISSION_LIST List = GetEntityBy(selector);
                    if (List != null && List.Admission_List_Id > 0)
                    {
                        List.Department_Id = admissionlist.Deprtment.Id;
                        if (admissionlist.Programme != null && admissionlist.Programme.Id > 0)
                        {
                            List.Programme_Id = admissionlist.Programme.Id;
                        }
                        if (admissionlist.DepartmentOption != null && admissionlist.DepartmentOption.Id > 0)
                        {
                            List.Department_Option_Id = admissionlist.DepartmentOption.Id;
                        }
                        else
                        {
                            List.Department_Option_Id = null;
                        }

                        int modifiedRecordCount = Save();

                       if (modifiedRecordCount > 0)
                       {
                           Audit.AdmissionList = admissionlist;
                           Audit.AdmissionList.Id = List.Admission_List_Id;
                           Audit.Form = admissionlist.Form;
                           Audit.Deprtment = admissionlist.Deprtment;
                           Audit.DepartmentOption = admissionlist.DepartmentOption;
                           Audit.Time = DateTime.Now;
                           auditLogic.Create(Audit);
                           return true;
                       }
                    }

                    return false;
                }
            }
            catch (Exception e)
            {

                throw;
            }

            return false;

        }
        public bool Modify(AdmissionList admissionlist, AdmissionListAudit admissionListAudit)
        {

            try
            {
                AdmissionListAuditLogic auditLogic = new AdmissionListAuditLogic();

                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.Admission_List_Id == admissionlist.Id;
                ADMISSION_LIST List = GetEntityBy(selector);

                if (List != null && List.Admission_List_Id > 0)
                {
                    if (admissionlist.Deprtment != null)
                    {
                        List.Department_Id = admissionlist.Deprtment.Id; 
                    }
                    if (admissionlist.Programme != null)
                    {
                        List.Programme_Id = admissionlist.Programme.Id;
                    }
                    if (admissionlist.DepartmentOption != null && admissionlist.DepartmentOption.Id > 0)
                    {
                        List.Department_Option_Id = admissionlist.DepartmentOption.Id;
                    }

                    List.Activated = admissionlist.Activated;

                    int modifiedRecordCount = Save();

                    if (modifiedRecordCount > 0)
                    {
                        admissionListAudit.AdmissionList = admissionlist;
                        admissionListAudit.AdmissionList.Id = List.Admission_List_Id;
                        admissionListAudit.Form = admissionlist.Form;
                        admissionListAudit.Deprtment = admissionlist.Deprtment;
                        admissionListAudit.DepartmentOption = admissionlist.DepartmentOption;
                        admissionListAudit.Time = DateTime.Now;
                        auditLogic.Create(admissionListAudit);

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }

            return false;
        }
        public bool ModifyListOnly(AdmissionList admissionlist)
        {
            try
            {
                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.Admission_List_Id == admissionlist.Id;
                ADMISSION_LIST List = GetEntityBy(selector);

                if (List != null && List.Admission_List_Id > 0)
                {
                    if (admissionlist.Deprtment != null)
                    {
                        List.Department_Id = admissionlist.Deprtment.Id;
                    }
                    if (admissionlist.DepartmentOption != null)
                    {
                        List.Department_Id = admissionlist.DepartmentOption.Id;
                    }
                    if (admissionlist.Form != null)
                    {
                        List.Application_Form_Id = admissionlist.Form.Id;
                    }

                    List.Activated = admissionlist.Activated;

                    int modifiedRecordCount = Save();

                    if (modifiedRecordCount > 0)
                    {                                
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
        public List<AdmissionListReportFormat> GetAdmissionCount(Session session)
        {
            try
            {
                List<AdmissionListReportFormat> admissionList = new List<AdmissionListReportFormat>();

                if (session == null || session.Id <= 0)
                {
                    throw new Exception("One or more criteria to get admission list not set! Please check your input criteria selection and try again.");
                }

                admissionList = (from sr in repository.GetBy<VW_ADMISSION_LIST>(x => x.Session_Id == session.Id)
                                 select new AdmissionListReportFormat()
                                 {
                                     Session = sr.Session_Name,
                                     ExamNumber = sr.Application_Exam_Number,
                                     Sex = sr.Sex_Name,
                                     Name = sr.FullName,
                                     ProgrammeCode = sr.Programme_Short_Name,
                                     ProgrammeName = sr.Programme_Name,
                                     DepartmentCode = sr.Department_Code,
                                     DepartmentName = sr.Department_Name,
                                     ApplicationNumber = sr.Application_Form_Number

                                 }).ToList();
                int count = 0;
                count = admissionList.Count;
                for (int i = 0; i < admissionList.Count; i++)
                {
                    admissionList[i].TotalCount = count;
                    admissionList[i].Count = 1;
                }

                return admissionList.OrderBy(a => a.ApplicationNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<AdmissionListReportFormat> GetListBy(Session session, Programme programme, Department department)
        {
            try
            {
                List<AdmissionListReportFormat> admissionList = new List<AdmissionListReportFormat>();

                if (programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || session == null || session.Id <= 0)
                {
                    throw new Exception("One or more criteria to get admission list not set! Please check your input criteria selection and try again.");
                }

                admissionList = (from sr in repository.GetBy<VW_ADMISSION_LIST>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id)
                                 select new AdmissionListReportFormat()
                                 {
                                     Session = sr.Session_Name,
                                     ExamNumber = sr.Application_Exam_Number,
                                     Sex = sr.Sex_Name,
                                     Name = sr.FullName,
                                     ProgrammeCode = sr.Programme_Name,
                                     ProgrammeName = sr.Programme_Name,
                                     DepartmentCode = sr.Department_Code,
                                     DepartmentName = sr.Department_Name,
                                     ApplicationNumber = sr.Application_Form_Number

                                 }).ToList();
                int count = 0;
                count = admissionList.Count;
                for (int i = 0; i < admissionList.Count; i++)
                {
                    admissionList[i].TotalCount = count;
                }

                return admissionList.OrderBy(a => a.ApplicationNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<AdmissionSummary> GetSummary(Session session)
        {
            try
            {
                List<AdmissionSummary> admissionSummary = (from a in repository.GetBy<VW_ADMISSION_SUMMARY>(a => a.Session_Id == session.Id)
                                                           select new AdmissionSummary
                                                           {
                                                               Programme_Id = a.Programme_Id,
                                                               Programme_Name = a.Programme_Name,
                                                               Department_Name = a.Department_Name,
                                                               Session_Name = a.Session_Name,
                                                               AdmissionCount = (int)a.AdmissionCount,
                                                           }).ToList();

                return admissionSummary;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }




}
