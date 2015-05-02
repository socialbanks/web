using System.Web;
using System.Web.Optimization;

namespace SocialBanksWeb
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.0.3.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapjs").Include(
           "~/Scripts/flat-ui/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryscroll").Include(
                       "~/Scripts/common-files/js/jquery.scrollTo-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/pagetransitions").Include(
                       "~/Scripts/common-files/js/page-transitions.js"));

            bundles.Add(new ScriptBundle("~/bundles/easing").Include(
                       "~/Scripts/common-files/js/easing.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerysvg").Include(
                       "~/Scripts/common-files/js/jquery.svg.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerysvganim").Include(
                       "~/Scripts/common-files/js/jquery.svganim.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryparallax").Include(
                       "~/Scripts/common-files/js/jquery.parallax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/startupkitjs").Include(
                       "~/Scripts/common-files/js/startup-kit.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));
           
            bundles.Add(new ScriptBundle("~/bundles/jqueryuicustom").Include(
                        "~/Scripts/flat-ui/js/jquery-ui-{version}.custom.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            
            bundles.Add(new ScriptBundle("~/bundles/modernizrcustom").Include(
                       "~/Scripts/common-files/js/modernizr.custom.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
            
            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
           "~/Content/themes/flat-ui/bootstrap/css/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/flatuicss").Include(
                        "~/Content/themes/flat-ui/css/flat-ui.css"));

            bundles.Add(new StyleBundle("~/Content/iconfont").Include(
                        "~/Content/themes/common-files/css/icon-font.css"));

            bundles.Add(new StyleBundle("~/Content/animations").Include(
                        "~/Content/themes/common-files/css/animations.css"));

            // startup frameworks elements style
            // header
            bundles.Add(new StyleBundle("~/Content/headercss").Include(
                        "~/Content/themes/ui-kit/ui-kit-header/css/style.css"));

            // content
            bundles.Add(new StyleBundle("~/Content/contentcss").Include(
                        "~/Content/themes/ui-kit/ui-kit-content/css/style.css"));

            // footer
            bundles.Add(new StyleBundle("~/Content/footercss").Include(
                        "~/Content/themes/ui-kit/ui-kit-footer/css/style.css"));
        }
    }
}