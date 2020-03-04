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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

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

        private ILogger mobjLogger;

        private readonly IFasonManager mobjFasonManager;
        private readonly IAgenteCompraManager mobjAgenteManager;
        private readonly IContratoAcuerdoManager mobjContratoAcuerdoManager;
        private readonly IConfiguracionInternaManager configuracionInternaManager;
        private IConfiguracionManager mobjConfiguracionManager;


        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public CompraNetController(IHomeManager oHomeManager, ILocalidadManager ojLocalidadManager,
            IProveedorManager oProveedorManager, IMaterialManager oMaterialManager,
            IContratoManager oContratoManager, IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager,
            ICompraNetManager oCompraNetManager, IComercialManager oComercialManager, ICampañaManager oCampañaManager,
            ILogger oLogger, IFasonManager oFasonManager, IAgenteCompraManager oAgenteManager, IContratoAcuerdoManager oContratoAcuerdoManager,
            IConfiguracionInternaManager configuracionInternaManager, IConfiguracionManager configuracionManager)
        {
            mobjHomeManager = oHomeManager;
            mobjComercialManager = oComercialManager;
            mobjCompraNetManager = oCompraNetManager;
            mobjContratoManager = oContratoManager;
            mobjFijacionDePrecioContratoManager = oFijacionDePrecioContratoManager;
            mobjCampañaManager = oCampañaManager;
            mobjMaterialManager = oMaterialManager;
            mobjProveedorManager = oProveedorManager;
            mobjLocalidadManager = ojLocalidadManager;
            mobjLogger = oLogger;
            mobjFasonManager = oFasonManager;
            mobjAgenteManager = oAgenteManager;
            mobjContratoAcuerdoManager = oContratoAcuerdoManager;
            mobjConfiguracionManager = configuracionManager;
            this.configuracionInternaManager = configuracionInternaManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        [Autorizacion(PermisosDataAgro.VisualizarCompraNet)]
        public ActionResult Index()
        {
            ViewBag.TieneEmpleadosACargo = GlobalVariables.TieneEmpleadosACargo;
            ViewBag.comercialId = GlobalVariables.ComercialId;
            return View();
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocios, PermisosDataAgro.NuevoNegocioExterno, PermisosDataAgro.ModificarNegocios, PermisosDataAgro.ModificarNegFinalizados)]
        public ActionResult CrearContrato(int? id, int? tipoId, string siguientes)
        {
            if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                return RedirectToAction("CrearContratoExterno");
            }
            ViewBag.ComercialId = GlobalVariables.ComercialId;
            ViewBag.Id = id;
            ViewBag.TipoId = tipoId;
            ViewBag.Siguientes = siguientes;
            ViewBag.ContratoAperturaPrecioPorcentajeDeComisionMaximo = mobjConfiguracionManager.TraerConfiguraciones().ContratoAperturaPrecioPorcentajeDeComisionMaximo;
            return View();
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

        public ActionResult InicializarContrato()
        {
            return new JsonResult()
            {
                Data = new ContratoModel_prueba
                {
                    Datos = mobjContratoManager.TraerDatosCombo()
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

            if (oParam.ComercialId.HasValue)
            {
                var comercial = mobjComercialManager.TraerComercial(oParam.ComercialId.Value);
                oParam.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }

            oParam.UsuarioId = GlobalVariables.IdActiveDirectory;
            GrabarContratoResult model;
            if (oParam.EstadoId == 5)
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

        public ActionResult FinalizarContrato(int contratoId)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.FinalizarContrato(contratoId, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }

        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarContrato(int contratoId)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ConfirmarContrato(contratoId),
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
                Data = mobjFijacionDePrecioContratoManager.ConfirmarFijacion(fijacionDePrecioContratoId),
                MaxJsonLength = Int32.MaxValue
            };
        }
        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarAcuerdo(int fijacionDePrecioContratoId)
        {
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.ConfirmarContratoAcuerdo(fijacionDePrecioContratoId),
                MaxJsonLength = Int32.MaxValue
            };
        }
        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarAgente(int fijacionDePrecioContratoId)
        {
            return new JsonResult()
            {
                Data = mobjAgenteManager.ConfirmarAgenteCompra(fijacionDePrecioContratoId),
                MaxJsonLength = Int32.MaxValue
            };
        }
        [Autorizacion(PermisosDataAgro.ConfirmarNegocioCorredoresBsAs, PermisosDataAgro.ConfirmarNegocioCorredoresRosario, PermisosDataAgro.ConfirmarNegocioOrigCentro, PermisosDataAgro.ConfirmarNegocioOrigNorte, PermisosDataAgro.ConfirmarNegocioOrigSur)]
        public ActionResult ConfirmarFason(int fijacionDePrecioContratoId)
        {
            return new JsonResult()
            {
                Data = mobjFasonManager.ConfirmarFason(fijacionDePrecioContratoId),
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult FinalizarFijacion(int fijacionDePrecioContratoId)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.FinalizarFijacion(fijacionDePrecioContratoId, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
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
            if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                oParam.ComercialId = mobjComercialManager.ComercialAsociado(oParam.CorredorId.HasValue && oParam.CorredorId != 0 ? oParam.CorredorId.Value : oParam.ProveedorId ?? 0);
                oParam.UsuarioId = PermisosHelper.ObtenerUsuario();
            }
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.GrabarFijacionDePrecio(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest request)
        {
            if (request.Sort != null)
            {
                request.Sort = request.Sort.Concat(new[] { new Sort {Field= "Estado_Order", Dir= "asc" } });
            }
            else
            {
                request.Sort = new List<Sort> { new Sort { Field = "Estado_Order", Dir = "asc" } };
            }
            request.Sort = request.Sort.Concat(new[] { new Sort { Field = "Fecha_Order", Dir = "desc" } });
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
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
            return mobjProveedorManager.TraerProveedorPorCuit(Cuit, corredor).ProveedorId;
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
            return Json(proveedores.Select(x => new { x.ProveedorId, Proveedor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
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
        public ActionResult ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.TraerDatosFijacion(cuitProveedor, cuitCorredor, materialId, filtro, fijacionId),
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
            return new JsonResult()
            {
                Data = mobjFasonManager.GrabarFason(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult GrabarAgente(AgenteCompra oParam)
        {
            return new JsonResult()
            {
                Data = mobjAgenteManager.GrabarAgente(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAcuerdo(ContratoAcuerdo oParam)
        {
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

        public JsonResult ObtenerContratosAcuerdo(string filtro)
        {
            return Json(mobjContratoManager.TraerContratosAcuerdo(filtro), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarGrupoDeCompras(string filtro)
        {
            return Json(mobjComercialManager.ListarGrupoDeCompras(filtro).OrderBy(x => x.Descripcion), JsonRequestBehavior.AllowGet);
        }
        public ActionResult BuscarTotales(Kendo.DynamicLinq.Filter filtros)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var request = new DataSourceRequest();
            request.Take = 0;
            request.Skip = 0;
            request.Sort = null;
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
        public ActionResult TraerPrecioMoa()
        {
            return new JsonResult()
            {
                Data = configuracionInternaManager.TraerPrecioCompraNet(),
                MaxJsonLength = Int32.MaxValue
            };
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
    }
}