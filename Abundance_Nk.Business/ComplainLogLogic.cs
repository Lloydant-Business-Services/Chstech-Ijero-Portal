using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
   public class ComplainLogLogic : BusinessBaseLogic<ComplaintLog, COMPLAIN_LOG>
    {
        public ComplainLogLogic()
        {
            translator = new ComplianLogTranslator();
        }

        public override ComplaintLog Create(ComplaintLog model)
        {
            ComplaintLog newComplain = new ComplaintLog();
            model.TicketID = "ILR" ;
            model.DateSubmitted = DateTime.Now;
            newComplain =  base.Create(model);
            newComplain.TicketID = "ILR" + PaddNumber(newComplain.Id, 10);
            Modify(newComplain);
            return newComplain;
        }
       
       public static string PaddNumber(long id, int maxCount)
        {
            try
            {
                string idInString = id.ToString();
                string paddNumbers = "";
                if (idInString.Count() < maxCount)
                {
                    int zeroCount = maxCount - id.ToString().Count();
                    StringBuilder builder = new StringBuilder();
                    for (int counter = 0; counter < zeroCount; counter++)
                    {
                        builder.Append("0");
                    }

                    builder.Append(id);
                    paddNumbers = builder.ToString();
                    return paddNumbers;
                }

                return paddNumbers;
            }
            catch (Exception)
            {
                throw;
            }
        }
       public List<ComplaintLog> GetAllUnResolved()
       {
           try
           {
               Expression<Func<COMPLAIN_LOG, bool>> selector = a => a.Status == false;
               return GetModelsBy(selector);
           }
           catch (Exception)
           {
               throw;
           }
       }
       public List<ComplaintLog> GetAllResolved()
       {
           try
           {
               Expression<Func<COMPLAIN_LOG, bool>> selector = a => a.Status == true;
               return GetModelsBy(selector);
           }
           catch (Exception)
           {
               throw;
           }
       }
              
        public ComplaintLog GetBy(long Id)
        {
            try
            {
                Expression<Func<COMPLAIN_LOG, bool>> selector = a => a.Id == Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ComplaintLog GetBy(string TicketId)
        {
            try
            {
                Expression<Func<COMPLAIN_LOG, bool>> selector = a => a.Ticket_Id == TicketId;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private COMPLAIN_LOG GetEntityBy(ComplaintLog complaintLog)
        {
            try
            {
                Expression<Func<COMPLAIN_LOG, bool>> selector = s => s.Id == complaintLog.Id;
                COMPLAIN_LOG entity = GetEntityBy(selector);
                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(ComplaintLog complaintLog)
        {
            try
            {
                COMPLAIN_LOG entity = GetEntityBy(complaintLog);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }
                if (complaintLog.TicketID != null)
                {
                    entity.Ticket_Id = complaintLog.TicketID;
                }
                if (complaintLog.Status != null)
                {
                    entity.Status = complaintLog.Status;
                }
                if (complaintLog.Comment != null)
                {
                    entity.Comment = complaintLog.Comment;
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
