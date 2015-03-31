using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [unused]
        public abstract string Frontend { get; }
    }
}