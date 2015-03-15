using System;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mvc.Models;

namespace mvc.Controllers
{
    public class AdminController : Controller
    {
        public User User { get; set; }
        public JsonResult Site()
        {
            return Json(new AdminSite(null), JsonRequestBehavior.AllowGet);
        }

        public bool CheckAuthorization()
        {
            if (Session["user"] != null) return true;
            return false;
        }

        public string Login(string email, string password)
        {
            try
            {
                var user = new User(email, password);
                Session["user"] = user;
                return "true";
            }
            catch (UnauthorizedException ex)
            {
                return ex.Message;
            }
            catch (MongoConnectionException)
            {
                return "database connection error";
            }
        }

        public string SetSettings(bool email, int loglevel)
        {
            try
            {
                var dbSettings = new MongoTable<Settings>();
                var settings = new Settings();
                var Settings = dbSettings.Collection().FindAllAs<Settings>().FirstOrDefault();

                if (Settings == null)
                {
                    settings._id = ObjectId.GenerateNewId(DateTime.Now);
                    settings.Email = email;
                    settings.LogLevel = loglevel;

                    dbSettings.Add(settings);
                }
                else
                {
                    dbSettings.Update(
                        () => Query<Settings>.EQ(s => s._id, dbSettings.Collection().FindOneAs<Settings>()._id),
                        () => Update<Settings>.Set(s => s.Email, settings.Email));

                }
                return "true";
            }
            catch (MongoConnectionException)
            {
                return "database connection error";
            }
        }
    }
}