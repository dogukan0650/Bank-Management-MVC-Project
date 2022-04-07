using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcProject.Models.Entity;

namespace MvcProject.Controllers
{
    public class AdminController : Controller
    {
        // GET: User
        
        denemeEntities1 db = new denemeEntities1();
        [Authorize]
        public ActionResult Index()
        {
            var degerler = db.Users.ToList();
            return View(degerler);
        }
        [Authorize]
        public ActionResult del(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }
            User user = db.Users.Find(id);
            return View(user);
        }

        [Authorize]
        public ActionResult Edit(User user)
        {
            var edit = db.Users.Find(user.ID);
            edit.name = user.name;
            edit.lastname = user.lastname;
            edit.password = user.password;
            edit.gender = user.gender;
            edit.address = user.address;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult editPage(int id)
        {
            var edit = db.Users.Find(id);
            return View("editPage", edit);
        }


        [HttpGet]
        [Authorize]
        public ActionResult newUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult newUser(User u3)
        {
            db.Users.Add(u3);
            db.SaveChanges();
            return View();
        }
        [Authorize]
        public ActionResult ShowDeletedUsers()
        {
            List<User> list = db.Users.Where(x => x.isDeleted).ToList();
            return View(list);
        }
        [Authorize]
        public ActionResult DeletePermanent(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }
        [Authorize]
        [HttpPost, ActionName("DeletePermanent")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePermanentConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
    }
}