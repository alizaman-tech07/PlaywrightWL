using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace PlaywrightTest2.Utilities
{
    /// <summary>
    /// Sends the ExtentReport HTML as an email attachment.
    ///
    /// SECURITY: Credentials are NEVER hardcoded. They are read from environment variables.
    /// TODO: Configure the following before enabling email delivery:
    ///   REPORT_EMAIL_FROM        - sender address (e.g. automation@yourdomain.com)
    ///   REPORT_EMAIL_APP_PASSWORD- app password for the sender account (Gmail: 16-digit app password)
    ///   REPORT_EMAIL_TO          - recipient address
    ///   REPORT_EMAIL_SMTP_HOST   - optional, defaults to smtp.gmail.com
    ///   REPORT_EMAIL_SMTP_PORT   - optional, defaults to 587
    ///
    /// Call site remains commented out in BaseTest.EndReport() during migration — enable
    /// only after the env vars are provisioned in your CI/local shell.
    /// </summary>
    public static class EmailHelper
    {
        public static void SendReport(string filePath)
        {
            string? fromEmail = Environment.GetEnvironmentVariable("REPORT_EMAIL_FROM");
            string? appPassword = Environment.GetEnvironmentVariable("REPORT_EMAIL_APP_PASSWORD");
            string? toEmail = Environment.GetEnvironmentVariable("REPORT_EMAIL_TO");
            string smtpHost = Environment.GetEnvironmentVariable("REPORT_EMAIL_SMTP_HOST") ?? "smtp.gmail.com";
            int smtpPort = int.TryParse(Environment.GetEnvironmentVariable("REPORT_EMAIL_SMTP_PORT"), out var p) ? p : 587;

            if (string.IsNullOrWhiteSpace(fromEmail) ||
                string.IsNullOrWhiteSpace(appPassword) ||
                string.IsNullOrWhiteSpace(toEmail))
            {
                Console.WriteLine("Email skipped: REPORT_EMAIL_FROM / REPORT_EMAIL_APP_PASSWORD / REPORT_EMAIL_TO not configured.");
                return;
            }

            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Automation Test Report - " + DateTime.Now.ToString("yyyy-MM-dd"),
                    Body = "Hi team,\n\nPlease find the attached Automation Execution Report."
                };
                mail.To.Add(toEmail);

                if (File.Exists(filePath))
                {
                    mail.Attachments.Add(new Attachment(filePath));
                }

                using var smtp = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, appPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                smtp.Send(mail);
                Console.WriteLine("Email successfully sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email send fail: " + ex.Message);
            }
        }
    }
}
