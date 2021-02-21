using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Business;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class FeeController : BasicSetupBaseController<Fee, FEE>
    {
        private List<FeeType> feeTypes;
        private FeeTypeLogic feeTypeLogic;
        
        public FeeController()
            : base(new FeeLogic())
        {
            ModelName = "Fee";
            Selector = f => f.Fee_Id == Id;

            feeTypeLogic = new FeeTypeLogic();
        }

        public ActionResult WebForm1()
        {
            return View();
        }

        public override ActionResult Create()
        {
            try
            {
                feeTypes = feeTypeLogic.GetAll();
                ViewBag.TypeId = new SelectList(feeTypes, "Id", "Name");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Create(Fee model)
        {
            List<FeeType> feeTypes = null;

            try
            {
                if (ModelState.IsValid)
                {
                    //if (model.TypeId > 0)
                    //{
                    //    model.Type = new FeeType() { Id = model.TypeId };

                    //    Fee newModel = db.Create(model);
                    //    if (newModel != null)
                    //    {
                    //        SetMessage(ModelName + " has been successfully created.", Message.Category.Information);
                    //        return RedirectToAction("Index");
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            feeTypes = feeTypeLogic.GetAll();
            //ViewBag.TypeId = new SelectList(feeTypes, "Id", "Name", model.TypeId);
            return View(model);
        }

        public override ActionResult Edit(int? id)
        {
            Fee model = null;

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Id = id;
                model = db.GetModelBy(Selector);
                if (model == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            feeTypes = feeTypeLogic.GetAll();
            //ViewBag.TypeId = new SelectList(feeTypes, "Id", "Name", model.TypeId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Edit(Fee model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if (model.TypeId > 0)
                    //{
                    //    model.Type = new FeeType() { Id = model.TypeId };

                    //    bool success = ModifyModel(model);
                    //    if (success)
                    //    {
                    //        SetMessage(ModelName + " modification was successfully saved.", Message.Category.Information);
                    //        return RedirectToAction("Index");
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            feeTypes = feeTypeLogic.GetAll();
            //ViewBag.TypeId = new SelectList(feeTypes, "Id", "Name", model.TypeId);
            return View(model);
        }

        protected override bool ModifyModel(Fee model)
        {
            try
            {
                FeeLogic modelLogic = new FeeLogic();
                return modelLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }







        //private AbundanceEntities db = new AbundanceEntities();

        //// GET: /Admin/Fee/
        //public ActionResult Index()
        //{
        //    var fee = db.FEE.Include(f => f.CURRENT_FEE).Include(f => f.FEE_TYPE);
        //    return View(fee.ToList());
        //}

        //// GET: /Admin/Fee/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    FEE fee = db.FEE.Find(id);
        //    if (fee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(fee);
        //}

        //// GET: /Admin/Fee/Create
        //public ActionResult Create()
        //{
        //    ViewBag.Fee_Id = new SelectList(db.CURRENT_FEE, "Fee_Id", "Fee_Id");
        //    ViewBag.Fee_Type_Id = new SelectList(db.FEE_TYPE, "Fee_Type_Id", "Fee_Type_Name");
        //    return View();
        //}

        //// POST: /Admin/Fee/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="Fee_Id,Fee_Type_Id,Amount,Date_Entered")] FEE fee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.FEE.Add(fee);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Fee_Id = new SelectList(db.CURRENT_FEE, "Fee_Id", "Fee_Id", fee.Fee_Id);
        //    ViewBag.Fee_Type_Id = new SelectList(db.FEE_TYPE, "Fee_Type_Id", "Fee_Type_Name", fee.Fee_Type_Id);
        //    return View(fee);
        //}

        //// GET: /Admin/Fee/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    FEE fee = db.FEE.Find(id);
        //    if (fee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Fee_Id = new SelectList(db.CURRENT_FEE, "Fee_Id", "Fee_Id", fee.Fee_Id);
        //    ViewBag.Fee_Type_Id = new SelectList(db.FEE_TYPE, "Fee_Type_Id", "Fee_Type_Name", fee.Fee_Type_Id);
        //    return View(fee);
        //}

        //// POST: /Admin/Fee/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="Fee_Id,Fee_Type_Id,Amount,Date_Entered")] FEE fee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(fee).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Fee_Id = new SelectList(db.CURRENT_FEE, "Fee_Id", "Fee_Id", fee.Fee_Id);
        //    ViewBag.Fee_Type_Id = new SelectList(db.FEE_TYPE, "Fee_Type_Id", "Fee_Type_Name", fee.Fee_Type_Id);
        //    return View(fee);
        //}

        //// GET: /Admin/Fee/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    FEE fee = db.FEE.Find(id);
        //    if (fee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(fee);
        //}

        //// POST: /Admin/Fee/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    FEE fee = db.FEE.Find(id);
        //    db.FEE.Remove(fee);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}




    }
}
