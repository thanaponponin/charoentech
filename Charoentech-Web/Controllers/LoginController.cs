using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Charoentech_Web.Controllers
{
    public class LoginController : Controller
    {
        CharoenTechEntities db = new CharoenTechEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(USERS model)
        {
            var user = db.USERS.FirstOrDefault(x => x.USERNAME == model.USERNAME && x.PASSWORD == model.PASSWORD && x.ACTIVE == "A");
            if (user != null)
            {
                HttpContext.Session["Username"] = model.USERNAME;

                return RedirectToAction("Index", "Admin");
            }

            TempData["notice"] = "Username or Password incorrect!";
            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            Session["Username"] = null;

            return RedirectToAction("Index");
        }
    }
}