
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;

namespace Abundance_Nk.Web.Models.Result
{
    public class StudentResultDataTableTranslator : DataTableTranslatorBase<StudentResultDetail>
    {
        private List<Course> courses;
        private StudentResultType resultType;
        private CourseLogic courseLogic;
        private int maximumScoreObtainable;

        public StudentResultDataTableTranslator(List<StudentLevel> studentLevels, StudentResultType resultType, List<Course> courses, int maximumScoreObtainable)
            : base(studentLevels)
        {
            this.courses = courses;
            this.resultType = resultType;
            this.maximumScoreObtainable = maximumScoreObtainable;

            courseLogic = new CourseLogic();
        }

        public override List<StudentResultDetail> Translate(DataTable excelData)
        {
            try
            {
                if (excelData == null || excelData.Rows.Count == 0)
                {
                    throw new Exception("No data found on excel!");
                }

                errorCount = 0;
                ValidationMessage = "";

                if (InvalidUploadCriteria())
                {
                    throw new Exception(ValidationMessage);
                }
                else if (InvalidTypeOnSheet(excelData))
                {
                    throw new Exception(ValidationMessage);
                }
                else if (DuplicateStudentIdExist(excelData))
                {
                    throw new Exception(ValidationMessage);
                }

                List<StudentResultDetail> results = new List<StudentResultDetail>();
                for (int column = 3; column < excelData.Columns.Count; column++)
                {
                    Course course = courses.Where(s => s.Code.Trim().ToLower() == excelData.Columns[column].ColumnName.Trim().ToLower()).FirstOrDefault();
                    if (course == null || course.Id <= 0)
                    {
                        throw new Exception("'" + excelData.Columns[column].ColumnName + "' cannot be found! Please correct the spelling on the file header and re-upload.");
                    }
                    
                    for (int row = 0; row < excelData.Rows.Count; row++)
                    {
                        long id = excelData.Rows[row][0] == DBNull.Value ? 0 : Convert.ToInt64(excelData.Rows[row][0]);
                        if (id <= 0)
                        {
                            continue;
                        }

                        if (InvalidStudent(id))
                        {
                            throw new Exception("No " + studentLevels[0].Level.Name + " student with ID of '" + id + "' found in the system! Please correct the ID on the sheet, and re-upload.");

                            //throw new Exception("Student with ID of '" + id + "' does not exist in " + studentLevels[0].Level.Name + "! Please correct the ID on the sheet, and re-upload.");
                        }

                        if (StudentDoesNotOfferCourse(course, id))
                        {
                            continue;
                        }

                        decimal score;
                        bool isValidScore = decimal.TryParse(excelData.Rows[row][column].ToString().Trim(), out score);
                        if (isValidScore == false)
                        {
                            continue;
                        }

                        StudentResultDetail resultDetail = new StudentResultDetail();
                        resultDetail.Student = new Student();
                        resultDetail.Student.Id = excelData.Rows[row][0] == DBNull.Value ? 0 : Convert.ToInt64(excelData.Rows[row][0]);
                        resultDetail.Student.MatricNumber = excelData.Rows[row][1] == DBNull.Value ? null : Convert.ToString(excelData.Rows[row][1]);
                        resultDetail.Student.LastName = excelData.Rows[row][2] == DBNull.Value ? null : Convert.ToString(excelData.Rows[row][2]);
                        resultDetail.Score = excelData.Rows[row][column] == DBNull.Value ? 0 : Convert.ToDecimal(excelData.Rows[row][column]);
                        resultDetail.Course = course;

                        Validate(resultDetail);

                        results.Add(resultDetail);
                    }
                }

                if (errorCount > 0)
                {
                    ValidationMessage = "Error detected on the uploaded file is " + errorCount + " ! Please fix and re-upload.";
                }

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidStudent(long id)
        {
            try
            {
                Func<StudentLevel, bool> selector = sl => sl.Student.Id == id;

                bool invalidStudentId = InvalidStudentLevelClassTerm(selector);
                if (invalidStudentId)
                {
                    return true;
                }


                //CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                //List<CourseRegistrationDetail> courseRegistrationDetails = courseRegistrationDetailLogic.GetBy(student, course, studentLevels[0].Level, studentLevels[0].SessionSemester.Session, studentLevels[0].SessionSemester.Semester);
                //if (courseRegistrationDetails == null || courseRegistrationDetails.Count <= 0)
                //{
                //    return true;
                //}

                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool StudentDoesNotOfferCourse(Course course, long studentId)
        {
            try
            {
                //Student student = new Student() { Id = studentId }; 
                //CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                
                //List<CourseRegistrationDetail> courseRegistrationDetails = courseRegistrationDetailLogic.GetBy(student, course, studentLevels[0].Level, studentLevels[0].Session, studentLevels[0].SessionSemester.Semester);
                ////List<CourseRegistrationDetail> courseRegistrationDetails = courseRegistrationDetailLogic.GetBy(student, course, studentLevels[0].Level, studentLevels[0].SessionSemester.Session, studentLevels[0].SessionSemester.Semester);

                //if (courseRegistrationDetails == null || courseRegistrationDetails.Count <= 0)
                //{
                //    return true;
                //}

                return false;
                  
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override void Validate(StudentResultDetail model)
        {
            try
            {
                if (model == null || model.Student == null)
                {
                    throw new Exception("Required object not set! Please try again. Contact your system administrator after three unsuccessful tries.");
                }
                               
                InvalidMatricNumber(model);
                InvalidAssessmentScore(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidMatricNumber(StudentResultDetail result)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                Student student = studentLogic.GetBy(result.Student.Id);
                if (student == null || student.Id <= 0)
                {
                    throw new Exception("Student with ID '" + result.Student.Id + "' does not exist in the system! Please correct and re-upload.");
                }

                if (student.MatricNumber != result.Student.MatricNumber)
                {
                    throw new Exception("Excel sheet Matric No '" + result.Student.MatricNumber + "' for " + result.Student.LastName.ToUpper() + " does not match what is in the system '" + student.MatricNumber + "' ! Please correct and re-upload.");
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void InvalidAssessmentScore(StudentResultDetail result)
        {
            try
            {
                string errorMessage = "";
                if (resultType != null)
                {
                    if (result.Score > maximumScoreObtainable)
                    {
                        errorMessage = "Student " + resultType.Name + " score of '" + result.Score + "' for " + result.Student.LastName.ToUpper() + " in " + result.Course.Name.ToUpper() + " ( " + result.Course.Code.ToUpper() + " ) cannot be greater than Maximum Obtainable Assessment Score of '" + maximumScoreObtainable + "' ! Please correct and re-upload.";
                        throw new Exception(errorMessage);
                    }
                }
                else
                {
                    errorMessage += "Assessment Score not set! Please contact your system administrator.";
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override bool InvalidTypeOnSheet(DataTable excelData)
        {
            try
            {
                const int FIRST_COURSE_COLUMN = 3;

                for (int row = 0; row < excelData.Rows.Count; row++)
                {
                    long id;
                    long admissionNo;
                    long name;
                    decimal score;

                    int rowNumber = row + 1;
                    bool isValidId = long.TryParse(excelData.Rows[row][0].ToString(), out id);
                    if (isValidId == false)
                    {
                        ValidationMessage += "Data '" + excelData.Rows[row][0].ToString() + "' on row " + rowNumber + ", column 1 must be a number! -- ";
                        return true;
                    }

                    bool isValidAdmissionNo = long.TryParse(excelData.Rows[row][1].ToString(), out admissionNo);
                    if (isValidAdmissionNo)
                    {
                        ValidationMessage += "Data '" + excelData.Rows[row][1].ToString() + "' on row '" + rowNumber + "', column 2 must be a text! -- ";
                        return true;
                    }

                    bool isValidName = long.TryParse(excelData.Rows[row][2].ToString(), out name);
                    if (isValidName)
                    {
                        ValidationMessage += "Data '" + excelData.Rows[row][2].ToString() + "' on row '" + rowNumber + "', column 3 must be a text! -- ";
                        return true;
                    }

                    for (int column = FIRST_COURSE_COLUMN; column < excelData.Columns.Count; column++)
                    {
                        bool isValidScore = decimal.TryParse(excelData.Rows[row][column].ToString(), out score);
                        if (isValidScore == false)
                        {
                            string noText = excelData.Rows[row][column].ToString();
                            if (!string.IsNullOrWhiteSpace(noText))
                            {
                                ValidationMessage += "Data '" + excelData.Rows[row][column].ToString() + "' on row " + rowNumber + ", column " + column + " must be a number! -- ";
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
       
        private bool InvalidUploadCriteria()
        {
            try
            {
                string errorMessage = "";

                if (studentLevels == null || studentLevels.Count <= 0)
                {
                    errorMessage = "No student found for the selected Level, Course of Study and Semester! Please change your selection and try again.";
                    SetErrorMessage(errorMessage);

                    return true;
                }
                else if (courses == null || courses.Count <= 0)
                {
                    errorMessage = "Course for the selected Level, Course of Study and Semester has not been set! Please contact your system administrator.";
                    SetErrorMessage(errorMessage);

                    return true;
                }
                else if (resultType == null || resultType.Id <= 0)
                {
                    errorMessage = "Result Type cannot be empty! Please contact your system administrator.";
                    SetErrorMessage(errorMessage);

                    return true;
                }
                else if (maximumScoreObtainable <= 0)
                {
                    errorMessage = "Maximum Score Obtainable for " + resultType.Name + " has not been set! Please contact your system administrator.";
                    SetErrorMessage(errorMessage);

                    return true;
                }
                else if (maximumScoreObtainable <= 0)
                {
                    errorMessage = "Maximum Percent Score Obtainable for " + resultType.Name + " has not been set! Please contact your system administrator.";
                    SetErrorMessage(errorMessage);

                    return true;
                }
               
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }





    }
}