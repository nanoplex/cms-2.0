using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace cms.Models
{
    public abstract class Log
    {
        protected static int LogLevel = -1;
        public void SetMessages(int logLevel)
        {
            if (LogLevel == -1)
            {
                LogLevel = logLevel;

                if (LogLevel == 0)
                {

                }
                if (LogLevel <= 1)
                {
                    User.OnUserUnauthorized += (s, e) =>
                    {
                        var user = (User)s;
                        OutputMessage("User " + user.Email + " unauthorized");
                    };
                }
                if (LogLevel <= 2)
                {
                    User.OnUserAuthorized += (s, e) =>
                    {
                        var user = (User)s;
                        OutputMessage("user " + user.Email + " auzthorized");
                    };
                    Page.OnPagePublished += (s, e) =>
                    {
                        var page = (Page)s;
                        OutputMessage(page.Name + " has been publised");
                    };
                }
            }
        }

        protected abstract void OutputMessage(string message);

    }
    public class FileLog : Log
    {
        private static FileLog instance;

        public static FileLog Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileLog();
                }
                return instance;
            }
        }

        protected override void OutputMessage(string message)
        {
            var logFile = File.AppendText(HttpContext.Current.Server.MapPath("\\log.txt"));
            var str = "";

            if (LogLevel == 2)
                str += "< OK > - ";
            else if (LogLevel == 1)
                str += "< WARN > - ";
            else if (LogLevel == 0)
                str += "< ERROR > - ";

            str += message + " - " + DateTime.Now.ToString("dd/MM/yy H:mm:ss") + "\n";

            logFile.Write(str);
            logFile.Close();
        }
    }
    public class EmailLog : Log
    {
        private static EmailLog instance;

        public static EmailLog Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EmailLog();
                }
                return instance;
            }
        }

        protected override void OutputMessage(string message)
        {
            // TODO - send message as an email
        }
    }

}