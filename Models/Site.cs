using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using cms.Components;
using System.Reflection;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization.Attributes;

namespace cms.Models
{
    public class Site
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        protected Settings Settings { get; set; }
        public List<Page> Pages { get; set; }

        public static MongoTable<Site> Db = new MongoTable<Site>();

        [BsonConstructor]
        public Site() { }

        public Site(int? CONTRUCTOR_FILLER)
        {
            try
            {
                Settings = Settings.Db.Collection().FindOneAs<Settings>();

                if (Settings != null)
                {
                    FileLog.Instance.SetMessages(Settings.LogLevel);

                    if (Settings.Email)
                        EmailLog.Instance.SetMessages(Settings.LogLevel);
                }

                var site = Db.Collection().FindAllAs<Site>().FirstOrDefault();

                var components = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.Namespace == "mvc.Components")
                    .ToList();

                Name = site.Name;

                Pages = Page.Db.Collection().FindAllAs<Page>().OrderByDescending(p => p.Order).ToList();
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