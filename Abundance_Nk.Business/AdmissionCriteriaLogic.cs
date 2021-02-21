using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Business
{
    public class AdmissionCriteriaLogic : BusinessBaseLogic<AdmissionCriteria, ADMISSION_CRITERIA>
    {
        private OLevelResultLogic oLevelResultLogic;
        private OLevelResultDetailLogic oLevelResultDetailLogic;
        private AdmissionCriteriaForOLevelSubjectLogic admissionCriteriaForOLevelSubjectLogic;
        private AdmissionCriteriaForOLevelTypeLogic admissionCriteriaForOLevelTypeLogic;
        private AdmissionCriteriaForOLevelSubjectAlternativeLogic admissionCriteriaForOLevelSubjectAlternativeLogic;

        public AdmissionCriteriaLogic()
        {
            translator = new AdmissionCriteriaTranslator();

            admissionCriteriaForOLevelTypeLogic = new AdmissionCriteriaForOLevelTypeLogic();
            admissionCriteriaForOLevelSubjectLogic = new AdmissionCriteriaForOLevelSubjectLogic();
            admissionCriteriaForOLevelSubjectAlternativeLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
            oLevelResultDetailLogic = new OLevelResultDetailLogic();
            oLevelResultLogic = new OLevelResultLogic();
        }

        public string EvaluateApplication(AppliedCourse applicantAppliedCourse)
        {
            try
            {
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();

                Person applicant = applicantAppliedCourse.Person;
                Programme programme = programmeLogic.GetModelBy(p => p.Programme_Id == applicantAppliedCourse.Programme.Id);
                Department department = departmentLogic.GetModelBy(d => d.Department_Id == applicantAppliedCourse.Department.Id);

                applicantAppliedCourse.Programme = programme;
                applicantAppliedCourse.Department = department; 

                //ADMISSION CRITERIA
                //get admission criteria
                Expression<Func<ADMISSION_CRITERIA, bool>> selector = ac => ac.Department_Id == department.Id && ac.Programme_Id == programme.Id;
                AdmissionCriteria admissionCriteria = GetModelsBy(selector).FirstOrDefault();

                if (admissionCriteria == null || admissionCriteria.Id <= 0)
                {
                    throw new Exception("No Admission Criteria found for " + programme.Name + " in " + department.Name);
                }

                //get admission criteria o-level type
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_TYPE, bool>> otSelector = ac => ac.Admission_Criteria_Id == admissionCriteria.Id;
                List<AdmissionCriteriaForOLevelType> requiredOlevelTypes = admissionCriteriaForOLevelTypeLogic.GetModelsBy(otSelector);

                //get admission criteria o-level subject
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT, bool>> scSelector = s => s.Admission_Criteria_Id == admissionCriteria.Id && s.Is_Compulsory == true;
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT, bool>> osSelector = s => s.Admission_Criteria_Id == admissionCriteria.Id && s.Is_Compulsory == false;
                List<AdmissionCriteriaForOLevelSubject> compulsorySubjects = admissionCriteriaForOLevelSubjectLogic.GetModelsBy(scSelector);
                List<AdmissionCriteriaForOLevelSubject> otherSubjects = admissionCriteriaForOLevelSubjectLogic.GetModelsBy(osSelector);
                
                //APPLICANT
                //get applicant result
                Expression<Func<APPLICANT_O_LEVEL_RESULT, bool>> firstSittingOlevelSelector = f => f.Person_Id == applicant.Id && f.O_Level_Exam_Sitting_Id == 1 && f.Application_Form_Id != null;
                Expression<Func<APPLICANT_O_LEVEL_RESULT, bool>> secondSittingOlevelSelector = s => s.Person_Id == applicant.Id && s.O_Level_Exam_Sitting_Id == 2 && s.Application_Form_Id != null;
                OLevelResult firstSittingOlevelResult = oLevelResultLogic.GetModelsBy(firstSittingOlevelSelector).LastOrDefault();
                OLevelResult secondSittingOlevelResult = oLevelResultLogic.GetModelsBy(secondSittingOlevelSelector).LastOrDefault();


                //get applicant o-level subjects
                List<OLevelResultDetail> firstSittingResultDetails = null;
                List<OLevelResultDetail> secondSittingResultDetails = null;
                if (firstSittingOlevelResult != null && firstSittingOlevelResult.Id > 0)
                {
                    Expression<Func<APPLICANT_O_LEVEL_RESULT_DETAIL, bool>> firstSittingSubjectSelector = fs => fs.Applicant_O_Level_Result_Id == firstSittingOlevelResult.Id;
                    firstSittingResultDetails = oLevelResultDetailLogic.GetModelsBy(firstSittingSubjectSelector);
                }

                if (secondSittingOlevelResult != null && secondSittingOlevelResult.Id > 0)
                {
                    Expression<Func<APPLICANT_O_LEVEL_RESULT_DETAIL, bool>> secondSittingSubjectSelector = ss => ss.Applicant_O_Level_Result_Id == secondSittingOlevelResult.Id;
                    secondSittingResultDetails = oLevelResultDetailLogic.GetModelsBy(secondSittingSubjectSelector);
                }

                if ((firstSittingResultDetails == null || firstSittingResultDetails.Count <= 0) && (secondSittingResultDetails == null || secondSittingResultDetails.Count <= 0))
                {
                    return "No O-Level result found for applicant!";
                }

                string oLevelTypeRejectReason = CheckOlevelType(requiredOlevelTypes, firstSittingOlevelResult, secondSittingOlevelResult, applicantAppliedCourse);
                string requiredSubjectsRejectReason = null;

                if (programme.Id == (int)Programmes.NDFullTime)
                {
                    RankingDataLogic rankingDataLogic = new RankingDataLogic();
                    requiredSubjectsRejectReason = rankingDataLogic.CheckRequiredOlevelSubjectsND(compulsorySubjects, otherSubjects, firstSittingResultDetails, secondSittingResultDetails, applicantAppliedCourse);
                }
                else
                {
                    requiredSubjectsRejectReason = CheckRequiredOlevelSubjects(compulsorySubjects, otherSubjects, firstSittingResultDetails, secondSittingResultDetails, applicantAppliedCourse);
                }
                
                string rejectReason = null;
                if (oLevelTypeRejectReason != null && requiredSubjectsRejectReason != null)
                {
                    rejectReason = oLevelTypeRejectReason + ". " + requiredSubjectsRejectReason;
                }
                else if (oLevelTypeRejectReason == null && requiredSubjectsRejectReason != null)
                {
                    rejectReason = requiredSubjectsRejectReason;
                }
                else if (oLevelTypeRejectReason != null && requiredSubjectsRejectReason == null)
                {
                    rejectReason = oLevelTypeRejectReason;
                }

                //rejectReason += CheckPreviousEducation(previousEducation, programme);

                if (!string.IsNullOrWhiteSpace(rejectReason))
                {
                    rejectReason += " Therefore did not qualify for admission.";
                }

                return rejectReason;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string EvaluateApplication(AppliedCourse applicantAppliedCourse, PreviousEducation previousEducation)
        {
            try
            {
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();

                Person applicant = applicantAppliedCourse.Person;
                Programme programme = programmeLogic.GetModelBy(p => p.Programme_Id == applicantAppliedCourse.Programme.Id);
                Department department = departmentLogic.GetModelBy(d => d.Department_Id == applicantAppliedCourse.Department.Id);

                applicantAppliedCourse.Programme = programme;
                applicantAppliedCourse.Department = department; 

                string rejectReason = null;
                //ADMISSION CRITERIA
                //get admission criteria
                Expression<Func<ADMISSION_CRITERIA, bool>> selector = ac => ac.Department_Id == department.Id && ac.Programme_Id == programme.Id;
                AdmissionCriteria admissionCriteria = GetModelsBy(selector).FirstOrDefault();

                if (admissionCriteria == null || admissionCriteria.Id <= 0)
                {
                    throw new Exception("No Admission Criteria found for " + programme.Name + " in " + department.Name);
                }

                //if (previousEducation == null || previousEducation.ResultGrade == null || previousEducation.ResultGrade.Id <= 0 || previousEducation.ITDuration == null || previousEducation.ITDuration.Id <= 0)
                //{
                //    if (previousEducation.ResultGrade == null || previousEducation.ResultGrade.Id <= 0)
                //    {
                //        throw new Exception("No ND Result Grade specified! Please specify ND Result Grade to continue.");
                //    }
                //    else if (previousEducation.ITDuration == null || previousEducation.ITDuration.Id <= 0)
                //    {
                //        throw new Exception("No IT Duration specified! Please specify IT Duration to continue.");
                //    }

                //    throw new Exception("Previous Education not set! Kindly fill Previous Education section to continue.");
                //}

                //get admission criteria olevel type
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_TYPE, bool>> otSelector = ac => ac.Admission_Criteria_Id == admissionCriteria.Id;
                List<AdmissionCriteriaForOLevelType> requiredOlevelTypes = admissionCriteriaForOLevelTypeLogic.GetModelsBy(otSelector);

                //get admission criteria olevel subject
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT, bool>> scSelector = s => s.Admission_Criteria_Id == admissionCriteria.Id && s.Is_Compulsory == true;
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT, bool>> osSelector = s => s.Admission_Criteria_Id == admissionCriteria.Id && s.Is_Compulsory == false;
                List<AdmissionCriteriaForOLevelSubject> compulsorySubjects = admissionCriteriaForOLevelSubjectLogic.GetModelsBy(scSelector);
                List<AdmissionCriteriaForOLevelSubject> otherSubjects = admissionCriteriaForOLevelSubjectLogic.GetModelsBy(osSelector);



                //APPLICANT
                //get applicant result
                Expression<Func<APPLICANT_O_LEVEL_RESULT, bool>> firstSittingOlevelSelector = f => f.Person_Id == applicant.Id && f.O_Level_Exam_Sitting_Id == 1;
                Expression<Func<APPLICANT_O_LEVEL_RESULT, bool>> secondSittingOlevelSelector = s => s.Person_Id == applicant.Id && s.O_Level_Exam_Sitting_Id == 2;
                OLevelResult firstSittingOlevelResult = oLevelResultLogic.GetModelsBy(firstSittingOlevelSelector).LastOrDefault();
                OLevelResult secondSittingOlevelResult = oLevelResultLogic.GetModelsBy(secondSittingOlevelSelector).LastOrDefault();


                //get applicant olevel subjects
                List<OLevelResultDetail> firstSittingResultDetails = null;
                List<OLevelResultDetail> secondSittingResultDetails = null;
                if (firstSittingOlevelResult != null && firstSittingOlevelResult.Id > 0)
                {
                    if (firstSittingOlevelResult.ExamYear < previousEducation.EndDate.Year)
                    {
                        Expression<Func<APPLICANT_O_LEVEL_RESULT_DETAIL, bool>> firstSittingSubjectSelector = fs => fs.Applicant_O_Level_Result_Id == firstSittingOlevelResult.Id;
                        firstSittingResultDetails = oLevelResultDetailLogic.GetModelsBy(firstSittingSubjectSelector);
                    }
                    else if (firstSittingOlevelResult.ExamYear == previousEducation.EndDate.Year)
                    {
                        rejectReason = "Student O-level was obtained the same year as National Diploma";
                        return rejectReason;
                    }
                     else
                    {
                        rejectReason = "Student O-level was obtained after National Diploma";
                        return rejectReason;
                    }
                   
                }

                if (secondSittingOlevelResult != null && secondSittingOlevelResult.Id > 0)
                {
                    if (secondSittingOlevelResult.ExamYear < previousEducation.EndDate.Year)
                    {
                    }
                    else if (secondSittingOlevelResult.ExamYear < previousEducation.EndDate.Year)
                    {
                        rejectReason = "Student second sitting  O-level was obtained the same year as National Diploma";
                        return rejectReason;
                    }
                    else
                    {
                        rejectReason = "Student second sitting O-level was obtained after National Diploma";
                        return rejectReason;
                    }
                    Expression<Func<APPLICANT_O_LEVEL_RESULT_DETAIL, bool>> secondSittingSubjectSelector = ss => ss.Applicant_O_Level_Result_Id == secondSittingOlevelResult.Id;
                    secondSittingResultDetails = oLevelResultDetailLogic.GetModelsBy(secondSittingSubjectSelector);
                }

                if ((firstSittingResultDetails == null || firstSittingResultDetails.Count <= 0) && (secondSittingResultDetails == null || secondSittingResultDetails.Count <= 0))
                {
                    return "No O-Level result found for applicant!";
                }

                string oLevelTypeRejectReason = CheckOlevelType(requiredOlevelTypes, firstSittingOlevelResult, secondSittingOlevelResult, applicantAppliedCourse);
                string requiredSubjectsRejectReason = CheckRequiredOlevelSubjects(compulsorySubjects, otherSubjects, firstSittingResultDetails, secondSittingResultDetails, applicantAppliedCourse);


               
                if (oLevelTypeRejectReason != null && requiredSubjectsRejectReason != null)
                {
                    rejectReason = oLevelTypeRejectReason + ". " + requiredSubjectsRejectReason;
                }
                else if (oLevelTypeRejectReason == null && requiredSubjectsRejectReason != null)
                {
                    rejectReason = requiredSubjectsRejectReason;
                }
                else if (oLevelTypeRejectReason != null && requiredSubjectsRejectReason == null)
                {
                    rejectReason = oLevelTypeRejectReason;
                }

                rejectReason += CheckPreviousEducation(previousEducation, programme);

                if (!string.IsNullOrWhiteSpace(rejectReason))
                {
                    rejectReason += " Therefore did not qualify for admission.";
                }

                return rejectReason;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CheckPreviousEducation(PreviousEducation previousEducation, Programme programme)
        {
            string rejectReason = null;

            try
            {
                //if (programme.Id == 4) // hnd part time
                //{
                //    if (previousEducation.ResultGrade.Id >= 4)
                //    {
                //        if (previousEducation.ITDuration.Id <= 1)
                //        {
                //            rejectReason += "Applicant with at least PASS at ND level for " + programme.Name + " programme, requires at least two years IT experience to qualify. The Applicant got " + previousEducation.ITDuration.Name;
                //            if (!string.IsNullOrWhiteSpace(rejectReason))
                //            {
                //                rejectReason = " " + rejectReason;
                //            }
                //        }
                //    }
                //}
                //else if (programme.Id == 3) //hnd full time
                //{
                //    if (previousEducation.ResultGrade.Id >= 4)
                //    {
                //        rejectReason += "LOWER CREDIT is the minimum acceptable ND Result Grade for " + programme.Name + " programme. Applicant got " + previousEducation.ResultGrade.LevelOfPass.ToUpper() + ".";
                //        if (!string.IsNullOrWhiteSpace(rejectReason))
                //        {
                //            rejectReason = " " + rejectReason;
                //        }
                //    }
                //}

                return rejectReason;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<OLevelResultDetail> GetConsolidatedApplicantOlevelResultAboveOrEqualToE8(List<OLevelResultDetail> firstSittingResultDetails, List<OLevelResultDetail> secondSittingResultDetails)
        {
            try
            {
                List<OLevelResultDetail> firstSittingOLevelResultsAboveOrEqualToE8 = GetOLevelResultsAboveOrEqualToE8(firstSittingResultDetails);
                List<OLevelResultDetail> secondSittingOLevelResultsAboveOrEqualToE8 = GetOLevelResultsAboveOrEqualToE8(secondSittingResultDetails);

                List<OLevelResultDetail> consolidatedOLevelResultsAboveOrEqualToC6 = new List<OLevelResultDetail>();
                if (firstSittingOLevelResultsAboveOrEqualToE8 != null && firstSittingOLevelResultsAboveOrEqualToE8.Count > 0)
                {
                    if (secondSittingOLevelResultsAboveOrEqualToE8 != null && secondSittingOLevelResultsAboveOrEqualToE8.Count > 0)
                    {
                        foreach (OLevelResultDetail oLevelResultDetail in firstSittingOLevelResultsAboveOrEqualToE8)
                        {
                            OLevelResultDetail oLevelResultDetailMatch = secondSittingOLevelResultsAboveOrEqualToE8.Where(ss => ss.Subject.Id == oLevelResultDetail.Subject.Id).SingleOrDefault();
                            if (oLevelResultDetailMatch != null)
                            {
                                if (oLevelResultDetailMatch.Grade.Id > oLevelResultDetail.Grade.Id)
                                {
                                    consolidatedOLevelResultsAboveOrEqualToC6.Add(oLevelResultDetail);
                                }
                                else
                                {
                                    consolidatedOLevelResultsAboveOrEqualToC6.Add(oLevelResultDetailMatch);
                                }
                            }
                            else
                            {
                                consolidatedOLevelResultsAboveOrEqualToC6.Add(oLevelResultDetail);
                            }
                        }

                        foreach (OLevelResultDetail oLevelResultDetail in secondSittingOLevelResultsAboveOrEqualToE8)
                        {
                            OLevelResultDetail oLevelResultDetailMatch = firstSittingOLevelResultsAboveOrEqualToE8.Where(ss => ss.Subject.Id == oLevelResultDetail.Subject.Id).SingleOrDefault();
                            if (oLevelResultDetailMatch == null)
                            {
                                consolidatedOLevelResultsAboveOrEqualToC6.Add(oLevelResultDetail);
                            }
                        }
                    }
                    else
                    {
                        foreach (OLevelResultDetail oLevelResultDetail in firstSittingOLevelResultsAboveOrEqualToE8)
                        {
                            if (oLevelResultDetail.Grade.Id <= 8)
                            {
                                consolidatedOLevelResultsAboveOrEqualToC6.Add(oLevelResultDetail);
                            }
                        }
                    }
                }

                return consolidatedOLevelResultsAboveOrEqualToC6;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<OLevelResultDetail> GetOLevelResultsAboveOrEqualToE8(List<OLevelResultDetail> resultDetails)
        {
            try
            {
                List<OLevelResultDetail> oLevelResultsAboveOrEqualToE8 = new List<OLevelResultDetail>();
                if (resultDetails != null && resultDetails.Count > 0)
                {
                    foreach (OLevelResultDetail firstSittingResultDetail in resultDetails)
                    {
                        if (firstSittingResultDetail.Grade.Id <= 8)
                        {
                            oLevelResultsAboveOrEqualToE8.Add(firstSittingResultDetail);
                        }
                    }
                }

                return oLevelResultsAboveOrEqualToE8;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CheckRequiredOlevelSubjects(List<AdmissionCriteriaForOLevelSubject> compulsorySubjects, List<AdmissionCriteriaForOLevelSubject> otherSubjects, List<OLevelResultDetail> firstSittingResultDetails, List<OLevelResultDetail> secondSittingResultDetails, AppliedCourse applicantAppliedCourse)
        {
            try
            {
                //check for the compulsory subjects
                if ((compulsorySubjects == null || compulsorySubjects.Count <= 0) && (otherSubjects == null || otherSubjects.Count <= 0))
                {
                    throw new Exception("Subject has not been set in Admission Criteria for " + applicantAppliedCourse.Programme.Name.ToUpper() + " in " + applicantAppliedCourse.Department.Name + "! Please contact you system administrator");
                }


                int minimumNoOfSubjectWithCreditOrAbove = compulsorySubjects[0].MainCriteria.MinimumRequiredNumberOfSubject;

                List<OLevelResultDetail> applicantConsolidatedResultsWithCreditOrAbove = GetConsolidatedApplicantOlevelResultAboveOrEqualToE8(firstSittingResultDetails, secondSittingResultDetails);
                if (applicantConsolidatedResultsWithCreditOrAbove == null || applicantConsolidatedResultsWithCreditOrAbove.Count <= 0)
                {
                    return "Required minimum number of subjects at Credit level for " + applicantAppliedCourse.Department.Name.ToUpper() + " is " + minimumNoOfSubjectWithCreditOrAbove + " and the applicant got none, ";
                }
                else if (applicantConsolidatedResultsWithCreditOrAbove.Count < minimumNoOfSubjectWithCreditOrAbove)
                {
                    return "Required minimum number of subjects at Credit level for " + applicantAppliedCourse.Department.Name.ToUpper() + " is " + minimumNoOfSubjectWithCreditOrAbove + " the applicant got only " + applicantConsolidatedResultsWithCreditOrAbove.Count + ".";
                }

                string rejectReason = null;


                //check for main compulsory subjects
                int compulsorySubjectMatchCount = 0;
                List<string> requiredSubjects = new List<string>();
                foreach (AdmissionCriteriaForOLevelSubject compulsoryOLevelSubject in compulsorySubjects)
                {
                    
                    string alternativeSubjects = null;
                    List<OLevelResultDetail> oLevelResultDetailAlternatives = new List<OLevelResultDetail>();
                    OLevelResultDetail oLevelResultDetail = applicantConsolidatedResultsWithCreditOrAbove.Where(cs => cs.Subject.Id == compulsoryOLevelSubject.Subject.Id && cs.Grade.Id <= compulsoryOLevelSubject.MinimumGrade.Id).SingleOrDefault();

                    List<AdmissionCriteriaForOLevelSubjectAlternative> compulsoryOLevelSubjectAlternatives = admissionCriteriaForOLevelSubjectAlternativeLogic.GetModelsBy(m => m.Admission_Criteria_For_O_Level_Subject_Id == compulsoryOLevelSubject.Id).ToList();


                    if (compulsoryOLevelSubjectAlternatives != null && compulsoryOLevelSubjectAlternatives.Count > 0)
                    {
                        foreach (AdmissionCriteriaForOLevelSubjectAlternative compulsoryOLevelSubjectAlternative in compulsoryOLevelSubjectAlternatives)
                        {
                            OLevelResultDetail oLevelResultDetailAlternative = applicantConsolidatedResultsWithCreditOrAbove.Where(cs => cs.Subject.Id == compulsoryOLevelSubjectAlternative.OLevelSubject.Id && cs.Grade.Id <= compulsoryOLevelSubject.MinimumGrade.Id).SingleOrDefault();
                            if (oLevelResultDetailAlternative != null)
                            {
                                oLevelResultDetailAlternatives.Add(oLevelResultDetailAlternative);
                            }
                        }
                    }

                    if (oLevelResultDetail == null && oLevelResultDetailAlternatives.Count <= 0)
                    {
                        //get all compulsory alternative subjects
                        if (compulsoryOLevelSubjectAlternatives != null && compulsoryOLevelSubjectAlternatives.Count > 0)
                        {
                            foreach (AdmissionCriteriaForOLevelSubjectAlternative alternative in compulsoryOLevelSubjectAlternatives)
                            {
                                alternativeSubjects += "/" + alternative.OLevelSubject.Name;
                            }
                        }

                        if (string.IsNullOrEmpty(alternativeSubjects))
                        {
                            requiredSubjects.Add(compulsoryOLevelSubject.Subject.Name.ToUpper());
                            //rejectReason += compulsoryOLevelSubject.Subject.Name.ToUpper() + " is a compulsory requirement for " + applicantAppliedCourse.Department.Name.ToUpper() + ", but was not obtained by the applicant. Therefore did not qualify for admission.";
                        }
                        else
                        {
                            requiredSubjects.Add(" " + compulsoryOLevelSubject.Subject.Name.ToUpper() + alternativeSubjects.ToUpper());
                            //rejectReason += "Any one of " + compulsoryOLevelSubject.Subject.Name.ToUpper() + alternativeSubjects.ToUpper() + " is a compulsory requirement for " + applicantAppliedCourse.Department.Name.ToUpper() + ", but none of them was obtained by the applicant. Therefore did not qualify for admission.";
                        }
                    }
                    else
                    {
                        compulsorySubjectMatchCount++;
                    }
                }

                if (requiredSubjects.Count > 0)
                {
                    string[] sbjectsNotGot = requiredSubjects.ToArray();
                    string missingSubjects = string.Join(", ", sbjectsNotGot);
                    rejectReason += missingSubjects + " is a compulsory requirement for " + applicantAppliedCourse.Department.Name.ToUpper() + ", but was not obtained by the applicant.";
                }


                //
                int orderSubjectMatchCount = 0;
                //List<string> otherNeededSubjects = new List<string>();
                foreach (AdmissionCriteriaForOLevelSubject otherSubject in otherSubjects)
                {
                    //string alternativeSubjects = null;
                    List<OLevelResultDetail> oLevelResultDetailAlternatives = new List<OLevelResultDetail>();
                    OLevelResultDetail oLevelResultDetail = applicantConsolidatedResultsWithCreditOrAbove.Where(cs => cs.Subject.Id == otherSubject.Subject.Id && cs.Grade.Id <= otherSubject.MinimumGrade.Id).SingleOrDefault();

                    List<AdmissionCriteriaForOLevelSubjectAlternative> otherSubjectAlternatives = admissionCriteriaForOLevelSubjectAlternativeLogic.GetModelsBy(m => m.Admission_Criteria_For_O_Level_Subject_Id == otherSubject.Id).ToList();
                    if (otherSubjectAlternatives != null && otherSubjectAlternatives.Count > 0)
                    {
                        foreach (AdmissionCriteriaForOLevelSubjectAlternative otherSubjectAlternative in otherSubjectAlternatives)
                        {
                            OLevelResultDetail oLevelResultDetailAlternative = applicantConsolidatedResultsWithCreditOrAbove.Where(cs => cs.Subject.Id == otherSubjectAlternative.OLevelSubject.Id && cs.Grade.Id <= otherSubject.MinimumGrade.Id).SingleOrDefault();
                            if (oLevelResultDetailAlternative != null)
                            {
                                //alternativeSubjects += "/" + oLevelResultDetailAlternative.Subject.Name;
                                oLevelResultDetailAlternatives.Add(oLevelResultDetailAlternative);
                            }
                        }
                    }

                    if (oLevelResultDetail != null || oLevelResultDetailAlternatives.Count > 0)
                    {
                        orderSubjectMatchCount++;
                    }
                }

                int totalSubjectsObtained = orderSubjectMatchCount + compulsorySubjectMatchCount;
                int otherSubjectCount = minimumNoOfSubjectWithCreditOrAbove - compulsorySubjects.Count;
                if (otherSubjectCount > orderSubjectMatchCount)
                {
                    if (orderSubjectMatchCount > 0)
                    {
                        rejectReason += "Applicant made a total of " + compulsorySubjectMatchCount + " compulsory subjects, and " + orderSubjectMatchCount + " other subject, which totals " + totalSubjectsObtained + " and does not meet the minimum requirement of " + minimumNoOfSubjectWithCreditOrAbove + " subjects for " + applicantAppliedCourse.Department.Name.ToUpper() + ".";
                    }
                    else
                    {
                        List<string> anyOfTheseSubjects = new List<string>();
                        foreach (AdmissionCriteriaForOLevelSubject otherOLevelSubject in otherSubjects)
                        {
                            anyOfTheseSubjects.Add(otherOLevelSubject.Subject.Name.ToUpper());
                        }

                        string[] anyOne = anyOfTheseSubjects.ToArray();
                        int totalSubjectsOutstanding = minimumNoOfSubjectWithCreditOrAbove - compulsorySubjects.Count;
                        string subjectsOutstanding = totalSubjectsOutstanding > 1 ? totalSubjectsOutstanding + " subjects" : totalSubjectsOutstanding + " subject";
                        rejectReason += "Applicant made a total of " + compulsorySubjectMatchCount + " compulsory subjects, but did not obtain any other " + subjectsOutstanding + " from (" + string.Join(", ", anyOne) + ") at credit level to complement. Which does not meet the minimum requirement of " + minimumNoOfSubjectWithCreditOrAbove + " subjects for " + applicantAppliedCourse.Department.Name.ToUpper() + ".";
                    }
                }

                return rejectReason;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        private string CheckOlevelType(List<AdmissionCriteriaForOLevelType> requiredOlevelTypes, OLevelResult firstSittingOlevelResult, OLevelResult secondSittingOlevelResult, AppliedCourse applicantAppliedCourse)
        {
            try
            {
                string rejectReason = null;
                if (requiredOlevelTypes == null || requiredOlevelTypes.Count <= 0)
                {
                    throw new Exception("O-Level Type has not been set in Admission Criteria for " + applicantAppliedCourse.Programme.Name + " in " + applicantAppliedCourse.Department.Name + "! Please contact your system administrator.");
                }

                string applicantOlevelTypes = null;
                if (firstSittingOlevelResult != null && firstSittingOlevelResult.Id > 0)
                {
                    applicantOlevelTypes += firstSittingOlevelResult.Type.Name;

                    AdmissionCriteriaForOLevelType secondSittingAdmissionCriteriaOLevelType = null;
                    AdmissionCriteriaForOLevelType firstSittingAdmissionCriteriaOLevelType = requiredOlevelTypes.Where(ac => ac.OLevelType.Id == firstSittingOlevelResult.Type.Id).SingleOrDefault();
                    if (firstSittingAdmissionCriteriaOLevelType == null)
                    {
                        if (secondSittingOlevelResult != null && secondSittingOlevelResult.Id > 0)
                        {
                            applicantOlevelTypes += " & " + secondSittingOlevelResult.Type.Name;
                            secondSittingAdmissionCriteriaOLevelType = requiredOlevelTypes.Where(ac => ac.OLevelType.Id == secondSittingOlevelResult.Type.Id).SingleOrDefault();
                        }
                    }

                    if (firstSittingAdmissionCriteriaOLevelType == null && secondSittingAdmissionCriteriaOLevelType == null)
                    {
                        string validOlevelTypes = GetStringOfValidOlevelTypes(requiredOlevelTypes);
                        rejectReason = "'" + applicantOlevelTypes + "' O-Level Type obtained by the applicant does not meet the O-Level Type (" + validOlevelTypes + ") requirement for " + applicantAppliedCourse.Department.Name.ToUpper();
                        
                    }
                }

                return rejectReason;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetStringOfValidOlevelTypes(List<AdmissionCriteriaForOLevelType> admissionCriteriaOLevelTypes)
        {
            try
            {
                string validOlevelTypes = null;
                if (admissionCriteriaOLevelTypes != null && admissionCriteriaOLevelTypes.Count > 0)
                {
                    string[] oLevelTypes = GetDistinctOLevelTypesFrom(admissionCriteriaOLevelTypes);
                    validOlevelTypes = string.Join(", ", oLevelTypes);

                }

                return validOlevelTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string[] GetDistinctOLevelTypesFrom(List<AdmissionCriteriaForOLevelType> admissionCriteriaOLevelTypes)
        {
            try
            {
                string[] oLevelTypes = null;
                if (admissionCriteriaOLevelTypes != null && admissionCriteriaOLevelTypes.Count > 0)
                {
                    oLevelTypes = admissionCriteriaOLevelTypes.Select(o => o.OLevelType.ShortName).Distinct().ToArray();
                }

                return oLevelTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int GetDistinctOLevelTypeCountFrom(List<AdmissionCriteriaForOLevelType> admissionCriteriaOLevelTypes)
        {
            try
            {
                string[] oLevelTypes = GetDistinctOLevelTypesFrom(admissionCriteriaOLevelTypes);
                if (oLevelTypes != null && oLevelTypes.Length > 0)
                {
                    return oLevelTypes.Length;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




    }

}
