using mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using mvc.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace mvc.Controllers
{
    public class InstallController : Controller
    {

        public string SetUser(string name, string email, string password)
        {
            try
            {
                var dbUser = new MongoTable<User>();
                var user = new User();

                if (dbUser.Collection().FindAll().FirstOrDefault() == null)
                {
                    var salt = Hash.GetRandomSalt(16);

                    user._id = ObjectId.GenerateNewId(DateTime.Now);
                    user.Name = name;
                    user.Email = email;
                    user.Password = Hash.HashPassword(password, salt);
                    user.Salt = salt;

                    dbUser.Add(user);
                    return "true";
                }
                return "you already created a user";
            }
            catch (MongoConnectionException)
            {
                return "database connection error";
            }
        }


        public string SetSite(string name, int loglevel, bool email)
        {
            try
            {
                var dbSite = new MongoTable<Site>();
                var dbSettings = new MongoTable<Settings>();

                var site = new Site();
                var settings = new Settings();

                site.Name = name;
                settings.LogLevel = loglevel;
                settings.Email = email;

                dbSite.Add(site);
                dbSettings.Add(settings);
                return "true";
            }
            catch (MongoConnectionException)
            {
                return "database connection error";
            }
        }
    }
}