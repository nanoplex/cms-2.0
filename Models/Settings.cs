using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using mongo;

namespace cms.Models
{
    public class Settings
    {
        public ObjectId _id { get; set; }
        public bool Email { get; set; }
        public int LogLevel { get; set; }

        public static MongoTable<Settings> Db = new MongoTable<Settings>();

        public static event EventHandler OnEmailSettingsChanged;
        public static event EventHandler OnLogLevelChanged;
    }
}