//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Abundance_Nk.Model.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class VW_APPLICANT_DETAILS_HND_ALT
    {
        public long Person_Id { get; set; }
        public string Name { get; set; }
        public int O_Level_Type_Id { get; set; }
        public string O_Level_Type_Short_Name { get; set; }
        public int O_Level_Subject_Id { get; set; }
        public string O_Level_Subject_Name { get; set; }
        public int O_Level_Grade_Id { get; set; }
        public string O_Level_Grade_Name { get; set; }
        public long Application_Form_Id { get; set; }
        public string Application_Form_Number { get; set; }
        public int Department_Id { get; set; }
        public string Department_Code { get; set; }
        public string Department_Name { get; set; }
        public int Programme_Id { get; set; }
        public string Programme_Short_Name { get; set; }
        public string Programme_Name { get; set; }
        public long Applicant_O_Level_Result_Id { get; set; }
        public string Exam_Number { get; set; }
        public int Exam_Year { get; set; }
        public int O_Level_Exam_Sitting_Id { get; set; }
        public string O_Level_Exam_Sitting_Name { get; set; }
        public long Applicant_O_Level_Result_Detail_Id { get; set; }
        public int Session_Id { get; set; }
        public string Session_Name { get; set; }
        public string Image_File_Url { get; set; }
        public string O_Level_Type_Name { get; set; }
        public string Scanned_Copy_Url { get; set; }
        public string Last_Name { get; set; }
        public string First_Name { get; set; }
        public string Other_Name { get; set; }
        public string Mobile_Phone { get; set; }
        public string Email { get; set; }
        public string Home_Address { get; set; }
        public Nullable<System.DateTime> Date_Of_Birth { get; set; }
        public string Sex_Name { get; set; }
        public string Level_of_Pass { get; set; }
        public string IT_Duration_Name { get; set; }
        public System.DateTime Previous_Education_End_Date { get; set; }
        public string Previous_School_Name { get; set; }
        public Nullable<int> Department_Option_Id { get; set; }
        public string Department_Option_Name { get; set; }
    }
}
