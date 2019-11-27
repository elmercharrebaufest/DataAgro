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
                var re = DevolverContactosIni(repositorio.ListarConsulta(new TraerCorredoresComercial(equipo)));
                res.Contactos.AddRange(re);
            }

            oParam.Estado = null;
            if (!PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial))
            {
                var queryPorEstado = repositorio.SelStore<Contactos>("DataAgro_BusquedaContactos", 0, oParam.Campaña,
                oParam.Segmentacion, oParam.Actividad, oParam.Material, oParam.Calificacion,
                oParam.Hectareas, oParam.Toneladas, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParam.Condicion, oParam.Estado, oParam.Comercial, oParam.Zona);

                res.TotalContactos = queryPorEstado.Count();
                res.TotalPotencialContactos = queryPorEstado.Count(x => x.Estado == "Cliente Potencial");
                res.TotalOperandoContactos = queryPorEstado.Count(x => x.Estado == "Operando");
                res.TotalNoOperandoContactos = queryPorEstado.Count(x => x.Estado == "No operando");
                res.TotalBajaContactos = queryPorEstado.Count(x => x.Estado == "Baja");
                res.TotalSinInteresContactos = queryPorEstado.Count(x => x.Estado == "Sin interés de operar");
            }
            else
            {
                var cuenta = DevolverContactosIni(repositorio.ListarConsulta(new TraerCorredoresComercial(equipo)));
                res.TotalContactos = cuenta.Count();
                res.TotalPotencialContactos = cuenta.Count(x => x.Estado == "Cliente Potencial");
                res.TotalOperandoContactos = cuenta.Count(x => x.Estado == "Operando");
                res.TotalNoOperandoContactos = cuenta.Count(x => x.Estado == "No operando");
                res.TotalBajaContactos = cuenta.Count(x => x.Estado == "Baja");
                res.TotalSinInteresContactos = cuenta.Count(x => x.Estado == "Sin interés de operar");
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
            return query;
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
                    var comerciales = repositorio.Listar<Comercial,int>(x=>x.ComercialId,x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso==PermisosDataAgro.VerCorredorComercial)));

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

                return exp;
            }
            else
            {
                return new ExportAll();
            }
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

        public ResultIniContacto TraerBusquedaContactoCorredores(List<int> comercialesId)
        {
            var lista = repositorio.ListarConsulta(new TraerCorredoresComercial(comercialesId));
            return null;
        }


    }


    public class FakeHome
    {
        public int Id { set; get; }
        public string Nombre { set; get; }
    }
}
