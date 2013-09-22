using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Controllers
{
    public class HomeController : Controller
    {
        public const string MENU = "Home";

        public HomeController()
        {
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.menu = MENU;
            return View();
        }
    }
}
