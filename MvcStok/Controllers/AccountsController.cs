using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.Security;
using MvcProject.Models.Entity;
using Newtonsoft.Json;
using PagedList;
using PagedList.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;

namespace MvcProject.Controllers
{
    public class AccountsController : Controller
    {
        private denemeEntities1 db = new denemeEntities1();

        // GET: Accounts
        [Authorize]
        public ActionResult Index()
        {
            if (Session["accType"] != null)
            {
                if (Session["accType"].Equals("admin"))
                {
                    List<Account> accounts = db.Accounts.Where(x => x.isDeleted == false).ToList();
                    return View(accounts.ToList());
                }
                if (Session["accType"].Equals("user"))
                {
                    User user = db.Users.SingleOrDefault(x => x.name.Equals(User.Identity.Name));
                    List<Account> accounts = db.Accounts.Where(a => a.isDeleted == false && a.accID == user.ID).ToList();
                    return View(accounts.ToList());
                }

            }
            return View();
        }

        // GET: Accounts/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.accID = new SelectList(db.Users, "ID", "username");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "accID,acc_,net")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.accID = new SelectList(db.Users, "ID", "username", account.accID);
            return View(account);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.accID = new SelectList(db.Users, "ID", "username", account.accID);
            return View(account);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "accID,accNum,net")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.accID = new SelectList(db.Users, "ID", "username", account.accID);
            return View(account);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            account.isDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize]
        public ActionResult ShowDeleted()
        {
            List<Account> list = db.Accounts.Where(x => x.isDeleted).ToList();
            return View(list);
        }
        [Authorize]
        public ActionResult DeletePermanent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult TransactionPage(int id)
        {
            List<User> UserList = db.Users.ToList();
            ViewBag.UserList = new SelectList(UserList, "ID", "ID");
            var transferID = db.Accounts.Find(id);
            return View(transferID);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetUserList(int accID)
        {
            List<Account> accounts = db.Accounts.Where(x => x.accID == accID).ToList();
            ViewBag.AccountList = new SelectList(accounts, "accNum", "accNum");

            return Json(ViewBag.AccountList, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public void Transaction(Transaction tr)
        {
            Account acc1 = db.Accounts.Find(tr.senderID);
            Account acc2 = db.Accounts.Find(tr.recieverID);
            if (acc1 != acc2 && tr.amount > 0)
            {
                acc1.net -= tr.amount;
                acc2.net += tr.amount;
                db.Transactions.Add(tr);
                db.SaveChanges();
            }
        }

        public ActionResult ActivateAccount(int? id)
        {
            if (!Session["accType"].Equals("admin"))
            {
                return RedirectToAction("Logout", "Home");
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Account account = db.Accounts.Find(id);
                if (account == null)
                {
                    return HttpNotFound();
                }
                return View(account);
            }

        }

        [Authorize]
        [HttpPost, ActionName("ActivateAccount")]
        public ActionResult ActivateAccountConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            account.isDeleted = false;
            db.SaveChanges();
            return RedirectToAction("ShowDeleted");
        }

        [Authorize]
        public ActionResult History(int? id)
        {
            // User user = db.Users.SingleOrDefault(x => x.name.Equals(User.Identity.Name));
            List<Account> accounts = db.Accounts.Where(a => a.isDeleted == false && a.accID == id).ToList();
            return View(accounts.ToList());
            // Account account = db.Accounts.SingleOrDefault(m => m.accID == id);
            //  return View(account);
        }
        //--------------------------1 tane için---------------------------------------------------------

        //--------------------------1den fazla ---------------------------------------------------------
        [Authorize]
        public ActionResult HistoryOf(int[] ids, int? id, int? page, int? rowsPerPage)
        {
            if (ids == null && id == null)
            {
                return Redirect("Index");
            }

            if (ids != null)
            {
                List<Transaction> list = new List<Transaction>();
                foreach (int accid in ids)
                {
                    list.AddRange(db.Transactions.Where(m => m.senderID == accid || m.recieverID == accid));
                }
                return View(list.ToList());
            }

            if (ids == null && id != null)
            {
                List<Transaction> list = db.Transactions.Where(m => m.senderID == id || m.recieverID == id).ToList();
                return View(list);
            }
            return Redirect("Index");
        }
    }
}
