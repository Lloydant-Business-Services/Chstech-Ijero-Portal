using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;


namespace Abundance_Nk.Business
{
    public class ExcelManager : IExcelManager
    {
        public MemoryStream DownloadExcel<T>(List<T> list) where T : class
        {

            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                if (type.Name == "String")
                {
                    dataTable.Columns.Add(prop.Name, type);
                }
                else if (type.Name == "Int32")
                {
                    dataTable.Columns.Add(prop.Name, typeof(int));
                }
                else if (type.Name == "Decimal")
                {
                    dataTable.Columns.Add(prop.Name, typeof(decimal));
                }

            }
            foreach (var item in list)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable

            MemoryStream result = new MemoryStream();
            ExcelPackage excelpack = new ExcelPackage();
            ExcelWorksheet worksheet = excelpack.Workbook.Worksheets.Add("Sheet1");
            int col = 1;
            int row = 1;
            foreach (DataColumn column in dataTable.Columns)
            {
                worksheet.Cells[row, col].Value = column.ColumnName;
                col++;
            }
            col = 1;
            row = 2;
            foreach (DataRow rw in dataTable.Rows)
            {
                foreach (DataColumn cl in dataTable.Columns)
                {
                    if (rw[cl.ColumnName] != DBNull.Value)
                        worksheet.Cells[row, col].Value = rw[cl.ColumnName];
                    col++;
                }
                row++;
                col = 1;
            }
            excelpack.SaveAs(result);

            return result;
        }


        public DataSet ReadExcel(string filePath)
        {
            DataSet dataSet = new DataSet();
            string[] ext = filePath.Split('.');
            string xConnStr = null;
            string exten = Path.GetExtension(filePath);
            if (exten == ".xlsx")
            {

                xConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" +
                             "Extended Properties=Excel 12.0;";
            }
            else if (exten == ".xls")
            {
                xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" +
                             "Extended Properties=Excel 8.0;";

            }

            if (!string.IsNullOrEmpty(xConnStr))
            {
                OleDbConnection connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow dataRow in sheet.Rows)
                {
                    string sheetName = dataRow[2].ToString();

                    OleDbCommand command = new OleDbCommand("Select * FROM [" + sheetName + "]", connection);
                    // Create DbDataReader to Data Worksheet

                    OleDbDataAdapter MyData = new OleDbDataAdapter();
                    MyData.SelectCommand = command;

                    dataSet.Clear();
                    dataSet.DataSetName = sheetName;
                    MyData.Fill(dataSet);
                    connection.Close();
                }

            }
            return dataSet;
        }
    }
}
