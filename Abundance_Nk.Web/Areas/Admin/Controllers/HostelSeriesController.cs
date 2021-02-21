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
    public class HostelSeriesController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/HostelSeries
        public ActionResult Index()
        {
            var hOSTEL_SERIES = db.HOSTEL_SERIES.Include(h => h.HOSTEL);
            return View(hOSTEL_SERIES.ToList());
        }

        // GET: Admin/HostelSeries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL_SERIES hOSTEL_SERIES = db.HOSTEL_SERIES.Find(id);
            if (hOSTEL_SERIES == null)
            {
                return HttpNotFound();
            }
            return View(hOSTEL_SERIES);
        }

        // GET: Admin/HostelSeries/Create
        public ActionResult Create()
        {
            ViewBag.Hostel_Id = new SelectList(db.HOSTEL, "Hostel_Id", "Hostel_Name");
            return View();
        }

        // POST: Admin/HostelSeries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Series_Id,Series_Name,Hostel_Id,Activated")] HOSTEL_SERIES hOSTEL_SERIES)
        {
            if (ModelState.IsValid)
            {
                db.HOSTEL_SERIES.Add(hOSTEL_SERIES);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Hostel_Id = new SelectList(db.HOSTEL, "Hostel_Id", "Hostel_Name", hOSTEL_SERIES.Hostel_Id);
            return View(hOSTEL_SERIES);
        }

        // GET: Admin/HostelSeries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL_SERIES hOSTEL_SERIES = db.HOSTEL_SERIES.Find(id);
            if (hOSTEL_SERIES == null)
            {
                return HttpNotFound();
            }
            ViewBag.Hostel_Id = new SelectList(db.HOSTEL, "Hostel_Id", "Hostel_Name", hOSTEL_SERIES.Hostel_Id);
            return View(hOSTEL_SERIES);
        }

        // POST: Admin/HostelSeries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Series_Id,Series_Name,Hostel_Id,Activated")] HOSTEL_SERIES hOSTEL_SERIES)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hOSTEL_SERIES).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Hostel_Id = new SelectList(db.HOSTEL, "Hostel_Id", "Hostel_Name", hOSTEL_SERIES.Hostel_Id);
            return View(hOSTEL_SERIES);
        }

        // GET: Admin/HostelSeries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL_SERIES hOSTEL_SERIES = db.HOSTEL_SERIES.Find(id);
            if (hOSTEL_SERIES == null)
            {
                return HttpNotFound();
            }
            return View(hOSTEL_SERIES);
        }

        // POST: Admin/HostelSeries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HOSTEL_SERIES hOSTEL_SERIES = db.HOSTEL_SERIES.Find(id);
            db.HOSTEL_SERIES.Remove(hOSTEL_SERIES);
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
