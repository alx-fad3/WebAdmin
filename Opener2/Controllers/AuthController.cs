using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Opener2.Controllers
{
    [Obsolete]
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string login, string pass)
        {
            if (login == "under_control" && pass == "xcGUbG8WVhkd#")
            {
                Session.Add("authorization", true);
                Session.Timeout = 30;
                return Redirect("/Home/Index");
            }
            else
            {
                Session.Add("authorization", false);
                ViewBag.error = "Access forbidden";
                return View("Error");
            }
        }
    }
}