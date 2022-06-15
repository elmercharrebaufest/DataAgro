using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;
using System.IO;
using Molinos.DataAgro.Business.Helpers;
using System.Data;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class CompraNetController : Controller
    {
        private IHomeManager mobjHomeManager;

        private ICompraNetManager mobjCompraNetManager;

        private IContratoManager mobjContratoManager;

        private IFijacionDePrecioContratoManager mobjFijacionDePrecioContratoManager;

        private IComercialManager mobjComercialManager;

        private ICampañaManager mobjCampañaManager;

        private ILocalidadManager mobjLocalidadManager;

        private IMaterialManager mobjMaterialManager;

        private IProveedorManager mobjProveedorManager;

        private IOperadorManager mobjOperadorManager;

        private ILogger mobjLogger;

        private INegocioManager mobjNegocioManager;

        private readonly IFasonManager mobjFasonManager;
        private readonly IAgenteCompraManager mobjAgenteManager;
        private readonly IContratoAcuerdoManager mobjContratoAcuerdoManager;
        private readonly IConfiguracionInternaManager configuracionInternaManager;
        private IConfiguracionManager mobjConfiguracionManager;
        private ITipoDeCambioAgent tipoDeCambioAgent;
        private ICentroManager centroManager;


        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public CompraNetController(IHomeManager oHomeManager, ILocalidadManager ojLocalidadManager,
            IProveedorManager oProveedorManager, IMaterialManager oMaterialManager,
            IContratoManager oContratoManager, IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager,
            ICompraNetManager oCompraNetManager, IComercialManager oComercialManager, ICampañaManager oCampañaManager,
            ILogger oLogger, IFasonManager oFasonManager, IAgenteCompraManager oAgenteManager, IContratoAcuerdoManager oContratoAcuerdoManager,
            IConfiguracionInternaManager configuracionInternaManager, IConfiguracionManager configuracionManager,
            IOperadorManager oOperadorManager, INegocioManager oNegocioManager,
            ITipoDeCambioAgent tipoDeCambioAgent, ICentroManager centroManager)
        {
            mobjHomeManager = oHomeManager;
            mobjComercialManager = oComercialManager;
            mobjCompraNetManager = oCompraNetManager;
            mobjContratoManager = oContratoManager;
            mobjFijacionDePrecioContratoManager = oFijacionDePrecioContratoManager;
            mobjCampañaManager = oCampañaManager;
            mobjMaterialManager = oMaterialManager;
            mobjProveedorManager = oProveedorManager;
            mobjOperadorManager = oOperadorManager;
            mobjLocalidadManager = ojLocalidadManager;
            mobjLogger = oLogger;
            mobjFasonManager = oFasonManager;
            mobjAgenteManager = oAgenteManager;
            mobjContratoAcuerdoManager = oContratoAcuerdoManager;
            mobjConfiguracionManager = configuracionManager;
            mobjNegocioManager = oNegocioManager;
            this.configuracionInternaManager = configuracionInternaManager;
            this.tipoDeCambioAgent = tipoDeCambioAgent;
            this.centroManager = centroManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

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
            ViewBag.ComercialId = GlobalVariables.ComercialId;
            ViewBag.Id = id;
            ViewBag.TipoId = tipoId;
            ViewBag.Contrato = mobjContratoManager.ObtenerSapContrato(id.HasValue ? id.Value : 0);
            ViewBag.Fijacion = mobjContratoManager.ObtenerSapFijacion(id.HasValue ? id.Value : 0);
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
                return View(tipoNegocio.TipoNegocioId == 1 || tipoNegocio.TipoNegocioId == 3 || tipoNegocio.TipoNegocioId == 6 ? tipoNegocio.Descripcion.Replace(" ", String.Empty) : "CrearContrato");
            }
            if (id > 0 || (PermisosHelper.Is(PermisosDataAgro.ModificarCanje)))
            {
                tipoId = PermisosHelper.Is(PermisosDataAgro.ModificarCanje) && (id == null || id == 0) ? 1 : tipoId.HasValue ? tipoId.Value : 2;
                var tipoNegocio = mobjContratoManager.DevolverNamespaceNegocio(tipoId.Value);
                return View(tipoNegocio.TipoNegocioId == 1 || tipoNegocio.TipoNegocioId == 3 || tipoNegocio.TipoNegocioId == 6 ? tipoNegocio.Descripcion.Replace(" ", String.Empty) : "CrearContrato");
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

        public ActionResult GrabarContrato(Contrato oParam)
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
            if (oParam.EstadoId == 5 || oParam.EstadoId == 11)
            {
                model = mobjContratoManager.ActualizarContratoFinalizado(oParam);
            }
            else
            {
                model = mobjContratoManager.GrabarContrato(oParam);
            }
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
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
                err.Error("acuerdo", "Solamente se puede utilizar acuerdos con fecha de hoy o del último día hábil anterior");
                return new JsonResult()
                {
                    Data = err,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            return new JsonResult()
            {
                Data = copia,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerContratoCompletoPorContratoSAP(string contratoSAP)
        {
            BasicoContrato contrato = null;
            contratoSAP = contratoSAP.ToString().PadLeft(10, '0');
            var ids = mobjContratoManager.TraerContratosPorSap(contratoSAP);
            if (ids.Count == 1)
            {
                contrato = mobjContratoManager.TraerContrato(ids[0].Id);
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
            return new JsonResult()
            {
                Data = mobjAgenteManager.GrabarAgente(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAcuerdo(ContratoAcuerdo oParam)
        {
            if (oParam.ComercialCreadorId.HasValue && oParam.GrupoCompra == null)
            {
                var comercial = mobjComercialManager.TraerComercial(oParam.ComercialCreadorId.Value);
                oParam.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.GrabarAcuerdo(oParam),
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
                Data = mobjContratoAcuerdoManager.TraerAcuerdo(id),
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
            contratos = contratos.Where(x=> Convert.ToDateTime(x.Fecha)>= DateTime.Now.Date.AddDays(-1)).ToList();
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

        public JsonResult TraerTipoDeCambio()
        {
            var precioDolar = tipoDeCambioAgent.TraerTipoDeCambio(DateTime.Now.AddDays(-1).Date);
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
        public ActionResult ValidarCredito(string cuit, double cantidad, decimal precio, string moneda)
        {
            var val = mobjContratoManager.ValidarCredito(cuit, cantidad, precio, moneda);
            return new JsonResult()
            {
                Data = String.IsNullOrEmpty(val) ? "" : val,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult ListarCartasDePortePendienteAplicar(CcPpPerndienteAplicarDto req)
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
                Data = result != null ? result : new HabilitacionPagoDiferidoDto(),
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
                Data = mobjFijacionDePrecioContratoManager.AnularFijacionVirtual(fijacionId, GlobalVariables.IdActiveDirectory),
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

        public ActionResult CalcularImporteDeOperacion(decimal precio, double cantidad, int materialId, DateTime fechaOperacion, string monedaId)
        {
            var result = mobjContratoManager.CalcularImporteDeOperacion(precio, cantidad, materialId, fechaOperacion, monedaId);
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

        public JsonResult ObtenerDatosMercaderiaEnDeposito(int? materialId, int? id, int? centro, int? corredorId, int? proveedorId, bool? tieneSustentable, bool? tieneBoleto)
        {
            var model = mobjContratoManager.ObtenerDatosMercaderiaEnDeposito(materialId, id, centro, corredorId, proveedorId, tieneSustentable, tieneBoleto);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocios, PermisosDataAgro.ModificarNegocios, PermisosDataAgro.ModificarNegFinalizados, PermisosDataAgro.ModificarCanje, PermisosDataAgro.ModificarDolarizadoExpress, PermisosDataAgro.ModificarDolarizadoFinalizado)]
        public ActionResult AltaMasivaContratos()
        {            
            return View();
        }

        [HttpPost]
        public ActionResult AltaMasivaContratosExcel(string contratoAcuerdo)//(System.Web.HttpPostedFileBase file)
        {
            List<string> errores = new List<string>();
            try
            {

                //var proveedor = ObtenerProveedor();


                //var contratoAcuerdo = Request.Form.Get("contratoId");

                int ncontratoAcuerdo;
                if (!int.TryParse(contratoAcuerdo, out ncontratoAcuerdo))
                {
                    errores.Add(string.Concat("Debe seleccionar el contrato acuerdo."));
                    return Json(new { Resume = errores, Resultado = false });
                }

                BasicoContrato acuerdo = mobjContratoAcuerdoManager.TraerAcuerdo(ncontratoAcuerdo);
                if (acuerdo.Id == 0)
                {
                    errores.Add(string.Concat("El Acuerdo seleccionado no es valido."));
                    return Json(new { Resume = errores, Resultado = false });
                }
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
                    if (dsExcel.Tables.Count == 0)
                    {
                        errores.Add(string.Concat("Archivo no contiene información."));
                        return Json(new { Resume = errores, Resultado = false });
                    }
                    if (dsExcel.Tables[0].Rows.Count == 0)
                    {
                        errores.Add(string.Concat("Archivo no contiene información."));
                        return Json(new { Resume = errores, Resultado = false });
                    }
                    if (dsExcel.Tables[0].TableName != "AltaMasiva")
                    {
                        if (dsExcel.Tables[0].TableName == "Data")
                        {
                            throw new Exception("El documento no contiene información de contratos.");
                        }
                        else
                        {
                            throw new Exception("El documento no tiene el formato correcto. Utilice el Archivo Modelo");
                        }
                    }

                    var materiales = mobjMaterialManager.TraerTodoMaterial();
                    var centros = centroManager.TraerTodoCentro();
                    var campanias = mobjCampañaManager.TraerTodoCampania();
                    var validations = GetValidatorContratos(materiales.Material, centros.Centro);
                    var validator = new ExcelValidator(validations);

                    var resultValidation = validator.Validate(dsExcel.Tables[0], false);

                    if (!resultValidation.IsValid)
                    {
                        return Json(new { Resume = resultValidation.Resume, Resultado = !resultValidation.IsValid }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        List<BasicoContrato> contratos = new List<BasicoContrato>();
                        int tiponegocioid = acuerdo.Precio > 0 ? 2 : 1;
                        List<int> rowsOk = resultValidation.RowsResult.Where(a => a.HasError).Select(a => a.Row).ToList();
                        if (rowsOk.Count == 0)
                        {
                            return Json(new { Resume = resultValidation.Resume, Resultado = resultValidation.IsValid }, JsonRequestBehavior.AllowGet);
                        }
                        var rows = dsExcel.Tables[0].AsEnumerable().Select(x => x.ItemArray).Skip(0);
                        for (int ii = 0; ii < rows.Count(); ii++)
                        {
                            if (!rowsOk.Contains(ii))
                                continue;
                            var contrato = new BasicoContrato();
                            contrato.ContratoAcuerdoId = acuerdo.Id;
                            contrato.CorredorId = acuerdo.CorredorId;

                            contrato.ContratoCorredor = rows.ElementAt(ii)[0].ToString().Trim();
                            contrato.ContratoVendedor = rows.ElementAt(ii)[1].ToString().Trim();
                            contrato.MaterialId = materiales.Material.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[2].ToString().Trim().ToLower()).Single().MaterialId;
                            contrato.CampanaId = campanias.Where(a => a.Descripcion.Replace("-", "").ToLower() == rows.ElementAt(ii)[3].ToString().Trim().ToLower()).Single().CampañaId;
                            //contrato.Fecha = DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim());
                            contrato.FechaOperacion = DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim());
                            contrato.FechaDesde = DateTime.Parse(rows.ElementAt(ii)[5].ToString().Trim());
                            contrato.FechaHasta = DateTime.Parse(rows.ElementAt(ii)[6].ToString().Trim());
                            contrato.FechaEntrega = DateTime.Parse(rows.ElementAt(ii)[6].ToString().Trim());
                            contrato.Cantidad = int.Parse(rows.ElementAt(ii)[7].ToString().Trim()) * 1000;
                            contrato.Cuit = rows.ElementAt(ii)[8].ToString().Trim();
                            contrato.ClasificacionId = rows.ElementAt(ii)[9].ToString().Trim().ToLower() == "productor" ? 1 : rows.ElementAt(ii)[9].ToString().Trim().ToLower() == "acopiador" ? 2 : 3;
                            contrato.PlanCanje = rows.ElementAt(ii)[10].ToString().Trim().ToUpper() == "X";
                            contrato.Consignatario = rows.ElementAt(ii)[11].ToString().Trim().ToUpper() == "X";
                            contrato.DestinoId = centros.Centro.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[12].ToString().Trim().ToLower()).Single().Id;
                            contrato.LocalidadId = int.Parse(rows.ElementAt(ii)[13].ToString().Trim());
                            contrato.ProvinciaId = int.Parse(rows.ElementAt(ii)[14].ToString().Trim());
                            contrato.Observacion = ii.ToString().Trim();
                            contrato.ComercialCreadorId = GlobalVariables.ComercialId;
                            //contrato.UsuarioTercero = GlobalVariables.ComercialId;
                            contratos.Add(contrato);

                        }

                        validacionContratoFatal(contratos, acuerdo, resultValidation);
                        if (!resultValidation.IsValid)
                        {
                            List<ExcelValidatorResumeItem> erroresList = new List<ExcelValidatorResumeItem>();
                            foreach(var item in resultValidation.RowsResult )
                            {
                                List<string> errorsList = new List<string>();
                                ExcelValidatorResumeItem erroresItem = new ExcelValidatorResumeItem();
                                erroresItem.ContratoCorredor = item.ContratoCorredor;
                                erroresItem.Row = item.Row;

                                foreach (var item2 in item.ItemsResult) {
                                    errorsList.AddRange(item2.Errors);                                    
                                }
                                erroresItem.Errors = errorsList;
                                
                                erroresList.Add(erroresItem);
                            }
                            return Json(new { Resume = erroresList, Resultado = true}, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            List<GrabarContratoResult> resultados = mobjContratoManager.GrabarContratoMasivo(contratos);

                            foreach (var item in resultados)
                            {
                                if (item.HayError)
                                {
                                    //item.ContratoId estoy usando ese campo para devolver el numero de row
                                    resultValidation.RowsResult[item.ContratoId ?? 0].ItemsResult.Add(new ExcelValidatorItemResult { Errors = item.Errores.Select(a => a.Message).ToList(), Item = new ExcelValidatorItem { ErrorType = ExcelValidationErrorType.Error, Name = "", Options = null, Position = 1, Required = true, Type = ExcelValidationColumnType.String } });
                                    //errores.AddRange(item.Errores.Select(a => a.Message).ToList());
                                }

                            }
                            //resultValidation.Resume.OrderBy(x => x.HasError);
                            return Json(new { Resume = resultValidation.Resume.OrderBy(x => x.HasError).ToList(), Resultado = resultValidation.IsValid }, JsonRequestBehavior.AllowGet);

                        }
                    }


                }
                else
                {
                    errores.Add(string.Concat("El archivo ", fileSubido.FileName, " está vacío."));
                }


                if (errores.Count > 0)
                {
                    return Json(new { info = errores });
                }

                return Json(new { data = "" });

            }
            catch (Exception e)
            {
                //Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                errores.Add(e.Message);
                return Json(new { Resume = errores, Resultado = false }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { info = "" });
        }

        private void validacionContratoFatal(List<BasicoContrato> contratos, BasicoContrato acuerdo, ExcelValidatorResult resultValidation)
        {
            int i = 0;
            foreach (var item in contratos)
            {
                List<ExcelValidatorItemResult> excelValidatorItemResults = new List<ExcelValidatorItemResult>();
                string indiceContrato = "Contrato corredor: " + item.ContratoCorredor + ". ";
                if (item.MaterialId != acuerdo.MaterialId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Grano", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "El material no concuerda con el del acuerdo seleccionado. " } });
                }
                if (item.CampanaId != acuerdo.CampanaId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Cosecha", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "La cosecha no concuerda con el del acuerdo seleccionado. " } });
                }
                if (item.FechaOperacion.Value.Date != acuerdo.FechaOperacion.Value.Date)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Operación", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "La Fecha Operación no concuerda con el del acuerdo seleccionado. " } });
                }
                if (acuerdo.FechaDesde.HasValue)
                {
                    if (item.FechaDesde.Value.Date != acuerdo.FechaDesde.Value.Date)
                    {
                        excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha DesdeEntrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "La Fecha Desde Entrega no concuerda con el del acuerdo seleccionado. " } });
                    }
                }
                else
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Vto. Entrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "El acuerdo no cuenta con Fecha Desde Entrega. " } });
                }
                if (acuerdo.FechaHasta.HasValue)
                {
                    if (item.FechaEntrega.Value.Date != acuerdo.FechaHasta.Value.Date)
                    {
                        excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Vto. Entrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "La Fecha Vto. Entrega no concuerda con el del acuerdo seleccionado. " } });
                    }
                }
                else
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Vto. Entrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "El acuerdo no cuenta con Fecha Vto. Entrega. " } });
                }
                if (item.DestinoId != acuerdo.DestinoId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Destino", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "El Destino no concuerda con el del acuerdo seleccionado. " } });
                }
                if (excelValidatorItemResults.Count > 0)
                {
                    resultValidation.RowsResult[i].ItemsResult.AddRange(excelValidatorItemResults);
                }
                //resultValidation.RowsResult[i].ContratoCorredor = item.ContratoCorredor;
                i++;
            }
        }

        private List<ExcelValidatorItem> GetValidatorContratos(List<MaterialIni> materiales, List<CentroIni> centros)
        {
            var ret = new List<ExcelValidatorItem>();
            var pos = 0;

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Corredor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Grano",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = materiales.Where(a => a.MaterialId < 5).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Cosecha",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Operación",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha DesdeEntrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Vto.Entrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "TN",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Decimal
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "CUIT Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Clasificacion",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Options = new List<string>() { "acopiador", "productor", "otros" },
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Plan Canje",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Consignatario",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Destino",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = centros.Where(a => a.Id != 10).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROCEDENCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROVINCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            //configurar el resto de campos

            return ret;
        }
    }
}