using System;
using System.IO;
using OfficeOpenXml;

namespace PlaywrightTest2.Utilities
{
    // EPPlus 6.2.0 pulls in System.Security.Cryptography.Pkcs 7.0.0 which has advisory
    // GHSA-555c-2p6r-68mm (NU1903). Pinned to match the Selenium project for parity;
    // revisit the EPPlus version after the migration is stable.
    public static class ExcelHelper
    {
        private static readonly string _path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "TestData",
            "TestData.xlsx");

        public static string GetCell(string sheet, int row, int col)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(_path));
            return package.Workbook.Worksheets[sheet].Cells[row, col].Text;
        }
    }
}
