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
        public List<Component> Components { get; set; }

        public static MongoTable<Site> Db = new MongoTable<Site>();

        public AdminSite()
        {
            try
            {
                var adminSite = Db.Collection().FindOneAs<Site>();

                _id = adminSite._id;
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

                var used = true;
                var image = false;
                var textbox = false;
                var typeComponent = false;

                foreach (var attr in attrs)
                {
                    var attrName = attr.GetType().Name;

                    if (attrName == "unusedAttribute")
                        used = false;

                    if (attrName == "imageAttribute")
                        image = true;

                    if (attrName == "textboxAttribute")
                        textbox = true;
                }

                if (used)
                {

                    var name = prop.Name;
                    var type = prop.PropertyType.Name;
                    object value = null;

                    if (type == "List`1")
                    {
                        type = "List " + prop.PropertyType.GenericTypeArguments[0].Name;
                        value = new string[]{};
                    }

                    foreach (var c in components)
                    {
                        if (type == c.Name)
                            typeComponent = true;
                    }

                    if (image)
                    {
                        type = "Image";
                    }
                    else if (textbox)
                    {
                        type = "Textbox";
                    }
                    else if (typeComponent)
                    {
                        type = "Component";
                        value = GetComponent(prop.PropertyType, components);
                    }

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