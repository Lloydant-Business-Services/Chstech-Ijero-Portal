using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

using System.Collections.Generic;
using System.Data.OleDb;

namespace Abundance_Nk.Web.Models.Result
{
    public class ExcelReader
    {
        private const string XLS_FILE_EXTENSION = ".xls";
        private const string XLSX_FILE_EXTENSION = ".xlsx";

        private static string excel03ConnString = ConfigurationManager.ConnectionStrings["excelO3ConString"].ToString();
        private static string excel07ConnString = ConfigurationManager.ConnectionStrings["excelO7ConString"].ToString();

        //private string appRootPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        
        public static string Excel03ConnString
        {
            get { return excel03ConnString; }
        }
        public static string Excel07ConnString
        {
            get { return excel07ConnString; }
        }

        public DataTable ReadExcel(string filePath)
        {
            try
            {
                //filePath = appRootPath + filePath;

                string connString = "";
                string fileExtension = GetFileExtension(filePath);

                if (fileExtension == XLS_FILE_EXTENSION)
                {
                    connString = string.Format(Excel03ConnString, filePath);
                }
                else if (fileExtension == XLSX_FILE_EXTENSION)
                {
                    connString = string.Format(Excel07ConnString, filePath);
                }

                OleDbConnection connExcel = new OleDbConnection(connString);
                OleDbCommand cmdExcel = new OleDbCommand();
                OleDbDataAdapter oda = new OleDbDataAdapter();
                DataTable excelData = new DataTable("ExcelData");
                cmdExcel.Connection = connExcel;

                //Get the name of First Sheet
                connExcel.Open();
                DataTable dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                sheetName = sheetName.Replace("''", "'");

                //Read Data from First Sheet
                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                oda.SelectCommand = cmdExcel;
                oda.Fill(excelData);
                connExcel.Close();

                return excelData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetFileExtension(string filePath)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                return fi.Extension;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}