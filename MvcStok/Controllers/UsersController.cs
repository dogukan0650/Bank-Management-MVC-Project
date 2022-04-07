using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcProject.Models.Entity;

namespace MvcProject.Controllers
{
    public class UsersController : Controller
    {
        private denemeEntities1 db = new denemeEntities1();

        // GET: Users
        [Authorize]
        public ActionResult Index()
        {
            User user = (db.Users.Find(Session["ID"]));
            return View(user);
            //return RedirectToAction("Accounts");
        }

        // GET: Users/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,username,password,name,lastname,gender,address,accType")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            List<Account> acList = db.Accounts.Where(x => x.accID == id).ToList();
            if (acList != null)
            {
                foreach (Account ac in acList)
                {
                    db.Accounts.Remove(ac);
                    db.SaveChanges();
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }
            else
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }

            return RedirectToAction("Home");
        }
    }
}
