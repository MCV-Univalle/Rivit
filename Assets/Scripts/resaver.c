using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace resaver
{
  class Program
  {
    static void Main(string[] args)
    {
      string srcFile = Path.GetFullPath(args[0]);
      Excel.Application excelApplication = new Excel.Application();
      excelApplication.Application.DisplayAlerts = false;
      Excel.Workbook srcworkBook = excelApplication.Workbooks.Open(srcFile);
      srcworkBook.Save();
      srcworkBook.Close();
      excelApplication.Quit();
    }
  }
}