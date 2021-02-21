using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Models
{
    public class StudentFormViewModelBase
    {
        private AppliedCourseLogic appliedCourseLogic;

        public StudentFormViewModelBase()
        {
            appliedCourseLogic = new AppliedCourseLogic();

            Sponsor = new Sponsor();
            Sponsor.Relationship = new Relationship();

            AppliedCourse = new AppliedCourse();
            AppliedCourse.Programme = new Programme();
            AppliedCourse.Department = new Department();
            AppliedCourse.Department.Faculty = new Faculty();

            FirstSittingOLevelResult = new OLevelResult();
            FirstSittingOLevelResult.Type = new OLevelType();

            SecondSittingOLevelResult = new OLevelResult();
            SecondSittingOLevelResult.Type = new OLevelType();

            ApplicantPreviousEducation = new PreviousEducation();
            ApplicantPreviousEducation.ResultGrade = new ResultGrade();
            ApplicantPreviousEducation.Qualification = new EducationalQualification();
            ApplicantPreviousEducation.ITDuration = new ITDuration();
            ApplicantPreviousEducation.StartYear = new Value();
            ApplicantPreviousEducation.StartMonth = new Value();
            ApplicantPreviousEducation.StartDay = new Value();
            ApplicantPreviousEducation.EndYear = new Value();
            ApplicantPreviousEducation.EndMonth = new Value();
            ApplicantPreviousEducation.EndDay = new Value();

            ApplicantJambDetail = new ApplicantJambDetail();
            ApplicantJambDetail.InstitutionChoice = new InstitutionChoice();

            //Person = new Person();
            //Person.LocalGovernment = new LocalGovernment();
            //Person.Religion = new Religion();
            //Person.State = new State();
            //Person.Sex = new Sex();
            //Person.YearOfBirth = new Value();
            //Person.MonthOfBirth = new Value();
            //Person.DayOfBirth = new Value();

            Applicant = new Abundance_Nk.Model.Model.Applicant();
            Applicant.Person = new Person();
            Applicant.ApplicationForm = new ApplicationForm();
            Applicant.Ability = new Ability();



            Person = new Person();
            Person.LocalGovernment = new LocalGovernment();
            Person.Religion = new Religion();
            Person.State = new State();
            Person.Sex = new Sex();
            Person.YearOfBirth = new Value();
            Person.MonthOfBirth = new Value();
            Person.DayOfBirth = new Value();

            Student = new Model.Model.Student();
            Student.Type = new StudentType();
            Student.Category = new StudentCategory();
            Student.Status = new StudentStatus();
            Student.Title = new Title();
            Student.MaritalStatus = new MaritalStatus();
            Student.BloodGroup = new BloodGroup();
            Student.Genotype = new Genotype();

            StudentNdResult = new StudentNdResult();
            StudentNdResult.DayAwarded = new Value();
            StudentNdResult.MonthAwarded = new Value();
            StudentNdResult.YearAwarded = new Value();

            StudentAcademicInformation = new StudentAcademicInformation();
            StudentAcademicInformation.ModeOfEntry = new ModeOfEntry();
            StudentAcademicInformation.ModeOfStudy = new ModeOfStudy();
            StudentAcademicInformation.Level = new Level();

            StudentFinanceInformation = new StudentFinanceInformation();
            StudentFinanceInformation.Mode = new ModeOfFinance();

            StudentSponsor = new StudentSponsor();
            StudentSponsor.Relationship = new Relationship();

            StudentEmploymentInformation = new StudentEmploymentInformation();
            StudentEmploymentInformation.Student = new Abundance_Nk.Model.Model.Student();
            StudentEmploymentInformation.StartYear = new Value();
            StudentEmploymentInformation.StartMonth = new Value();
            StudentEmploymentInformation.StartDay = new Value();
            StudentEmploymentInformation.EndYear = new Value();
            StudentEmploymentInformation.EndMonth = new Value();
            StudentEmploymentInformation.EndDay = new Value();

            InitialiseOLevelResult();
        }

        public Person Person { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public Model.Model.Applicant Applicant { get; set; }
        public Sponsor ApplicantSponsor { get; set; }
        public OLevelResult ApplicantFirstSittingOLevelResult { get; set; }
        public OLevelResult ApplicantSecondSittingOLevelResult { get; set; }
        public PreviousEducation ApplicantPreviousEducation { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public List<OLevelResultDetail> ApplicantFirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> ApplicantSecondSittingOLevelResultDetails { get; set; }

        public Student Student { get; set; }
        public Sponsor Sponsor { get; set; }
        public StudentAcademicInformation StudentAcademicInformation { get; set; }
        public StudentSponsor StudentSponsor { get; set; }
        public StudentFinanceInformation StudentFinanceInformation { get; set; }
        public StudentNdResult StudentNdResult { get; set; }
        public StudentEmploymentInformation StudentEmploymentInformation { get; set; }
        public OLevelResult FirstSittingOLevelResult { get; set; }
        public OLevelResult SecondSittingOLevelResult { get; set; }
        public OLevelResultDetail FirstSittingOLevelResultDetail { get; set; }
        public OLevelResultDetail SecondSittingOLevelResultDetail { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetails { get; set; }

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

        public void LoadApplicantionFormBy(long id)
        {
            try
            {
                ApplicationForm = GetApplicationFormBy(id);
                if (ApplicationForm != null && ApplicationForm.Id > 0)
                {
                    PersonLogic personLogic = new PersonLogic();
                    SponsorLogic sponsorLogic = new SponsorLogic();
                    OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                    OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
                    PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();
                    ApplicantJambDetailLogic studentJambDetailLogic = new ApplicantJambDetailLogic();
                    ApplicantLogic applicantLogic = new ApplicantLogic();

                    Person person = ApplicationForm.Person;
                    Person = personLogic.GetModelBy(p => p.Person_Id == person.Id);
                    Applicant = applicantLogic.GetModelBy(p => p.Person_Id == person.Id);
                    ApplicantSponsor = sponsorLogic.GetModelBy(p => p.Person_Id == person.Id);
                    ApplicantFirstSittingOLevelResult = oLevelResultLogic.GetModelBy(p => p.Person_Id == person.Id && p.O_Level_Exam_Sitting_Id == 1 && p.Application_Form_Id!= null);
                    ApplicantSecondSittingOLevelResult = oLevelResultLogic.GetModelBy(p => p.Person_Id == person.Id && p.O_Level_Exam_Sitting_Id == 2 && p.Application_Form_Id != null);
                    ApplicantPreviousEducation = previousEducationLogic.GetModelBy(p => p.Person_Id == person.Id);
                    ApplicantJambDetail = studentJambDetailLogic.GetModelBy(p => p.Person_Id == person.Id);
                    AppliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);

                    if (ApplicantFirstSittingOLevelResult != null && ApplicantFirstSittingOLevelResult.Id > 0)
                    {
                        ApplicantFirstSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == ApplicantFirstSittingOLevelResult.Id);
                    }

                    if (ApplicantSecondSittingOLevelResult != null && ApplicantSecondSittingOLevelResult.Id > 0)
                    {
                        ApplicantSecondSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == ApplicantSecondSittingOLevelResult.Id);
                    }

                    SetApplicant(Applicant);
                    SetApplicantBioData(Person);
                    SetApplicantSponsor(ApplicantSponsor);
                    SetApplicantFirstSittingOLevelResult(ApplicantFirstSittingOLevelResult, ApplicantFirstSittingOLevelResultDetails);
                    SetApplicantSecondSittingOLevelResult(ApplicantSecondSittingOLevelResult, ApplicantSecondSittingOLevelResultDetails);
                    SetApplicantPreviousEducation(ApplicantPreviousEducation);
                    SetAppliedCourse(AppliedCourse);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void LoadStudentInformationFormBy(long personId)
        {
            try
            {
                if (personId > 0)
                {

                    StudentSponsorLogic sponsorLogic = new StudentSponsorLogic();
                    StudentAcademicInformationLogic academicInformationLogic = new StudentAcademicInformationLogic();
                    StudentFinanceInformationLogic financeInformationLogic = new StudentFinanceInformationLogic();
                    StudentEmploymentInformationLogic employmentInformationLogic = new StudentEmploymentInformationLogic();
                    StudentNdResultLogic ndResultLogic = new StudentNdResultLogic();


                    Student = GetStudentBy(personId);
                    StudentSponsor = sponsorLogic.GetModelBy(p => p.Person_Id == personId);
                    StudentAcademicInformation = academicInformationLogic.GetModelBy(p => p.Person_Id == personId);
                    StudentFinanceInformation = financeInformationLogic.GetModelBy(p => p.Person_Id == personId);
                    StudentEmploymentInformation = employmentInformationLogic.GetModelBy(p => p.Person_Id == personId);
                    StudentNdResult = ndResultLogic.GetModelBy(p => p.Person_Id == personId);


                    SetStudent(Student);
                    SetStudentSponsor(StudentSponsor);
                    SetStudentAcademicInformation(StudentAcademicInformation);
                    SetStudentFinanceInformation(StudentFinanceInformation);
                    SetStudentEmploymentInformation(StudentEmploymentInformation);
                    SetStudentEmploymentInformation(StudentEmploymentInformation);
                    SetStudentNdResult(StudentNdResult);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Model.Model.Student GetStudentBy(long personId)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                return studentLogic.GetModelBy(p => p.Person_Id == personId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationForm GetApplicationFormBy(long id)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                return applicationFormLogic.GetModelBy(a => a.Application_Form_Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetApplicantPreviousEducation(PreviousEducation previousEducation)
        {
            try
            {
                if (previousEducation != null && previousEducation.Id > 0)
                {
                    ApplicantPreviousEducation.SchoolName = previousEducation.SchoolName;
                    ApplicantPreviousEducation.Course = previousEducation.Course;

                    if (previousEducation.Person != null && previousEducation.Person.Id > 0)
                    {
                        ApplicantPreviousEducation.Person = previousEducation.Person;
                    }
                    else
                    {
                        ApplicantPreviousEducation.Person = new Person();
                    }

                    if (previousEducation.Qualification != null && previousEducation.Qualification.Id > 0)
                    {
                        ApplicantPreviousEducation.Qualification = previousEducation.Qualification;
                    }
                    else
                    {
                        ApplicantPreviousEducation.Qualification = new EducationalQualification();
                    }

                    if (previousEducation.ResultGrade != null && previousEducation.ResultGrade.Id > 0)
                    {
                        ApplicantPreviousEducation.ResultGrade = previousEducation.ResultGrade;
                    }
                    else
                    {
                        ApplicantPreviousEducation.ResultGrade = new ResultGrade();
                    }

                    if (previousEducation.ITDuration != null && previousEducation.ITDuration.Id > 0)
                    {
                        ApplicantPreviousEducation.ITDuration = previousEducation.ITDuration;
                    }
                    else
                    {
                        ApplicantPreviousEducation.ITDuration = new ITDuration();
                    }

                    if (previousEducation.StartDate != null)
                    {
                        ApplicantPreviousEducation.StartDay = previousEducation.StartDay;
                        ApplicantPreviousEducation.StartMonth = previousEducation.StartMonth;
                        ApplicantPreviousEducation.StartYear = previousEducation.StartYear;
                    }
                    else
                    {
                        ApplicantPreviousEducation.StartDay = new Value();
                        ApplicantPreviousEducation.StartMonth = new Value();
                        ApplicantPreviousEducation.StartYear = new Value();
                    }

                    if (previousEducation.EndDate != null)
                    {
                        ApplicantPreviousEducation.EndDay = previousEducation.EndDay;
                        ApplicantPreviousEducation.EndMonth = previousEducation.EndMonth;
                        ApplicantPreviousEducation.EndYear = previousEducation.EndYear;
                    }
                    else
                    {
                        ApplicantPreviousEducation.EndDay = new Value();
                        ApplicantPreviousEducation.EndMonth = new Value();
                        ApplicantPreviousEducation.EndYear = new Value();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetApplicantBioData(Person person)
        {
            try
            {
                if (person != null && person.Id > 0)
                {
                    Person.DateOfBirth = person.DateOfBirth;
                    Person.HomeTown = person.HomeTown;
                    Person.HomeAddress = person.HomeAddress;
                    Person.ImageFileUrl = person.ImageFileUrl;

                    if (person.State != null && !string.IsNullOrEmpty(person.State.Id))
                    {
                        Person.State = person.State;
                    }
                    else
                    {
                        Person.State = new State();
                    }

                    if (person.Sex != null && person.Sex.Id > 0)
                    {
                        Person.Sex = person.Sex;
                    }
                    else
                    {
                        Person.Sex = new Sex();
                    }

                    if (person.LocalGovernment != null && person.LocalGovernment.Id > 0)
                    {
                        Person.LocalGovernment = person.LocalGovernment;
                    }
                    else
                    {
                        Person.LocalGovernment = new LocalGovernment();
                    }

                    if (person.Religion != null && person.Religion.Id > 0)
                    {
                        Person.Religion = person.Religion;
                    }
                    else
                    {
                        Person.Religion = new Religion();
                    }

                    if (person.DateOfBirth.HasValue)
                    {
                        Person.DayOfBirth = person.DayOfBirth;
                        Person.MonthOfBirth = person.MonthOfBirth;
                        Person.YearOfBirth = person.YearOfBirth;
                    }
                    else
                    {
                        Person.DayOfBirth = new Value();
                        Person.MonthOfBirth = new Value();
                        Person.YearOfBirth = new Value();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetApplicant(Model.Model.Applicant applicant)
        {
            try
            {
                if (applicant != null)
                {
                    Applicant.ExtraCurricullarActivities = applicant.ExtraCurricullarActivities;
                    Applicant.OtherAbility = applicant.OtherAbility;

                    if (applicant.Ability != null)
                    {
                        Applicant.Ability = applicant.Ability;
                    }
                    else
                    {
                        Applicant.Ability = new Ability();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetStudent(Model.Model.Student student)
        {
            try
            {
                if (student != null)
                {
                    Student.MatricNumber = student.MatricNumber;
                    Student.SchoolContactAddress = student.SchoolContactAddress;

                    if (student.ApplicationForm != null)
                    {
                        Student.ApplicationForm = student.ApplicationForm;
                    }
                    else
                    {
                        Student.ApplicationForm = new ApplicationForm();
                    }

                    if (student.Category != null)
                    {
                        Student.Category = student.Category;
                    }
                    else
                    {
                        Student.Category = new StudentCategory();
                    }

                    if (student.Type != null)
                    {
                        Student.Type = student.Type;
                    }
                    else
                    {
                        Student.Type = new StudentType();
                    }

                    if (student.Status != null)
                    {
                        Student.Status = student.Status;
                    }
                    else
                    {
                        Student.Status = new StudentStatus();
                    }

                    if (student.Title != null)
                    {
                        Student.Title = student.Title;
                    }
                    else
                    {
                        Student.Title = new Title();
                    }

                    if (student.MaritalStatus != null)
                    {
                        Student.MaritalStatus = student.MaritalStatus;
                    }
                    else
                    {
                        Student.MaritalStatus = new MaritalStatus();
                    }

                    if (student.BloodGroup != null)
                    {
                        Student.BloodGroup = student.BloodGroup;
                    }
                    else
                    {
                        Student.BloodGroup = new BloodGroup();
                    }

                    if (student.Genotype != null)
                    {
                        Student.Genotype = student.Genotype;
                    }
                    else
                    {
                        Student.Genotype = new Genotype();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetAppliedCourse(AppliedCourse appliedCourse)
        {
            try
            {
                if (appliedCourse != null)
                {


                    AppliedCourse.Option = appliedCourse.Option;

                    if (appliedCourse.Department != null)
                    {
                        AppliedCourse.Department = appliedCourse.Department;
                    }
                    else
                    {
                        AppliedCourse.Department = new Department();
                    }

                    if (appliedCourse.Programme != null)
                    {
                        AppliedCourse.Programme = appliedCourse.Programme;
                    }
                    else
                    {
                        AppliedCourse.Programme = new Programme();
                    }

                    if (appliedCourse.Option != null)
                    {
                        AppliedCourse.Option = appliedCourse.Option;
                    }
                    else
                    {
                        AppliedCourse.Option = new DepartmentOption();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetApplicantSponsor(Sponsor sponsor)
        {
            try
            {
                if (sponsor != null)
                {
                    Sponsor.Name = sponsor.Name;
                    Sponsor.ContactAddress = sponsor.ContactAddress;
                    Sponsor.MobilePhone = sponsor.MobilePhone;

                    if (sponsor.Relationship != null)
                    {
                        Sponsor.Relationship = sponsor.Relationship;
                    }
                    else
                    {
                        Sponsor.Relationship = new Relationship();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetStudentSponsor(StudentSponsor sponsor)
        {
            try
            {
                if (sponsor != null)
                {
                    StudentSponsor.Name = sponsor.Name;
                    StudentSponsor.ContactAddress = sponsor.ContactAddress;
                    StudentSponsor.MobilePhone = sponsor.MobilePhone;
                    StudentSponsor.Email = sponsor.Email;

                    if (sponsor.Relationship != null)
                    {
                        StudentSponsor.Relationship = sponsor.Relationship;
                    }
                    else
                    {
                        StudentSponsor.Relationship = new Relationship();
                    }
                }
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

        public void SetStudentAcademicInformation(StudentAcademicInformation academicInformation)
        {
            try
            {
                if (academicInformation != null)
                {
                    StudentAcademicInformation.YearOfAdmission = academicInformation.YearOfAdmission;
                    StudentAcademicInformation.YearOfGraduation = academicInformation.YearOfGraduation;

                    if (academicInformation.ModeOfEntry != null)
                    {
                        StudentAcademicInformation.ModeOfEntry = academicInformation.ModeOfEntry;
                    }
                    else
                    {
                        StudentAcademicInformation.ModeOfEntry = new ModeOfEntry();
                    }

                    if (academicInformation.ModeOfStudy != null)
                    {
                        StudentAcademicInformation.ModeOfStudy = academicInformation.ModeOfStudy;
                    }
                    else
                    {
                        StudentAcademicInformation.ModeOfStudy = new ModeOfStudy();
                    }

                    if (academicInformation.Level != null)
                    {
                        StudentAcademicInformation.Level = academicInformation.Level;
                    }
                    else
                    {
                        StudentAcademicInformation.Level = new Level();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetStudentFinanceInformation(StudentFinanceInformation financeInformation)
        {
            try
            {
                if (financeInformation != null)
                {
                    StudentFinanceInformation.ScholarshipTitle = financeInformation.ScholarshipTitle;

                    if (financeInformation.Mode != null)
                    {
                        StudentFinanceInformation.Mode = financeInformation.Mode;
                    }
                    else
                    {
                        StudentFinanceInformation.Mode = new ModeOfFinance();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetStudentEmploymentInformation(StudentEmploymentInformation employmentInformation)
        {
            try
            {
                if (employmentInformation != null && employmentInformation.Id > 0)
                {
                    StudentEmploymentInformation.PlaceOfLastEmployment = employmentInformation.PlaceOfLastEmployment;

                    if (employmentInformation.Student != null)
                    {
                        StudentEmploymentInformation.Student = employmentInformation.Student;
                    }
                    else
                    {
                        StudentEmploymentInformation.Student = new Model.Model.Student();
                    }

                    if (employmentInformation.StartDate != null)
                    {
                        StudentEmploymentInformation.StartDay = employmentInformation.StartDay;
                        StudentEmploymentInformation.StartMonth = employmentInformation.StartMonth;
                        StudentEmploymentInformation.StartYear = employmentInformation.StartYear;
                    }
                    else
                    {
                        StudentEmploymentInformation.StartDay = new Value();
                        StudentEmploymentInformation.StartMonth = new Value();
                        StudentEmploymentInformation.StartYear = new Value();
                    }

                    if (employmentInformation.EndDate != null)
                    {
                        StudentEmploymentInformation.EndDay = employmentInformation.EndDay;
                        StudentEmploymentInformation.EndMonth = employmentInformation.EndMonth;
                        StudentEmploymentInformation.EndYear = employmentInformation.EndYear;
                    }
                    else
                    {
                        StudentEmploymentInformation.EndDay = new Value();
                        StudentEmploymentInformation.EndMonth = new Value();
                        StudentEmploymentInformation.EndYear = new Value();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetStudentNdResult(StudentNdResult ndResult)
        {
            try
            {
                if (ndResult != null)
                {
                    if (ndResult.Student != null)
                    {
                        StudentNdResult.Student = ndResult.Student;
                    }
                    else
                    {
                        StudentNdResult.Student = new Model.Model.Student();
                    }

                    if (ndResult.DateAwarded != null)
                    {
                        StudentNdResult.DayAwarded = ndResult.DayAwarded;
                        StudentNdResult.MonthAwarded = ndResult.MonthAwarded;
                        StudentNdResult.YearAwarded = ndResult.YearAwarded;
                    }
                    else
                    {
                        StudentNdResult.DayAwarded = new Value();
                        StudentNdResult.MonthAwarded = new Value();
                        StudentNdResult.YearAwarded = new Value();
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