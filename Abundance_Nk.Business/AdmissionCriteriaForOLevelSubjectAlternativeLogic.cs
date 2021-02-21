using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;
using System.Transactions;

namespace Abundance_Nk.Business
{
    public class AdmissionCriteriaForOLevelSubjectAlternativeLogic : BusinessBaseLogic<AdmissionCriteriaForOLevelSubjectAlternative, ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE>
    {
        public AdmissionCriteriaForOLevelSubjectAlternativeLogic()
        {
            translator = new AdmissionCriteriaForOLevelSubjectAlternativeTranslator();
        }

        public AdmissionCriteriaForOLevelSubjectAlternative GetPkBy(int Id)
        {
            return GetModelBy(a => a.Admission_Criteria_For_O_Level_Subject_Alternative_Id == Id);
        }

        public List<AdmissionCriteriaForOLevelSubjectAlternative> GetBy(long Id)
        {
            return GetModelsBy(a => a.Admission_Criteria_For_O_Level_Subject_Id == Id);
        }

        public bool DeleteAlternative(AdmissionCriteriaForOLevelSubjectAlternative subjectAlternative)
        {
            try
            {
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE, bool>> selector = a => a.Admission_Criteria_For_O_Level_Subject_Alternative_Id == subjectAlternative.Id;
                bool deleted = Delete(selector);
                return deleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteAllAlternatives(AdmissionCriteriaForOLevelSubject subject)
        {
            try
            {
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE, bool>> selector = a => a.Admission_Criteria_For_O_Level_Subject_Id == subject.Id;
                bool deleted = Delete(selector);
                return deleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Modify(List<AdmissionCriteriaForOLevelSubject> subjects)
        {
            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (AdmissionCriteriaForOLevelSubject subject in subjects)
                    {
                        
                            foreach (AdmissionCriteriaForOLevelSubjectAlternative subjectAlternative in subject.Alternatives)
                            {
                                 Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE, bool>> selector = a => a.Admission_Criteria_For_O_Level_Subject_Id == subject.Id;
                                 ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE criteriaAlternative = GetEntityBy(selector);


                                if (criteriaAlternative == null)
                                {
                                    AdmissionCriteriaForOLevelSubjectAlternative  criteriaAlternativeSubject = new AdmissionCriteriaForOLevelSubjectAlternative();
                                    criteriaAlternativeSubject.OLevelSubject.Id = subjectAlternative.OLevelSubject.Id;
                                    criteriaAlternativeSubject.Alternative.Id = subject.Id;
                                    Create(criteriaAlternativeSubject);
                                }
                                else
                                {
                                    criteriaAlternative.O_Level_Subject_Id = subjectAlternative.OLevelSubject.Id;
                                    int modifiedRecordCount = Save();
                                }


                            }
                        }
                    scope.Complete();
                      return true;
                    }
                }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool Modify(List<AdmissionCriteriaForOLevelSubjectAlternative> subjects)
        {
            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                   

                   foreach (AdmissionCriteriaForOLevelSubjectAlternative subjectAlternative in subjects)
                        {
                            Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE, bool>> selector = a => a.Admission_Criteria_For_O_Level_Subject_Id == subjectAlternative.Alternative.Id && a.O_Level_Subject_Id == subjectAlternative.OLevelSubject.Id;
                            ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE criteriaAlternative = GetEntityBy(selector);


                            if (criteriaAlternative == null && subjectAlternative.OLevelSubject.Id > 0)
                            {
                                AdmissionCriteriaForOLevelSubjectAlternative criteriaAlternativeSubject = new AdmissionCriteriaForOLevelSubjectAlternative();
                                criteriaAlternativeSubject.OLevelSubject = new OLevelSubject();
                                criteriaAlternativeSubject.OLevelSubject.Id = subjectAlternative.OLevelSubject.Id;
                                criteriaAlternativeSubject.Alternative = new AdmissionCriteriaForOLevelSubject();
                                criteriaAlternativeSubject.Alternative.Id = subjectAlternative.Alternative.Id;
                                Create(criteriaAlternativeSubject);
                            }
                            else if(criteriaAlternative != null && criteriaAlternative.Admission_Criteria_For_O_Level_Subject_Alternative_Id > 0)
                            {
                                criteriaAlternative.O_Level_Subject_Id = subjectAlternative.OLevelSubject.Id;
                                int modifiedRecordCount = Save();
                            }

                       
                        }
                    scope.Complete();

                    return true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public bool ModifyOnly(AdmissionCriteriaForOLevelSubjectAlternative model)
        {
            try
            {
                Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE, bool>> selector = a => a.Admission_Criteria_For_O_Level_Subject_Alternative_Id == model.Id;
                ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE criteriaAlternative = GetEntityBy(selector);

                if (criteriaAlternative == null)
                {
                    return false;
                }

                if (model.Alternative != null )
                {
                    criteriaAlternative.Admission_Criteria_For_O_Level_Subject_Id = model.Alternative.Id;
                }

                criteriaAlternative.O_Level_Subject_Id = model.OLevelSubject.Id;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
  
    }






}
