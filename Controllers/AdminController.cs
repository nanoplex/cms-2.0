using cms.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;

namespace cms.Controllers
{
    public class AdminController : Controller
    {
        public static User User { get; set; }
        public static AdminSite site = new AdminSite();

        [HttpGet]
        public JsonResult Site()
        {
            try
            {
                // list of pages with id as string;
                var pages = new List<object>();

                site.Pages = GetPages();

                site.Pages.ForEach((p) =>
                {
                    pages.Add(new
                    {
                        _id = p._id.ToString(),
                        Name = p.Name,
                        PublishDate = p.PublishDate,
                        Order = p.Order,
                        Visibile = p.Visibile,
                        Components = p.Components
                    });
                });

                return Auth(Json(new
                {
                    Name = site.Name,
                    Settings = site.Settings,
                    Pages = pages,
                    User = site.User,
                    Components = site.Components
                },
                    JsonRequestBehavior.AllowGet));
            }
            catch (MongoConnectionException)
            {
                return Auth(Json(new
                {
                    Name = "Database Connection Error"
                }, JsonRequestBehavior.AllowGet));
            }
        }

        private List<Page> GetPages()
        {
            return Page.Db.Collection().FindAllAs<Page>()
                .Where(p => p.Visibile == true)
                .OrderByDescending(p => p.Order)
                .ToList();
        }

        [HttpGet]
        public bool CheckAuthorization()
        {
            if (Session["user"] != null)
                return true;
            return false;
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
                page.Visibile = true;

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
                    var c = Cast(com, new
                    {
                        Name = "",
                        Props = new List<dynamic>()
                    });

                    if (c.Name == componentName)
                    {
                        var type = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.Namespace == "cms.Components")
                            .Where(comp => comp.Name == componentName)
                            .FirstOrDefault();

                        var images = new List<string>();
                        foreach (var prop in c.Props)
                        {
                            if (prop.Type == "Image")
                            {
                                images.Add(prop.Name);
                            }
                        }

                        dynamic obj = JsonConvert.DeserializeObject(component, type);
                        obj._id = ObjectId.GenerateNewId();

                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var image = Image.FromStream(Request.Files[i].InputStream);
                            var path = "\\uploads\\images\\" + obj._id + i + ".jpeg";

                            image.Save(Server.MapPath(path), ImageFormat.Jpeg);
                        }

                        if (page.Components == null)
                            page.Components = new List<object>();

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