using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc.Models;

namespace mvc.Controllers
{
    public class HomeController : Controller
    {
        public JsonResult Site()
        {
            return Json(new Site(null), JsonRequestBehavior.AllowGet);
        }
    }
}