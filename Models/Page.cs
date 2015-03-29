using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mongo;
using MongoDB.Driver.Builders;
namespace cms.Models
{
    public class Page
    {
        public ObjectId _id { get; set; }
	    public string Name { get; set; }
	    public DateTime PublishDate { get; set; }
        public int Order { get; set; }
	    public bool Visibile { get; set; }
	    public List<object> Components { get; set; }

        public static MongoTable<Page> Db = new MongoTable<Page>();

        public Page()
        {
            
        }

	    public static event EventHandler OnPagePublished;
    }
}