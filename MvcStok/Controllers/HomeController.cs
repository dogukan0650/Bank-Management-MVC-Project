using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcProject.Models.Entity;

namespace MvcProject.Controllers
{
    public class HomeController : Controller
    {
        
        denemeEntities1 db = new denemeEntities1();
        
        public ActionResult Index()
        {
            Session["ID"] = "-1";
            Session["accType"] = "notlogged";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            
            return View();
        }
        public ActionResult Register()
        {
            ViewBag.Message = "Register page";
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            var usr = db.Users.FirstOrDefault(x => x.username == user.username && x.password == user.password && !(x.isDeleted));
            if (usr != null)
            {
                FormsAuthentication.SetAuthCookie(usr.username, false);
                Session["ID"] = usr.ID;
                Session["accType"] = usr.accType;
                if (usr.accType == "admin")
                {
                    return RedirectToAction("Index","Admin",usr); //admin page
                }
                else
                {
                    return RedirectToAction("Index", "Users"); //user page
                }
            }
            else
            {
                user.LoginError = "Wrong username or password! ";
                return View("Index",user);
            }
            
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }


    }
}