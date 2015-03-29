using mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using cms.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace cms.Controllers
{
    public class InstallController : Controller
    {
        public string SetUser(string name, string email, string password)
        {
            try
            {
                var user = new User();

                if (cms.Models.User.Db.Collection().FindAll().FirstOrDefault() == null)
                {
                    var salt = Hash.GetRandomSalt(16);

                    user._id = ObjectId.GenerateNewId(DateTime.Now);
                    user.Name = name;
                    user.Email = email;
                    user.Password = Hash.HashPassword(password, salt);
                    user.Salt = salt;

                    cms.Models.User.Db.Add(user);
                    return "true";
                }
                return "you already created a user";
            }
            catch (MongoConnectionException)
            {
                return "database connection error";
            }
        }


        public string SetSite(string name, int loglevel, bool email = false)
        {
            try
            {
                var site = new Site();
                var settings = new Settings();

                site.Name = name;

                settings.LogLevel = loglevel;
                settings.Email = email;

                Site.Db.Add(site);
                Settings.Db.Add(settings);

                return "true";
            }
            catch (MongoConnectionException ex)
            {
                return "database connection error";
            }
        }
    }
}