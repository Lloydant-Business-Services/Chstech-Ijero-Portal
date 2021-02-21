using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Models;
using Microsoft.Ajax.Utilities;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class FeeDetailController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        //private const string FEENAME = "Name"+"-"+ "Amount";
        private const string FEENAME = "Name";
        private const string FEEID = "Id";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private FeeDetailViewModel viewmodel;
        public bool IsError { get; set; }

        public ActionResult Index()
        {
            viewmodel = new FeeDetailViewModel();
            try
            {
                viewmodel.GetFeeDetails();
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(viewmodel);
        }

        public ActionResult Create()
        {
            viewmodel = new FeeDetailViewModel();
            try
            {
                ViewBag.FeeTypeId = viewmodel.FeeTypeSelectListItem;
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.LevelId = viewmodel.LevelSelectListItem;
                ViewBag.PaymentModeId = viewmodel.PaymentModeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.feeId = viewmodel.feeSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FeeDetailViewModel model)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                FeeDetail feeDetail = new FeeDetail();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                feeDetail.Department = model.Department;
                feeDetail.Fee = model.fee;
                feeDetail.FeeType = model.feeType;
                feeDetail.Level = model.level;
                feeDetail.PaymentMode = model.PaymentMode;
                feeDetail.Programme = model.Programme;
                feeDetail.Session = model.CurrentSession;
                feeDetailLogic.Create(feeDetail);
                //}

                viewmodel = new FeeDetailViewModel();
                ViewBag.FeeTypeId = viewmodel.FeeTypeSelectListItem;
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.LevelId = viewmodel.LevelSelectListItem;
                ViewBag.PaymentModeId = viewmodel.PaymentModeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.feeId = viewmodel.feeSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }


            return View(model);
        }

        public JsonResult GetDepartmentByProgrammeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };
                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);
                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult AllSchoolFees()
        {

            try
            {
                FeeType feeType = new FeeType() { Id = 3 };
                Session session = new Model.Model.Session() { Id = 1 };
                PaymentMode paymentMode = new PaymentMode() { Id = 1 };

                List<DepartmentalSchoolFees> DepartmentalFees = new List<DepartmentalSchoolFees>();
                List<DepartmentalSchoolFees> DepartmentalFeeXXX = new List<DepartmentalSchoolFees>();
                DepartmentalSchoolFees DepartmentalFee = new DepartmentalSchoolFees();

                List<Level> levels = new List<Level>();
                LevelLogic leveLogic = new LevelLogic();
                levels = leveLogic.GetAll();

                List<ProgrammeDepartment> programmeDepartments = new List<ProgrammeDepartment>();
                ProgrammeDepartmentLogic programDeptLogic = new ProgrammeDepartmentLogic();
                programmeDepartments = programDeptLogic.GetAll();

                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                foreach (Level level in levels)
                {
                    foreach (ProgrammeDepartment progDept in programmeDepartments)
                    {
                        DepartmentalFee = new DepartmentalSchoolFees();
                        DepartmentalFee.Amount = feeDetailLogic.GetFeeByDepartmentLevel(progDept.Department, level, progDept.Programme, feeType, session, paymentMode);
                        DepartmentalFee.department = progDept.Department;
                        DepartmentalFee.feetype = feeType;
                        DepartmentalFee.level = level;
                        DepartmentalFee.programme = progDept.Programme;
                        if (DepartmentalFee.Amount > 0)
                        {
                            DepartmentalFees.Add(DepartmentalFee);
                        }

                    }

                }
                return View(DepartmentalFees);

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        public ActionResult AllFeeDetails()
        {
            try
            {
                FeeDetailViewModel viewModel = new FeeDetailViewModel();
                ViewBag.FeeTypeId = viewModel.FeeTypeSelectListItem;

                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpPost]
        public ActionResult AllFeeDetails(FeeDetailViewModel viewModel)
        {
            try
            {
                FeeType feeType = new FeeType() { Id = viewModel.feeType.Id };
                FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                Session session = new Model.Model.Session() { Id = 1 };
                PaymentMode paymentMode = new PaymentMode() { Id = 1 };

                List<DepartmentalSchoolFees> DepartmentalFees = new List<DepartmentalSchoolFees>();
                List<DepartmentalSchoolFees> DepartmentalFeeXXX = new List<DepartmentalSchoolFees>();
                DepartmentalSchoolFees DepartmentalFee = new DepartmentalSchoolFees();

                List<Level> levels = new List<Level>();
                LevelLogic leveLogic = new LevelLogic();
                levels = leveLogic.GetAll();

                List<ProgrammeDepartment> programmeDepartments = new List<ProgrammeDepartment>();
                ProgrammeDepartmentLogic programDeptLogic = new ProgrammeDepartmentLogic();
                programmeDepartments = programDeptLogic.GetAll();

                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                foreach (Level level in levels)
                {
                    foreach (ProgrammeDepartment progDept in programmeDepartments)
                    {
                        DepartmentalFee = new DepartmentalSchoolFees();
                        DepartmentalFee.Amount = feeDetailLogic.GetFeeByDepartmentLevel(progDept.Department, level, progDept.Programme, feeType, session, paymentMode);
                        DepartmentalFee.department = progDept.Department;
                        DepartmentalFee.feetype = feeType;
                        DepartmentalFee.level = level;
                        DepartmentalFee.programme = progDept.Programme;
                        DepartmentalFees.Add(DepartmentalFee);


                    }

                }
                viewModel.DepartmentalSchoolFeeList = DepartmentalFees;
                ViewBag.FeeTypeId = new SelectList(feeTypeLogic.GetAll(), ID, NAME, viewModel.feeType.Id);
                return View(viewModel);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult ManageFees()
        {
            try
            {
                viewmodel = new FeeDetailViewModel();
                ViewBag.Departments = viewmodel.DepartmentSelectListItem;
                ViewBag.Programmes = viewmodel.ProgrammeSelectListItem;
                ViewBag.FeeTypes = viewmodel.FeeTypeSelectListItem;
                ViewBag.Sessions = viewmodel.SessionSelectListItem;
                viewmodel.feeDetail = new FeeDetail();
                viewmodel.feeDetail.Department = new Department();
                viewmodel.feeDetail.FeeType = new FeeType();
                viewmodel.feeDetail.Level = new Level();
                viewmodel.feeDetail.PaymentMode = new PaymentMode();
                viewmodel.feeDetail.Programme = new Programme();
                viewmodel.feeDetail.Session = new Session();
            }
            catch (Exception ex)
            {
                SetMessage("Error:" + ex.Message, Message.Category.Information);
            }
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult ManageFees(FeeDetailViewModel viewModel)
        {
            try
            {
                var feeDetailLogic = new FeeDetailLogic();
                viewModel.feeDetails = feeDetailLogic.GetFee(viewModel.feeDetail.Department,
                    viewModel.feeDetail.Programme, viewModel.feeDetail.FeeType, viewModel.feeDetail.Session);
                var feeDetail = new FeeDetail();
                feeDetail.Department = viewModel.feeDetail.Department;
                feeDetail.Programme = viewModel.feeDetail.Programme;
                feeDetail.PaymentMode = new PaymentMode();
                feeDetail.Session = viewModel.feeDetail.Session;
                feeDetail.Fee = new Fee();
                feeDetail.Level = new Level();
                feeDetail.FeeType = viewModel.feeDetail.FeeType;

                int blankCount = 5;
                if (viewModel.feeDetails.Count < 1 && viewModel.feeDetails.Count < 1)
                {
                    blankCount = 15;
                }
                for (int i = 0; i < blankCount; i++)
                {
                    viewModel.feeDetails.Add(feeDetail);
                }

                RetainDropdown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error:" + ex.Message, Message.Category.Information);
            }
            return View(viewModel);
        }

        public void RetainDropdown(FeeDetailViewModel vModel)
        {
            try
            {
                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);

                if (vModel.feeDetail.Department != null && vModel.feeDetail.Department.Id > 0)
                {
                    ViewBag.Departments = new SelectList(vModel.DepartmentSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.feeDetail.Department.Id);
                }
                else
                {
                    ViewBag.Departments = viewmodel.DepartmentSelectListItem;
                }

                if (vModel.feeDetail.Programme != null && vModel.feeDetail.Programme.Id > 0)
                {
                    ViewBag.Programmes = new SelectList(vModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.feeDetail.Programme.Id);
                }
                else
                {
                    ViewBag.Programmes = vModel.ProgrammeSelectListItem;
                }
                if (vModel.feeDetail.FeeType != null && vModel.feeDetail.FeeType.Id > 0)
                {
                    ViewBag.FeeTypes = new SelectList(vModel.FeeTypeSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.feeDetail.FeeType.Id);
                }
                else
                {
                    ViewBag.FeeTypes = vModel.FeeTypeSelectListItem;
                }
                if (vModel.feeDetail.Session != null && vModel.feeDetail.Session.Id > 0)
                {
                    ViewBag.Sessions = new SelectList(vModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.feeDetail.Session.Id);
                }
                else
                {
                    ViewBag.Sessions = vModel.ProgrammeSelectListItem;
                }

                if (vModel.feeDetails != null)
                {
                    for (int i = 0; i < vModel.feeDetails.Count; i++)
                    {
                        if (vModel.feeDetails[i].Fee != null && vModel.feeDetails[i].Fee.Id > 0)
                        {
                            ViewData["FeeIdViewData" + i] = new SelectList(vModel.feeSelectListItem, VALUE, TEXT,
                                vModel.feeDetails[i].Fee.Id);
                        }
                        else
                        {
                            ViewData["FeeIdViewData" + i] = new SelectList(vModel.feeSelectListItem, VALUE, TEXT, 0);
                        }
                        if (vModel.feeDetails[i].PaymentMode != null && vModel.feeDetails[i].PaymentMode.Id > 0)
                        {
                            ViewData["PaymentModeIdViewData" + i] = new SelectList(vModel.PaymentModeSelectListItem,
                                VALUE, TEXT, vModel.feeDetails[i].PaymentMode.Id);
                        }
                        else
                        {
                            ViewData["PaymentModeIdViewData" + i] = new SelectList(vModel.PaymentModeSelectListItem,
                                VALUE, TEXT, 0);
                        }
                        if (vModel.feeDetails[i].Level != null && vModel.feeDetails[i].Level.Id > 0)
                        {
                            ViewData["LevelIdViewData" + i] = new SelectList(vModel.LevelSelectListItem, VALUE, TEXT,
                                vModel.feeDetails[i].Level.Id);
                        }
                        else
                        {
                            ViewData["LevelIdViewData" + i] = new SelectList(vModel.LevelSelectListItem, VALUE, TEXT, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void RetainDropdownThree(FeeDetailViewModel viewModel)
        {
            try
            {
                if (viewModel.feeDetail.FeeType != null && viewModel.feeDetail.FeeType.Id > 0)
                {
                    ViewBag.FeeTypes = new SelectList(viewModel.FeeTypeSelectListItem.OrderBy(x => x.Text), Utility.VALUE, Utility.TEXT,
                        viewModel.feeDetail.FeeType.Id);
                }
                else
                {
                    ViewBag.FeeTypes = viewModel.FeeTypeSelectListItem.OrderBy(x => x.Text);
                }
                if (viewModel.feeDetail.Session != null && viewModel.feeDetail.Session.Id > 0)
                {
                    ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem.OrderBy(x => x.Text), Utility.VALUE, Utility.TEXT,
                        viewModel.feeDetail.Session.Id);
                }
                else
                {
                    ViewBag.Sessions = viewModel.SessionSelectListItem.OrderBy(x => x.Text);
                }
                if (viewModel.feeDetail.PaymentMode != null && viewModel.feeDetail.PaymentMode.Id > 0)
                {
                    ViewBag.Paymentmodes = new SelectList(viewModel.PaymentModeSelectListItem.OrderBy(x => x.Text), Utility.VALUE, Utility.TEXT,
                        viewModel.feeDetail.PaymentMode.Id);
                }
                else
                {
                    ViewBag.Paymentmodes = viewModel.PaymentModeSelectListItem.OrderBy(x => x.Text);
                }
                if (viewModel.feeDetail.Level != null && viewModel.feeDetail.Level.Id > 0)
                {
                    ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem.OrderBy(x => x.Text), Utility.VALUE, Utility.TEXT,
                        viewModel.feeDetail.Level.Id);
                }
                else
                {
                    ViewBag.Levels = viewModel.LevelSelectListItem.OrderBy(x => x.Text);
                }
                if (viewModel.feeDetail.Programme != null && viewModel.feeDetail.Programme.Id > 0)
                {
                    ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem.OrderBy(x => x.Text), Utility.VALUE, Utility.TEXT,
                        viewModel.feeDetail.Programme.Id);
                }
                else
                {
                    ViewBag.Programmes = viewModel.ProgrammeSelectListItem.OrderBy(x => x.Text);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public void RetainDropdownTwo(FeeDetailViewModel vModel)
        {
            try
            {
                if (vModel.feeDetail.FeeType != null && vModel.feeDetail.FeeType.Id > 0)
                {
                    ViewBag.FeeTypes = new SelectList(vModel.FeeTypeSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.feeDetail.FeeType.Id);
                }
                else
                {
                    ViewBag.FeeTypes = vModel.FeeTypeSelectListItem;
                }
                if (vModel.feeDetail.Session != null && vModel.feeDetail.Session.Id > 0)
                {
                    ViewBag.Sessions = new SelectList(vModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.feeDetail.Session.Id);
                }
                else
                {
                    ViewBag.Sessions = vModel.ProgrammeSelectListItem;
                }

                if (vModel.feeDetails != null)
                {
                    for (int i = 0; i < vModel.feeDetails.Count; i++)
                    {
                        if (vModel.feeDetails[i].Fee != null && vModel.feeDetails[i].Fee.Id > 0)
                        {
                            ViewData["FeeIdViewData" + i] = new SelectList(vModel.feeSelectListItem, VALUE, TEXT,
                                vModel.feeDetails[i].Fee.Id);
                        }
                        else
                        {
                            ViewData["FeeIdViewData" + i] = new SelectList(vModel.feeSelectListItem, VALUE, TEXT, 0);
                        }
                        if (vModel.feeDetails[i].PaymentMode != null && vModel.feeDetails[i].PaymentMode.Id > 0)
                        {
                            ViewData["PaymentModeIdViewData" + i] = new SelectList(vModel.PaymentModeSelectListItem,
                                VALUE, TEXT, vModel.feeDetails[i].PaymentMode.Id);
                        }
                        else
                        {
                            ViewData["PaymentModeIdViewData" + i] = new SelectList(vModel.PaymentModeSelectListItem,
                                VALUE, TEXT, 0);
                        }
                        if (vModel.feeDetails[i].Level != null && vModel.feeDetails[i].Level.Id > 0)
                        {
                            ViewData["LevelIdViewData" + i] = new SelectList(vModel.LevelSelectListItem, VALUE, TEXT,
                                vModel.feeDetails[i].Level.Id);
                        }
                        else
                        {
                            ViewData["LevelIdViewData" + i] = new SelectList(vModel.LevelSelectListItem, VALUE, TEXT, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveFeeDetailChanges(FeeDetailViewModel vModel)
        {
            try
            {
                if (vModel.feeDetails.Count > 0)
                {
                    var feeDetailLogic = new FeeDetailLogic();
                    var semester = new Semester { Id = 1 };
                    feeDetailLogic.Modify(vModel.feeDetails);
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
            SetMessage("Fees were updated successfully", Message.Category.Information);
            return RedirectToAction("ManageFees");
        }

        public ActionResult FeeSetup()
        {
            viewmodel = new FeeDetailViewModel();
            try
            {
                ViewBag.FeeTypeId = viewmodel.FeeTypeSelectListItem;
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.LevelId = viewmodel.LevelSelectListItem;
                ViewBag.PaymentModeId = viewmodel.PaymentModeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.feeId = viewmodel.feeSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewmodel);
        }
        public JsonResult PopulateSetupTables(int tableType, string feeType, string session)
        {
            JsonResultModel result = new JsonResultModel();

            List<ArrayJsonView> formSettings = new List<ArrayJsonView>();
            List<ArrayJsonView> programmeFeeModels = new List<ArrayJsonView>();
            List<ArrayJsonView> etranzactTypeModels = new List<ArrayJsonView>();
            List<ArrayJsonView> feeDetailModels = new List<ArrayJsonView>();
            List<ArrayJsonView> paymentTerminalModels = new List<ArrayJsonView>();
            List<ArrayJsonView> feeModels = new List<ArrayJsonView>();
            List<ArrayJsonView> feeTypeModels = new List<ArrayJsonView>();
            try
            {
                //1 = applicationFormSetting, 2 = applicationProgrammeFee, 3 = PaymentEtranzactType, 4 = FeeDetail, 5 = PaymentTerminal, 6 = Fee, 7 = FeeType

                if (!string.IsNullOrEmpty(feeType) && !string.IsNullOrEmpty(session))
                {
                    feeType = feeType.Trim().Replace(" ", "");
                    session = session.Trim().Replace(" ", "");
                }

                switch (tableType)
                {
                    case 1:
                        ApplicationFormSettingLogic formSettingLogic = new ApplicationFormSettingLogic();
                        result.FormSettings = formSettingLogic.GetAll();
                        for (int i = 0; i < result.FormSettings.Count; i++)
                        {
                            formSettings.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.FormSettings[i].Id.ToString(),
                                EnteredBy = result.FormSettings[i].EnteredBy != null ? result.FormSettings[i].EnteredBy.Id > 0 ? result.FormSettings[i].EnteredBy.Username : "" : "",
                                PaymentMode = result.FormSettings[i].PaymentMode.Name,
                                PaymentType = result.FormSettings[i].PaymentType.Name,
                                PersonType = result.FormSettings[i].PersonType.Name,
                                Session = result.FormSettings[i].Session.Name
                            });
                        }
                        break;

                    case 2:
                        ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                        result.ProgrammeFees = programmeFeeLogic.GetAll();
                        for (int i = 0; i < result.ProgrammeFees.Count; i++)
                        {
                            programmeFeeModels.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.ProgrammeFees[i].Id.ToString(),
                                Programme = result.ProgrammeFees[i].Programme.Name,
                                FeeType = result.ProgrammeFees[i].FeeType.Name,
                                Session = result.ProgrammeFees[i].Session.Name
                            });
                        }
                        break;

                    case 3:
                        PaymentEtranzactTypeLogic etranzactTypeLogic = new PaymentEtranzactTypeLogic();
                        result.PaymentEtranzactTypes = etranzactTypeLogic.GetAll();
                        for (int i = 0; i < result.PaymentEtranzactTypes.Count; i++)
                        {
                            etranzactTypeModels.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.PaymentEtranzactTypes[i].Id.ToString(),
                                Programme = result.PaymentEtranzactTypes[i].Programme != null ? result.PaymentEtranzactTypes[i].Programme.Name : "",
                                Level = result.PaymentEtranzactTypes[i].Level != null ? result.PaymentEtranzactTypes[i].Level.Name : "",
                                PaymentMode = result.PaymentEtranzactTypes[i].PaymentMode != null ? result.PaymentEtranzactTypes[i].PaymentMode.Name : "",
                                FeeType = result.PaymentEtranzactTypes[i].FeeType.Name,
                                Session = result.PaymentEtranzactTypes[i].Session != null ? result.PaymentEtranzactTypes[i].Session.Name : "",
                                Name = result.PaymentEtranzactTypes[i].Name
                            });
                        }
                        break;

                    case 4:
                        FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                        result.FeeDetails = feeDetailLogic.GetModelsBy(f => f.FEE_TYPE.Fee_Type_Name.Trim().Replace(" ", "").Contains(feeType) && f.SESSION.Session_Name.Trim().Replace(" ", "").Contains(session));
                        for (int i = 0; i < result.FeeDetails.Count; i++)
                        {
                            feeDetailModels.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.FeeDetails[i].Id.ToString(),
                                Fee = result.FeeDetails[i].Fee.Name + " " + result.FeeDetails[i].Fee.Amount,
                                FeeType = result.FeeDetails[i].FeeType != null ? result.FeeDetails[i].FeeType.Name : "",
                                Programme = result.FeeDetails[i].Programme != null ? result.FeeDetails[i].Programme.Name : "",
                                Level = result.FeeDetails[i].Level != null ? result.FeeDetails[i].Level.Name : "",
                                PaymentMode = result.FeeDetails[i].PaymentMode != null ? result.FeeDetails[i].PaymentMode.Name : "",
                                Department = result.FeeDetails[i].Department != null ? result.FeeDetails[i].Department.Name : "",
                                Session = result.FeeDetails[i].Session.Name
                            });
                        }
                        break;

                    case 5:
                        PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                        result.PaymentTerminals = paymentTerminalLogic.GetAll();
                        for (int i = 0; i < result.PaymentTerminals.Count; i++)
                        {
                            paymentTerminalModels.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.PaymentTerminals[i].Id.ToString(),
                                TerminalId = result.PaymentTerminals[i].TerminalId,
                                FeeType = result.PaymentTerminals[i].FeeType.Name,
                                Session = result.PaymentTerminals[i].Session.Name
                            });
                        }
                        break;
                    case 7:
                        FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                        result.FeeTypes = feeTypeLogic.GetAll();
                        for (int i = 0; i < result.FeeTypes.Count; i++)
                        {
                            feeTypeModels.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.FeeTypes[i].Id.ToString(),
                                FeeTypeName = result.FeeTypes[i].Name,
                                FeeTypeDescription = result.FeeTypes[i].Description
                            });
                        }
                        break;
                    case 6:
                        FeeLogic feeLogic = new FeeLogic();
                        result.Fees = feeLogic.GetAll();
                        for (int i = 0; i < result.Fees.Count; i++)
                        {
                            feeModels.Add(new ArrayJsonView()
                            {
                                IsError = false,
                                Id = result.Fees[i].Id.ToString(),
                                FeeName = result.Fees[i].Name + " " + result.Fees[i].Amount,
                                Amount = Convert.ToString(result.Fees[i].Amount),
                                FeeDescription = result.Fees[i].Description
                            });
                        }
                        break;
                }

                switch (tableType)
                {
                    case 1:
                        return Json(formSettings, JsonRequestBehavior.AllowGet);
                        break;
                    case 2:
                        return Json(programmeFeeModels, JsonRequestBehavior.AllowGet);
                        break;
                    case 3:
                        return Json(etranzactTypeModels, JsonRequestBehavior.AllowGet);
                        break;
                    case 4:
                        return Json(feeDetailModels, JsonRequestBehavior.AllowGet);
                        break;
                    case 5:
                        return Json(paymentTerminalModels, JsonRequestBehavior.AllowGet);
                        break;
                    case 6:
                        return Json(feeModels, JsonRequestBehavior.AllowGet);
                        break;
                    case 7:
                        return Json(feeTypeModels, JsonRequestBehavior.AllowGet);
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
        public JsonResult CreateRecord(int tableType, string myRecordArray)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ArrayJsonView arrayJsonView = serializer.Deserialize<ArrayJsonView>(myRecordArray);

                int sessionId = 0;
                int paymentModeId = 0;
                int programmeId = 0;
                int departmentId = 0;
                int feeTypeId = 0;
                int feeId = 0;
                int levelId = 0;
                string etranzactTypeName = null;
                string terminalId = null;
                string feeName = null;
                decimal amount = 0M;
                string feeDescription = null;
                string feeTypeName = null;
                string feeTypeDescription = null;

                AssignJsonArrayValues(arrayJsonView, ref sessionId, ref paymentModeId, ref programmeId, ref departmentId, ref feeTypeId, ref feeId, ref levelId, ref etranzactTypeName, ref terminalId, ref feeName, ref amount, ref feeDescription, ref feeTypeName, ref feeTypeDescription);

                UserLogic userLogic = new UserLogic();

                switch (tableType)
                {
                    case 1:
                        result.Message = CreateApplicationFormSetting(sessionId, paymentModeId, userLogic);
                        result.IsError = IsError;
                        break;
                    case 2:
                        result.Message = CreateProgrammeFee(programmeId, sessionId, feeTypeId);
                        result.IsError = IsError;
                        break;
                    case 3:
                        result.Message = CreatePaymentEtranzactType(programmeId, levelId, sessionId, feeTypeId, paymentModeId, etranzactTypeName);
                        result.IsError = IsError;
                        break;
                    case 4:
                        result.Message = CreateFeeDetail(feeId, feeTypeId, programmeId, levelId, paymentModeId, departmentId, sessionId);
                        result.IsError = IsError;
                        break;
                    case 5:
                        result.Message = CreatePaymentTerminal(sessionId, feeTypeId, terminalId);
                        result.IsError = IsError;
                        break;
                    case 6:
                        result.Message = CreateFee(feeName, amount, feeDescription);
                        result.IsError = IsError;
                        break;
                    case 7:
                        result.Message = CreateFeeType(feeTypeName, feeTypeDescription);
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
                            ApplicationFormSettingLogic formSettingLogic = new ApplicationFormSettingLogic();
                            ApplicationFormSetting formSetting = new ApplicationFormSetting();
                            if (id > 0)
                            {
                                formSetting = formSettingLogic.GetModelsBy(f => f.Application_Form_Setting_Id == id).LastOrDefault();
                                if (formSetting != null)
                                {
                                    resultModel.IsError = false;
                                    resultModel.Id = formSetting.Id.ToString();
                                    resultModel.EnteredBy = formSetting.EnteredBy != null ? formSetting.EnteredBy.Username : null;
                                    resultModel.PaymentMode = formSetting.PaymentMode.Id.ToString();
                                    resultModel.Session = formSetting.Session.Id.ToString();

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
                            int programmeFeeId = Convert.ToInt32(recordId);
                            ArrayJsonView programmeFeeModel = new ArrayJsonView();
                            ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                            ApplicationProgrammeFee programmeFee = new ApplicationProgrammeFee();
                            if (programmeFeeId > 0)
                            {
                                programmeFee = programmeFeeLogic.GetModelsBy(f => f.Application_Programme_Fee_Id == programmeFeeId).LastOrDefault();
                                if (programmeFee != null)
                                {
                                    programmeFeeModel.IsError = false;
                                    programmeFeeModel.Id = programmeFee.Id.ToString();
                                    programmeFeeModel.Programme = programmeFee.Programme.Id.ToString();
                                    programmeFeeModel.FeeType = programmeFee.FeeType.Id.ToString();
                                    programmeFeeModel.Session = programmeFee.Session.Id.ToString();

                                    return Json(programmeFeeModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    programmeFeeModel.IsError = true;
                                    programmeFeeModel.Message = "Record does not exist in the database.";
                                    return Json(programmeFeeModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                programmeFeeModel.IsError = true;
                                programmeFeeModel.Message = "Edit parameter was not set.";
                                return Json(programmeFeeModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 3:
                            int etranzactTypeId = Convert.ToInt32(recordId);
                            ArrayJsonView etranzactTypeModel = new ArrayJsonView();
                            PaymentEtranzactTypeLogic etranzactTypeLogic = new PaymentEtranzactTypeLogic();
                            PaymentEtranzactType etranzactType = new PaymentEtranzactType();
                            if (etranzactTypeId > 0)
                            {
                                etranzactType = etranzactTypeLogic.GetModelsBy(f => f.Payment_Etranzact_Type_Id == etranzactTypeId).LastOrDefault();
                                if (etranzactType != null)
                                {
                                    etranzactTypeModel.IsError = false;
                                    etranzactTypeModel.Id = etranzactType.Id.ToString();
                                    etranzactTypeModel.Programme = etranzactType.Programme != null ? etranzactType.Programme.Id.ToString() : "";
                                    etranzactTypeModel.Level = etranzactType.Level != null ? etranzactType.Level.Id.ToString() : "";
                                    etranzactTypeModel.FeeType = etranzactType.FeeType.Id.ToString();
                                    etranzactTypeModel.Session = etranzactType.Session.Id.ToString();
                                    etranzactTypeModel.Name = etranzactType.Name;

                                    return Json(etranzactTypeModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    etranzactTypeModel.IsError = true;
                                    etranzactTypeModel.Message = "Record does not exist in the database.";
                                    return Json(etranzactTypeModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                etranzactTypeModel.IsError = true;
                                etranzactTypeModel.Message = "Edit parameter was not set.";
                                return Json(etranzactTypeModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 4:
                            int feeDetailId = Convert.ToInt32(recordId);
                            ArrayJsonView feeDetailModel = new ArrayJsonView();
                            FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                            FeeDetail feeDetail = new FeeDetail();
                            if (feeDetailId > 0)
                            {
                                feeDetail = feeDetailLogic.GetModelsBy(f => f.Fee_Detail_Id == feeDetailId).LastOrDefault();
                                if (feeDetail != null)
                                {
                                    feeDetailModel.IsError = false;
                                    feeDetailModel.Id = feeDetail.Id.ToString();
                                    if (feeDetail.Programme != null && feeDetail.Programme.Id > 0)
                                    {
                                        feeDetailModel.Programme = feeDetail.Programme.Id.ToString();
                                    }
                                    feeDetailModel.FeeType = feeDetail.FeeType.Id.ToString();
                                    feeDetailModel.Session = feeDetail.Session.Id.ToString();
                                    if (feeDetail.Level != null && feeDetail.Level.Id > 0)
                                    {
                                        feeDetailModel.Level = feeDetail.Level.Id.ToString();
                                    }
                                    feeDetailModel.Fee = feeDetail.Fee.Id.ToString();
                                    if (feeDetail.Department != null && feeDetail.Department.Id > 0)
                                    {
                                        feeDetailModel.Department = feeDetail.Department.Id.ToString();
                                    }
                                    if (feeDetail.PaymentMode != null && feeDetail.PaymentMode.Id > 0)
                                    {
                                        feeDetailModel.PaymentMode = feeDetail.PaymentMode.Id.ToString();
                                    }

                                    return Json(feeDetailModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    feeDetailModel.IsError = true;
                                    feeDetailModel.Message = "Record does not exist in the database.";
                                    return Json(feeDetailModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                feeDetailModel.IsError = true;
                                feeDetailModel.Message = "Edit parameter was not set.";
                                return Json(feeDetailModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 5:
                            int paymentTerminalId = Convert.ToInt32(recordId);
                            ArrayJsonView paymentTerminalModel = new ArrayJsonView();
                            PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                            PaymentTerminal paymentTerminal = new PaymentTerminal();
                            if (paymentTerminalId > 0)
                            {
                                paymentTerminal = paymentTerminalLogic.GetModelsBy(f => f.Payment_Terminal_Id == paymentTerminalId).LastOrDefault();
                                if (paymentTerminal != null)
                                {
                                    paymentTerminalModel.IsError = false;
                                    paymentTerminalModel.Id = paymentTerminal.Id.ToString();
                                    paymentTerminalModel.TerminalId = paymentTerminal.TerminalId;
                                    paymentTerminalModel.FeeType = paymentTerminal.FeeType.Id.ToString();
                                    paymentTerminalModel.Session = paymentTerminal.Session.Id.ToString();

                                    return Json(paymentTerminalModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    paymentTerminalModel.IsError = true;
                                    paymentTerminalModel.Message = "Record does not exist in the database.";
                                    return Json(paymentTerminalModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                paymentTerminalModel.IsError = true;
                                paymentTerminalModel.Message = "Edit parameter was not set.";
                                return Json(paymentTerminalModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 6:
                            int feeId = Convert.ToInt32(recordId);
                            ArrayJsonView feeModel = new ArrayJsonView();
                            FeeLogic feeLogic = new FeeLogic();
                            Fee fee = new Fee();
                            if (feeId > 0)
                            {
                                fee = feeLogic.GetModelsBy(f => f.Fee_Id == feeId).LastOrDefault();
                                if (fee != null)
                                {
                                    feeModel.IsError = false;
                                    feeModel.Id = fee.Id.ToString();
                                    feeModel.FeeName = fee.Name;
                                    feeModel.Amount = fee.Amount.ToString();
                                    feeModel.FeeDescription = fee.Description;

                                    return Json(feeModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    feeModel.IsError = true;
                                    feeModel.Message = "Record does not exist in the database.";
                                    return Json(feeModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                feeModel.IsError = true;
                                feeModel.Message = "Edit parameter was not set.";
                                return Json(feeModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        case 7:
                            int feeTypeId = Convert.ToInt32(recordId);
                            ArrayJsonView feeTypeModel = new ArrayJsonView();
                            FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                            FeeType feeType = new FeeType();
                            if (feeTypeId > 0)
                            {
                                feeType = feeTypeLogic.GetModelsBy(f => f.Fee_Type_Id == feeTypeId).LastOrDefault();
                                if (feeType != null)
                                {
                                    feeTypeModel.IsError = false;
                                    feeTypeModel.Id = feeType.Id.ToString();
                                    feeTypeModel.FeeTypeName = feeType.Name;
                                    feeTypeModel.FeeTypeDescription = feeType.Description;

                                    return Json(feeTypeModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    feeTypeModel.IsError = true;
                                    feeTypeModel.Message = "Record does not exist in the database.";
                                    return Json(feeTypeModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                feeTypeModel.IsError = true;
                                feeTypeModel.Message = "Edit parameter was not set.";
                                return Json(feeTypeModel, JsonRequestBehavior.AllowGet);
                            }
                            break;
                    }
                }
                else if (action == "save")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    ArrayJsonView arrayJsonView = serializer.Deserialize<ArrayJsonView>(myRecordArray);

                    int sessionId = 0;
                    int paymentModeId = 0;
                    int programmeId = 0;
                    int departmentId = 0;
                    int feeTypeId = 0;
                    int feeId = 0;
                    int levelId = 0;
                    string etranzactTypeName = null;
                    string terminalId = null;
                    string feeName = null;
                    decimal amount = 0M;
                    string feeDescription = null;
                    string feeTypeName = null;
                    string feeTypeDescription = null;

                    AssignJsonArrayValues(arrayJsonView, ref sessionId, ref paymentModeId, ref programmeId, ref departmentId, ref feeTypeId, ref feeId, ref levelId, ref etranzactTypeName, ref terminalId, ref feeName, ref amount, ref feeDescription, ref feeTypeName, ref feeTypeDescription);

                    switch (tableType)
                    {
                        case 1:
                            int id = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveApplicationFormSetting(sessionId, paymentModeId, id);
                            result.IsError = IsError;
                            break;
                        case 2:
                            int programmeFeeId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveApplicationProgrammeFee(programmeId, sessionId, feeTypeId, programmeFeeId);
                            result.IsError = IsError;
                            break;
                        case 3:
                            int etranzactTypeId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SavePaymentEtranzactType(programmeId, levelId, sessionId, feeTypeId, paymentModeId, etranzactTypeName, etranzactTypeId);
                            result.IsError = IsError;
                            break;
                        case 4:
                            int feeDetailId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveFeeDetail(feeId, feeTypeId, programmeId, levelId, paymentModeId, departmentId, sessionId, feeDetailId);
                            result.IsError = IsError;
                            break;
                        case 5:
                            int paymentTerminalId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SavePaymentTerminal(sessionId, feeTypeId, terminalId, paymentTerminalId);
                            result.IsError = IsError;
                            break;
                        case 6:
                            int currentFeeId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveFee(feeName, amount, feeDescription, currentFeeId);
                            result.IsError = IsError;
                            break;
                        case 7:
                            int currentFeeTypeId = Convert.ToInt32(arrayJsonView.Id);
                            result.Message = SaveFeeType(feeTypeName, feeTypeDescription, currentFeeTypeId);
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
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (tableType > 0 && !string.IsNullOrEmpty(recordId))
                {
                    switch (tableType)
                    {
                        case 1:
                            int id = Convert.ToInt32(recordId);
                            ApplicationFormSettingLogic formSettingLogic = new ApplicationFormSettingLogic();
                            var deletedAppFormSettingAudit = formSettingLogic.GetModelBy(x => x.Application_Form_Setting_Id == id);
                            bool deleted = formSettingLogic.Delete(f => f.Application_Form_Setting_Id == id);
                            if (deleted)
                            {
                                result.IsError = false;
                                if (deletedAppFormSettingAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "FormSettingId=" + deletedAppFormSettingAudit.Id + "SessionId=" + deletedAppFormSettingAudit.Session.Id + "PaymentModeId" + deletedAppFormSettingAudit.PaymentMode.Id + "PaymentTypeId=" + deletedAppFormSettingAudit.PaymentType.Id;
                                    generalAudit.Operation = "Delete Form Setting";
                                    generalAudit.TableNames = "FormSetting Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }

                            break;
                        case 2:
                            int programmeFeeId = Convert.ToInt32(recordId);
                            ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                            var deletedAppProgrammeFeeAudit = programmeFeeLogic.GetModelBy(x => x.Application_Programme_Fee_Id == programmeFeeId);
                            bool deletedProgrammeFee = programmeFeeLogic.Delete(f => f.Application_Programme_Fee_Id == programmeFeeId);
                            if (deletedProgrammeFee)
                            {
                                result.IsError = false;
                                if (deletedAppProgrammeFeeAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "AppProgrammeFeeId="+ deletedAppProgrammeFeeAudit.Id + "FeeTypeId=" + deletedAppProgrammeFeeAudit.FeeType.Id + "SessionId=" + deletedAppProgrammeFeeAudit.Session.Id + "ProgrammeId" + deletedAppProgrammeFeeAudit.Programme.Id;
                                    generalAudit.Operation = "Delete Programme Fee";
                                    generalAudit.TableNames = "Programme Fee Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 3:
                            int etranzactTypeId = Convert.ToInt32(recordId);
                            PaymentEtranzactTypeLogic etranzactTypeLogic = new PaymentEtranzactTypeLogic();
                            var deletedPaymentEtranzactTypeAudit = etranzactTypeLogic.GetModelBy(x => x.Payment_Etranzact_Type_Id == etranzactTypeId);
                            bool deletedEtranzactType = etranzactTypeLogic.Delete(f => f.Payment_Etranzact_Type_Id == etranzactTypeId);
                            if (deletedEtranzactType)
                            {
                                result.IsError = false;
                                if (deletedPaymentEtranzactTypeAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "PaymentEtranzactTypeId="+ deletedPaymentEtranzactTypeAudit.Id + "FeeTypeId=" + deletedPaymentEtranzactTypeAudit.FeeType.Id + "SessionId=" + deletedPaymentEtranzactTypeAudit.Session.Id + "ProgrammeId" + deletedPaymentEtranzactTypeAudit.Programme.Id + "EtranzactTypeName=" + deletedPaymentEtranzactTypeAudit.Name;
                                    generalAudit.Operation = "Delete PaymentEtranzactType";
                                    generalAudit.TableNames = "PaymentEtranzactType Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 4:
                            int feeDetailId = Convert.ToInt32(recordId);
                            FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                            var deletedFeeDetailAudit = feeDetailLogic.GetModelBy(x => x.Fee_Detail_Id == feeDetailId);
                            bool deletedFeeDetail = feeDetailLogic.Delete(f => f.Fee_Detail_Id == feeDetailId);
                            if (deletedFeeDetail)
                            {
                                result.IsError = false;
                                if (deletedFeeDetailAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.CurrentValues = "FeeDetail="+ deletedFeeDetailAudit.Id+ "FeeId=" + deletedFeeDetailAudit.Fee.Id + "DepartmentId=" + deletedFeeDetailAudit.Department.Id + "PaymentModeId" + deletedFeeDetailAudit.PaymentMode.Id;
                                    generalAudit.Operation = "Delete FeeDetail";
                                    generalAudit.TableNames = "FeeDetail Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 5:
                            int paymentTerminalId = Convert.ToInt32(recordId);
                            PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                            var deletedPaymentTerminalAudit = paymentTerminalLogic.GetModelBy(x => x.Payment_Terminal_Id == paymentTerminalId);
                            bool deletedPaymentTerminal = paymentTerminalLogic.Delete(f => f.Payment_Terminal_Id == paymentTerminalId);
                            if (deletedPaymentTerminal)
                            {
                                result.IsError = false;
                                if (deletedPaymentTerminalAudit != null)
                                {
                                    //Create Audit For Operation
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues = "PaymentTerminalId="+ deletedPaymentTerminalAudit.Id+ "FeeTypeId=" + deletedPaymentTerminalAudit.FeeType.Id + "SessionId=" + deletedPaymentTerminalAudit.Session.Id + "LastTerminalId" + deletedPaymentTerminalAudit.Id;
                                    generalAudit.Operation = "Delete PaymentTerminal";
                                    generalAudit.TableNames = "PaymentTerminal Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 6:
                            int feeId = Convert.ToInt32(recordId);
                            FeeLogic feeLogic = new FeeLogic();
                            var deletedFeeAudit = feeLogic.GetModelBy(x => x.Fee_Id == feeId);
                            bool deletedFee = feeLogic.Delete(f => f.Fee_Id == feeId);
                            if (deletedFee)
                            {
                                result.IsError = false;
                                if (deletedFeeAudit != null)
                                {
                                    //Creat Audit
                                    generalAudit.Action = "Delete";
                                    generalAudit.InitialValues ="FeeId"+ deletedFeeAudit.Id + "FeeName=" + deletedFeeAudit.Name + "Amount=" + deletedFeeAudit.Amount;
                                    generalAudit.Operation = "Delete Fee";
                                    generalAudit.TableNames = "Fee Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Delete operation failed.";
                            }
                            break;
                        case 7:
                            int feeTypeId = Convert.ToInt32(recordId);
                            FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                            var deletedFeeTypeAudit = feeTypeLogic.GetModelBy(x => x.Fee_Type_Id == feeTypeId);
                            bool deletedFeeType = feeTypeLogic.Delete(f => f.Fee_Type_Id == feeTypeId);
                            if (deletedFeeType)
                            {
                                result.IsError = false;
                                if (deletedFeeTypeAudit != null)
                                {
                                    //Creat Audit
                                    generalAudit.Action = "Delete";
                                    generalAudit.CurrentValues ="FeeTypeId="+ deletedFeeTypeAudit.Id + "FeeTypeName=" + deletedFeeTypeAudit.Name + "FeeTypeDescription=" + deletedFeeTypeAudit.Description;
                                    generalAudit.Operation = "Delete FeeType";
                                    generalAudit.TableNames = "FeeType Table";
                                    generalAuditLogic.CreateGeneralAudit(generalAudit);
                                }
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

        public void AssignJsonArrayValues(ArrayJsonView arrayJsonView, ref int sessionId, ref int paymentModeId, ref int programmeId, ref int departmentId, ref int feeTypeId, ref int feeId, ref int levelId, ref string etranzactTypeName, ref string terminalId, ref string feeName, ref decimal amount, ref string feeDescription, ref string feeTypeName, ref string feeTypeDescription)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrayJsonView.Session))
                {
                    sessionId = Convert.ToInt32(arrayJsonView.Session);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.PaymentMode))
                {
                    paymentModeId = Convert.ToInt32(arrayJsonView.PaymentMode);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Programme))
                {
                    programmeId = Convert.ToInt32(arrayJsonView.Programme);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Department))
                {
                    departmentId = Convert.ToInt32(arrayJsonView.Department);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.FeeType))
                {
                    feeTypeId = Convert.ToInt32(arrayJsonView.FeeType);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Fee))
                {
                    feeId = Convert.ToInt32(arrayJsonView.Fee);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Level))
                {
                    levelId = Convert.ToInt32(arrayJsonView.Level);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.EtranzactTypeName))
                {
                    etranzactTypeName = arrayJsonView.EtranzactTypeName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.TerminalId))
                {
                    terminalId = arrayJsonView.TerminalId;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.FeeName))
                {
                    feeName = arrayJsonView.FeeName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.Amount))
                {
                    amount = Convert.ToDecimal(arrayJsonView.Amount);
                }
                if (!string.IsNullOrEmpty(arrayJsonView.FeeDescription))
                {
                    feeDescription = arrayJsonView.FeeDescription;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.FeeTypeName))
                {
                    feeTypeName = arrayJsonView.FeeTypeName;
                }
                if (!string.IsNullOrEmpty(arrayJsonView.FeeTypeDescription))
                {
                    feeTypeDescription = arrayJsonView.FeeTypeDescription;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CreateFeeType(string feeTypeName, string feeTypeDescription)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(feeTypeName))
                {
                    FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                    FeeType existingFeeType = feeTypeLogic.GetModelsBy(p => p.Fee_Type_Name == feeTypeName).LastOrDefault();

                    if (existingFeeType == null)
                    {
                        existingFeeType = new FeeType();
                        existingFeeType.Name = feeTypeName;
                        existingFeeType.Description = feeTypeDescription;

                        feeTypeLogic.Create(existingFeeType);

                        IsError = false;
                        message = "FeeType created successfully.";
                        //Creat Audit
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "FeeTypeName=" + feeTypeName + "FeeTypeDescription=" + feeTypeDescription;
                        generalAudit.Operation = "Created New FeeType";
                        generalAudit.TableNames = "FeeType Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
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

        private string CreateFee(string feeName, decimal amount, string feeDescription)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (amount > 0M && !string.IsNullOrEmpty(feeName))
                {
                    FeeLogic feeLogic = new FeeLogic();
                    Fee existingFee = feeLogic.GetModelsBy(p => p.Fee_Name == feeName && p.Amount == amount).LastOrDefault();

                    if (existingFee == null)
                    {
                        existingFee = new Fee();
                        existingFee.Name = feeName;
                        existingFee.Amount = amount;
                        existingFee.Description = feeDescription;

                        feeLogic.Create(existingFee);

                        IsError = false;
                        message = "Fee created successfully.";
                        //Creat Audit
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "FeeName=" + feeName + "Amount=" + amount;
                        generalAudit.Operation = "Created New Fee";
                        generalAudit.TableNames = "Fee Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);


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

        private string CreateFeeDetail(int feeId, int feeTypeId, int programmeId, int levelId, int paymentModeId, int departmentId, int sessionId)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (feeId > 0 && feeTypeId > 0 && sessionId > 0)
                {
                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                    FeeDetail existingFeeDetail = new FeeDetail();
                    if (programmeId > 0 && levelId > 0 && paymentModeId > 0 && departmentId > 0)
                    {
                        existingFeeDetail = feeDetailLogic.GetModelsBy(f => f.Session_Id == sessionId && f.Department_Id == departmentId && f.Fee_Id == feeId && f.Fee_Type_Id == feeTypeId && f.Level_Id == levelId && f.Payment_Mode_Id == paymentModeId && f.Programme_Id == programmeId).LastOrDefault();
                    }
                    else
                    {
                        existingFeeDetail = feeDetailLogic.GetModelsBy(f => f.Session_Id == sessionId && f.Fee_Id == feeId && f.Fee_Type_Id == feeTypeId).LastOrDefault();
                    }
                    if (existingFeeDetail == null)
                    {
                        existingFeeDetail = new FeeDetail();
                        existingFeeDetail.Fee = new Fee() { Id = feeId };
                        existingFeeDetail.FeeType = new FeeType() { Id = feeTypeId };
                        existingFeeDetail.Session = new Session() { Id = sessionId };
                        if (programmeId > 0 && levelId > 0 && paymentModeId > 0 && departmentId > 0)
                        {
                            existingFeeDetail.Level = new Level() { Id = levelId };
                            existingFeeDetail.PaymentMode = new PaymentMode() { Id = paymentModeId };
                            existingFeeDetail.Programme = new Programme() { Id = programmeId };
                            existingFeeDetail.Department = new Department() { Id = departmentId };
                        }

                        feeDetailLogic.Create(existingFeeDetail);

                        IsError = false;
                        message = "FeeDetail created successfully.";
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "FeeId=" + existingFeeDetail.Fee.Id + "DepartmentId=" + existingFeeDetail.Department.Id+"PaymentModeId"+ existingFeeDetail.PaymentMode.Id;
                        generalAudit.Operation = "Created New FeeDetail";
                        generalAudit.TableNames = "FeeDetail Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
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

        public string CreatePaymentTerminal(int sessionId, int feeTypeId, string terminalId)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionId > 0 && feeTypeId > 0 && !string.IsNullOrEmpty(terminalId))
                {
                    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                    PaymentTerminal existingPaymentTerminal =
                        paymentTerminalLogic.GetModelsBy(p => p.Session_Id == sessionId && p.Fee_Type_Id == feeTypeId)
                            .LastOrDefault();
                    PaymentTerminal lastPaymentTerminal = paymentTerminalLogic.GetAll().LastOrDefault();

                    long lastPaymentTerminalId = 1;
                    if (lastPaymentTerminal != null)
                    {
                        lastPaymentTerminalId = lastPaymentTerminal.Id + 1;
                    }

                    if (existingPaymentTerminal == null)
                    {
                        existingPaymentTerminal = new PaymentTerminal();
                        existingPaymentTerminal.Id = lastPaymentTerminalId;
                        existingPaymentTerminal.FeeType = new FeeType() { Id = feeTypeId };
                        existingPaymentTerminal.Session = new Session() { Id = sessionId };
                        existingPaymentTerminal.TerminalId = terminalId;

                        paymentTerminalLogic.Create(existingPaymentTerminal);

                        IsError = false;
                        message = "Payment Terminal created successfully.";
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "FeeTypeId=" + existingPaymentTerminal.FeeType.Id + "SessionId=" + existingPaymentTerminal.Session.Id + "LastTerminalId" + existingPaymentTerminal.Id;
                        generalAudit.Operation = "Created New PaymentTerminal";
                        generalAudit.TableNames = "PaymentTerminal Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
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

        public string CreatePaymentEtranzactType(int programmeId, int levelId, int sessionId, int feeTypeId, int paymentModeId, string etranzactTypeName)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (programmeId > 0 && levelId > 0 && sessionId > 0 && feeTypeId > 0 && paymentModeId > 0 && !string.IsNullOrEmpty(etranzactTypeName))
                {
                    PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    PaymentEtranzactType existingPaymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(e => e.Session_Id == sessionId && e.Fee_Type_Id == feeTypeId && e.Payment_Mode_Id == paymentModeId && e.Programme_Id == programmeId && e.Level_Id == levelId).LastOrDefault();
                    PaymentEtranzactType lastPaymentEtranzactType = paymentEtranzactTypeLogic.GetAll().LastOrDefault();

                    int lastPaymentEtranzactTypeId = 1;

                    if (lastPaymentEtranzactType != null)
                    {
                        lastPaymentEtranzactTypeId = lastPaymentEtranzactType.Id + 1;
                    }

                    if (existingPaymentEtranzactType == null)
                    {
                        existingPaymentEtranzactType = new PaymentEtranzactType();
                        existingPaymentEtranzactType.FeeType = new FeeType() { Id = feeTypeId };
                        existingPaymentEtranzactType.PaymentMode = new PaymentMode() { Id = paymentModeId };
                        existingPaymentEtranzactType.Session = new Session() { Id = sessionId };
                        existingPaymentEtranzactType.Programme = new Programme() { Id = programmeId };
                        existingPaymentEtranzactType.Level = new Level() { Id = levelId };
                        existingPaymentEtranzactType.Id = lastPaymentEtranzactTypeId;
                        existingPaymentEtranzactType.Name = etranzactTypeName;

                        paymentEtranzactTypeLogic.Create(existingPaymentEtranzactType);
                        IsError = false;
                        message = "Payment Etranzact Type created succesfully.";
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "FeeTypeId=" + existingPaymentEtranzactType.FeeType.Id + "SessionId=" + existingPaymentEtranzactType.Session.Id + "ProgrammeId" + existingPaymentEtranzactType.Programme.Id+"EtranzactTypeName="+ etranzactTypeName;
                        generalAudit.Operation = "Created New PaymentEtranzactType";
                        generalAudit.TableNames = "PaymentEtranzactType Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        IsError = true;
                        message = "Record aleady exist.";
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

        public string CreateProgrammeFee(int programmeId, int sessionId, int feeTypeId)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (programmeId > 0 && sessionId > 0 && feeTypeId > 0)
                {
                    ApplicationProgrammeFeeLogic applicationProgrammeFeeLogic = new ApplicationProgrammeFeeLogic();
                    ApplicationProgrammeFee existingProgrammeFee = applicationProgrammeFeeLogic.GetModelsBy(a => a.Session_Id == sessionId && a.Fee_Type_Id == feeTypeId && a.Programme_Id == programmeId).LastOrDefault();
                    ApplicationProgrammeFee lastProgrammeFee = applicationProgrammeFeeLogic.GetAll().LastOrDefault();
                    int lastProgrammeFeeId = 1;
                    if (lastProgrammeFee != null)
                    {
                        lastProgrammeFeeId = lastProgrammeFee.Id + 1;
                    }

                    if (existingProgrammeFee == null)
                    {
                        existingProgrammeFee = new ApplicationProgrammeFee();
                        existingProgrammeFee.Id = lastProgrammeFeeId;
                        existingProgrammeFee.DateEntered = DateTime.Now;
                        existingProgrammeFee.FeeType = new FeeType() { Id = feeTypeId };
                        existingProgrammeFee.Programme = new Programme() { Id = programmeId };
                        existingProgrammeFee.Session = new Session() { Id = sessionId };

                        applicationProgrammeFeeLogic.Create(existingProgrammeFee);
                        IsError = false;
                        message = "Programme Fee created successfully.";
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "FeeTypeId=" + existingProgrammeFee.FeeType.Id + "SessionId=" + existingProgrammeFee.Session.Id + "ProgrammeId" + existingProgrammeFee.Programme.Id;
                        generalAudit.Operation = "Created New Programme Fee";
                        generalAudit.TableNames = "Programme Fee Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        IsError = true;
                        message = "Programme Fee already exist.";
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

        public string CreateApplicationFormSetting(int sessionId, int paymentModeId, UserLogic userLogic)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionId > 0 && paymentModeId > 0)
                {
                    ApplicationFormSettingLogic applicationFormSettingLogic = new ApplicationFormSettingLogic();

                    ApplicationFormSetting existingFormSetting = applicationFormSettingLogic.GetModelsBy(a => a.Session_Id == sessionId && a.Payment_Mode_Id == paymentModeId).LastOrDefault();
                    ApplicationFormSetting applicationFormSetting = applicationFormSettingLogic.GetAll().LastOrDefault();


                    int lastFormSettingId = 1;
                    if (applicationFormSetting != null)
                    {
                        lastFormSettingId = applicationFormSetting.Id;
                    }

                    if (existingFormSetting == null)
                    {
                        existingFormSetting = new ApplicationFormSetting();

                        existingFormSetting.Id = lastFormSettingId + 1;
                        existingFormSetting.DateEntered = DateTime.Now;
                        existingFormSetting.StartDate = DateTime.Now;
                        existingFormSetting.EndDate = DateTime.Now + new TimeSpan(30, 0, 0, 0);
                        existingFormSetting.EnteredBy = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);
                        existingFormSetting.ExamDate = existingFormSetting.EndDate;
                        existingFormSetting.ExamTime = new TimeSpan?(new TimeSpan(0, 10, 0, 0));
                        existingFormSetting.ExamVenue = "WEST CAMPUS";
                        existingFormSetting.PaymentMode = new PaymentMode() { Id = paymentModeId };
                        existingFormSetting.PaymentType = new PaymentType() { Id = (int)PaymentTypeEnum.OnlinePayment };
                        existingFormSetting.PersonType = new PersonType() { Id = (int)PersonTypes.Applicant };
                        existingFormSetting.RegistrationEndDate = existingFormSetting.EndDate;
                        existingFormSetting.RegistrationEndTime = existingFormSetting.ExamTime;
                        existingFormSetting.Session = new Session() { Id = sessionId };

                        applicationFormSettingLogic.Create(existingFormSetting);

                        message = "Form Setting created";
                        IsError = false;
                        //Create Audit For Operation
                        generalAudit.Action = "Create";
                        generalAudit.CurrentValues = "FormSettingId=" + existingFormSetting.Id + "SessionId=" + existingFormSetting.Session.Id + "PaymentModeId" + existingFormSetting.PaymentMode.Id + "PaymentTypeId=" + existingFormSetting.PaymentType.Id;
                        generalAudit.Operation = "Created New Form Setting";
                        generalAudit.TableNames = "FormSetting Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Form Setting already exist";
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
        public string SaveApplicationFormSetting(int sessionId, int paymentModeId, int id)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionId > 0)
                {
                    ApplicationFormSettingLogic applicationFormSettingLogic = new ApplicationFormSettingLogic();

                    ApplicationFormSetting existingFormSetting = applicationFormSettingLogic.GetModelsBy(a => a.Session_Id == sessionId).LastOrDefault();
                    var initialAppFormSetting=applicationFormSettingLogic.GetModelBy(x => x.Application_Form_Setting_Id == id);
                    if (existingFormSetting == null)
                    {
                        existingFormSetting = new ApplicationFormSetting();
                        
                        existingFormSetting.Id = id;
                        existingFormSetting.Session = new Session() { Id = sessionId };
                        existingFormSetting.PaymentMode = new PaymentMode() { Id = paymentModeId };

                        applicationFormSettingLogic.Modify(existingFormSetting);

                        message = "Form Setting was modified";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialAppFormSetting != null)
                        {
                            generalAudit.InitialValues = "FormSettingId=" + initialAppFormSetting.Id + "SessionId=" + initialAppFormSetting.Session.Id + "PaymentModeId" + initialAppFormSetting.PaymentMode.Id + "PaymentTypeId=" + initialAppFormSetting.PaymentType.Id;
                        }
                        generalAudit.Action = "Modify";  
                        generalAudit.CurrentValues = "FormSettingId=" + existingFormSetting.Id + "SessionId=" + existingFormSetting.Session.Id + "PaymentModeId" + existingFormSetting.PaymentMode.Id + "PaymentTypeId=" + existingFormSetting.PaymentType.Id;
                        generalAudit.Operation = "Modify Form Setting";
                        generalAudit.TableNames = "FormSetting Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Form Setting values already exist";
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
        public string SaveApplicationProgrammeFee(int programmeId, int sessionId, int feeTypeId, int id)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionId > 0 && feeTypeId > 0 && programmeId > 0)
                {
                    ApplicationProgrammeFeeLogic applicationProgrammeFeeLogic = new ApplicationProgrammeFeeLogic();

                    ApplicationProgrammeFee existingProgrammeFee = applicationProgrammeFeeLogic.GetModelsBy(a => a.Session_Id == sessionId && a.Programme_Id == programmeId && a.Fee_Type_Id == feeTypeId).LastOrDefault();
                    var initialAppProgrammeFee=applicationProgrammeFeeLogic.GetModelBy(x => x.Application_Programme_Fee_Id == id);
                    if (existingProgrammeFee == null)
                    {
                        existingProgrammeFee = new ApplicationProgrammeFee();
                        existingProgrammeFee.Id = id;
                        existingProgrammeFee.Session = new Session() { Id = sessionId };
                        existingProgrammeFee.Programme = new Programme() { Id = programmeId };
                        existingProgrammeFee.FeeType = new FeeType() { Id = feeTypeId };

                        applicationProgrammeFeeLogic.Modify(existingProgrammeFee);

                        message = "Programme Fee was modified";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialAppProgrammeFee != null)
                        {
                            generalAudit.InitialValues = "FeeTypeId=" + initialAppProgrammeFee.FeeType.Id + "SessionId=" + initialAppProgrammeFee.Session.Id + "ProgrammeId" + initialAppProgrammeFee.Programme.Id;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "FeeTypeId=" + existingProgrammeFee.FeeType.Id + "SessionId=" + existingProgrammeFee.Session.Id + "ProgrammeId" + existingProgrammeFee.Programme.Id;
                        generalAudit.Operation = "Modify New Programme Fee";
                        generalAudit.TableNames = "Programme Fee Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Programme Fee values already exist";
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
        public string SavePaymentEtranzactType(int programmeId, int levelId, int sessionId, int feeTypeId, int paymentModeId, string etranzactTypeName, int id)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionId > 0 && programmeId > 0 && levelId > 0 && feeTypeId > 0 && paymentModeId > 0 && !string.IsNullOrEmpty(etranzactTypeName))
                {
                    PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();

                    PaymentEtranzactType paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(a => a.Session_Id == sessionId && a.Programme_Id == programmeId && a.Level_Id == levelId && a.Fee_Type_Id == feeTypeId && a.Payment_Mode_Id == paymentModeId && a.Payment_Etranzact_Type_Name == etranzactTypeName).LastOrDefault();
                    var initialPaymentEtranzactType= paymentEtranzactTypeLogic.GetModelBy(x => x.Payment_Etranzact_Type_Id == id);
                    if (paymentEtranzactType == null)
                    {
                        paymentEtranzactType = new PaymentEtranzactType();

                        paymentEtranzactType.Id = id;
                        paymentEtranzactType.Session = new Session() { Id = sessionId };
                        paymentEtranzactType.PaymentMode = new PaymentMode() { Id = paymentModeId };
                        paymentEtranzactType.Programme = new Programme() { Id = programmeId };
                        paymentEtranzactType.Level = new Level() { Id = levelId };
                        paymentEtranzactType.FeeType = new FeeType() { Id = feeTypeId };
                        paymentEtranzactType.Name = etranzactTypeName;

                        paymentEtranzactTypeLogic.Modify(paymentEtranzactType);

                        message = "Etranzact Type was modified";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialPaymentEtranzactType != null)
                        {
                            generalAudit.InitialValues = "FeeTypeId=" + initialPaymentEtranzactType.FeeType.Id + "SessionId=" + initialPaymentEtranzactType.Session.Id + "ProgrammeId" + initialPaymentEtranzactType.Programme.Id + "EtranzactTypeName=" + initialPaymentEtranzactType.Name;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "FeeTypeId=" + paymentEtranzactType.FeeType.Id + "SessionId=" + paymentEtranzactType.Session.Id + "ProgrammeId" + paymentEtranzactType.Programme.Id + "EtranzactTypeName=" + etranzactTypeName;
                        generalAudit.Operation = "Modify PaymentEtranzactType";
                        generalAudit.TableNames = "PaymentEtranzactType Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited EtranzactType values already exist";
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
        public string SaveFeeDetail(int feeId, int feeTypeId, int programmeId, int levelId, int paymentModeId, int departmentId, int sessionId, int id)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionId > 0 && feeId > 0 && feeTypeId > 0)
                {
                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                    FeeDetail feeDetail = null;
                    if (programmeId == 0 && paymentModeId == 0 && levelId == 0 && departmentId == 0)
                    {
                        feeDetail = feeDetailLogic.GetModelsBy(a => a.Session_Id == sessionId && a.Fee_Type_Id == feeTypeId && a.Fee_Id == feeId).LastOrDefault();
                    }
                    else
                    {
                        feeDetail = feeDetailLogic.GetModelsBy(a => a.Session_Id == sessionId && a.Department_Id == departmentId && a.Payment_Mode_Id == paymentModeId && a.Level_Id == levelId && a.Programme_Id == programmeId && a.Fee_Type_Id == feeTypeId && a.Fee_Id == feeId).LastOrDefault();
                    }
                   var initialFeeDetail= feeDetailLogic.GetModelBy(x => x.Fee_Detail_Id == id);
                    if (feeDetail == null)
                    {
                        feeDetail = new FeeDetail();

                        feeDetail.Id = id;
                        feeDetail.Session = new Session() { Id = sessionId };
                        feeDetail.Fee = new Fee() { Id = feeId };
                        feeDetail.FeeType = new FeeType() { Id = feeTypeId };
                        if (programmeId > 0 && levelId > 0 && departmentId > 0 && paymentModeId > 0)
                        {
                            feeDetail.Programme = new Programme() { Id = programmeId };
                            feeDetail.Level = new Level() { Id = levelId };
                            feeDetail.Department = new Department() { Id = departmentId };
                            feeDetail.PaymentMode = new PaymentMode() { Id = paymentModeId };
                        }

                        feeDetailLogic.Modify(feeDetail);

                        message = "FeeDetail was modified";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialFeeDetail != null)
                        {
                            generalAudit.InitialValues = "FeeId=" + initialFeeDetail.Fee.Id + "DepartmentId=" + initialFeeDetail.Department.Id + "PaymentModeId" + initialFeeDetail.PaymentMode.Id + "LevelId" + initialFeeDetail.Level.Id + "SessionId" + initialFeeDetail.Session.Id;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "FeeId=" + feeDetail.Fee.Id + "DepartmentId=" + feeDetail.Department.Id + "PaymentModeId" + feeDetail.PaymentMode.Id+"LevelId"+ feeDetail.Level.Id+"SessionId"+ feeDetail.Session.Id;
                        generalAudit.Operation = "Modify New FeeDetail";
                        generalAudit.TableNames = "FeeDetail Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited FeeDetail values already exist";
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
        public string SavePaymentTerminal(int sessionId, int feeTypeId, string terminalId, int id)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (sessionId > 0 && feeTypeId > 0 && !string.IsNullOrEmpty(terminalId))
                {
                    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();

                    PaymentTerminal existingPaymentTerminal = paymentTerminalLogic.GetModelsBy(a => a.Session_Id == sessionId && a.Fee_Type_Id == feeTypeId && a.Terminal_Id == terminalId).LastOrDefault();
                    var initialPaymentTerminal=paymentTerminalLogic.GetModelBy(x => x.Payment_Terminal_Id == id);
                    if (existingPaymentTerminal == null)
                    {
                        existingPaymentTerminal = new PaymentTerminal();

                        existingPaymentTerminal.Id = id;
                        existingPaymentTerminal.Session = new Session() { Id = sessionId };
                        existingPaymentTerminal.FeeType = new FeeType() { Id = feeTypeId };
                        existingPaymentTerminal.TerminalId = terminalId;

                        paymentTerminalLogic.Modify(existingPaymentTerminal);

                        message = "Payment Terminal was modified";
                        IsError = false;
                        //Create Audit For Operation
                        if (initialPaymentTerminal != null)
                        {
                            generalAudit.InitialValues = "FeeTypeId=" + initialPaymentTerminal.FeeType.Id + "SessionId=" + initialPaymentTerminal.Session.Id + "TerminalId" + initialPaymentTerminal.Id;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "FeeTypeId=" + existingPaymentTerminal.FeeType.Id + "SessionId=" + existingPaymentTerminal.Session.Id + "TerminalId" + existingPaymentTerminal.Id;
                        generalAudit.Operation = "Modify New PaymentTerminal";
                        generalAudit.TableNames = "PaymentTerminal Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);

                    }
                    else
                    {
                        message = "Edited Payment Terminal values already exist";
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
        public string SaveFee(string feeName, decimal amount, string feeDescription, int id)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(feeName) && amount > 0M)
                {
                    FeeLogic feeLogic = new FeeLogic();

                    Fee existingFee = feeLogic.GetModelsBy(a => a.Fee_Name == feeName && a.Amount == amount).LastOrDefault();
                    var initialFee=feeLogic.GetModelBy(x => x.Fee_Id == id);
                    if (existingFee == null)
                    {
                        existingFee = new Fee();

                        existingFee.Id = id;
                        existingFee.Name = feeName;
                        existingFee.Amount = amount;
                        existingFee.Description = feeDescription;

                        feeLogic.Modify(existingFee);

                        message = "Fee was modified";
                        IsError = false;
                        //Creat Audit
                        if (initialFee != null)
                        {
                            generalAudit.InitialValues = "FeeName=" + initialFee.Name + "Amount=" + initialFee.Amount;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "FeeName=" + feeName + "Amount=" + amount;
                        generalAudit.Operation = "Modify Fee";
                        generalAudit.TableNames = "Fee Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited Fee values already exist";
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
        public string SaveFeeType(string feeTypeName, string feeTypeDescription, int id)
        {
            GeneralAudit generalAudit = new GeneralAudit();
            GeneralAuditLogic generalAuditLogic = new GeneralAuditLogic();
            string message = "";
            try
            {
                if (!string.IsNullOrEmpty(feeTypeName))
                {
                    FeeTypeLogic feeTypeLogic = new FeeTypeLogic();

                    FeeType existingFeeType = feeTypeLogic.GetModelsBy(a => a.Fee_Type_Name == feeTypeName).LastOrDefault();
                    var initialFeeType=feeTypeLogic.GetModelBy(x => x.Fee_Type_Id == id);
                    if (existingFeeType == null)
                    {
                        existingFeeType = new FeeType();

                        existingFeeType.Id = id;
                        existingFeeType.Name = feeTypeName;
                        existingFeeType.Description = feeTypeDescription;

                        feeTypeLogic.Modify(existingFeeType);

                        message = "FeeType was modified";
                        IsError = false;
                        //Creat Audit
                        if (initialFeeType != null)
                        {
                            generalAudit.InitialValues = "FeeTypeName=" + initialFeeType.Name + "FeeTypeDescription=" + initialFeeType.Description;
                        }
                        generalAudit.Action = "Modify";
                        generalAudit.CurrentValues = "FeeTypeName=" + feeTypeName + "FeeTypeDescription=" + feeTypeDescription;
                        generalAudit.Operation = "Modify FeeType";
                        generalAudit.TableNames = "FeeType Table";
                        generalAuditLogic.CreateGeneralAudit(generalAudit);
                    }
                    else
                    {
                        message = "Edited FeeType values already exist";
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
        public ActionResult UploadFeeDetail()
        {
            try
            {
                FeeDetailViewModel viewModel = new FeeDetailViewModel();
                ViewBag.SessionId = viewModel.SessionSelectListItem;

            }
            catch (Exception ex)
            {

                SetMessage("Error Occured" + ex.Message, Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadFeeDetail(FeeDetailViewModel viewModel)
        {
            try
            {

                string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                string savedFileName = "";

                List<FeeDetail> feeDetails = new List<FeeDetail>();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                FeeLogic feeLogic = new FeeLogic();
                FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                SessionLogic sessionLogic = new SessionLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                LevelLogic levelLogic = new LevelLogic();
                PaymentModeLogic paymentModeLogic = new PaymentModeLogic();


                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    if (hpf.ContentLength == 0)
                    {
                        continue;
                    }
                    if (CreateFolderIfNeeded(pathForSaving))
                    {
                        FileInfo fileInfo = new FileInfo(hpf.FileName);
                        string fileExtension = fileInfo.Extension;
                        string newFile = "FeeDetail" + "__";
                        string newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;

                        savedFileName = Path.Combine(pathForSaving, newFileName);
                        hpf.SaveAs(savedFileName);
                        DataSet dsFeeDetailList = ReadExcelFile(savedFileName);
                        if (dsFeeDetailList != null && dsFeeDetailList.Tables[0].Rows.Count > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < dsFeeDetailList.Tables[0].Rows.Count; i++)
                            {


                                viewModel.SN = Convert.ToInt32(dsFeeDetailList.Tables[0].Rows[i][0].ToString().Trim());
                                viewModel.fee = new Fee
                                {
                                    Id = Convert.ToInt32(dsFeeDetailList.Tables[0].Rows[i][1].ToString().Trim())
                                };
                                viewModel.fee.Name = feeLogic.GetEntityBy(f => f.Fee_Id == viewModel.fee.Id).Fee_Name;

                                viewModel.feeType = new FeeType
                                {
                                    Id = Convert.ToInt32(dsFeeDetailList.Tables[0].Rows[i][2].ToString().Trim())
                                };
                                viewModel.feeType.Name = feeTypeLogic.GetEntityBy(f => f.Fee_Type_Id == viewModel.feeType.Id).Fee_Type_Name;

                                viewModel.Department = new Department
                                {
                                    Id = Convert.ToInt32(dsFeeDetailList.Tables[0].Rows[i][6].ToString().Trim())
                                };
                                if (viewModel.Department.Id <= 0)
                                {
                                    viewModel.Department = null;
                                }
                                else
                                {
                                    viewModel.Department.Name = departmentLogic.GetEntityBy(d => d.Department_Id == viewModel.Department.Id).Department_Name;
                                }

                                viewModel.PaymentMode = new PaymentMode
                                {
                                    Id = Convert.ToInt32(dsFeeDetailList.Tables[0].Rows[i][5].ToString().Trim())
                                };
                                if (viewModel.PaymentMode.Id <= 0)
                                {
                                    viewModel.PaymentMode = null;
                                }
                                else
                                {
                                    viewModel.PaymentMode.Name = paymentModeLogic.GetEntityBy(p => p.Payment_Mode_Id == viewModel.PaymentMode.Id).Payment_Mode_Name;
                                }

                                viewModel.Programme = new Programme
                                {
                                    Id = Convert.ToInt32(dsFeeDetailList.Tables[0].Rows[i][3].ToString().Trim())
                                };
                                if (viewModel.Programme.Id <= 0)
                                {
                                    viewModel.Programme = null;
                                }
                                else
                                {
                                    viewModel.Programme.Name = programmeLogic.GetEntityBy(p => p.Programme_Id == viewModel.Programme.Id).Programme_Short_Name;
                                }

                                viewModel.level = new Level
                                {
                                    Id = Convert.ToInt32(dsFeeDetailList.Tables[0].Rows[i][4].ToString().Trim())
                                };
                                if (viewModel.level.Id <= 0)
                                {
                                    viewModel.level = null;
                                }
                                else
                                {
                                    viewModel.level.Name = levelLogic.GetEntityBy(l => l.Level_Id == viewModel.level.Id).Level_Name;
                                }

                                viewModel.CurrentSession.Name = sessionLogic.GetEntityBy(s => s.Session_Id == viewModel.CurrentSession.Id).Session_Name;

                                if (viewModel.fee.Id > 0 && viewModel.feeType.Id > 0 && viewModel.CurrentSession.Id > 0 && viewModel.Programme != null && viewModel.Programme.Id > 0 && viewModel.Department != null && viewModel.Department.Id > 0 && viewModel.level != null && viewModel.level.Id > 0 && viewModel.PaymentMode != null && viewModel.PaymentMode.Id > 0)
                                {
                                    FEE fee = feeLogic.GetEntitiesBy(f => f.Fee_Id == viewModel.fee.Id).LastOrDefault();
                                    FEE_TYPE feeType = feeTypeLogic.GetEntitiesBy(ft => ft.Fee_Type_Id == viewModel.feeType.Id).LastOrDefault();
                                    SESSION session = sessionLogic.GetEntityBy(s => s.Session_Id == viewModel.CurrentSession.Id);
                                    PROGRAMME programme = programmeLogic.GetEntityBy(p => p.Programme_Id == viewModel.Programme.Id);
                                    DEPARTMENT department = departmentLogic.GetEntityBy(p => p.Department_Id == viewModel.Department.Id);
                                    LEVEL level = levelLogic.GetEntityBy(l => l.Level_Id == viewModel.level.Id);
                                    PAYMENT_MODE paymentMode = paymentModeLogic.GetEntityBy(p => p.Payment_Mode_Id == viewModel.PaymentMode.Id);
                                    if (fee != null && feeType != null && session != null && paymentMode != null && programme != null && department != null && level != null)
                                    {
                                        var feeDetailCheck = feeDetailLogic.GetEntityBy(
                                                                                   fd =>
                                                                                       fd.Fee_Id == fee.Fee_Id &&
                                                                                       fd.Fee_Type_Id == feeType.Fee_Type_Id &&
                                                                                       fd.Programme_Id == programme.Programme_Id &&
                                                                                       fd.Department_Id == department.Department_Id &&
                                                                                       fd.Level_Id == level.Level_Id &&
                                                                                       fd.Payment_Mode_Id == paymentMode.Payment_Mode_Id &&
                                                                                       fd.Session_Id == viewModel.CurrentSession.Id);

                                        if (feeDetailCheck == null)
                                        {

                                            var listAdd = new FeeDetail
                                            {
                                                Fee = viewModel.fee,
                                                FeeType = viewModel.feeType,
                                                Department = viewModel.Department,
                                                Programme = viewModel.Programme,
                                                Level = viewModel.level,
                                                Session = viewModel.CurrentSession,
                                                SN = viewModel.SN,
                                                PaymentMode = viewModel.PaymentMode
                                            };
                                            feeDetails.Add(listAdd);
                                        }
                                        else
                                        {
                                            count += 1;
                                            //countSN = new[] {model.SN};
                                        }
                                    }
                                    else
                                    {
                                        SetMessage("Error Occured FeeTyype or Fee does not exist", Message.Category.Error);
                                        break;
                                    }

                                }
                            }
                            if (count > 0)
                            {

                                SetMessage(count + " " + "items could not be added because they alreay exist", Message.Category.Error);
                            }

                            viewModel.feeDetails = feeDetails;
                            TempData["viewModel"] = viewModel;

                        }
                    }
                }
                KeepDropDownState(viewModel);
            }
            catch (Exception)
            {

                SetMessage("Error Occured", Message.Category.Error);
            }
            return View(viewModel);
        }

        public void KeepDropDownState(FeeDetailViewModel keepFeeDetail)
        {
            try
            {

                keepFeeDetail.SessionSelectListItem = Utility.PopulateSessionSelectListItem();

                if (keepFeeDetail.CurrentSession != null && keepFeeDetail.CurrentSession.Id > 0)
                {
                    ViewBag.SessionId = new SelectList(keepFeeDetail.SessionSelectListItem, VALUE, TEXT, keepFeeDetail.CurrentSession.Id);
                }
                else
                {
                    ViewBag.SessionId = new SelectList(keepFeeDetail.SessionSelectListItem, VALUE, TEXT);
                }
            }
            catch (Exception)
            {

                SetMessage("Error Occured", Message.Category.Error);
            }
        }
        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {

                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static DataSet ReadExcelFile(string filepath)
        {
            DataSet result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + "Extended Properties=Excel 8.0;";
                OleDbConnection connection = new OleDbConnection(xConnStr);
                OleDbCommand command = new OleDbCommand("Select * FROM [Sheet1$]", connection);
                connection.Open();
                // Create DbDataReader to Data Worksheet

                OleDbDataAdapter MyData = new OleDbDataAdapter();
                MyData.SelectCommand = command;
                DataSet ds = new DataSet();
                ds.Clear();
                MyData.Fill(ds);
                connection.Close();

                result = ds;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        [HttpPost]
        public ActionResult SaveFeeDetail(FeeDetailViewModel viewModel)
        {
            try
            {
                int UploadCount = 0;
                string operation = "INSERT";
                string action = "UPLOADING FEE DETAIL";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                UserLogic userLogic = new UserLogic();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                FeeDetailAuditLogic feeDetailAuditLogic = new FeeDetailAuditLogic();

                viewModel = (FeeDetailViewModel)TempData["viewModel"];
                if (viewModel.feeDetails != null && viewModel.feeDetails.Count > 0)
                {
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        FeeDetailAudit feeDetailAudit = new FeeDetailAudit
                        {
                            Operatiion = operation,
                            Action = action,
                            Client = client,
                            DateUploaded = DateTime.Now,
                            User = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name)
                        };


                        if (viewModel.feeDetails != null)
                        {
                            for (int i = 0; i < viewModel.feeDetails.Count; i++)
                            {
                                FeeDetail feeDetail = new FeeDetail
                                {
                                    Fee = viewModel.feeDetails[i].Fee,
                                    FeeType = viewModel.feeDetails[i].FeeType,
                                    Department = viewModel.feeDetails[i].Department,
                                    Programme = viewModel.feeDetails[i].Programme,
                                    Level = viewModel.feeDetails[i].Level,
                                    Session = viewModel.feeDetails[i].Session,
                                    PaymentMode = viewModel.feeDetails[i].PaymentMode
                                };
                                FeeDetail createdFeeDetail = feeDetailLogic.Create(feeDetail);
                                if (createdFeeDetail != null)
                                {
                                    feeDetailAudit.FeeType = viewModel.feeDetails[i].FeeType;
                                    feeDetailAudit.Fee = viewModel.feeDetails[i].Fee;
                                    feeDetailAudit.Department = viewModel.feeDetails[i].Department;
                                    feeDetailAudit.Programme = viewModel.feeDetails[i].Programme;
                                    feeDetailAudit.Level = viewModel.feeDetails[i].Level;
                                    feeDetailAudit.Session = viewModel.feeDetails[i].Session;
                                    feeDetailAudit.FeeDetail = createdFeeDetail;

                                    feeDetailAuditLogic.Create(feeDetailAudit);
                                }

                                UploadCount++;
                            }
                        }

                        transactionScope.Complete();
                    }
                    SetMessage(UploadCount + " " + "Out of" + " " + viewModel.feeDetails.Count + " " + "Uploaded Successfully", Message.Category.Information);
                    return RedirectToAction("UploadFeeDetail");
                }
            }
            catch (Exception)
            {

                SetMessage("Error Occured", Message.Category.Error);
            }
            return HttpNotFound("No resourse found");
        }

        public ActionResult UploadPayment()
        {
            FeeDetailViewModel viewModel = new FeeDetailViewModel();
            try
            {
                ViewBag.SessionId = viewModel.SessionSelectListItem;
                ViewBag.FeeTypeId = viewModel.SchoolFeeAcceptanceFeeTypeSelectListItem;

            }
            catch (Exception ex)
            {
                SetMessage("Error Occured" + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult UploadPayment(FeeDetailViewModel viewModel)
        {
            try
            {
                var paymentList = new List<PaymentUploadModel>();

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);

                    string filePath = string.Empty;
                    string fileExt = string.Empty;
                    filePath = savedFileName; //get the path of the file  
                    fileExt = Path.GetExtension(filePath); //get the file extension  

                    //DataSet studentList = ReadExcel(savedFileName);
                    DataSet payments = ReadExcel(savedFileName, fileExt);

                    if (payments != null && payments.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < payments.Tables[0].Rows.Count; i++)
                        {
                            var paymentModel = new PaymentUploadModel();

                            paymentModel.PaymentDate = payments.Tables[0].Rows[i][1].ToString();
                            paymentModel.PaymentRefNumber = payments.Tables[0].Rows[i][2].ToString();
                            paymentModel.MerchantReference = payments.Tables[0].Rows[i][3].ToString();
                            paymentModel.MerchantId = payments.Tables[0].Rows[i][4].ToString();
                            paymentModel.CustomerId = payments.Tables[0].Rows[i][5].ToString();
                            paymentModel.CustomerName = payments.Tables[0].Rows[i][6].ToString();
                            paymentModel.PaymentItem = payments.Tables[0].Rows[i][7].ToString();
                            paymentModel.Amount = payments.Tables[0].Rows[i][8].ToString();
                            paymentModel.PaymentMethod = payments.Tables[0].Rows[i][9].ToString();
                            paymentModel.Bank = payments.Tables[0].Rows[i][10].ToString();

                            paymentList.Add(paymentModel);
                        }
                    }
                }

                viewModel.Payments = paymentList;

                ViewBag.SessionId = new SelectList(viewModel.SessionSelectListItem, "Value", "Text", viewModel.CurrentSession.Id);
                ViewBag.FeeTypeId = new SelectList(viewModel.SchoolFeeAcceptanceFeeTypeSelectListItem, "Value", "Text", viewModel.feeType.Id);
                TempData["viewModel"] = viewModel;
                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult SavePayment(FeeDetailViewModel viewModel)
        {
            try
            {
                int UploadCount = 0;
                List<PaymentUploadModel> successFullPayments = new List<PaymentUploadModel>();
                //string operation = "INSERT";
                //string action = "UPLOADING FEE DETAIL";
                //string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                //UserLogic userLogic = new UserLogic();

                viewModel = (FeeDetailViewModel)TempData["viewModel"];
                if (viewModel.Payments != null && viewModel.Payments.Count > 0)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                    BankLogic bankLogic = new BankLogic();
                    StudentLogic studentLogic = new StudentLogic();

                    Payment payment = new Payment();
                    StudentPayment studentPayment = new StudentPayment();
                    PaymentEtranzact paymentEtranzact = new PaymentEtranzact();

                    for (int i = 0; i < viewModel.Payments.Count; i++)
                    {
                        PaymentUploadModel paymentUpload = viewModel.Payments[i];
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            Model.Model.Student student = studentLogic.GetModelsBy(s => s.Matric_Number == paymentUpload.CustomerId || s.APPLICATION_FORM.Application_Form_Number == paymentUpload.CustomerId).LastOrDefault();
                            if (student == null)
                            {
                                continue;
                            }

                            payment.PaymentMode = new PaymentMode(){ Id = (int)PaymentModes.Full};
                            payment.PaymentType = new PaymentType(){ Id = (int)Paymenttypes.OnlinePayment};
                            payment.PersonType = new PersonType(){ Id = (int)PersonTypes.Student};
                            payment.FeeType = viewModel.feeType;
                            payment.Person = new Person(){ Id = student.Id, Type = new PersonType() { Id = (int)PersonTypes.Student}};
                            payment.Session = viewModel.CurrentSession;

                            Payment newPayment = CreatePayment(payment);

                            if (newPayment != null && newPayment.Id > 0)
                            {
                                Level level = null;
                                StudentLevel studentLevel = studentLevelLogic.GetModelBy(s => s.Person_Id == student.Id);
                                if (studentLevel != null)
                                {
                                    level = studentLevel.Level;
                                }
                                else
                                {
                                    continue;
                                }

                                string[] amountSplit = paymentUpload.Amount.Split(',');
                                string amtSecondHalf = amountSplit[1].Split('.').FirstOrDefault();
                                string amtFirstHalf = amountSplit[0].Split(' ').LastOrDefault();
                                string amount = amtFirstHalf + amtSecondHalf;
                                decimal amountDecimal = Convert.ToDecimal(amount);

                                studentPayment.Amount = amountDecimal;
                                studentPayment.Level = level;
                                studentPayment.Status = true;
                                studentPayment.Session = viewModel.CurrentSession;
                                studentPayment.Id = newPayment.Id;
                                studentPayment.Person = new Person(){ Id = student.Id };

                                if (studentPaymentLogic.GetModelBy(p => p.Payment_Id == newPayment.Id) == null)
                                {
                                    studentPaymentLogic.Create(studentPayment);
                                }

                                PaymentEtranzactType paymentEtranzactType = paymentEtranzactTypeLogic.GetModelBy( p => p.Programme_Id == studentLevel.Programme.Id && 
                                                        p.Fee_Type_Id == viewModel.feeType.Id && p.Level_Id == level.Id && p.Session_Id == viewModel.CurrentSession.Id);
                                PaymentTerminal paymentTerminal = paymentTerminalLogic.GetModelsBy(p => p.Fee_Type_Id == viewModel.feeType.Id && p.Session_Id == viewModel.CurrentSession.Id).LastOrDefault();
                                if (paymentEtranzactType == null || paymentTerminal == null)
                                {
                                    continue;
                                }

                                string bankStr = paymentUpload.Bank.Split(' ').FirstOrDefault();
                                string bankCode = null;
                                Bank bank = bankLogic.GetModelsBy(b => b.Bank_Name.Contains(bankStr)).LastOrDefault();
                                if (bank != null)
                                {
                                    bankCode = bank.Code;
                                }
                                else
                                {
                                    bankCode = "011";
                                }

                                paymentEtranzact.BankCode = bankCode;
                                paymentEtranzact.BranchCode = "001";
                                paymentEtranzact.ConfirmationNo = paymentUpload.MerchantReference;
                                paymentEtranzact.CustomerAddress = "The Federal Polytechnic Ado-Ekiti";
                                paymentEtranzact.CustomerID = newPayment.InvoiceNumber;
                                paymentEtranzact.CustomerName = paymentUpload.CustomerName;
                                paymentEtranzact.EtranzactType = paymentEtranzactType;
                                paymentEtranzact.MerchantCode = paymentUpload.PaymentRefNumber;
                                paymentEtranzact.Payment = new OnlinePayment(){ Payment = newPayment };
                                paymentEtranzact.PaymentCode = paymentUpload.MerchantReference;
                                paymentEtranzact.ReceiptNo = paymentUpload.PaymentRefNumber;
                                paymentEtranzact.Terminal = paymentTerminal;
                                paymentEtranzact.TransactionAmount = amountDecimal;
                                paymentEtranzact.TransactionDate = new DateTime(Convert.ToInt32(paymentUpload.PaymentDate.Split('/')[2].Split(' ').FirstOrDefault()), Convert.ToInt32(paymentUpload.PaymentDate.Split('/')[0]), Convert.ToInt32(paymentUpload.PaymentDate.Split('/')[1]));
                                paymentEtranzact.TransactionDescription = paymentUpload.PaymentItem;
                                paymentEtranzact.Used = true;
                                paymentEtranzact.UsedBy = student.Id;

                                if (paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == newPayment.Id) == null)
                                {
                                    paymentEtranzactLogic.Create(paymentEtranzact);
                                }
                                
                            }

                            successFullPayments.Add(paymentUpload);
                            UploadCount++;
                            transactionScope.Complete();
                        }
                    }
                }

                ViewBag.SessionId = new SelectList(viewModel.SessionSelectListItem, "Value", "Text", viewModel.CurrentSession.Id);
                ViewBag.FeeTypeId = new SelectList(viewModel.SchoolFeeAcceptanceFeeTypeSelectListItem, "Value", "Text", viewModel.feeType.Id);
                viewModel.SuccessfulPayments = successFullPayments;
                SetMessage(UploadCount + " " + "Out of" + " " + viewModel.Payments.Count + " " + "Uploaded Successfully", Message.Category.Information);

                return View("UploadPayment", viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured." + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("UploadPayment");
        }
        private Payment CreatePayment(Payment paymetModel)
        {
            Payment newPayment = new Payment();
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();

                Payment payment = new Payment();
                payment.PaymentMode = paymetModel.PaymentMode;
                payment.PaymentType = paymetModel.PaymentType;
                payment.PersonType = paymetModel.Person.Type;
                payment.FeeType = paymetModel.FeeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = paymetModel.Person;
                payment.Session = paymetModel.Session;

                OnlinePayment newOnlinePayment = null;
                Payment existingPayment = paymentLogic.GetModelsBy(p => p.Person_Id == paymetModel.Person.Id && p.Fee_Type_Id == paymetModel.FeeType.Id && p.Session_Id == paymetModel.Session.Id && p.Payment_Mode_Id == paymetModel.PaymentMode.Id).LastOrDefault();

                if (existingPayment != null)
                {
                    return existingPayment;
                }

                newPayment = paymentLogic.Create(payment);

                if (newPayment != null && newPayment.Id > 0)
                {
                    PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                    OnlinePayment onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataSet ReadExcel(string fileName, string fileExt)
        {
            try
            {
                string conn = string.Empty;
                DataSet dtexcel = new DataSet();
                if (fileExt.CompareTo(".xls") == 0)
                    conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';"; //for below excel 2007  
                else
                    conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=Yes';"; //for above excel 2007  
                using (OleDbConnection con = new OleDbConnection(conn))
                {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Sheet1$]", con); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable 
                }
                return dtexcel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult AutomatedFeeDetailSetup()
        {

            try
            {
                viewmodel = new FeeDetailViewModel();
                ViewBag.Sessions = viewmodel.SessionSelectListItem.OrderBy(x => x.Text);
                ViewBag.FeeTypes = viewmodel.FeeTypeSelectListItem.OrderBy(x => x.Text);
                ViewBag.Paymentmodes = viewmodel.PaymentModeSelectListItem.OrderBy(x => x.Text);
                ViewBag.Levels = viewmodel.LevelSelectListItem.OrderBy(x => x.Text);
                ViewBag.Programmes = viewmodel.ProgrammeSelectListItem.OrderBy(x => x.Text);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occured" + ex.Message, Message.Category.Error);
            }
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult AutomatedFeeDetailSetup(FeeDetailViewModel viewModel, string feeSetupid)
        {
            try
            {
                var feeSetUpLogic = new FeeSetUpLogic();
                if (feeSetupid != null)
                {
                    int id = Convert.ToInt32(feeSetupid);
                    viewModel.FeeSetup = feeSetUpLogic.GetModelsBy(x => x.FeeSetup_Id == id).FirstOrDefault();
                    RetainDropdownThree(viewModel);
                    return View(viewModel);
                }
                viewModel.FeeSetups = feeSetUpLogic.GetFeeSetup(viewModel.feeDetail.Session, viewModel.feeDetail.FeeType);
                RetainDropdownThree(viewModel);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occured" + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }
        //[HttpPost]
        //public ActionResult ApplyFeeSetUp(string feeSetupid)
        //{
        //    FeeDetailViewModel viewModel = new FeeDetailViewModel();
        //    try
        //    {
        //        if (String.IsNullOrEmpty(feeSetupid))
        //        {

        //        }
        //        int id = Convert.ToInt32(feeSetupid);
        //        var feeSetUpLogic = new FeeSetUpLogic();
        //        viewModel.FeeSetup=feeSetUpLogic.GetModelsBy(x => x.FeeSetup_Id == id).FirstOrDefault();

        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(view)
        //}
        public JsonResult AddFeeSetup(string feeSetUpArrayRecord, string feeArrayRecord)
        {
            JsonResultModel result = new JsonResultModel();
            List<decimal> setupAmount = new List<decimal>();
            string action = "CREATE FEE SETUP";
            string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
            try
            {
                var logonUsername = User.Identity.Name;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                FeeSetupJsonView feeSetupArrayJsonView = serializer.Deserialize<FeeSetupJsonView>(feeSetUpArrayRecord);
                List<string> feeArrayJsonView = serializer.Deserialize<List<string>>(feeArrayRecord);

                if (feeSetupArrayJsonView != null && feeArrayRecord != null)
                {
                    FeeSetup feeSetup = new Model.Model.FeeSetup();
                    FeeSetupFee feeSetupFee = new FeeSetupFee();
                    FeeSetUpLogic feeSetUpLogic = new FeeSetUpLogic();
                    FeeSetupFeeLogic feeSetupFeeLogic = new FeeSetupFeeLogic();
                    UserLogic userLogic = new UserLogic();
                    FeeLogic feeLogic = new FeeLogic();
                    FeeSetupAudit feeSetupAudit = new FeeSetupAudit();
                    FeeSetupAuditLogic feeSetupAuditLogic = new FeeSetupAuditLogic();
                    //Get Login User Information
                    var User = userLogic.GetModelBy(u => u.User_Name == logonUsername);
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        if (feeSetupArrayJsonView.Activated == "on")
                        {
                            feeSetup.Activated = true;

                        }
                        else
                        {
                            feeSetup.Activated = false;
                        }
                        foreach (var feeId in feeArrayJsonView)
                        {
                            Fee fee = new Fee();
                            int id = Convert.ToInt32(feeId);
                            fee = feeLogic.GetModelBy(x => x.Fee_Id == id);
                            setupAmount.Add(fee.Amount);
                        }
                        var Amount = setupAmount.Sum();
                        feeSetup.Amount = Amount;
                        feeSetup.FeeSetUpName = feeSetupArrayJsonView.FeeSetupName;
                        feeSetup.FeeType = new FeeType { Id = Convert.ToInt32(feeSetupArrayJsonView.FeeTypeId) };
                        feeSetup.PaymentMode = new PaymentMode { Id = Convert.ToInt32(feeSetupArrayJsonView.PaymentModeId) };
                        feeSetup.Session = new Model.Model.Session { Id = Convert.ToInt32(feeSetupArrayJsonView.SessionId) };
                        feeSetup.User = new User { Id = User.Id };
                        var createdFeeSetup = feeSetUpLogic.Create(feeSetup);
                        if (createdFeeSetup != null)
                        {
                            foreach (var feeId in feeArrayJsonView)
                            {
                                int id = Convert.ToInt32(feeId);
                                feeSetupFee = new FeeSetupFee();
                                feeSetupFee.Activated = true;
                                feeSetupFee.Fee = new Fee { Id = id };
                                feeSetupFee.FeeSetUp = createdFeeSetup;
                                feeSetupFeeLogic.Create(feeSetupFee);
                            }
                        }
                        if (createdFeeSetup != null)
                        {
                            feeSetupAudit = new FeeSetupAudit();
                            feeSetupAudit.Action = action;
                            feeSetupAudit.Client = client;
                            feeSetupAudit.FeeType = new FeeType { Id = Convert.ToInt32(feeSetupArrayJsonView.FeeTypeId) };
                            feeSetupAudit.PaymentMode = new PaymentMode { Id = Convert.ToInt32(feeSetupArrayJsonView.PaymentModeId) };
                            feeSetupAudit.Session = new Model.Model.Session { Id = Convert.ToInt32(feeSetupArrayJsonView.SessionId) };
                            feeSetupAudit.FeeSetup = createdFeeSetup;
                            feeSetupAudit.User = new User { Id = User.Id };
                            feeSetupAudit.Amount = Amount;
                            feeSetupAuditLogic.Create(feeSetupAudit);

                        }
                        transactionScope.Complete();
                        result.IsError = false;
                        result.Message = "Fee SetUp Added Successfully";
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
        public JsonResult FetchFees()
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                FeeLogic feeLogic = new FeeLogic();
                var fees = feeLogic.GetAll().OrderBy(x => x.Name).ToList();
                for (int i = 0; i < fees.Count(); i++)
                {
                    fees[i].Name = fees[i].Name.ToUpper() + " " + "--" + " " + fees[i].Amount;
                }
                fees.OrderBy(x => x.Name);
                return Json(new SelectList(fees, FEEID, FEENAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApplyFeeSetUp(int feeSetUpId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                FeeSetup feeSetup = new Model.Model.FeeSetup();
                FeeSetUpLogic feeSetUpLogic = new FeeSetUpLogic();
                if (feeSetUpId > 0)
                {
                    result.FeeSetup = feeSetUpLogic.GetModelBy(x => x.FeeSetup_Id == feeSetUpId);
                    result.IsError = false;
                }


            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartments()
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetModelsBy(x => x.Active == true).OrderBy(x => x.Name).ToList();

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveToFeeDetail(int setupId, string programmeId, string levelId, string deptArrayRecord)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<string> deptArrayJsonView = serializer.Deserialize<List<string>>(deptArrayRecord);
                FeeSetUpLogic feeSetUpLogic = new FeeSetUpLogic();
                FeeSetup feeSetup = new Model.Model.FeeSetup();

                if (setupId > 0 && deptArrayJsonView != null)
                {
                    feeSetup = feeSetUpLogic.GetModelBy(x => x.FeeSetup_Id == setupId);
                    int lvlId = Convert.ToInt32(levelId);
                    int proId = Convert.ToInt32(programmeId);

                    bool isCreated = CreateFeeDetail(feeSetup, lvlId, proId, deptArrayJsonView);
                    if (isCreated)
                    {
                        result.IsError = false;
                        result.Message = "You have finally created Fee-Detail";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Fee-Detail Creation was not successfull";
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
        public bool CreateFeeDetail(FeeSetup setup, int levelId, int programmeId, List<string> deptIds)
        {
            try
            {
                DateTime date = DateTime.Now;
                var logonUsername = User.Identity.Name;
                string operation = "INSERT";
                string action = "UPLOADING FEE DETAIL";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                FeeDetail feeDetail = new FeeDetail();
                List<Fee> feeList = new List<Fee>();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                FeeDetailAudit feeDetailAudit = new FeeDetailAudit();
                FeeSetUpLogic feeSetUpLogic = new FeeSetUpLogic();
                FeeSetup feeSetup = new Model.Model.FeeSetup();
                FeeSetupFeeLogic feeSetupFeeLogic = new FeeSetupFeeLogic();
                FeeDetailAuditLogic feeDetailAuditLogic = new FeeDetailAuditLogic();
                UserLogic userLogic = new UserLogic();
                var setUpFees = feeSetupFeeLogic.GetModelsBy(x => x.FeeSetUp_Id == setup.FeeSetupId);
                var user = userLogic.GetModelBy(u => u.User_Name == logonUsername);
                foreach (var deptId in deptIds)
                {
                    //create for individual feeId
                    foreach (var setUpFee in setUpFees)
                    {
                        int id = Convert.ToInt32(deptId);
                        feeDetail = new FeeDetail();
                        feeDetail.Department = new Department { Id = id };
                        feeDetail.Level = new Level { Id = levelId };
                        feeDetail.Programme = new Programme { Id = programmeId };
                        feeDetail.Session = setup.Session;
                        feeDetail.FeeType = setup.FeeType;
                        feeDetail.PaymentMode = setup.PaymentMode;

                        feeDetail.Fee = setUpFee.Fee;

                        var createdFeeDetail = feeDetailLogic.Create(feeDetail);
                        if (createdFeeDetail != null)
                        {
                            feeDetailAudit = new FeeDetailAudit();
                            feeDetailAudit.Action = action;
                            feeDetailAudit.Operatiion = operation;
                            feeDetailAudit.Client = client;
                            feeDetailAudit.FeeType = setup.FeeType;
                            feeDetailAudit.Fee = setUpFee.Fee;
                            feeDetailAudit.Department = new Department { Id = id };
                            feeDetailAudit.Programme = new Programme { Id = programmeId };
                            feeDetailAudit.Level = new Level { Id = levelId };
                            feeDetailAudit.Session = setup.Session;
                            feeDetailAudit.User = new User { Id = user.Id };
                            feeDetailAudit.DateUploaded = date;
                            feeDetailAudit.FeeDetail = createdFeeDetail;
                            feeDetailAuditLogic.Create(feeDetailAudit);

                        }
                    }


                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        public JsonResult GetFeeIdBy(int setUpId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                FeeLogic feeLogic = new FeeLogic();
                FeeSetupFeeLogic feeSetupFeeLogic = new FeeSetupFeeLogic();
                var fees = feeSetupFeeLogic.GetModelsBy(x => x.FeeSetUp_Id == setUpId && x.Activated == true).Select(x => x.Fee).OrderBy(x => x.Name).ToList();
                for (int i = 0; i < fees.Count(); i++)
                {
                    fees[i].Name = fees[i].Name.ToUpper() + " " + "--" + " " + fees[i].Amount;
                }
                fees.OrderBy(x => x.Name);
                return Json(new SelectList(fees, FEEID, FEENAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditFeeSetup(int setupId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                viewmodel = new FeeDetailViewModel();
                ViewBag.Sessions = viewmodel.SessionSelectListItem.OrderBy(x => x.Text);
                ViewBag.FeeTypes = viewmodel.FeeTypeSelectListItem.OrderBy(x => x.Text);
                ViewBag.Paymentmodes = viewmodel.PaymentModeSelectListItem.OrderBy(x => x.Text);
                FeeSetup feeSetup = new Model.Model.FeeSetup();
                FeeSetUpLogic feeSetUpLogic = new FeeSetUpLogic();
                result.FeeSetup = feeSetUpLogic.GetModelBy(x => x.FeeSetup_Id == setupId);
                if (result.FeeSetup != null)
                {
                    result.IsError = false;
                    ViewBag.Edit = "notNull";
                }
                else
                {
                    result.Message = "No Record Found";
                }

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UnRegisteredFees(int setUpId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                FeeLogic feeLogic = new FeeLogic();
                FeeSetupFeeLogic feeSetupFeeLogic = new FeeSetupFeeLogic();
                var registeredFees = feeSetupFeeLogic.GetModelsBy(x => x.FeeSetUp_Id == setUpId && x.Activated == true).Select(x => x.Fee).OrderBy(x => x.Name).ToList();
                var allFees = feeLogic.GetAll();
                var unregisteredFees = allFees.Where(x => !registeredFees.Any(y => y.Id == x.Id)).ToList();
                for (int i = 0; i < unregisteredFees.Count(); i++)
                {
                    unregisteredFees[i].Name = unregisteredFees[i].Name.ToUpper() + " " + "--" + " " + unregisteredFees[i].Amount;
                }
                unregisteredFees.OrderBy(x => x.Name);
                return Json(new SelectList(unregisteredFees, FEEID, FEENAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveEditFeeSetup(string feeSetUpArrayRecord, string feeArrayRecord)
        {
            JsonResultModel result = new JsonResultModel();
            List<decimal> setupAmount = new List<decimal>();
            try
            {
                var logonUsername = User.Identity.Name;
                string action = "MODIFY FEE SETUP";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                FeeSetupJsonView feeSetupArrayJsonView = serializer.Deserialize<FeeSetupJsonView>(feeSetUpArrayRecord);
                List<string> feeArrayJsonView = serializer.Deserialize<List<string>>(feeArrayRecord);

                if (feeSetupArrayJsonView != null && feeArrayRecord != null)
                {
                    FeeSetup feeSetup = new Model.Model.FeeSetup();
                    FeeSetupFee feeSetupFee = new FeeSetupFee();
                    FeeSetUpLogic feeSetUpLogic = new FeeSetUpLogic();
                    FeeSetupFeeLogic feeSetupFeeLogic = new FeeSetupFeeLogic();
                    UserLogic userLogic = new UserLogic();
                    FeeLogic feeLogic = new FeeLogic();
                    FeeSetupAudit feeSetupAudit = new FeeSetupAudit();
                    FeeSetupAuditLogic feeSetupAuditLogic = new FeeSetupAuditLogic();
                    //Get Login User Information
                    var User = userLogic.GetModelBy(u => u.User_Name == logonUsername);
                    int setUpId = Convert.ToInt32(feeSetupArrayJsonView.FeeSetUpId);
                    feeSetup = feeSetUpLogic.GetModelBy(x => x.FeeSetup_Id == setUpId);
                    if (feeSetup != null)
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            if (feeSetupArrayJsonView.Activated == "on")
                            {
                                feeSetup.Activated = true;

                            }
                            else
                            {
                                feeSetup.Activated = false;
                            }
                            foreach (var feeId in feeArrayJsonView)
                            {
                                Fee fee = new Fee();
                                int id = Convert.ToInt32(feeId);
                                fee = feeLogic.GetModelBy(x => x.Fee_Id == id);
                                setupAmount.Add(fee.Amount);
                            }
                            var Amount = setupAmount.Sum();
                            feeSetup.Amount = Amount;
                            feeSetup.FeeSetUpName = feeSetupArrayJsonView.FeeSetupName;
                            feeSetup.FeeType = new FeeType { Id = Convert.ToInt32(feeSetupArrayJsonView.FeeTypeId) };
                            feeSetup.PaymentMode = new PaymentMode { Id = Convert.ToInt32(feeSetupArrayJsonView.PaymentModeId) };
                            feeSetup.Session = new Model.Model.Session { Id = Convert.ToInt32(feeSetupArrayJsonView.SessionId) };
                            feeSetup.User = new User { Id = User.Id };
                            var modifiedFeeSetup = feeSetUpLogic.Modify(feeSetup);
                            if (modifiedFeeSetup)
                            {
                                feeSetupFeeLogic.Delete(x => x.FeeSetUp_Id == setUpId);
                                foreach (var feeId in feeArrayJsonView)
                                {
                                    int id = Convert.ToInt32(feeId);
                                    feeSetupFee = new FeeSetupFee();
                                    feeSetupFee.Activated = true;
                                    feeSetupFee.Fee = new Fee { Id = id };
                                    feeSetupFee.FeeSetUp = feeSetup;
                                    feeSetupFeeLogic.Create(feeSetupFee);
                                }
                                feeSetupAudit = new FeeSetupAudit();
                                feeSetupAudit.Action = action;
                                feeSetupAudit.Client = client;
                                feeSetupAudit.FeeType = new FeeType { Id = Convert.ToInt32(feeSetupArrayJsonView.FeeTypeId) };
                                feeSetupAudit.PaymentMode = new PaymentMode { Id = Convert.ToInt32(feeSetupArrayJsonView.PaymentModeId) };
                                feeSetupAudit.Session = new Model.Model.Session { Id = Convert.ToInt32(feeSetupArrayJsonView.SessionId) };
                                feeSetupAudit.FeeSetup = feeSetup;
                                feeSetupAudit.User = new User { Id = User.Id };
                                feeSetupAudit.Amount = Amount;
                                feeSetupAuditLogic.Create(feeSetupAudit);

                            }
                            transactionScope.Complete();
                            result.IsError = false;
                            result.Message = "Fee SetUp Modified Successfully";
                        }
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

        public ActionResult DepartmentalFeeBreakdown()
        {
            FeeDetailViewModel viewModel = new FeeDetailViewModel();
            try
            {
                ViewBag.Departments = new SelectList(new List<Department>(), "Id", "Name");
                ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
                ViewBag.FeeTypes = viewModel.FeeTypeSelectListItem;
                ViewBag.Sessions = viewModel.SessionSelectListItem;
                ViewBag.Levels = viewModel.LevelSelectListItem;
                ViewBag.Modes = viewModel.PaymentModeSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Something went wrong! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public JsonResult GetDepartmentFeeBreakdown(int[] data)
        {
            Abundance_Nk.Model.Entity.Model.JsonResultModel result = new Abundance_Nk.Model.Entity.Model.JsonResultModel();
            try
            {
                int sessionId = data[0];
                int feeTypeId = data[1];
                int progId = data[2];
                int deptId = data[3];
                int levelId = data[4];
                int modeId = data[5];

                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(f => f.Department_Id == deptId && f.Fee_Type_Id == feeTypeId && f.Level_Id == levelId && f.Payment_Mode_Id == modeId &&
                                                                                f.Programme_Id == progId && f.Session_Id == sessionId);
                if (feeDetails != null && feeDetails.Count > 0)
                {
                    result.Fees = new List<Fee>();
                    for (int i = 0; i < feeDetails.Count; i++)
                    {
                        Fee fee = new Fee();
                        fee.Amount = feeDetails[i].Fee.Amount;
                        fee.Name = feeDetails[i].Fee.Name;

                        result.Fees.Add(fee);
                    }

                    result.IsError = false;
                }
                else
                {
                    result.IsError = true;
                    result.Message = "No fees found.";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Something went wrong." + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public bool CreateFeeSetup(int paymentModeId, int feetypeId, int sessionId,bool activated,decimal Amount,string Name, string feeString)
        //{

        //    try
        //    {
        //        FeeSetup feeSetup = new Model.Model.FeeSetup();
        //        FeeSetupFee feeSetupFee = new FeeSetupFee();
        //        FeeSetUpLogic feeSetUpLogic = new FeeSetUpLogic();
        //        FeeSetupFeeLogic feeSetupFeeLogic = new FeeSetupFeeLogic();



        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}