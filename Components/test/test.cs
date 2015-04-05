using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cms.Components
{
    public class test : Component
    {
        public string String { get; set; }
        [textbox]
        public string Textbox { get; set; }
        public int Int { get; set; }
        public bool Bool { get; set; }
        [image]
        public string Image { get; set; }

        public List<string> stringList { get; set; }
        public List<int> intList { get; set; }

        public text TextComponent { get; set; }

        public List<text> ListTextComponent { get; set; }

        [unused]
        public string Internal { get; set; }

        public override string Frontend
        {
            get 
            {
                var str = "<p>" + String +"</p>" +
                          "<p>" + Textbox + "</p>" +
                          "<p>" + Int + "</p>" +
                          "<p>" + Bool + "</p>" +
                          "<img src='/uploads/images/" + _id + "0.jpeg'/>" +

                "<p>string list</p>";
                foreach (var s in stringList)
                {
                    str += "<p>" + s + "</p>";
                }

                str += "<p>int list</p>";
                foreach (var i in intList)
                {
                    str += "<p>" + i + "</p>";
                }

                str += TextComponent.Frontend;

                foreach (var text in ListTextComponent)
                {
                    str += text.Frontend;
                }

                return str;
            }
        }
    }
}