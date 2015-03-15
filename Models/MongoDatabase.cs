using System;
using MongoDB.Driver;

namespace mongo
{
    public static class MongoDatabase
    {
        static string _Password = "cws52qbz";
        static string _Username = "mathias";
        static string _Host = "93.160.108.34";
        static string _Database = "cms";

        private static MongoDB.Driver.MongoDatabase Database { get; set; }

        public static MongoDB.Driver.MongoDatabase Init()
        {
            try
            {
                if (Database == null)
                {
                    string connectionString = (_Username != null && _Password != null)
                        ? "mongodb://" + _Username + ":" + _Password + "@" + _Host + "/" + _Database
                        : "mongodb://@" + _Host + "/" + _Database;

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