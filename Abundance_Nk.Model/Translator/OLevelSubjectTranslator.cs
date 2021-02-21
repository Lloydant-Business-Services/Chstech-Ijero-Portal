using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class OLevelSubjectTranslator : TranslatorBase<OLevelSubject, O_LEVEL_SUBJECT>
    {
        public override OLevelSubject TranslateToModel(O_LEVEL_SUBJECT subjectEntity)
       {
           try
           {
               OLevelSubject oLevelSubject = null;
               if (subjectEntity != null)
               {
                   oLevelSubject = new OLevelSubject();
                   oLevelSubject.Id = subjectEntity.O_Level_Subject_Id;
                   oLevelSubject.Name = subjectEntity.O_Level_Subject_Name;
                   oLevelSubject.Description = subjectEntity.O_Level_Subject_Description;
               }

               return oLevelSubject;
           }
           catch (Exception)
           {
               throw;
           }
       }

        public override O_LEVEL_SUBJECT TranslateToEntity(OLevelSubject oLevelSubject)
       {
           try
           {
               O_LEVEL_SUBJECT subjectEntity = null;
               if (oLevelSubject != null)
               {
                   subjectEntity = new O_LEVEL_SUBJECT();
                   subjectEntity.O_Level_Subject_Id = oLevelSubject.Id;
                   subjectEntity.O_Level_Subject_Name = oLevelSubject.Name;
                   subjectEntity.O_Level_Subject_Description = oLevelSubject.Description;
               }

               return subjectEntity;
           }
           catch (Exception)
           {
               throw;
           }
       }



    }
}
