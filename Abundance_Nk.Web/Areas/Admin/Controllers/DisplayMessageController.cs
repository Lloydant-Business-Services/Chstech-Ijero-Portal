using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class DisplayMessageController : BaseController
    {
        private DisplayMessageViewModel viewModel;
        // private DisplayMessage displayMessage;
        private DisplayMessageLogic displayMessageLogic;
        // GET: Admin/DisplayMessage
        public ActionResult Index()
        {
            DisplayMessageLogic messageLogic = new DisplayMessageLogic();
            List<DisplayMessage> displaymessage = new List<DisplayMessage>();
            displaymessage = messageLogic.GetAll();


            return View(displaymessage);
        }

        // GET: Admin/DisplayMessage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/DisplayMessage/Create
        [HttpPost]
        public ActionResult Create(DisplayMessageViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                if (viewModel.DisplayMessage != null)
                {
                    DisplayMessageLogic messageLogic = new DisplayMessageLogic();

                    messageLogic.Create(viewModel.DisplayMessage);

                    SetMessage("Message created successfully! ", Message.Category.Information);

                    return RedirectToAction("Index");
                }
                else
                {
                    SetMessage("Enter display message! ", Message.Category.Error);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View();
        }

        // GET: Admin/DisplayMessage/Edit/5
        public ActionResult Edit(int id)
        {
            viewModel = null;
            try
            {
                if (id != 0)
                {
                    TempData["MessageId"] = id;
                    displayMessageLogic = new DisplayMessageLogic();
                    viewModel = new DisplayMessageViewModel();
                    viewModel.DisplayMessage = displayMessageLogic.GetModelBy(p => p.Id == id);
                }
                else
                {
                    SetMessage("Select a Message to Edit", Message.Category.Error);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // POST: Admin/DisplayMessage/Edit/5
        [HttpPost]
        public ActionResult Edit(DisplayMessageViewModel viewModel)
        {
            try
            {
                // TODO: Add update logic here
                if (viewModel != null)
                {
                    //viewModel.DisplayMessage.Id = (int)TempData["MessageId"];
                    //viewModel.DisplayMessage = displayMessageLogic.GetModelBy(p => p.Id == viewModel.Id).Id;
                    DisplayMessageLogic messageLogic = new DisplayMessageLogic();

                    messageLogic.Modify(viewModel.DisplayMessage);

                    SetMessage("Message Modified successfully! ", Message.Category.Information);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/DisplayMessage/Delete/5
        public ActionResult Delete(int id)
        {
            viewModel = null;
            try
            {
                if (id != 0)
                {

                    displayMessageLogic = new DisplayMessageLogic();
                    viewModel = new DisplayMessageViewModel();
                    viewModel.DisplayMessage = displayMessageLogic.GetModelBy(p => p.Id == id);
                }
                else
                {
                    SetMessage("Select a Message to Edit", Message.Category.Error);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // POST: Admin/DisplayMessage/Delete/5
        [HttpPost]
        public ActionResult Delete(DisplayMessageViewModel viewModel)
        {
            try
            {

                DisplayMessageLogic messageLogic = new DisplayMessageLogic();

                bool deleted = messageLogic.Delete(m => m.Id == viewModel.DisplayMessage.Id);
                if (deleted)
                {
                    SetMessage("Message Deleted successfully! ", Message.Category.Information);
                }


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
