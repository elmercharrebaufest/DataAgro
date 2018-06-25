using System.Web;
using System.Web.Optimization;

namespace WebDataAgro
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // -------------------------------------------------
            //   Estilos generales
            // -------------------------------------------------

            bundles.Add(new StyleBundle("~/Content/awesome/css").Include(
                        "~/Content/assets/global/plugins/font-awesome/css/font-awesome.min.css",
                        "~/Content/assets/global/plugins/simple-line-icons/simple-line-icons.min.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap/css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/bootstrap-switch.min.css",
                        "~/Content/bootstrap-dialog.min.css"));

            bundles.Add(new StyleBundle("~/Content/plugins/css").Include(
                        "~/Content/assets/global/css/components.min.css",
                        "~/Content/assets/global/css/plugins.min.css")); 

            bundles.Add(new StyleBundle("~/Content/layouts/css").Include(
                       "~/Content/assets/layouts/layout4/css/layout.min.css",
                       "~/Content/assets/layouts/layout4/css/themes/default.min.css",
                       "~/Content/assets/layouts/layout4/css/custom.min.css"));
            
            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
                        "~/Content/kendo/kendo.common-bootstrap.min.css",
                        "~/Content/kendo/kendo.metro.min.css"));
            
            bundles.Add(new StyleBundle("~/Content/mastersoft/css").Include(
                        "~/Content/Mastersoft.css"));
            
            // -------------------------------------------------
            //   Scripts generales
            // -------------------------------------------------

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/bootstrap-dialog.min.js",
                        "~/Scripts/jquery.bootstrap.wizard.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                       "~/Content/assets/global/plugins/js.cookie.min.js",
                       "~/Content/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                       "~/Content/assets/global/plugins/jquery.blockui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/layouts").Include(
                        "~/Content/assets/global/scripts/app.min.js",
                        "~/Content/assets/layouts/layout4/scripts/layout.min.js",
                        "~/Content/assets/layouts/layout4/scripts/demo.min.js",
                        "~/Content/assets/layouts/global/scripts/quick-sidebar.min.js",
                        "~/Content/assets/layouts/global/scripts/quick-nav.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                        "~/Scripts/kendo/jszip.min.js",
                        "~/Scripts/kendo/kendo.all.min.js",
                        // "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
                        "~/Scripts/kendo/cultures/kendo.culture.es-AR.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/mastersoft").Include(
                        "~/Scripts/Mastersoft.js"));

            // -------------------------------------------------
            //   Scripts de aplicacion
            // -------------------------------------------------

            bundles.Add(new ScriptBundle("~/bundles/AbmAreaInfluencia").Include(
                                         "~/Scripts/App/AbmAreaInfluencia.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmCanalOperacion").Include(
                                         "~/Scripts/App/AbmCanalOperacion.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmCondicion").Include(
                                         "~/Scripts/App/AbmCondicion.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmDestinatario").Include(
                                         "~/Scripts/App/AbmDestinatario.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmMaterial").Include(
                                         "~/Scripts/App/AbmMaterial.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmProvincia").Include(
                                         "~/Scripts/App/AbmProvincia.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmLocalidad").Include(
                                         "~/Scripts/App/AbmLocalidad.js"));

            bundles.Add(new ScriptBundle("~/bundles/CubProveedores").Include(
                                         "~/Scripts/App/CubProveedores.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmComercial").Include(
                                       "~/Scripts/App/AbmComercial.js"));

            bundles.Add(new ScriptBundle("~/bundles/InformeComercial").Include(
                                      "~/Scripts/App/InformeComercial.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmFijacionDePrecio").Include(
                                         "~/Scripts/App/AbmFijacionDePrecio.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmPreslip").Include(
                                         "~/Scripts/App/AbmPreslip.js"));

            bundles.Add(new ScriptBundle("~/bundles/CompraNetContrato").Include(
                                         "~/Scripts/App/CrearContrato.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/CompraNetFijacion").Include(
                                         "~/Scripts/App/CrearFijacion.js"));
                                         
            bundles.Add(new ScriptBundle("~/bundles/CompraNetIndex").Include(
                                         "~/Scripts/KendoExtensions.js",
                                         "~/Scripts/App/CompraNetIndex.js"));

            bundles.Add(new ScriptBundle("~/bundles/ContratoIndex").Include(
                                         "~/Scripts/KendoExtensions.js",
                                         "~/Scripts/App/ReporteContrato.js"));
            //------------------------

            bundles.IgnoreList.Clear();
        }
    }
}










































