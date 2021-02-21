using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abundance_Nk.Web.Models;
using Abundance_Nk.Business;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class RegistrationViewModel
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
        private StudentCategoryLogic studentCategoryLogic;
        private StudentLevelLogic studentLevelLogic;
        private StudentLogic studentLogic;

        const int START_YEAR = 1920;

        public RegistrationViewModel()
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
            studentLevelLogic = new StudentLevelLogic();
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
            studentCategoryLogic = new StudentCategoryLogic();
            studentLogic = new StudentLogic();


            Programme = new Programme();

            NextOfKin = new NextOfKin();
            NextOfKin.Relationship = new Relationship();
            NextOfKin.PersonType = new PersonType();

            StudentLevel = new StudentLevel();
            StudentLevel.Programme = new Programme();
            StudentLevel.Department = new Department();
            StudentLevel.Department.Faculty = new Faculty();

            FirstSittingOLevelResult = new OLevelResult();
            FirstSittingOLevelResult.Type = new OLevelType();
            FirstSittingOLevelResult.PersonType = new PersonType();

            SecondSittingOLevelResult = new OLevelResult();
            SecondSittingOLevelResult.Type = new OLevelType();
            SecondSittingOLevelResult.PersonType = new PersonType();

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
            PreviousEducation.PersonType = new PersonType();

            Person = new Person();
            Person.LocalGovernment = new LocalGovernment();
            Person.Religion = new Religion();
            Person.State = new State();
            Person.Sex = new Sex();
            Person.YearOfBirth = new Value();
            Person.MonthOfBirth = new Value();
            Person.DayOfBirth = new Value();

            //Applicant = new Abundance_Nk.Model.Model.Applicant();
            //Applicant.Person = new Person();
            //Applicant.ApplicationForm = new ApplicationForm();
            //Applicant.Ability = new Ability();

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
            PopulateAllApplicantDropDowns();
            PopulateAllStudentDropDowns();
        }

        public Person Person { get; set; }
        //public ApplicationForm ApplicationForm { get; set; }
        //public Model.Model.Applicant Applicant { get; set; }
        public NextOfKin NextOfKin { get; set; }
        public OLevelResult StudentFirstSittingOLevelResult { get; set; }
        public OLevelResult StudentSecondSittingOLevelResult { get; set; }
        public PreviousEducation PreviousEducation { get; set; }
        public StudentLevel StudentLevel { get; set; }
        //public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public List<OLevelResultDetail> StudentFirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> StudentSecondSittingOLevelResultDetails { get; set; }

        public Model.Model.Student Student { get; set; }
        public NextOfKin Sponsor { get; set; }
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


        //public string ApplicationFormNumber { get; set; }
        //public bool ApplicationAlreadyExist { get; set; }

        public bool StudentAlreadyExist { get; set; }
        public HttpPostedFileBase PassportFile { get; set; }
       
        //public ApplicationFormSetting ApplicationFormSetting { get; set; }
        //public ApplicationProgrammeFee ApplicationProgrammeFee { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public Payment Payment { get; set; }

        public List<Sex> Genders { get; set; }
        public List<State> States { get; set; }
        public List<LocalGovernment> Lgas { get; set; }
        public List<Religion> Religions { get; set; }
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
        public List<Title> Titles { get; set; }
        public List<MaritalStatus> MaritalStatuses { get; set; }
        public List<BloodGroup> BloodGroups { get; set; }
        public List<Genotype> Genotypes { get; set; }
        public List<ModeOfEntry> ModeOfEntries { get; set; }
        public List<ModeOfStudy> ModeOfStudies { get; set; }
        public List<StudentType> StudentTypes { get; set; }
        public List<StudentCategory> StudentCategories { get; set; }
        public List<StudentStatus> StudentStatuses { get; set; }
        public List<Level> Levels { get; set; }
        public List<ModeOfFinance> ModeOfFinances { get; set; }
        public List<DepartmentOption> DepartmentOptions { get; set; }

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
        public List<SelectListItem> StudentCategorySelectList { get; set; }
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
                OLevelResultDetail oLevelResultDetail1 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail2 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail3 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail4 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail5 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail6 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail7 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail8 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail9 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };

                OLevelResultDetail oLevelResultDetail11 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail22 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail33 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail44 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail55 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail66 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail77 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail88 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };
                OLevelResultDetail oLevelResultDetail99 = new OLevelResultDetail() { Grade = new OLevelGrade() { Id = 0 }, Subject = new OLevelSubject() { Id = 0 } };

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
                StudentCategories = studentCategoryLogic.GetAll();

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

                StudentCategorySelectList = Utility.PopulateStudentCategorySelectListItem();
                AdmissionYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, true);

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
                GraduationYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR,true);
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
                ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();

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
        
        public void LoadStudentInformationFormBy(long personId)
        {
            try
            {
                if (personId > 0)
                {
                    StudentSponsorLogic studentSponsorLogic = new StudentSponsorLogic();
                    StudentAcademicInformationLogic academicInformationLogic = new StudentAcademicInformationLogic();
                    StudentFinanceInformationLogic financeInformationLogic = new StudentFinanceInformationLogic();
                    //StudentEmploymentInformationLogic employmentInformationLogic = new StudentEmploymentInformationLogic();
                    //StudentNdResultLogic ndResultLogic = new StudentNdResultLogic();

                    PersonLogic personLogic = new PersonLogic();
                    NextOfKinLogic nextOfKinLogic = new NextOfKinLogic();
                    OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                    OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
                    //PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();


                    Model.Model.Student student = studentLogic.GetModelBy(p => p.Person_Id == personId);
                    StudentSponsor studentSponsor = studentSponsorLogic.GetModelBy(p => p.Person_Id == personId);
                    StudentAcademicInformation studentAcademicInformation = academicInformationLogic.GetModelBy(p => p.Person_Id == personId);
                    StudentFinanceInformation studentFinanceInformation = financeInformationLogic.GetModelBy(p => p.Person_Id == personId);
                    //StudentEmploymentInformation studentEmploymentInformation = employmentInformationLogic.GetModelBy(p => p.Person_Id == personId);
                    //StudentNdResult studentNdResult = ndResultLogic.GetModelBy(p => p.Person_Id == personId);

                    

                    Person person = personLogic.GetModelBy(p => p.Person_Id == personId);
                    NextOfKin nextofKin = nextOfKinLogic.GetModelsBy(p => p.Person_Id == personId).FirstOrDefault();
                    OLevelResult studentFirstSittingOLevelResult = oLevelResultLogic.GetModelsBy(p => p.Person_Id == personId && p.O_Level_Exam_Sitting_Id == 1).FirstOrDefault();
                    OLevelResult studentSecondSittingOLevelResult = oLevelResultLogic.GetModelsBy(p => p.Person_Id == personId && p.O_Level_Exam_Sitting_Id == 2).FirstOrDefault();
                    //PreviousEducation previousEducation = previousEducationLogic.GetModelsBy(p => p.Person_Id == personId).FirstOrDefault();

                    StudentLevel studentLevel = studentLevelLogic.GetBy(personId);
                    


                    if (studentSponsor != null && studentAcademicInformation != null && studentFinanceInformation != null && (studentFirstSittingOLevelResult != null || studentSecondSittingOLevelResult != null))
                    {
                        StudentAlreadyExist = true;
                    }

                    if (studentFirstSittingOLevelResult != null && studentFirstSittingOLevelResult.Id > 0)
                    {
                        StudentFirstSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == studentFirstSittingOLevelResult.Id);
                    }

                    if (studentSecondSittingOLevelResult != null && studentSecondSittingOLevelResult.Id > 0)
                    {
                        StudentSecondSittingOLevelResultDetails = oLevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == studentSecondSittingOLevelResult.Id);
                    }


                    SetStudent(student);
                    SetStudentSponsor(studentSponsor);
                    SetStudentAcademicInformation(studentAcademicInformation);
                    SetStudentFinanceInformation(studentFinanceInformation);
                    //SetStudentEmploymentInformation(studentEmploymentInformation);
                    //SetStudentNdResult(studentNdResult);

                    SetStudentBioData(person);
                    SetStudentNextOfKin(nextofKin);
                    SetStudentFirstSittingOLevelResult(studentFirstSittingOLevelResult, StudentFirstSittingOLevelResultDetails);
                    SetStudentSecondSittingOLevelResult(studentSecondSittingOLevelResult, StudentSecondSittingOLevelResultDetails);
                    //SetStudentPreviousEducation(previousEducation);
                    SetStudentLevel(studentLevel);

                    
                }
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

        public void SetStudentPreviousEducation(PreviousEducation previousEducation)
        {
            try
            {
                if (previousEducation != null && previousEducation.Id > 0)
                {
                    PreviousEducation = previousEducation;
                    PreviousEducation.SchoolName = previousEducation.SchoolName;
                    PreviousEducation.Course = previousEducation.Course;

                    if (previousEducation.Person != null && previousEducation.Person.Id > 0)
                    {
                        PreviousEducation.Person = previousEducation.Person;
                    }
                    else
                    {
                        PreviousEducation.Person = new Person();
                    }

                    if (previousEducation.PersonType != null && previousEducation.PersonType.Id > 0)
                    {
                        PreviousEducation.PersonType = previousEducation.PersonType;
                    }
                    else
                    {
                        PreviousEducation.PersonType = new PersonType();
                    }

                    if (previousEducation.Qualification != null && previousEducation.Qualification.Id > 0)
                    {
                        PreviousEducation.Qualification = previousEducation.Qualification;
                    }
                    else
                    {
                        PreviousEducation.Qualification = new EducationalQualification();
                    }

                    if (previousEducation.ResultGrade != null && previousEducation.ResultGrade.Id > 0)
                    {
                        PreviousEducation.ResultGrade = previousEducation.ResultGrade;
                    }
                    else
                    {
                        PreviousEducation.ResultGrade = new ResultGrade();
                    }

                    if (previousEducation.ITDuration != null && previousEducation.ITDuration.Id > 0)
                    {
                        PreviousEducation.ITDuration = previousEducation.ITDuration;
                    }
                    else
                    {
                        PreviousEducation.ITDuration = new ITDuration();
                    }

                    if (previousEducation.StartDate != null)
                    {
                        PreviousEducation.StartDay = previousEducation.StartDay;
                        PreviousEducation.StartMonth = previousEducation.StartMonth;
                        PreviousEducation.StartYear = previousEducation.StartYear;
                    }
                    else
                    {
                        PreviousEducation.StartDay = new Value();
                        PreviousEducation.StartMonth = new Value();
                        PreviousEducation.StartYear = new Value();
                    }

                    if (previousEducation.EndDate != null)
                    {
                        PreviousEducation.EndDay = previousEducation.EndDay;
                        PreviousEducation.EndMonth = previousEducation.EndMonth;
                        PreviousEducation.EndYear = previousEducation.EndYear;
                    }
                    else
                    {
                        PreviousEducation.EndDay = new Value();
                        PreviousEducation.EndMonth = new Value();
                        PreviousEducation.EndYear = new Value();
                    }
                }
              
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetStudentBioData(Person person)
        {
            try
            {
                if (person != null && person.Id > 0)
                {
                    Person = person;
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

        public void SetStudent(Model.Model.Student student)
        {
            try
            {
                if (student != null)
                {
                    Student = student;
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

        public void SetStudentLevel(StudentLevel studentLevel)
        {
            try
            {
                if (studentLevel != null)
                {
                    StudentLevel = studentLevel;
                    if (studentLevel.Department != null)
                    {
                        StudentLevel.Department = studentLevel.Department;
                    }
                    else
                    {
                        StudentLevel.Department = new Department();
                    }

                    if (studentLevel.Programme != null)
                    {
                        StudentLevel.Programme = studentLevel.Programme;
                    }
                    else
                    {
                        StudentLevel.Programme = new Programme();
                    }

                    if (studentLevel.DepartmentOption != null)
                    {
                        StudentLevel.DepartmentOption = studentLevel.DepartmentOption;
                    }
                    else
                    {
                        StudentLevel.DepartmentOption = new DepartmentOption();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetStudentNextOfKin(NextOfKin nextofKin)
        {
            try
            {
                if (nextofKin != null)
                {
                    NextOfKin = nextofKin;
                    NextOfKin.Name = nextofKin.Name;
                    NextOfKin.ContactAddress = nextofKin.ContactAddress;
                    NextOfKin.MobilePhone = nextofKin.MobilePhone;

                    if (nextofKin.Relationship != null)
                    {
                        NextOfKin.Relationship = nextofKin.Relationship;
                    }
                    else
                    {
                        NextOfKin.Relationship = new Relationship();
                    }

                    if (nextofKin.PersonType != null)
                    {
                        NextOfKin.PersonType = nextofKin.PersonType;
                    }
                    else
                    {
                        NextOfKin.PersonType = new PersonType();
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
                    StudentSponsor = sponsor;
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

        public void SetStudentFirstSittingOLevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                if (oLevelResult != null && oLevelResult.Id > 0)
                {
                    FirstSittingOLevelResult = oLevelResult;

                    if (oLevelResult.Type != null)
                    {
                        FirstSittingOLevelResult.Type = oLevelResult.Type;
                    }
                    else
                    {
                        FirstSittingOLevelResult.Type = new OLevelType();
                    }

                    if (oLevelResult.PersonType != null)
                    {
                        FirstSittingOLevelResult.PersonType = oLevelResult.PersonType;
                    }
                    else
                    {
                        FirstSittingOLevelResult.PersonType = new PersonType();
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

        public void SetStudentSecondSittingOLevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                if (oLevelResult != null && oLevelResult.Id > 0)
                {
                    SecondSittingOLevelResult = oLevelResult;
                    if (oLevelResult.Type != null)
                    {
                        SecondSittingOLevelResult.Type = oLevelResult.Type;
                    }
                    else
                    {
                        SecondSittingOLevelResult.Type = new OLevelType();
                    }

                    if (oLevelResult.PersonType != null)
                    {
                        SecondSittingOLevelResult.PersonType = oLevelResult.PersonType;
                    }
                    else
                    {
                        SecondSittingOLevelResult.PersonType = new PersonType();
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
                    StudentAcademicInformation = academicInformation;
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
                    StudentFinanceInformation = financeInformation;
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
                    StudentEmploymentInformation = employmentInformation;
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
                    StudentNdResult = ndResult;
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