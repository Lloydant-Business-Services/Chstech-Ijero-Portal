using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;


namespace Abundance_Nk.Model.Translator
{
    public class AdmissionListBatchTranslator : TranslatorBase<AdmissionListBatch, ADMISSION_LIST_BATCH>
    {
        private AdmissionListTypeTranslator admissionListTypeTranslator;

        public AdmissionListBatchTranslator()
        {
            admissionListTypeTranslator = new AdmissionListTypeTranslator();
        }

        public override AdmissionListBatch TranslateToModel(ADMISSION_LIST_BATCH entity)
        {
            try
            {
                AdmissionListBatch model = null;
                if (entity != null)
                {
                    model = new AdmissionListBatch();
                    model.Id = entity.Admission_List_Batch_Id;
                    model.Type = admissionListTypeTranslator.Translate(entity.ADMISSION_LIST_TYPE);
                    model.DateUploaded = entity.Date_Uploaded;
                    model.IUploadedFilePath = entity.Uploaded_File_Path;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_LIST_BATCH TranslateToEntity(AdmissionListBatch model)
        {
            try
            {
                ADMISSION_LIST_BATCH entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_LIST_BATCH();
                    entity.Admission_List_Batch_Id = model.Id;
                    entity.Admission_List_Type_Id = model.Type.Id;
                    entity.Date_Uploaded = model.DateUploaded;
                    entity.Uploaded_File_Path = model.IUploadedFilePath;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }






    }


}
