using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cms.Components
{
    public abstract class Component
    {
        [unused]
        public ObjectId _id { get; set; }
        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
        }

        [unused]
        public abstract string Frontend { get; }
    }
}