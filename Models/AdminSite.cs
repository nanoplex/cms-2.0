using mongo;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
namespace cms.Models
{
    public class AdminSite : Site
    {
        public User User { get; set; }
        public Settings Settings { get; set; }
        public List<Component> Components { get; set; }

        public static MongoTable<Site> Db = new MongoTable<Site>();

        public AdminSite()
        {
            try
            {
                var adminSite = Db.Collection().FindOneAs<Site>();

                if (adminSite != null)
                {
                    _id = adminSite._id;
                    Name = adminSite.Name;

                    Pages = Page.Db.Collection().FindAllAs<Page>()
                        .Where(p => p.Visible == true)
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

            Components = new List<Component>();

            foreach (var component in components)
            {
                Components.Add(GetComponent(component, components));
            }
        }

        private Component GetComponent(Type component, List<Type> components)
        {
            var properties = component.GetProperties();
            var selectedProperties = new List<Prop>();

            foreach (var prop in properties)
            {
                var attrs = prop.GetCustomAttributes();
                var type = prop.PropertyType.Name;
                var name = prop.Name;
                var used = true;
                object value = null;

                foreach (var attr in attrs)
                {
                    var attrName = attr.GetType().Name;

                    if (attrName == "unusedAttribute")
                        used = false;

                    if (attrName == "imageAttribute")
                        type = "Image";

                    if (attrName == "textboxAttribute")
                        type = "Textbox";

                }

                if (type == "List`1")
                {
                    type = "List " + prop.PropertyType.GenericTypeArguments[0].Name;
                    value = new string[] { };
                }

                foreach (var c in components)
                {
                    if (type == c.Name)
                    {
                        value = GetComponent(prop.PropertyType, components);
                    }
                }

                if (used)
                {
                    selectedProperties.Add(new Prop
                    {
                        Name = name,
                        Type = type,
                        Value = value
                    });
                }
            }

            return new Component
            {
                Name = component.Name,
                Props = selectedProperties
            };
        }
    }
    public class Component
    {
        public string Name { get; set; }
        public List<Prop> Props { get; set; }
    }
    public class Prop
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }
}