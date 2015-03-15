using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mvc.Models
{
    public class Site
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        protected Settings Settings { get; set; }
        public List<Page> Pages { get; set; }

        private MongoTable<Site> DbSite = new MongoTable<Site>();

        public Site()
        {
            
        }

        public Site(string t)
        {
            if (Settings != null)
            {
                new FileLog().SetMessages(Settings.LogLevel);
                if (Settings.Email) new EmailLog().SetMessages(Settings.LogLevel);
            }

            try
            {
                var site = DbSite.Collection().FindAllAs<Site>().FirstOrDefault();
                Name = site.Name;
                if (site.Pages != null)
                    Pages = site.Pages.Where(p => p.Visibile == true).ToList();
            }
            catch (NullReferenceException)
            {
                Name = "Site does not exist";
            }
            catch (MongoConnectionException)
            {
                Name = "Server could not connect to database";
            }

        }
    }

}