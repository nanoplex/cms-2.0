using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mongo;
using MongoDB.Driver.Builders;
using System.Reflection;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
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

        public dynamic parseComponent(string componentName, List<Prop> props, string component, HttpFileCollectionBase files)
        {
            var type = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "cms.Components")
                .Where(comp => comp.Name == componentName)
                .FirstOrDefault();

            var images = new List<string>();
            foreach (var prop in props)
            {
                if (prop.Type == "Image")
                {
                    images.Add(prop.Name);
                }
            }

            dynamic obj = JsonConvert.DeserializeObject(component, type);
            obj._id = ObjectId.GenerateNewId();

            for (int i = 0; i < files.Count; i++)
            {
                var image = Image.FromStream(files[i].InputStream);
                var path = "\\uploads\\images\\" + obj._id + i + ".jpeg";

                image.Save(HttpContext.Current.Server.MapPath(path), ImageFormat.Jpeg);
            }

            return obj;
        }

	    public static event EventHandler OnPagePublished;
    }
}