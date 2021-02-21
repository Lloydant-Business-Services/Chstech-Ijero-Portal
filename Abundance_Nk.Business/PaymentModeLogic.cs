using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Business
{
    public class PaymentModeLogic : BusinessBaseLogic<PaymentMode, PAYMENT_MODE>
    {
        public PaymentModeLogic()
        {
            translator = new PaymentModeTranslator();
        }

        public bool Modify(PaymentMode paymentMode)
        {
            try
            {
                Expression<Func<PAYMENT_MODE, bool>> selector = p => p.Payment_Mode_Id == paymentMode.Id;
                PAYMENT_MODE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Payment_Mode_Name = paymentMode.Name;
                entity.Payment_Mode_Description = paymentMode.Description;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PaymentView> GetPaymentCount(Session session, PaymentMode paymentMode, string dateFrom, string dateTo)
        {
            List<PaymentView> paymentsOld = new List<PaymentView>();
            List<PaymentView> paymentsNew = new List<PaymentView>();
            List<PaymentView> Masterpayments = new List<PaymentView>();

            try
            {
                int[] ndProgrammes = { 1, 2, 5 };
                int[] hndProgrammes = { 3, 4 };
                int[] otherProgrammes = { 6, 7, 8 };
                string[] ndProgrammeNames = { "ND Morning (JAMB)", "ND Evening", "ND Part Time", };
                string[] hndProgrammeNames = { "HND Morning", "HND Evening", };
                string[] otherProgrammeNames = { "PRE-ND Programme", "Certificate", "Advance Certificate" };

                int NDCount = 0;
                int HNDCount = 0;
                int OtherProgrammesCount = 0;
                int NewStudentCount = 0;
                int OldStudentCount = 0;
                int TotalCount = 0;

                if (paymentMode.Id == 100)
                {
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        DateTime paymentFrom = ConvertToDate(dateFrom);
                        DateTime paymentTo = ConvertToDate(dateTo);

                        if (session.Id == (int)Sessions._20162017)
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id) && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }
                        else
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }

                        //OldStudentCount = paymentsOld.Count;

                        List<long> personIdListOld = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < personIdListOld.Count; i++)
                        {
                            PaymentView currentPerson = paymentsOld.Where(p => p.PersonId == personIdListOld[i]).LastOrDefault();

                            if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND II";
                                NDCount += 1;
                            }
                            else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND II";
                                HNDCount += 1;
                            }
                            else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                OtherProgrammesCount += 1;
                            }
                            else
                            {
                                continue;
                            }

                            OldStudentCount += 1;
                            Masterpayments.Add(currentPerson);
                        }

                        paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_COMPREHENSIVE>(x => x.Admitted_Session_Id == session.Id && x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo)
                                       select new PaymentView
                                       {
                                           PersonId = sr.Person_Id,
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           MatricNumber = sr.Application_Form_Number,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           PaymentModeId = sr.Payment_Mode_Id,
                                           TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           Amount = sr.Transaction_Amount,
                                           PaymentCount = 1
                                       }).ToList();

                        //NewStudentCount = paymentsNew.Count;

                        List<long> personIdListNew = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < personIdListNew.Count; i++)
                        {
                            PaymentView currentPerson = paymentsNew.Where(p => p.PersonId == personIdListNew[i]).LastOrDefault();

                            if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND I";
                                NDCount += 1;
                            }
                            else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND I";
                                HNDCount += 1;
                            }
                            else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                OtherProgrammesCount += 1;
                            }
                            else
                            {
                                continue;
                            }

                            NewStudentCount += 1;
                            Masterpayments.Add(currentPerson);
                        }

                    }
                    else
                    {
                        if (session.Id == (int)Sessions._20162017)
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }
                        else
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }

                        // OldStudentCount = paymentsOld.Count;

                        List<long> personIdListOld = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < personIdListOld.Count; i++)
                        {
                            PaymentView currentPerson = paymentsOld.Where(p => p.PersonId == personIdListOld[i]).LastOrDefault();

                            if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND II";
                                NDCount += 1;
                            }
                            else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND II";
                                HNDCount += 1;
                            }
                            else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                OtherProgrammesCount += 1;
                            }
                            else
                            {
                                continue;
                            }

                            OldStudentCount += 1;
                            Masterpayments.Add(currentPerson);
                        }

                        paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_COMPREHENSIVE>(x => x.Admitted_Session_Id == session.Id)
                                       select new PaymentView
                                       {
                                           PersonId = sr.Person_Id,
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           MatricNumber = sr.Application_Form_Number,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           PaymentModeId = sr.Payment_Mode_Id,
                                           TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           Amount = sr.Transaction_Amount,
                                           PaymentCount = 1
                                       }).ToList();

                        //NewStudentCount = paymentsNew.Count;

                        List<long> personIdListNew = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < personIdListNew.Count; i++)
                        {
                            PaymentView currentPerson = paymentsNew.Where(p => p.PersonId == personIdListNew[i]).LastOrDefault();

                            if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND I";
                                NDCount += 1;
                            }
                            else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND I";
                                HNDCount += 1;
                            }
                            else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                OtherProgrammesCount += 1;
                            }
                            else
                            {
                                continue;
                            }

                            NewStudentCount += 1;
                            Masterpayments.Add(currentPerson);
                        }
                    }
                }
                if (paymentMode.Id == 2)
                {
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        DateTime paymentFrom = ConvertToDate(dateFrom);
                        DateTime paymentTo = ConvertToDate(dateTo);

                        if (session.Id == (int)Sessions._20162017)
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id) && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }
                        else
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }


                        List<long> distinctPersonOld = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonOld.Count; i++)
                        {
                            PaymentView firstInstallment = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                            PaymentView secondInstallment = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                            if (secondInstallment == null)
                            {
                                PaymentView currentPerson = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i]).LastOrDefault();

                                if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND II";
                                    NDCount += 1;
                                }
                                else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND II";
                                    HNDCount += 1;
                                }
                                else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    OtherProgrammesCount += 1;
                                }
                                else
                                {
                                    continue;
                                }

                                OldStudentCount += 1;
                                Masterpayments.Add(currentPerson);

                                //payments.Add(firstInstallment);
                            }
                        }

                        paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FIRST>(x => x.Admitted_Session_Id == session.Id && x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo)
                                       select new PaymentView
                                       {
                                           PersonId = sr.Person_Id,
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           MatricNumber = sr.Application_Form_Number,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           PaymentModeId = sr.Payment_Mode_Id,
                                           TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           Amount = sr.Transaction_Amount,
                                           PaymentCount = 1
                                       }).ToList();

                        List<long> distinctPersonNew = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonNew.Count; i++)
                        {
                            PaymentView firstInstallment = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                            PaymentView secondInstallment = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                            if (secondInstallment == null)
                            {
                                PaymentView currentPerson = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i]).LastOrDefault();

                                if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND I";
                                    NDCount += 1;
                                }
                                else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND I";
                                    HNDCount += 1;
                                }
                                else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    OtherProgrammesCount += 1;
                                }
                                else
                                {
                                    continue;
                                }

                                NewStudentCount += 1;
                                Masterpayments.Add(currentPerson);

                                //payments.Add(firstInstallment);
                            }
                        }
                    }
                    else
                    {
                        if (session.Id == (int)Sessions._20162017)
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }
                        else
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }


                        List<long> distinctPersonOld = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonOld.Count; i++)
                        {
                            PaymentView firstInstallment = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                            PaymentView secondInstallment = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                            if (secondInstallment == null)
                            {
                                PaymentView currentPerson = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i]).LastOrDefault();

                                if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND II";
                                    NDCount += 1;
                                }
                                else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND II";
                                    HNDCount += 1;
                                }
                                else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    OtherProgrammesCount += 1;
                                }
                                else
                                {
                                    continue;
                                }

                                OldStudentCount += 1;
                                Masterpayments.Add(currentPerson);

                                //payments.Add(firstInstallment);
                            }
                        }

                        paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FIRST>(x => x.Admitted_Session_Id == session.Id)
                                       select new PaymentView
                                       {
                                           PersonId = sr.Person_Id,
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           MatricNumber = sr.Application_Form_Number,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           PaymentModeId = sr.Payment_Mode_Id,
                                           TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           Amount = sr.Transaction_Amount,
                                           PaymentCount = 1
                                       }).ToList();

                        List<long> distinctPersonNew = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonNew.Count; i++)
                        {
                            PaymentView firstInstallment = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                            PaymentView secondInstallment = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                            if (secondInstallment == null)
                            {
                                PaymentView currentPerson = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i]).LastOrDefault();

                                if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND I";
                                    NDCount += 1;
                                }
                                else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND I";
                                    HNDCount += 1;
                                }
                                else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    OtherProgrammesCount += 1;
                                }
                                else
                                {
                                    continue;
                                }

                                NewStudentCount += 1;
                                Masterpayments.Add(currentPerson);

                                //payments.Add(firstInstallment);
                            }
                        }
                    }
                }
                if (paymentMode.Id == 3)
                {
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        DateTime paymentFrom = ConvertToDate(dateFrom);
                        DateTime paymentTo = ConvertToDate(dateTo);

                        if (session.Id == (int)Sessions._20162017)
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id) && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }
                        else
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }


                        List<long> distinctPersonOld = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonOld.Count; i++)
                        {
                            PaymentView firstInstallment = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                            PaymentView secondInstallment = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                            if (secondInstallment != null)
                            {
                                PaymentView currentPerson = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i]).LastOrDefault();

                                if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND II";
                                    NDCount += 1;
                                }
                                else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND II";
                                    HNDCount += 1;
                                }
                                else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    OtherProgrammesCount += 1;
                                }
                                else
                                {
                                    continue;
                                }

                                OldStudentCount += 1;
                                Masterpayments.Add(currentPerson);
                                //payments.Add(firstInstallment);
                                //payments.Add(secondInstallment);
                            }
                        }

                        paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FIRST>(x => x.Admitted_Session_Id == session.Id && x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo)
                                       select new PaymentView
                                       {
                                           PersonId = sr.Person_Id,
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           MatricNumber = sr.Application_Form_Number,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           PaymentModeId = sr.Payment_Mode_Id,
                                           TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           Amount = sr.Transaction_Amount,
                                           PaymentCount = 1
                                       }).ToList();

                        List<long> distinctPersonNew = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonNew.Count; i++)
                        {
                            PaymentView firstInstallment = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                            PaymentView secondInstallment = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                            if (secondInstallment != null)
                            {
                                PaymentView currentPerson = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i]).LastOrDefault();

                                if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND I";
                                    NDCount += 1;
                                }
                                else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND I";
                                    HNDCount += 1;
                                }
                                else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    OtherProgrammesCount += 1;
                                }
                                else
                                {
                                    continue;
                                }

                                NewStudentCount += 1;
                                Masterpayments.Add(currentPerson);
                                //payments.Add(firstInstallment);
                                //payments.Add(secondInstallment);
                            }
                        }
                    }
                    else
                    {
                        if (session.Id == (int)Sessions._20162017)
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }
                        else
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               PaymentCount = 1
                                           }).ToList();
                        }

                        List<long> distinctPersonOld = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonOld.Count; i++)
                        {
                            PaymentView firstInstallment = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                            PaymentView secondInstallment = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                            if (secondInstallment != null)
                            {
                                PaymentView currentPerson = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i]).LastOrDefault();

                                if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND II";
                                    NDCount += 1;
                                }
                                else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND II";
                                    HNDCount += 1;
                                }
                                else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    OtherProgrammesCount += 1;
                                }
                                else
                                {
                                    continue;
                                }

                                OldStudentCount += 1;
                                Masterpayments.Add(currentPerson);
                                //payments.Add(firstInstallment);
                                //payments.Add(secondInstallment);
                            }
                        }

                        paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FIRST>(x => x.Admitted_Session_Id == session.Id)
                                       select new PaymentView
                                       {
                                           PersonId = sr.Person_Id,
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           MatricNumber = sr.Application_Form_Number,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           PaymentModeId = sr.Payment_Mode_Id,
                                           TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           Amount = sr.Transaction_Amount,
                                           PaymentCount = 1
                                       }).ToList();

                        List<long> distinctPersonNew = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonNew.Count; i++)
                        {
                            PaymentView firstInstallment = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                            PaymentView secondInstallment = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                            if (secondInstallment != null)
                            {
                                PaymentView currentPerson = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i]).LastOrDefault();

                                if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND I";
                                    NDCount += 1;
                                }
                                else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND I";
                                    HNDCount += 1;
                                }
                                else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                                {
                                    OtherProgrammesCount += 1;
                                }
                                else
                                {
                                    continue;
                                }

                                NewStudentCount += 1;
                                Masterpayments.Add(currentPerson);
                                //payments.Add(firstInstallment);
                                //payments.Add(secondInstallment);
                            }
                        }
                    }
                }
                if (paymentMode.Id == 1)
                {
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        DateTime paymentFrom = ConvertToDate(dateFrom);
                        DateTime paymentTo = ConvertToDate(dateTo);

                        if (session.Id == (int)Sessions._20162017)
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FULL>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id) && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               FullPaymentAmount = sr.Transaction_Amount,
                                               FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               PaymentCount = 1
                                           }).ToList();
                        }
                        else
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FULL>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               FullPaymentAmount = sr.Transaction_Amount,
                                               FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               PaymentCount = 1
                                           }).ToList();
                        }


                        List<long> distinctPersonOld = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonOld.Count; i++)
                        {
                            PaymentView currentPerson = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i]).LastOrDefault();

                            if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND II";
                                NDCount += 1;
                            }
                            else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND II";
                                HNDCount += 1;
                            }
                            else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                OtherProgrammesCount += 1;
                            }
                            else
                            {
                                continue;
                            }

                            OldStudentCount += 1;
                            Masterpayments.Add(currentPerson);
                        }

                        paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FULL>(x => x.Admitted_Session_Id == session.Id && x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo)
                                       select new PaymentView
                                       {
                                           PersonId = sr.Person_Id,
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           MatricNumber = sr.Application_Form_Number,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           PaymentModeId = sr.Payment_Mode_Id,
                                           TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           Amount = sr.Transaction_Amount,
                                           FullPaymentAmount = sr.Transaction_Amount,
                                           FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           PaymentCount = 1
                                       }).ToList();

                        List<long> distinctPersonNew = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonNew.Count; i++)
                        {
                            PaymentView currentPerson = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i]).LastOrDefault();

                            if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND I";
                                NDCount += 1;
                            }
                            else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND I";
                                HNDCount += 1;
                            }
                            else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                OtherProgrammesCount += 1;
                            }
                            else
                            {
                                continue;
                            }

                            NewStudentCount += 1;
                            Masterpayments.Add(currentPerson);
                        }

                    }
                    else
                    {
                        if (session.Id == (int)Sessions._20162017)
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FULL>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               FullPaymentAmount = sr.Transaction_Amount,
                                               FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               PaymentCount = 1
                                           }).ToList();
                        }
                        else
                        {
                            paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FULL>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                           select new PaymentView
                                           {
                                               PersonId = sr.Person_Id,
                                               SessionId = sr.Session_Id,
                                               SessionName = sr.Session_Name,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number,
                                               ProgrammeId = sr.Programme_Id,
                                               ProgrammeName = sr.Programme_Name,
                                               DepartmentId = sr.Department_Id,
                                               DepartmentName = sr.Department_Name,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               Amount = sr.Transaction_Amount,
                                               FullPaymentAmount = sr.Transaction_Amount,
                                               FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString(),
                                               PaymentCount = 1
                                           }).ToList();
                        }


                        List<long> distinctPersonOld = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonOld.Count; i++)
                        {
                            PaymentView currentPerson = paymentsOld.Where(p => p.PersonId == distinctPersonOld[i]).LastOrDefault();

                            if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND II";
                                NDCount += 1;
                            }
                            else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND II";
                                HNDCount += 1;
                            }
                            else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                OtherProgrammesCount += 1;
                            }
                            else
                            {
                                continue;
                            }

                            OldStudentCount += 1;
                            Masterpayments.Add(currentPerson);
                        }

                        paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FULL>(x => x.Admitted_Session_Id == session.Id)
                                       select new PaymentView
                                       {
                                           PersonId = sr.Person_Id,
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           MatricNumber = sr.Application_Form_Number,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           PaymentModeId = sr.Payment_Mode_Id,
                                           TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           Amount = sr.Transaction_Amount,
                                           FullPaymentAmount = sr.Transaction_Amount,
                                           FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString(),
                                           PaymentCount = 1
                                       }).ToList();

                        List<long> distinctPersonNew = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                        for (int i = 0; i < distinctPersonNew.Count; i++)
                        {
                            PaymentView currentPerson = paymentsNew.Where(p => p.PersonId == distinctPersonNew[i]).LastOrDefault();

                            if (ndProgrammes.Contains(currentPerson.ProgrammeId) && ndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " ND I";
                                NDCount += 1;
                            }
                            else if (hndProgrammes.Contains(currentPerson.ProgrammeId) && hndProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                currentPerson.ProgrammeName = currentPerson.ProgrammeName + " HND I";
                                HNDCount += 1;
                            }
                            else if (otherProgrammes.Contains(currentPerson.ProgrammeId) && otherProgrammeNames.Contains(currentPerson.ProgrammeName))
                            {
                                OtherProgrammesCount += 1;
                            }
                            else
                            {
                                continue;
                            }

                            NewStudentCount += 1;
                            Masterpayments.Add(currentPerson);
                        }
                    }
                }

                for (int i = 0; i < Masterpayments.Count; i++)
                {
                    Masterpayments[i].TotalCount = Masterpayments.Count;
                    Masterpayments[i].NewStudentDebtorsCount = NewStudentCount;
                    Masterpayments[i].OldStudentDebtorsCount = OldStudentCount;
                    Masterpayments[i].NDCount = NDCount;
                    Masterpayments[i].HNDCount = HNDCount;
                    Masterpayments[i].OtherProgrammesCount = OtherProgrammesCount;
                }

                return Masterpayments;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PaymentView> GetComprehensivePayment(Session session, Level level, Programme programme, Department department, PaymentMode paymentMode, string dateFrom, string dateTo)
        {
            List<PaymentView> payments = new List<PaymentView>();
            List<PaymentView> paymentsOld = new List<PaymentView>();
            List<PaymentView> paymentsNew = new List<PaymentView>();
            List<PaymentView> Masterpayments = new List<PaymentView>();
            //List<StudentDetailFormat> students = new List<StudentDetailFormat>();

            //PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            LevelLogic levelLogic = new LevelLogic();
            level = levelLogic.GetModelBy(l => l.Level_Id == level.Id);
            if (level == null)
            {
                level = new Level();
                level.Id = 1001;
                level.Name = "All LevelList";
            }

            try
            {
                if (programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || session == null || session.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                if (programme.Id == 1001 && level.Id != 1001 && department.Id != 1001)
                {
                    payments = GetPaymentsByLevelDepartmentSession(session, level, department, dateFrom, dateTo, payments);
                }
                else if (level.Id == 1001 && programme.Id != 1001 && department.Id != 1001)
                {
                    payments = GetPaymentsByProgrammeDepartmentSession(session, programme, department, dateFrom, dateTo, payments, level);
                }
                else if (department.Id == 1001 && level.Id != 1001 && programme.Id != 1001)
                {
                    payments = GetPaymentsByProgrammeLevelListession(session, level, programme, dateFrom, dateTo, payments);
                }
                else if (department.Id == 1001 && level.Id == 1001 && programme.Id != 1001)
                {
                    payments = GetPaymentsByProgrammeSession(session, programme, dateFrom, dateTo, payments, level);
                }
                else if (department.Id == 1001 && programme.Id == 1001 && level.Id != 1001)
                {
                    payments = GetPaymentsByLevelListession(session, level, dateFrom, dateTo, payments);
                }
                else if (level.Id == 1001 && programme.Id == 1001 && department.Id != 1001)
                {
                    payments = GetPaymentsByDepartmentSession(session, department, dateFrom, dateTo, payments, level);
                }
                else if (level.Id == 1001 && programme.Id == 1001 && department.Id == 1001)
                {
                    //payments = GetPaymentsBySession(session, dateFrom, dateTo, payments, level, paymentMode);
                }
                else
                {
                    if (paymentMode.Id == 100)
                    {
                        if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                        {
                            DateTime paymentFrom = ConvertToDate(dateFrom);
                            DateTime paymentTo = ConvertToDate(dateTo);

                            if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                            {
                                if (session.Id == (int)Sessions._20162017)
                                {
                                    payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id) && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                select new PaymentView
                                                {
                                                    PersonId = sr.Person_Id,
                                                    SessionId = sr.Session_Id,
                                                    SessionName = sr.Session_Name,
                                                    Name = sr.Name,
                                                    ImageUrl = sr.Image_File_Url,
                                                    MatricNumber = sr.Matric_Number,
                                                    ProgrammeId = sr.Programme_Id,
                                                    ProgrammeName = sr.Programme_Name,
                                                    DepartmentId = sr.Department_Id,
                                                    DepartmentName = sr.Department_Name,
                                                    PaymentModeId = sr.Payment_Mode_Id,
                                                    TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                    Amount = sr.Transaction_Amount,
                                                }).ToList();
                                }
                                else
                                {
                                    payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Admitted_Session_Id == null && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                select new PaymentView
                                                {
                                                    PersonId = sr.Person_Id,
                                                    SessionId = sr.Session_Id,
                                                    SessionName = sr.Session_Name,
                                                    Name = sr.Name,
                                                    ImageUrl = sr.Image_File_Url,
                                                    MatricNumber = sr.Matric_Number,
                                                    ProgrammeId = sr.Programme_Id,
                                                    ProgrammeName = sr.Programme_Name,
                                                    DepartmentId = sr.Department_Id,
                                                    DepartmentName = sr.Department_Name,
                                                    PaymentModeId = sr.Payment_Mode_Id,
                                                    TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                    Amount = sr.Transaction_Amount,
                                                }).ToList();
                                }
                            }
                            else
                            {
                                payments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_COMPREHENSIVE>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                            select new PaymentView
                                            {
                                                PersonId = sr.Person_Id,
                                                SessionId = sr.Admitted_Session_Id,
                                                SessionName = sr.Admitted_Session,
                                                Name = sr.Name,
                                                ImageUrl = sr.Image_File_Url,
                                                MatricNumber = sr.Application_Form_Number,
                                                ProgrammeId = sr.Admitted_Programme_Id,
                                                ProgrammeName = sr.Admitted_Programme,
                                                DepartmentId = sr.Admitted_Department_Id,
                                                DepartmentName = sr.Admitted_Department,
                                                PaymentModeId = sr.Payment_Mode_Id,
                                                TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                Amount = sr.Transaction_Amount,
                                            }).ToList();
                            }
                        }
                        else
                        {
                            if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                            {
                                if (session.Id == (int)Sessions._20162017)
                                {
                                    payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                                select new PaymentView
                                                {
                                                    PersonId = sr.Person_Id,
                                                    SessionId = sr.Session_Id,
                                                    SessionName = sr.Session_Name,
                                                    Name = sr.Name,
                                                    ImageUrl = sr.Image_File_Url,
                                                    MatricNumber = sr.Matric_Number,
                                                    ProgrammeId = sr.Programme_Id,
                                                    ProgrammeName = sr.Programme_Name,
                                                    DepartmentId = sr.Department_Id,
                                                    DepartmentName = sr.Department_Name,
                                                    PaymentModeId = sr.Payment_Mode_Id,
                                                    TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                    Amount = sr.Transaction_Amount,
                                                }).ToList();
                                }
                                else
                                {
                                    payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                                select new PaymentView
                                                {
                                                    PersonId = sr.Person_Id,
                                                    SessionId = sr.Session_Id,
                                                    SessionName = sr.Session_Name,
                                                    Name = sr.Name,
                                                    ImageUrl = sr.Image_File_Url,
                                                    MatricNumber = sr.Matric_Number,
                                                    ProgrammeId = sr.Programme_Id,
                                                    ProgrammeName = sr.Programme_Name,
                                                    DepartmentId = sr.Department_Id,
                                                    DepartmentName = sr.Department_Name,
                                                    PaymentModeId = sr.Payment_Mode_Id,
                                                    TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                    Amount = sr.Transaction_Amount,
                                                }).ToList();
                                }
                            }
                            else
                            {
                                payments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_COMPREHENSIVE>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id)
                                            select new PaymentView
                                            {
                                                PersonId = sr.Person_Id,
                                                SessionId = sr.Admitted_Session_Id,
                                                SessionName = sr.Admitted_Session,
                                                Name = sr.Name,
                                                ImageUrl = sr.Image_File_Url,
                                                MatricNumber = sr.Application_Form_Number,
                                                ProgrammeId = sr.Admitted_Programme_Id,
                                                ProgrammeName = sr.Admitted_Programme,
                                                DepartmentId = sr.Admitted_Department_Id,
                                                DepartmentName = sr.Admitted_Department,
                                                PaymentModeId = sr.Payment_Mode_Id,
                                                TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                Amount = sr.Transaction_Amount,
                                            }).ToList();
                            }
                        }
                    }
                    if (paymentMode.Id == 2)
                    {
                        if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                        {
                            DateTime paymentFrom = ConvertToDate(dateFrom);
                            DateTime paymentTo = ConvertToDate(dateTo);

                            if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                            {
                                if (session.Id == (int)Sessions._20162017)
                                {
                                    paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id) && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                   select new PaymentView
                                                   {
                                                       PersonId = sr.Person_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                       Amount = sr.Transaction_Amount,
                                                   }).ToList();
                                }
                                else
                                {
                                    paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Admitted_Session_Id == null && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                   select new PaymentView
                                                   {
                                                       PersonId = sr.Person_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                       Amount = sr.Transaction_Amount,
                                                   }).ToList();
                                }


                                List<long> distinctPerson = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                                for (int i = 0; i < distinctPerson.Count; i++)
                                {
                                    PaymentView firstInstallment = paymentsOld.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                                    PaymentView secondInstallment = paymentsOld.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                                    if (secondInstallment == null)
                                    {
                                        payments.Add(firstInstallment);
                                    }
                                }
                            }
                            else
                            {
                                //paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_COMPREHENSIVE>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))

                                               paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_COMPREHENSIVE>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Payment_Mode_Id == paymentMode.Id && x.Admitted_Session_Id == session.Id && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                           select new PaymentView
                                               {
                                                   PersonId = sr.Person_Id,
                                                   SessionId = sr.Admitted_Session_Id,
                                                   SessionName = sr.Admitted_Session,
                                                   Name = sr.Name,
                                                   ImageUrl = sr.Image_File_Url,
                                                   MatricNumber = sr.Application_Form_Number,
                                                   ProgrammeId = sr.Admitted_Programme_Id,
                                                   ProgrammeName = sr.Admitted_Programme,
                                                   DepartmentId = sr.Admitted_Department_Id,
                                                   DepartmentName = sr.Admitted_Department,
                                                   PaymentModeId = sr.Payment_Mode_Id,
                                                   TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                   Amount = sr.Transaction_Amount,
                                               }).ToList();

                                List<long> distinctPerson = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                                for (int i = 0; i < distinctPerson.Count; i++)
                                {
                                    PaymentView firstInstallment = paymentsNew.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                                    PaymentView secondInstallment = paymentsNew.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                                    if (secondInstallment == null)
                                    {
                                        payments.Add(firstInstallment);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                            {
                                if (session.Id == (int)Sessions._20162017)
                                {
                                    paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                                   select new PaymentView
                                                   {
                                                       PersonId = sr.Person_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                       Amount = sr.Transaction_Amount,
                                                   }).ToList();
                                }
                                else
                                {
                                    paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                                   select new PaymentView
                                                   {
                                                       PersonId = sr.Person_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                       Amount = sr.Transaction_Amount,
                                                   }).ToList();
                                }


                                List<long> distinctPerson = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                                for (int i = 0; i < distinctPerson.Count; i++)
                                {
                                    PaymentView firstInstallment = paymentsOld.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                                    PaymentView secondInstallment = paymentsOld.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                                    if (secondInstallment == null)
                                    {
                                        payments.Add(firstInstallment);
                                    }
                                }
                            }
                            else
                            {
                                paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FIRST>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id)
                                               select new PaymentView
                                               {
                                                   PersonId = sr.Person_Id,
                                                   SessionId = sr.Admitted_Session_Id,
                                                   SessionName = sr.Admitted_Session,
                                                   Name = sr.Name,
                                                   ImageUrl = sr.Image_File_Url,
                                                   MatricNumber = sr.Application_Form_Number,
                                                   ProgrammeId = sr.Admitted_Programme_Id,
                                                   ProgrammeName = sr.Admitted_Programme,
                                                   DepartmentId = sr.Admitted_Department_Id,
                                                   DepartmentName = sr.Admitted_Department,
                                                   PaymentModeId = sr.Payment_Mode_Id,
                                                   TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                   Amount = sr.Transaction_Amount,
                                               }).ToList();

                                List<long> distinctPerson = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                                for (int i = 0; i < distinctPerson.Count; i++)
                                {
                                    PaymentView firstInstallment = paymentsNew.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                                    PaymentView secondInstallment = paymentsNew.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                                    if (secondInstallment == null)
                                    {
                                        payments.Add(firstInstallment);
                                    }
                                }
                            }
                        }
                    }
                    if (paymentMode.Id == 3)
                    {
                        if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                        {
                            DateTime paymentFrom = ConvertToDate(dateFrom);
                            DateTime paymentTo = ConvertToDate(dateTo);

                            if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                            {
                                if (session.Id == (int)Sessions._20162017)
                                {
                                    paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_COMPREHENSIVE>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Payment_Mode_Id == paymentMode.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id) && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                   select new PaymentView
                                                   {
                                                       PersonId = sr.Person_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                       Amount = sr.Transaction_Amount,
                                                   }).ToList();
                                }
                                else
                                {
                                    paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Admitted_Session_Id == null && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                   select new PaymentView
                                                   {
                                                       PersonId = sr.Person_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                       Amount = sr.Transaction_Amount,
                                                   }).ToList();
                                }


                                List<long> distinctPerson = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                                for (int i = 0; i < distinctPerson.Count; i++)
                                {
                                    PaymentView firstInstallment = paymentsOld.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                                    PaymentView secondInstallment = paymentsOld.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                                    if (secondInstallment != null)
                                    {
                                        payments.Add(firstInstallment);
                                        payments.Add(secondInstallment);
                                    }
                                }
                            }
                            else
                            {
                                paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FIRST>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                               select new PaymentView
                                               {
                                                   PersonId = sr.Person_Id,
                                                   SessionId = sr.Admitted_Session_Id,
                                                   SessionName = sr.Admitted_Session,
                                                   Name = sr.Name,
                                                   ImageUrl = sr.Image_File_Url,
                                                   MatricNumber = sr.Application_Form_Number,
                                                   ProgrammeId = sr.Admitted_Programme_Id,
                                                   ProgrammeName = sr.Admitted_Programme,
                                                   DepartmentId = sr.Admitted_Department_Id,
                                                   DepartmentName = sr.Admitted_Department,
                                                   PaymentModeId = sr.Payment_Mode_Id,
                                                   TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                   Amount = sr.Transaction_Amount,
                                               }).ToList();

                                List<long> distinctPerson = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                                for (int i = 0; i < distinctPerson.Count; i++)
                                {
                                    PaymentView firstInstallment = paymentsNew.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                                    PaymentView secondInstallment = paymentsNew.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                                    if (secondInstallment != null)
                                    {
                                        payments.Add(firstInstallment);
                                        payments.Add(secondInstallment);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                            {
                                if (session.Id == (int)Sessions._20162017)
                                {
                                    paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                                   select new PaymentView
                                                   {
                                                       PersonId = sr.Person_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                       Amount = sr.Transaction_Amount,
                                                   }).ToList();
                                }
                                else
                                {
                                    paymentsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                                   select new PaymentView
                                                   {
                                                       PersonId = sr.Person_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                       Amount = sr.Transaction_Amount,
                                                   }).ToList();
                                }


                                List<long> distinctPerson = paymentsOld.Select(p => p.PersonId).Distinct().ToList();

                                for (int i = 0; i < distinctPerson.Count; i++)
                                {
                                    PaymentView firstInstallment = paymentsOld.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                                    PaymentView secondInstallment = paymentsOld.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                                    if (secondInstallment != null)
                                    {
                                        payments.Add(firstInstallment);
                                        payments.Add(secondInstallment);
                                    }
                                }
                            }
                            else
                            {
                                paymentsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FIRST>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id)
                                               select new PaymentView
                                               {
                                                   PersonId = sr.Person_Id,
                                                   SessionId = sr.Admitted_Session_Id,
                                                   SessionName = sr.Admitted_Session,
                                                   Name = sr.Name,
                                                   ImageUrl = sr.Image_File_Url,
                                                   MatricNumber = sr.Application_Form_Number,
                                                   ProgrammeId = sr.Admitted_Programme_Id,
                                                   ProgrammeName = sr.Admitted_Programme,
                                                   DepartmentId = sr.Admitted_Department_Id,
                                                   DepartmentName = sr.Admitted_Department,
                                                   PaymentModeId = sr.Payment_Mode_Id,
                                                   TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                   Amount = sr.Transaction_Amount,
                                               }).ToList();

                                List<long> distinctPerson = paymentsNew.Select(p => p.PersonId).Distinct().ToList();

                                for (int i = 0; i < distinctPerson.Count; i++)
                                {
                                    PaymentView firstInstallment = paymentsNew.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.FirstInstallment).LastOrDefault();
                                    PaymentView secondInstallment = paymentsNew.Where(p => p.PersonId == distinctPerson[i] && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();
                                    if (secondInstallment != null)
                                    {
                                        payments.Add(firstInstallment);
                                        payments.Add(secondInstallment);
                                    }
                                }
                            }
                        }
                    }
                    if (paymentMode.Id == 1)
                    {
                        if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                        {
                            DateTime paymentFrom = ConvertToDate(dateFrom);
                            DateTime paymentTo = ConvertToDate(dateTo);

                            if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                            {
                                if (session.Id == (int)Sessions._20162017)
                                {
                                    Masterpayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FULL>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id) && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                      select new PaymentView
                                                      {
                                                          PersonId = sr.Person_Id,
                                                          SessionId = sr.Session_Id,
                                                          SessionName = sr.Session_Name,
                                                          Name = sr.Name,
                                                          ImageUrl = sr.Image_File_Url,
                                                          MatricNumber = sr.Matric_Number,
                                                          ProgrammeId = sr.Programme_Id,
                                                          ProgrammeName = sr.Programme_Name,
                                                          DepartmentId = sr.Department_Id,
                                                          DepartmentName = sr.Department_Name,
                                                          PaymentModeId = sr.Payment_Mode_Id,
                                                          TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                          Amount = sr.Transaction_Amount,
                                                          FullPaymentAmount = sr.Transaction_Amount,
                                                          FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString()
                                                      }).ToList();
                                }
                                else
                                {
                                    Masterpayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FULL>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Admitted_Session_Id == null && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                      select new PaymentView
                                                      {
                                                          PersonId = sr.Person_Id,
                                                          SessionId = sr.Session_Id,
                                                          SessionName = sr.Session_Name,
                                                          Name = sr.Name,
                                                          ImageUrl = sr.Image_File_Url,
                                                          MatricNumber = sr.Matric_Number,
                                                          ProgrammeId = sr.Programme_Id,
                                                          ProgrammeName = sr.Programme_Name,
                                                          DepartmentId = sr.Department_Id,
                                                          DepartmentName = sr.Department_Name,
                                                          PaymentModeId = sr.Payment_Mode_Id,
                                                          TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                          Amount = sr.Transaction_Amount,
                                                          FullPaymentAmount = sr.Transaction_Amount,
                                                          FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString()
                                                      }).ToList();
                                }

                            }
                            else
                            {
                                Masterpayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FULL>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id && (x.Transaction_Date >= paymentFrom && x.Transaction_Date <= paymentTo))
                                                  select new PaymentView
                                                  {
                                                      PersonId = sr.Person_Id,
                                                      SessionId = sr.Admitted_Session_Id,
                                                      SessionName = sr.Admitted_Session,
                                                      Name = sr.Name,
                                                      ImageUrl = sr.Image_File_Url,
                                                      MatricNumber = sr.Application_Form_Number,
                                                      ProgrammeId = sr.Admitted_Programme_Id,
                                                      ProgrammeName = sr.Admitted_Programme,
                                                      DepartmentId = sr.Admitted_Department_Id,
                                                      DepartmentName = sr.Admitted_Department,
                                                      PaymentModeId = sr.Payment_Mode_Id,
                                                      TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                      Amount = sr.Transaction_Amount,
                                                      FullPaymentAmount = sr.Transaction_Amount,
                                                      FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString()
                                                  }).ToList();
                            }
                        }
                        else
                        {
                            if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                            {
                                if (session.Id == (int)Sessions._20162017)
                                {
                                    payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FULL>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                                select new PaymentView
                                                {
                                                    PersonId = sr.Person_Id,
                                                    SessionId = sr.Session_Id,
                                                    SessionName = sr.Session_Name,
                                                    Name = sr.Name,
                                                    ImageUrl = sr.Image_File_Url,
                                                    MatricNumber = sr.Matric_Number,
                                                    ProgrammeId = sr.Programme_Id,
                                                    ProgrammeName = sr.Programme_Name,
                                                    DepartmentId = sr.Department_Id,
                                                    DepartmentName = sr.Department_Name,
                                                    PaymentModeId = sr.Payment_Mode_Id,
                                                    TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                    Amount = sr.Transaction_Amount,
                                                    FullPaymentAmount = sr.Transaction_Amount,
                                                    FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString()
                                                }).ToList();
                                }
                                else
                                {
                                    payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FULL>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                                select new PaymentView
                                                {
                                                    PersonId = sr.Person_Id,
                                                    SessionId = sr.Session_Id,
                                                    SessionName = sr.Session_Name,
                                                    Name = sr.Name,
                                                    ImageUrl = sr.Image_File_Url,
                                                    MatricNumber = sr.Matric_Number,
                                                    ProgrammeId = sr.Programme_Id,
                                                    ProgrammeName = sr.Programme_Name,
                                                    DepartmentId = sr.Department_Id,
                                                    DepartmentName = sr.Department_Name,
                                                    PaymentModeId = sr.Payment_Mode_Id,
                                                    TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                    Amount = sr.Transaction_Amount,
                                                    FullPaymentAmount = sr.Transaction_Amount,
                                                    FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString()
                                                }).ToList();
                                }

                            }
                            else
                            {
                                payments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FULL>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id)
                                            select new PaymentView
                                            {
                                                PersonId = sr.Person_Id,
                                                SessionId = sr.Admitted_Session_Id,
                                                SessionName = sr.Admitted_Session,
                                                Name = sr.Name,
                                                ImageUrl = sr.Image_File_Url,
                                                MatricNumber = sr.Application_Form_Number,
                                                ProgrammeId = sr.Admitted_Programme_Id,
                                                ProgrammeName = sr.Admitted_Programme,
                                                DepartmentId = sr.Admitted_Department_Id,
                                                DepartmentName = sr.Admitted_Department,
                                                PaymentModeId = sr.Payment_Mode_Id,
                                                TransactionDate = sr.Transaction_Date.Value.ToLongDateString(),
                                                Amount = sr.Transaction_Amount,
                                                FullPaymentAmount = sr.Transaction_Amount,
                                                FullPaymentDate = sr.Transaction_Date.Value.ToLongDateString()
                                            }).ToList();
                            }
                        }

                    }

                    SessionLogic sessionLogic = new SessionLogic();
                    Session currentSession = sessionLogic.GetModelsBy(s => s.Session_Id == session.Id).LastOrDefault();

                    int lstSessionYear = Convert.ToInt32(currentSession.Name.Split('/').FirstOrDefault());
                    string lstSessionName = (lstSessionYear - 1) + "/" + lstSessionYear;

                    Session lastSession = sessionLogic.GetModelBy(s => s.Session_Name.Trim() == lstSessionName.Trim());

                    int[] otherYears = { 3, 4, 5, 6, 7 };

                    if (paymentMode.Id == 101)
                    {
                        if (level.Id == (int)LevelList.ND2 || level.Id == (int)LevelList.HND2)
                        {
                            //session.Id == (int)Sessions._20162017
                            if (otherYears.Contains(lastSession.Id))
                            {
                                Masterpayments = (from sr in repository.GetBy<VW_DEBTORS_OLD_STUDENT>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id && (x.Admitted_Session_Id == lastSession.Id || x.Admitted_Session_Id == null))
                                                  select new PaymentView
                                                  {
                                                      SessionId = sr.Session_Id,
                                                      SessionName = sr.Session_Name,
                                                      PersonId = sr.Person_Id,
                                                      Name = sr.Name,
                                                      ImageUrl = sr.Image_File_Url,
                                                      ProgrammeId = sr.Programme_Id,
                                                      ProgrammeName = sr.Programme_Name,
                                                      DepartmentId = sr.Department_Id,
                                                      DepartmentName = sr.Department_Name,
                                                      MatricNumber = sr.Matric_Number

                                                  }).ToList();
                            }
                            else
                            {
                                Masterpayments = (from sr in repository.GetBy<VW_DEBTORS_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == lastSession.Id)
                                                  select new PaymentView
                                                  {
                                                      SessionId = sr.Admitted_Session_Id,
                                                      SessionName = sr.Admitted_Session,
                                                      PersonId = sr.Person_Id,
                                                      Name = sr.Name,
                                                      ImageUrl = sr.Image_File_Url,
                                                      ProgrammeId = sr.Admitted_Programme_Id,
                                                      ProgrammeName = sr.Admitted_Programme,
                                                      DepartmentId = sr.Admitted_Department_Id,
                                                      DepartmentName = sr.Admitted_Department,
                                                      MatricNumber = sr.Application_Form_Number

                                                  }).ToList();
                            }

                        }
                        else
                        {
                            Masterpayments = (from sr in repository.GetBy<VW_STUDENTS_WITH_NO_SCHOOL_FEES>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id)
                                              select new PaymentView
                                              {
                                                  SessionId = sr.Session_Id,
                                                  SessionName = sr.Session_Name,
                                                  PersonId = sr.Person_Id,
                                                  Name = sr.Full_Name,
                                                  ImageUrl = sr.Image_File_Url,
                                                  MobilePhone = sr.Mobile_Phone,
                                                  ProgrammeId = sr.Programme_Id,
                                                  ProgrammeName = sr.Programme_Name,
                                                  DepartmentId = sr.Department_Id,
                                                  DepartmentName = sr.Department_Name,
                                                  MatricNumber = sr.Application_Form_Number

                                              }).ToList();
                        }
                    }
                }

                if (paymentMode.Id == 100 || paymentMode.Id == 2 || paymentMode.Id == 3 || paymentMode.Id == 1)
                {
                    List<long> Ids = payments.Select(p => p.PersonId).Distinct().ToList();
                    for (int i = 0; i < Ids.Count; i++)
                    {
                        long currentPersonId = Ids[i];
                        List<PaymentView> currentPayments = payments.Where(p => p.PersonId == currentPersonId).ToList();
                        for (int j = 0; j < currentPayments.Count; j++)
                        {
                            if (currentPayments[j].PaymentModeId == 3 || currentPayments[j].PaymentModeId == 6)
                            {
                                currentPayments[0].SecondInstallmentAmount = currentPayments[j].Amount;
                                currentPayments[0].SecondInstallmentDate = currentPayments[j].TransactionDate;

                            }
                            if (currentPayments[j].PaymentModeId == 2 || currentPayments[j].PaymentModeId == 4)
                            {
                                currentPayments[0].FirstInstallmentAmount = currentPayments[j].Amount;
                                currentPayments[0].FirstInstallmentDate = currentPayments[j].TransactionDate;
                            }
                            if (currentPayments[j].PaymentModeId == 1)
                            {
                                currentPayments[0].FullPaymentAmount = currentPayments[j].Amount;
                                currentPayments[0].FullPaymentDate = currentPayments[j].TransactionDate;
                            }
                        }

                        Masterpayments.Add(currentPayments[0]);
                    }
                }

                return Masterpayments;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<PaymentView> GetPaymentsByProgrammeLevelDepartmentSession(Session session, Level level, Programme programme,
            Department department, string dateFrom, string dateTo, List<PaymentView> payments)
        {
            List<StudentDetailFormat> students = new List<StudentDetailFormat>();
            List<StudentDetailFormat> newStudents = new List<StudentDetailFormat>();
            List<StudentDetailFormat> oldStudents = new List<StudentDetailFormat>();

            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

            if (level.Id == 1001)
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    newStudents = (from sr in repository.GetAll<VW_NEW_STUDENT>()
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => !x.Matric_Number.Contains("2016"))
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
                else
                {
                    newStudents = (from sr in repository.GetAll<VW_NEW_STUDENT>()
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => !x.Matric_Number.Contains("2016"))
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {

                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>()
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    //payments = GetPaymentsByProgrammeLevelDepartmentSession(session, level, programme, department, dateFrom, dateTo, payments);
                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {
                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>()
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
            }


            return payments;
        }

        private List<PaymentView> GetPaymentsByDepartmentSession(Session session, Department department, string dateFrom, string dateTo, List<PaymentView> payments, Level level)
        {
            List<StudentDetailFormat> students = new List<StudentDetailFormat>();
            List<StudentDetailFormat> newStudents = new List<StudentDetailFormat>();
            List<StudentDetailFormat> oldStudents = new List<StudentDetailFormat>();

            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

            if (level.Id == 1001)
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
                else
                {
                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {

                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    //payments = GetPaymentsByProgrammeLevelDepartmentSession(session, level, programme, department, dateFrom, dateTo, payments);
                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {
                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
            }


            return payments;
        }

        private List<PaymentView> GetPaymentsByLevelListession(Session session, Level level, string dateFrom, string dateTo, List<PaymentView> payments)
        {
            List<StudentDetailFormat> students = new List<StudentDetailFormat>();
            List<StudentDetailFormat> newStudents = new List<StudentDetailFormat>();
            List<StudentDetailFormat> oldStudents = new List<StudentDetailFormat>();

            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                DateTime paymentFrom = ConvertToDate(dateFrom);
                DateTime paymentTo = ConvertToDate(dateTo);


                if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                {

                    students = (from sr in repository.GetAll<VW_NEW_STUDENT>()
                                select new StudentDetailFormat
                                {
                                    PersonId = sr.Person_Id,
                                    ApplicationNumber = sr.Application_Form_Number,
                                    SessionId = sr.Admitted_Session_Id,
                                    Session = sr.Admitted_Session,
                                    Name = sr.Name,
                                    ImageUrl = sr.Image_File_Url,
                                    MatricNumber = sr.Application_Form_Number,
                                    ProgrammeId = sr.Admitted_Programme_Id,
                                    Programme = sr.Admitted_Programme,
                                    DepartmentId = sr.Admitted_Department_Id,
                                    Department = sr.Admitted_Department
                                }).ToList();

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
                else
                {
                    students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => !x.Matric_Number.Contains("2016"))
                                select new StudentDetailFormat
                                {
                                    PersonId = sr.Person_Id,
                                    ApplicationNumber = sr.Application_Form_Number,
                                    SessionId = sr.Session_Id,
                                    Session = sr.Session_Name,
                                    Name = sr.Name,
                                    ImageUrl = sr.Image_File_Url,
                                    MatricNumber = sr.Matric_Number,
                                    ProgrammeId = sr.Programme_Id,
                                    Programme = sr.Programme_Name,
                                    DepartmentId = sr.Department_Id,
                                    Department = sr.Department_Name
                                }).ToList();

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
            }
            else
            {
                //payments = GetPaymentsByProgrammeLevelDepartmentSession(session, level, programme, department, dateFrom, dateTo, payments);
                if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                {
                    students = (from sr in repository.GetAll<VW_NEW_STUDENT>()
                                select new StudentDetailFormat
                                {
                                    PersonId = sr.Person_Id,
                                    ApplicationNumber = sr.Application_Form_Number,
                                    SessionId = sr.Admitted_Session_Id,
                                    Session = sr.Admitted_Session,
                                    Name = sr.Name,
                                    ImageUrl = sr.Image_File_Url,
                                    MatricNumber = sr.Application_Form_Number,
                                    ProgrammeId = sr.Admitted_Programme_Id,
                                    Programme = sr.Admitted_Programme,
                                    DepartmentId = sr.Admitted_Department_Id,
                                    Department = sr.Admitted_Department
                                }).ToList();

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
                else
                {
                    students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => !x.Matric_Number.Contains("2016"))
                                select new StudentDetailFormat
                                {
                                    PersonId = sr.Person_Id,
                                    ApplicationNumber = sr.Application_Form_Number,
                                    SessionId = sr.Session_Id,
                                    Session = sr.Session_Name,
                                    Name = sr.Name,
                                    ImageUrl = sr.Image_File_Url,
                                    MatricNumber = sr.Matric_Number,
                                    ProgrammeId = sr.Programme_Id,
                                    Programme = sr.Programme_Name,
                                    DepartmentId = sr.Department_Id,
                                    Department = sr.Department_Name
                                }).ToList();

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
            }


            return payments;
        }

        private List<PaymentView> GetPaymentsByProgrammeSession(Session session, Programme programme, string dateFrom, string dateTo, List<PaymentView> payments, Level level)
        {
            List<StudentDetailFormat> students = new List<StudentDetailFormat>();
            List<StudentDetailFormat> newStudents = new List<StudentDetailFormat>();
            List<StudentDetailFormat> oldStudents = new List<StudentDetailFormat>();

            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

            if (level.Id == 1001)
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);

                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
                else
                {
                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {

                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Programme_Id == programme.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    //payments = GetPaymentsByProgrammeLevelDepartmentSession(session, level, programme, department, dateFrom, dateTo, payments);
                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {
                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Programme_Id == programme.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
            }


            return payments;
        }

        private List<PaymentView> GetPaymentsByProgrammeLevelListession(Session session, Level level, Programme programme, string dateFrom, string dateTo, List<PaymentView> payments)
        {
            List<StudentDetailFormat> students = new List<StudentDetailFormat>();
            List<StudentDetailFormat> newStudents = new List<StudentDetailFormat>();
            List<StudentDetailFormat> oldStudents = new List<StudentDetailFormat>();

            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

            if (level.Id == 1001)
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Programme_Id == programme.Id && !x.Matric_Number.Contains("2016"))
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
                else
                {
                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Programme_Id == programme.Id && !x.Matric_Number.Contains("2016"))
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {

                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Programme_Id == programme.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    //payments = GetPaymentsByProgrammeLevelDepartmentSession(session, level, programme, department, dateFrom, dateTo, payments);
                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {
                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Programme_Id == programme.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
            }


            return payments;
        }

        private List<PaymentView> GetPaymentsByProgrammeDepartmentSession(Session session, Programme programme, Department department, string dateFrom, string dateTo, List<PaymentView> payments, Level level)
        {
            List<StudentDetailFormat> students = new List<StudentDetailFormat>();
            List<StudentDetailFormat> newStudents = new List<StudentDetailFormat>();
            List<StudentDetailFormat> oldStudents = new List<StudentDetailFormat>();

            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

            if (level.Id == 1001)
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id && x.Admitted_Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id && x.Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
                else
                {
                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id && x.Admitted_Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id && x.Programme_Id == programme.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {

                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id && x.Admitted_Programme_Id == programme.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    //payments = GetPaymentsByProgrammeLevelDepartmentSession(session, level, programme, department, dateFrom, dateTo, payments);
                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {
                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id && x.Admitted_Programme_Id == programme.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
            }


            return payments;
        }

        private List<PaymentView> GetPaymentsByLevelDepartmentSession(Session session, Level level, Department department, string dateFrom, string dateTo, List<PaymentView> payments)
        {
            List<StudentDetailFormat> students = new List<StudentDetailFormat>();
            List<StudentDetailFormat> newStudents = new List<StudentDetailFormat>();
            List<StudentDetailFormat> oldStudents = new List<StudentDetailFormat>();

            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

            if (level.Id == 1001)
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
                else
                {
                    newStudents = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Admitted_Session_Id,
                                       Session = sr.Admitted_Session,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Application_Form_Number,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       Programme = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       Department = sr.Admitted_Department
                                   }).ToList();

                    oldStudents = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id)
                                   select new StudentDetailFormat
                                   {
                                       PersonId = sr.Person_Id,
                                       ApplicationNumber = sr.Application_Form_Number,
                                       SessionId = sr.Session_Id,
                                       Session = sr.Session_Name,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       MatricNumber = sr.Matric_Number,
                                       ProgrammeId = sr.Programme_Id,
                                       Programme = sr.Programme_Name,
                                       DepartmentId = sr.Department_Id,
                                       Department = sr.Department_Name
                                   }).ToList();

                    students.AddRange(newStudents);
                    students.AddRange(oldStudents);

                    List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                    for (int i = 0; i < distinctPersonId.Count; i++)
                    {
                        long personId = distinctPersonId[i];
                        StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                        List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                        if (studentPaymentEtranzacts.Count > 0)
                        {
                            for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                            {
                                payments.Add(new PaymentView()
                                {
                                    PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                    InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                    PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                    PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                    PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                    PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                    FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                    FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                    SessionId = session.Id,
                                    SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                    PersonId = personId,
                                    Name = format.Name,
                                    ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                    MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                    MatricNumber = format.MatricNumber,
                                    LevelId = level.Id,
                                    LevelName = level.Name,
                                    ProgrammeId = format.ProgrammeId,
                                    ProgrammeName = format.Programme,
                                    DepartmentId = format.DepartmentId,
                                    DepartmentName = format.Department,
                                    BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                    ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                    ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                    Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                });
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime paymentFrom = ConvertToDate(dateFrom);
                    DateTime paymentTo = ConvertToDate(dateTo);


                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {

                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3 && (p.Transaction_Date >= paymentFrom && p.Transaction_Date <= paymentTo));
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    //payments = GetPaymentsByProgrammeLevelDepartmentSession(session, level, programme, department, dateFrom, dateTo, payments);
                    if (level.Id == (int)LevelList.ND1 || level.Id == (int)LevelList.HND1)
                    {
                        students = (from sr in repository.GetBy<VW_NEW_STUDENT>(x => x.Admitted_Department_Id == department.Id)
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Admitted_Session_Id,
                                        Session = sr.Admitted_Session,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Application_Form_Number,
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        Programme = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        Department = sr.Admitted_Department
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        students = (from sr in repository.GetBy<VW_OLD_STUDENT>(x => x.Department_Id == department.Id && !x.Matric_Number.Contains("2016"))
                                    select new StudentDetailFormat
                                    {
                                        PersonId = sr.Person_Id,
                                        ApplicationNumber = sr.Application_Form_Number,
                                        SessionId = sr.Session_Id,
                                        Session = sr.Session_Name,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        ProgrammeId = sr.Programme_Id,
                                        Programme = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        Department = sr.Department_Name
                                    }).ToList();

                        List<long> distinctPersonId = students.Select(s => s.PersonId).Distinct().ToList();
                        for (int i = 0; i < distinctPersonId.Count; i++)
                        {
                            long personId = distinctPersonId[i];
                            StudentDetailFormat format = students.Where(s => s.PersonId == personId).LastOrDefault();

                            List<PAYMENT_ETRANZACT> studentPaymentEtranzacts = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == personId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 3);
                            if (studentPaymentEtranzacts.Count > 0)
                            {
                                for (int j = 0; j < studentPaymentEtranzacts.Count; j++)
                                {
                                    payments.Add(new PaymentView()
                                    {
                                        PaymentId = studentPaymentEtranzacts[j].Payment_Id,
                                        InvoiceNumber = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Invoice_Number,
                                        PaymentTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Type_Id,
                                        PaymentTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_TYPE.Payment_Type_Name,
                                        PaymentModeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id,
                                        PaymentModeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PAYMENT_MODE.Payment_Mode_Name,
                                        FeeTypeId = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.Fee_Type_Id,
                                        FeeTypeName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.FEE_TYPE.Fee_Type_Name,
                                        SessionId = session.Id,
                                        SessionName = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.SESSION.Session_Name,
                                        PersonId = personId,
                                        Name = format.Name,
                                        ImageUrl = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Image_File_Url,
                                        MobilePhone = studentPaymentEtranzacts[j].ONLINE_PAYMENT.PAYMENT.PERSON.Mobile_Phone,
                                        MatricNumber = format.MatricNumber,
                                        LevelId = level.Id,
                                        LevelName = level.Name,
                                        ProgrammeId = format.ProgrammeId,
                                        ProgrammeName = format.Programme,
                                        DepartmentId = format.DepartmentId,
                                        DepartmentName = format.Department,
                                        BankCode = studentPaymentEtranzacts[j].Bank_Code,
                                        ConfirmationOrderNumber = studentPaymentEtranzacts[j].Confirmation_No,
                                        ReceiptNumber = studentPaymentEtranzacts[j].Receipt_No,
                                        Amount = studentPaymentEtranzacts[j].Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(studentPaymentEtranzacts[j].Transaction_Date).ToShortDateString()

                                    });
                                }
                            }
                        }
                    }
                }
            }


            return payments;
        }

        public List<PaymentView> GetDebtorsList(Session session, Level level, Programme programme, Department department)
        {
            List<PaymentView> masterpayments = new List<PaymentView>();
            List<PaymentView> payments = new List<PaymentView>();
            SessionLogic sessionLogic = new SessionLogic();
            LevelLogic levelLogic = new LevelLogic();

            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

            try
            {
                if (session == null || session.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                level = levelLogic.GetModelBy(l => l.Level_Id == level.Id);


                if (level.Id == (int)Levels.NDII || level.Id == (int)Levels.HNDII)
                {
                    Session fullSession = sessionLogic.GetModelBy(s => s.Session_Id == session.Id);
                    int sessionStart = Convert.ToInt32(fullSession.Name.Split('/').FirstOrDefault());
                    string prevSessionName = sessionStart - 1 + "/" + sessionStart;

                    Session prevSession = sessionLogic.GetModelBy(s => s.Session_Name == prevSessionName);

                    Level prevLevel = new Level() { Id = (int)Levels.NDI };
                    if (level.Id == (int)Levels.HNDII)
                    {
                        prevLevel = new Level() { Id = (int)Levels.HNDI };
                    }

                    //payments = (from sr in repository.GetBy<VW_ADMISSION_LIST>(x => x.Session_Id == prevSession.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                    //                  select new PaymentView
                    //                  {
                    //                      SessionId = Convert.ToInt32(sr.Session_Id),
                    //                      SessionName = sr.Session_Name,
                    //                      PersonId = sr.Person_Id,
                    //                      Name = sr.FullName,
                    //                      ProgrammeId = sr.Programme_Id,
                    //                      ProgrammeName = sr.Programme_Name,
                    //                      DepartmentId = sr.Department_Id,
                    //                      DepartmentName = sr.Department_Name,
                    //                      MatricNumber = sr.Application_Form_Number,
                    //                      LevelName = level.Name

                    //                  }).ToList();

                    List<STUDENT_COURSE_REGISTRATION> courseRegistrations = courseRegistrationLogic.GetEntitiesBy(x => x.Session_Id == prevSession.Id && x.Programme_Id == programme.Id &&
                                                                                                                        x.Department_Id == department.Id && x.Level_Id == prevLevel.Id);
                    List<long> distinctPersons = courseRegistrations.Select(s => s.Person_Id).Distinct().ToList();

                    for (int i = 0; i < distinctPersons.Count; i++)
                    {
                        long currentPersonId = distinctPersons[i];
                        STUDENT_COURSE_REGISTRATION courseRegistration = courseRegistrations.LastOrDefault(c => c.Person_Id == currentPersonId);

                        PaymentView paymentView = new PaymentView();
                        paymentView.PersonId = courseRegistration.Person_Id;
                        paymentView.SessionId = fullSession.Id;
                        paymentView.SessionName = fullSession.Name;
                        paymentView.Name = courseRegistration.STUDENT.PERSON.Last_Name + " " + courseRegistration.STUDENT.PERSON.First_Name + " " + courseRegistration.STUDENT.PERSON.Other_Name;
                        paymentView.ProgrammeId = courseRegistration.Programme_Id;
                        paymentView.ProgrammeName = courseRegistration.PROGRAMME.Programme_Name;
                        paymentView.DepartmentId = courseRegistration.Department_Id;
                        paymentView.DepartmentName = courseRegistration.DEPARTMENT.Department_Name;
                        paymentView.MatricNumber = courseRegistration.STUDENT.Matric_Number;
                        paymentView.LevelName = courseRegistration.LEVEL.Level_Name;

                        payments.Add(paymentView);
                    }
                }
                else
                {
                    payments = (from sr in repository.GetBy<VW_ADMISSION_LIST>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                                select new PaymentView
                                {
                                    SessionId = Convert.ToInt32(sr.Session_Id),
                                    SessionName = sr.Session_Name,
                                    PersonId = sr.Person_Id,
                                    Name = sr.FullName,
                                    ProgrammeId = sr.Programme_Id,
                                    ProgrammeName = sr.Programme_Name,
                                    DepartmentId = sr.Department_Id,
                                    DepartmentName = sr.Department_Name,
                                    MatricNumber = sr.Application_Form_Number,
                                    LevelName = level.Name

                                }).ToList();
                }

                for (int i = 0; i < payments.Count; i++)
                {
                    PaymentView currentPayment = payments[i];

                    if (level.Id == (int)Levels.NDI || level.Id == (int)Levels.HNDI)
                    {
                        PAYMENT_ETRANZACT acceptancePayment = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == currentPayment.PersonId &&
                                p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.AcceptanceFee && p.ONLINE_PAYMENT.PAYMENT.Session_Id == currentPayment.SessionId).LastOrDefault();
                        if (acceptancePayment == null)
                        {
                            REMITA_PAYMENT acceptanceRemitaPayment = remitaPaymentLogic.GetEntitiesBy(r => r.PAYMENT.Person_Id == currentPayment.PersonId &&
                                        r.PAYMENT.Fee_Type_Id == (int)FeeTypes.AcceptanceFee && r.PAYMENT.Session_Id == currentPayment.SessionId && (r.Status.Contains("01:") || r.Status.Contains("00"))).LastOrDefault();
                            if (acceptanceRemitaPayment == null)
                            {
                                continue;
                            }
                        }
                    }

                    PAYMENT_ETRANZACT schoolFeePayment = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == currentPayment.PersonId &&
                                p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.ONLINE_PAYMENT.PAYMENT.Session_Id == currentPayment.SessionId).LastOrDefault();
                    if (schoolFeePayment == null)
                    {
                        REMITA_PAYMENT schoolFeeRemitaPayment = remitaPaymentLogic.GetEntitiesBy(r => r.PAYMENT.Person_Id == currentPayment.PersonId &&
                                    r.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees && r.PAYMENT.Session_Id == currentPayment.SessionId && (r.Status.Contains("01:") || r.Status.Contains("00")) ).LastOrDefault();
                        if (schoolFeeRemitaPayment == null)
                        {
                            masterpayments.Add(currentPayment);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }

            return masterpayments.OrderBy(o => o.Name).ToList();
        }

        public List<PaymentView> GetDebtorsListFull(Session session, Level level)
        {
            List<PaymentView> Masterpayments = new List<PaymentView>();
            SessionLogic sessionLogic = new SessionLogic();
            LevelLogic levelLogic = new LevelLogic();

            try
            {
                if (session == null || session.Id <= 0 || level == null || level.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                level = levelLogic.GetModelBy(l => l.Level_Id == level.Id);

                DateTime paymentDateTo = new DateTime();

                if (level.Id == (int)Levels.NDII || level.Id == (int)Levels.HNDII)
                {
                    Session fullSession = sessionLogic.GetModelBy(s => s.Session_Id == session.Id);
                    int sessionStart = Convert.ToInt32(fullSession.Name.Split('/').FirstOrDefault());
                    string prevSessionName = sessionStart - 1 + "/" + sessionStart;

                    Session prevSession = sessionLogic.GetModelBy(s => s.Session_Name == prevSessionName);

                    Masterpayments = (from sr in repository.GetBy<VW_STUDENTS_WITH_NO_SCHOOL_FEES>(x => x.Session_Id == prevSession.Id)
                                      select new PaymentView
                                      {
                                          SessionId = sr.Session_Id,
                                          SessionName = sr.Session_Name,
                                          PersonId = sr.Person_Id,
                                          Name = sr.Full_Name,
                                          ImageUrl = sr.Image_File_Url,
                                          MobilePhone = sr.Mobile_Phone,
                                          ProgrammeId = sr.Programme_Id,
                                          ProgrammeName = sr.Programme_Name,
                                          DepartmentId = sr.Department_Id,
                                          DepartmentName = sr.Department_Name,
                                          MatricNumber = sr.Application_Form_Number,
                                          LevelName = level.Name

                                      }).ToList();
                }
                else
                {
                    Masterpayments = (from sr in repository.GetBy<VW_STUDENTS_WITH_NO_SCHOOL_FEES>(x => x.Session_Id == session.Id)
                                      select new PaymentView
                                      {
                                          SessionId = sr.Session_Id,
                                          SessionName = sr.Session_Name,
                                          PersonId = sr.Person_Id,
                                          Name = sr.Full_Name,
                                          ImageUrl = sr.Image_File_Url,
                                          MobilePhone = sr.Mobile_Phone,
                                          ProgrammeId = sr.Programme_Id,
                                          ProgrammeName = sr.Programme_Name,
                                          DepartmentId = sr.Department_Id,
                                          DepartmentName = sr.Department_Name,
                                          MatricNumber = sr.Application_Form_Number,
                                          LevelName = level.Name

                                      }).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }

            return Masterpayments.OrderBy(o => o.Name).ToList();
        }

        private DateTime ConvertToDate(string date)
        {
            DateTime newDate = new DateTime();
            try
            {
                string[] dateSplit = date.Split('/');
                newDate = new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[0]));
            }
            catch (Exception)
            {
                throw;
            }

            return newDate;
        }

        public List<PaymentView> GetComprehensivePaymentFull(Session session, PaymentMode paymentMode)
        {
            List<PaymentView> payments = new List<PaymentView>();
            List<PaymentView> Masterpayments = new List<PaymentView>();
            try
            {
                if (session == null || session.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                payments = (from sr in repository.GetBy<VW_STUDENT_PAYMENT>(x => x.Session_Id == session.Id && x.Fee_Type_Id == 3)
                            select new PaymentView
                            {
                                PaymentId = sr.Payment_Id,
                                InvoiceNumber = sr.Invoice_Number,
                                PaymentTypeId = sr.Payment_Type_Id,
                                PaymentTypeName = sr.Payment_Type_Name,
                                PaymentModeId = sr.Payment_Mode_Id,
                                PaymentModeName = sr.Payment_Mode_Name,
                                FeeTypeId = sr.Fee_Type_Id,
                                FeeTypeName = sr.Fee_Type_Name,
                                SessionId = sr.Session_Id,
                                SessionName = sr.Session_Name,
                                PersonId = sr.Person_Id,
                                Name = sr.Full_Name,
                                ImageUrl = sr.Image_File_Url,
                                MobilePhone = sr.Mobile_Phone,
                                MatricNumber = sr.Matric_Number,
                                LevelId = sr.Level_Id,
                                LevelName = sr.Level_Name,
                                ProgrammeId = sr.Programme_Id,
                                ProgrammeName = sr.Programme_Name,
                                DepartmentId = sr.Department_Id,
                                DepartmentName = sr.Department_Name,
                                BankCode = sr.Bank_Code,
                                ConfirmationOrderNumber = sr.Confirmation_No,
                                ReceiptNumber = sr.Receipt_No,
                                Amount = sr.Transaction_Amount,
                                TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString()

                            }).ToList();

                if (paymentMode.Id == 100)
                {
                    List<long> Ids = payments.Where(a => a.ConfirmationOrderNumber != null).Select(p => p.PersonId).Distinct().ToList();
                    for (int i = 0; i < Ids.Count; i++)
                    {
                        long currentPersonId = Ids[i];
                        List<PaymentView> currentPayments = payments.Where(p => p.PersonId == currentPersonId && p.ConfirmationOrderNumber != null).ToList();
                        for (int j = 0; j < currentPayments.Count; j++)
                        {
                            if (currentPayments[j].PaymentModeId == 3 || currentPayments[j].PaymentModeId == 6)
                            {
                                currentPayments[0].SecondInstallmentAmount = currentPayments[j].Amount;
                                currentPayments[0].SecondInstallmentDate = currentPayments[j].TransactionDate;

                            }
                            if (currentPayments[j].PaymentModeId == 2 || currentPayments[j].PaymentModeId == 4)
                            {
                                currentPayments[0].FirstInstallmentAmount = currentPayments[j].Amount;
                                currentPayments[0].FirstInstallmentDate = currentPayments[j].TransactionDate;
                            }
                            if (currentPayments[j].PaymentModeId == 1)
                            {
                                currentPayments[0].FullPaymentAmount = currentPayments[j].Amount;
                                currentPayments[0].FullPaymentDate = currentPayments[j].TransactionDate;
                            }
                        }

                        Masterpayments.Add(currentPayments[0]);
                    }
                }
                if (paymentMode.Id == 2)
                {
                    List<long> Ids = payments.Where(a => a.ConfirmationOrderNumber != null && a.PaymentModeId != 1 && (a.PaymentModeId == 2 || a.PaymentModeId == 4)).Select(p => p.PersonId).Distinct().ToList();
                    for (int i = 0; i < Ids.Count; i++)
                    {
                        long currentPersonId = Ids[i];
                        bool checkPayment = true;
                        //int firstPaymentMode;
                        List<PaymentView> currentPayments = payments.Where(p => p.PersonId == currentPersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId != 1).ToList();
                        //firstPaymentMode = currentPayments[0].PaymentModeId;
                        for (int j = 0; j < currentPayments.Count; j++)
                        {
                            if (currentPayments[j].PaymentModeId == 2 || currentPayments[j].PaymentModeId == 4)
                            {
                                currentPayments[0].FirstInstallmentAmount = currentPayments[j].Amount;
                                currentPayments[0].FirstInstallmentDate = currentPayments[j].TransactionDate;
                            }
                            if (currentPayments[j].PaymentModeId == 3 || currentPayments[j].PaymentModeId == 6)
                            {
                                checkPayment = false;
                            }
                        }
                        if (checkPayment)
                        {
                            Masterpayments.Add(currentPayments[0]);
                        }
                    }
                }
                if (paymentMode.Id == 3)
                {
                    List<long> Ids = payments.Where(a => a.ConfirmationOrderNumber != null && a.PaymentModeId != 1).Select(p => p.PersonId).Distinct().ToList();
                    for (int i = 0; i < Ids.Count; i++)
                    {
                        long currentPersonId = Ids[i];
                        int firstPaymentMode;
                        decimal? firstPaymentAmount;
                        string firstPaymentDate;
                        List<PaymentView> currentPayments = payments.Where(p => p.PersonId == currentPersonId && p.PaymentModeId != 1 && p.ConfirmationOrderNumber != null).ToList();
                        if (currentPayments.Count >= 2)
                        {
                            for (int j = 0; j < currentPayments.Count; j++)
                            {
                                if (currentPayments[j].PaymentModeId == 3 || currentPayments[j].PaymentModeId == 6)
                                {
                                    currentPayments[0].SecondInstallmentAmount = currentPayments[j].Amount;
                                    currentPayments[0].SecondInstallmentDate = currentPayments[j].TransactionDate;

                                }
                                if (currentPayments[j].PaymentModeId == 2 || currentPayments[j].PaymentModeId == 4)
                                {
                                    currentPayments[0].FirstInstallmentAmount = currentPayments[j].Amount;
                                    currentPayments[0].FirstInstallmentDate = currentPayments[j].TransactionDate;
                                }
                            }

                            Masterpayments.Add(currentPayments[0]);
                        }
                    }
                }
                if (paymentMode.Id == 1)
                {
                    Masterpayments = payments.Where(a => a.ConfirmationOrderNumber != null && a.PaymentModeId == 1).ToList();

                }
                if (paymentMode.Id == 101)
                {
                    Masterpayments = (from sr in repository.GetBy<VW_STUDENTS_WITH_NO_SCHOOL_FEES>(x => x.Session_Id == session.Id)
                                      select new PaymentView
                                      {
                                          SessionId = sr.Session_Id,
                                          SessionName = sr.Session_Name,
                                          PersonId = sr.Person_Id,
                                          Name = sr.Full_Name,
                                          ImageUrl = sr.Image_File_Url,
                                          MobilePhone = sr.Mobile_Phone,
                                          ProgrammeId = sr.Programme_Id,
                                          ProgrammeName = sr.Programme_Name,
                                          DepartmentId = sr.Department_Id,
                                          DepartmentName = sr.Department_Name,
                                          MatricNumber = sr.Application_Form_Number

                                      }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Masterpayments.OrderBy(o => o.Name).OrderBy(o => o.ProgrammeName).ThenBy(o => o.LevelName).ThenBy(o => o.DepartmentName).ThenBy(o => o.MatricNumber).ToList();
        }

        public List<PaymentView> GetDebtorsCountFullPayment(Session session)
        {
            try
            {
                List<PaymentView> paymentViewsNew = new List<PaymentView>();
                List<PaymentView> paymentViewsOld = new List<PaymentView>();
                List<PaymentView> allPaymentViews = new List<PaymentView>();
                List<PaymentView> Masterpayments = new List<PaymentView>();

                List<PaymentView> newStudents = new List<PaymentView>();
                List<PaymentView> oldStudents = new List<PaymentView>();

                SessionLogic sessionLogic = new SessionLogic();
                Session currentSession = sessionLogic.GetModelsBy(s => s.Session_Id == session.Id).LastOrDefault();

                int lstSessionYear = Convert.ToInt32(currentSession.Name.Split('/').FirstOrDefault());
                string lstSessionName = (lstSessionYear - 1) + "/" + lstSessionYear;

                Session lastSession = sessionLogic.GetModelBy(s => s.Session_Name.Trim() == lstSessionName.Trim());

                //PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

                int[] ndProgrammes = { 1, 2 };
                int[] hndProgrammes = { 3, 4 };

                string[] ndProgrammeNames = { "ND Full Time", "ND Part Time" };
                string[] hndProgrammeNames = { "HND Full Time", "HND Part Time" };

                paymentViewsNew = (from sr in repository.GetBy<VW_DEBTORS_NEW_STUDENT>(a => a.Admitted_Session_Id == session.Id)
                                   select new PaymentView
                                   {
                                       SessionId = sr.Admitted_Session_Id,
                                       SessionName = sr.Admitted_Session,
                                       PersonId = sr.Person_Id,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       ProgrammeName = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       DepartmentName = sr.Admitted_Department,
                                       MatricNumber = sr.Application_Form_Number

                                   }).ToList();

                List<long> newStudentDistinctId = paymentViewsNew.Select(p => p.PersonId).Distinct().ToList();
                for (int i = 0; i < newStudentDistinctId.Count; i++)
                {
                    long currentId = newStudentDistinctId[i];
                    List<PaymentView> currentViews = paymentViewsNew.Where(p => p.PersonId == currentId).ToList();

                    for (int j = 0; j < currentViews.Count; j++)
                    {
                        if (ndProgrammes.Contains(currentViews[j].ProgrammeId) && ndProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            currentViews[j].ProgrammeName = currentViews[j].ProgrammeName + " ND I";
                        }
                        else if (hndProgrammes.Contains(currentViews[j].ProgrammeId) && hndProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            currentViews[j].ProgrammeName = currentViews[j].ProgrammeName + " HND I";
                        }
                        else
                        {
                            continue;
                        }

                        currentViews[j].PaymentCount = 1;
                        newStudents.Add(currentViews[j]);
                    }
                }

                int[] otherYears = { 3, 4, 5, 6, 7 };

                //session.Id == (int)Sessions._20162017
                if (otherYears.Contains(lastSession.Id))
                {
                    paymentViewsOld = (from sr in repository.GetBy<VW_DEBTORS_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Confirmation_No == null && (x.Admitted_Session_Id == lastSession.Id || x.Admitted_Session_Id == null))
                                       select new PaymentView
                                       {
                                           SessionId = sr.Session_Id,
                                           SessionName = sr.Session_Name,
                                           PersonId = sr.Person_Id,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           ProgrammeId = sr.Programme_Id,
                                           ProgrammeName = sr.Programme_Name,
                                           DepartmentId = sr.Department_Id,
                                           DepartmentName = sr.Department_Name,
                                           MatricNumber = sr.Matric_Number
                                       }).ToList();
                }
                else
                {
                    paymentViewsOld = (from sr in repository.GetBy<VW_DEBTORS_NEW_STUDENT>(x => x.Admitted_Session_Id == lastSession.Id)
                                       select new PaymentView
                                       {
                                           SessionId = sr.Admitted_Session_Id,
                                           SessionName = sr.Admitted_Session,
                                           PersonId = sr.Person_Id,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           ProgrammeId = sr.Admitted_Programme_Id,
                                           ProgrammeName = sr.Admitted_Programme,
                                           DepartmentId = sr.Admitted_Department_Id,
                                           DepartmentName = sr.Admitted_Department,
                                           MatricNumber = sr.Application_Form_Number
                                       }).ToList();
                }

                List<long> oldStudentDistinctId = paymentViewsOld.Select(p => p.PersonId).Distinct().ToList();
                for (int i = 0; i < oldStudentDistinctId.Count; i++)
                {
                    long currentId = oldStudentDistinctId[i];
                    List<PaymentView> currentViews = paymentViewsOld.Where(p => p.PersonId == currentId).ToList();

                    for (int j = 0; j < currentViews.Count; j++)
                    {
                        if (ndProgrammes.Contains(currentViews[j].ProgrammeId) && ndProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            currentViews[j].ProgrammeName = currentViews[j].ProgrammeName + " ND II";
                        }
                        else if (hndProgrammes.Contains(currentViews[j].ProgrammeId) && hndProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            currentViews[j].ProgrammeName = currentViews[j].ProgrammeName + " HND II";
                        }
                        else
                        {
                            continue;
                        }

                        currentViews[j].PaymentCount = 1;
                        oldStudents.Add(currentViews[j]);
                    }

                }

                allPaymentViews.AddRange(newStudents);
                allPaymentViews.AddRange(oldStudents);

                for (int i = 0; i < allPaymentViews.Count; i++)
                {
                    allPaymentViews[i].NewStudentDebtorsCount = newStudents.Count;
                    allPaymentViews[i].OldStudentDebtorsCount = oldStudents.Count;
                    allPaymentViews[i].TotalDebtorsCount = allPaymentViews.Count;
                }

                Masterpayments = allPaymentViews;

                return Masterpayments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PaymentView> GetDebtorsCountSecondInstallment(Session session)
        {
            try
            {
                List<PaymentView> paymentViewsNew = new List<PaymentView>();
                List<PaymentView> paymentViewsOld = new List<PaymentView>();
                List<PaymentView> allPaymentViews = new List<PaymentView>();
                List<PaymentView> Masterpayments = new List<PaymentView>();

                List<PaymentView> newStudents = new List<PaymentView>();
                List<PaymentView> oldStudents = new List<PaymentView>();

                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

                int[] ndProgrammes = { 1, 2, 5 };
                int[] hndProgrammes = { 3, 4 };
                int[] otherProgrammes = { 6, 7, 8 };
                string[] ndProgrammeNames = { "ND Morning (JAMB)", "ND Evening", "ND Part Time" };
                string[] hndProgrammeNames = { "HND Morning", "HND Evening" };
                string[] otherProgrammeNames = { "PRE-ND Programme", "Certificate", "Advance Certificate" };

                paymentViewsNew = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT_FIRST>(x => x.Admitted_Session_Id == session.Id)
                                   select new PaymentView
                                   {
                                       SessionId = sr.Admitted_Session_Id,
                                       SessionName = sr.Admitted_Session,
                                       PersonId = sr.Person_Id,
                                       Name = sr.Name,
                                       ImageUrl = sr.Image_File_Url,
                                       ProgrammeId = sr.Admitted_Programme_Id,
                                       ProgrammeName = sr.Admitted_Programme,
                                       DepartmentId = sr.Admitted_Department_Id,
                                       DepartmentName = sr.Admitted_Department,
                                       MatricNumber = sr.Application_Form_Number,
                                       PaymentModeId = sr.Payment_Mode_Id

                                   }).ToList();

                List<long> newStudentDistinctId = paymentViewsNew.Select(p => p.PersonId).Distinct().ToList();
                for (int i = 0; i < newStudentDistinctId.Count; i++)
                {
                    long currentId = newStudentDistinctId[i];
                    List<PaymentView> currentViews = paymentViewsNew.Where(p => p.PersonId == currentId).ToList();

                    for (int j = 0; j < currentViews.Count; j++)
                    {
                        if (ndProgrammes.Contains(currentViews[j].ProgrammeId) && ndProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            currentViews[j].ProgrammeName = currentViews[j].ProgrammeName + " ND I";
                        }
                        else if (hndProgrammes.Contains(currentViews[j].ProgrammeId) && hndProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            currentViews[j].ProgrammeName = currentViews[j].ProgrammeName + " HND I";
                        }
                        else if (otherProgrammes.Contains(currentViews[j].ProgrammeId) && otherProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            //do nothing
                        }
                        else
                        {
                            continue;
                        }

                        currentViews[j].PaymentCount = 1;
                        newStudents.Add(currentViews[j]);
                    }
                }

                if (session.Id == (int)Sessions._20162017)
                {
                    paymentViewsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && (x.Admitted_Session_Id != null && x.Admitted_Session_Id != session.Id))
                                       select new PaymentView
                                       {
                                           SessionId = sr.Session_Id,
                                           SessionName = sr.Session_Name,
                                           PersonId = sr.Person_Id,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           ProgrammeId = sr.Programme_Id,
                                           ProgrammeName = sr.Programme_Name,
                                           DepartmentId = sr.Department_Id,
                                           DepartmentName = sr.Department_Name,
                                           MatricNumber = sr.Matric_Number,
                                           PaymentModeId = sr.Payment_Mode_Id
                                       }).ToList();
                }
                else
                {
                    paymentViewsOld = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT_FIRST>(x => x.Session_Id == session.Id && x.Admitted_Session_Id == null)
                                       select new PaymentView
                                       {
                                           SessionId = sr.Session_Id,
                                           SessionName = sr.Session_Name,
                                           PersonId = sr.Person_Id,
                                           Name = sr.Name,
                                           ImageUrl = sr.Image_File_Url,
                                           ProgrammeId = sr.Programme_Id,
                                           ProgrammeName = sr.Programme_Name,
                                           DepartmentId = sr.Department_Id,
                                           DepartmentName = sr.Department_Name,
                                           MatricNumber = sr.Matric_Number,
                                           PaymentModeId = sr.Payment_Mode_Id
                                       }).ToList();
                }

                List<long> oldStudentDistinctId = paymentViewsOld.Select(p => p.PersonId).Distinct().ToList();
                for (int i = 0; i < oldStudentDistinctId.Count; i++)
                {
                    long currentId = oldStudentDistinctId[i];
                    List<PaymentView> currentViews = paymentViewsOld.Where(p => p.PersonId == currentId).ToList();

                    for (int j = 0; j < currentViews.Count; j++)
                    {
                        if (ndProgrammes.Contains(currentViews[j].ProgrammeId) && ndProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            currentViews[j].ProgrammeName = currentViews[j].ProgrammeName + " ND II";
                        }
                        else if (hndProgrammes.Contains(currentViews[j].ProgrammeId) && hndProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            currentViews[j].ProgrammeName = currentViews[j].ProgrammeName + " HND II";
                        }
                        else if (otherProgrammes.Contains(currentViews[j].ProgrammeId) && otherProgrammeNames.Contains(currentViews[j].ProgrammeName))
                        {
                            //do nothing
                        }
                        else
                        {
                            continue;
                        }

                        currentViews[j].PaymentCount = 1;
                        oldStudents.Add(currentViews[j]);
                    }

                }

                allPaymentViews.AddRange(newStudents);
                allPaymentViews.AddRange(oldStudents);

                for (int i = 0; i < allPaymentViews.Count; i++)
                {
                    //PaymentView thisView = allPaymentViews[i];
                    //PAYMENT_ETRANZACT paymentEtranzact = paymentEtranzactLogic.GetEntityBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == thisView.PersonId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id == (int)PaymentModes.Part);
                    PaymentView secondInstallment = allPaymentViews.Where(p => p.PersonId == allPaymentViews[i].PersonId && p.PaymentModeId == (int)PaymentModes.SecondInstallment).LastOrDefault();

                    if (secondInstallment == null)
                    {
                        allPaymentViews[i].NewStudentDebtorsCount = newStudents.Count;
                        allPaymentViews[i].OldStudentDebtorsCount = oldStudents.Count;
                        allPaymentViews[i].TotalDebtorsCount = allPaymentViews.Count;

                        Masterpayments.Add(allPaymentViews[i]);
                    }
                }

                return Masterpayments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PaymentView> GetDebtorsCountFullPaymentAlt(Session session)
        {
            List<PaymentView> paymentViewsNew = new List<PaymentView>();
            List<PaymentView> paymentViewsOld = new List<PaymentView>();
            List<PaymentView> allPaymentViews = new List<PaymentView>();
            List<PaymentView> Masterpayments = new List<PaymentView>();

            List<PaymentView> newStudents = new List<PaymentView>();
            List<PaymentView> oldStudents = new List<PaymentView>();

            int[] ndProgrammes = { 1, 2 };
            int[] hndProgrammes = { 3, 4 };

            string[] ndProgrammeNames = { "ND Full Time", "ND Part Time" };
            string[] hndProgrammeNames = { "HND Full Time", "HND Part Time" };

            SessionLogic sessionLogic = new SessionLogic();
            LevelLogic levelLogic = new LevelLogic();

            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

            try
            {
                if (session == null || session.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                //Old Students

                Session fullSession = sessionLogic.GetModelBy(s => s.Session_Id == session.Id);
                int sessionStart = Convert.ToInt32(fullSession.Name.Split('/').FirstOrDefault());
                string prevSessionName = sessionStart - 1 + "/" + sessionStart;

                Session prevSession = sessionLogic.GetModelBy(s => s.Session_Name == prevSessionName);

                List<STUDENT_COURSE_REGISTRATION> courseRegistrations = courseRegistrationLogic.GetEntitiesBy(x => x.Session_Id == prevSession.Id);
                List<long> distinctPersons = courseRegistrations.Select(s => s.Person_Id).Distinct().ToList();

                for (int i = 0; i < distinctPersons.Count; i++)
                {
                    long currentPersonId = distinctPersons[i];
                    STUDENT_COURSE_REGISTRATION courseRegistration = courseRegistrations.LastOrDefault(c => c.Person_Id == currentPersonId);

                    PaymentView paymentView = new PaymentView();
                    paymentView.PersonId = courseRegistration.Person_Id;
                    paymentView.SessionId = fullSession.Id;
                    paymentView.SessionName = fullSession.Name;
                    paymentView.Name = courseRegistration.STUDENT.PERSON.Last_Name + " " + courseRegistration.STUDENT.PERSON.First_Name + " " + courseRegistration.STUDENT.PERSON.Other_Name;
                    paymentView.ProgrammeId = courseRegistration.Programme_Id;
                    paymentView.ProgrammeName = courseRegistration.PROGRAMME.Programme_Name;
                    paymentView.DepartmentId = courseRegistration.Department_Id;
                    paymentView.DepartmentName = courseRegistration.DEPARTMENT.Department_Name;
                    paymentView.MatricNumber = courseRegistration.STUDENT.Matric_Number;
                    paymentView.LevelName = courseRegistration.LEVEL.Level_Name;

                    PaymentView currentPayment = paymentView;

                    PAYMENT_ETRANZACT schoolFeePayment = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == currentPayment.PersonId &&
                                p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.ONLINE_PAYMENT.PAYMENT.Session_Id == currentPayment.SessionId).LastOrDefault();
                    if (schoolFeePayment == null)
                    {
                        REMITA_PAYMENT schoolFeeRemitaPayment = remitaPaymentLogic.GetEntitiesBy(r => r.PAYMENT.Person_Id == currentPayment.PersonId &&
                                    r.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees && r.PAYMENT.Session_Id == currentPayment.SessionId && (r.Status.Contains("01:") || r.Status.Contains("00"))).LastOrDefault();
                        if (schoolFeeRemitaPayment == null)
                        {
                            if (ndProgrammes.Contains(currentPayment.ProgrammeId) && ndProgrammeNames.Contains(currentPayment.ProgrammeName))
                            {
                                currentPayment.ProgrammeName = currentPayment.ProgrammeName + " ND II";
                            }
                            else if (hndProgrammes.Contains(currentPayment.ProgrammeId) && hndProgrammeNames.Contains(currentPayment.ProgrammeName))
                            {
                                currentPayment.ProgrammeName = currentPayment.ProgrammeName + " HND II";
                            }
                            else
                            {
                                continue;
                            }

                            currentPayment.PaymentCount = 1;
                            paymentViewsOld.Add(currentPayment);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }


                //New Students
                newStudents = (from sr in repository.GetBy<VW_ADMISSION_LIST>(x => x.Session_Id == session.Id)
                               select new PaymentView
                               {
                                   SessionId = Convert.ToInt32(sr.Session_Id),
                                   SessionName = sr.Session_Name,
                                   PersonId = sr.Person_Id,
                                   Name = sr.FullName,
                                   ProgrammeId = sr.Programme_Id,
                                   ProgrammeName = sr.Programme_Name,
                                   DepartmentId = sr.Department_Id,
                                   DepartmentName = sr.Department_Name,
                                   MatricNumber = sr.Application_Form_Number

                               }).ToList();


                for (int i = 0; i < newStudents.Count; i++)
                {
                    PaymentView currentPayment = newStudents[i];


                    PAYMENT_ETRANZACT acceptancePayment = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == currentPayment.PersonId &&
                            p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.AcceptanceFee && p.ONLINE_PAYMENT.PAYMENT.Session_Id == currentPayment.SessionId).LastOrDefault();
                    if (acceptancePayment == null)
                    {
                        REMITA_PAYMENT acceptanceRemitaPayment = remitaPaymentLogic.GetEntitiesBy(r => r.PAYMENT.Person_Id == currentPayment.PersonId &&
                                    r.PAYMENT.Fee_Type_Id == (int)FeeTypes.AcceptanceFee && r.PAYMENT.Session_Id == currentPayment.SessionId && (r.Status.Contains("01:") || r.Status.Contains("00"))).LastOrDefault();
                        if (acceptanceRemitaPayment == null)
                        {
                            continue;
                        }
                    }


                    PAYMENT_ETRANZACT schoolFeePayment = paymentEtranzactLogic.GetEntitiesBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == currentPayment.PersonId &&
                                p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.ONLINE_PAYMENT.PAYMENT.Session_Id == currentPayment.SessionId).LastOrDefault();
                    if (schoolFeePayment == null)
                    {
                        REMITA_PAYMENT schoolFeeRemitaPayment = remitaPaymentLogic.GetEntitiesBy(r => r.PAYMENT.Person_Id == currentPayment.PersonId &&
                                    r.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees && r.PAYMENT.Session_Id == currentPayment.SessionId && (r.Status.Contains("01:") || r.Status.Contains("00"))).LastOrDefault();
                        if (schoolFeeRemitaPayment == null)
                        {
                            if (ndProgrammes.Contains(currentPayment.ProgrammeId) && ndProgrammeNames.Contains(currentPayment.ProgrammeName))
                            {
                                currentPayment.ProgrammeName = currentPayment.ProgrammeName + " ND I";
                            }
                            else if (hndProgrammes.Contains(currentPayment.ProgrammeId) && hndProgrammeNames.Contains(currentPayment.ProgrammeName))
                            {
                                currentPayment.ProgrammeName = currentPayment.ProgrammeName + " HND I";
                            }
                            else
                            {
                                continue;
                            }

                            currentPayment.PaymentCount = 1;
                            paymentViewsNew.Add(currentPayment);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                allPaymentViews.AddRange(paymentViewsNew);
                allPaymentViews.AddRange(paymentViewsOld);

                for (int i = 0; i < allPaymentViews.Count; i++)
                {
                    allPaymentViews[i].NewStudentDebtorsCount = paymentViewsNew.Count;
                    allPaymentViews[i].OldStudentDebtorsCount = paymentViewsOld.Count;
                    allPaymentViews[i].TotalDebtorsCount = allPaymentViews.Count;
                }

                Masterpayments = allPaymentViews;

            }
            catch (Exception)
            {
                throw;
            }

            return Masterpayments.OrderBy(o => o.Name).ToList();
        }
    }



}


