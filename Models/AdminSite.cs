using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;
using cms.Components;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
namespace cms.Models
{
    public class AdminSite : Site
    {
        public User User { get; set; }
        public Settings Settings { get; set; }
        public List<object> Components { get; set; }

        public static MongoTable<Site> Db = new MongoTable<Site>();

        public AdminSite()
        {
            try
            {
                var adminSite = Db.Collection().FindOneAs<Site>();

                Name = adminSite.Name;

                Pages = Page.Db.Collection().FindAllAs<Page>()
                    .Where(p => p.Visibile == true)
                    .OrderByDescending(p => p.Order).ToList();

                User = cms.Models.User.Db.Collection().FindOneAs<User>();

                Settings = cms.Models.Settings.Db.Collection().FindOneAs<Settings>();

                if (Settings != null)
                {
                    FileLog.Instance.SetMessages(Settings.LogLevel);

                    if (Settings.Email)
                        EmailLog.Instance.SetMessages(Settings.LogLevel);
                }

                GetComponents();
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

        private void GetComponents()
        {
            var components = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "cms.Components").Where(t =>
                {
                    var Base = t.BaseType;
                    if (Base.Name == "Component") return true;
                    return false;
                })
                .ToList();

            Components = new List<object>();

            foreach (var component in components)
            {
                var properties = component.GetProperties();
                var selected = new List<object>();

                foreach (var prop in properties)
                {
                    var attrs = prop.GetCustomAttributes();
                    var used = true;
                    var image = false;
                    foreach (var attr in attrs)
                    {
                        if (attr.GetType().Name == "unusedAttribute")
                            used = false;

                        if (attr.GetType().Name == "imageAttribute")
                            image = true;
                    }

                    var type = prop.PropertyType;
                    var name = prop.Name;

                    if (used)
                    {
                        if (image)
                            selected.Add(new
                            {
                                Name = name,
                                Type = "Image"
                            });
                        else
                            selected.Add(new
                            {
                                Name = name,
                                Type = type.Name
                            });
                    }
                }
                Components.Add(new 
                {
                    Name = component.Name,
                    Props = selected 
                });
            }
        }
    }
}