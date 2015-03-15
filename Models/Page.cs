using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc.Models
{
    public class Page
    {
        public ObjectId _id { get; set; }
	    public string Name { get; set; }
	    public DateTime PublishDate { get; set; }
	    public bool Visibile { get; set; }
	    public List<IContent> Contents { get; set; }

	    public static event EventHandler OnPagePublished;
    }
}