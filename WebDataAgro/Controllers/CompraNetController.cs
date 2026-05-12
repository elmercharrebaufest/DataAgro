using NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Helpers.Excel;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class CompraNetController : Controller
    {
        private readonly IHomeManager homeManager;
        private readonly ICompraNetManager mobjCompraNetManager;
        private readonly IContratoManager mobjContratoManager;
        private readonly IFijacionDePrecioContratoManager mobjFijacionDePrecioContratoManager;
        private readonly IComercialManager mobjComercialManager;
        private readonly ICampañaManager mobjCampañaManager;
        private readonly ILocalidadManager mobjLocalidadManager;
        private readonly IMaterialManager mobjMaterialManager;
        private readonly IProveedorManager mobjProveedorManager;
        private readonly IOperadorManager mobjOperadorManager;
        private readonly ILogger logger;
        private readonly INegocioManager mobjNegocioManager;
        private readonly IFasonManager mobjFasonManager;
        private readonly IAgenteCompraManager mobjAgenteManager;
        private readonly IContratoAcuerdoManager mobjContratoAcuerdoManager;
        private readonly IConfiguracionInternaManager configuracionInternaManager;
        private readonly IConfiguracionManager mobjConfiguracionManager;
        //private readonly IContrato2Manager contratoMediator;
        private readonly ITipoDeCambioAgent tipoDeCambioAgent;
        private readonly ICentroManager centroManager;
        private readonly IDiasHabilesAgent diasHabilesAgent;
        private readonly IReportesManager reportesManager;

        public CompraNetController(IHomeManager oHomeManager, ILocalidadManager ojLocalidadManager,
            IProveedorManager oProveedorManager, IMaterialManager oMaterialManager,
            IContratoManager oContratoManager, IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager,
            ICompraNetManager oCompraNetManager, IComercialManager oComercialManager, ICampañaManager oCampañaManager,
            ILogger oLogger, IFasonManager oFasonManager, IAgenteCompraManager oAgenteManager, IContratoAcuerdoManager oContratoAcuerdoManager,
            IConfiguracionInternaManager oConfiguracionInternaManager, IConfiguracionManager configuracionManager,
            IOperadorManager oOperadorManager, INegocioManager oNegocioManager, //IContrato2Manager oContratoMediator,
            ITipoDeCambioAgent oTipoDeCambioAgent, ICentroManager oCentroManager, IDiasHabilesAgent oDiasHabilesAgent, IReportesManager oReportesManager)
        {
            homeManager = oHomeManager;
            mobjComercialManager = oComercialManager;
            mobjCompraNetManager = oCompraNetManager;
            mobjContratoManager = oContratoManager;
            mobjFijacionDePrecioContratoManager = oFijacionDePrecioContratoManager;
            mobjCampañaManager = oCampañaManager;
            mobjMaterialManager = oMaterialManager;
            mobjProveedorManager = oProveedorManager;
            mobjOperadorManager = oOperadorManager;
            mobjLocalidadManager = ojLocalidadManager;
            logger = oLogger;
            mobjFasonManager = oFasonManager;
            mobjAgenteManager = oAgenteManager;
            mobjContratoAcuerdoManager = oContratoAcuerdoManager;
            mobjConfiguracionManager = configuracionManager;
            mobjNegocioManager = oNegocioManager;
            reportesManager = oReportesManager;
            configuracionInternaManager = oConfiguracionInternaManager;
            //contratoMediator = oContratoMediator;
            tipoDeCambioAgent = oTipoDeCambioAgent;
            centroManager = oCentroManager;
            diasHabilesAgent = oDiasHabilesAgent;
        }

        [Autorizacion(PermisosDataAgro.VisualizarCompraNet)]
        public ActionResult Index()
        {
            ViewBag.TieneEmpleadosACargo = GlobalVariables.TieneEmpleadosACargo;
            ViewBag.comercialId = GlobalVariables.ComercialId;
            ViewBag.Actualizacion = mobjContratoManager.DevolverMilisegundos();
            return View();
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocios, PermisosDataAgro.NuevoNegocioExterno, PermisosDataAgro.ModificarNegocios, PermisosDataAgro.ModificarNegFinalizados, PermisosDataAgro.ModificarCanje, PermisosDataAgro.ModificarDolarizadoExpress, PermisosDataAgro.ModificarDolarizadoFinalizado)]
        public ActionResult CrearContrato(int? id, int? tipoId, string siguientes, string obj)
        {
            if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                return RedirectToAction("CrearContratoExterno");
            }
            ViewBag.ActivarSojaEUDR = ConfigurationManager.AppSettings["ActivarSojaEUDR"];

            ViewBag.ComercialId = GlobalVariables.ComercialId;
            ViewBag.Id = id;
            ViewBag.TipoId = tipoId;
            ViewBag.Contrato = mobjContratoManager.ObtenerSapContrato(id ?? 0);
            ViewBag.Fijacion = mobjContratoManager.ObtenerSapFijacion(id ?? 0);
            ViewBag.Siguientes = siguientes;
            ViewBag.ContratoAperturaPrecioPorcentajeDeComisionMaximo = mobjConfiguracionManager.TraerConfiguraciones().ContratoAperturaPrecioPorcentajeDeComisionMaximo;
            ViewBag.Obj = null;
            ViewBag.esEdicion = false;
            if (!String.IsNullOrEmpty(obj))
            {
                var tipoNegocio = mobjContratoManager.DevolverNamespaceNegocio(tipoId.Value);
                //var contrato = typeof(CompraNetController).GetMethod("DeserializarJson").MakeGenericMethod(Type.GetType($"{tipoNegocio.ClaseDescripcion}, Molinos.DataAgro.Entities")).Invoke(null, new object[] { obj });
                var negocio = typeof(CompraNetController).GetMethod("DeserializarJson").MakeGenericMethod(Type.GetType($"{tipoNegocio.ClaseDescripcion}, Molinos.DataAgro.Entities")).Invoke(null, new object[] { obj }) as Negocio;
                ViewBag.Obj = mobjContratoManager.NegocioABasicoContrato(negocio);
                ViewBag.esEdicion = true;
                return View(tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.CONTRATO_ACUERDO ? tipoNegocio.Descripcion.Replace(" ", String.Empty) : "CrearContrato");
            }
            if (id > 0 || (PermisosHelper.Is(PermisosDataAgro.ModificarCanje)))
            {
                tipoId = PermisosHelper.Is(PermisosDataAgro.ModificarCanje) && (id == null || id == 0) ? 1 : tipoId ?? 2;
                var tipoNegocio = mobjContratoManager.DevolverNamespaceNegocio(tipoId.Value);
                return View(tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.CONTRATO_ACUERDO ? tipoNegocio.Descripcion.Replace(" ", String.Empty) : "CrearContrato");
            }

            return View();
        }

        public static Negocio DeserializarJson<T>(string obj) where T : Negocio
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter()
            {
                DateTimeFormat = "dd-MM-yyyy",
                Culture = CultureInfo.CurrentCulture, //CultureInfo.InvariantCulture,
                DateTimeStyles = DateTimeStyles.AssumeUniversal,
            });
            return JsonConvert.DeserializeObject<T>(obj, settings);
        }

        public ActionResult ValidarModificarFinalizado(int? id)
        {
            var resultado = id.HasValue ? mobjContratoManager.ValidarStatus(id.Value) : new EstadoSAPDto();
            return Json(resultado);
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocioExterno, PermisosDataAgro.ModificarNegocioExterno)]
        public ActionResult CrearContratoExterno(int? id, int? tipoId)
        {
            ViewBag.ComercialId = GlobalVariables.ComercialId;
            ViewBag.Id = id;
            ViewBag.TipoId = tipoId;
            var cuit = PermisosHelper.ObtenerCuit();
            var directo = mobjProveedorManager.ValidarDirecto(cuit);
            if (directo)
            {
                ViewBag.Proveedor = $"Externo({cuit})";
                ViewBag.Rol = "Proveedor";
            }
            else
            {
                ViewBag.Corredor = $"Externo({cuit})";
                ViewBag.Rol = "Corredor";
            }
            return View();
        }

        public ActionResult Inicializar()
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            return new JsonResult()
            {
                Data = new DatosIniCompraNetModel
                {
                    Datos = mobjCompraNetManager.TraerDatosIniciales(equipo)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult InicializarContrato(int? tipoNegocioId)
        {
            return new JsonResult()
            {
                Data = new ContratoModel_prueba
                {
                    Datos = mobjContratoManager.TraerDatosCombo(tipoNegocioId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult InicializarFijacion()
        {
            return new JsonResult()
            {
                Data = new FijacionDePrecioContratoModel
                {
                    Datos = mobjFijacionDePrecioContratoManager.TraerDatosIniciales()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult GrabarContrato(Contrato oParam, List<CupoConDescargaFechasDto> listCupoConDescargaFechas = null)
        {
            if (oParam.Base == null) oParam.Base = false;
            if (oParam.NoInformaSio == null) oParam.NoInformaSio = false;
            if (oParam.TrigoEspecial == null) oParam.TrigoEspecial = false;
            if (oParam.EsFason == null) oParam.EsFason = false;

            if (oParam.ComercialId.HasValue)
            {
                var comercial = mobjComercialManager.TraerComercial(oParam.ComercialId.Value);
                oParam.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }

            oParam.UsuarioId = GlobalVariables.IdActiveDirectory;
            GrabarContratoResult model;
            if (oParam.EstadoId == (int)EnumEstadoContrato.Finalizado || oParam.EstadoId == (int)EnumEstadoContrato.ReconfirmarFinalizado)
            {
                model = mobjContratoManager.ActualizarContratoFinalizado(oParam, listCupoConDescargaFechas);
            }
            else
            {
                model = mobjContratoManager.GrabarContrato(oParam, listCupoConDescargaFechas);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        static readonly object _lockFinalizarContrato = new object();
        public ActionResult FinalizarContrato(int contratoId)
        {
            lock (_lockFinalizarContrato)
            {
                return new JsonResult()
                {
                    Data = mobjContratoManager.FinalizarContrato(contratoId, GlobalVariables.IdActiveDirectory),
                    MaxJsonLength = Int32.MaxValue
                };
            }

        }

        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarContrato(int contratoId)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ConfirmarContrato(contratoId, GlobalVariables.ComercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BorrarContrato(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.BorrarContrato(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BorrarFijacion(FijacionDePrecioContrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.BorrarFijacion(oParam),
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult BorrarFijacionPreAprobacion(int id, string motivoRechazo)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.BorrarFijacionPreAprobacion(id, motivoRechazo),
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult BorrarFason(Fason oParam)
        {
            return new JsonResult()
            {
                Data = mobjFasonManager.BorrarFason(oParam),
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult BorrarAgente(AgenteCompra oParam)
        {
            return new JsonResult()
            {
                Data = mobjAgenteManager.BorrarAgente(oParam),
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult BorrarAcuerdo(ContratoAcuerdo oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.BorrarAcuerdo(oParam),
                MaxJsonLength = Int32.MaxValue
            };

        }

        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarFijacion(int fijacionDePrecioContratoId)
        {

            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.ConfirmarFijacion(fijacionDePrecioContratoId, GlobalVariables.ComercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarAcuerdo(int fijacionDePrecioContratoId)
        {
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.ConfirmarContratoAcuerdo(fijacionDePrecioContratoId, GlobalVariables.ComercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarAgente(int fijacionDePrecioContratoId)
        {
            return new JsonResult()
            {
                Data = mobjAgenteManager.ConfirmarAgenteCompra(fijacionDePrecioContratoId, GlobalVariables.ComercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarFason(int fijacionDePrecioContratoId)
        {
            return new JsonResult()
            {
                Data = mobjFasonManager.ConfirmarFason(fijacionDePrecioContratoId, GlobalVariables.ComercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        static readonly object _lockFinalizarFijacion = new object();
        public ActionResult FinalizarFijacion(int fijacionDePrecioContratoId)
        {
            lock (_lockFinalizarFijacion)
            {
                return new JsonResult()
                {
                    Data = mobjFijacionDePrecioContratoManager.FinalizarFijacion(fijacionDePrecioContratoId, GlobalVariables.IdActiveDirectory),
                    MaxJsonLength = Int32.MaxValue
                };
            }
        }

        public ActionResult ReenviarMails(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = new GrabarContratoResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAmpliacionContrato(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.GrabarAmpliacionContrato(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult GrabarAmpliacionAcuerdo(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.GrabarAmpliacionContrato(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAmpliacionFijacion(FijacionDePrecioContrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.GrabarAmpliacionFijacion(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarFijacion(FijacionDePrecioContrato oParam)
        {
            if (oParam.PorcentajeComision.HasValue)
            {
                oParam.PorcentajeComision = 0;
            }
            var model = new GrabarFijacionResult();
            if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                oParam.ComercialId = mobjComercialManager.ComercialAsociado(oParam.CorredorId.HasValue && oParam.CorredorId != 0 ? oParam.CorredorId.Value : oParam.ProveedorId ?? 0);
                oParam.UsuarioId = PermisosHelper.ObtenerUsuario();
            }
            if (oParam.EstadoId == (int)EnumEstadoContrato.Finalizado)
            {
                model = mobjFijacionDePrecioContratoManager.ActualizarFijacion(oParam);
            }
            else
            {
                model = mobjFijacionDePrecioContratoManager.GrabarFijacionDePrecio(oParam);
            }
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest request)
        {
            if (request.Sort != null)
            {
                request.Sort = request.Sort.Concat(new[] { new Sort { Field = "Estado_Order", Dir = "asc" } });
            }
            else
            {
                request.Sort = new List<Sort> { new Sort { Field = "Estado_Order", Dir = "asc" } };
            }
            request.Sort = request.Sort.Concat(new[] { new Sort { Field = "Fecha_Order", Dir = "desc" } });

            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;

            if (request.Filter != null && request.Filter.Filters != null && request.Filter.Filters.Any(x => x.Filters != null ? x.Filters.Any(y => y.Field != null ? y.Field.Contains("ContratoSAP") || y.Field.Contains("ContratoCorredor") : false) : false))
            {
                equipo = GlobalVariables.EquipoReal;
            }
            if (request.Filter != null && request.Filter.Filters != null)
            {
                foreach (var item in request.Filter.Filters)
                {
                    if (item.Field == "Negocio" && item.Operator == "eq")
                    {
                        item.Value = item.Value.ToString().PadLeft(10, '0');
                        if (item.Value.ToString().TrimStart('0').Length > 7)
                        {
                            item.Operator = "contains";
                        }
                        else if (item.Value.ToString().TrimStart('0').Length <= 6)
                        {
                            item.Value = item.Value.ToString().TrimStart('0');
                            item.Operator = "eq";
                        }
                    }
                    if (item.Filters != null)
                    {
                        foreach (var item2 in item.Filters)
                        {
                            if (item2.Field == "Negocio" && item.Operator == "eq")
                            {
                                item2.Value = item2.Value.ToString().PadLeft(10, '0');
                                if (item2.Value.ToString().TrimStart('0').Length > 7)
                                {
                                    item2.Operator = "contains";
                                }
                                else if (item.Value.ToString().TrimStart('0').Length <= 6)
                                {
                                    item.Value = item.Value.ToString().TrimStart('0');
                                    item.Operator = "eq";
                                }
                            }
                        }
                    }
                }
            }
            if (request.Filter != null && !string.IsNullOrEmpty(request.Filter.Logic) && request.Filter.Filters == null)
            {
                return Json(null);

            }
            var model = mobjContratoManager.TraerTodosContratos(request, PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial), equipo, GlobalVariables.CorredoresComercial);

            return Json(model);
        }

        public ActionResult TraerCampanaPorMaterial(int? materialId)
        {
            if (materialId == null)
            {
                materialId = 0;
            }

            return new JsonResult()
            {
                Data = mobjCampañaManager.TraerCampañaPorMaterial(Convert.ToInt32(materialId)),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerLocalidadPorProvincia(int? provinciaId)
        {
            if (provinciaId == null)
            {
                provinciaId = 0;
            }

            return new JsonResult()
            {
                Data = mobjLocalidadManager.TraerLocalidadPorProvincia(provinciaId.Value),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public int? TraerCampanaActualMaterial(int MaterialId)
        {
            return mobjMaterialManager.TraerMaterial(MaterialId).CampañaId;
        }

        public ActionResult TraerCalidadesPorMaterial(int? MaterialId)
        {
            if (MaterialId == null)
            {
                MaterialId = 0;
            }

            return new JsonResult()
            {
                Data = mobjCampañaManager.TraerCalidadPorMaterial(Convert.ToInt32(MaterialId)),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public int ObtenerProveedorId(string Cuit, bool corredor)
        {
            var proveedor = mobjProveedorManager.TraerProveedorPorCuit(Cuit, corredor);
            if (proveedor == null)
            {
                return 0;
            }
            else
            {
                return proveedor.ProveedorId;
            }
        }

        public ActionResult ObtenerLocalidadId(string localidad, string provincia)
        {
            return new JsonResult()
            {
                Data = mobjLocalidadManager.TraerLocalidadProvincia(localidad, provincia),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerProvinciaLocalidad(string Cuit)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerLocalidadProveedorPorCuit(Cuit),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerProvinciaLocalidadProv(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerLocalidadProveedorPorCuit(oDatosLocalidadProvinciaFiltro),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BuscarCuitPorId(int? ProveedorId)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerProveedor(ProveedorId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarProveedorTodos(string text = "")
        {
            var proveedores = mobjProveedorManager.ListarProveedorTodos(text);
            var operadores = mobjOperadorManager.ListarOperador(text).Select(a => new ProveedorDto { ProveedorId = a.Id, RazonSocial = a.Descripcion });
            if (operadores.Count() > 0)
                proveedores.AddRange(operadores);

            return Json(proveedores.Select(x => new { x.ProveedorId, Proveedor = !string.IsNullOrEmpty(x.Alias) ? x.Alias + " - " + x.RazonSocial : x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProveedor(string text = "")
        {
            var proveedores = mobjProveedorManager.ListarProveedor(text);
            var operadores = mobjOperadorManager.ListarOperador(text).Select(a => new ProveedorDto { ProveedorId = a.Id, RazonSocial = a.Descripcion });
            if (operadores.Count() > 0)
                proveedores.AddRange(operadores);

            return Json(proveedores.Select(x => new { x.ProveedorId, Proveedor = !string.IsNullOrEmpty(x.Alias) ? x.Alias + " - " + x.RazonSocial : x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarCorredor(string text = "")
        {
            var corredores = mobjProveedorManager.ListarCorredor(text);
            return Json(corredores.Select(x => new { x.ProveedorId, Proveedor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarComercial(string text = "")
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var comerciales = mobjComercialManager.ListarComercial(text, equipo);
            return Json(comerciales.Select(x => new { x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TraerDescuentosPorContrato(int contratoId = 0)
        {
            var model = mobjContratoManager.TraerDescuentosPorContrato(contratoId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult TraerCalidadesPorContrato(int contratoId = 0, int acuerdoId = 0)
        {
            var model = mobjContratoManager.TraerCalidadesPorContrato(contratoId, acuerdoId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerDatosDeContratoAcuerdo(int contratoId)
        {
            var model = mobjContratoManager.TraerDatosDeContratoAcuerdo(contratoId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerDatosDeContrato(int contratoId)
        {
            var model = mobjContratoManager.TraerDatosDeContrato(contratoId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerAperturaPrecioPorContrato(int contratoId = 0, string tipo = "contrato")
        {
            List<AperturaPrecioDto> model;
            if (tipo.ToUpper() != "FIJACION")
            {
                model = mobjContratoManager.TraerAperturaDePrecioPorContrato(contratoId);
            }
            else
            {
                //cambiar a fijacion manager
                model = mobjFijacionDePrecioContratoManager.TraerAperturaDePrecioPorFijacion(contratoId);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerContratoCompleto(int id, string tipo)
        {
            var copia = (tipo != "acuerdo") ? mobjContratoManager.TraerContrato(id) : mobjContratoManager.TraerContratoAcuerdoACopiar(id);
            if (copia.ContratoId == 0)
            {
                var err = new Resultado();
                err.Error("acuerdo", "Solamente se puede utilizar acuerdos con fecha de hoy o del último día hábil anterior.");
                return new JsonResult()
                {
                    Data = err,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            if (copia.Venta == true && copia.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                copia.Cantidad = Math.Abs(copia.Cantidad);

            return new JsonResult()
            {
                Data = copia,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerContratoCompletoPorContratoSAP(string contratoSAP)
        {
            contratoSAP = contratoSAP.ToString().PadLeft(10, '0');
            var ids = mobjContratoManager.TraerContratosPorSap(contratoSAP);
            if (ids.Count == 1)
            {
                BasicoContrato contrato = mobjContratoManager.TraerContrato(ids[0].Id);
                return new JsonResult()
                {
                    Data = contrato,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            return new JsonResult()
            {
                Data = "",
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerFijacionCompleto(int id)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.TraerFijacion(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult SuscripcionNotificaciones(string key)
        {
            var comercialId = GlobalVariables.ComercialId;
            return new JsonResult()
            {
                Data = mobjCompraNetManager.GrabarSuscripcion(key, comercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult UsuarioSuscripto()
        {
            var comercialId = GlobalVariables.ComercialId;
            return new JsonResult()
            {
                Data = mobjCompraNetManager.UsuarioSuscripto(comercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerContratosPendientes()
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            return new JsonResult()
            {
                Data = mobjContratoManager.TraerContratosPendientes(equipo),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerDatosCompraNet(int id)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.TraerDatosCompraNet(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual = false)
        {
            //var esVirtual = false;
            var result = esVirtual ? mobjFijacionDePrecioContratoManager.TraerDatosFijacionVirtual(cuitProveedor, cuitCorredor, materialId, filtro, fijacionId) :
                mobjFijacionDePrecioContratoManager.TraerDatosFijacion(cuitProveedor, cuitCorredor, materialId, filtro, fijacionId);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerContratoMadre(string sap)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.TraerContratoMadre(sap),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarFason(Fason oParam)
        {
            if (oParam.ComercialCreadorId.HasValue && oParam.GrupoCompra == null)
            {
                var comercial = mobjComercialManager.TraerComercial(oParam.ComercialCreadorId.Value);
                oParam.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }
            if (oParam.PorcentajeComision.HasValue)
            {
                oParam.PorcentajeComision = 0;
            }
            return new JsonResult()
            {
                Data = mobjFasonManager.GrabarFason(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAgente(AgenteCompra oParam)
        {
            if (oParam.ComercialCreadorId.HasValue && oParam.GrupoCompra == null)
            {
                var comercial = mobjComercialManager.TraerComercial(oParam.ComercialCreadorId.Value);
                oParam.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }
            if (oParam.PorcentajeComision.HasValue)
            {
                oParam.PorcentajeComision = 0;
            }
            return new JsonResult()
            {
                Data = mobjAgenteManager.GrabarAgente(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAcuerdo(ContratoAcuerdo oParam, List<CupoConDescargaFechasDto> listCupoConDescargaFechas = null)
        {
            if (oParam.Base == null) oParam.Base = false;
            if (oParam.NoInformaSio == null) oParam.NoInformaSio = false;
            if (oParam.TrigoEspecial == null) oParam.TrigoEspecial = false;
            if (oParam.EsFason == null) oParam.EsFason = false;
            if (oParam.ComercialId.HasValue)
            {
                var comercial = mobjComercialManager.TraerComercial(oParam.ComercialId.Value);
                oParam.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }
            oParam.UsuarioId = GlobalVariables.IdActiveDirectory;
            GrabarAcuerdoResult model = mobjContratoAcuerdoManager.GrabarAcuerdo(oParam, listCupoConDescargaFechas);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerFasonCompleto(int id)
        {
            return new JsonResult()
            {
                Data = mobjFasonManager.TraerFason(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerAgenteCompleto(int id)
        {
            return new JsonResult()
            {
                Data = mobjAgenteManager.TraerAgente(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerAcuerdoCompleto(int id)
        {
            return new JsonResult()
            {
                Data = mobjNegocioManager.TraerAcuerdo(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult FinalizarFason(int fasonId)
        {
            return new JsonResult()
            {
                Data = mobjFasonManager.FinalizarFason(fasonId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult FinalizarAgente(int agenteId)
        {
            return new JsonResult()
            {
                Data = mobjAgenteManager.FinalizarAgente(agenteId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult FinalizarAcuerdo(int acuerdoId)
        {
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.FinalizarAcuerdo(acuerdoId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAmpliacionFason(Fason oParam)
        {
            return new JsonResult()
            {
                Data = mobjFasonManager.GrabarAmpliacionFason(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAmpliacionAgente(AgenteCompra oParam)
        {
            return new JsonResult()
            {
                Data = mobjAgenteManager.GrabarAmpliacionAgente(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult AnularContrato(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.AnularContrato(oParam, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }
        public JsonResult ObtenerContratosParaCopiar(string filtro)
        {
            return Json(mobjContratoManager.TraerContratosPorSap(filtro), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerContratosCondicional(string filtro)
        {
            return Json(mobjContratoManager.TraerContratosCondicionalPorSap(filtro), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerContratosAcuerdo(string filtro)
        {
            return Json(mobjContratoManager.TraerContratosAcuerdo(filtro), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerContratosAcuerdoAlDia(string filtro)
        {
            var contratos = mobjContratoManager.TraerContratosAcuerdo(filtro);
            var ultimoDiaHabil = diasHabilesAgent.UltimoDiaHabil(null);
            contratos = contratos.Where(x => Convert.ToDateTime(x.Fecha) >= ultimoDiaHabil).ToList();
            return Json(contratos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarGrupoDeCompras(string filtro)
        {
            return Json(mobjComercialManager.ListarGrupoDeCompras(filtro).OrderBy(x => x.Descripcion), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarTotales(Kendo.DynamicLinq.Filter filtros)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var request = new DataSourceRequest();
            //request.Take = 0;
            //request.Skip = 0;
            //request.Sort = null;
            request.Filter = filtros;
            var model = mobjContratoManager.TraerTotalesPesosDolares(request, equipo, GlobalVariables.CorredoresComercial);

            return Json(model);
        }

        public ActionResult ValidarProveedor(int proveedorId)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ValidarProveedor(proveedorId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerPrecioMoa(int? tipoNegocioId)
        {
            var json = new JsonResult()
            {
                Data = configuracionInternaManager.TraerPrecioCompraNet(tipoNegocioId),
                MaxJsonLength = Int32.MaxValue,
            };
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        public ActionResult TraerPrecioMOAPorMaterial(int materialId)
        {
            var precio = configuracionInternaManager.TraerPrecioCompraNet(materialId);

            return new JsonResult()
            {
                Data = precio,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult HabilitarPizarra(int material)
        {
            return new JsonResult()
            {
                Data = configuracionInternaManager.HabilitarPizarra(material),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult AprobarFijacion(int id)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.AprobarFijacion(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerRangoDePrecios(int materialId, string monedaId)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ObtenerRangoDePrecios(materialId, monedaId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult NoMostrarEnTablero(Negocio negocioAnular)
        {
            return new JsonResult()
            {
                Data = mobjNegocioManager.OcultarEnTablero(negocioAnular),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult TraerTipoDeCambio(DateTime? fechaOperacion, string typeOfRate = "M")
        {
            if (fechaOperacion == null || fechaOperacion == DateTime.Now.Date)
            {
                fechaOperacion = DateTime.Now.Date;
            }
            var precioDolar = tipoDeCambioAgent.TraerTipoDeCambioUltimoDiaHabil(fechaOperacion.Value, typeOfRate);
            return Json(precioDolar, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReconfirmarFinalizado(int contratoId)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ReconfirmarFinalizado(contratoId, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult PreAnularContrato(int contratoId, string motivo)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.PreAnularContrato(contratoId, motivo),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult AnularContratoPreAnulado(int contratoId)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.AnularContratoPreAnulado(contratoId, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult RechazarPreAnularContrato(int contratoId/*, string motivoRechazo*/)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.RechazarPreAnularContrato(contratoId/*, motivoRechazo*/),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult CompararNegocioReconfirmado(int contratoId)
        {
            var contratos = mobjContratoManager.CompararNegocioReconfirmado(contratoId);
            return new JsonResult()
            {
                Data = contratos,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ValidarCalidades(int contratoId)
        {
            var validarCalidad = mobjContratoManager.DiferenciaEnCalidades(contratoId);
            return new JsonResult()
            {
                Data = validarCalidad,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerListaDeCbu(string cuitProveedor, string filtro)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ListarCBU(cuitProveedor, filtro),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult FechaFeriados()
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.FechaFeriados(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult UltimoDiaHabil()
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.UltimoDiaHabil(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult AprobarContrato(int id)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.AprobarContrato(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BorrarContratoPreAprobacion(int id, string motivoRechazo)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.BorrarContratoPreAprobacion(id, motivoRechazo),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerProyeccion(BasicoContrato basico)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.UltimoDiaHabil(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ValidarCredito(string cuit, double cantidad, decimal precio, string moneda, string typeOfRate = "M")
        {
            var val = mobjContratoManager.ValidarCredito(cuit, cantidad, precio, moneda, typeOfRate);
            return new JsonResult()
            {
                Data = String.IsNullOrEmpty(val) ? "" : val,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarCartasDePortePendienteAplicar(CcPpPendienteAplicarDto req)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ListarCartasDePortePendienteAplicar(req),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerPagosDiferido(int cantidadDia)
        {
            var result = configuracionInternaManager.TraerPagosDiferido().Where(x => x.CantidadDia >= cantidadDia).OrderBy(x => x.CantidadDia).FirstOrDefault();
            return new JsonResult()
            {
                Data = result ?? new HabilitacionPagoDiferidoDto(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult ObtenerCapacidadProductivaPendiente(int proveedorId)
        {
            var lista = mobjContratoManager.ObtenerCapacidadProductivaPendiente(proveedorId);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PreAnularFijacionVirtual(int fijacionId, string motivo)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.PreAnularFijacion(fijacionId, motivo),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult AnularFijacionVirtual(int fijacionId)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.AnularFijacion(fijacionId, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult RechazarPreAnularFijacionVirtual(int fijacionId/*, string motivoRechazo*/)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.RechazarPreAnularFijacionVirtual(fijacionId/*, motivoRechazo*/),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult DevolverKilosPendientesAnularFijacionCanje(int id)
        {
            ResultadoDevolverKilosPendientesAnularFijacionCanjeDto kilos = mobjFijacionDePrecioContratoManager.DevolverKilosPendientesAnularFijacionCanje(id);
            return Json(kilos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidacionesAnulaYReemplaza(string contratoSap)
        {
            var tieneFijaciones = mobjContratoManager.ValidacionesAnulaYReemplaza(contratoSap);
            return Json(tieneFijaciones, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DevolverContratosParaAsociar(int contratoId, string numero)
        {
            var result = mobjContratoManager.DevolverContratosParaAsociar(contratoId, numero);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult DevolverContratoAsociadosPase(int contratoId)
        {
            var result = mobjContratoManager.DevolverContratoAsociadosPase(contratoId);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult CalcularImporteDeOperacion(decimal precio, double cantidad, int materialId, DateTime fechaOperacion, string monedaId, string typeOfRate = "M")
        {
            var result = mobjContratoManager.CalcularImporteDeOperacion(precio, cantidad, materialId, fechaOperacion, monedaId, typeOfRate);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ActualizarNegociosAsociados(List<NegocioAsociadoDto> negocio, int contratoId, decimal precioPonderado)
        {
            var result = mobjContratoManager.GrabarNegociosAsociados(negocio, contratoId, precioPonderado);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult DevolverSiTieneAsociados(int contratoId)
        {
            var result = mobjContratoManager.TieneAsociados(contratoId);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult EsUnContratoAsociado(int negocioId)
        {
            var result = mobjContratoManager.EsUnContratoAsociado(negocioId);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult EstaConfirmadoEnSAP(string contratoSAP, int TipoNegocioId)
        {
            var estado = mobjContratoManager.EstaConfirmadoEnSAP(contratoSAP, TipoNegocioId);
            return Json(estado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidarProveedorSISA(int proveedorId, int clasificacion, bool planCanje = false, bool consignatario = false)
        {
            var result = mobjContratoManager.ValidarProveedor(proveedorId, clasificacion, planCanje, consignatario);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult ValidarCopiarContrato(int id)
        {
            var estado = mobjContratoManager.ValidarCopiarContrato(id);
            return Json(estado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidarComisionEnCentro(int id)
        {
            var estado = centroManager.TraerCentro(id);
            return Json(estado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Tiene2doCondicionalAsociado(int contratoId)
        {
            bool estado = mobjContratoManager.Tiene2doCondicionalAsociado(contratoId);
            return Json(estado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerDatosMercaderiaEnDeposito(int? materialId, int? id, int? centro, int? corredorId, int? proveedorId, bool tieneSustentable, bool? sinBoleto)
        {
            var model = mobjContratoManager.ObtenerDatosMercaderiaEnDeposito(materialId, id, centro, corredorId, proveedorId, tieneSustentable, sinBoleto);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocios, PermisosDataAgro.ModificarNegocios, PermisosDataAgro.ModificarNegFinalizados, PermisosDataAgro.ModificarCanje, PermisosDataAgro.ModificarDolarizadoExpress, PermisosDataAgro.ModificarDolarizadoFinalizado)]
        public ActionResult AltaMasivaContratos()
        {
            var tipoAlta = new List<SelectListItem>();
            tipoAlta.Add(new SelectListItem
            {
                Text = "Seleccione",
                Value = "0",
                Selected = true
            });
            tipoAlta.Add(new SelectListItem
            {
                Text = "Acuerdo",
                Value = "1",
                Selected = false
            });
            tipoAlta.Add(new SelectListItem
            {
                Text = "Convenio - A Fijar",
                Value = "2",
                Selected = false
            });
            tipoAlta.Add(new SelectListItem
            {
                Text = "MATBA",
                Value = "3",
                Selected = false
            });
            ViewBag.TipoAlta = tipoAlta;
            return View();
        }

        [HttpPost]
        public ActionResult AltaMasivaContratosExcel(string tipoAlta, string contratoAcuerdo = null)
        {
            List<string> errores = new List<string>();
            try
            {

                if (Request.Files.Count == 0)
                {
                    errores.Add(string.Concat("Debe seleccionar el archivo."));
                    return Json(new { Resume = errores, Resultado = false });
                }
                if (Request.Files.Count > 1)
                {
                    errores.Add(string.Concat("Debe seleccionar un solo archivo."));
                    return Json(new { Resume = errores, Resultado = false });
                }

                var fileSubido = Request.Files[0];
                var extension = Path.GetExtension(fileSubido.FileName).ToUpper();
                if (extension != ".XLSX")
                {
                    errores.Add(string.Concat("Archivo no soportado. Debe subir un Excel en formato xlsx."));
                    return Json(new { Resume = errores, Resultado = false });
                }

                if (fileSubido.ContentLength > 0)
                {
                    var dsExcel = ExcelImport.LeerExcelDesdeHttpRequest(Request);
                    if (tipoAlta == "1")
                    {
                        var resultado = mobjContratoManager.AltaMasivaContratos(dsExcel, contratoAcuerdo, GlobalVariables.ComercialId);
                        return Json(new { Resume = resultado, Resultado = true });
                    }
                    if (tipoAlta == "2")
                    {
                        var resultado = mobjContratoManager.AltaMasivaConvenios(dsExcel, GlobalVariables.ComercialId);
                        return Json(new { Resume = resultado, Resultado = true });
                    }
                    if (tipoAlta == "3")
                    {
                        var resultado = mobjContratoManager.AltaMasivaMATBA(dsExcel, GlobalVariables.ComercialId);
                        return Json(new { Resume = resultado, Resultado = true });
                    }
                }
                else
                {
                    errores.Add(string.Concat("El archivo ", fileSubido.FileName, " está vacío."));
                }


                if (errores.Count > 0)
                {
                    return Json(new { Resume = errores, Resultado = false });
                }

                return Json(new { data = "" });

            }
            catch (Exception e)
            {
                errores.Add(e.Message);
                return Json(new { Resume = errores, Resultado = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReporteLocalidades()
        {
            var model = reportesManager.ObtenerDatosReporteLocalidades();
            return File(ExcelReporteCompleto.GenerarExcelLocalidades(model), "application/vnd.ms-excel");
        }

        public JsonResult TraerServicios(int? materialId, int? centroId)
        {
            var model = mobjContratoManager.TraerTodoServicio(materialId, centroId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        static readonly object _lockReenviarMailContrato = new object();
        public ActionResult ReenviarMailContrato(int contratoId)
        {
            lock (_lockReenviarMailContrato)
            {
                mobjContratoManager.ReenviarMailContrato(contratoId, GlobalVariables.IdActiveDirectory);
                return new JsonResult()
                {
                    Data = "Reenvio de email exitoso.",
                    MaxJsonLength = Int32.MaxValue
                };
            }
        }

        static readonly object _lockEnviarMailFijacion = new object();
        public ActionResult EnviarMailFijacion(int fijacionDePrecioContratoId)
        {
            lock (_lockEnviarMailFijacion)
            {
                mobjFijacionDePrecioContratoManager.EnviarMailFijacion(fijacionDePrecioContratoId, GlobalVariables.IdActiveDirectory);
                return new JsonResult()
                {
                    Data = "Reenvio de email de fijacion exitoso.",
                    MaxJsonLength = Int32.MaxValue
                };
            }
        }

        public ActionResult ControlesAccesoConDescarga(Negocio oParam)
        {
            oParam.UsuarioId = GlobalVariables.IdActiveDirectory;

            Resultado model = mobjNegocioManager.ControlesAccesoConDescarga(oParam);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ValidarPantallaEnUso(PantallaEnUsoDto pantallaEnUso)
        {
            pantallaEnUso.UsuarioId = GlobalVariables.IdActiveDirectory;
            Resultado model;

            if (pantallaEnUso.usar)
            {
                model = mobjContratoManager.ValidarPantallaEnUso(pantallaEnUso);
            }
            else
            {
                model = mobjContratoManager.LiberarPantalla(pantallaEnUso);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult CantidadDiasCuposConDescarga(string fechaDesdeNegocio, string fechaHastaNegocio, int materialId, int centroId, int comercialId)
        {

            List<ConfiguracionCupoDto> configCupo = mobjContratoManager.CantidadDiasCuposConDescarga(fechaDesdeNegocio, fechaHastaNegocio, materialId, centroId, comercialId);

            return new JsonResult()
            {
                Data = configCupo,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerCuposConDescarga(int contratoId)
        {
            var cuposDelNegocio = mobjContratoManager.TraerCuposConDescarga(contratoId);
            return new JsonResult()
            {
                Data = cuposDelNegocio,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerTypeOfRate(int tipoNegocioId, string monedaId, int? tipoAgenteCompraId, DateTime fecha, bool modifica)
        {
            var result = mobjContratoManager.ObtenerTypeOfRate(tipoNegocioId, monedaId, tipoAgenteCompraId, fecha, modifica);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ConsultaRangoPrecio(int materialId, string moneda, decimal precio)
        {

            return new JsonResult()
            {
                Data = mobjContratoManager.ConsultarRangoPrecio(materialId, moneda, precio),
                MaxJsonLength = Int32.MaxValue
            };
        }

        static readonly object _lockEmailImpuestos = new object();
        public ActionResult EnviarMailImpuestos(int contratoId)
        {
            lock (_lockFinalizarContrato)
            {
                mobjContratoManager.EnviarMailImpuestos(contratoId);
                return new JsonResult()
                {
                    Data = new GrabarContratoResult(),
                    MaxJsonLength = Int32.MaxValue
                };
            }
        }

        public ActionResult ValidarCapacidadProductiva(Contrato negocio)
        {
            var result = mobjContratoManager.ValidarCapacidadProductiva(negocio);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
