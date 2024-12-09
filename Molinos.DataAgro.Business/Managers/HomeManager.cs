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
using System.Globalization;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class HomeManager : IHomeManager
    {
        private readonly IRepositorio repositorio;
        private readonly ICampañaManager mobCampaña;
        private readonly IObjetivoManager objetivoManager;
        private readonly ILogger logger;

        public HomeManager(ILogger logger, IRepositorio repositorio, ICampañaManager campañaManager, IObjetivoManager objetivoManager)
        {
            this.logger = logger;
            this.mobCampaña = campañaManager;
            this.objetivoManager = objetivoManager;
            this.repositorio = repositorio;
        }

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

                res.TotalHabilitadoContactos = queryPorEstado.Where(x => x.EstadoHomeDescripcion == "Habilitado").Select(a => a.CUIT).Distinct().Count();
                res.TotalLegajoIrregularContactos = queryPorEstado.Where(x => x.EstadoHomeDescripcion == "Legajo irregular").Select(a => a.CUIT).Distinct().Count();
                res.TotalNoHabilitadoContactos = queryPorEstado.Where(x => x.EstadoHomeDescripcion == "No habilitado").Select(a => a.CUIT).Distinct().Count();
            }
            else
            {
                //var cuenta = DevolverContactosIni(repositorio.ListarConsulta(new TraerCorredoresComercial()));
                res.TotalContactos = res.Contactos.Select(a => a.Cuit).Distinct().Count();
                res.TotalPotencialContactos = res.Contactos.Where(x => x.Estado == "Cliente Potencial").Select(a => a.Cuit).Distinct().Count();
                res.TotalOperandoContactos = res.Contactos.Where(x => x.Estado == "Operando").Select(a => a.Cuit).Distinct().Count();
                res.TotalNoOperandoContactos = res.Contactos.Where(x => x.Estado == "No operando").Select(a => a.Cuit).Distinct().Count();
                res.TotalBajaContactos = res.Contactos.Where(x => x.Estado == "Baja").Select(a => a.Cuit).Distinct().Count();
                res.TotalSinInteresContactos = res.Contactos.Where(x => x.Estado == "Sin interés de operar").Select(a => a.Cuit).Distinct().Count();

                res.TotalHabilitadoContactos = res.Contactos.Where(x => x.EstadoHomeDescripcion == "Habilitado").Select(a => a.Cuit).Distinct().Count();
                res.TotalLegajoIrregularContactos = res.Contactos.Where(x => x.EstadoHomeDescripcion == "Legajo irregular").Select(a => a.Cuit).Distinct().Count();
                res.TotalNoHabilitadoContactos = res.Contactos.Where(x => x.EstadoHomeDescripcion == "No habilitado").Select(a => a.Cuit).Distinct().Count();
            }


            return res;
        }

        private List<ContactoIni> DevolverContactosIni(List<Contactos> list)
        {
            var lista = new List<ContactoIni>();
            ContactoIni cont = null;
            var a = list.GroupBy(x => x.ProveedorId);
            foreach (var con in a)
            {
                cont = new ContactoIni();
                cont.Calificacion = con.FirstOrDefault().Calificacion;
                cont.ComercialCargo = string.Join("; ", con.Select(x => x.ComercialAcargo).Distinct());
                cont.Operando = true;
                cont.Cuit = con.FirstOrDefault().CUIT;
                cont.Mail = (String.IsNullOrEmpty(con.FirstOrDefault().Email1) ? String.Empty : (con.FirstOrDefault().Email1)) +
                            (String.IsNullOrEmpty(con.FirstOrDefault().Email2) ? String.Empty : (";" + con.FirstOrDefault().Email2)) +
                            (String.IsNullOrEmpty(con.FirstOrDefault().Email3) ? String.Empty : (";" + con.FirstOrDefault().Email3)) +
                            (String.IsNullOrEmpty(con.FirstOrDefault().Email4) ? String.Empty : (";" + con.FirstOrDefault().Email4));
                cont.ProveedorId = con.FirstOrDefault().ProveedorId;
                cont.RazonSocial = con.FirstOrDefault().RazonSocial;
                cont.Telefono = (String.IsNullOrEmpty(con.FirstOrDefault().Telefono1) ? String.Empty : (con.FirstOrDefault().Telefono1)) +
                            (String.IsNullOrEmpty(con.FirstOrDefault().Telefono2) ? String.Empty : (";" + con.FirstOrDefault().Telefono2)) +
                            (String.IsNullOrEmpty(con.FirstOrDefault().Telefono3) ? String.Empty : (";" + con.FirstOrDefault().Telefono3)) +
                            (String.IsNullOrEmpty(con.FirstOrDefault().Telefono4) ? String.Empty : (";" + con.FirstOrDefault().Telefono4));
                cont.UltimoContacto = DevolverUltimoContacto(con.FirstOrDefault().FechaUltimoContacto);
                cont.Estado = con.FirstOrDefault().Estado;
                cont.FechaAlta = con.FirstOrDefault().FechaAlta;
                cont.GrupoDeCompras = string.Join("; ", con.Select(x => x.GrupoDeCompras).Distinct());
                cont.Corredor = con.FirstOrDefault().Segmentacion == 5 || con.FirstOrDefault().Segmentacion == 7 ? true : false;
                CargarOperabilidad(cont, con.FirstOrDefault());
                if (cont.NoOperable == true)
                {
                    cont.RptOpera = "No operable";
                }
                else
                {
                    cont.RptOpera = "Operable";
                }
                cont.EstadoHomeId = con.FirstOrDefault().EstadoHomeId;
                cont.EstadoHomeMensaje = con.FirstOrDefault().EstadoHomeMensaje;
                cont.EstadoHomeDescripcion = con.FirstOrDefault().EstadoHomeDescripcion;

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

        public ObjetivoHome TraerInfoObjetivo(int? idComercial, List<int> equipo, int? idZona, int? idComercialLogeado)
        {
            return objetivoManager.TraerObjetivoHome(idComercial, equipo, idZona, idComercialLogeado);
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

                Segmentacion = repositorio.Listar<Segmentacion, SegmentacionQry>(x => new SegmentacionQry() { SegmentacionId = x.SegmentacionId, Descripcion = x.Descripcion, Grupo = x.Grupo }),

                TipoActividad = repositorio.Listar<TipoActividad, TipoActividadQry>(x => new TipoActividadQry() { TipoActividadId = x.TipoActividadId, Descripcion = x.Descripcion }),

                est = repositorio.Listar<Estado, EstadoQry>(x => new EstadoQry() { EstadoId = x.EstadoId, Descripcion = x.Descripcion }),

                cond = repositorio.Listar<Condicion, CondicionPreferenteQry>(x => new CondicionPreferenteQry() { CondicionId = x.CondicionId, Descripcion = x.Descripcion }),

                come = repositorio.Listar<Comercial, ComercialQry>(s => new ComercialQry() { ComercialId = s.ComercialId, Comercial = s.Nombres + " " + s.Apellido }, x => equipo.Contains(x.ComercialId) && x.AsignarNegocios, 0, "Comercial", Entities.Helpers.DirOrden.Asc),

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
                    exp.Contacto = repositorio.SelStore<ContactoAll>("DataAgro_ExportAll_Contacto", 0, idsStr, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
                }
                else
                {
                    var comerciales = repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VerCorredorComercial)));

                    var re = repositorio.ListarConsulta(new TraerCorredoresComercialExportarAll(comerciales, oParam.ComercialId));
                    exp.Contacto.AddRange(re);
                }
                exp.Contacto = DarFormato(exp.Contacto);
                exp.Objetivo = repositorio.SelStore<ObjetivoAll>("DataAgro_ExportAll_Objetivos", 0, idsStr);

                //exp.ContactosPrincipales = repositorio.SelStore<ContactosPrincipalesAll>("DataAgro_ExportAll_ContactosPrincipales", 0, idsStr);
                exp.ContactosPrincipales = repositorio.ListarConsulta(new TraerExportarAllContactosComerciales(exp.Contacto.Select(x => x.Cuit).ToList()));

                exp.Produccion = repositorio.SelStore<ProduccionAll>("DataAgro_ExportAll_Produccion", 0, idsStr);

                exp.Almacenamiento = repositorio.SelStore<AlmacenamientoAll>("DataAgro_ExportAll_Almacenamiento", 0, idsStr);

                exp.Agenda = repositorio.SelStore<AgendaAll>("DataAgro_ExportAll_Actividades", 0, idsStr);

                exp.Compras = repositorio.SelStore<ComprasAll>("DataAgro_ExportAll_Compras", 0, idsStr);

                exp.CapacidadProductiva = repositorio.ListarConsulta(new TraerExportarAllCapacidadProductiva(exp.Contacto.Select(x => x.Cuit).ToList()));

                exp.Establecimiento = repositorio.ListarConsulta(new TraerExportarAllEstablecimientos(exp.Contacto.Select(x => x.Cuit).ToList()));

                exp.CompraCampanaActual = TraerTodoCompraCampanaActual(equipo);

                exp.Situacion = TraerTodoCompraDetalleExcel(equipo);

                exp.ActividadComercial = UltimaActividadComercial(equipo);

                exp.ObjetivoCampania = ObjetivoCampania(equipo);

                return exp;
            }
            else
            {
                return new ExportAll();
            }
        }

        /// <summary>
        /// Busca la última actividad de un comercial sobre sus proveedores asignados en las tablas Negocio, Actividad, Cupo, InformeComercial y LogProveedor.
        /// </summary>
        /// <param name="equipo">Lista del equipo de trabajo al cual un comercial pertenece.</param>
        /// <returns>Lista con las últimas actividades de uno o más comerciales sobre sus proveedores asignados.</returns>
        private List<ActividadComercial> UltimaActividadComercial(List<int> equipo)
        {
            var negocios = repositorio.Listar<Negocio, ActividadComercial>(x => new ActividadComercial
            { Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres, Proveedor = x.Proveedor.RazonSocial, Negocio = x.Fecha, CUIT = x.Proveedor.CUIT, FechaAlta = x.Proveedor.FechaAlta }
            , x => equipo.Contains(x.ComercialId ?? 0) && x.ComercialId != null && x.ProveedorId != null);

            var resultadoFinal = negocios.GroupBy(a => new { a.Comercial, a.Proveedor, a.CUIT, a.FechaAlta })
            .Select(a => new ActividadComercial
            {
                Comercial = a.Key.Comercial,
                Proveedor = a.Key.Proveedor,
                CUIT = a.Key.CUIT,
                FechaAlta = a.Key.FechaAlta,
                Negocio = a.OrderByDescending(b => b.Negocio).First().Negocio
            }).ToList();

            var actividades = repositorio.Listar<Actividad, ActividadComercial>(x => new ActividadComercial
            { Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres, Proveedor = x.Proveedor.RazonSocial, Agenda = x.FechaHoraActividad, CUIT = x.Proveedor.CUIT, FechaAlta = x.Proveedor.FechaAlta }
                , x => equipo.Contains(x.ComercialId ?? 0) && x.ComercialId != null)
                .GroupBy(a => new { a.Comercial, a.Proveedor, a.CUIT, a.FechaAlta })
                .Select(a => new ActividadComercial
                {
                    Comercial = a.Key.Comercial,
                    Proveedor = a.Key.Proveedor,
                    CUIT = a.Key.CUIT,
                    FechaAlta = a.Key.FechaAlta,
                    Agenda = a.OrderByDescending(b => b.Agenda).First().Agenda
                }).ToList();

            AgregarAlResultadoActividad(resultadoFinal, actividades);

            var cupos = repositorio.Listar<Cupo, ActividadComercial>(x => new ActividadComercial
            { Comercial = x.ComercialCreador.Apellido + " " + x.ComercialCreador.Nombres, Proveedor = x.Proveedor.RazonSocial, Cupo = x.FechaGeneracion, CUIT = x.Proveedor.CUIT, FechaAlta = x.Proveedor.FechaAlta }
                , x => equipo.Contains(x.ComercialCreadorId ?? 0) && x.ComercialCreadorId != null)
                .GroupBy(a => new { a.Comercial, a.Proveedor, a.CUIT, a.FechaAlta })
                .Select(a => new ActividadComercial
                {
                    Comercial = a.Key.Comercial,
                    Proveedor = a.Key.Proveedor,
                    CUIT = a.Key.CUIT,
                    FechaAlta = a.Key.FechaAlta,
                    Cupo = a.OrderByDescending(b => b.Cupo).First().Cupo
                }).ToList();

            AgregarAlResultadoActividad(resultadoFinal, cupos);

            var informesCom = repositorio.Listar<InformeComercial, ActividadComercial>(x => new ActividadComercial
            { Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres, Proveedor = x.Proveedor.RazonSocial, InformeComercial = x.FechaAlta, CUIT = x.Proveedor.CUIT, FechaAlta = x.Proveedor.FechaAlta }
                , x => equipo.Contains(x.ComercialId ?? 0) && x.ComercialId != null)
                .GroupBy(a => new { a.Comercial, a.Proveedor, a.CUIT, a.FechaAlta })
                .Select(a => new ActividadComercial
                {
                    Comercial = a.Key.Comercial,
                    Proveedor = a.Key.Proveedor,
                    CUIT = a.Key.CUIT,
                    FechaAlta = a.Key.FechaAlta,
                    InformeComercial = a.OrderByDescending(b => b.InformeComercial).First().InformeComercial
                }).ToList();

            AgregarAlResultadoActividad(resultadoFinal, informesCom);

            var logProveedor = repositorio.Listar<LogProveedor, ActividadComercial>(x => new ActividadComercial
            { Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres, Proveedor = x.Proveedor.RazonSocial, ModificacionProveedor = x.Fecha, CUIT = x.Proveedor.CUIT, FechaAlta = x.Proveedor.FechaAlta }
                , x => equipo.Contains(x.ComercialId ?? 0) && x.ComercialId != null)
                .GroupBy(a => new { a.Comercial, a.Proveedor, a.CUIT, a.FechaAlta })
                .Select(a => new ActividadComercial
                {
                    Comercial = a.Key.Comercial,
                    Proveedor = a.Key.Proveedor,
                    CUIT = a.Key.CUIT,
                    FechaAlta = a.Key.FechaAlta,
                    ModificacionProveedor = a.OrderByDescending(b => b.ModificacionProveedor).First().ModificacionProveedor
                }).ToList();

            AgregarAlResultadoActividad(resultadoFinal, logProveedor);

            return resultadoFinal.OrderBy(x => x.Comercial).ThenBy(x => x.Proveedor).ToList();
        }
        
        private static void AgregarAlResultadoActividad(List<ActividadComercial> resultadoFinal, List<ActividadComercial> lista)
        {
            foreach (var itemNuevo in lista)
            {
                var existe = resultadoFinal.Where(a => a.Proveedor == itemNuevo.Proveedor && a.Comercial == itemNuevo.Comercial && a.FechaAlta == itemNuevo.FechaAlta).SingleOrDefault();
                if (existe != null)
                {
                    if (itemNuevo.Negocio != null) existe.Negocio = itemNuevo.Negocio;
                    if (itemNuevo.Agenda != null) existe.Agenda = itemNuevo.Agenda;
                    if (itemNuevo.Cupo != null) existe.Cupo = itemNuevo.Cupo;
                    if (itemNuevo.InformeComercial != null) existe.InformeComercial = itemNuevo.InformeComercial;
                    if (itemNuevo.ModificacionProveedor != null) existe.ModificacionProveedor = itemNuevo.ModificacionProveedor;
                }
                else
                {
                    resultadoFinal.Add(itemNuevo);
                }
            }
        }
        
        private List<ObjetivoCampania> ObjetivoCampania(List<int> equipo)
        {
            var objetivoCampania = new List<ObjetivoCampania>();
            var objetivosPorCom = TraerInfoObjetivo(null, equipo, null, null).Comerciales;
            foreach (var comercial in objetivosPorCom)
            {
                var campaña = new CampañaHome();
                var comprasDelComercial = TraerTodoCompraDetalle(equipo, comercial.ComercialId, null);
                foreach (var compras in comprasDelComercial)
                {
                    campaña.Materiales.Add(
                        new MaterialCampaña
                        {
                            Campaña = compras.Campana,
                            Nombre = compras.Material,
                            Toneladas = compras.TotalCompra
                        });
                }
                foreach (var objetivo in comercial.Objetivos)
                {
                    var add = new ObjetivoCampania { Comercial = comercial.Comercial, Material = objetivo.Material, Campania = objetivo.Campana, ToneladasObjetivo = objetivo.Toneladas.ToString("#,##0.##", CultureInfo.CreateSpecificCulture("es-AR")) };
                    var find = campaña.Materiales.Find(x => x.Nombre == objetivo.Material);
                    if (find != null)
                    {
                        add.ToneladasCompradas = find.Toneladas.ToString("#,##0.##", CultureInfo.CreateSpecificCulture("es-AR")); //no muestra decimales en cero y muestra separación de miles 
                        add.PorcentajeDeCumplimiento = (find.Toneladas * 100 / objetivo.Toneladas).ToString("#,##0.##", CultureInfo.CreateSpecificCulture("es-AR"));
                    }
                    else
                    {
                        add.ToneladasCompradas = "0";
                        add.PorcentajeDeCumplimiento = "0";
                    }
                    objetivoCampania.Add(add);
                }
            }

            return objetivoCampania;
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

        public List<CompraDto> TraerTodoCompraDetalle(List<int> equipo, int? comercialId, int? zonaId)
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
            int año = DateTime.Now.Year;

            if (DateTime.Now.Date > new DateTime(DateTime.Now.Year, 4, 1))
                año += 1;
            var fechaDesde = new DateTime(año - 1, 04, 01);
            var fechaHasta = new DateTime(año, 03, 31);
            foreach (var mat in material)
            {
                var c = repositorio.Listar<CampanaMaterialDetallePorMes>(x => 1 == 1
                && equipo.Contains(x.ComercialId.Value)
                && (proveedorIds.Contains(x.ProveedorId) || proveedorIds.Contains(x.CorredorId))
                && mat.MaterialId == x.MaterialId
                && (comercialId == null || comercialId == x.ComercialId)
                && (zonaId == null || zonaId == x.Comercial.GrupoDeComprasId)
                && x.FechaHasta <= fechaHasta && x.FechaHasta >= fechaDesde
                );

                año = int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + mat.Campaña.Descripcion.Substring(3, 2)) + 1;
                var fechaCorteCampaña = new DateTime(año, 04, 01);

                ///Clase doc
                ///ZPAF A Fijar
                ///ZFAS MP-Fason
                ///ZFJ$ A Precio
                ///ZHIJ Contrato hijo (negocio.Madre == false // que no es lo mismo que != true en este caso puntual...)
                var detalle = new CompraDto
                {
                    Campana = mat.Campaña.Descripcion,// c.Select(x => x.Campana.Descripcion).FirstOrDefault(),
                    Material = mat.Descripcion,// c.Select(x => x.Material.Descripcion).FirstOrDefault(),

                    ConCorredor = new CompraDetalleDto
                    {
                        ComprasConPrecio = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc != "ZPAF" && mat.CampañaId == x.CampanaId).
                         Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
                        x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
                        x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
                        c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF" && mat.CampañaId == x.CampanaId).Sum(x => x.ToneladaFijada),

                        RecibidoSinPrecio = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF")
                        .Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),

                        ARecibirAFijar = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.Centro != "8107" && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0 && x.FechaDesde < fechaCorteCampaña)
                            .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
                            ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 :
                            x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),

                        FasonFas = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)
                    },
                    DirectoAcopiador = new CompraDetalleDto
                    {
                        ComprasConPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc != "ZPAF" && mat.CampañaId == x.CampanaId).
                         Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
                        x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
                        x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
                        c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF" && mat.CampañaId == x.CampanaId).Sum(x => x.ToneladaFijada),

                        RecibidoSinPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF").Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),

                        ARecibirAFijar = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.Centro != "8107" && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0 && x.FechaDesde < fechaCorteCampaña)
                            .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
                            ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),

                        FasonFas = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)

                    },
                    DirectoProductor = new CompraDetalleDto
                    {
                        ComprasConPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc != "ZPAF" && mat.CampañaId == x.CampanaId).
                         Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
                        x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
                        x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
                        c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF" && mat.CampañaId == x.CampanaId).Sum(x => x.ToneladaFijada),

                        RecibidoSinPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF").Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),

                        ARecibirAFijar = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.Centro != "8107" && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0 && x.FechaDesde < fechaCorteCampaña)
                            .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
                            ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),

                        FasonFas = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)
                    },
                    TotalCompra = c.Where(a => a.ClaseDoc == "ZFJ$").Sum(x => x.ToneladaContrato) + c.Where(a => a.ClaseDoc == "ZPAF").Sum(x => x.ToneladaFijada)
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

        //public List<CompraDto> TraerTodoCompraDetalle(List<int> equipo, int? comercialId, int? zonaId)
        //{
        //    var material = repositorio.Listar<Material>();
        //    var compraDto = new List<CompraDto>();
        //    var proveedorIds = new List<int?>();
        //    var proveedores = repositorio.Listar<Proveedor>().ToList();
        //    var cuits = proveedores.Select(x => x.CUIT).Distinct().ToList();
        //    foreach (var cuit in cuits)
        //    {
        //        var provid = proveedores.Where(x => x.CUIT == cuit).First().ProveedorId;
        //        proveedorIds.Add(provid);
        //    }
        //    foreach (var mat in material)
        //    {
        //        var c = repositorio.Listar<CampanaMaterialDetallePorMes>(x => 1==1
        //        && equipo.Contains(x.ComercialId.Value)
        //        && (proveedorIds.Contains(x.ProveedorId) || proveedorIds.Contains(x.CorredorId))
        //        && mat.MaterialId == x.MaterialId
        //        && (comercialId == null || comercialId == x.ComercialId)
        //        && (zonaId == null || zonaId == x.Comercial.GrupoDeComprasId)
        //        );

        //        int año = int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + mat.Campaña.Descripcion.Substring(3, 2)) + 1;
        //        var fechaCorteCampaña = new DateTime(año, 04, 01);

        //        ///Clase doc
        //        ///ZPAF A Fijar
        //        ///ZFAS MP-Fason
        //        ///ZFJ$ A Precio
        //        ///ZHIJ Contrato hijo (negocio.Madre == false // que no es lo mismo que != true en este caso puntual...)
        //        var detalle = new CompraDto
        //        {
        //            Campana = mat.Campaña.Descripcion,// c.Select(x => x.Campana.Descripcion).FirstOrDefault(),
        //            Material = mat.Descripcion,// c.Select(x => x.Material.Descripcion).FirstOrDefault(),

        //            ConCorredor = new CompraDetalleDto
        //            {
        //                ComprasConPrecio = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc != "ZPAF" && mat.CampañaId == x.CampanaId).
        //                 Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
        //                x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
        //                x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
        //                c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF" && mat.CampañaId == x.CampanaId).Sum(x => x.ToneladaFijada),

        //                RecibidoSinPrecio = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF")
        //                .Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),

        //                ARecibirAFijar = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.Centro != "8107" && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0 && x.FechaDesde < fechaCorteCampaña)
        //                    .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
        //                    ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 :
        //                    x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),

        //                FasonFas = c.Where(x => !String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)
        //            },
        //            DirectoAcopiador = new CompraDetalleDto
        //            {
        //                ComprasConPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc != "ZPAF" && mat.CampañaId == x.CampanaId).
        //                 Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
        //                x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
        //                x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
        //                c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF" && mat.CampañaId == x.CampanaId).Sum(x => x.ToneladaFijada),

        //                RecibidoSinPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF").Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),

        //                ARecibirAFijar = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.Centro != "8107" && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0 && x.FechaDesde < fechaCorteCampaña)
        //                    .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
        //                    ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),

        //                FasonFas = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)

        //            },
        //            DirectoProductor = new CompraDetalleDto
        //            {
        //                ComprasConPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc != "ZPAF" && mat.CampañaId == x.CampanaId).
        //                 Sum(x => x.ToneladaAplicada > x.ToneladaContrato ?
        //                x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
        //                x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) +
        //                c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF" && mat.CampañaId == x.CampanaId).Sum(x => x.ToneladaFijada),

        //                RecibidoSinPrecio = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF").Sum(x => (x.ToneladaAplicada - x.ToneladaFijada) < 0 ? 0 : x.ToneladaAplicada - x.ToneladaFijada),

        //                ARecibirAFijar = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.Centro != "8107" && x.ClaseDoc == "ZPAF" && x.PendienteAplicar > 0 && x.FechaDesde < fechaCorteCampaña)
        //                    .Sum(x => x.ToneladaFijada - x.ToneladaAplicada > 0 ?
        //                    ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar),

        //                FasonFas = c.Where(x => String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZFAZ").Sum(x => x.ToneladaContrato)
        //            },
        //            TotalCompra = c.Where(a => a.ClaseDoc == "ZFJ$").Sum(x => x.ToneladaContrato) + c.Where(a => a.ClaseDoc == "ZPAF").Sum(x => x.ToneladaFijada)
        //        };
        //        if (detalle.ConCorredor.ARecibirAFijar + detalle.ConCorredor.ComprasConPrecio + detalle.ConCorredor.FasonFas + detalle.ConCorredor.RecibidoSinPrecio
        //            + detalle.DirectoAcopiador.ARecibirAFijar + detalle.DirectoAcopiador.ComprasConPrecio + detalle.DirectoAcopiador.FasonFas + detalle.DirectoAcopiador.RecibidoSinPrecio
        //            + detalle.DirectoProductor.ARecibirAFijar + detalle.DirectoProductor.ComprasConPrecio + detalle.DirectoProductor.FasonFas + detalle.DirectoProductor.RecibidoSinPrecio
        //            > 0)
        //        {
        //            compraDto.Add(detalle);
        //        }
        //    }
        //    return compraDto;
        //}

        public List<CampanaMaterialDetallePorMeseExcelDto> TraerTodoCompraDetalleExcel(List<int> equipo)
        {
            logger.Info("TraerTodoCompraDetalleExcel get data");
            var compraDto = new List<CompraDto>();
            var proveedorIds = new List<int?>();
            var proveedores = repositorio.Listar<Proveedor>().ToList();
            var comerciales = repositorio.Listar<Comercial>().ToList();
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
                itemDto.Comercial = comerciales.FirstOrDefault(c => c.ComercialId == x.ComercialId).Nombres + ' ' + comerciales.FirstOrDefault(c => c.ComercialId == x.ComercialId).Apellido;

                if (((!String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc != "ZPAF") || (!String.IsNullOrEmpty(x.CorredorCuit) && x.ClaseDoc == "ZPAF")) ||
                    (String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc != "ZPAF") || (String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion != "PRODUCTOR" && x.ClaseDoc == "ZPAF") ||
                    (String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc != "ZPAF") || (String.IsNullOrEmpty(x.CorredorCuit) && x.Clasificacion == "PRODUCTOR" && x.ClaseDoc == "ZPAF")
                    )
                {
                    CampanaMaterialDetallePorMeseExcelDto cloned = (CampanaMaterialDetallePorMeseExcelDto)itemDto.Clone();
                    cloned.Situacion = "Compras Con Precio";
                    cloned.Tn = x.ClaseDoc != "ZPAF" ? (x.ToneladaAplicada > x.ToneladaContrato ?
                        x.ToneladaAplicada + x.ToneladaAmpliada - x.ToneladaAnulada :
                        x.ToneladaContrato + x.ToneladaAmpliada - x.ToneladaAnulada) : 0;
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
                    cloned.Tn = x.ToneladaFijada - x.ToneladaAplicada > 0 ? ((x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) < 0 ? 0 : x.PendienteAplicar - (x.ToneladaFijada - x.ToneladaAplicada)) : x.PendienteAplicar;
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

        private List<ContactoAll> DarFormato(List<ContactoAll> resultado)
        {
            var res = resultado.GroupBy(x => x.ProveedorId).ToList();
            var lista = new List<ContactoAll>();
            foreach (var item in res)
            {
                item.FirstOrDefault().ComercialACargo = string.Join("; ", item.Select(x => x.ComercialACargo).Distinct());
                lista.Add(item.FirstOrDefault());
            }
            return lista;
        }
    }
    public class FakeHome
    {
        public int Id { set; get; }
        public string Nombre { set; get; }
    }
}