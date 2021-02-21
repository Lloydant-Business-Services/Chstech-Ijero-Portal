using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class PaymentScholarshipController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/PaymentScholarship
        public ActionResult Index()
        {
            var pAYMENT_SCHOLARSHIP = db.PAYMENT_SCHOLARSHIP.Include(p => p.PERSON).Include(p => p.SESSION);
            return View(pAYMENT_SCHOLARSHIP.ToList());
        }

        // GET: Admin/PaymentScholarship/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYMENT_SCHOLARSHIP pAYMENT_SCHOLARSHIP = db.PAYMENT_SCHOLARSHIP.Find(id);
            if (pAYMENT_SCHOLARSHIP == null)
            {
                return HttpNotFound();
            }
            return View(pAYMENT_SCHOLARSHIP);
        }

        // GET: Admin/PaymentScholarship/Create
        public ActionResult Create()
        {
            ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name");
            ViewBag.Session_id = new SelectList(db.SESSION, "Session_Id", "Session_Name");
            return View();
        }

        // POST: Admin/PaymentScholarship/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Person_Id,Session_id,Amount,Scholarship_Name")] PAYMENT_SCHOLARSHIP pAYMENT_SCHOLARSHIP)
        {
            if (ModelState.IsValid)
            {
                db.PAYMENT_SCHOLARSHIP.Add(pAYMENT_SCHOLARSHIP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", pAYMENT_SCHOLARSHIP.Person_Id);
            ViewBag.Session_id = new SelectList(db.SESSION, "Session_Id", "Session_Name", pAYMENT_SCHOLARSHIP.Session_id);
            return View(pAYMENT_SCHOLARSHIP);
        }

        // GET: Admin/PaymentScholarship/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYMENT_SCHOLARSHIP pAYMENT_SCHOLARSHIP = db.PAYMENT_SCHOLARSHIP.Find(id);
            if (pAYMENT_SCHOLARSHIP == null)
            {
                return HttpNotFound();
            }
            ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", pAYMENT_SCHOLARSHIP.Person_Id);
            ViewBag.Session_id = new SelectList(db.SESSION, "Session_Id", "Session_Name", pAYMENT_SCHOLARSHIP.Session_id);
            return View(pAYMENT_SCHOLARSHIP);
        }

        // POST: Admin/PaymentScholarship/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Person_Id,Session_id,Amount,Scholarship_Name")] PAYMENT_SCHOLARSHIP pAYMENT_SCHOLARSHIP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pAYMENT_SCHOLARSHIP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", pAYMENT_SCHOLARSHIP.Person_Id);
            ViewBag.Session_id = new SelectList(db.SESSION, "Session_Id", "Session_Name", pAYMENT_SCHOLARSHIP.Session_id);
            return View(pAYMENT_SCHOLARSHIP);
        }

        // GET: Admin/PaymentScholarship/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYMENT_SCHOLARSHIP pAYMENT_SCHOLARSHIP = db.PAYMENT_SCHOLARSHIP.Find(id);
            if (pAYMENT_SCHOLARSHIP == null)
            {
                return HttpNotFound();
            }
            return View(pAYMENT_SCHOLARSHIP);
        }

        // POST: Admin/PaymentScholarship/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            PAYMENT_SCHOLARSHIP pAYMENT_SCHOLARSHIP = db.PAYMENT_SCHOLARSHIP.Find(id);
            db.PAYMENT_SCHOLARSHIP.Remove(pAYMENT_SCHOLARSHIP);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
