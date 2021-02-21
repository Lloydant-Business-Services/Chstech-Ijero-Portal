using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abundance_Nk.Model.Model
{
    public class PersonRight
    {
        public List<Right> Rights { get; set; }
        public List<Right> View { get; set; }

        public bool CanManageStudent { get; set; }
        public bool CanManageClassRoom { get; set; }
        public bool CanManageSubject { get; set; }
        public bool CanManageExam { get; set; }
        public bool CanManageResultUpload { get; set; }
        public bool CanManageUploadedResultEdit { get; set; }
        public bool CanCheckResult { get; set; }
        public bool CanManageStaff { get; set; }
        public bool CanManageDepartment { get; set; }
        public bool CanManageNews { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanSendSms { get; set; }

        public bool CanViewStudentList { get; set; }
        public bool CanViewStudentWithoutPassport { get; set; }
        public bool CanModifyStudentPassport { get; set; }
        public bool CanViewAndPrintAdmissionLetter { get; set; }

        public bool CanAssignClassToStudent { get; set; }

        public bool CanSetupSubject { get; set; }
        public bool CanSetupSubjectCategory { get; set; }
        public bool CanSetupLevelSubject { get; set; }

        public bool CanViewAndPrintEntranceExamResult { get; set; }
        public bool CanSetupClassAssessment { get; set; }

        public bool CanUploadResult { get; set; }
        public bool CanUploadComment { get; set; }
        public bool CanUploadAttendance { get; set; }
        public bool CanUploadPsychomotor { get; set; }

        public bool CanEditUploadedResult { get; set; }
        public bool CanEditUploadedComment { get; set; }
        public bool CanEditUploadedAttendance { get; set; }
        public bool CanEditUploadedPsychomotor { get; set; }

        public bool CanViewAndPrintStudentResultMasterSheet { get; set; }
        public bool CanViewAndPrintStudentReportCard { get; set; }

        public bool CanSetupClassTeacher { get; set; }
        public bool CanAssignSubjectToTeacher { get; set; }

        public bool CanSetupDepartment { get; set; }
        public bool CanAssignDepartmentToStaff { get; set; }

        public bool CanSetupNews { get; set; }
        public bool CanSetupTermactivity { get; set; }

        public bool CanSetupUsers { get; set; }
        public bool CanSetupRights { get; set; }
        public bool CanSetupRoles { get; set; }
        public bool CanSetupAssignRightToRole { get; set; }
        public bool CanSetupAssignRoleToUser { get; set; }
        public bool CanResetUserPassword { get; set; }

        public bool CanSendSMS { get; set; }
        public bool CanSendMail { get; set; }

        public void Set()
        {
            if (Rights != null)
            {
                if (Rights.Count > 0)
                {

                    foreach (Right right in Rights)
                    {
                        switch (right.Id)
                        {
                            case 1:
                                {
                                    CanViewStudentList = true;
                                    break;
                                }
                            case 2:
                                {
                                    CanViewStudentWithoutPassport = true;
                                    break;
                                }
                            case 3:
                                {
                                    CanModifyStudentPassport = true;
                                    break;
                                }
                            case 4:
                                {
                                    CanViewAndPrintAdmissionLetter = true;
                                    break;
                                }
                            case 5:
                                {
                                    CanAssignClassToStudent = true;
                                    break;
                                }
                            case 6:
                                {
                                    CanSetupSubject = true;
                                    break;
                                }
                            case 7:
                                {
                                    CanSetupSubjectCategory = true;
                                    break;
                                }
                            case 8:
                                {
                                    CanSetupLevelSubject = true;
                                    break;
                                }
                            case 9:
                                {
                                    CanViewAndPrintEntranceExamResult = true;
                                    break;
                                }
                            case 10:
                                {
                                    CanSetupClassAssessment = true;
                                    break;
                                }
                            case 11:
                                {
                                    CanUploadResult = true;
                                    break;
                                }
                            case 12:
                                {
                                    CanUploadComment = true;
                                    break;
                                }
                            case 13:
                                {
                                    CanUploadAttendance = true;
                                    break;
                                }
                            case 14:
                                {
                                    CanUploadPsychomotor = true;
                                    break;
                                }
                            case 15:
                                {
                                    CanEditUploadedResult = true;
                                    break;
                                }
                            case 16:
                                {
                                    CanEditUploadedComment = true;
                                    break;
                                }
                            case 17:
                                {
                                    CanEditUploadedAttendance = true;
                                    break;
                                }
                            case 18:
                                {
                                    CanEditUploadedPsychomotor = true;
                                    break;
                                }
                            case 19:
                                {
                                    CanViewAndPrintStudentResultMasterSheet = true;
                                    break;
                                }
                            case 20:
                                {
                                    CanViewAndPrintStudentReportCard = true;
                                    break;
                                }
                            case 21:
                                {
                                    CanSetupClassTeacher = true;
                                    break;
                                }
                            case 22:
                                {
                                    CanAssignSubjectToTeacher = true;
                                    break;
                                }
                            case 23:
                                {
                                    CanSetupDepartment = true;
                                    break;
                                }
                            case 24:
                                {
                                    CanAssignDepartmentToStaff = true;
                                    break;
                                }

                            case 25:
                                {
                                    CanSetupNews = true;
                                    break;
                                }
                            case 26:
                                {
                                    CanSetupTermactivity = true;
                                    break;
                                }
                            case 27:
                                {
                                    CanSetupUsers = true;
                                    break;
                                }

                            case 28:
                                {
                                    CanSetupRights = true;
                                    break;
                                }
                            case 29:
                                {
                                    CanSetupRoles = true;
                                    break;
                                }
                            case 30:
                                {
                                    CanSetupAssignRightToRole = true;
                                    break;
                                }

                            case 31:
                                {
                                    CanSetupAssignRoleToUser = true;
                                    break;
                                }
                            case 32:
                                {
                                    CanResetUserPassword = true;
                                    break;
                                }
                            case 33:
                                {
                                    CanSendSMS = true;
                                    break;
                                }
                            case 34:
                                {
                                    CanSendMail = true;
                                    break;
                                }

                        }


                        //can manage student
                        if (CanViewStudentList == false && CanViewStudentWithoutPassport == false && CanModifyStudentPassport == false && CanViewAndPrintAdmissionLetter == false)
                        {
                            CanManageStudent = false;
                        }
                        else
                        {
                            CanManageStudent = true;
                        }

                        //can manage class room
                        if (CanAssignClassToStudent == false)
                        {
                            CanManageClassRoom = false;
                        }
                        else
                        {
                            CanManageClassRoom = true;
                        }

                        //can manage subject
                        if (CanSetupSubject == false && CanSetupSubjectCategory == false && CanSetupLevelSubject == false)
                        {
                            CanManageSubject = false;
                        }
                        else
                        {
                            CanManageSubject = true;
                        }

                        //can view report
                        if (CanViewAndPrintEntranceExamResult == false && CanSetupClassAssessment == false)
                        {
                            CanManageExam = false;
                        }
                        else
                        {
                            CanManageExam = true;
                        }

                        //can manage result upload
                        if (CanUploadResult == false && CanUploadComment == false && CanUploadAttendance == false && CanUploadPsychomotor == false)
                        {
                            CanManageResultUpload = false;
                        }
                        else
                        {
                            CanManageResultUpload = true;
                        }

                        //can manage uploaded result edit
                        if (CanEditUploadedResult == false && CanEditUploadedComment == false && CanEditUploadedAttendance == false && CanEditUploadedPsychomotor == false)
                        {
                            CanManageUploadedResultEdit = false;
                        }
                        else
                        {
                            CanManageUploadedResultEdit = true;
                        }

                        //can check result
                        if (CanViewAndPrintStudentResultMasterSheet == false && CanViewAndPrintStudentReportCard == false)
                        {
                            CanCheckResult = false;
                        }
                        else
                        {
                            CanCheckResult = true;
                        }

                        //can manage staff
                        if (CanSetupClassTeacher == false && CanAssignSubjectToTeacher == false)
                        {
                            CanManageStaff = false;
                        }
                        else
                        {
                            CanManageStaff = true;
                        }

                        //can manage department
                        if (CanSetupDepartment == false && CanAssignDepartmentToStaff == false)
                        {
                            CanManageDepartment = false;
                        }
                        else
                        {
                            CanManageDepartment = true;
                        }

                        //can manage news
                        if (CanSetupNews == false && CanSetupTermactivity == false)
                        {
                            CanManageNews = false;
                        }
                        else
                        {
                            CanManageNews = true;
                        }

                        //can manage users
                        if (CanSetupUsers == false && CanSetupRights == false && CanSetupRoles == false && CanSetupAssignRightToRole == false && CanSetupAssignRoleToUser == false && CanResetUserPassword == false)
                        {
                            CanManageUsers = false;
                        }
                        else
                        {
                            CanManageUsers = true;
                        }

                        //can manage sms
                        if (CanSendSMS == false && CanSendMail == false)
                        {
                            CanSendSms = false;
                        }
                        else
                        {
                            CanSendSms = true;
                        }


                    }



                }
            }



        }
    }
}
