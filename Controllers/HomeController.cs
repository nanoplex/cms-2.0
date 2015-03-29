using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cms.Models;

namespace cms.Controllers
{
    public class HomeController : Controller
    {
        public static Site site = new Site(null);

        [HttpGet]
        public JsonResult Site()
        {
            site = new Site(null);
            return Json(site, JsonRequestBehavior.AllowGet);
        }
    }
}