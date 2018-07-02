using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class HomeManager : IHomeManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ICampañaManager mobCampaña;
        private IEstadoProveedorManager mobEstado;
        private ILogger logger;

        public HomeManager(ILogger logger, IMSContextProvider oMSContextProvider, IEstadoProveedorManager mobEstado, ICampañaManager campañaManager)
        {
            this.logger = logger;
            this.mobEstado = mobEstado;
            this.mobCampaña = campañaManager;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<ResultIniContacto> TraerTodoContactoAsync(int idComercial)
        {
            var res = new ResultIniContacto();

            var query = mobjUnitOfWork.SelStore<Contactos>("DataAgro_TraerContactos", idComercial);

            res.Contactos = DevolverContactosIni(query.ToList());

            return res;
        }

        public async Task<ResultIniContacto> TraerBusquedaContactoAsync(oParamBusqueda oParam)
        {
            var res = new ResultIniContacto();

            var query = mobjUnitOfWork.SelStore<Contactos>("DataAgro_BusquedaContactos", oParam.Campaña,
                oParam.Segmentacion, oParam.Actividad, oParam.Material, oParam.Calificacion,
                oParam.Hectareas, oParam.Toneladas, oParam.ComercialId, oParam.Condicion,oParam.Comercial,oParam.Zona);

            res.Contactos = DevolverContactosIni(query.ToList());

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

        public async Task<CampañaHome> TraerInfoCampañaAsync(int idComercial)
        {
            return await mobCampaña.TraerCampañaHomeAsync(idComercial);
        }

        public async Task<int> TraerIdComercial(string idActiveDirectory)
        {
            var oComercial = new Comercial();

            oComercial = await mobjUnitOfWork.Repository<Comercial>()
                                .Queryable()
                                .AsNoTracking()
                                .Where(x => x.IdActiveDirectory == idActiveDirectory)
                                .SingleOrDefaultAsync();

            if (oComercial != null)
            {
                return oComercial.ComercialId;
            }

            return 0;
        }

        public async Task<DatosIniciales> TraerInfoInicialesAsync(int comercialId)
        {
            var DatosIni = new DatosIniciales();

            var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();

            DatosIni.camp = await mobjUnitOfWork.Repository<Campaña>()
                                .Queryable()
                                .AsNoTracking()
                                .OrderByDescending(x=> x.CampañaId)
                                .Select(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion }).ToListAsync();

            DatosIni.mat = await mobjUnitOfWork.Repository<Material>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new MaterialesQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion }).ToListAsync();

            DatosIni.segm = await mobjUnitOfWork.Repository<Segmentacion>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new SegmentacionQry() { SegmentacionId = x.SegmentacionId, Descripcion = x.Descripcion, Grupo = x.Grupo }).ToListAsync();

            DatosIni.tipoact = await mobjUnitOfWork.Repository<TipoActividad>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new TipoActividadQry() { TipoActividadId = x.TipoActividadId, Descripcion = x.Descripcion }).ToListAsync();

            DatosIni.est = await mobjUnitOfWork.Repository<Estado>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new EstadoQry() { EstadoId = x.EstadoId, Descripcion = x.Descripcion }).ToListAsync();

            DatosIni.cond = await mobjUnitOfWork.Repository<Condicion>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new CondicionPreferenteQry() { CondicionId = x.CondicionId, Descripcion = x.Descripcion }).ToListAsync();

            var query = mobjUnitOfWork.SelStore<FakeHome>("DataAgro_Comercial_TraerPorComerciales", comercialId);
            DatosIni.come = query.ToList().Select(s => new ComercialQry() {ComercialId = s.Id,IdActiveDirectory=s.Nombre }).ToList();

            query = mobjUnitOfWork.SelStore<FakeHome>("DataAgro_Zona_TraerPorComerciales", comercialId);
            DatosIni.zona = query.ToList().Select(s => new ZonaQry() { ZonaId = s.Id, Descripcion = s.Nombre }).ToList();

            return DatosIni;

        }

        public async Task<List<BusquedaHome>> BusquedaHome(string filtro,int ComercialId)
        {
            var query = mobjUnitOfWork.SelStore<BusquedaHome>("DataAgro_BusquedaHome", filtro, ComercialId);
            return query.ToList(); 
        }
        //public async Task<List<BusquedaHome>> BusquedaHome(string filtro)
        //{
        //    var query = mobjUnitOfWork.SelStore<BusquedaHome>("DataAgro_BusquedaProveedores", filtro);
        //    return query.ToList();
        //}
        public async Task<List<ActividadRecordatorio>> TraerActividadesPorComercialId(int ComercialId)
        {
            var oActividad = mobjUnitOfWork.Repository<Actividad>().Queryable();
            var oTipoActividad = mobjUnitOfWork.Repository<TipoActividad>().Queryable();
            var oContacto = mobjUnitOfWork.Repository<ContactoComercial>().Queryable();

            var list = new List<ActividadRecordatorio>();
            ActividadRecordatorio act = null;

            var query = oActividad
                        .Join(oTipoActividad, a => a.TipoActividadId, b => b.TipoActividadId, (a, b) => new { AC = a, TA = b })
                        .Join(oContacto,a=> a.AC.ContactoComercialId, b => b.ContactoComercialId, (a, b) => new { a.AC, a.TA , CO = b })
                        .Where(x => x.AC.ComercialId == ComercialId && x.AC.FechaHoraRecordatorio.HasValue
                                && x.AC.FechaHoraRecordatorio.Value >= DateTime.Now)
                        .Select(x => new ActividadRecordatorioGrid
                        {
                            ActividadId = x.AC.ActividadId,
                            Comentarios = x.AC.Detalle,
                            FechaRecordatorio = x.AC.FechaHoraRecordatorio.Value,
                            Tema = x.TA.Descripcion,
                            Contacto = x.CO.Nombres + " " + x.CO.Apellido,
                            ProveedorId = x.CO.ProveedorId
                        });


            var result = await query.ToListAsync();

            result = result.OrderBy(x => x.FechaRecordatorio).ToList();

            foreach (var item in result)
            {
                act = new ActividadRecordatorio();
                act.ActividadId = item.ActividadId;
                act.Comentarios = item.Comentarios;
                act.Contacto = item.Contacto;
                act.Tema = item.Tema;
                if (item.FechaRecordatorio.Date == DateTime.Now.Date)
                    act.Dia = "Hoy ";
                else
                    act.Dia = item.FechaRecordatorio.ToString("dd/MM/yyyy") + " ";

                act.Hora = item.FechaRecordatorio.ToString("HH:mm");
                act.ProveedorId = item.ProveedorId;
                list.Add(act);
            }

            return list;

        }

        public async Task<List<ContactoIni>> ExportarContactos(List<int> Ids,string idActiveDirectory)
        {

            var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.IdActiveDirectory.ToLower() == idActiveDirectory.ToLower());

            if (oComerciales != null)
            {
                var query = mobjUnitOfWork.SelStoreAsync<Contactos>("DataAgro_Contactos_Exportar", string.Join(",", Ids.Select(n => n.ToString()).ToArray()), oComerciales.ComercialId);
                var Contactos = await query.ToListAsync();

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

        public async Task<ExportAll> ExportarAll(List<int> Ids, string idActiveDirectory)
        {
            ExportAll exp = new ExportAll();

            var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.IdActiveDirectory.ToLower() == idActiveDirectory.ToLower());

            if (oComerciales != null)
            {
                var Contacto = mobjUnitOfWork.SelStoreAsync<ContactoAll>("DataAgro_ExportAll_Contacto", string.Join(",", Ids.Select(n => n.ToString()).ToArray()), oComerciales.ComercialId);
                exp.contacto = await Contacto.ToListAsync();

                var Objetivo = mobjUnitOfWork.SelStoreAsync<ObjetivoAll>("DataAgro_ExportAll_Objetivos", string.Join(",", Ids.Select(n => n.ToString()).ToArray()));
                exp.objetivo = await Objetivo.ToListAsync();

                var ContactoPrincipal = mobjUnitOfWork.SelStoreAsync<ContactosPrincipalesAll>("DataAgro_ExportAll_ContactosPrincipales", string.Join(",", Ids.Select(n => n.ToString()).ToArray()));
                exp.ContactosPrincipales = await ContactoPrincipal.ToListAsync();

                var produccion = mobjUnitOfWork.SelStoreAsync<ProduccionAll>("DataAgro_ExportAll_Produccion", string.Join(",", Ids.Select(n => n.ToString()).ToArray()));
                exp.produccion = await produccion.ToListAsync();

                var almacenamiento = mobjUnitOfWork.SelStoreAsync<AlmacenamientoAll>("DataAgro_ExportAll_Almacenamiento", string.Join(",", Ids.Select(n => n.ToString()).ToArray()));
                exp.almacenamiento = await almacenamiento.ToListAsync();

                var agenda = mobjUnitOfWork.SelStoreAsync<AgendaAll>("DataAgro_ExportAll_Actividades", string.Join(",", Ids.Select(n => n.ToString()).ToArray()));
                exp.agenda = await agenda.ToListAsync();

                var compras = mobjUnitOfWork.SelStoreAsync<ComprasAll>("DataAgro_ExportAll_Compras", string.Join(",", Ids.Select(n => n.ToString()).ToArray()));
                exp.compras = await compras.ToListAsync();

                return exp;
            }
            else
                return new ExportAll();
        }


        public async Task<PostIt> TraerTextoAsync(int idComercial)
        {
            var oPos = mobjUnitOfWork.Repository<PostIt>().Queryable().AsNoTracking();

            var post = await oPos
                .Where(x => x.ComercialId == idComercial)
                .FirstOrDefaultAsync();

            return post;
        }

        public async Task<GrabarPostItResult> GuardarPostItAsync(PostIt post)
        {
            var oEntityErrors = new GrabarPostItResult();
            oEntityErrors.errores = new EntityErrors();
            try
            { 
                var oPos = mobjUnitOfWork.Repository<PostIt>().Queryable();

                var postit = await oPos
                    .Where(x => x.ComercialId == post.ComercialId)
                    .SingleOrDefaultAsync();

                if (postit == null)
                {
                    postit = new PostIt()
                    {
                        ComercialId = post.ComercialId,
                        ObjectState = Constants.Object_Added
                    };
                }
                else
                    postit.ObjectState = Constants.Object_Modified;



                postit.Texto = post.Texto;
                mobjUnitOfWork.Repository<PostIt>().SaveEntity(postit);
                mobjUnitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                oEntityErrors.errores= new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
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
