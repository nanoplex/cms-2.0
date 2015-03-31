﻿using System;
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

        [unused]
        public string Internal { get; set; }

        public override string Frontend
        {
            get { throw new NotImplementedException(); }
        }
    }
}