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
    public class ProfileController : BaseController
    {
        public ProfileViewModel ViewModel;
        // GET: Admin/Profile
        public ActionResult Index()
        {
            try
            {
                StaffLogic staffLogic = new StaffLogic();

                ViewModel = new ProfileViewModel();
                ViewBag.StateId = ViewModel.StateSelectList;
                ViewBag.MaritalStatusId = ViewModel.MaritalStatusSelectList;
                ViewBag.ReligionId = ViewModel.ReligionSelectList;
                string username = User.Identity.Name;
                ViewModel.staffDetail = staffLogic.GetBy(username);
            }
            catch (Exception)
            {
                    
                throw;
            }
            return View(ViewModel);
        }
        [HttpPost]
        public ActionResult Index(ProfileViewModel ViewModel)
        {
            try
            {
                PersonLogic personLogic = new PersonLogic();
                StaffLogic staffLogic = new StaffLogic();
                UserLogic userLogic = new UserLogic();
                string username = User.Identity.Name;
                Model.Model.User user = new User();
                user = userLogic.GetModelBy(a => a.User_Name == username);

                if (ViewModel.staffDetail != null && ViewModel.staffDetail.Id > 0)
                {
                    personLogic.Modify(ViewModel.staffDetail);
                    staffLogic.Modify(ViewModel.staffDetail);
                     SetMessage("Profile updated successfully. You may now proceed",Message.Category.Information);
                }
                else
                {
                    
                    Role role = new Role() { Id = 6 };
                    PersonType personType = new PersonType() { Id = 1 };
                    Nationality nationality = new Nationality() { Id = 1 };

                    ViewModel.staffDetail.Role = role;
                    ViewModel.staffDetail.Nationality = nationality;
                    ViewModel.staffDetail.DateEntered = DateTime.Now;
                    ViewModel.staffDetail.Type = personType;
                    Person person = personLogic.Create(ViewModel.staffDetail);
                    ViewModel.staffDetail.Id = person.Id;
                    ViewModel.staffDetail.StaffType = new StaffType(){Id = 1};
                    ViewModel.staffDetail.User = user;
                    staffLogic.Create(ViewModel.staffDetail);
                    SetMessage("Profile Created successfully",Message.Category.Information);
                }

                 ViewBag.StateId = ViewModel.StateSelectList;
                ViewBag.MaritalStatusId = ViewModel.MaritalStatusSelectList;
                ViewBag.ReligionId = ViewModel.ReligionSelectList;
            }
            catch (Exception)
            {
                    
                throw;
            }
            return View(ViewModel);
        }
    }
}