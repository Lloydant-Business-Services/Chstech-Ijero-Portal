using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class StudentResultTranslator : TranslatorBase<StudentResult, STUDENT_RESULT>
    {
        private LevelTranslator levelTranslator;
        private StudentResultTypeTranslator studentResultTypeTranslator;
        private ProgrammeTranslator programmeTranslator;
        private DepartmentTranslator departmentTranslator;
        private SessionSemesterTranslator sessionSemesterTranslator;
        private UserTranslator userTranslator;

        public StudentResultTranslator()
        {
            levelTranslator = new LevelTranslator();
            studentResultTypeTranslator = new StudentResultTypeTranslator();
            programmeTranslator = new ProgrammeTranslator();
            departmentTranslator = new DepartmentTranslator();
            sessionSemesterTranslator = new SessionSemesterTranslator();
            userTranslator = new UserTranslator();
        }

        public override StudentResult TranslateToModel(STUDENT_RESULT entity)
        {
            try
            {
                StudentResult model = null;
                if (entity != null)
                {
                    model = new StudentResult();
                    model.Id = entity.Student_Result_Id;
                    model.Type = studentResultTypeTranslator.Translate(entity.STUDENT_RESULT_TYPE);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.SessionSemester = sessionSemesterTranslator.Translate(entity.SESSION_SEMESTER);
                    model.MaximumObtainableScore = (int) entity.Maximum_Score_Obtainable;
                    model.Uploader = userTranslator.Translate(entity.USER);
                    model.DateUploaded = entity.Date_Uploaded;
                    model.UploadedFileUrl = entity.Uploaded_File_Url;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_RESULT TranslateToEntity(StudentResult model)
        {
            try
            {
                STUDENT_RESULT entity = null;
                if (model != null)
                {
                    entity = new STUDENT_RESULT();
                    entity.Student_Result_Id = model.Id;
                    entity.Student_Result_Type_Id = model.Type.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Session_Semester_Id = model.SessionSemester.Id;
                    entity.Maximum_Score_Obtainable = model.MaximumObtainableScore;
                    entity.Uploader_Id = model.Uploader.Id;
                    entity.Date_Uploaded = model.DateUploaded;
                    entity.Uploaded_File_Url = model.UploadedFileUrl;
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
