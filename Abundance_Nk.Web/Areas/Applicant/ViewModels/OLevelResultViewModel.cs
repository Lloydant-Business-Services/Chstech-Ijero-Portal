using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Models;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class OLevelResultViewModel
    {
        private OLevelTypeLogic oLevelTypeLogic;
        private OLevelGradeLogic oLevelGradeLogic;
        private OLevelSubjectLogic oLevelSubjectLogic;

        public OLevelResultViewModel()
        {
            FirstSittingOLevelResult = new OLevelResult();
            FirstSittingOLevelResult.Type = new OLevelType();
            SecondSittingOLevelResult = new OLevelResult();
            SecondSittingOLevelResult.Type = new OLevelType();

            oLevelTypeLogic = new OLevelTypeLogic();
            oLevelGradeLogic = new OLevelGradeLogic();
            oLevelSubjectLogic = new OLevelSubjectLogic();

            InitialiseOLevelResult();
            PopulateAllDropDowns();
        }

        public List<Value> ExamYears { get; set; }
        public List<OLevelType> OLevelTypes { get; set; }
        public List<OLevelGrade> OLevelGrades { get; set; }
        public List<OLevelSubject> OLevelSubjects { get; set; }

        public OLevelResult ApplicantFirstSittingOLevelResult { get; set; }
        public OLevelResult ApplicantSecondSittingOLevelResult { get; set; }
        public List<OLevelResultDetail> ApplicantFirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> ApplicantSecondSittingOLevelResultDetails { get; set; }

        public OLevelResult FirstSittingOLevelResult { get; set; }
        public OLevelResult SecondSittingOLevelResult { get; set; }
        public OLevelResultDetail FirstSittingOLevelResultDetail { get; set; }
        public OLevelResultDetail SecondSittingOLevelResultDetail { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetails { get; set; }

        public List<SelectListItem> ExamYearSelectList { get; set; }
        public List<SelectListItem> OLevelTypeSelectList { get; set; }
        public List<SelectListItem> OLevelGradeSelectList { get; set; }
        public List<SelectListItem> OLevelSubjectSelectList { get; set; }

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

                FirstSittingOLevelResultDetails = new List<OLevelResultDetail>();
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail1);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail2);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail3);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail4);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail5);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail6);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail7);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail8);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail9);

                SecondSittingOLevelResultDetails = new List<OLevelResultDetail>();
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail11);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail22);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail33);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail44);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail55);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail66);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail77);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail88);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail99);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PopulateAllDropDowns()
        {
            const int START_YEAR = 1920;

            try
            {
                ExamYears = Utility.CreateYearListFrom(START_YEAR); 
                OLevelTypes = oLevelTypeLogic.GetAll();
                OLevelGrades = oLevelGradeLogic.GetAll();
                OLevelSubjects = oLevelSubjectLogic.GetAll();
                               
                ExamYearSelectList = Utility.PopulateExamYearSelectListItem(START_YEAR);
                OLevelTypeSelectList = Utility.PopulateOLevelTypeSelectListItem();
                OLevelGradeSelectList = Utility.PopulateOLevelGradeSelectListItem();
                OLevelSubjectSelectList = Utility.PopulateOLevelSubjectSelectListItem();
              
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GetOLevelResultBy(long applicationFormId)
        {
            try
            {
                ApplicationForm applicationForm = GetApplicationFormBy(applicationFormId);
                if (applicationForm != null && applicationForm.Id > 0)
                {
                    OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                    OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();

                    ApplicantFirstSittingOLevelResult = oLevelResultLogic.GetModelsBy(p => p.Person_Id == applicationForm.Person.Id && p.O_Level_Exam_Sitting_Id == 1 && p.Application_Form_Id != null).LastOrDefault();
                    ApplicantSecondSittingOLevelResult = oLevelResultLogic.GetModelsBy(p => p.Person_Id == applicationForm.Person.Id && p.O_Level_Exam_Sitting_Id == 2 && p.Application_Form_Id != null).LastOrDefault();

                    if (ApplicantFirstSittingOLevelResult != null && ApplicantFirstSittingOLevelResult.Id > 0)
                    {
                        ApplicantFirstSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == ApplicantFirstSittingOLevelResult.Id);
                    }
                    if (ApplicantSecondSittingOLevelResult != null && ApplicantSecondSittingOLevelResult.Id > 0)
                    {
                        ApplicantSecondSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == ApplicantSecondSittingOLevelResult.Id);
                    }

                    SetApplicantFirstSittingOLevelResult(ApplicantFirstSittingOLevelResult, ApplicantFirstSittingOLevelResultDetails);
                    SetApplicantSecondSittingOLevelResult(ApplicantSecondSittingOLevelResult, ApplicantSecondSittingOLevelResultDetails);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationForm GetApplicationFormBy(long applicationFormId)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                return applicationFormLogic.GetBy(applicationFormId);
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

                    FirstSittingOLevelResult.Id = oLevelResult.Id;
                    FirstSittingOLevelResult.ExamNumber = oLevelResult.ExamNumber;
                    FirstSittingOLevelResult.ExamYear = oLevelResult.ExamYear;

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        for (int i = 0; i < oLevelResultDetails.Count; i++)
                        {
                            FirstSittingOLevelResultDetails[i].Subject = oLevelResultDetails[i].Subject;
                            FirstSittingOLevelResultDetails[i].Grade = oLevelResultDetails[i].Grade;
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

                    SecondSittingOLevelResult.Id = oLevelResult.Id;
                    SecondSittingOLevelResult.ExamNumber = oLevelResult.ExamNumber;
                    SecondSittingOLevelResult.ExamYear = oLevelResult.ExamYear;

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        for (int i = 0; i < oLevelResultDetails.Count; i++)
                        {
                            if (oLevelResultDetails[i].Subject != null)
                            {
                                SecondSittingOLevelResultDetails[i].Subject = oLevelResultDetails[i].Subject;
                            }
                            else
                            {
                                SecondSittingOLevelResultDetails[i].Subject = new OLevelSubject();
                            }
                            if (oLevelResultDetails[i].Grade != null)
                            {
                                SecondSittingOLevelResultDetails[i].Grade = oLevelResultDetails[i].Grade;
                            }
                            else
                            {
                                SecondSittingOLevelResultDetails[i].Grade = new OLevelGrade();
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



    }
}