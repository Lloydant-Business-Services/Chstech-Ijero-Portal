using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;

namespace Abundance_Nk.Web.Areas.Student.Models
{
    public class FormViewModel
    {
        private SexLogic sexLogic;
        private StateLogic stateLogic;
        private LocalGovernmentLogic lgaLogic;
        private ProgrammeLogic programmeLogic;
        private DepartmentLogic departmentLogic;
        private OLevelTypeLogic oLevelTypeLogic;
        private OLevelGradeLogic oLevelGradeLogic;
        private OLevelSubjectLogic oLevelSubjectLogic;
        private ResultGradeLogic resultGradeLogic;
        private EducationalQualificationLogic educationalQualificationLogic;
        private ITDurationLogic iTDurationLogic;
        private InstitutionChoiceLogic institutionChoiceLogic;
        private AbilityLogic abilityLogic;
        private ReligionLogic religionLogic;


        private TitleLogic titleLogic;
        private MaritalStatusLogic maritalStatusLogic;
        private BloodGroupLogic bloodGroupLogic;
        private GenotypeLogic genotypeLogic;
        private ModeOfEntryLogic modeOfEntryLogic;
        private ModeOfStudyLogic modeOfStudyLogic;
        private StudentTypeLogic studentTypeLogic;
        private StudentStatusLogic studentStatusLogic;
        private LevelLogic levelLogic;
        private ModeOfFinanceLogic modeOfFinanceLogic;
        private RelationshipLogic relationshipLogic;
        private DepartmentOptionLogic departmentOptionLogic;
        private AppliedCourseLogic appliedCourseLogic;

        const int START_YEAR = 1920;

        public FormViewModel()
        {
            sexLogic = new SexLogic();
            stateLogic = new StateLogic();
            lgaLogic = new LocalGovernmentLogic();
            relationshipLogic = new RelationshipLogic();
            programmeLogic = new ProgrammeLogic();
            departmentLogic = new DepartmentLogic();
            oLevelTypeLogic = new OLevelTypeLogic();
            oLevelGradeLogic = new OLevelGradeLogic();
            oLevelSubjectLogic = new OLevelSubjectLogic();
            resultGradeLogic = new ResultGradeLogic();
            educationalQualificationLogic = new EducationalQualificationLogic();
            iTDurationLogic = new ITDurationLogic();
            institutionChoiceLogic = new InstitutionChoiceLogic();
            abilityLogic = new AbilityLogic();
            religionLogic = new ReligionLogic();

            Sponsor = new Sponsor();
            Sponsor.Relationship = new Relationship();

            AppliedCourse = new AppliedCourse();
            AppliedCourse.Programme = new Programme();
            AppliedCourse.Department = new Department();
            AppliedCourse.Department.Faculty = new Faculty();

            Programme = new Programme();

            FirstSittingOLevelResult = new OLevelResult();
            FirstSittingOLevelResult.Type = new OLevelType();

            SecondSittingOLevelResult = new OLevelResult();
            SecondSittingOLevelResult.Type = new OLevelType();

            PreviousEducation = new PreviousEducation();
            PreviousEducation.ResultGrade = new ResultGrade();
            PreviousEducation.Qualification = new EducationalQualification();
            PreviousEducation.ITDuration = new ITDuration();
            PreviousEducation.StartYear = new Value();
            PreviousEducation.StartMonth = new Value();
            PreviousEducation.StartDay = new Value();
            PreviousEducation.EndYear = new Value();
            PreviousEducation.EndMonth = new Value();
            PreviousEducation.EndDay = new Value();

            ApplicantJambDetail = new ApplicantJambDetail();
            ApplicantJambDetail.InstitutionChoice = new InstitutionChoice();

            Person = new Person();
            Person.LocalGovernment = new LocalGovernment();
            Person.Religion = new Religion();
            Person.State = new State();
            Person.Sex = new Sex();
            Person.YearOfBirth = new Value();
            Person.MonthOfBirth = new Value();
            Person.DayOfBirth = new Value();

            Applicant = new Abundance_Nk.Model.Model.Applicant();
            Applicant.Person = new Person();
            Applicant.ApplicationForm = new ApplicationForm();
            Applicant.Ability = new Ability();

            Payment = new Payment();
            ApplicationProgrammeFee = new ApplicationProgrammeFee();

            InitialiseOLevelResult();





            levelLogic = new LevelLogic();
            titleLogic = new TitleLogic();
            maritalStatusLogic = new MaritalStatusLogic();
            bloodGroupLogic = new BloodGroupLogic();
            genotypeLogic = new GenotypeLogic();
            modeOfEntryLogic = new ModeOfEntryLogic();
            modeOfStudyLogic = new ModeOfStudyLogic();
            studentTypeLogic = new StudentTypeLogic();
            studentStatusLogic = new StudentStatusLogic();
            modeOfFinanceLogic = new ModeOfFinanceLogic();
            relationshipLogic = new RelationshipLogic();
            departmentOptionLogic = new DepartmentOptionLogic();
            appliedCourseLogic = new AppliedCourseLogic();

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

            //StudentNdResultDetails = new List<StudentNdResultDetail>();

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

            PopulateAllApplicantDropDowns();
            PopulateAllStudentDropDowns();
        }

        public string ApplicationFormNumber { get; set; }
        public bool ApplicationAlreadyExist { get; set; }
        public HttpPostedFileBase MyFile { get; set; }
        public Person Person { get; set; }
        public ApplicationFormSetting ApplicationFormSetting { get; set; }
        public ApplicationProgrammeFee ApplicationProgrammeFee { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public Sponsor ApplicantSponsor { get; set; }
        public OLevelResult ApplicantFirstSittingOLevelResult { get; set; }
        public OLevelResult ApplicantSecondSittingOLevelResult { get; set; }
        public List<OLevelResultDetail> ApplicantFirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> ApplicantSecondSittingOLevelResultDetails { get; set; }
        public PreviousEducation ApplicantPreviousEducation { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public Abundance_Nk.Model.Model.Applicant Applicant { get; set; }
        public Payment Payment { get; set; }
        public Sponsor Sponsor { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public OLevelResult FirstSittingOLevelResult { get; set; }
        public OLevelResult SecondSittingOLevelResult { get; set; }
        public OLevelResultDetail FirstSittingOLevelResultDetail { get; set; }
        public OLevelResultDetail SecondSittingOLevelResultDetail { get; set; }
        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public PreviousEducation PreviousEducation { get; set; }

        public List<Sex> Genders { get; set; }
        public List<State> States { get; set; }
        public List<LocalGovernment> Lgas { get; set; }
        public List<Religion> Religions { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetails { get; set; }
        public List<Programme> Programmes { get; set; }
        public List<Department> Departments { get; set; }
        public List<Value> ExamYears { get; set; }
        public List<OLevelType> OLevelTypes { get; set; }
        public List<OLevelGrade> OLevelGrades { get; set; }
        public List<OLevelSubject> OLevelSubjects { get; set; }
        public List<Value> GraduationYears { get; set; }
        public List<ResultGrade> ResultGrades { get; set; }
        public List<EducationalQualification> EducationalQualifications { get; set; }
        public List<Ability> Abilities { get; set; }
        public List<Relationship> Relationships { get; set; }
        public List<ITDuration> ITDurations { get; set; }
        public List<InstitutionChoice> InstitutionChoices { get; set; }

        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> SexSelectList { get; set; }
        public List<SelectListItem> FacultySelectList { get; set; }
        public List<SelectListItem> RelationshipSelectList { get; set; }
        public List<SelectListItem> GraduationYearSelectList { get; set; }
        public List<SelectListItem> GraduationMonthSelectList { get; set; }
        public List<SelectListItem> ExamYearSelectList { get; set; }
        public List<SelectListItem> OLevelTypeSelectList { get; set; }
        public List<SelectListItem> OLevelGradeSelectList { get; set; }
        public List<SelectListItem> OLevelSubjectSelectList { get; set; }
        public List<SelectListItem> ResultGradeSelectList { get; set; }
        public List<SelectListItem> LocalGovernmentSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<SelectListItem> ReligionSelectList { get; set; }
        public List<SelectListItem> AbilitySelectList { get; set; }
        public List<SelectListItem> EducationalQualificationSelectList { get; set; }
        public List<SelectListItem> ITDurationSelectList { get; set; }
        public List<SelectListItem> JambScoreSelectList { get; set; }
        public List<SelectListItem> InstitutionChoiceSelectList { get; set; }
        public List<SelectListItem> DayOfBirthSelectList { get; set; }
        public List<SelectListItem> MonthOfBirthSelectList { get; set; }
        public List<SelectListItem> YearOfBirthSelectList { get; set; }
        public List<SelectListItem> PreviousEducationStartDaySelectList { get; set; }
        public List<SelectListItem> PreviousEducationStartMonthSelectList { get; set; }
        public List<SelectListItem> PreviousEducationStartYearSelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndDaySelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndMonthSelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndYearSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
       




        public List<Title> Titles { get; set; }
        public List<MaritalStatus> MaritalStatuses { get; set; }
        public List<BloodGroup> BloodGroups { get; set; }
        public List<Genotype> Genotypes { get; set; }
        public List<ModeOfEntry> ModeOfEntries { get; set; }
        public List<ModeOfStudy> ModeOfStudies { get; set; }
        public List<StudentType> StudentTypes { get; set; }
        public List<StudentStatus> StudentStatuses { get; set; }
        public List<Level> Levels { get; set; }
        public List<ModeOfFinance> ModeOfFinances { get; set; }
        public List<DepartmentOption> DepartmentOptions { get; set; }
               
        public Abundance_Nk.Model.Model.Student Student { get; set; }
        public StudentAcademicInformation StudentAcademicInformation { get; set; }
        public StudentSponsor StudentSponsor { get; set; }
        public StudentFinanceInformation StudentFinanceInformation { get; set; }
        public StudentNdResult StudentNdResult { get; set; }
        //public List<StudentNdResultDetail> StudentNdResultDetails { get; set; }
        public StudentEmploymentInformation StudentEmploymentInformation { get; set; }

        public List<SelectListItem> TitleSelectList { get; set; }
        public List<SelectListItem> MaritalStatusSelectList { get; set; }
        public List<SelectListItem> BloodGroupSelectList { get; set; }
        public List<SelectListItem> GenotypeSelectList { get; set; }
        public List<SelectListItem> ModeOfEntrySelectList { get; set; }
        public List<SelectListItem> ModeOfStudySelectList { get; set; }
        public List<SelectListItem> StudentTypeSelectList { get; set; }
        public List<SelectListItem> StudentStatusSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ModeOfFinanceSelectList { get; set; }
        public List<SelectListItem> DepartmentOptionSelectList { get; set; }
        public List<SelectListItem> AdmissionYearSelectList { get; set; }
        //public List<SelectListItem> GraduationYearSelectList { get; set; }

        public List<SelectListItem> StudentNdResultDayAwardedSelectList { get; set; }
        public List<SelectListItem> StudentNdResultMonthAwardedSelectList { get; set; }
        public List<SelectListItem> StudentNdResultYearAwardedSelectList { get; set; }

        public List<SelectListItem> StudentLastEmploymentStartDaySelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentStartMonthSelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentStartYearSelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentEndDaySelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentEndMonthSelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentEndYearSelectList { get; set; }

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

        private void PopulateAllStudentDropDowns()
        {
            try
            {
                Titles = titleLogic.GetAll();
                MaritalStatuses = maritalStatusLogic.GetAll();
                BloodGroups = bloodGroupLogic.GetAll();
                Genotypes = genotypeLogic.GetAll();
                ModeOfEntries = modeOfEntryLogic.GetAll();
                ModeOfStudies = modeOfStudyLogic.GetAll();
                StudentTypes = studentTypeLogic.GetAll();
                StudentStatuses = studentStatusLogic.GetAll();
                Levels = levelLogic.GetAll();
                ModeOfFinances = modeOfFinanceLogic.GetAll();
                Relationships = relationshipLogic.GetAll();
                DepartmentOptions = departmentOptionLogic.GetAll();

                TitleSelectList = Utility.PopulateTitleSelectListItem();
                MaritalStatusSelectList = Utility.PopulateMaritalStatusSelectListItem();
                BloodGroupSelectList = Utility.PopulateBloodGroupSelectListItem();
                GenotypeSelectList = Utility.PopulateGenotypeSelectListItem();
                ModeOfEntrySelectList = Utility.PopulateModeOfEntrySelectListItem();
                ModeOfStudySelectList = Utility.PopulateModeOfStudySelectListItem();
                StudentTypeSelectList = Utility.PopulateStudentTypeSelectListItem();
                StudentStatusSelectList = Utility.PopulateStudentStatusSelectListItem();
                LevelSelectList = Utility.PopulateLevelSelectListItem();
                GenotypeSelectList = Utility.PopulateGenotypeSelectListItem();
                ModeOfFinanceSelectList = Utility.PopulateModeOfFinanceSelectListItem();
                RelationshipSelectList = Utility.PopulateRelationshipSelectListItem();

                //DepartmentOptionSelectList = Utility.PopulateAbilitySelectListIte
                AdmissionYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, true);
                //GraduationYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, true);

                StudentNdResultMonthAwardedSelectList = Utility.PopulateMonthSelectListItem();
                StudentNdResultYearAwardedSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);
                
                StudentLastEmploymentStartMonthSelectList = Utility.PopulateMonthSelectListItem();
                StudentLastEmploymentStartYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);

                StudentLastEmploymentEndMonthSelectList = Utility.PopulateMonthSelectListItem();
                StudentLastEmploymentEndYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PopulateAllApplicantDropDowns()
        {
            const int ONE = 1;
            const int FOUR_HUNDRED = 400;
           
            try
            {
                Genders = sexLogic.GetAll();
                States = stateLogic.GetAll();
                Lgas = lgaLogic.GetAll();
                Relationships = relationshipLogic.GetAll();
                Programmes = programmeLogic.GetAll();
                ITDurations = iTDurationLogic.GetAll();
                InstitutionChoices = institutionChoiceLogic.GetAll();
                Abilities = abilityLogic.GetAll();
                Religions = religionLogic.GetAll();

                GraduationYears = Utility.CreateYearListFrom(START_YEAR);
                ExamYears = GraduationYears;
                OLevelTypes = oLevelTypeLogic.GetAll();
                OLevelGrades = oLevelGradeLogic.GetAll();
                OLevelSubjects = oLevelSubjectLogic.GetAll();
                ResultGrades = resultGradeLogic.GetAll();
                EducationalQualifications = educationalQualificationLogic.GetAll();

                ReligionSelectList = Utility.PopulateReligionSelectListItem();
                SexSelectList = Utility.PopulateSexSelectListItem();
                StateSelectList = Utility.PopulateStateSelectListItem();
                FacultySelectList = Utility.PopulateFacultySelectListItem();
                
                RelationshipSelectList = Utility.PopulateRelationshipSelectListItem();
                GraduationYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, true);
                GraduationMonthSelectList = Utility.PopulateGraduationMonthSelectListItem();
                ExamYearSelectList = Utility.PopulateExamYearSelectListItem(START_YEAR);
                OLevelTypeSelectList = Utility.PopulateOLevelTypeSelectListItem();
                OLevelGradeSelectList = Utility.PopulateOLevelGradeSelectListItem();
                OLevelSubjectSelectList = Utility.PopulateOLevelSubjectSelectListItem();
                ResultGradeSelectList = Utility.PopulateResultGradeSelectListItem();
                LocalGovernmentSelectList = Utility.PopulateLocalGovernmentSelectListItem();
                AbilitySelectList = Utility.PopulateAbilitySelectListItem();
                EducationalQualificationSelectList = Utility.PopulateEducationalQualificationSelectListItem();
                ITDurationSelectList = Utility.PopulateITDurationSelectListItem();
                JambScoreSelectList = Utility.PopulateJambScoreSelectListItem(ONE, FOUR_HUNDRED);
                InstitutionChoiceSelectList = Utility.PopulateInstitutionChoiceSelectListItem();
                ProgrammeSelectList = Utility.PopulateProgrammeSelectListItem();

                MonthOfBirthSelectList = Utility.PopulateMonthSelectListItem();
                YearOfBirthSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);

                PreviousEducationStartMonthSelectList = Utility.PopulateMonthSelectListItem();
                PreviousEducationStartYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);
                PreviousEducationEndMonthSelectList = Utility.PopulateMonthSelectListItem();
                PreviousEducationEndYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);
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
                    ApplicantFirstSittingOLevelResult = oLevelResultLogic.GetModelBy(p => p.Person_Id == person.Id && p.O_Level_Exam_Sitting_Id == 1);
                    ApplicantSecondSittingOLevelResult = oLevelResultLogic.GetModelBy(p => p.Person_Id == person.Id && p.O_Level_Exam_Sitting_Id == 2);
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

        public void SetApplicant(Abundance_Nk.Model.Model.Applicant applicant)
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

        public void SetStudent(Abundance_Nk.Model.Model.Student student)
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