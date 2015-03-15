using System;
using System.Net.Sockets;
using System.Reflection;
using MongoDB.Driver;

namespace mongo
{
    public class MongoTable<T>
    {
        public delegate IMongoQuery QueryHandler();

        public delegate IMongoUpdate SetHandler();

        public MongoCollection<T> Collection()
        {
            return MongoDatabase.Init().GetCollection<T>(typeof (T).Name);
        }

        public void Add(T obj)
        {
            Collection().Insert(obj);
        }

        public void Update(QueryHandler query, SetHandler update)
        {
            IMongoQuery q = query();
            IMongoUpdate u = update();
            Collection().Update(q, u);
        }

        public void Delete(QueryHandler query)
        {
            IMongoQuery q = query();
            Collection().Remove(q);
        }
    }
}