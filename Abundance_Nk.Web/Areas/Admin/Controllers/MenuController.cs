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
    public class MenuController : BaseController
    {
        private MenuViewModel viewModel;
        public ActionResult AddMenu()
        {
            try
            {
                viewModel = new MenuViewModel();
                PopulateAllDropDown(viewModel);
            }
            catch (Exception ex)
            {   
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddMenu(MenuViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    MenuLogic menuLogic = new MenuLogic();

                    List<Menu> MenuList = menuLogic.GetModelsBy(m => m.Display_Name == viewModel.Menu.Action && m.Controller == viewModel.Menu.Controller && m.Menu_Group_Id == viewModel.MenuGroup.Id);
                    if (MenuList.Count > 0)
                    {
                        SetMessage("This Menu has already been added to this menuGroup!", Message.Category.Error);
                        RetainDropDown(viewModel);
                        return View(viewModel);
                    }

                    viewModel.Menu.Activated = true;
                    viewModel.Menu.MenuGroup = viewModel.MenuGroup;
                    menuLogic.Create(viewModel.Menu);

                    SetMessage("Operation Successful! ", Message.Category.Information);
                    return RedirectToAction("AddMenu"); 
                }
                
            }
            catch (Exception ex)
            {
                SetMessage("Error!" + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }

        public ActionResult ViewMenuByMenuGroup()
        {
            try
            {
                viewModel = new MenuViewModel();
                PopulateAllDropDown(viewModel);
            }
            catch (Exception ex)
            {   
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewMenuByMenuGroup(MenuViewModel viewModel)
        {
            try
            {
                if (viewModel.MenuGroup != null && viewModel.MenuGroup.Id > 0)
                {
                    MenuLogic menuLogic = new MenuLogic();
                    viewModel.MenuList = menuLogic.GetModelsBy(m => m.Menu_Group_Id == viewModel.MenuGroup.Id);

                    RetainDropDown(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel); 
        }
        public ActionResult EditMenu(int mid)
        {
            try
            {
                viewModel = new MenuViewModel();
                if (mid > 0)
                {
                    MenuLogic menuLogic = new MenuLogic();
                    viewModel.Menu = menuLogic.GetModelBy(x => x.Menu_Id == mid);
                    if (viewModel.Menu != null)
                    {
                        viewModel.MenuGroup = viewModel.Menu.MenuGroup; 
                    }  

                    RetainDropDown(viewModel);
                    return View(viewModel); 
                } 
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }
        
        [HttpPost]
        public ActionResult EditMenu(MenuViewModel viewModel)
        {
            try
            {   
                MenuLogic menuLogic = new MenuLogic();
                viewModel.Menu.MenuGroup = viewModel.MenuGroup;
                bool isUpdated = menuLogic.Modify(viewModel.Menu);

                if (isUpdated == false)
                {
                    SetMessage("Edit Unsuccessful! ", Message.Category.Error);
                    return RedirectToAction("EditMenu", new{ mid = viewModel.Menu.Id});
                }

                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewMenuByMenuGroup");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View("ViewMenuByMenuGroup", viewModel);
        }

        public ActionResult ConfirmDeleteMenu(int mid)
        {
            try
            {
                viewModel = new MenuViewModel();
                if (mid > 0)
                {
                    MenuLogic menuLogic = new MenuLogic();
                    viewModel.Menu = menuLogic.GetModelBy(x => x.Menu_Id == mid);
                    if (viewModel.Menu != null)
                    {
                        viewModel.MenuGroup = viewModel.Menu.MenuGroup;
                    }

                    RetainDropDown(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteMenu(MenuViewModel viewModel)
        {
            try
            {
                MenuLogic menuLogic = new MenuLogic();
                menuLogic.Delete(x => x.Menu_Id == viewModel.Menu.Id);

                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewMenuByMenuGroup");

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }    

            return RedirectToAction("ConfirmDeleteMenu", new { mid = viewModel.Menu.Id});
        }

        public void PopulateAllDropDown(MenuViewModel viewModel)
        {
            try
            {
                ViewBag.MenuGroup = viewModel.MenuGroupSelectList;
                ViewBag.Menu = viewModel.MenuSelectList;
                ViewBag.Role = viewModel.RoleSelectList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RetainDropDown(MenuViewModel viewModel)
        {
            try
            {
                if (viewModel.MenuGroup != null)
                {
                    ViewBag.MenuGroup = new SelectList(viewModel.MenuGroupSelectList, "Value", "Text", viewModel.MenuGroup.Id);
                }
                else
                {
                    ViewBag.MenuGroup = viewModel.MenuGroupSelectList; 
                }
                if (viewModel.Menu != null)
                {
                    ViewBag.Menu = new SelectList(viewModel.MenuSelectList, "Value", "Text", viewModel.Menu.Id);
                }
                else
                {
                    ViewBag.Menu = viewModel.MenuSelectList;
                }
                if (viewModel.Role != null)
                {
                    ViewBag.Role = new SelectList(viewModel.RoleSelectList, "Value", "Text", viewModel.Role.Id);
                }
                else
                {
                    ViewBag.Role = viewModel.RoleSelectList;
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult AddMenuInRole()
        {
            try
            {
                viewModel = new MenuViewModel();
                PopulateAllDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddMenuInRole(MenuViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    MenuInRoleLogic menuInRoleLogic = new MenuInRoleLogic();

                    List<MenuInRole> menuInRoleList = menuInRoleLogic.GetModelsBy(m => m.Menu_Id == viewModel.Menu.Id && m.Role_Id == viewModel.Role.Id);
                    if (menuInRoleList.Count > 0)
                    {
                        SetMessage("This Menu has already been added to this Role!", Message.Category.Error);
                        RetainDropDown(viewModel);
                        return View(viewModel); 
                    }

                    viewModel.MenuInRole = new MenuInRole();
                    viewModel.MenuInRole.Activated = true;
                    viewModel.MenuInRole.Menu = viewModel.Menu;
                    viewModel.MenuInRole.Role = viewModel.Role;
                    menuInRoleLogic.Create(viewModel.MenuInRole);

                    SetMessage("Operation Successful! ", Message.Category.Information);
                    return RedirectToAction("AddMenuInRole");
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error!" + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }

        public ActionResult ViewMenuInRole()
        {
            try
            {
                viewModel = new MenuViewModel();
                PopulateAllDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewMenuInRole(MenuViewModel viewModel)
        {
            try
            {
                if (viewModel.Role != null && viewModel.Role.Id > 0)
                {
                    MenuInRoleLogic menuInRoleLogic = new MenuInRoleLogic();
                    viewModel.MenuInRoleList = menuInRoleLogic.GetModelsBy(m => m.Role_Id == viewModel.Role.Id);

                    RetainDropDown(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }
        public ActionResult EditMenuInRole(int mid)
        {
            try
            {
                viewModel = new MenuViewModel();
                if (mid > 0)
                {
                    MenuInRoleLogic menuInRoleLogic = new MenuInRoleLogic();
                    viewModel.MenuInRole = menuInRoleLogic.GetModelBy(x => x.Menu_In_Role_Id == mid);
                    if (viewModel.MenuInRole != null)
                    {
                        viewModel.Menu = viewModel.MenuInRole.Menu;
                        viewModel.Role = viewModel.MenuInRole.Role;
                    }

                    RetainDropDown(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditMenuInRole(MenuViewModel viewModel)
        {
            try
            {
                MenuInRoleLogic menuInRoleLogic = new MenuInRoleLogic();
                viewModel.MenuInRole.Menu = viewModel.Menu;
                viewModel.MenuInRole.Role = viewModel.Role;
                bool isUpdated = menuInRoleLogic.Modify(viewModel.MenuInRole);

                if (isUpdated == false)
                {
                    SetMessage("Edit Unsuccessful! ", Message.Category.Error);
                    return RedirectToAction("EditMenuInRole", new { mid = viewModel.Menu.Id });
                }

                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewMenuInRole");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View("ViewMenuInRole", viewModel);
        }

        public ActionResult ConfirmDeleteMenuInRole(int mid)
        {
            try
            {
                viewModel = new MenuViewModel();
                if (mid > 0)
                {
                    MenuInRoleLogic menuInRoleLogic = new MenuInRoleLogic();
                    viewModel.MenuInRole = menuInRoleLogic.GetModelBy(x => x.Menu_In_Role_Id == mid);
                    if (viewModel.MenuInRole != null)
                    {
                        viewModel.Menu = viewModel.MenuInRole.Menu;
                        viewModel.Role = viewModel.MenuInRole.Role;
                    }

                    RetainDropDown(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDown(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteMenuInRole(MenuViewModel viewModel)
        {
            try
            {
                MenuInRoleLogic menuInRoleLogic = new MenuInRoleLogic();
                menuInRoleLogic.Delete(x => x.Menu_In_Role_Id == viewModel.MenuInRole.Id);

                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewMenuInRole");

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ConfirmDeleteMenuInRole", new { mid = viewModel.Menu.Id });
        }

    }
}