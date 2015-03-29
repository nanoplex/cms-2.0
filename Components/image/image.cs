using MongoDB.Bson;
using System;

namespace cms.Components
{
    public class image : Component
    {
        [image]
        public string Image
        {
            get
            {
                return "\\uploads\\images\\" + _id + "0.jpeg";
            }
        }

        public override string Frontend
        {
            get { return "<img src='" + Image + "'/>"; }
        }
    }
}