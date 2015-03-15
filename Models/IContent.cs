using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc.Models
{
    public interface IContent
    {
        ObjectId _id { get; set; }
    }
}