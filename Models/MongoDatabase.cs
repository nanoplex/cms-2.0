using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using cms.Components;
using cms.Models;
using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;

namespace mongo
{
    public static class MongoDatabase
    {
        static string _Password = null;
        static string _Username = null;
        static string _Host = "localhost";
        static string _Database = "cms";

        private static MongoDB.Driver.MongoDatabase Database { get; set; }

        public static MongoDB.Driver.MongoDatabase Init()
        {
            try
            {
                if (Database == null)
                {
                    BsonClassMap.RegisterClassMap<Site>(cm => {
                        cm.AutoMap();
                        cm.MapCreator(s => new Site { _id = s._id, Name = s.Name });
                    });

                    var components = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "mvc.Components").ToList();

                    BsonClassMap.RegisterClassMap<text>(cm => cm.AutoMap());
                    BsonClassMap.RegisterClassMap<image>(cm => cm.AutoMap());
                    BsonClassMap.RegisterClassMap<test>(cm => cm.AutoMap());

                    string connectionString = (_Username != null && _Password != null)
                        ? "mongodb://" + _Username + ":" + _Password + "@" + _Host + "/" + _Database
                        : "mongodb://" + _Host + "/" + _Database;

                    var client = new MongoClient(connectionString);

                    MongoServer server = client.GetServer();

                    Database = server.GetDatabase(_Database);
                }

                return Database;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}