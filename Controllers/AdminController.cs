using cms.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace cms.Controllers
{
    public class AdminController : Controller
    {
        public static User User { get; set; }
        public static AdminSite site = new AdminSite();

        [HttpGet]
        public string Site()
        {
            if (site == null)
                site = new AdminSite();

            site.Pages = GetPages();

            var str = BsonExtensionMethods.ToJson<AdminSite>(site);

            str = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(
                    str, 
                    @":\s*ObjectId\(", ":", RegexOptions.Multiline),
                    @":\s*ISODate\(", ":", RegexOptions.Multiline), 
                    @"System.String\[", "[", RegexOptions.Multiline), 
                    @"\)\s*,", ",", RegexOptions.Multiline), 
                    @"\)\s*}", "}", RegexOptions.Multiline);

            return str;
        }

        private List<Page> GetPages()
        {
            return Page.Db.Collection().FindAllAs<Page>()
                .Where(p => p.Visible == true)
                .OrderByDescending(p => p.Order)
                .ToList();
        }

        [HttpGet]
        public bool CheckAuthorization()
        {
            if (Session["user"] != null) return true;
            else
            {
                site = null;
                return false;
            }
        }

        [HttpPost]
        public string Login(string email, string password)
        {
            try
            {
                var user = new User(email, password);

                Session["user"] = user;

                return "true";
            }
            catch (User.UnauthorizedException ex)
            {
                return ex.Message;
            }
            catch (MongoConnectionException)
            {
                return "database connection error";
            }
        }

        [HttpPost]
        public string SetSettings(bool email, int loglevel)
        {
            try
            {
                var existingSettings = Settings.Db.Collection().FindOneAs<Settings>();

                if (existingSettings == null)
                {
                    var settings = new Settings();

                    settings._id = ObjectId.GenerateNewId(DateTime.Now);
                    settings.Email = email;
                    settings.LogLevel = loglevel;

                    Settings.Db.Add(settings);
                }
                else
                {
                    Settings.Db.Update(
                        () => Query<Settings>.EQ(s => s._id, existingSettings._id),
                        () => Update<Settings>.Set(s => s.Email, email));
                }
                return "true";
            }
            catch (MongoConnectionException)
            {
                return "database connection error";
            }
        }

        [HttpPost]
        public bool AddPage(string name, int order)
        {
            try
            {
                var page = new Page();

                page.Name = name;
                page.Order = order;
                page.PublishDate = DateTime.Now;
                page.Visible = true;

                Page.Db.Add(page);

                return Auth(true);
            }
            catch (MongoConnectionException)
            {
                return Auth(false);
            }
        }

        [HttpPost]
        public bool EditPage(string id, string name, int? order)
        {
            try
            {
                var Id = new ObjectId(id);

                Page.Db.Update(
                    () => Query<Page>.EQ(p => p._id, Id),
                    () => Update<Page>
                        .Set(p => p.Name, name)
                        .Set(p => p.Order, (int)order));

                return Auth(true);
            }
            catch (MongoConnectionException)
            {
                return Auth(false);
            }
        }

        [HttpPost]
        public bool DeletePage(string id)
        {
            try
            {
                var Id = new ObjectId(id);

                Page.Db.Delete(() => Query<Page>.EQ(p => p._id, Id));

                return Auth(true);
            }
            catch (MongoConnectionException)
            {
                return Auth(false);
            }
        }

        [HttpPost]
        public bool AddContent(string component, string componentName, string pageName)
        {
            try
            {
                var page = Page.Db.Collection()
                    .FindOneAs<Page>(Query<Page>.EQ(p => p.Name, pageName));

                foreach (var com in site.Components)
                {
                    var c = Cast(com, new Component
                    {
                        Name = "",
                        Props = new List<Prop>()
                    });

                    if (c.Name == componentName)
                    {

                        if (page.Components == null)
                            page.Components = new List<object>();

                        dynamic obj = page.parseComponent(
                            componentName,
                            c.Props,
                            component,
                            Request.Files);

                        page.Components.Add(obj);
                    }
                }

                Page.Db.Update(
                    () => Query<Page>.EQ(p => p._id, page._id),
                    () => Update<Page>.Set(p => p.Components, page.Components));

                return Auth(true);
            }
            catch (MongoConnectionException)
            {
                return Auth(false);
            }
        }

        private static T Cast<T>(Object x, T typeHolder)
        {
            return (T)x;
        }

        public JsonResult Auth(JsonResult res)
        {
            if (Session["user"] != null)
                return res;
            return null;
        }
        public bool Auth(bool res)
        {
            if (Session["user"] != null)
                return res;
            return false;
        }
    }
}