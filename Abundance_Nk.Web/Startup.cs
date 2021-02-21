using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Abundance_Nk.Web.Startup))]
namespace Abundance_Nk.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
