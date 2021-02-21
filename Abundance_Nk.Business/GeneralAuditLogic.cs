using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Web;

namespace Abundance_Nk.Business
{
    public class GeneralAuditLogic : BusinessBaseLogic<GeneralAudit, GENERAL_AUDIT>
    {
        public GeneralAuditLogic()
        {
            translator = new GeneralAuditTranslator();
        }
        public GeneralAudit CreateGeneralAudit(GeneralAudit generalAudit)
        {
            try
            {
                if (generalAudit != null)
                {
                    UserLogic userLogic = new UserLogic();

                    User user = userLogic.GetModelBy(p => p.User_Name == HttpContext.Current.User.Identity.Name.ToString());
                    string client = user.Username + " (" + HttpContext.Current.Request.UserHostAddress + ")";
                    generalAudit.User = user;
                    generalAudit.Client = client;
                    generalAudit.Time = DateTime.Now;

                    generalAudit = base.Create(generalAudit);
                }

                return generalAudit;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
