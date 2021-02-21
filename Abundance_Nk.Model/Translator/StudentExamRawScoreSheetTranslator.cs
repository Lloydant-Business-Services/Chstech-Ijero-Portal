using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class StudentExamRawScoreSheetTranslator : TranslatorBase<StudentExamRawScoreSheet,STUDENT_EXAM_RAW_SCORE_SHEET_RESULT>
    {
        private SessionTranslator sessionTranslator;
        private SemesterTranslator semesterTranslaror;
        private LevelTranslator levelTranslator;
        private StudentTranslator studentTranslator;
        private CourseTranslator courseTranslator;
        private UserTranslator userTranslator;

        public StudentExamRawScoreSheetTranslator()
        {
            sessionTranslator = new SessionTranslator();
            semesterTranslaror = new SemesterTranslator();
            levelTranslator = new LevelTranslator();
            studentTranslator = new StudentTranslator();
            courseTranslator = new CourseTranslator();
            userTranslator = new UserTranslator();
        }
        public override StudentExamRawScoreSheet TranslateToModel(STUDENT_EXAM_RAW_SCORE_SHEET_RESULT entity)
        {
            try
            {
                StudentExamRawScoreSheet model = null;
                if (entity != null)
                {
                    model = new StudentExamRawScoreSheet();
                    model.Id = entity.Student_Result_Id;
                    model.Course = courseTranslator.Translate(entity.COURSE);
                    model.EX_CA = entity.EX_CA;
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.QU1 = entity.QU1;
                    model.QU2 = entity.QU2;
                    model.QU3 = entity.QU3;
                    model.QU4 = entity.QU4;
                    model.QU5 = entity.QU5;
                    model.QU6 = entity.QU6;
                    model.QU7 = entity.QU7;
                    model.QU8 = entity.QU8;
                    model.QU9 = entity.QU9;
                    model.Remark = entity.Remark;
                    model.Semester = semesterTranslaror.Translate(entity.SEMESTER);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Special_Case = entity.Special_Case;
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.T_CA = entity.T_CA;
                    model.T_EX = entity.T_EX;
                    model.Uploader = userTranslator.Translate(entity.USER);
                    

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_EXAM_RAW_SCORE_SHEET_RESULT TranslateToEntity(StudentExamRawScoreSheet model)
        {
            try
            {
                STUDENT_EXAM_RAW_SCORE_SHEET_RESULT entity = null;
                if (model != null)
                {
                    entity = new STUDENT_EXAM_RAW_SCORE_SHEET_RESULT();
                    entity.Student_Result_Id = model.Id;
                    entity.Course_Id = model.Course.Id;
                    entity.EX_CA = model.EX_CA;
                    entity.Level_Id = model.Level.Id;
                    entity.QU1 = model.QU1;
                    entity.QU2 = model.QU2;
                    entity.QU3 = model.QU3;
                    entity.QU4 = model.QU4;
                    entity.QU5 = model.QU5;
                    entity.QU6 = model.QU6;
                    entity.QU7 = model.QU7;
                    entity.QU8 = model.QU8;
                    entity.QU9 = model.QU9;
                    entity.Remark = model.Special_Case;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Special_Case = model.Special_Case;
                    entity.Student_Id = model.Student.Id;
                    
                    entity.T_CA = model.T_CA;
                    entity.T_EX = model.T_EX;
                    entity.Uploader_Id = model.Uploader.Id;
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
