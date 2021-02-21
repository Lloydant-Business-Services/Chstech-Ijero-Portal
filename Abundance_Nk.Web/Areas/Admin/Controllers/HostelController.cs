using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class HostelController : BaseController
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private HostelViewModel viewModel = null;
        public bool IsError { get; set; }

        // GET: Admin/Hostel
        public ActionResult Index()
        {
            var hOSTEL = db.HOSTEL.Include(h => h.HOSTEL_TYPE);
            return View(hOSTEL.ToList());
        }

        // GET: Admin/Hostel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL hOSTEL = db.HOSTEL.Find(id);
            if (hOSTEL == null)
            {
                return HttpNotFound();
            }
            return View(hOSTEL);
        }

        // GET: Admin/Hostel/Create
        public ActionResult Create()
        {
            ViewBag.Hostel_Type_Id = new SelectList(db.HOSTEL_TYPE, "Hostel_Type_Id", "Hostel_Type_Name");
            return View();
        }

        // POST: Admin/Hostel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Hostel_Id,Hostel_Type_Id,Hostel_Name,Capacity,Description,Date_Entered,Activated")] HOSTEL hOSTEL)
        {
            if (ModelState.IsValid)
            {
                db.HOSTEL.Add(hOSTEL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Hostel_Type_Id = new SelectList(db.HOSTEL_TYPE, "Hostel_Type_Id", "Hostel_Type_Name", hOSTEL.Hostel_Type_Id);
            return View(hOSTEL);
        }

        // GET: Admin/Hostel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL hOSTEL = db.HOSTEL.Find(id);
            if (hOSTEL == null)
            {
                return HttpNotFound();
            }
            ViewBag.Hostel_Type_Id = new SelectList(db.HOSTEL_TYPE, "Hostel_Type_Id", "Hostel_Type_Name", hOSTEL.Hostel_Type_Id);
            return View(hOSTEL);
        }

        // POST: Admin/Hostel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Hostel_Id,Hostel_Type_Id,Hostel_Name,Capacity,Description,Date_Entered,Activated")] HOSTEL hOSTEL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hOSTEL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Hostel_Type_Id = new SelectList(db.HOSTEL_TYPE, "Hostel_Type_Id", "Hostel_Type_Name", hOSTEL.Hostel_Type_Id);
            return View(hOSTEL);
        }

        // GET: Admin/Hostel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL hOSTEL = db.HOSTEL.Find(id);
            if (hOSTEL == null)
            {
                return HttpNotFound();
            }
            return View(hOSTEL);
        }

        // POST: Admin/Hostel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HOSTEL hOSTEL = db.HOSTEL.Find(id);
            db.HOSTEL.Remove(hOSTEL);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ViewHostelTypes()
        {
            try
            {
                viewModel = new HostelViewModel();
                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();

                viewModel.HostelTypes = hostelTypeLogic.GetAll();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult HostelTypeDetails(int id)
        {
            try
            {
                if (id > 0)
                {
                    viewModel = new HostelViewModel();
                    HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                    HostelType hostelType = hostelTypeLogic.GetModelBy(h => h.Hostel_Type_Id == id);

                    viewModel.HostelType = hostelType;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult CreateHostelType()
        {
            HostelViewModel viewModel = new HostelViewModel();
            
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateHostelType(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                    hostelTypeLogic.Create(viewModel.HostelType);

                    return RedirectToAction("ViewHostelTypes");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult EditHostelType(int id)
        {
            viewModel = new HostelViewModel();
            try
            {
                if (id == null || id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                HostelType hostelType = hostelTypeLogic.GetModelBy(h => h.Hostel_Type_Id == id);
                if (hostelType == null)
                {
                    return HttpNotFound();
                }

                viewModel.HostelType = hostelType;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHostelType(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                    hostelTypeLogic.Modify(viewModel.HostelType);

                    SetMessage("Operation Successful!", Message.Category.Information);
                    return RedirectToAction("ViewHostelTypes");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult DeleteHostelType(int id)
        {
            try
            {
                if (id == null || id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                viewModel = new HostelViewModel();
                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                viewModel.HostelType = hostelTypeLogic.GetModelBy(h => h.Hostel_Type_Id == id);

                if (viewModel.HostelType == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteHostelType(HostelViewModel viewModel)
        {
            try
            {
                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                hostelTypeLogic.Delete(h => h.Hostel_Type_Id == viewModel.HostelType.Hostel_Type_Id);

                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewHostelTypes");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult HostelSetup()
        {
            try
            {
                viewModel = new HostelViewModel();
                ViewBag.Hostels = viewModel.HostelSelectListItem;
                ViewBag.HostelTypes = viewModel.HostelTypeSelectListItem;
                HostelLogic hostelLogic = new HostelLogic();

                viewModel.Hostels = hostelLogic.GetAll();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public JsonResult CreateRecord(int tableType, string myRecordArray)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ArrayJsonView arrayJsonView = serializer.Deserialize<ArrayJsonView>(myRecordArray);

                string name = null;
                string description = null;
                string capacity = null;
                DateTime date = new DateTime();
                bool activated = false;
                int hostelType = 0;
                int hostel = 0;

                AssignJsonArrayValues(arrayJsonView, ref name, ref description, ref date, ref activated, ref hostelType, ref hostel, ref capacity);

                switch (tableType)
                {
                    case 1:
                        result.Message = CreateHostel(name, capacity, description, date, activated, hostelType);
                        result.IsError = IsError;
                        break;
                    case 2:
                        result.Message = CreateHostelType(name, description);
                        result.IsError = IsError;
                        break;
                    case 3:
                        result.Message = CreateHostelSeries(name, hostel, activated);
                        result.IsError = IsError;
                        break;
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PopulateSetupTables(int tableType, string hostel)
        {
            JsonResultModel result = new JsonResultModel();

            List<ArrayJsonView> hostels = new List<ArrayJsonView>();
            List<ArrayJsonView> hostelTypes = new List<ArrayJsonView>();
            List<ArrayJsonView> hostelSeries = new List<ArrayJsonView>();

            try
            {
                //1 = hostel, 2 = hostelType, 3 = hostelSeries

                if (!string.IsNullOrEmpty(hostel))
                {
                    hostel = hostel.Trim().Replace(" ", "");
                }

                switch (tableType)
                {
                    case 1:
                        HostelLogic hostelLogic = new HostelLogic();
                        result.Hostels = hostelLogic.GetAll();
                        for (int i = 0; i < result.Hostels.Count; i++)
                        {
                            hostels.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.Hostels[i].Id.ToString(),
                                Description = result.Hostels[i].Description,
                                Name = result.Hostels[i].Name,
                                Capacity = result.Hostels[i].Capacity.ToString(),
                                Date = result.Hostels[i].DateEntered.ToLongDateString(),
                                Activated = result.Hostels[i].Activated.ToString(),
                                HostelType = result.Hostels[i].HostelType.Hostel_Type_Id.ToString(),
                                HostelTypeName = result.Hostels[i].HostelType.Hostel_Type_Name
                            });
                        }
                        break;

                    case 2:
                        HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                        result.HostelTypes = hostelTypeLogic.GetAll();
                        for (int i = 0; i < result.HostelTypes.Count; i++)
                        {
                            hostelTypes.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.HostelTypes[i].Hostel_Type_Id.ToString(),
                                Name = result.HostelTypes[i].Hostel_Type_Name,
                                Description = result.HostelTypes[i].Hostel_Type_Description
                            });
                        }
                        break;

                    case 3:
                        HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
                        int hostelInt = Convert.ToInt32(hostel);
                        result.HostelSeries = hostelSeriesLogic.GetModelsBy(h => h.Hostel_Id == hostelInt);
                        for (int i = 0; i < result.HostelSeries.Count; i++)
                        {
                            hostelSeries.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.HostelSeries[i].Id.ToString(),
                                Name = result.HostelSeries[i].Name,
                                Hostel = result.HostelSeries[i].Hostel.Id.ToString(),
                                HostelName = result.HostelSeries[i].Hostel.Name,
                                Activated = result.HostelSeries[i].Activated.ToString()
                            });
                        }
                        break;


                }

                switch (tableType)
                {
                    case 1:
                        return Json(hostels, JsonRequestBehavior.AllowGet);
                        break;
                    case 2:
                        return Json(hostelTypes, JsonRequestBehavior.AllowGet);
                        break;
                    case 3:
                        return Json(hostelSeries, JsonRequestBehavior.AllowGet);
                        break;
                }

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditRecord(int tableType, string recordId, string myRecordArray, string action)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (action == "edit" && !string.IsNullOrEmpty(recordId))
                {
                    switch (tableType)
                    {
                        case 1:
                            int id = Convert.ToInt32(recordId);
                            ArrayJsonView resultModel = new ArrayJsonView();
                            HostelLogic hostelLogic = new HostelLogic();
                            Hostel hostel = new Hostel();
                            if (id > 0)
                            {
                                hostel = hostelLogic.GetModelsBy(f => f.Hostel_Id == id).LastOrDefault();
                                if (hostel != null)
                                {
                                    resultModel.IsError = false;
                                    resultModel.Id = hostel.Id.ToString();
                                    resultModel.Name = hostel.Name;
                                    resultModel.Capacity = hostel.Capacity.ToString();
                                    resultModel.Description = hostel.Description;
                                    resultModel.Date = hostel.DateEntered.ToLongDateString();
                                    resultModel.Activated = hostel.Activated.ToString();
                                    resultModel.HostelType = hostel.HostelType.Hostel_Type_Id.ToString();

                                    return Json(resultModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    resultModel.IsError = true;
                                    resultModel.Message = "Record does not exist in the database.";
                                    return Json(resultModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                resultModel.IsError = true;
                                resultModel.Message = "Edit parameter was not set.";
                                return Json(resultModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 2:
                            int hostelTypeId = Convert.ToInt32(recordId);
                            ArrayJsonView hostelTypeModel = new ArrayJsonView();
                            HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                            HostelType hostelType = new HostelType();
                            if (hostelTypeId > 0)
                            {
                                hostelType = hostelTypeLogic.GetModelsBy(f => f.Hostel_Type_Id == hostelTypeId).LastOrDefault();
                                if (hostelType != null)
                                {
                                    hostelTypeModel.IsError = false;
                                    hostelTypeModel.Id = hostelType.Hostel_Type_Id.ToString();
                                    hostelTypeModel.Name = hostelType.Hostel_Type_Name;
                                    hostelTypeModel.Description = hostelType.Hostel_Type_Description;

                                    return Json(hostelTypeModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    hostelTypeModel.IsError = true;
                                    hostelTypeModel.Message = "Record does not exist in the database.";
                                    return Json(hostelTypeModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                hostelTypeModel.IsError = true;
                                hostelTypeModel.Message = "Edit parameter was not set.";
                                return Json(hostelTypeModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 3:
                            int hostelSeriesId = Convert.ToInt32(recordId);
                            ArrayJsonView hostelSeriesModel = new ArrayJsonView();
                            HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
                            HostelSeries hostelSeries = new HostelSeries();
                            if (hostelSeriesId > 0)
                            {
                                hostelSeries = hostelSeriesLogic.GetModelsBy(f => f.Series_Id == hostelSeriesId).LastOrDefault();
                                if (hostelSeries != null)
                                {
                                    hostelSeriesModel.IsError = false;
                                    hostelSeriesModel.Id = hostelSeries.Id.ToString();
                                    hostelSeriesModel.Name = hostelSeries.Name;
                                    hostelSeriesModel.Hostel = hostelSeries.Hostel.Id.ToString();
                                    hostelSeriesModel.Activated = hostelSeries.Activated.ToString();

                                    return Json(hostelSeriesModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    hostelSeriesModel.IsError = true;
                                    hostelSeriesModel.Message = "Record does not exist in the database.";
                                    return Json(hostelSeriesModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                hostelSeriesModel.IsError = true;
                                hostelSeriesModel.Message = "Edit parameter was not set.";
                                return Json(hostelSeriesModel, JsonRequestBehavior.AllowGet);
                            }
                            break;

                    }
                }
                else if (action == "save")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    ArrayJsonView arrayJsonView = serializer.Deserialize<ArrayJsonView>(myRecordArray);

                    string name = null;
                    string description = null;
                    string capacity = null;
                    DateTime date = new DateTime();
                    bool activated = false;
                    int hostelType = 0;
                    int hostel = 0;

                    AssignJsonArrayValues(arrayJsonView, ref name, ref description, ref date, ref activated, ref hostelType, ref hostel, ref capacity);

                    switch (tableType)
                    {
                        case 1:
                            int id = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveHostel(name, capacity, description, date, activated, hostelType, id);
                            result.IsError = IsError;
                            break;
                        case 2:
                            int hostelTypeId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveHostelType(name, description, hostelTypeId);
                            result.IsError = IsError;
                            break;
                        case 3:
                            int hostelSeriesId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveHostelSeries(name, hostel, activated, hostelSeriesId);
                            result.IsError = IsError;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteRecord(int tableType, string recordId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (tableType > 0 && !string.IsNullOrEmpty(recordId))
                {
                    switch (tableType)
                    {
                        case 1:
                            int id = Convert.ToInt32(recordId);
                            HostelLogic hostelLogic = new HostelLogic();
                            bool deleted = hostelLogic.Delete(f => f.Hostel_Id == id);
                            if (deleted)
                            {
                                result.IsError = false;
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }

                            break;
                        case 2:
                            int hostelTypeId = Convert.ToInt32(recordId);
                            HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                            bool deletedHostelType = hostelTypeLogic.Delete(f => f.Hostel_Type_Id == hostelTypeId);
                            if (deletedHostelType)
                            {
                                result.IsError = false;
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 3:
                            int hostelSeriesId = Convert.ToInt32(recordId);
                            HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
                            bool deletedHostelSeries = hostelSeriesLogic.Delete(f => f.Series_Id == hostelSeriesId);
                            if (deletedHostelSeries)
                            {
                                result.IsError = false;
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Parameter was not set.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void AssignJsonArrayValues(ArrayJsonView arrayJsonView, ref string name, ref string description, ref DateTime date, ref bool activated, ref int hostelType, ref int hostel, ref string capacity)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrayJsonView.Name))
                {
                    name = arrayJsonView.Name;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Description))
                {
                    description = arrayJsonView.Description;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Date))
                {
                    date = Convert.ToDateTime(arrayJsonView.Date);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Activated))
                {
                    activated = arrayJsonView.Activated.ToLower() == "true" ? true : false;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.HostelType))
                {
                    hostelType = Convert.ToInt32(arrayJsonView.HostelType);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Hostel))
                {
                    hostel = Convert.ToInt32(arrayJsonView.Hostel);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Capacity))
                {
                    capacity = arrayJsonView.Capacity;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CreateHostel(string name, string capacity, string description, DateTime dateAdded, bool activated, int hostelTypeId)
        {
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(capacity) && hostelTypeId > 0 )
                {
                    HostelLogic hostelLogic = new HostelLogic();
                    Hostel existingHostel = hostelLogic.GetModelsBy(p => p.Hostel_Name == name && p.Hostel_Type_Id == hostelTypeId).LastOrDefault();

                    if (existingHostel == null)
                    {
                        existingHostel = new Hostel();
                        existingHostel.Name = name;
                        existingHostel.Description = description;
                        existingHostel.DateEntered = new DateTime(dateAdded.Year, dateAdded.Month, dateAdded.Day);
                        existingHostel.Activated = activated;
                        existingHostel.HostelType = new HostelType() { Hostel_Type_Id = hostelTypeId };
                        existingHostel.Capacity = Convert.ToInt32(capacity);

                        hostelLogic.Create(existingHostel);

                        IsError = false;
                        message = "Hostel created successfully.";
                    }
                    else
                    {
                        IsError = true;
                        message = "Record already exist.";
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                message = ex.Message;
            }

            return message;
        }

        private string CreateHostelType(string name, string description)
        {
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                    HostelType existingHostelType = hostelTypeLogic.GetModelsBy(p => p.Hostel_Type_Name == name).LastOrDefault();

                    if (existingHostelType == null)
                    {
                        existingHostelType = new HostelType();
                        existingHostelType.Hostel_Type_Name = name;
                        existingHostelType.Hostel_Type_Description = description;

                        hostelTypeLogic.Create(existingHostelType);

                        IsError = false;
                        message = "Hostel-Type created successfully.";
                    }
                    else
                    {
                        IsError = true;
                        message = "Record already exist.";
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                message = ex.Message;
            }

            return message;
        }

        private string CreateHostelSeries(string name, int hostel, bool activated)
        {
            string message = "";
            try
            {
                if (hostel > 0 && !string.IsNullOrEmpty(name))
                {
                    HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
                    HostelSeries existingHostelSeries = new HostelSeries();

                    existingHostelSeries = hostelSeriesLogic.GetModelsBy(f => f.Hostel_Id == hostel && f.Series_Name == name).LastOrDefault();

                    if (existingHostelSeries == null)
                    {
                        existingHostelSeries = new HostelSeries();
                        existingHostelSeries.Name = name;
                        existingHostelSeries.Hostel = new Hostel() { Id = hostel };
                        existingHostelSeries.Activated = activated;

                        hostelSeriesLogic.Create(existingHostelSeries);

                        IsError = false;
                        message = "Hostel-Series created successfully.";
                    }
                    else
                    {
                        IsError = true;
                        message = "Record already exist.";
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                message = ex.Message;
            }

            return message;
        }


        public string SaveHostel(string name, string capacity, string description, DateTime date, bool activated, int hostelType, int id)
        {
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(name) && hostelType > 0 && !string.IsNullOrEmpty(capacity) && id > 0)
                {
                    HostelLogic hostelLogic = new HostelLogic();

                    Hostel existingHostel = hostelLogic.GetModelsBy(a => a.Hostel_Name == name && a.Hostel_Type_Id == hostelType && a.Hostel_Id != id).LastOrDefault();

                    if (existingHostel == null)
                    {
                        existingHostel = new Hostel();

                        existingHostel.Id = id;
                        existingHostel.Name = name;
                        existingHostel.Capacity = Convert.ToInt32(capacity);
                        existingHostel.Description = description;
                        existingHostel.DateEntered = new DateTime(date.Year, date.Month, date.Day);
                        existingHostel.Activated = activated;
                        existingHostel.HostelType = new HostelType() { Hostel_Type_Id = hostelType };

                        hostelLogic.Modify(existingHostel);

                        message = "Hostel was modified";
                        IsError = false;
                    }
                    else
                    {
                        message = "Edited Hostel values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveHostelType(string name, string description, int id)
        {
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(name) && id > 0)
                {
                    HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();

                    HostelType existingHostelType = hostelTypeLogic.GetModelsBy(a => a.Hostel_Type_Name == name && a.Hostel_Type_Id != id).LastOrDefault();

                    if (existingHostelType == null)
                    {
                        existingHostelType = new HostelType();

                        existingHostelType.Hostel_Type_Id = id;
                        existingHostelType.Hostel_Type_Name = name;
                        existingHostelType.Hostel_Type_Description = description;

                        hostelTypeLogic.Modify(existingHostelType);

                        message = "Hostel-Type was modified";
                        IsError = false;
                    }
                    else
                    {
                        message = "Edited Hostel-Type values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
        public string SaveHostelSeries(string name, int hostel, bool activated, int id)
        {
            string message = "";
            try
            {
                if (hostel > 0 && !string.IsNullOrEmpty(name) && id > 0)
                {
                    HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();

                    HostelSeries hostelSeries = hostelSeriesLogic.GetModelsBy(a => a.Hostel_Id == hostel && a.Series_Name == name && a.Series_Id != id).LastOrDefault();

                    if (hostelSeries == null)
                    {
                        hostelSeries = new HostelSeries();

                        hostelSeries.Id = id;
                        hostelSeries.Name = name;
                        hostelSeries.Hostel = new Hostel() { Id = hostel };
                        hostelSeries.Activated = activated;

                        hostelSeriesLogic.Modify(hostelSeries);

                        message = "Hostel-Series was modified";
                        IsError = false;
                    }
                    else
                    {
                        message = "Hostel-Series values already exist";
                        IsError = true;
                    }
                }
                else
                {
                    IsError = true;
                    message = "One or more of the parameters required was not set.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsError = true;
            }

            return message;
        }
    }
}
