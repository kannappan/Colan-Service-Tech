using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Collections.Specialized;
using System.Net.Mime;
using System.Configuration;

namespace ServicePleaseServiceLibrary.Helpers
{
    public class EmailHelper
    {
        public static void SendMessageWithAttachment(string fromAddress,
                                                     StringCollection toAddresses,
                                                     StringCollection ccAddresses,
                                                     StringCollection bccAddresses,
                                                     string subjectText,
                                                     string bodyText,
                                                     string smtpServerAddress,
                                                     string smtpUserName,
                                                     string smtpPassword)
        {
            Attachment attachmentData = null;

            try
            {
                MailMessage message = new MailMessage();

                message.IsBodyHtml = true;

                message.From = new MailAddress(fromAddress);

                message.To.Clear();

                if (toAddresses != null && toAddresses.Count > 0)
                {
                    foreach (string address in toAddresses)
                    {
                        message.To.Add(new MailAddress(address));
                    }
                }

                message.CC.Clear();

                if (ccAddresses != null && ccAddresses.Count > 0)
                {
                    foreach (string address in ccAddresses)
                    {
                        message.CC.Add(new MailAddress(address));
                    }
                }

                message.Bcc.Clear();

                if (bccAddresses != null && bccAddresses.Count > 0)
                {
                    foreach (string address in bccAddresses)
                    {
                        message.Bcc.Add(new MailAddress(address));
                    }
                }

                message.Subject = subjectText;

                message.Body = bodyText;

                //Send the message.
                int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);

                var client = new SmtpClient(smtpServerAddress, smtpPort)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpUserName, smtpPassword),
                    EnableSsl = true
                };

                client.Send(message);
            }
            catch (SmtpException sex)
            {
                Logger.Write("SMTP Exception caught in SendMessageWithAttachment(): {0}",
                             sex.Message);
            }
            catch (Exception ex)
            {
                Logger.Write("Exception caught in SendMessageWithAttachment(): {0}",
                             ex.Message);
            }
            finally
            {
                if (attachmentData != null)
                {
                    attachmentData.Dispose();
                }
            }
        }
    }
}
