using System.Web;
using System.Web.Optimization;

namespace DISTRESSUITY.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"
                      //"~/Scripts/distressuity-theme.js"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/jquery.signalR").Include(
                "~/Scripts/jquery.signalR-2.2.0.min.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.min.css",
                      //"~/Content/font-awesome.min.css"
                      //"~/Content/bootstrap-social/bootstrap-social.css",
                      //"~/Content/bootstrap-social/bootstrap-social.less",
                      //"~/Content/bootstrap-social/bootstrap-social.scss",
                      //"~/Content/distressuity-theme.css",
                      //"~/Content/distressuity-theme-responsive.css"
                      ));
        }
    }
}
