using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DailyRecap.Model;
using XLSExportDemo;


namespace DailyRecap
{
    public partial class DailyRecapService : ServiceBase
    {

        #region Common Variables

        string path;
        string folderName;
        string folderPath;
        string pathString;
        string filepath;

        #endregion Common Variables

        #region DailyRecap Variables
        Guid recapSettingId;
        string recapmail = string.Empty;
        string name = string.Empty;
        string location = string.Empty;
        string locations = string.Empty;
        string tech = string.Empty;
        string techs = string.Empty;
        string category = string.Empty;
        string categories = string.Empty;

        string timezone = string.Empty;
        string action = string.Empty;
        string objects = string.Empty;
        string value = string.Empty;
        string timestamp = string.Empty;
        string locationid = string.Empty;
        DateTime broadcast;
        DateTime starttime;
        DateTime endtime;
        string dayName = string.Empty;
        string sysDayName = string.Empty;
        string brcTime = string.Empty;
        string sysTime = string.Empty;

        string brcTiming = string.Empty;
        DateTime broadcasting;
        #endregion DailyRecap Variables

        #region Snoz Variables
        Guid snozId = Guid.Empty;
        string snozInterval = string.Empty;
        string intervalName = string.Empty;
        string userName = string.Empty;
        string email = string.Empty;
        string phoneNumber = string.Empty;
        DateTime mailTime;
        DateTime startDate;
        DateTime endDate;
        string startTime;
        string endTime;
        string emailTime;
        string systemTime;
        string isSent;

        string subject;
        string content;

        string message;
        #endregion Snoz Variables

        #region EventType
        private System.Diagnostics.EventLogEntryType entryType = EventLogEntryType.Error;
        private System.Diagnostics.EventLogEntryType entryTypeInfo = EventLogEntryType.Information;
        private System.Diagnostics.EventLogEntryType entryTypeWarning = EventLogEntryType.Warning;
        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer intervalTimer = new System.Timers.Timer();
        #endregion

        #region DailyRecapService
        public DailyRecapService()
        {
            InitializeComponent();
            this.ServiceName = "DailyRecapService";

            //DailyRecap Timer
            timer.Enabled = true;
            timer.Interval = 60000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            //Snooze Timer
            intervalTimer.Enabled = true;
            intervalTimer.Interval = 60000;
            intervalTimer.Elapsed += new System.Timers.ElapsedEventHandler(intervalTimer_Elapsed);
        }
        #endregion DailyRecapService

        #region DailyRecap Clear
        public void RecapClear()
        {
            recapSettingId = Guid.Empty;
            recapmail = "";
            name = "";
            location = "";
            locations = "";
            techs = "";
            tech = "";
            timezone = "";
            action = "";
            objects = "";
            category = "";
            categories = "";
            value = "";
            timestamp = "";
            locationid = "";
            dayName = "";
            sysDayName = "";
            brcTime = "";
            sysTime = "";
        }
        #endregion DailyRecap Clear

        #region Snoz Clear
        public void SnozClear()
        {
            Guid snozId = Guid.Empty;
            string snozInterval = string.Empty;
            string intervalName = string.Empty;
            string userName = string.Empty;
            string email = string.Empty;
            string phoneNumber = string.Empty;
            string startTime = string.Empty;
            string endTime = string.Empty;
            string emailTime = string.Empty;
            string systemTime = string.Empty;
            string isSent = string.Empty;

            string subject = string.Empty;
            string head = string.Empty;
            string content = string.Empty;
        }
        #endregion Snoz Clear

        #region Start And Stop
        protected override void OnStart(string[] args)
        {
            //EventLog evntLog = new EventLog("Application", System.Environment.MachineName, "DailyRecapService.LogEntry");
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    evntLog.WriteEntry(ex.ToString(), entryType, 0, (short)105);
            //}
        }

        protected override void OnStop()
        {

        }
        #endregion

        #region DailyRecap Timer Events

        protected void timer_Elapsed(object source, System.Timers.ElapsedEventArgs aa)
        {
            SmtpClient mailClient = new SmtpClient();
            EventLog evntLog = new EventLog("Application", System.Environment.MachineName, "DailyRecapService.LogEntry");
            try
            {
                using (DailyRecapDataContext drContext = new DailyRecapDataContext())
                {
                    //Create Directory for saving .XLS files
                    //CreateDir();

                    StringBuilder sb = new StringBuilder();
                    DateTime dtToday = DateTime.Now;
                    string dayName = dtToday.ToString("ddd").ToUpper();

                    var recapsetting = from rs in drContext.RecapSettings
                                       join rsd in drContext.RecapSettingDays
                                           on rs.RecapSettingId equals rsd.RecapSettingId
                                       where rsd.DayName == dayName
                                       select new
                                       {
                                           rs.RecapSettingId,
                                           rs.Name,
                                           rs.BroadcastTime,
                                           rs.StartTime,
                                           rs.EndTime,
                                           rs.RecapMail,
                                           rs.TimeZone,
                                           rs.Active,
                                           rs.CreateDate,
                                           rs.EditDate
                                       };

                    if (recapsetting != null && recapsetting.Any())
                    {
                        foreach (var feRS in recapsetting)
                        {
                            recapSettingId = feRS.RecapSettingId;
                            recapmail = feRS.RecapMail;
                            name = feRS.Name;
                            broadcasting = DateTime.Parse(feRS.BroadcastTime);
                            brcTiming = broadcasting.ToString("hh:mm tt");
                            starttime = DateTime.Parse(feRS.StartTime);
                            endtime = DateTime.Parse(feRS.EndTime);
                            timezone = feRS.TimeZone;
                            DateTime utcBroadCast = DateTime.Parse(feRS.BroadcastTime);
                            if (timezone == "PST")
                            {
                                timezone = "Pacific Standard Time";
                            }
                            else if (timezone == "EST")
                            {
                                timezone = "Eastern Standard Time";
                            }
                            else if (timezone == "MST")
                            {
                                timezone = "Mountain Standard Time";
                            }
                            else if (timezone == "CST")
                            {
                                timezone = "Central Standard Time";
                            }
                            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                            broadcast = TimeZoneInfo.ConvertTime(utcBroadCast, timeZoneInfo, TimeZoneInfo.Local);

                            brcTime = broadcast.ToString("hh:mm tt");
                            sysTime = dtToday.ToString("hh:mm tt");
                            bool dailyRecapSent = false;

                            var drAudit = from dra in drContext.DailyRecapAudits
                                          join rs in drContext.RecapSettings on dra.RecapSettingId equals rs.RecapSettingId
                                          where Convert.ToDateTime(dra.TimeStamp).Date == DateTime.Now.Date && Convert.ToDateTime(rs.BroadcastTime) <= dtToday
                                          && dra.RecapSettingId == recapSettingId
                                          select new
                                          {
                                              dra.DailyRecapAuditId,
                                              dra.RecapSettingId,
                                              dra.TimeStamp
                                          };

                            if (drAudit != null && drAudit.Any())
                            {
                                dailyRecapSent = true;
                            }//if (drAudit != null && drAudit.Count() > 0)

                            if ((dailyRecapSent == false && DateTime.Now >= Convert.ToDateTime(brcTime)) || brcTime == sysTime)
                            {
                                var dailyRecap = from dr in drContext.DailyRecaps
                                                 join lo in drContext.Locations on dr.LocationId equals lo.LocationId
                                                 join ca in drContext.Categories on dr.CategoryId equals ca.CategoryId
                                                 join us in drContext.Users on dr.TechId equals us.UserId
                                                 join rslo in drContext.RecapSettingLocations on recapSettingId equals rslo.RecapSettingId
                                                 join rsus in drContext.RecapSettingUsers on recapSettingId equals rsus.RecapSettingId
                                                 join rsc in drContext.RecapSettingCategories on recapSettingId equals rsc.RecapSettingId
                                                 where dr.LocationId == rslo.LocationId && dr.TechId == rsus.UserId && dr.CategoryId == rsc.CategoryId && Convert.ToDateTime(dr.TimeStamp) >= starttime && Convert.ToDateTime(dr.TimeStamp) <= endtime
                                                 orderby Convert.ToDateTime(dr.TimeStamp)
                                                 select new
                                                 {
                                                     dr.Action,
                                                     dr.TimeStamp,
                                                     dr.Object,
                                                     dr.Value,
                                                     lo.LocationName,
                                                     us.UserName,
                                                     ca.CategoryName,
                                                 };
                                if (dailyRecap != null && dailyRecap.Any())
                                {
                                    StringBuilder sbChild = new StringBuilder();
                                    ExcelDocument document = new ExcelDocument();
                                    //int i = 1;

                                    foreach (var feDR in dailyRecap)
                                    {
                                        tech = feDR.UserName;
                                        location = feDR.LocationName;
                                        category = feDR.CategoryName;
                                        if (!techs.Contains(tech))
                                        {
                                            if (!string.IsNullOrEmpty(techs))
                                            {
                                                techs += ", " + tech;
                                            }
                                            else
                                            {
                                                techs += tech;
                                            }

                                        }
                                        if (!locations.Contains(location))
                                        {
                                            if (!string.IsNullOrEmpty(locations))
                                            {
                                                locations += ", " + location;
                                            }
                                            else
                                            {
                                                locations += location;
                                            }
                                        }
                                        if (!categories.Contains(category))
                                        {
                                            if (!string.IsNullOrEmpty(categories))
                                            {
                                                categories += ", " + category;
                                            }
                                            else
                                            {
                                                categories += category;
                                            }
                                        }

                                        //Generate Table Structure data
                                        sbChild.Append("<tr><td height=24 align=left valign=middle>");
                                        sbChild.Append(feDR.LocationName);
                                        sbChild.Append("</td><td width=1 align=left valign=middle bgcolor=#000000></td><td align=left valign=middle bgcolor=#FFFFFF>");
                                        sbChild.Append(feDR.Action);
                                        sbChild.Append("</td><td width=1 align=left valign=middle bgcolor=#000000></td><td align=left valign=middle bgcolor=#FFFFFF>");
                                        sbChild.Append(feDR.TimeStamp);
                                        sbChild.Append("</td><td width=1 align=left valign=middle bgcolor=#000000></td><td align=left valign=middle bgcolor=#FFFFFF>");
                                        sbChild.Append(feDR.Object);
                                        sbChild.Append("</td><td width=1 align=left valign=middle bgcolor=#000000></td><td align=left valign=middle bgcolor=#FFFFFF>");
                                        sbChild.Append(feDR.CategoryName);
                                        sbChild.Append("</td><td width=1 align=left valign=middle bgcolor=#000000></td><td align=left valign=middle bgcolor=#FFFFFF>");
                                        sbChild.Append(feDR.Value);
                                        sbChild.Append("</td></tr>");

                                        ////Generate Column for Excel
                                        //document.CodePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
                                        //document.ColumnWidth(0, 120);
                                        //document.ColumnWidth(1, 80);
                                        //document[0, 0].Value = "LOCATIONNAME";
                                        //document[0, 1].Value = "ACTION";
                                        //document[0, 2].Value = "TIMESTAMP";
                                        //document[0, 3].Value = "OBJECT";
                                        //document[0, 4].Value = "CATEGORY";
                                        //document[0, 5].Value = "VALUE";

                                        //if (i <= dailyRecap.Count())
                                        //{
                                        //    document[i, 0].Value = feDR.LocationName;
                                        //    document[i, 1].Value = feDR.Action;
                                        //    document[i, 2].Value = feDR.TimeStamp;
                                        //    document[i, 3].Value = feDR.Object;
                                        //    document[i, 4].Value = feDR.CategoryName;
                                        //    document[i, 5].Value = feDR.Value;
                                        //    i++;
                                        //}
                                    }

                                    ////Create FileName for excel
                                    //string dir = filepath;
                                    //string filename = String.Format(name + ".xls");
                                    //string combined = Path.Combine(dir, filename);
                                    //FileStream stream = new FileStream(combined, FileMode.Create);
                                    //document.Save(stream);
                                    //stream.Close();

                                    //History Records table design
                                    StringBuilder sbHistTop = new StringBuilder();
                                    sbHistTop.Append("<table width=600 border=0 cellspacing=0 cellpadding=0><tr><td width=1 height=1 align=left valign=top bgcolor=#000000></td><td align=left valign=top bgcolor=#000000></td><td width=1 align=left valign=top bgcolor=#000000></td></tr><tr><td align=left valign=top bgcolor=#000000></td><td align=left valign=top><table width=598 border=0 cellspacing=0 cellpadding=0><tr  style=font-family:Arial, Helvetica, sans-serif; font-size:14px; line-height:21px; color:#000000; text-align:left; font-weight:bold; ><td height=22 align=center valign=middle bgcolor=#95b3d7>LOCATIONNAME</td><td width=1 align=center valign=middle bgcolor=#000000></td><td align=center valign=middle bgcolor=#95b3d7>ACTION</td><td width=1 align=center valign=middle bgcolor=#000000></td><td align=center valign=middle bgcolor=#95b3d7>TIMESTAMP </td><td width=1 align=center valign=middle bgcolor=#000000></td><td align=center valign=middle bgcolor=#95b3d7>OBJECT</td><td width=1 align=center valign=middle bgcolor=#000000></td><td align=center valign=middle bgcolor=#95b3d7>CATEGORY</td><td width=1 align=center valign=middle bgcolor=#000000></td><td align=center valign=middle bgcolor=#95b3d7>VALUE</td></tr>");

                                    StringBuilder sbHistBot = new StringBuilder();
                                    sbHistBot.Append("</table></td><td align=left valign=top bgcolor=#000000></td></tr><tr><td align=left valign=top bgcolor=#000000></td><td align=left valign=top bgcolor=#000000></td><td height=1 align=left valign=top bgcolor=#000000></td></tr></table>");

                                    sb.Append("<span style=font-family:Arial;font-size:15pt>Daily Recap Details</span>");
                                    sb.Append("<br /><br />");
                                    sb.Append("<table><tr>");
                                    sb.Append("<td>Name</td><td>:</td><td>");
                                    sb.Append(name);
                                    sb.Append("</td></tr><tr>");
                                    sb.Append("<td>Location(s)</td><td>:</td><td>");
                                    sb.Append(locations);
                                    sb.Append("</td></tr><tr>");
                                    sb.Append("<td>Tech(s)</td><td>:</td><td>");
                                    sb.Append(techs);
                                    sb.Append("</td></tr><tr>");
                                    sb.Append("<td>Category(s)</td><td>:</td><td>");
                                    sb.Append(categories);
                                    sb.Append("</td></tr><tr>");
                                    sb.Append("<td>BroadCast</td><td>:</td><td>");
                                    sb.Append(feRS.BroadcastTime);
                                    sb.Append("</td></tr><tr>");
                                    sb.Append("<td>Start Time</td><td>:</td><td>");
                                    sb.Append(starttime);
                                    sb.Append("</td></tr><tr>");
                                    sb.Append("<td>End Time</td><td>:</td><td>");
                                    sb.Append(endtime);
                                    sb.Append("</td></tr><tr>");
                                    sb.Append("<td>TimeZone</td><td>:</td><td>");
                                    sb.Append(timezone);
                                    sb.Append("</td>");
                                    sb.Append("</tr></table><br />");
                                    sb.Append(sbHistTop.ToString());
                                    sb.Append(sbChild.ToString());
                                    sb.Append(sbHistBot.ToString());
                                    sb.Append("<br /><br />Thanks");
                                    sb.Append("<br /><b>Service Tracking Systems</b>");
                                    sb.Append("<br />clienthappiness@servicetrackingsystems.net");
                                    //sb.Append("<br />M: +173537383833");

                                    //send to email
                                    MailMessage mail = new MailMessage();                                    
                                    //Attachment att = new Attachment(combined);
                                    mail.To.Add(recapmail);
                                    char[] separator = new char[] { ',' };
                                    string connectionInfo = GetConfigurationValue("CC");
                                    string[] strSplitArr = connectionInfo.Split(separator);

                                    foreach (string arrStr in strSplitArr)
                                    {
                                        mail.CC.Add(arrStr);
                                    }

                                    mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                                    mail.Subject = "Daily Recap Report";
                                    mail.Body = sb.ToString();
                                    //mail.Attachments.Add(att);
                                    mail.IsBodyHtml = true;
                                    SmtpClient smtp = new SmtpClient();

                                    smtp.Host = GetConfigurationValue("Host");
                                    smtp.Port = Int32.Parse(GetConfigurationValue("Port"));
                                    smtp.Credentials = new System.Net.NetworkCredential(GetConfigurationValue("UserName"), GetConfigurationValue("Password"));
                                    smtp.EnableSsl = true;
                                    smtp.Send(mail);
                                    DailyRecapAudit objDRAudit = new DailyRecapAudit();
                                    objDRAudit.DailyRecapAuditId = Guid.NewGuid();
                                    objDRAudit.RecapSettingId = recapSettingId;
                                    objDRAudit.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    drContext.DailyRecapAudits.InsertOnSubmit(objDRAudit);
                                    drContext.SubmitChanges();
                                    evntLog.WriteEntry("Mail Sent");
                                    sb.Clear();
                                    sbChild.Clear();
                                    RecapClear();

                                    //Clear DailyRecapAudit table every weak records 
                                    var deldailyadt = (from d in drContext.DailyRecapAudits where Convert.ToDateTime(d.TimeStamp).DayOfWeek < DateTime.Now.DayOfWeek select d);

                                    if (deldailyadt != null)
                                    {
                                        drContext.DailyRecapAudits.DeleteAllOnSubmit(deldailyadt);
                                        drContext.SubmitChanges();
                                    }
                                }//if(dailyRecap!=null)

                            }//if (sysTime == brcTime)
                        }
                    }//if (recapsetting!=null)
                }
            }
            catch (Exception ex)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                mail.CC.Add("magesh.ponnusamy@colanonline.com");
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Prod DailyRecap Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = GetConfigurationValue("Host");
                smtp.Port = Int32.Parse(GetConfigurationValue("Port"));
                smtp.Credentials = new System.Net.NetworkCredential(GetConfigurationValue("UserName"), GetConfigurationValue("Password"));
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }

        #endregion DailyRecap Timer Events

        #region Snooze Timer Event
        protected void intervalTimer_Elapsed(object source, System.Timers.ElapsedEventArgs aa)
        {

            EventLog evntLog = new EventLog("Application", System.Environment.MachineName, "SnoozeIntervalWindowsService.LogEntry");
            try
            {
                using (DailyRecapDataContext snoozIntContext = new DailyRecapDataContext())
                {
                    DateTime dtToday = DateTime.Now;
                    var snozAudit = from sa in snoozIntContext.SnoozeAudits
                                    join snz in snoozIntContext.Snoozes on sa.SnoozeId equals snz.SnoozeId
                                    join inter in snoozIntContext.IntervalTypes on snz.IntervalTypeId equals inter.IntervalTypeId
                                    join usr in snoozIntContext.Users on snz.UserId equals usr.UserId
                                    where Convert.ToDateTime(sa.EmailDate).Date == dtToday.Date 
                                    select new
                                    {
                                        sa.SnoozeId,
                                        sa.StartDate,
                                        sa.EndDate,
                                        sa.EmailDate,
                                        sa.IntervalTime,
                                        sa.IsSent,
                                        usr.UserName,
                                        usr.Email,
                                        usr.Phone,
                                        inter.Name,
                                        snz.SnoozeInterval
                                    };

                    if (snozAudit != null && snozAudit.Any())
                    {

                        foreach (var snoz in snozAudit)
                        {
                            snozId = (Guid)snoz.SnoozeId;
                            snozInterval = Convert.ToString(snoz.SnoozeInterval);
                            intervalName = snoz.Name;
                            email = snoz.Email;
                            userName = snoz.UserName;
                            phoneNumber = snoz.Phone;
                            isSent = snoz.IsSent;
                            startDate = DateTime.Parse(snoz.StartDate);
                            startTime = startDate.ToString("hh:mm tt");

                            endDate = DateTime.Parse(snoz.EndDate);
                            endTime = endDate.ToString("hh:mm tt");

                            mailTime = DateTime.Parse(snoz.EmailDate);
                            emailTime = mailTime.ToString("hh:mm tt");

                            systemTime = dtToday.ToString("hh:mm tt");

                            //Start
                            if ((startTime == systemTime || DateTime.Now >= Convert.ToDateTime(startTime)) && isSent == "start" && isSent != "no" && isSent != "end")
                            {
                                //Email Content and method
                                subject = "STS - ServiceTech - Reminder";
                                content = "You're in Snooze";
                                SendEmail(subject, content);

                                //SMS Content and method
                                message = "Hey " + userName + ", Your Snooze from " + startDate + " to " + endDate + " starts now";
                                SendSMS(message, phoneNumber); 

                            }

                            //Remainder
                            else if ((emailTime == systemTime || DateTime.Now >= Convert.ToDateTime(emailTime)) && isSent == "no" && isSent != "start" && isSent != "end")
                            {
                                //Email Content and method
                                subject = "STS - ServiceTech - Reminder";
                                content = "You're in Snooze";
                                SendEmail(subject, content);

                                //SMS Content and method
                                message = "Hey " + userName + ", You have Snooze from " + startDate + " to " + endDate;
                                SendSMS(message, phoneNumber);

                            }

                            //Expiry
                            else if ((systemTime == endTime || DateTime.Now >= Convert.ToDateTime(endTime)) && isSent == "end" && isSent != "start" && isSent != "no")
                            {
                                //Email Content and method
                                subject = "STS - ServiceTech - Snooze is Up";
                                content = "Your Snooze is up";
                                SendEmail(subject, content);

                                //SMS Content and method
                                message = "Hey " + userName + ", Your Snooze from " + startDate + " to " + endDate + " is up";
                                SendSMS(message, phoneNumber);

                                //Clear SnoozeAudit table after sending email & sms
                                var delsnzadt = (from sa in snoozIntContext.SnoozeAudits where sa.SnoozeId == snozId select sa);

                                if (delsnzadt != null)
                                {
                                    snoozIntContext.SnoozeAudits.DeleteAllOnSubmit(delsnzadt);
                                    snoozIntContext.SubmitChanges();
                                }

                            }

                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                //mail.CC.Add("magesh.ponnusamy@colanonline.com");
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Snooze Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = GetConfigurationValue("Host");
                smtp.Port = Int32.Parse(GetConfigurationValue("Port"));
                smtp.Credentials = new System.Net.NetworkCredential(GetConfigurationValue("UserName"), GetConfigurationValue("Password"));
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }
        #endregion Snooze Timer Event

        #region Email & SMS Methods
        public void SendEmail(string subject, string content)
        {
            SmtpClient mailClient = new SmtpClient();
            try
            {
                using (DailyRecapDataContext snoozIntContext = new DailyRecapDataContext())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Hey " + userName);
                    sb.Append("<br /><br />");
                    sb.Append(content);
                    sb.Append("<br /><br />");
                    sb.Append("<table><tr>");
                    sb.Append("<td>SnoozeID</td><td>:</td><td>");
                    sb.Append(snozId);
                    sb.Append("</td></tr><tr>");
                    sb.Append("<td>SnoozeInterval</td><td>:</td><td>");
                    sb.Append(snozInterval + " " + intervalName);
                    sb.Append("</td></tr><tr>");
                    sb.Append("<td>StartDate</td><td>:</td><td>");
                    sb.Append(startDate);
                    sb.Append("</td></tr><tr>");
                    sb.Append("<td>EndDate</td><td>:</td><td>");
                    sb.Append(endDate);
                    sb.Append("</td></tr><tr>");
                    sb.Append("<td>Email</td><td>:</td><td>");
                    sb.Append(email);
                    sb.Append("</td></tr><tr>");
                    sb.Append("<td>End Time</td><td>:</td><td>");
                    sb.Append(endTime);
                    sb.Append("</td></tr><tr>");
                    sb.Append("</tr></table>");
                    sb.Append("<br /><br />Thanks");
                    sb.Append("<br /><b>Service Tracking Systems</b>");
                    sb.Append("<br />clienthappiness@servicetrackingsystems.net");
                    //sb.Append("<br />M: +173537383833");

                    MailMessage mail = new MailMessage();
                    mail.To.Add(email);

                    char[] separator = new char[] { ',' };
                    string connectionInfo = GetConfigurationValue("CC");
                    string[] strSplitArr = connectionInfo.Split(separator);

                    foreach (string arrStr in strSplitArr)
                    {
                        mail.CC.Add(arrStr);
                    }

                    mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                    mail.Subject = subject;
                    mail.Body = sb.ToString();
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = GetConfigurationValue("Host");
                    smtp.Port = Int32.Parse(GetConfigurationValue("Port"));
                    smtp.Credentials = new System.Net.NetworkCredential(GetConfigurationValue("UserName"), GetConfigurationValue("Password"));
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                    var snz = (from c in snoozIntContext.SnoozeAudits
                               where c.SnoozeId == snozId && Convert.ToDateTime(c.EmailDate) == mailTime
                               select c).FirstOrDefault();

                    if (snz != null)
                    {
                        snz.IsSent = "yes";
                        snz.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        snoozIntContext.SubmitChanges();
                    }
                    SnozClear();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SendSMS(string msg, string ph)
        {
            SmtpClient mailClient = new SmtpClient();
            string msgg = string.Empty;

            try
            {
                string strurl = "https://www.primemessage.net/TxTNotify/TxTNotify?PhoneDestination=" + ph + "&Message=" + msg + "&CustomerNickname=VALET&Username=valetplease&Password=vp0209";
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(strurl, false));
                HttpWebResponse httpResponse = (HttpWebResponse)(httpReq.GetResponse());
                msgg = httpResponse.StatusCode.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion Email & SMS Methods        

        #region Read App.Config

        private static string GetConfigurationValue(string key)
        {
            var service = Assembly.GetAssembly(typeof(ProgramInstaller));
            Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] == null)
            {
                throw new IndexOutOfRangeException("Settings collection does not contain the requested key:" + key);
            }

            return config.AppSettings.Settings[key].Value;
        }

        #endregion Read App.Config

        #region Create Directory
        //public string CreateDir()
        //{
        //    folderName = GetConfigurationValue("FolderName");
        //    path = AppDomain.CurrentDomain.BaseDirectory;
        //    folderPath = @path + folderName;

        //    if (Directory.Exists(folderPath))
        //    {
        //        filepath = folderPath;
        //    }
        //    else
        //    {
        //        pathString = Path.Combine(folderPath);
        //        Directory.CreateDirectory(pathString);
        //        filepath = pathString;
        //    }
        //    return filepath;
        //}
        #endregion Create Directory

    }
}
