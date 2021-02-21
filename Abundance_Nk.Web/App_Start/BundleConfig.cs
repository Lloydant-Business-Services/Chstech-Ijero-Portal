using System.Web;
using System.Web.Optimization;

namespace Abundance_Nk.Web
{
    public class BundleConfig
    {
       
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",

                        "~/Scripts/jquery.tagsinput.min.js",
                        "~/Scripts/jquery.autogrow-textarea.js",
                       
                        //"~/Scripts/misc.js",

                        "~/Scripts/jquery-migrate-1.2.1.min.js",

                        "~/Scripts/gridmvc.min.js",

                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.print.js",

                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/dataTables.js"

                        //"~/Scripts/PostJAMB.js"

                        ));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/site.css",
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-theme.min.css",
                      "~/Content/bootstrap-override.css",
                       "~/Content/style.default.css",
                         "~/Content/Gridmvc.css",
                         "~/Content/misc.css",
                         "~/Content/dataTables.css"



                          ));
        }


    }
}

  //<script src="@Url.Content("~/Scripts/jquery.min.js")" type="text/javascript"> </script>
  //  <script src="@Url.Content("~/Scripts/gridmvc.min.js")" type="text/javascript"> </script>
  //  <link href="@Url.Content("~/Content/Gridmvc.css")" rel="stylesheet" type="text/css" />


//"~/Content/bootstrap.css",
//"~/Content/bootstrap-override.css",
//"~/Content/weather-icons.min.css",
//"~/Content/jquery-ui-1.10.3.css",
//"~/Content/font-awesome.min.css",
//"~/Content/animate.min.css",
//"~/Content/animate.delay.css",
//"~/Content/toggles.css",
//"~/Content/pace.css"






// "~/Scripts/jquery.min.js",
//"~/Scripts/gridmvc.min.js",

//"~/Scripts/jquery-1.11.1.min.js",
//"~/Scripts/jquery-migrate-1.2.1.min.js",

//"~/Scripts/modernizr.min.js",
//"~/Scripts/pace.min.js",
//"~/Scripts/retina.min.js",
//"~/Scripts/jquery.cookies.js",
//"~/Scripts/custom.js",
//"~/Scripts/jquery.unobtrusive-ajax.min.js"



//"~/Content/style.default.css",
//"~/Content/Gridmvc.css"