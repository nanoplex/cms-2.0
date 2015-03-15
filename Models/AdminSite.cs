using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mvc.Models
{
    public class AdminSite : Site
    {
        public User User { get; set; }
        public Settings Settings { get; set; }

        private static MongoTable<Site> DbAdminSite = new MongoTable<Site>();

        public AdminSite()
        {

        }

        public AdminSite(string t)
        {
            var adminSite = DbAdminSite.Collection().FindAllAs<Site>().FirstOrDefault();

            Name = adminSite.Name;
            Pages = adminSite.Pages;
            User = new MongoTable<User>().Collection().FindAllAs<User>().FirstOrDefault();
            Settings = new MongoTable<Settings>().Collection().FindAllAs<Settings>().FirstOrDefault();
        }
    }
}