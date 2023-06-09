using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DISTRESSUITY.Common.Utility
{
    public class MailHelper
    {
        public string ToEmail { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtmlBody { get; set; }
        public bool ReadBodyFromFile { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public List<string> AttachmentPath { get; set; }


        public bool SendEmail()
        {
            try
            {

                StringBuilder strBody = new StringBuilder();
                string MailTo = this.ToEmail;

                MailMessage mailObj = new MailMessage();
                mailObj.From = new MailAddress(ReadConfiguration.FromEmail, ReadConfiguration.FromName);
                mailObj.Subject = this.Subject;
                mailObj.Body = this.Body;
                mailObj.To.Add(this.ToEmail);
                if (this.AttachmentPath != null)
                    foreach (string filePath in this.AttachmentPath)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.Net.Mail.Attachment attachment;
                            attachment = new System.Net.Mail.Attachment(filePath);
                            mailObj.Attachments.Add(attachment);
                        }
                    }
                mailObj.IsBodyHtml = true;
                mailObj.Priority = MailPriority.High;
                SmtpClient SMTPServer = new SmtpClient(ReadConfiguration.HostName);
                SMTPServer.UseDefaultCredentials = false;
                SMTPServer.Port = ReadConfiguration.SmtpServerPort;
                //  SMTPServer.Credentials = auth;
                SMTPServer.Credentials = new System.Net.NetworkCredential(ReadConfiguration.SmtpAccount, ReadConfiguration.SmtpPassword);
                SMTPServer.EnableSsl = ReadConfiguration.EnableSSL;
                SMTPServer.Send(mailObj);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

        }

        public string GenerateMailBody(string path)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Templates/" + path))) //ApproveProject.html"
            {
                body = reader.ReadToEnd();
            }
            //body = body.Replace("{UserName}", userName);
            //body = body.Replace("{Title}", title);
            //body = body.Replace("{Url}", url);
            //body = body.Replace("{Description}", description);
            return body;
            //return "";
        }

    }
}
