using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net;
using Abundance_Nk.Business;
using System.Linq.Expressions;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Controllers
{
    public abstract class BasicSetupBaseController<T, E> : BaseController
        where T : class
        where E : class
    {
        protected BusinessBaseLogic<T, E> db;

        public BasicSetupBaseController(BusinessBaseLogic<T, E> _db)
        {
            db = _db;
        }

        protected int? Id { get; set; }
        protected Expression<Func<E, bool>> Selector { get; set; }
        protected string ModelName { get; set; }
        

        // GET: /Admin/Faculty/
        public ActionResult Index()
        {
            List<T> models = null;

            try
            {
                models = db.GetAll().ToList();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(models);
        }

        // GET: /Admin/Faculty/Details/5
        public ActionResult Details(int? id)
        {
            T model = null;

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

            return View(model);
        }

         public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(T model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //T newModel = CreateModel(model);

                    T newModel = db.Create(model);

                    if (newModel != null)
                    {
                        SetMessage(ModelName + " has been successfully created.", Message.Category.Information);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(model);
        }

       
        // GET: /Admin/Faculty/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            T model = null;

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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(T model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool success = ModifyModel(model);
                    if (success)
                    {
                        SetMessage(ModelName + " modification was successfully saved.", Message.Category.Information);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(model);
        }

        // GET: /Admin/Faculty/Delete/5
        public ActionResult Delete(int? id)
        {
            T model = null;

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                
                //model = GetModelBy(id);

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

            return View(model);
        }

        // POST: /Admin/Faculty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T model = null;

            try
            {
                //bool success = DeleteModel(id);

                Id = id;
                bool success = db.Delete(Selector);
                if (success)
                {
                    SetMessage(ModelName + " hass been successfully deleted.", Message.Category.Information);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            Id = id;
            model = db.GetModelBy(Selector);
            return View(model);
        }

        protected abstract bool ModifyModel(T model);
       
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

