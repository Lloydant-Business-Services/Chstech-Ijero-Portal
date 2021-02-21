using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Security.Controllers
{
	
	public class AccountController : BaseController
	{
		public ActionResult Home()
		{
			return View();
		}

		public ActionResult ChangePassword()
		{
			ManageUserViewModel manageUserviewModel = new ManageUserViewModel();
			
			try
			{
				ViewBag.UserId = User.Identity.Name;
				manageUserviewModel.Username = User.Identity.Name;
			}
			catch (Exception)
			{
				throw;
			}
			return View(manageUserviewModel);
		}
		[HttpPost]
		public ActionResult ChangePassword(ManageUserViewModel manageUserviewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{
					UserLogic userLogic = new UserLogic();
					Abundance_Nk.Model.Model.User LoggedInUser = new Model.Model.User();
					LoggedInUser = userLogic.GetModelBy(u => u.User_Name == manageUserviewModel.Username && u.Password == manageUserviewModel.OldPassword);
					if (LoggedInUser != null)
					{
						LoggedInUser.Password = manageUserviewModel.NewPassword;
						userLogic.ChangeUserPassword(LoggedInUser);
						SetMessage("Password Changed successfully! Please keep password in a safe place", Message.Category.Information);
						return RedirectToAction("Home", "Account", new { Area = "Security" });
					}
					else
					{
						SetMessage("Please log off and log in then try again.", Message.Category.Error);
					}
				   
					return View(manageUserviewModel);
				}
			}
			catch (Exception)
			{
				throw;
			}
			return View();
		}

		[AllowAnonymous]
		public ActionResult Login(string ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Login(LoginViewModel viewModel, string returnUrl)
		{
			try
			{
				if (viewModel.UserName.Contains("/"))
				{
					StudentLogic studentLogic = new StudentLogic();
					if (studentLogic.ValidateUser(viewModel.UserName, viewModel.Password))
					{
						FormsAuthentication.SetAuthCookie(viewModel.UserName, false);
						var student = studentLogic.GetBy(viewModel.UserName);
						Session["student"] = student;
						if (string.IsNullOrEmpty(returnUrl))
						{
							return RedirectToAction("Index", "Home", new {Area = "Student"});
						}
						return RedirectToLocal(returnUrl);
					}
				}
				else
				{
					UserLogic userLogic = new UserLogic();
					if (userLogic.ValidateUser(viewModel.UserName, viewModel.Password))
					{
						FormsAuthentication.SetAuthCookie(viewModel.UserName, false);

						if (string.IsNullOrEmpty(returnUrl))
						{
							return RedirectToAction("Index", "Profile", new {Area = "Admin"});
						}
						else
						{
							return RedirectToLocal(returnUrl);
						}

					}
				}
			}
			catch (Exception ex)
			{
				SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
			}

			SetMessage("Invalid Username or Password!", Message.Category.Error);
			return View();
		}

		[HttpPost]
		public ActionResult LogOff()
		{
			FormsAuthentication.SignOut();
			System.Web.HttpContext.Current.Session.Clear();
            TempData.Clear();
            return RedirectToAction("Login", "Account", new { Area = "Security" });
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}
        [AllowAnonymous]
        public JsonResult ResetPassword(string user, string userMail)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(userMail))
                {
                    StudentLogic studentLogic = new StudentLogic();
                    Model.Model.Student student = studentLogic.GetModelsBy(u => u.Matric_Number == user.Trim()).LastOrDefault();

                    if (student != null)
                    {
                        student.PasswordHash = "1234567";

                        studentLogic.Modify(student);

                        string msgBody = "Hello " + student.FullName + ", \n Your password for access into The Federal Polytechnic Ado-Ekiti Web Portal has been reset. \n Your new password " +
                                            "is '1234567'. \n Ensure to change your password immediately you login.\n Thank you for choosing our school.";

                        EmailClient emailClient = new EmailClient("Fedpoly Ado-Ekiti Portal: Password Reset", msgBody, userMail);

                        result.IsError = false;
                        result.Message = "Your password has been reset, follow the instructions in the mail sent to you to see your new password.";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "User/Student with this username was not found.";
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Parameter not set!";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}