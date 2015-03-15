using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc.Models
{
    public abstract class Log
    {
        public void SetMessages(int logLevel)
        {

            if (logLevel == 0)
            {

            }
            if (logLevel <= 1)
            {
                User.OnUserUnauthorized += (s, e) =>
                {
                    var user = (User)s;
                    OutputMessage("User " + user.Email + " unauthorized", 1);
                };
            }
            if (logLevel <= 2)
            {
                User.OnUserAuthorized += (s, e) =>
                {
                    var user = (User)s;
                    OutputMessage("User " + user.Email + " logged in", 0);
                };
                Page.OnPagePublished += (s, e) =>
                {
                    var page = (Page)s;
                    OutputMessage(page.Name + " has been publised", 0);
                };
            }
        }

        protected abstract void OutputMessage(string message, int level);

    }
    public class FileLog : Log
    {
        protected override void OutputMessage(string message, int level)
        {
            // TODO - add message to logfile
        }
    }
    public class EmailLog : Log
    {
        protected override void OutputMessage(string message, int level)
        {
            // TODO - send message as an email
        }
    }

}