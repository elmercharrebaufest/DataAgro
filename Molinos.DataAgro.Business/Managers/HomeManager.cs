using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class HomeManager : IHomeManager
    {
        private readonly IRepositorio repositorio;
        private ICampañaManager mobCampaña;
        private readonly IObjetivoManager objetivoManager;
        private ILogger logger;

        public HomeManager(ILogger logger, IRepositorio repositorio, ICampañaManager campañaManager, IObjetivoManager objetivoManager)
        {
            this.logger = logger;
            this.mobCampaña = campañaManager;
            this.objetivoManager = objetivoManager;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public ResultIniContacto TraerBusquedaContacto(oParamBusqueda oParam, int pagina, List<int> equipo)
        {
            var res = new ResultIniContacto();
            res.Contactos = new List<ContactoIni>();
            if (!PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial))
            {
                var query = repositorio.SelStorePaginado<Contactos>("DataAgro_BusquedaContactos", 50, pagina, oParam.Campaña,
                    oParam.Segmentacion, oParam.Actividad, oParam.Material, oParam.Calificacion,
                    oParam.Hectareas, oParam.Toneladas, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParam.Condicion, oParam.Estado, oParam.Comercial, oParam.Zona);

                res.Contactos = DevolverContactosIni(query);
            }
            else
            {
                var re = DevolverContactosIni(repositorio.ListarConsulta(new TraerCorredoresComercial()));
                res.Contactos.AddRange(re);
            }

            oParam.Estado = null;
            if (!PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial))
            {
                var queryPorEstado = repositorio.SelStore<Contactos>("DataAgro_BusquedaContactos", 0, oParam.Campaña,
                oParam.Segmentacion, oParam.Actividad, oParam.Material, oParam.Calificacion,
                oParam.Hectareas, oParam.Toneladas, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParam.Condicion, oParam.Estado, oParam.Comercial, oParam.Zona);

                res.TotalContactos = queryPorEstado.Select(a => a.CUIT).Distinct().Count();
                res.TotalPotencialContactos = queryPorEstado.Where(x => x.Estado == "Cliente Potencial").Select(a => a.CUIT).Distinct().Count();
                res.TotalOperandoContactos = queryPorEstado.Where(x => x.Estado == "Operando").Select(a => a.CUIT).Distinct().Count();
                res.TotalNoOperandoContactos = queryPorEstado.Where(x => x.Estado == "No operando").Select(a => a.CUIT).Distinct().Count();
                res.TotalBajaContactos = queryPorEstado.Where(x => x.Estado == "Baja").Select(a => a.CUIT).Distinct().Count();
                res.TotalSinInteresContactos = queryPorEstado.Where(x => x.Estado == "Sin interés de operar").Select(a => a.CUIT).Distinct().Count();
            }
            else
            {
                var cuenta = DevolverContactosIni(repositorio.ListarConsulta(new TraerCorredoresComercial()));
                res.TotalContactos = cuenta.Select(a => a.Cuit).Distinct().Count();
                res.TotalPotencialContactos = cuenta.Where(x => x.Estado == "Cliente Potencial").Select(a => a.Cuit).Distinct().Count();
                res.TotalOperandoContactos = cuenta.Where(x => x.Estado == "Operando").Select(a => a.Cuit).Distinct().Count();
                res.TotalNoOperandoContactos = cuenta.Where(x => x.Estado == "No operando").Select(a => a.Cuit).Distinct().Count();
                res.TotalBajaContactos = cuenta.Where(x => x.Estado == "Baja").Select(a => a.Cuit).Distinct().Count();
                res.TotalSinInteresContactos = cuenta.Where(x => x.Estado == "Sin interés de operar").Select(a => a.Cuit).Distinct().Count();
            }


            return res;
        }

        private List<ContactoIni> DevolverContactosIni(List<Contactos> list)
        {
            var lista = new List<ContactoIni>();
            ContactoIni cont = null;

            foreach (var con in list)
            {
                cont = new ContactoIni();
                cont.Calificacion = con.Calificacion;
                cont.ComercialCargo = con.ComercialAcargo;
                cont.Operando = true;
                cont.Cuit = con.CUIT;
                cont.Mail = (String.IsNullOrEmpty(con.Email1) ? String.Empty : (con.Email1)) +
                            (String.IsNullOrEmpty(con.Email2) ? String.Empty : (";" + con.Email2)) +
                            (String.IsNullOrEmpty(con.Email3) ? String.Empty : (";" + con.Email3)) +
                            (String.IsNullOrEmpty(con.Email4) ? String.Empty : (";" + con.Email4));
                cont.ProveedorId = con.ProveedorId;
                cont.RazonSocial = con.RazonSocial;
                cont.Telefono = (String.IsNullOrEmpty(con.Telefono1) ? String.Empty : (con.Telefono1)) +
                            (String.IsNullOrEmpty(con.Telefono2) ? String.Empty : (";" + con.Telefono2)) +
                            (String.IsNullOrEmpty(con.Telefono3) ? String.Empty : (";" + con.Telefono3)) +
                            (String.IsNullOrEmpty(con.Telefono4) ? String.Empty : (";" + con.Telefono4));
                cont.UltimoContacto = DevolverUltimoContacto(con.FechaUltimoContacto);
                cont.Estado = con.Estado;
                cont.FechaAlta = con.FechaAlta;
                cont.GrupoDeCompras = con.GrupoDeCompras;
                cont.Corredor = con.Segmentacion == 5 || con.Segmentacion == 7 ? true : false;
                CargarOperabilidad(cont, con);
                if (cont.NoOperable == true)
                {
                    cont.RptOpera = "No operable";
                }
                else
                {
                    cont.RptOpera = "Operable";
                }
                lista.Add(cont);
            }

            lista = lista.OrderBy(x => x.RazonSocial).ToList();

            return lista;
        }

        private void CargarOperabilidad(ContactoIni cont, Contactos con)
        {
            cont.NoOperable = false;
            cont.Operando = true;
            if (!String.IsNullOrEmpty(con.RiesgoComercialSap))
            {
                if (con.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                {
                    cont.NoOperable = true;
                    cont.Operando = false;
                    cont.TooltipNoOperable = "Riesgo Comercial Alto";
                }
            }
            if (con.EstadoCuit == 0)
            {
                cont.NoOperable = true;
                cont.Operando = false;
                cont.TooltipNoOperable = "Inactivo";
            }
            if (con.EstadoCuit == 3)
            {
                cont.NoOperable = true;
                cont.Operando = false;
                cont.TooltipNoOperable = "Estado 3";
            }
            if (con.Facacop == 1)
            {
                cont.Operando = false;
                cont.NoOperable = true;
                cont.TooltipNoOperable = "Apocrifos";
            }

        }

        private string DevolverUltimoContacto(DateTime? fechaUltimoContacto)
        {

            if (fechaUltimoContacto.HasValue)
            {
                var newdt = DateTime.Now;
                var olddt = fechaUltimoContacto.Value;

                Int32 anios;
                Int32 meses;
                Int32 dias;
                String str = "";

                anios = (newdt.Year - olddt.Year);
                meses = (newdt.Month - olddt.Month);
                dias = (newdt.Day - olddt.Day);

                if (meses < 0)
                {
                    anios -= 1;
                    meses += 12;
                }
                if (dias < 0)
                {
                    meses -= 1;
                    dias += DateTime.DaysInMonth(newdt.Year, newdt.Month);
                }

                if (anios < 0)
                {
                    return "Fecha Invalida";
                }

                if (anios > 0)
                    str = "hace " + anios + " años.";
                else if (meses > 0)
                    str = "hace " + meses + " meses.";
                else if (dias > 0)
                    str = "hace " + dias + " dias.";
                else if (dias == 0 && meses == 0 && anios == 0)
                    str = "hoy";

                return str;
            }
            else
                return "";
        }

        public CampañaHome TraerInfoCampaña(int idComercial, List<int> equipo)
        {
            return mobCampaña.TraerCampañaHome(idComercial, equipo);
        }
        public ObjetivoHome TraerInfoObjetivo(int idComercial, List<int> equipo)
        {
            return objetivoManager.TraerObjetivoHome(idComercial, equipo);
        }
        public int TraerIdComercial(string idActiveDirectory)
        {
            return repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == idActiveDirectory, x => x.ComercialId);
        }

        public DatosIniciales TraerInfoIniciales(List<int> equipo)
        {
            var DatosIni = new DatosIniciales
            {
                camp = repositorio.Listar<Campaña, CampañaQry>(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion }, null, 0, "CampañaId", Entities.Helpers.DirOrden.Desc),

                mat = repositorio.Listar<Material, MaterialesQry>(x => new MaterialesQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion }),

                segm = repositorio.Listar<Segmentacion, SegmentacionQry>(x => new SegmentacionQry() { SegmentacionId = x.SegmentacionId, Descripcion = x.Descripcion, Grupo = x.Grupo }),

                tipoact = repositorio.Listar<TipoActividad, TipoActividadQry>(x => new TipoActividadQry() { TipoActividadId = x.TipoActividadId, Descripcion = x.Descripcion }),

                est = repositorio.Listar<Estado, EstadoQry>(x => new EstadoQry() { EstadoId = x.EstadoId, Descripcion = x.Descripcion }),

                cond = repositorio.Listar<Condicion, CondicionPreferenteQry>(x => new CondicionPreferenteQry() { CondicionId = x.CondicionId, Descripcion = x.Descripcion }),

                come = repositorio.Listar<Comercial, ComercialQry>(s => new ComercialQry() { ComercialId = s.ComercialId, IdActiveDirectory = s.IdActiveDirectory }, x => equipo.Contains(x.ComercialId)),

                zona = repositorio.Listar<Comercial, ZonaQry>(x => new ZonaQry { Id = x.GrupoDeCompras.Id, Descripcion = x.GrupoDeCompras.Descripcion }, x => equipo.Contains(x.ComercialId) && x.GrupoDeCompras != null, 0, "Descripcion")
            };
            return DatosIni;

        }

        public List<BusquedaHome> BusquedaHome(string filtro, int comercialId, List<int> equipo, List<int> corredoresComercial)
        {
            //var query = repositorio.SelStore<BusquedaHome>("DataAgro_BusquedaHome", 0, filtro, ComercialId);
            var query = repositorio.ListarConsulta(new ConsultaBusquedaHome(equipo, comercialId, filtro, corredoresComercial, PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial)));
            var listaOrdenada = query.Where(x => string.IsNullOrEmpty(x.Alias)).OrderBy(x => x.RazonSocial);
            return query.Where(x => !string.IsNullOrEmpty(x.Alias)).OrderBy(x => x.Alias).ThenBy(x => x.RazonSocial).Concat(listaOrdenada).ToList();
        }

        public List<ActividadRecordatorio> TraerActividadesPorComercialId(int ComercialId)
        {
            var list = new List<ActividadRecordatorio>();

            var result = repositorio.Listar<Actividad, ActividadRecordatorioGrid>(x => new ActividadRecordatorioGrid
            {
                ActividadId = x.ActividadId,
                Comentarios = x.Detalle,
                FechaRecordatorio = x.FechaHoraRecordatorio.Value,
                Tema = x.TipoActividad.Descripcion,
                Contacto = x.ContactoComercial.Nombres + " " + x.ContactoComercial.Apellido,
                ProveedorId = x.ContactoComercial.Proveedor.ProveedorId
            }, x => x.ComercialId == ComercialId && x.FechaHoraRecordatorio.HasValue && x.FechaHoraRecordatorio.Value > DateTime.Now).OrderBy(x => x.FechaRecordatorio);

            foreach (var item in result)
            {
                list.Add(new ActividadRecordatorio
                {
                    ActividadId = item.ActividadId,
                    Comentarios = item.Comentarios,
                    Contacto = item.Contacto,
                    Tema = item.Tema,
                    Dia = (item.FechaRecordatorio.Date == DateTime.Now.Date) ? "Hoy " : item.FechaRecordatorio.ToString("dd/MM/yyyy") + " ",
                    Hora = item.FechaRecordatorio.ToString("HH:mm"),
                    ProveedorId = item.ProveedorId
                });
            }
            list.AddRange(repositorio.ListarConsulta(new ConsultarActividadResearch(ComercialId)));
            return list;
        }

        public List<ContactoIni> ExportarContactos(oParamBusqueda oParam, string idActiveDirectory, List<int> equipo)
        {

            var oComerciales = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory.ToLower() == idActiveDirectory.ToLower());

            if (oComerciales != null)
            {
                var ids = repositorio.SelStore<Contactos>("DataAgro_BusquedaContactos", 0, oParam.Campaña,
                oParam.Segmentacion, oParam.Actividad, oParam.Material, oParam.Calificacion,
                oParam.Hectareas, oParam.Toneladas, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParam.Condicion, oParam.Estado, oParam.Comercial, oParam.Zona).Select(x => x.ProveedorId).ToList(); ;

                var Contactos = repositorio.SelStore<Contactos>("DataAgro_Contactos_Exportar", 0, string.Join(",", ids.Select(n => n.ToString()).ToArray()), string.Join(",", equipo.Select(n => n.ToString()).ToArray()));

                var aux = DevolverContactosIni(Contactos);

                foreach (var i in aux)
                {
                    if (i.Mail != "")
                    {
                        i.Mail = i.Mail.Replace(";", "; ");
                    }
                    if (i.Telefono != "")
                    {
                        i.Telefono = i.Telefono.Replace(";", "; ");
                    }
                }
                return aux;
            }
            else
                return new List<ContactoIni>();
        }

        public ExportAll ExportarAll(oParamBusqueda oParam, string idActiveDirectory, List<int> equipo)
        {
            ExportAll exp = new ExportAll();

            var oComerciales = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory.ToLower() == idActiveDirectory.ToLower());

            if (oComerciales != null)
            {
                var ids = repositorio.SelStore<Contactos>("DataAgro_BusquedaContactos", 0, oParam.Campaña,
                oParam.Segmentacion, oParam.Actividad, oParam.Material, oParam.Calificacion,
                oParam.Hectareas, oParam.Toneladas, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParam.Condicion, oParam.Estado, oParam.Comercial, oParam.Zona).Select(x => x.ProveedorId.ToString()).ToArray(); ;
                var idsStr = string.Join(",", ids);

                var idProveedores = repositorio.ListarConsulta(new TraerBusquedaContactosProveedorId(oParam, equipo)).Select(x => x.ProveedorId).Distinct();


                if (!PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial))
                {
                    exp.contacto = repositorio.SelStore<ContactoAll>("DataAgro_ExportAll_Contacto", 0, idsStr, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
                }
                else
                {
                    var comerciales = repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VerCorredorComercial)));

                    var re = repositorio.ListarConsulta(new TraerCorredoresComercialExportarAll(comerciales, oParam.ComercialId));
                    exp.contacto.AddRange(re);
                }

                exp.objetivo = repositorio.SelStore<ObjetivoAll>("DataAgro_ExportAll_Objetivos", 0, idsStr);

                //exp.ContactosPrincipales = repositorio.SelStore<ContactosPrincipalesAll>("DataAgro_ExportAll_ContactosPrincipales", 0, idsStr);
                exp.ContactosPrincipales = repositorio.ListarConsulta(new TraerExportarAllContactosComerciales(exp.contacto.Select(x => x.Cuit).ToList()));

                exp.produccion = repositorio.SelStore<ProduccionAll>("DataAgro_ExportAll_Produccion", 0, idsStr);

                exp.almacenamiento = repositorio.SelStore<AlmacenamientoAll>("DataAgro_ExportAll_Almacenamiento", 0, idsStr);

                exp.agenda = repositorio.SelStore<AgendaAll>("DataAgro_ExportAll_Actividades", 0, idsStr);

                exp.compras = repositorio.SelStore<ComprasAll>("DataAgro_ExportAll_Compras", 0, idsStr);

                exp.establecimiento = repositorio.ListarConsulta(new TraerExportarAllEstablecimientos(exp.contacto.Select(x => x.Cuit).ToList()));
                exp.CompraCampanaActual = this.TraerTodoCompraCampanaActual(equipo);
                exp.Situacion = this.TraerTodoCompraDetalleExcel(equipo);
                return exp;
            }
            else
            {
                return new ExportAll();
            }
        }
        public List<int> ListarTodosLosComercialesConMismaZona(int comercialId)
        {
            var grupoId = repositorio.Obtener<Comercial, int>(x => x.ComercialId == comercialId, x => x.GrupoDeComprasId.Value);
            return repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.GrupoDeComprasId == grupoId);
        }

        public PostItDto TraerTexto(int idComercial)
        {
            return repositorio.Obtener<PostIt, PostItDto>(x => x.ComercialId == idComercial, x => new PostItDto { ComercialId = x.ComercialId, Texto = x.Texto });
        }

        public GrabarPostItResult GuardarPostIt(PostIt post)
        {
            var oEntityErrors = new GrabarPostItResult();
            try
            {
                var postit = repositorio.Obtener<PostIt>(x => x.ComercialId == post.ComercialId);

                if (postit == null)
                {
                    repositorio.Agregar(post);
                }
                else
                {
                    postit.Texto = post.Texto;
                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error("", ex.Message);
            }
            return oEntityErrors;
        }
        public List<CompraDto> TraerTodoCompraDetalle(List<int> equipo)
        {
            var material = repositorio.Listar<Material>();
            var compraDto = new List<CompraDto>();
            var proveedorIds = new List<int?>();
            var proveedores = repositorio.Listar<Proveedor>().ToList();
            var cuits = proveedores.Select(x => x.CUIT).Distinct().ToList();
            foreach (var cuit in cuits)
            {
                var provid = proveedores.Where(x => x.CUIT == cuit).First().ProveedorId;
                proveedorIds.Add(provid);
            }
            foreach (var mat in material)
            {
                var c = repositorio.Listar<CampanaMaterialDetallePorMes>(x =>
                equipo.Contains(x.ComercialId.Value)
                && (proveedorIds.Contains(x.ProveedorId) || proveedorIds.Contains(x.CorredorId))
                && mat.MaterialId == x.MaterialId
                && mat.CampañaId == x.CampanaId);


                var detalle = new CompraDto
                {
                    Campana = c.Select(x => x.Campana.Descripcion).FirstOrDefault(),
                    Material = c.Select(x => x.Material.Descripcion).FirstOrDefault(),

                    ConCorredor = new CompraDetalleDto
                    {
                        ComprasConPrecio = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc != "ZPAF").
                         Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
                        x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
                        x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
                        c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF").Sum(x => x.ToneladaFijada),
                        RecibidoSinPrecio = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF").Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),
                        ARecibirAFijar = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0)
                            .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
                            ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),
                        FasonFas = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)
                    },
                    DirectoAcopiador = new CompraDetalleDto
                    {
                        ComprasConPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc != "ZPAF").
                         Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
                        x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
                        x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
                        c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF").Sum(x => x.ToneladaFijada),
                        RecibidoSinPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF").Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),
                        ARecibirAFijar = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0)
                            .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
                            ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),
                        FasonFas = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)

                    },
                    DirectoProductor = new CompraDetalleDto
                    {
                        ComprasConPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc != "ZPAF").
                         Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
                        x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
                        x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
                        c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF").Sum(x => x.ToneladaFijada),
                        RecibidoSinPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF").Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),
                        ARecibirAFijar = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0)
                            .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
                            ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),
                        FasonFas = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)
                    }

                };
                if (detalle.ConCorredor.ARecibirAFijar + detalle.ConCorredor.ComprasConPrecio + detalle.ConCorredor.FasonFas + detalle.ConCorredor.RecibidoSinPrecio
                    + detalle.DirectoAcopiador.ARecibirAFijar + detalle.DirectoAcopiador.ComprasConPrecio + detalle.DirectoAcopiador.FasonFas + detalle.DirectoAcopiador.RecibidoSinPrecio
                    + detalle.DirectoProductor.ARecibirAFijar + detalle.DirectoProductor.ComprasConPrecio + detalle.DirectoProductor.FasonFas + detalle.DirectoProductor.RecibidoSinPrecio
                    > 0)
                {
                    compraDto.Add(detalle);
                }
            }
            return compraDto;
        }

        public List<CampanaMaterialDetallePorMeseExcelDto> TraerTodoCompraDetalleExcel(List<int> equipo)
        {
            logger.Info("TraerTodoCompraDetalleExcel get data");
            var compraDto = new List<CompraDto>();
            var proveedorIds = new List<int?>();
            var proveedores = repositorio.Listar<Proveedor>().ToList();
            var cuits = proveedores.Select(x => x.CUIT).Distinct().ToList();
            foreach (var cuit in cuits)
            {
                var provid = proveedores.Where(x => x.CUIT == cuit).First().ProveedorId;
                proveedorIds.Add(provid);
            }
            var compras = repositorio.Listar<CampanaMaterialDetallePorMes, CampanaMaterialDetallePorMesDto>(x => new CampanaMaterialDetallePorMesDto
            {
                ClaseDoc = x.ClaseDoc,
                Clasificacion = x.Clasificacion,
                ComercialId = x.ComercialId,
                Contrato = x.Contrato,
                Fecha = x.Fecha,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion,
                PendienteAFijar = x.PendienteAFijar,
                PendienteAplicar = x.PendienteAplicar,
                ToneladaAmpliada = x.ToneladaAmpliada,
                ToneladaAnulada = x.ToneladaAnulada,
                ToneladaAplicada = x.ToneladaAplicada,
                ToneladaContrato = x.ToneladaContrato,
                CorredorCuit = x.CorredorCuit,
                ProveedorId = x.ProveedorId,
                ToneladaFijada = x.ToneladaFijada,
                CampanaId = x.CampanaId,
                Campana = x.Campana.Descripcion,
                Proveedor = x.Proveedor.RazonSocial,
                CUIT = x.Proveedor.CUIT,
                RazonSocialCorredor = x.Corredor != null ? x.Corredor.RazonSocial : ""
            }, x => equipo.Contains(x.ComercialId.Value) && (proveedorIds.Contains(x.ProveedorId) || proveedorIds.Contains(x.CorredorId))); 

            var campanaMaterialDetallePorMeseExcelDtos = new List<CampanaMaterialDetallePorMeseExcelDto>();

            foreach (var x in compras)
            {
                var itemDto = new CampanaMaterialDetallePorMeseExcelDto();
                itemDto.Material = x.Material;
                itemDto.Campaña = x.Campana;
                itemDto.Cuit = x.CUIT;
                itemDto.RazonSocial = x.Proveedor;
                itemDto.Contrato = x.Contrato;
                itemDto.CorredorCuit = x.CorredorCuit;
                itemDto.RazonSocialCorredor = x.RazonSocialCorredor;
                itemDto.Clasificacion = !String.IsNullOrEmpty(x.CorredorCuit) ? "CORREDOR" : x.Clasificacion;
                if (((!String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc != "ZPAF") || (!String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF")) ||
                    (String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc != "ZPAF") || (String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF") ||
                    (String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc != "ZPAF") || (String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF")
                    )
                {
                    CampanaMaterialDetallePorMeseExcelDto cloned = (CampanaMaterialDetallePorMeseExcelDto)itemDto.Clone();
                    cloned.Situacion = "Compras Con Precio";
                    cloned.Tn = x.ClaseDoc != "ZPAF" ? (x.ToneladaAplicada > x.ToneladaContrato ?
                        x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
                        x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada): 0;
                    cloned.Tn += x.ClaseDoc == "ZPAF" ? x.ToneladaFijada : 0;
                    if (cloned.Tn != 0)
                        campanaMaterialDetallePorMeseExcelDtos.Add(cloned);
                }
                if (x.ClaseDoc == "ZPAF")
                {
                    CampanaMaterialDetallePorMeseExcelDto cloned = (CampanaMaterialDetallePorMeseExcelDto)itemDto.Clone();
                    cloned.Situacion = "Recibido Sin Precio";
                    cloned.Tn = x.ToneladaAplicada - x.ToneladaFijada < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada;
                    if (cloned.Tn != 0)
                        campanaMaterialDetallePorMeseExcelDtos.Add(cloned);
                }
                if (x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0)
                {
                    CampanaMaterialDetallePorMeseExcelDto cloned = (CampanaMaterialDetallePorMeseExcelDto)itemDto.Clone();
                    cloned.Situacion = "A Recibir a Fijar";
                    cloned.Tn = x.ToneladaFijada - x.ToneladaAplicada > 0 ?((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar;
                    if (cloned.Tn != 0)
                        campanaMaterialDetallePorMeseExcelDtos.Add(cloned);
                }
                if (x.ClaseDoc == "ZFAZ")
                {
                    CampanaMaterialDetallePorMeseExcelDto cloned = (CampanaMaterialDetallePorMeseExcelDto)itemDto.Clone();
                    cloned.Situacion = "Fasón Fas";
                    cloned.Tn = x.ToneladaContrato;
                    if (cloned.Tn != 0)
                        campanaMaterialDetallePorMeseExcelDtos.Add(cloned);
                }

            }


            return campanaMaterialDetallePorMeseExcelDtos;
        }
        public List<CompraCampanaActualDto> TraerTodoCompraCampanaActual(List<int> equipo)
        {
            var proveedorIds = new List<int?>();
            var proveedores = repositorio.Listar<Proveedor>().ToList();
            var cuits = proveedores.Select(x => x.CUIT).Distinct().ToList();
            foreach (var cuit in cuits)
            {
                var provid = proveedores.Where(x => x.CUIT == cuit).First().ProveedorId;
                proveedorIds.Add(provid);
            }
            return repositorio.Listar<CampanaMaterialDetallePorMes, CompraCampanaActualDto>(x => new CompraCampanaActualDto()
            {
                Fecha = x.Fecha,
                Contrato = x.Contrato,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                Cuit = x.Proveedor.CUIT,
                RazonSocial = x.Proveedor.RazonSocial,
                CorredorCuit = x.CorredorCuit,
                Material = x.Material.Descripcion,
                Campana = x.Campana.Descripcion,
                ClaseDoc = x.ClaseDoc,
                Clasificacion = x.Clasificacion,
                PendienteAFijar = x.PendienteAFijar,
                ToneladaAmpliada = x.ToneladaAmpliada,
                ToneladaAnulada = x.ToneladaAnulada,
                ToneladaAplicada = x.ToneladaAplicada,
                ToneladaContrato = x.ToneladaContrato,
                ToneladaFijada = x.ToneladaFijada,
                PendienteAplicar = x.PendienteAplicar
                

            }, x => equipo.Contains(x.ComercialId.Value) && (proveedorIds.Contains(x.ProveedorId) || proveedorIds.Contains(x.CorredorId)));
        }
    }
    public class FakeHome
    {
        public int Id { set; get; }
        public string Nombre { set; get; }
    }




}
