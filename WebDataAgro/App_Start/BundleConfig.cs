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
            var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Styles.DefaultTagFormat = "<link href='{0}?v=" + assemblyVersion + "' rel='stylesheet'/>";
            Scripts.DefaultTagFormat = "<script src='{0}?v=" + assemblyVersion + "'></script>";

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
                        "~/Scripts/jquery.min.js",
                        "~/Scripts/App/firebase-messaging-sw.js"));

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

            bundles.Add(new ScriptBundle("~/bundles/AbmCentro").Include(
                                       "~/Scripts/App/AbmCentro.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmZona").Include(
                                       "~/Scripts/App/AbmZona.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmZonaCupo").Include(
                                      "~/Scripts/App/AbmZonaCupo.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmOperador").Include(
                                       "~/Scripts/App/AbmOperador.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmContratoAcuerdo").Include(
                                                 "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/App/AbmContratoAcuerdo.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmRangoPrecio").Include(
                                       "~/Scripts/App/AbmRangoPrecio.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmTareaProgramada").Include(
                                       "~/Scripts/App/AbmTareaProgramada.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmRangoConfirmacionAutomatica").Include(
                                       "~/Scripts/App/AbmRangoConfirmacionAutomatica.js"));

            bundles.Add(new ScriptBundle("~/bundles/InformeComercial").Include(
                                      "~/Scripts/App/InformeComercial.js"));

            bundles.Add(new ScriptBundle("~/bundles/InformeAdministrativoIndex").Include(
                                        "~/Scripts/KendoExtensions.js",
                                        "~/Scripts/App/Filtros.js",
                                        "~/Scripts/App/InformeAdministrativo.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmFijacionDePrecio").Include(
                                         "~/Scripts/App/AbmFijacionDePrecio.js"));

            bundles.Add(new ScriptBundle("~/bundles/CompraNetContratoAPrecio").Include(
                                         "~/Scripts/App/CopiarContrato.js",
                                         "~/Scripts/App/CrearContrato.js",
                                         "~/Scripts/jquery.mask.js"));

            bundles.Add(new ScriptBundle("~/bundles/CompraNetContratoAFijar").Include(
                                        "~/Scripts/App/CopiarContrato.js",
                                        "~/Scripts/App/AFijar.js",
                                        "~/Scripts/jquery.mask.js"));

            bundles.Add(new ScriptBundle("~/bundles/CompraNetContratoFijacion").Include(
                                        "~/Scripts/App/CopiarContrato.js",
                                        "~/Scripts/App/Fijacion.js",
                                        "~/Scripts/jquery.mask.js"));

            bundles.Add(new ScriptBundle("~/bundles/CompraNetContratoAcuerdo").Include(
                                       "~/Scripts/App/CopiarContrato.js",
                                       "~/Scripts/App/ContratoAcuerdo.js",
                                       "~/Scripts/jquery.mask.js"));

            bundles.Add(new ScriptBundle("~/bundles/CompraNetContratoExterno").Include(
                                       "~/Scripts/App/CrearContratoExterno.js",
                                       "~/Scripts/jquery.mask.js"));

            bundles.Add(new ScriptBundle("~/bundles/CompraNetIndex").Include(
                                         "~/Scripts/KendoExtensions.js",
                                         "~/Scripts/App/firebase-suscribir.js",
                                          "~/Scripts/App/Filtros.js",
                                         "~/Scripts/App/CompraNetIndex.js"));
            bundles.Add(new ScriptBundle("~/bundles/ContratoIndex").Include(
                                         "~/Scripts/KendoExtensions.js",
                                         "~/Scripts/App/Filtros.js",
                                         "~/Scripts/App/ReporteContrato.js"));

            bundles.Add(new ScriptBundle("~/bundles/ReporteProveedores").Include(
                                         "~/Scripts/KendoExtensions.js",
                                         "~/Scripts/App/Filtros.js",
                                         "~/Scripts/App/ReporteProveedores.js"));

            bundles.Add(new ScriptBundle("~/bundles/Cupos").Include(
                             "~/Scripts/KendoExtensions.js",
                             "~/Scripts/App/Filtros.js",
                             "~/Scripts/App/Cupo.js",
                             "~/Scripts/jquery.mask.js",
                             "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/Rango").Include(
                           "~/Scripts/KendoExtensions.js",
                           "~/Scripts/App/Filtros.js",
                            "~/Scripts/jquery.mask.js",
                           "~/Scripts/App/reporteRangoConfirmacionAutomatica.js"));

            bundles.Add(new ScriptBundle("~/bundles/PrecioMoaPizarra").Include(
                           "~/Scripts/KendoExtensions.js",
                           "~/Scripts/App/Filtros.js",
                            "~/Scripts/jquery.mask.js",
                           "~/Scripts/App/reportePrecioMoaPizarra.js"));

            bundles.Add(new ScriptBundle("~/bundles/Log").Include(
                                       "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/App/log.js"));

            bundles.Add(new ScriptBundle("~/bundles/PrecioPizarra").Include(
                                      "~/Scripts/KendoExtensions.js",
                                      "~/Scripts/App/PrecioPizarra.js"));

            bundles.Add(new ScriptBundle("~/bundles/ConfiguracionInterna").Include(
                                     "~/Scripts/KendoExtensions.js",
                                     "~/Scripts/App/ConfiguracionInterna.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmRol").Include(
                                     "~/Scripts/KendoExtensions.js",
                                     "~/Scripts/App/AbmRol.js"));

            bundles.Add(new ScriptBundle("~/bundles/ReporteCupo").Include(
                                    "~/Scripts/KendoExtensions.js",
                                    "~/Scripts/App/Filtros.js",
                                    "~/Scripts/App/ReporteCupo.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmFormula").Include(
                                       "~/Scripts/App/AbmFormula.js"));

            bundles.Add(new ScriptBundle("~/bundles/LogDataAgro").Include(
                                          "~/Scripts/KendoExtensions.js",
                                          "~/Scripts/App/Filtros.js",
                                          "~/Scripts/App/LogDataAgro.js"));

            bundles.Add(new ScriptBundle("~/bundles/AbmFechaFeriado").Include(
                                      "~/Scripts/App/AbmFechaFeriado.js"));
            bundles.Add(new ScriptBundle("~/bundles/ReporteEvolucionFijacion").Include(
                             //"~/Scripts/KendoExtensions.js",
                             //"~/Scripts/App/Filtros.js",
                             "~/Scripts/App/ReporteEvolucionFijacion.js"));

            bundles.Add(new ScriptBundle("~/bundles/Pesificado").Include(
                                       "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/App/Filtros.js",
                                       "~/Scripts/App/ReportePesificados.js"));

            bundles.Add(new ScriptBundle("~/bundles/ReporteAfijarIndex").Include(
                                    "~/Scripts/KendoExtensions.js",
                                    "~/Scripts/App/Filtros.js",
                                    "~/Scripts/App/ReporteContratosAFijarPase.js"));

            bundles.Add(new ScriptBundle("~/bundles/NegocioPesificacion").Include(
                                    "~/Scripts/KendoExtensions.js",
                                    "~/Scripts/App/Filtros.js",
                                    "~/Scripts/App/NegocioPesificacion.js"));

            bundles.Add(new ScriptBundle("~/bundles/Proveedor").Include(
                                    "~/Scripts/App/AgregarCorredor.js",
                                    "~/Scripts/App/agregarproveedor.js",
                                    "~/Scripts/App/agregarproveedoredit.js",
                                    "~/Scripts/App/ValidacionProveedor.js"));

            bundles.Add(new ScriptBundle("~/bundles/ConfiguracionCupo").Include(
                                    "~/Scripts/KendoExtensions.js",
                                    "~/Scripts/App/ConfiguracionCupo.js",
                                    "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/AdministracionCupo").Include(
                                    "~/Scripts/KendoExtensions.js",
                                    "~/Scripts/App/AdministracionCupoGrilla.js",
                                    "~/Scripts/jquery.unobtrusive-ajax.js",
                                    "~/Scripts/bootstrap-toggle.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/SugerenciaCupo").Include(
                                    "~/Scripts/KendoExtensions.js",
                                    "~/Scripts/App/SugerenciaCupo.js",
                                    "~/Scripts/App/SolicitudCupo.js",
                                    "~/Scripts/kendo/messages/kendo.messages.es-AR.min.js",
                                    "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/FAQ").Include(
                                    "~/Scripts/KendoExtensions.js",
                                    "~/Scripts/App/FAQ.js"));

            bundles.Add(new ScriptBundle("~/bundles/Research").Include(
                                       "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/App/Research.js"));
            bundles.Add(new ScriptBundle("~/bundles/ResearchReporte").Include(
                                         "~/Scripts/KendoExtensions.js",
                                         "~/Scripts/App/Filtros.js",
                                         "~/Scripts/App/ResearchReporte.js"));
            bundles.Add(new ScriptBundle("~/bundles/ResearchMapa").Include(
                                       "~/Scripts/KendoExtensions.js"
                                       //"~/Scripts/App/ResearchMapa.js"
                                       ));
            bundles.Add(new ScriptBundle("~/bundles/GenerarBoletos").Include(
                                       "~/Scripts/App/GenerarBoleto.js",
                                       "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/App/Filtros.js",
                                       "~/Scripts/moment.js"
                                       ));
            bundles.Add(new ScriptBundle("~/bundles/DescargarBoletos").Include(
                                       "~/Scripts/App/DescargarBoleto.js",
                                       "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/moment.js"
                                       ));
            bundles.Add(new ScriptBundle("~/bundles/GenerarConfirma").Include(
                                       "~/Scripts/App/Filtros.js",
                                       "~/Scripts/App/GenerarConfirma.js",
                                       "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/moment.js"
                                       ));
            bundles.Add(new ScriptBundle("~/bundles/DescargarConfirma").Include(
                                       "~/Scripts/App/DescargarConfirma.js",
                                       "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/moment.js"
                                       ));
            bundles.Add(new ScriptBundle("~/bundles/GestionarClausulas").Include(
                                       "~/Scripts/App/GestionarClausulas.js",
                                       "~/Scripts/KendoExtensions.js"
                                       ));
            bundles.Add(new ScriptBundle("~/bundles/GestionarClausulasConfirma").Include(
                                       "~/Scripts/App/GestionarClausulasConfirma.js",
                                       "~/Scripts/KendoExtensions.js"
                                       ));

            bundles.Add(new ScriptBundle("~/bundles/CambioDePerfil").Include(
                                      "~/Scripts/App/CambioDePerfil.js"));

            bundles.Add(new ScriptBundle("~/bundles/ControlDeBoletos").Include(
                                       "~/Scripts/App/Filtros.js",
                                       "~/Scripts/App/ControlDeBoletos.js",
                                       "~/Scripts/App/ControlDeBoletosModificarContrato.js",
                                       "~/Scripts/App/ControlDeBoletosTracking.js",
                                       "~/Scripts/App/ControlDeBoletosVisualizarContrato.js",
                                       "~/Scripts/KendoExtensions.js",
                                       "~/Scripts/moment.js"
                                       ));
            bundles.IgnoreList.Clear();
        }
    }
}