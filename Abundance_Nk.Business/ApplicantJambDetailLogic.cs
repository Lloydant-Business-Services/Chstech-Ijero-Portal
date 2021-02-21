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
    public class ApplicantJambDetailLogic : BusinessBaseLogic<ApplicantJambDetail, APPLICANT_JAMB_DETAIL>
    {
        public ApplicantJambDetailLogic()
        {
            translator = new ApplicantJambDetailTranslator();
        }

        public bool Modify(ApplicantJambDetail jambDetail)
        {
            try
            {
                Expression<Func<APPLICANT_JAMB_DETAIL, bool>> selector = p => p.Person_Id == jambDetail.Person.Id;
                APPLICANT_JAMB_DETAIL entity = GetEntityBy(selector);

                if (entity == null || entity.Person_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Person_Id = jambDetail.Person.Id;
                entity.Applicant_Jamb_Registration_Number = jambDetail.JambRegistrationNumber;
                entity.Applicant_Jamb_Score = jambDetail.JambScore != null ? jambDetail.JambScore : entity.Applicant_Jamb_Score;

                if (jambDetail.InstitutionChoice != null)
                {
                    entity.Institution_Choice_Id = jambDetail.InstitutionChoice.Id;
                }
                if (jambDetail.Person != null)
                {
                    entity.Person_Id = jambDetail.Person.Id;
                }

                if (jambDetail.ApplicationForm != null)
                {
                    entity.Application_Form_Id = jambDetail.ApplicationForm.Id;
                }

                if (jambDetail.Subject1 != null)
                {
                    entity.Subject1 = jambDetail.Subject1.Id;
                }
                if (jambDetail.Subject2 != null)
                {
                    entity.Subject2 = jambDetail.Subject2.Id;
                }
                if (jambDetail.Subject3 != null)
                {
                    entity.Subject3 = jambDetail.Subject3.Id;
                }
                if (jambDetail.Subject4 != null)
                {
                    entity.Subject4 = jambDetail.Subject4.Id;
                }

                if (jambDetail.Subject1Score != null)
                {
                    entity.Subject1_Score = jambDetail.Subject1Score;
                }
                if (jambDetail.Subject2Score != null)
                {
                    entity.Subject2_Score = jambDetail.Subject2Score;
                }
                if (jambDetail.Subject3Score != null)
                {
                    entity.Subject3_Score = jambDetail.Subject3Score;
                }
                if (jambDetail.Subject4Score != null)
                {
                    entity.Subject4_Score = jambDetail.Subject4Score;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;

                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ApplicantResult> GetScannedNDJambApplicantResults(Department department, Session session)
        {
            try
            {
                List<ApplicantResult> applicantResults = (from sr in repository.GetBy<VW_APPLICANT_RESULT>(x => x.Programme_Id == 1 && x.Department_Id == department.Id && x.Session_Id == session.Id)
                                                          select new ApplicantResult()
                                                          {
                                                              JambRegNumber = sr.JambRegNumber,
                                                              JambScore = sr.Applicant_Jamb_Score,
                                                              JambSubjects = sr.JambSubjects,
                                                              OLevelType = sr.O_Level_Type_Name,
                                                              OLevelYear = sr.Exam_Year.ToString(),
                                                              NumberOfSittings = sr.O_Level_Exam_Sitting_Id,
                                                              SubjectName = sr.O_Level_Subject_Name,
                                                              Grade = sr.O_Level_Grade_Name,
                                                              Name = sr.Name,
                                                              Programme = sr.Programme_Name,
                                                              Department = sr.Department_Name,
                                                              Session = sr.Session_Name,
                                                              ImageUrl = sr.Image_File_Url,
                                                              ApplicationFormNumber = sr.Application_Form_Number ,
                                                              ScannedCopyUrl = sr.Scanned_Copy_Url,
                                                              PersonId = sr.Person_Id
                                                          }).ToList();

                return applicantResults.OrderBy(m => m.Name).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<ApplicantResult> GetApplicantAggregateScore(Person person, Programme programme, Department department, Session session)
        {
            try
            {
                List<ApplicantResult> applicantResults = (from sr in repository.GetBy<VW_APPLICANT_RESULT>(x => x.Person_Id == person.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id)
                                                          select new ApplicantResult()
                                                          {
                                                              LastName = sr.Last_Name,
                                                              FirstName = sr.First_Name,
                                                              AggregateScore = sr.Total, 
                                                          }).ToList();
                return applicantResults;


               
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ApplicantResult> GetApplicantResults(Department department, Session session)
        {
            try
            {
                List<ApplicantResult> applicantResults = (from sr in repository.GetBy<VW_APPLICANT_RESULT>(x => x.Programme_Id == 1 && x.Department_Id == department.Id && x.Session_Id == session.Id)
                                                          select new ApplicantResult()
                                                          {
                                                              JambRegNumber = sr.JambRegNumber,
                                                              JambScore = sr.Applicant_Jamb_Score,
                                                              JambSubjects = sr.JambSubjects,
                                                              OLevelType = sr.O_Level_Type_Name,
                                                              OLevelYear = sr.Exam_Year.ToString(),
                                                              NumberOfSittings = sr.O_Level_Exam_Sitting_Id,
                                                              SubjectName = sr.O_Level_Subject_Name,
                                                              Grade = sr.O_Level_Grade_Name,
                                                              Name = sr.Name,
                                                              Programme = sr.Programme_Name,
                                                              Department = sr.Department_Name,
                                                              Session = sr.Session_Name,
                                                              ImageUrl = sr.Image_File_Url,
                                                              ApplicationFormNumber = sr.Application_Form_Number,
                                                              LastName = sr.Last_Name,
                                                              FirstName =  sr.First_Name,
                                                              OtherName = sr.Other_Name,
                                                              JambPoint = sr.Jamb_Raw_Score,
                                                              OlevelPoint = sr.OLevel_Raw_Score,
                                                              AggregateScore =  sr.Total,
                                                              DateOfBirth =  sr.Date_Of_Birth.Value.ToShortDateString(),
                                                              Sex = sr.Sex_Name,
                                                              Phone =  sr.Mobile_Phone,
                                                              Email = sr.Email,
                                                              ContactAddress = sr.Home_Address,
                                                              OlevelResultCheckingPin = sr.Scratch_Card_Pin,
                                                              OlevelResultCheckingSerialNumber = sr.Scratch_Card_SerialNo

                                                          }).ToList();
                List<ApplicantResult> masterList = new List<ApplicantResult>();
                List<string> regNumbers = applicantResults.Select(r => r.JambRegNumber).Distinct().ToList();
                for (int i = 0; i < regNumbers.Count; i++)
                {
                    List<ApplicantResult> results = applicantResults.Where(r => r.JambRegNumber == regNumbers[i]).ToList();
                    string firstOLevelType = results[0].OLevelType;
                    string firstOLevelYear = results[0].OLevelYear;
                    int firstOlevelSitting = results[0].NumberOfSittings;
                    string OLevelResults = "";
                    int checkSittingChange = 0;
                    string oLevelType = "";
                    string oLevelYear = "";
                    string sitting = "";

                    for (int j = 0; j < results.Count; j++)
                    {
                        if (results[j].NumberOfSittings != firstOlevelSitting)
                        {
                            checkSittingChange += 1;
                            results[j].Sitting = "Two Sittings";
                            sitting = results[j].Sitting;
                            results[j].OLevelType = firstOLevelType + " | " + results[j].OLevelType;
                            oLevelType = results[j].OLevelType;
                            results[j].OLevelYear = firstOLevelYear + " | " + results[j].OLevelYear;
                            oLevelYear = results[j].OLevelYear;
                        }
                        else
                        {
                            if (oLevelType != "" && oLevelYear != "" && sitting != "")
                            {
                                results[j].Sitting = sitting;
                                results[j].OLevelYear = oLevelYear;
                                results[j].OLevelType = oLevelType;
                            }
                            else
                            {
                               results[j].Sitting = "One Sitting"; 
                            }   
                        }

                        if (checkSittingChange == 1)
                        {
                            OLevelResults += "==== 2ND RESULT ====> ";
                        }
                        OLevelResults += results[j].SubjectName + " : " + results[j].Grade + " | ";
                        results[j].OLevelResults = OLevelResults;
                    }

                    results.LastOrDefault().OLevelResults = OLevelResults;
                    
                }

                masterList = applicantResults.GroupBy(a => a.JambRegNumber).Select(a => a.Last()).ToList();

                return masterList.OrderBy(m => m.JambRegNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ApplicantResult> GetApplicantsByChoice(Department department, Session session, InstitutionChoice institutionChoice)
        {
            try
            {
                List<ApplicantResult> applicantResults = (from sr in repository.GetBy<VW_APPLICANT_RESULT>(x => x.Programme_Id == 1 && x.Department_Id == department.Id && x.Session_Id == session.Id && x.Institution_Choice_Id == institutionChoice.Id)
                                                          select new ApplicantResult()
                                                          {
                                                              JambRegNumber = sr.JambRegNumber,
                                                              JambScore = sr.Applicant_Jamb_Score,
                                                              JambSubjects = sr.JambSubjects,
                                                              OLevelType = sr.O_Level_Type_Name,
                                                              OLevelYear = sr.Exam_Year.ToString(),
                                                              NumberOfSittings = sr.O_Level_Exam_Sitting_Id,
                                                              SubjectName = sr.O_Level_Subject_Name,
                                                              Grade = sr.O_Level_Grade_Name,
                                                              Name = sr.Name,
                                                              Programme = sr.Programme_Name,
                                                              Department = sr.Department_Name,
                                                              Session = sr.Session_Name,
                                                              ImageUrl = sr.Image_File_Url,
                                                              ApplicationFormNumber = sr.Application_Form_Number,
                                                              InstitutionChoice = sr.Institution_Choice_Name
                                                          }).ToList();
                List<ApplicantResult> masterList = new List<ApplicantResult>();
                List<string> regNumbers = applicantResults.Select(r => r.JambRegNumber).Distinct().ToList();
                for (int i = 0; i < regNumbers.Count; i++)
                {
                    List<ApplicantResult> results = applicantResults.Where(r => r.JambRegNumber == regNumbers[i]).ToList();
                    string firstOLevelType = results.FirstOrDefault().OLevelType;
                    string firstOLevelYear = results.FirstOrDefault().OLevelYear;
                    int firstOlevelSitting = results.FirstOrDefault().NumberOfSittings;
                    string OLevelResults = "";
                    int checkSittingChange = 0;
                    string oLevelType = "";
                    string oLevelYear = "";
                    string sitting = "";

                    for (int j = 0; j < results.Count; j++)
                    {
                        if (results[j].NumberOfSittings != firstOlevelSitting)
                        {
                            checkSittingChange += 1;
                            results[j].Sitting = "Two Sittings";
                            sitting = results[j].Sitting;
                            results[j].OLevelType = firstOLevelType + " | " + results[j].OLevelType;
                            oLevelType = results[j].OLevelType;
                            results[j].OLevelYear = firstOLevelYear + " | " + results[j].OLevelYear;
                            oLevelYear = results[j].OLevelYear;
                        }
                        else
                        {
                            if (oLevelType != "" && oLevelYear != "" && sitting != "")
                            {
                                results[j].Sitting = sitting;
                                results[j].OLevelYear = oLevelYear;
                                results[j].OLevelType = oLevelType;
                            }
                            else
                            {
                                results[j].Sitting = "One Sitting";
                            }
                        }

                        if (checkSittingChange == 1)
                        {
                            OLevelResults += "==== 2ND RESULT ====> ";
                        }
                        OLevelResults += results[j].SubjectName + " : " + results[j].Grade + " | ";
                        results[j].OLevelResults = OLevelResults;
                    }

                    results.LastOrDefault().OLevelResults = OLevelResults;
                }

                masterList = applicantResults.GroupBy(a => a.JambRegNumber).Select(a => a.Last()).ToList();

                return masterList.OrderBy(m => m.JambRegNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ApplicantResult> GetVerificationReport(Department department, Session session, Programme programme)
        {
            try
            {
                List<ApplicantResult> applicantResults = (from sr in repository.GetBy<VW_VERIFICATION_REPORT>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Session_Id == session.Id)
                                                          select new ApplicantResult()
                                                          {
                                                              OLevelType = sr.O_Level_Type_Name,
                                                              OLevelYear = sr.Exam_Year.ToString(),
                                                              NumberOfSittings = sr.O_Level_Exam_Sitting_Id,
                                                              SubjectName = sr.O_Level_Subject_Name,
                                                              Grade = sr.O_Level_Grade_Name,
                                                              ExamNumber = sr.Exam_Number,
                                                              Name = sr.Name,
                                                              Programme = sr.Programme_Name,
                                                              Department = sr.Department_Name,
                                                              Session = sr.Session_Name,
                                                              ImageUrl = "",
                                                              ApplicationFormNumber = sr.Application_Form_Number,
                                                              VerificationStatus = sr.Verification_Status,
                                                              VerificationComment = sr.Verification_Comment,
                                                              VerificationOfficer = sr.Verification_Officer

                                                          }).ToList();
                List<ApplicantResult> masterList = new List<ApplicantResult>();
                List<string> regNumbers = applicantResults.Select(r => r.ApplicationFormNumber).Distinct().ToList();
                for (int i = 0; i < regNumbers.Count; i++)
                {
                    List<ApplicantResult> results = applicantResults.Where(r => r.ApplicationFormNumber == regNumbers[i]).ToList();
                    string firstOLevelType = results[0].OLevelType;
                    string firstOLevelYear = results[0].OLevelYear;
                    int firstOlevelSitting = results[0].NumberOfSittings;
                    string OLevelResults = "";
                    int checkSittingChange = 0;
                    string oLevelType = "";
                    string oLevelYear = "";
                    string sitting = "";

                    for (int j = 0; j < results.Count; j++)
                    {
                        if (results[j].NumberOfSittings != firstOlevelSitting)
                        {
                            checkSittingChange += 1;
                            results[j].Sitting = "Two Sittings";
                            sitting = results[j].Sitting;
                            results[j].OLevelType = firstOLevelType + " | " + results[j].OLevelType;
                            oLevelType = results[j].OLevelType;
                            results[j].OLevelYear = firstOLevelYear + " | " + results[j].OLevelYear;
                            oLevelYear = results[j].OLevelYear;
                        }
                        else
                        {
                            if (oLevelType != "" && oLevelYear != "" && sitting != "")
                            {
                                results[j].Sitting = sitting;
                                results[j].OLevelYear = oLevelYear;
                                results[j].OLevelType = oLevelType;
                            }
                            else
                            {
                                results[j].Sitting = "One Sitting";
                            }
                        }

                        if (checkSittingChange == 1)
                        {
                            OLevelResults += "==== 2ND RESULT ====> ";
                        }

                        if (results[j].VerificationStatus == null)
                        {
                            results[j].VerificationStatusStr = "Pending";
                        }
                        else if (results[j].VerificationStatus == true)
                        {
                            results[j].VerificationStatusStr = "Accepted";
                        }
                        else if (results[j].VerificationStatus == false)
                        {
                            results[j].VerificationStatusStr = "Rejected";
                        }

                        OLevelResults += results[j].SubjectName + " : " + results[j].Grade + " | ";
                        results[j].OLevelResults = OLevelResults;
                    }

                    results.LastOrDefault().OLevelResults = OLevelResults;

                }

                masterList = applicantResults.GroupBy(a => a.ApplicationFormNumber).Select(a => a.Last()).ToList();

                return masterList.OrderBy(m => m.ApplicationFormNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }




}
