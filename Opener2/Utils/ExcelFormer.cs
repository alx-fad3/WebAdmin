using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.Data.SqlClient;
using System.Configuration;
using Opener2.Models;

namespace Opener2.Utils
{
	public static class ExcelFormer
	{
		public static string RelatedProductListToExcel(List<RelatedProduct> inputList, string filePath)
		{
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                //add all the content from the List<> collection, starting at cell A1. 
                //second parameter = use class properties names as headers, default = false
                worksheet.Cells["A1"].LoadFromCollection(inputList, true);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var reportFile = new System.IO.FileInfo(filePath);
                excelPackage.SaveAs(reportFile);

                return reportFile.FullName;
            }
        }
        public static string RelatedProductListToExcel(List<Agreement> inputList, string filePath)
		{
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                //add all the content from the List<> collection, starting at cell A1. 
                //second parameter = use class properties names as headers, default = false
                worksheet.Cells["A1"].LoadFromCollection(inputList, true);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var reportFile = new System.IO.FileInfo(filePath);
                excelPackage.SaveAs(reportFile);

                return reportFile.FullName;
            }
        }
    }
}