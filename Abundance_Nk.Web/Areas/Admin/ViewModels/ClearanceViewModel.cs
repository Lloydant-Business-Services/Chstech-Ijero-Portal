using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ClearanceViewModel
    {
        public ClearanceViewModel()
        {
            applicationFormList = new List<ApplicationForm>();
            appliedCourseList = new List<AppliedCourse>();
            personList = new List<Person>();

            FirstSittingOLevelResult = new OLevelResult();
            FirstSittingOLevelResult.Type = new OLevelType();

            SecondSittingOLevelResult = new OLevelResult();
            SecondSittingOLevelResult.Type = new OLevelType();
            FirstSittingOLevelResultDetailsList = new List<OLevelResultDetail>();
            SecondSittingOLevelResultDetailsList = new List<OLevelResultDetail>();

            AdmissionCriteriaLogic AdmissionCriteriaLogic = new Business.AdmissionCriteriaLogic();
            admissionCriteriaList = AdmissionCriteriaLogic.GetAll();

            InitialiseOLevelResult();
            //InitialiseOLevelSubjects();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            OLevelSubjectSelectList = Utility.PopulateOLevelSubjectSelectListItem();
            OLevelGradeSelectList = Utility.PopulateOLevelGradeSelectListItem();
            OlevelTypeSelectListItem = Utility.PopulateOLevelTypeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
        }
        public string ApplicationNumber { get; set; }
        public List<ApplicationForm> applicationFormList { get; set; }
        public List<AppliedCourse> appliedCourseList { get; set; }
        public List<Person> personList { get; set; }
        public List<OLevelType> OLevelTypes { get; set; }
        public List<OLevelGrade> OLevelGrades { get; set; }
        public List<OLevelSubject> OLevelSubjects { get; set; }
        public List<OLevelSubject> OLevelSubjectsAlternatives { get; set; }
        public List<SelectListItem> OLevelSubjectSelectList { get; set; }
        public List<SelectListItem> OLevelGradeSelectList { get; set; }
        public OLevelResult FirstSittingOLevelResult { get; set; }
        public OLevelResult SecondSittingOLevelResult { get; set; }
        public OLevelResultDetail FirstSittingOLevelResultDetail { get; set; }
        public OLevelResultDetail SecondSittingOLevelResultDetail { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetailsList { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetailsList { get; set; }
        public OLevelResult ApplicantFirstSittingOLevelResult { get; set; }
        public OLevelResult ApplicantSecondSittingOLevelResult { get; set; }
        public List<OLevelResultDetail> ApplicantFirstSittingOLevelResultDetailsList { get; set; }
        public List<OLevelResultDetail> ApplicantSecondSittingOLevelResultDetailsList { get; set; }
        public List<AdmissionCriteria> admissionCriteriaList { get; set; }
        public List<AdmissionCriteriaForOLevelSubject> admissionCriteriaForOLevelSubject { get; set; }
        public List<AdmissionCriteriaForOLevelSubjectAlternative> admissionCriteriaForOLevelSubjectAlternative { get; set; }
        public List<AdmissionCriteriaForOLevelType> admissionCriteriaForOLevelType { get; set; }

        public AdmissionCriteria admissionCriteria = new AdmissionCriteria();
        public ApplicationForm applicationForm { get; set; }
        public AppliedCourse appliedCourse { get; set; }
        public PreviousEducation PreviousEducation { get; set; }
        public Person person { get; set; }
        public User ClearanceOfficer { get; set; }
        public List<SelectListItem> OlevelTypeSelectListItem { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public void InitialiseOLevelResult()
        {
            try
            {
                List<OLevelResultDetail> oLevelResultDetails = new List<OLevelResultDetail>();
                OLevelResultDetail oLevelResultDetail1 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail2 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail3 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail4 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail5 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail6 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail7 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail8 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail9 = new OLevelResultDetail();

                OLevelResultDetail oLevelResultDetail11 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail22 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail33 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail44 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail55 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail66 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail77 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail88 = new OLevelResultDetail();
                OLevelResultDetail oLevelResultDetail99 = new OLevelResultDetail();

                FirstSittingOLevelResultDetailsList = new List<OLevelResultDetail>();
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail1);
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail2);
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail3);
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail4);
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail5);
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail6);
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail7);
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail8);
                FirstSittingOLevelResultDetailsList.Add(oLevelResultDetail9);

                SecondSittingOLevelResultDetailsList = new List<OLevelResultDetail>();
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail11);
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail22);
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail33);
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail44);
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail55);
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail66);
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail77);
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail88);
                SecondSittingOLevelResultDetailsList.Add(oLevelResultDetail99);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void LoadApplicantResult(Person person)
        {
            try
            {
              
                    OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                    OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
                    

                    ApplicantFirstSittingOLevelResult = oLevelResultLogic.GetModelBy(p => p.Person_Id == person.Id && p.O_Level_Exam_Sitting_Id == 1);
                    ApplicantSecondSittingOLevelResult = oLevelResultLogic.GetModelBy(p => p.Person_Id == person.Id && p.O_Level_Exam_Sitting_Id == 2);
                   
                    if (ApplicantFirstSittingOLevelResult != null && ApplicantFirstSittingOLevelResult.Id > 0)
                    {
                        ApplicantFirstSittingOLevelResultDetailsList = oLevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == ApplicantFirstSittingOLevelResult.Id);
                    }

                    if (ApplicantSecondSittingOLevelResult != null && ApplicantSecondSittingOLevelResult.Id > 0)
                    {
                        ApplicantSecondSittingOLevelResultDetailsList = oLevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == ApplicantSecondSittingOLevelResult.Id);
                    }

                  
                    SetApplicantFirstSittingOLevelResult(ApplicantFirstSittingOLevelResult, ApplicantFirstSittingOLevelResultDetailsList);
                    SetApplicantSecondSittingOLevelResult(ApplicantSecondSittingOLevelResult, ApplicantSecondSittingOLevelResultDetailsList);
                   
            }
            catch (Exception)
            {
                throw;
            }
        }
      
        public void SetApplicantFirstSittingOLevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                if (oLevelResult != null && oLevelResult.Id > 0)
                {
                    if (oLevelResult.Type != null)
                    {
                        FirstSittingOLevelResult.Type = oLevelResult.Type;
                    }
                    else
                    {
                        FirstSittingOLevelResult.Type = new OLevelType();
                    }

                    FirstSittingOLevelResult.ExamNumber = oLevelResult.ExamNumber;
                    FirstSittingOLevelResult.ExamYear = oLevelResult.ExamYear;

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        for (int i = 0; i < oLevelResultDetails.Count; i++)
                        {
                            FirstSittingOLevelResultDetailsList[i].Subject = oLevelResultDetails[i].Subject;
                            FirstSittingOLevelResultDetailsList[i].Grade = oLevelResultDetails[i].Grade;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetApplicantSecondSittingOLevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                if (oLevelResult != null && oLevelResult.Id > 0)
                {
                    if (oLevelResult.Type != null)
                    {
                        SecondSittingOLevelResult.Type = oLevelResult.Type;
                    }
                    else
                    {
                        SecondSittingOLevelResult.Type = new OLevelType();
                    }

                    SecondSittingOLevelResult.ExamNumber = oLevelResult.ExamNumber;
                    SecondSittingOLevelResult.ExamYear = oLevelResult.ExamYear;

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        for (int i = 0; i < oLevelResultDetails.Count; i++)
                        {
                            if (oLevelResultDetails[i].Subject != null)
                            {
                                SecondSittingOLevelResultDetailsList[i].Subject = oLevelResultDetails[i].Subject;
                            }
                            else
                            {
                                SecondSittingOLevelResultDetailsList[i].Subject = new OLevelSubject();
                            }
                            if (oLevelResultDetails[i].Grade != null)
                            {
                                SecondSittingOLevelResultDetailsList[i].Grade = oLevelResultDetails[i].Grade;
                            }
                            else
                            {
                                SecondSittingOLevelResultDetailsList[i].Grade = new OLevelGrade();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetails { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public Session Session { get; set; }
        public List<AdmissionList> AdmissionLists { get; set; }
    }
}