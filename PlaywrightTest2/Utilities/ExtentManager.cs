using System;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

namespace PlaywrightTest2.Utilities
{
    public static class ExtentManager
    {
        private static ExtentReports? _extent;

        /// <summary>
        /// Singleton ExtentReports instance.
        /// Report:      {ProjectRoot}/TestResults/ExtentReport.html
        /// Screenshots: {ProjectRoot}/TestResults/Screenshots/ (PNG copies kept for CI artifacts)
        /// Step/failure images are embedded inline as Base64 so the HTML is self-contained
        /// when the report is emailed as a standalone attachment.
        /// </summary>
        public static ExtentReports GetInstance()
        {
            if (_extent == null)
            {
                string root = PathHelper.GetRoot();

                if (!Directory.Exists(root))
                    Directory.CreateDirectory(root);

                string reportPath = Path.Combine(root, "ExtentReport.html");

                var spark = new ExtentSparkReporter(reportPath);
                spark.Config.DocumentTitle = "Automation Report";
                spark.Config.ReportName = "UI Test Execution";
                spark.Config.Theme = Theme.Standard;

                _extent = new ExtentReports();
                _extent.AttachReporter(spark);

                string env = ExcelHelper.GetCell("Environment", 2, 1);

                _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
                _extent.AddSystemInfo("Env", env);
                _extent.AddSystemInfo("Browser", "Chromium (Playwright)");
                _extent.AddSystemInfo("Tester", "Ali");
                _extent.AddSystemInfo("Build", "v1.0");
            }

            return _extent;
        }
    }
}
