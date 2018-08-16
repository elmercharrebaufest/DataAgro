using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
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
        private ILogger logger;

        public HomeManager(ILogger logger, IRepositorio repositorio, ICampañaManager campañaManager)
        {
            this.logger = logger;
            this.mobCampaña = campañaManager;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------
        
        public ResultIniContacto TraerBusquedaContacto(oParamBusqueda oParam, int pagina)
        {
            var res = new ResultIniContacto();

            var query = repositorio.SelStorePaginado<Contactos>("DataAgro_BusquedaContactos", 50, pagina, oParam.Campaña,
                oParam.Segmentacion, oParam.Actividad, oParam.Material, oParam.Calificacion,
                oParam.Hectareas, oParam.Toneladas, oParam.ComercialId, oParam.Condicion, oParam.Estado, oParam.Comercial, oParam.Zona).OrderBy(x=>x.RazonSocial);

            res.Contactos = DevolverContactosIni(query.ToList());
            res.TotalContactos = repositorio.Contar<ProveedorComercial>(x => oParam.Equipo.Contains(x.ComercialId));
            res.TotalPotencialContactos = repositorio.Contar<ProveedorComercial>(x => oParam.Equipo.Contains(x.ComercialId) && x.Proveedor.Estado.EstadoId == 1);
            res.TotalOperandoContactos = repositorio.Contar<ProveedorComercial>(x => oParam.Equipo.Contains(x.ComercialId) && x.Proveedor.Estado.EstadoId == 2);
            res.TotalNoOperandoContactos = repositorio.Contar<ProveedorComercial>(x => oParam.Equipo.Contains(x.ComercialId) && x.Proveedor.Estado.EstadoId == 3);
            res.TotalBajaContactos = repositorio.Contar<ProveedorComercial>(x => oParam.Equipo.Contains(x.ComercialId) && x.Proveedor.Estado.EstadoId == 4);
            res.TotalSinInteresContactos = repositorio.Contar<ProveedorComercial>(x => oParam.Equipo.Contains(x.ComercialId) && x.Proveedor.Estado.EstadoId == 5);
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
            if (!String.IsNullOrEmpty(con.Situacion))
            {
                if ((con.Situacion.ToLower() == ConfigurationManager.AppSettings["SitNoIncluida"]) 
                    || (con.Situacion.ToLower() == ConfigurationManager.AppSettings["SitExcluido"])
                        || (con.Situacion.ToLower() == ConfigurationManager.AppSettings["SitSuspendido"]))
                {
                    cont.NoOperable = true;
                    cont.Operando = false;
                    cont.TooltipNoOperable = con.Situacion;
                }
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

        public int TraerIdComercial(string idActiveDirectory)
        {
            return repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == idActiveDirectory, x => x.ComercialId);
        }

        public DatosIniciales TraerInfoIniciales(List<int> equipo)
        {
            var DatosIni = new DatosIniciales
            {
                camp = repositorio.Listar<Campaña, CampañaQry>(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion },null, 0, "CampañaId", Entities.Helpers.DirOrden.Desc),

                mat = repositorio.Listar<Material, MaterialesQry>(x => new MaterialesQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion }),

                segm = repositorio.Listar<Segmentacion, SegmentacionQry>(x => new SegmentacionQry() { SegmentacionId = x.SegmentacionId, Descripcion = x.Descripcion, Grupo = x.Grupo }),

                tipoact = repositorio.Listar<TipoActividad, TipoActividadQry>(x => new TipoActividadQry() { TipoActividadId = x.TipoActividadId, Descripcion = x.Descripcion }),

                est = repositorio.Listar<Estado, EstadoQry>(x => new EstadoQry() { EstadoId = x.EstadoId, Descripcion = x.Descripcion }),

                cond = repositorio.Listar<Condicion, CondicionPreferenteQry>(x => new CondicionPreferenteQry() { CondicionId = x.CondicionId, Descripcion = x.Descripcion }),

                come = repositorio.Listar<Comercial, ComercialQry>(s => new ComercialQry() { ComercialId = s.ComercialId, IdActiveDirectory = s.IdActiveDirectory }, x => equipo.Contains(x.ComercialId)),

                zona = repositorio.Listar<Comercial, ZonaQry>(x => new ZonaQry { ZonaId = x.GrupoDeCompras.Id, Descripcion = x.GrupoDeCompras.Descripcion }, x => equipo.Contains(x.ComercialId)&&x.GrupoDeCompras!=null, 0, "Descripcion")
            };
            return DatosIni;

        }

        public List<BusquedaHome> BusquedaHome(string filtro, int ComercialId)
        {
            var query = repositorio.SelStore<BusquedaHome>("DataAgro_BusquedaHome", 0, filtro, ComercialId);
            return query.ToList();
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

            return list;
        }

        public List<ContactoIni> ExportarContactos(List<int> Ids, string idActiveDirectory)
        {

            var oComerciales = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory.ToLower() == idActiveDirectory.ToLower());

            if (oComerciales != null)
            {
                var Contactos = repositorio.SelStore<Contactos>("DataAgro_Contactos_Exportar", 0, string.Join(",", Ids.Select(n => n.ToString()).ToArray()), oComerciales.ComercialId);

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

        public ExportAll ExportarAll(List<int> Ids, string idActiveDirectory)
        {
            ExportAll exp = new ExportAll();

            var oComerciales = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory.ToLower() == idActiveDirectory.ToLower());

            if (oComerciales != null)
            {
                exp.contacto = repositorio.SelStore<ContactoAll>("DataAgro_ExportAll_Contacto", 0, string.Join(",", Ids.Select(n => n.ToString()).ToArray()), oComerciales.ComercialId);

                exp.objetivo = repositorio.SelStore<ObjetivoAll>("DataAgro_ExportAll_Objetivos", 0, string.Join(",", Ids.Select(n => n.ToString()).ToArray()));

                exp.ContactosPrincipales = repositorio.SelStore<ContactosPrincipalesAll>("DataAgro_ExportAll_ContactosPrincipales", 0, string.Join(",", Ids.Select(n => n.ToString()).ToArray()));

                exp.produccion = repositorio.SelStore<ProduccionAll>("DataAgro_ExportAll_Produccion", 0, string.Join(",", Ids.Select(n => n.ToString()).ToArray()));

                exp.almacenamiento = repositorio.SelStore<AlmacenamientoAll>("DataAgro_ExportAll_Almacenamiento", 0, string.Join(",", Ids.Select(n => n.ToString()).ToArray()));

                exp.agenda = repositorio.SelStore<AgendaAll>("DataAgro_ExportAll_Actividades", 0, string.Join(",", Ids.Select(n => n.ToString()).ToArray()));

                exp.compras = repositorio.SelStore<ComprasAll>("DataAgro_ExportAll_Compras", 0, string.Join(",", Ids.Select(n => n.ToString()).ToArray()));

                return exp;
            }
            else
            {
                return new ExportAll();
            }
        }
        
        public PostItDto TraerTexto(int idComercial)
        {
            return repositorio.Obtener<PostIt, PostItDto>(x => x.ComercialId == idComercial, x => new PostItDto { ComercialId = x.ComercialId, Texto = x.Texto});
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


    }


    public class FakeHome
    {
        public int Id { set; get; }
        public string Nombre { set; get; }
    }
}
