using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class FeeDetailLogic : BusinessBaseLogic<FeeDetail, FEE_DETAIL>
    {
        public FeeDetailLogic()
        {
            translator = new FeeDetailTranslator();
        }

        public Decimal GetFeeByDepartmentLevel(Department department, Level level, Programme programme, FeeType feetype, Session session, PaymentMode paymentMode)
        {
            Decimal Amount = 0;
            try
            {
                List<FeeDetail> feeDetails = GetModelsBy(a => a.Department_Id == department.Id && a.Level_Id == level.Id && a.Programme_Id == programme.Id && a.Fee_Type_Id == feetype.Id && a.Session_Id == session.Id && a.Payment_Mode_Id == paymentMode.Id);
                if (feeDetails != null && feeDetails.Count > 0)
                {
                    Amount = feeDetails.Sum(p => p.Fee.Amount);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Amount;
        }


        public List<FeeDetail> GetFee(Department department, Programme programme, FeeType feetype, Session session)
        {
            Decimal Amount = 0;
            try
            {
                List<FeeDetail> feeDetails =
                    GetModelsBy(
                        a =>
                            a.Department_Id == department.Id && a.Programme_Id == programme.Id &&
                            a.Fee_Type_Id == feetype.Id && a.Session_Id == session.Id);
                return feeDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Modify(List<FeeDetail> feeDetails)
        {
            try
            {
                foreach (FeeDetail feeDetail in feeDetails)
                {
                    int modified = 0;
                    Expression<Func<FEE_DETAIL, bool>> selector = c => c.Fee_Detail_Id == feeDetail.Id;
                    FEE_DETAIL feeDetailEntity = GetEntityBy(selector);
                    if (feeDetailEntity == null && feeDetail.Fee != null && feeDetail.Department != null)
                    {
                        var newFeeDetail = new FeeDetail();
                        newFeeDetail.Department = feeDetail.Department;
                        newFeeDetail.Fee = feeDetail.Fee;
                        newFeeDetail.FeeType = feeDetail.FeeType;
                        newFeeDetail.Level = feeDetail.Level;
                        newFeeDetail.PaymentMode = feeDetail.PaymentMode;
                        newFeeDetail.Programme = feeDetail.Programme;
                        newFeeDetail.Session = feeDetail.Session;
                        Create(newFeeDetail);
                    }
                    else
                    {
                        if (feeDetail.Id > 0 && feeDetail.Fee.Id <= 0 && feeDetail.PaymentMode.Id <= 0 &&
                            feeDetail.Level.Id <= 0)
                        {
                            Delete(selector);
                        }
                        else
                        {
                            feeDetailEntity.Fee_Id = feeDetail.Fee.Id;
                            feeDetailEntity.Level_Id = feeDetail.Level.Id;
                            feeDetailEntity.Payment_Mode_Id = feeDetail.PaymentMode.Id;
                            modified = Save();
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public bool Modify(FeeDetail model)
        {
            try
            {
                Expression<Func<FEE_DETAIL, bool>> selector = s => s.Fee_Detail_Id == model.Id;
                FEE_DETAIL entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.Session != null && model.Session.Id > 0)
                {
                    entity.Session_Id = model.Session.Id;
                }
                if (model.Programme != null && model.Programme.Id > 0)
                {
                    entity.Programme_Id = model.Programme.Id;
                }
                if (model.FeeType != null && model.FeeType.Id > 0)
                {
                    entity.Fee_Type_Id = model.FeeType.Id;
                }
                if (model.Level != null && model.Level.Id > 0)
                {
                    entity.Level_Id = model.Level.Id;
                }
                if (model.PaymentMode != null && model.PaymentMode.Id > 0)
                {
                    entity.Payment_Mode_Id = model.PaymentMode.Id;
                }
                if (model.Fee != null && model.Fee.Id > 0)
                {
                    entity.Fee_Id = model.Fee.Id;
                }
                if (model.Department != null && model.Department.Id > 0)
                {
                    entity.Department_Id = model.Department.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
    }
}
