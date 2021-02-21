using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;


namespace Abundance_Nk.Business
{
    public class OLevelSubjectLogic : BusinessBaseLogic<OLevelSubject, O_LEVEL_SUBJECT>
    {
        public OLevelSubjectLogic()
        {
            translator = new OLevelSubjectTranslator();
        }

        public bool Modify(OLevelSubject oLevelSubject)
        {
            try
            {
                Expression<Func<O_LEVEL_SUBJECT, bool>> selector = s => s.O_Level_Subject_Id == oLevelSubject.Id;
                O_LEVEL_SUBJECT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.O_Level_Subject_Name = oLevelSubject.Name;
                entity.O_Level_Subject_Description = oLevelSubject.Description;

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




        //public override void ModifyHelper(Subject subject)
        //{
        //    try
        //    {
        //        SUBJECT subjectEntity = GetEntityBy(s => s.Subject_Id == subject.Id);
        //        subjectEntity.Subject_Id = subject.Id;
        //        subjectEntity.Subject_Name = subject.Name;
        //        subjectEntity.Subject_Description = subject.Description;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public override bool Remove(Func<SUBJECT, bool> selector)
        //{
        //    bool suceeded = base.Remove(selector);
        //    repository.SaveChanges();
        //    return suceeded;
        //}

        //public List<Subject> GetBy(Level level)
        //{
        //    try
        //    {
        //        var results = (from s in repository.Fetch<VW_LEVEL_SUBJECT>()
        //                       where s.Level_Id == level.Id
        //                       select new Subject
        //                       {
        //                           Id = s.Subject_Id,
        //                           Name = s.Subject_Name,
        //                       }).Distinct().OrderBy(s => s.Name).ToList();

        //        return results;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public List<Subject> GetBy(int subjectCategoryId)
        //{
        //    try
        //    {
        //        var results = (from s in repository.Fetch<VW_SUBJECT_CATEGORY>()
        //                       where s.Subject_Category_Id == subjectCategoryId
        //                       select new Subject
        //                       {
        //                           Id = s.Subject_Id,
        //                           Name = s.Subject_Name,
        //                           Rank = s.Subject_Rank,
        //                       }).ToList();

        //        return results;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public List<Subject> Get(int levelSubjectCategoryId)
        //{
        //    try
        //    {
        //        var subjects = (from s in repository.Fetch<VW_LEVEL_SUBJECT_CATEGORY>()
        //                        where s.Level_Subject_Category_Id == levelSubjectCategoryId
        //                        select new Subject
        //                        {
        //                            Id = s.Subject_Id,
        //                            Name = s.Subject_Name,
        //                            Rank = s.Subject_Rank,
        //                        }).ToList();

        //        return subjects;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public List<Subject> GetDistinctSubject()
        //{
        //    try
        //    {
        //        List<Subject> subjects = (from s in repository.Fetch<VW_STUDENT_RESULT_SUBJECT_UPLOADED>()
        //                                  select new Subject
        //                                  {
        //                                      Id = s.Subject_Id,
        //                                      Name = s.Subject_Name,
        //                                  }
        //                                 ).Distinct().ToList();
        //        return subjects;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}





    }
}
