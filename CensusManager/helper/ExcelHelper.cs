using CensusManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 人口普查
{
    public delegate void ExcelParseCallback(System.Data.DataTable sheet);
    public class ExcelHelper
    {
        public static void ParseExcel(string excelFilePath, string sheetName, ExcelParseCallback excelParseCallback)
        {
            OleDbConnection myConn = null;
            try
            {

                string connectString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=No;IMEX=1;'", excelFilePath);
                myConn = new OleDbConnection(connectString);//建立链接

                DataSet ds = new DataSet();
                new OleDbDataAdapter("Select * from [" + sheetName + "$]", myConn).Fill(ds, sheetName);

                //
                var sheet = ds.Tables[sheetName];

                excelParseCallback(sheet);

            }
            catch (Exception)
            {
                MessageBox.Show("导入数据错误，请注意格式。");
            }
            finally
            {
                if (myConn != null)
                    myConn.Close();
            }
            //
        }

    }
}
