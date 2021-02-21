using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant
{
    public class ApplicantAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Applicant";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Applicant_default",
                "Applicant/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }




    }
}