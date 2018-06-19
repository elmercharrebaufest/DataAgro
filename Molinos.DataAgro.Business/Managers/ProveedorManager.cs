using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Agent.Compras;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.DirectoryServices;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.Common.Enums;
using static Mastersoft.Framework.Standard.Constantes;

namespace Molinos.DataAgro.Business.Managers
{


    public class ProveedorManager : IProveedorManager
    {
        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;
        private IComercialManager mobComercial;
        private ICampañaMaterial mobCampañaMaterial;

        ComercialManager mobjComercialManager = new ComercialManager();


        public void Inicializar(MSContext oContexto)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = new UnitOfWork(oContexto, new DataAgroContext(oContexto));
        }

        public void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = oUnitOfWork;
        }

        public void Inicializar(MSContext oContexto,  IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            mobjContexto = oContexto;
            //mobjUnitOfWork = oUnitOfWork;
            mobComercial = oComercial;
            mobCampañaMaterial = oCampañaMaterial;

        }


        public async Task<StoredHistorialResult> TraerHistorialActividad(HistorialActiviad oParam, int ProveedorId, string TipoActividadId)
        {
            var res = new StoredHistorialResult();

            var actividadhistoria = mobjUnitOfWork.SelStore<HistorialTraer>("DataAgro_ActividadHistoriaTraerPorProveedorId", ProveedorId, oParam.detalle, TipoActividadId).ToList();

            res.ActividadHistoriaTraerPorProveedores = actividadhistoria.ToList();

            return res;
        }

        public async Task<StoredPorProveedorResult> TraerProveedor(int ProveedorId, string UsuarioDirectory)
        {
            var res = new StoredPorProveedorResult();

            var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.IdActiveDirectory.ToLower() == UsuarioDirectory.ToLower());


            //canales de operacion, ProveedorDestinatario y ProveedorCondicion

            try
            {
                var actividad = mobjUnitOfWork.SelStore<ActividadTraer>("DataAgro_ActividadTraerPorProveedorId", ProveedorId).ToList();

                var basico = mobjUnitOfWork.SelStore<BasicoProveedor>("DataAgro_BasicoProveedorTraerPorProveedorId", ProveedorId, oComerciales.ComercialId).ToList();
                var contactoscomercial = mobjUnitOfWork.SelStore<ContactosComerciales>("DataAgro_ContactosComercialesTraerPorProveedorId", ProveedorId).ToList();
                var campoacopio = mobjUnitOfWork.SelStore<CampoProduccionAcopio>("DataAgro_CampoProduccionAcopioPorProveedorId", ProveedorId).ToList();
                var actividadhistoria = mobjUnitOfWork.SelStore<ActividadTraer>("DataAgro_ActividadHistoriaTraerPorProveedorId", ProveedorId, null, null).ToList();
                var objetivos = mobjUnitOfWork.SelStore<ObjetivosTraer>("DataAgro_ObjetivosTraerPorProveedorId", ProveedorId).ToList();
                var acopiomateriales = mobjUnitOfWork.SelStore<AcopioMaterialPorProveedor>("DataAgro_AcopioMaterialPorProveedorId", ProveedorId).ToList();

                res.ActividadTraerPorProveedores = actividad.ToList();
                res.ActividadHistoriaTraerPorProveedores = actividadhistoria.ToList();
                res.BasicoProveedorTraerPorProveedores = basico.ToList();
                res.ContactosComercialesTraerPorProveedores = contactoscomercial.ToList();
                res.CampoProduccionAcopioPorProveedores = campoacopio.Where(z => z.EsCampoProduccion == true).ToList();
                res.Acopio = campoacopio.Where(z => z.EsCampoProduccion == false).ToList();
                res.DatosContacto = await DevolverDatosContacto(ProveedorId);
                res.Historial = DevolverHistorial(ProveedorId, UsuarioDirectory);
                res.CanalesDeOperacion = DevolverCanalesDeOperacionPorProveedor(ProveedorId);
                res.ProveedorCondicion = DevolverProveedorCondicionPorProveedor(ProveedorId);
                res.ProveedorDestinatario = DevolverProveedorDestinatarioPorProveedor(ProveedorId);
                res.ObjetivosTraerPorProveedorId = objetivos;
                res.AcopioMaterialPorProveedores = acopiomateriales;
            }
            catch (Exception ex)
            {
                throw;
            }


            foreach (var aux in res.BasicoProveedorTraerPorProveedores)
            {
                aux.NoOperable = false;
                aux.Operando = true;


                if (!String.IsNullOrEmpty(aux.RiesgoComercialSap))
                {
                    if (aux.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                    {
                        aux.NoOperable = true;
                        aux.Operando = false;
                        aux.TooltipNoOperable = "Riesgo Comercial Alto";
                    }
                }

                if (!String.IsNullOrEmpty(aux.Situacion))
                {
                    if ((aux.Situacion.ToLower() == ConfigurationManager.AppSettings["SitNoIncluida"])
                         || (aux.Situacion.ToLower() == ConfigurationManager.AppSettings["SitExcluido"])
                             || (aux.Situacion.ToLower() == ConfigurationManager.AppSettings["SitSuspendido"]))
                    {
                        aux.NoOperable = true;
                        aux.Operando = false;
                        aux.TooltipNoOperable = aux.Situacion;
                    }
                }
                if (aux.Facacop == 1)
                {
                    aux.Operando = false;
                    aux.NoOperable = true;
                    aux.TooltipNoOperable = "Apocrifos";
                }
            }

            return res;
        }

        private List<Destinatario> DevolverProveedorDestinatarioPorProveedor(int proveedorId)
        {
            var res = new List<Destinatario>();
            var oProveedores = mobjUnitOfWork.Repository<ProveedorDestinatario>().Queryable().AsNoTracking();
            var oDestinatario = mobjUnitOfWork.Repository<Destinatario>().Queryable().AsNoTracking();

            res = oProveedores
                        .Join(oDestinatario, a => a.DestinatarioId, b => b.DestinatarioId, (a, b) => new { P = a, CO = b })
                        .Where(x => x.P.ProveedorId == proveedorId)
                        .Select(x => x.CO).ToList();
            return res;
        }

        private List<Condicion> DevolverProveedorCondicionPorProveedor(int proveedorId)
        {
            var res = new List<Condicion>();
            var oProveedores = mobjUnitOfWork.Repository<ProveedorCondicion>().Queryable().AsNoTracking();
            var oCondicion = mobjUnitOfWork.Repository<Condicion>().Queryable().AsNoTracking();

            res = oProveedores
                        .Join(oCondicion, a => a.CondicionId, b => b.CondicionId, (a, b) => new { P = a, CO = b })
                        .Where(x => x.P.ProveedorId == proveedorId)
                        .Select(x => x.CO).ToList();
            return res;
        }

        private List<CanalOperacion> DevolverCanalesDeOperacionPorProveedor(int proveedorId)
        {
            var res = new List<CanalOperacion>();
            var oProveedores = mobjUnitOfWork.Repository<ProveedorCanalOperacion>().Queryable().AsNoTracking();
            var oCanalOperacion = mobjUnitOfWork.Repository<CanalOperacion>().Queryable().AsNoTracking();

            res = oProveedores
                        .Join(oCanalOperacion, a => a.CanalOperacionId, b => b.CanalOperacionId, (a, b) => new { P = a, CO = b })
                        .Where(x => x.P.ProveedorId == proveedorId)
                        .Select(x => x.CO).ToList();
            return res;
        }

        public async Task<Actividad> TraerRecordatorioAsync(int ActividadId)
        {
            var oActividad = new Actividad();

            oActividad = await mobjUnitOfWork.Repository<Actividad>()
                                 .Queryable()
                                 .Where(x => x.ActividadId == ActividadId)
                                 .SingleOrDefaultAsync();

            if (oActividad == null)
            {
                oActividad = new Actividad()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oActividad.ObjectState = Constants.Object_Modified;
            }

            return oActividad;
        }

        public async Task<DatosLocalidadProvincia> TraerLocalidadProveedorPorCuitAsync(string CUIT)
        {
            var valor = new DatosLocalidadProvincia();

            valor = mobjUnitOfWork.SelStore<DatosLocalidadProvincia>("DataAgro_BasicoProveedorTraerPorCuit", CUIT).SingleOrDefault();

            return valor;
        }


        public async Task<DatosLocalidadProvincia> TraerLocalidadProveedorPorCuitAsync(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro)
        {
            var valor = new DatosLocalidadProvincia();
            try {

                var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().FirstOrDefault(x => x.CUIT == oDatosLocalidadProvinciaFiltro.CUIT);

                var oCampoIdAx = mobjUnitOfWork.Repository<Campo>().Queryable().AsNoTracking().Where(x => x.ProveedorId == oProveedor.ProveedorId).Select(z => z.CampoId);

                var oCampoMaterial = mobjUnitOfWork.Repository<CampoMaterial>().Queryable().AsNoTracking().Where(x => oCampoIdAx.Contains(x.CampoId) && x.MaterialId == oDatosLocalidadProvinciaFiltro.MaterialId && x.CampañaId == oDatosLocalidadProvinciaFiltro.CampanaId).Select(z => z.CampoId);

                int? localidadId = mobjUnitOfWork.Repository<Campo>().Queryable().AsNoTracking().Where(x => oCampoMaterial.Contains(x.CampoId)).Select(z => z.LocalidadId).FirstOrDefault();

                if (localidadId != null) {
                    valor = mobjUnitOfWork.Repository<Localidad>().Queryable().AsNoTracking()
                        .Where(x => x.LocalidadId == localidadId)
                        .Join(mobjUnitOfWork.Repository<Provincia>().Queryable().AsNoTracking(), a => a.ProvinciaId, b => b.ProvinciaId, (a, b) => new DatosLocalidadProvincia { CUIT = oProveedor.CUIT, Localidad = a.Nombre, LocalidadId = a.LocalidadId, Provincia = b.Nombre, ProvinciaId = b.ProvinciaId, ProveedorId = oProveedor.ProveedorId, RazonSocial = oProveedor.RazonSocial }).FirstOrDefault();
                }
            }
            catch
            {
            }
            
            return valor;
        }

        private async Task<DatosContacto> DevolverDatosContacto(int proveedorId)
        {
            var valor = new DatosContacto();
            //valor.CondicionesPreferentesId = await mobjUnitOfWork.Repository<Condicion>()
            //                   .Queryable()
            //                   .AsNoTracking()
            //                   .Where(x => x.)
            //                   .Select(x =>  x.CondicionId ).ToListAsync();

            valor = await mobjUnitOfWork.Repository<Proveedor>()
                               .Queryable()
                               .AsNoTracking()
                               .Where(x => x.ProveedorId == proveedorId)
                               .Select(x => new DatosContacto()
                               {
                                   Direccion = x.Direccion,
                                   CodigoPostal = x.CodigoPostal,
                                   Intermediario = x.Intermediario
                               }).FirstOrDefaultAsync();

            return valor;
        }

        public async Task<EntityErrors> GrabarRecordatorioAsync(ActividadInsetarIni oParam)
        {
            var oEntityErrors = new EntityErrors();

            Actividad oActividadSave;

            if (oParam.ActividadId == 0)
            {
                oActividadSave = new Actividad()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oActividadSave = await TraerRecordatorioAsync(oParam.ActividadId);
            }

            oActividadSave.ComercialId = oParam.ComercialId;
            oActividadSave.ContactoComercialId = oParam.contacto;  // ToDo cambiar por el id del contact
            oActividadSave.Detalle = oParam.detalle;
            oActividadSave.FechaHoraActividad = oParam.fechaYHoraActividad;
            oActividadSave.FechaHoraRecordatorio = oParam.fechaYHoraRecordatorio;
            oActividadSave.ProveedorId = oParam.ProveedorId;
            oActividadSave.TipoActividadId = oParam.tipoactividad;
            oActividadSave.asunto = oParam.asunto;
            oActividadSave.FechaHoraRecordatorioFin = oParam.fechaYHoraRecordatorioFin;

            if (oActividadSave.ObjectState == Constants.Object_Added)
            {
                oActividadSave.ActividadId = ((mobjUnitOfWork.Repository<Actividad>().Queryable().Max(x => (int?)x.ActividadId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<Actividad>().SaveEntity(oActividadSave);

            var proveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().FirstOrDefault(x => x.ProveedorId == oParam.ProveedorId);
            proveedor.ObjectState = Constants.Object_Modified;
            proveedor.FechaUltimoContacto = DateTime.Now;
            mobjUnitOfWork.Repository<Proveedor>().SaveEntity(proveedor);


            try
            {


                await mobjUnitOfWork.SaveChangesAsync();



                if ((oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaCita"]))
                    /*|| (oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaTareas"]))*/)
                {
                    EnviarCita(oParam);
                }
            }
            catch (Exception ex)
            {

                throw;
            }



            return oEntityErrors;
        }

        private void EnviarCita(ActividadInsetarIni oParam)
        {
            try
            {
                var oContacto = mobjUnitOfWork.Repository<ContactoComercial>()
                                 .Queryable().AsNoTracking();

                var resContacto = oContacto.Where(x => x.ContactoComercialId == oParam.contacto).FirstOrDefault();

                MailMessage oMensaje = new MailMessage();

                oMensaje.From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"]);

                if (ConfigurationManager.AppSettings["EmailAgenda"] == "1")
                    oMensaje.To.Add(ConfigurationManager.AppSettings["maildeUsuarios"]);
                else
                    oMensaje.To.Add(GetEmailUserActiveDirectory(oParam.UserName));


                if ((oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaCita"]))
                    || ((oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaTareas"]))))
                {
                    oMensaje.Subject = oParam.asunto;
                }
                
                var proveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking().FirstOrDefault(x => x.ProveedorId == oParam.ProveedorId);
                var comercial = mobjUnitOfWork.Repository<ContactoComercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.ProveedorId == oParam.ProveedorId && x.ContactoComercialId == oParam.contacto);
                

                oMensaje.Body = "CUIT: " + proveedor.CUIT + "\r\nRazón Social: " + proveedor.RazonSocial;
                if (comercial != null)
                {
                    oMensaje.Body = oMensaje.Body + "\r\nContacto: " + (!string.IsNullOrEmpty(comercial.Nombres) ? comercial.Nombres : "") + " " + (!string.IsNullOrEmpty(comercial.Apellido) ? comercial.Apellido : "");
                    oMensaje.Body = oMensaje.Body + (comercial.Telefono1 != null ? " Tel: " + comercial.Telefono1 : "");
                    oMensaje.Body = oMensaje.Body + (!string.IsNullOrEmpty(comercial.Email1) ? " Email: " + comercial.Email1 : "");
                }
                oMensaje.Body = oMensaje.Body + "\r\nDetalle: " + oParam.detalle;

                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                // Now Contruct the ICS file using string builder
                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine("PRODID:-//Schedule a Meeting");
                str.AppendLine("VERSION:2.0");
                str.AppendLine("METHOD:REQUEST");
                str.AppendLine("BEGIN:VEVENT");
                //validar para las llamadas
                if (oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaCita"]))
                {
                    str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", oParam.fechaYHoraRecordatorio.Value.ToUniversalTime()));
                    str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", oParam.fechaYHoraRecordatorio.Value.ToUniversalTime()));
                    str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", oParam.fechaYHoraRecordatorioFin.Value.ToUniversalTime().AddMinutes(+30)));
                }
                else if (oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaTareas"]))
                {
                    str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", DateTime.Now.ToUniversalTime()));
                    str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.Now.ToUniversalTime()));
                    str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", DateTime.Now.ToUniversalTime().AddMinutes(+30)));
                }

                str.AppendLine("LOCATION: " + "");
                str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
                str.AppendLine(string.Format("DESCRIPTION:{0}", oMensaje.Body));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", oMensaje.Body));
                str.AppendLine(string.Format("SUMMARY:{0}", oMensaje.Subject));
                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", oMensaje.From.Address));

                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", oMensaje.To[0].DisplayName, oMensaje.To[0].Address));

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");

                System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
                contype.Parameters.Add("method", "REQUEST");
                contype.Parameters.Add("name", "Recordatorio.ics");
                AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
                oMensaje.AlternateViews.Add(avCal);

                SmtpClient oCliente = default(SmtpClient);

                int Condicion = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["SmtpServerPort"], out Condicion))
                    oCliente = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
                else
                    oCliente = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);

                if (ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S")
                    oCliente.UseDefaultCredentials = true;
                else
                    oCliente.UseDefaultCredentials = false;


                if (ConfigurationManager.AppSettings["EnableSSL"] == "S")
                    oCliente.EnableSsl = true;
                else
                    oCliente.EnableSsl = false;

                oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                        ConfigurationManager.AppSettings["CredentialPassword"]);

                oCliente.Send(oMensaje);

            }
            catch (Exception ex)
            {
                var a = 1;
            }

        }

        public void EnviarEmail(Contrato oContrato, string idActiveDirectory)
        {
            try
            {
                Contrato contrato = mobjUnitOfWork.Repository<Contrato>().Queryable().FirstOrDefault(x => x.ContratoId == oContrato.ContratoId);

                ContactoComercial proveedorContacto = mobjUnitOfWork.Repository<ContactoComercial>().Queryable().FirstOrDefault(x => x.ProveedorId == contrato.ProveedorId);

                Comercial comercial = mobjUnitOfWork.Repository<Comercial>().Queryable().FirstOrDefault(x => x.ComercialId == contrato.ComercialId);

                string emailComercial = "";
                string emailJefe = "";

                if (comercial != null)
                {
                    try { emailComercial = GetEmailUserActiveDirectory(comercial.IdActiveDirectory); } catch { }

                    Comercial jefeComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault( x=> x.ComercialId == comercial.EmpleadorACargo);

                    try { emailJefe = GetEmailUserActiveDirectory(jefeComercial.IdActiveDirectory); } catch { }

                }

                if ((proveedorContacto != null && proveedorContacto.Email1 != "" && proveedorContacto.Email1 != null) || (emailJefe != null && emailJefe != "") || (emailComercial != null && emailComercial == "")) {
                    MailMessage oMensaje = new MailMessage();

                    oMensaje.From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"]);

                    oMensaje.To.Add(proveedorContacto.Email1);
                    if(emailJefe != "" && emailJefe != null) oMensaje.CC.Add(emailJefe);
                    if(emailComercial != "" && emailComercial != null) oMensaje.CC.Add(emailComercial);
                    
                    TipoNegocio tipoNegocio = mobjUnitOfWork.Repository<TipoNegocio>().Queryable().AsNoTracking().FirstOrDefault(x => x.TipoNegocioId == oContrato.TipoNegocioId);
                    Material material = mobjUnitOfWork.Repository<Material>().Queryable().AsNoTracking().FirstOrDefault(x => x.MaterialId == oContrato.MaterialId);
                    Moneda moneda = mobjUnitOfWork.Repository<Moneda>().Queryable().AsNoTracking().FirstOrDefault(x => x.MonedaId == oContrato.MonedaId);
                    Campaña campania = mobjUnitOfWork.Repository<Campaña>().Queryable().AsNoTracking().FirstOrDefault(x => x.CampañaId == oContrato.CampanaId);
                    Localidad localidad = mobjUnitOfWork.Repository<Localidad>().Queryable().AsNoTracking().FirstOrDefault(x => x.LocalidadId == oContrato.LocalidadId);
                    Provincia provincia = mobjUnitOfWork.Repository<Provincia>().Queryable().AsNoTracking().FirstOrDefault(x => x.ProvinciaId == oContrato.ProvinciaId);
                    Moneda monedaSust = mobjUnitOfWork.Repository<Moneda>().Queryable().AsNoTracking().FirstOrDefault(x => x.MonedaId == oContrato.MonedaIdSustentable);
                    Proveedor proveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().FirstOrDefault(x => x.ProveedorId == contrato.ProveedorId);

                    oMensaje.Subject = "Nuevo negocio Molinos Agro S.A. - " + proveedor.RazonSocial;

                    oMensaje.Body = "En el presente mail, se detalla el nuevo negocio generado con Molinos Agro S.A.:\r\n\r\n";

                    if (oContrato.ContratoSAP != null) oMensaje.Body += "Contrato SAP: " + oContrato.ContratoSAP.Value + " \r\n";
                    if (proveedor != null) oMensaje.Body += "Vendedor: " + proveedor.RazonSocial + " \r\n";
                    if (tipoNegocio != null) oMensaje.Body += "Tipo de Negocio: " + tipoNegocio.Descripcion + " \r\n";
                    if (material != null) oMensaje.Body += "Grano: " + material.Descripcion + " \r\n";
                    if (oContrato.Cantidad != 0) oMensaje.Body += "Kg: " + oContrato.Cantidad + " \r\n";
                    if (oContrato.Precio != 0 && moneda != null) oMensaje.Body += "Precio - Moneda: " + oContrato.Precio + " " + moneda.Descripcion + " \r\n";
                    if (campania != null) oMensaje.Body += "Campaña: " + campania.Descripcion + " \r\n";
                    if (localidad != null && provincia != null) oMensaje.Body += "Procedencia: " + provincia.Nombre + ", " + localidad.Nombre + " \r\n";
                    if (oContrato.ImporteSustentable != null && monedaSust != null) oMensaje.Body += "Sustentable: " + oContrato.ImporteSustentable.Value + " " + monedaSust.Descripcion + " \r\n";
                    if (oContrato.FechaDolarizado != null) oMensaje.Body += "Dolarizado: " + oContrato.FechaDolarizado.Value.ToString("dd/MM/yyyy") + " \r\n";
                    if (oContrato.DiasPesificado != null) oMensaje.Body += "Pesificado: " + oContrato.DiasPesificado.Value + " \r\n";
                    if (oContrato.NoInformaSio != null && oContrato.NoInformaSio == true) oMensaje.Body += "No Informa SIO \r\n";
                    if (oContrato.TrigoEspecial != null && oContrato.TrigoEspecial == true ) oMensaje.Body += "Trigo especial: Si" + " \r\n";

                    oMensaje.Body += "\r\nPor consultas, contactarse con " + (comercial != null ? comercial.Nombres + " " + comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") : "Mesa de Ayuda.") +
                        "\r\n\r\nSaludos Cordiales" +
                        "\r\n\r\nMolinos Agro S.A." +
                        "\r\n\r\nwww.molinosagro.com.ar";

                    oMensaje.BodyEncoding = System.Text.Encoding.UTF8;

                    oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                    SmtpClient oCliente = default(SmtpClient);

                    int Condicion = 0;
                    if (int.TryParse(ConfigurationManager.AppSettings["SmtpServerPort"], out Condicion))
                        oCliente = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
                    else
                        oCliente = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);

                    if (ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S")
                        oCliente.UseDefaultCredentials = true;
                    else
                        oCliente.UseDefaultCredentials = false;


                    if (ConfigurationManager.AppSettings["EnableSSL"] == "S")
                        oCliente.EnableSsl = true;
                    else
                        oCliente.EnableSsl = false;

                    oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                            ConfigurationManager.AppSettings["CredentialPassword"]);

                    oCliente.Send(oMensaje);
                }
            }
            catch (Exception ex)
            {
                var a = 1;
            }
        }

        public void EnviarEmailFijacion(FijacionDePrecioContrato oFijacionDePrecioContrato, string idActiveDirectory) {
            try {
                ContactoComercial proveedorContacto = mobjUnitOfWork.Repository<ContactoComercial>().Queryable().FirstOrDefault(x => x.ProveedorId == oFijacionDePrecioContrato.ProveedorId);
                Comercial comercial = mobjUnitOfWork.Repository<Comercial>().Queryable().FirstOrDefault(x => x.ComercialId == oFijacionDePrecioContrato.ComercialId);
                string emailComercial = "";
                string emailJefe = "";

                if (comercial != null)
                {
                    try { emailComercial = GetEmailUserActiveDirectory(comercial.IdActiveDirectory); } catch { }

                    Comercial jefeComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.ComercialId == comercial.EmpleadorACargo);

                    try { emailJefe = GetEmailUserActiveDirectory(jefeComercial.IdActiveDirectory); } catch { }

                }

                if ((proveedorContacto != null && proveedorContacto.Email1 != "" && proveedorContacto.Email1 != null) || (emailJefe != null && emailJefe != "") || (emailComercial != null && emailComercial == ""))
                {
                    MailMessage oMensaje = new MailMessage();

                    oMensaje.From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"]);

                    oMensaje.To.Add(proveedorContacto.Email1);
                    if (emailJefe != "" && emailJefe != null) oMensaje.CC.Add(emailJefe);
                    if (emailComercial != "" && emailComercial != null) oMensaje.CC.Add(emailComercial);

                    oMensaje.Subject = "Nuevo negocio Molinos Agro S.A. - DataAgro";

                    Proveedor proveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking().FirstOrDefault(x => x.ProveedorId == oFijacionDePrecioContrato.ProveedorId);
                    
                    TipoNegocio tipoNegocio = mobjUnitOfWork.Repository<TipoNegocio>().Queryable().AsNoTracking().FirstOrDefault(x => x.TipoNegocioId == 3);
                    Material material = mobjUnitOfWork.Repository<Material>().Queryable().AsNoTracking().FirstOrDefault(x => x.MaterialId == oFijacionDePrecioContrato.MaterialId);
                    Moneda moneda = mobjUnitOfWork.Repository<Moneda>().Queryable().AsNoTracking().FirstOrDefault(x => x.MonedaId == oFijacionDePrecioContrato.MonedaId);

                    oMensaje.Body = "En el presente mail, se detalla el nuevo negocio generado con Molinos Agro S.A.:\r\n\r\n";

                    if (oFijacionDePrecioContrato.ContratoId != null) oMensaje.Body += "Se creo una fijacion para el Contrato SAP: " + oFijacionDePrecioContrato.ContratoId + " \r\n";
                    if (proveedor != null) oMensaje.Body += "Vendedor: " + proveedor.RazonSocial + " \r\n";
                    if (tipoNegocio != null) oMensaje.Body += "Tipo de Negocio: " + tipoNegocio.Descripcion + " \r\n";
                    if (material != null) oMensaje.Body += "Grano: " + material.Descripcion + " \r\n";
                    if (oFijacionDePrecioContrato.Cantidad != 0) oMensaje.Body += "Kg: " + oFijacionDePrecioContrato.Cantidad + " \r\n";
                    if (oFijacionDePrecioContrato.Precio != 0 && moneda != null) oMensaje.Body += "Precio - Moneda: " + oFijacionDePrecioContrato.Precio + " " + moneda.Descripcion + " \r\n";

                    oMensaje.Body += "\r\nPor consultas, contactarse con " + (comercial != null ? comercial.Nombres + " " + comercial.Apellido + (emailComercial != "" && emailComercial  != null ? "(" + emailComercial + ")." : ".") : "Mesa de Ayuda.") +
                        "\r\n\r\nSaludos Cordiales" +
                        "\r\n\r\nMolinos Agro S.A." +
                        "\r\n\r\nwww.molinosagro.com.ar";

                    oMensaje.BodyEncoding = System.Text.Encoding.UTF8;

                    oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                    SmtpClient oCliente = default(SmtpClient);

                    int Condicion = 0;
                    if (int.TryParse(ConfigurationManager.AppSettings["SmtpServerPort"], out Condicion))
                        oCliente = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
                    else
                        oCliente = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);

                    if (ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S")
                        oCliente.UseDefaultCredentials = true;
                    else
                        oCliente.UseDefaultCredentials = false;


                    if (ConfigurationManager.AppSettings["EnableSSL"] == "S")
                        oCliente.EnableSsl = true;
                    else
                        oCliente.EnableSsl = false;

                    oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                            ConfigurationManager.AppSettings["CredentialPassword"]);

                    oCliente.Send(oMensaje);
                }
            }
            catch (Exception ex) {
                var a = 1;
            }
        }

        private string GetEmailUserActiveDirectory(string UserName)
        {
            DirectoryEntry entry = new DirectoryEntry();
            string userName = UserName;

            try { userName = UserName.Split('\\')[1]; } catch { }
       

            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = String.Format("(sAMAccountName={0})", userName);


            search.PropertiesToLoad.Add("givenName");   // first name
            search.PropertiesToLoad.Add("sn");          // last name
            search.PropertiesToLoad.Add("mail");        // smtp mail address

            // perform the search
            SearchResult result = search.FindOne();
            string email = result.Properties["mail"][0].ToString();


            return email;
        }

        public async Task<EntityErrors> EliminarRecordatorio(int Id)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Actividad>();

            var oActividad = await oRepository
                             .Queryable()
                             .Where(x => x.ActividadId == Id)
                             .SingleOrDefaultAsync();

            if (oActividad != null)
            {
                oRepository.Delete(oActividad);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<DatosIniProveedor> TraerDatosCombo(int ProveedorId)
        {
            var DatosCombo = new DatosIniProveedor();

            DatosCombo.segm = await mobjUnitOfWork.Repository<Segmentacion>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new SegmentacionQry() { SegmentacionId = x.SegmentacionId, Descripcion = x.Descripcion, Grupo = x.Grupo }).ToListAsync();

            DatosCombo.tiptel = await mobjUnitOfWork.Repository<TipoTelefono>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new TipoTelefonoQry() { TipoTelefonoId = x.TipoTelefonoId, Descripcion = x.Descripcion }).ToListAsync();

            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable().AsNoTracking();

            DatosCombo.prov = await mobjUnitOfWork.Repository<Provincia>()
                                .Queryable()
                                .AsNoTracking()
                                .Join(oLocalidad, a => a.ProvinciaId, b => b.ProvinciaId, (a, b) => new { P = a, L = b })
                                .GroupBy(x => new { x.P.ProvinciaId, x.P.Nombre })
                                .Select(x => new ProvinciaQry() { Provinciaid = x.Key.ProvinciaId, Nombre = x.Key.Nombre }).ToListAsync();

            DatosCombo.loc = new List<LocalidadQry>(); /*await mobjUnitOfWork.Repository<Localidad>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new LocalidadQry() { LocalidadId = x.LocalidadId, Nombre = x.Nombre, CodLocalidad = x.CodLocalidad, ProvinciaId = x.ProvinciaId }).ToListAsync();*/

            DatosCombo.cope = await mobjUnitOfWork.Repository<CanalOperacion>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new CanalOperacionQry() { CanalOperacionId = x.CanalOperacionId, Descripcion = x.Descripcion, Inhabilitado = false }).ToListAsync();

            DatosCombo.gran = await mobjUnitOfWork.Repository<Material>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => new MaterialQry() { MaterialId = x.MaterialId, Codigo = x.Codigo, Descripcion = x.Descripcion/*, CampañaIdActual = x.CampañaIdActual*/ }).ToListAsync();

            DatosCombo.dest = await mobjUnitOfWork.Repository<Destinatario>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => new DestinatarioQry() { DestinatarioId = x.DestinatarioId, Descripcion = x.Descripcion, Inhabilitado = false }).ToListAsync();

            DatosCombo.cond = await mobjUnitOfWork.Repository<Condicion>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new CondicionQry() { CondicionId = x.CondicionId, Descripcion = x.Descripcion, Inhabilitado = false }).ToListAsync();

            DatosCombo.inte = await mobjUnitOfWork.Repository<Interes>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new InteresQry() { InteresId = x.InteresId, Descripcion = x.Descripcion }).ToListAsync();

            DatosCombo.tipoact = await mobjUnitOfWork.Repository<TipoActividad>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new TipoActividadQry() { TipoActividadId = x.TipoActividadId, Descripcion = x.Descripcion }).ToListAsync();


            //DatosCombo.tipoact.ForEach(x=> (x.TipoActividadId == int.Parse(ConfigurationManager.AppSettings["AgendaLlamada"]) ? x.CamposExtra );

            DatosCombo.concom = await mobjUnitOfWork.Repository<ContactoComercial>()
                                .Queryable()
                                .AsNoTracking()
                                .Where(x => x.ProveedorId == ProveedorId)
                                .Select(x => new ContactoComercialQry() { ContactoComercialId = x.ContactoComercialId, Nombres = x.Nombres }).ToListAsync();

            return DatosCombo;
        }

        public async Task<List<Localidad>> TraerLocalidad(int Id)
        {
            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable();

            List<Localidad> localiadad = new List<Localidad>();

            localiadad = oLocalidad.Where(x => x.ProvinciaId == Id).ToList();

            return localiadad;
        }

        public async Task<ProveedorNuevo> TraerRazonSocial(string cuit)
        {
            var oRazonSocial = mobjUnitOfWork.Repository<RG2300>().Queryable();

            var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable();

            ProveedorNuevo razonsocial = new ProveedorNuevo();




            razonsocial = await oRazonSocial.Where(x => x.CUIT == cuit).Select(x => new ProveedorNuevo
            {
                CUIT = x.CUIT,
                Operable = 1,
                Condicion = x.Situacion,
                razonSocial = x.RazonSocial
            }).FirstOrDefaultAsync();

            if (razonsocial == null)
            {
                razonsocial = new ProveedorNuevo() { Condicion = "no incluido", CUIT = cuit, Operable = 0, razonSocial = "No existe Razon Social" };
            }

            razonsocial.Existe = oProveedor.Where(x => x.CUIT == cuit).Select(z => 1).DefaultIfEmpty(0).First();

            TraerEstado(razonsocial);

            return razonsocial;
        }

        public async Task<ProveedorQry> TraerProveedorPorCuit (string cuit) {

            var oProveedor = await mobjUnitOfWork.Repository<Proveedor>()
                   
                   .Queryable()
                   .AsNoTracking()
                   .Where(x => x.CUIT == cuit)
                   .Select(x => new ProveedorQry() { ProveedorId = x.ProveedorId, Descripcion = x.RazonSocial }).FirstOrDefaultAsync();

            return oProveedor;
        }

        public void TraerEstado(ProveedorNuevo prov)
        {
            var oFacacop = mobjUnitOfWork.Repository<FACACOP>().Queryable().AsNoTracking()
                            .Where(x => x.CUIT == prov.CUIT)
                            .FirstOrDefault();

            if (oFacacop != null)
            {
                prov.Operable = 0;
                prov.Condicion = "Apocrifos";
                return;
            }
            else if (!String.IsNullOrEmpty(prov.Condicion))
            {
                if ((prov.Condicion.ToLower() == ConfigurationManager.AppSettings["SitNoIncluida"])
                     || (prov.Condicion.ToLower() == ConfigurationManager.AppSettings["SitExcluido"])
                         || (prov.Condicion.ToLower() == ConfigurationManager.AppSettings["SitSuspendido"]))
                {
                    prov.Operable = 0;
                }
            }

            if (ConfigurationManager.AppSettings["SinConexionSap"] != "1")
            {
                var riesgo = new Agent.RiesgoComerciales();

                prov.RiesgoComercial = riesgo.ObtenerRiesgoComercial(prov.CUIT);

                if (prov.RiesgoComercial == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                {
                    prov.Operable = 0;
                    prov.Condicion = "Riesgo Comercial Alto";
                    return;
                }
            }


        }

        public async Task<List<ContactoComercial>> TraerContacto(int ProveedorId)
        {
            var oContactoComercial = mobjUnitOfWork.Repository<ContactoComercial>().Queryable();
            List<ContactoComercial> contactocomercial = new List<ContactoComercial>();

            contactocomercial = await oContactoComercial.AsNoTracking().Where(z => z.ProveedorId == ProveedorId).ToListAsync();

            return contactocomercial;

        }

        public async Task<GrabarProveedorResult> GrabarNuevoProveedor(NuevoProveedor oParam, string idActiveDirectory)
        {

            

            var oEntityErrors = new GrabarProveedorResult();
            oEntityErrors.errores = new EntityErrors();

            /*
             * 
            var Validar = new NuevoProveedorDatos();
            Validar.Proveedor = oParam;
            EntityValid.ValidateAll(Validar, oEntityErrors.ListaErrores);
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }
                      
            */

            var oProveedores = mobjUnitOfWork.Repository<Proveedor>().Queryable();
            var oRg2300 = mobjUnitOfWork.Repository<RG2300>().Queryable();
            var oFACACOP = mobjUnitOfWork.Repository<FACACOP>().Queryable();
            var oContactoComercial = mobjUnitOfWork.Repository<ContactoComercial>().Queryable();
            var oContactoComercialInteres = mobjUnitOfWork.Repository<ContactoComercialInteres>().Queryable();
            var oCampo = mobjUnitOfWork.Repository<Campo>().Queryable();
            var oCampoMaterial = mobjUnitOfWork.Repository<CampoMaterial>().Queryable();
            var oAcopio = mobjUnitOfWork.Repository<Acopio>().Queryable();
            var oAcopioMaterial = mobjUnitOfWork.Repository<AcopioMaterial>().Queryable();
            var oAcopioCampaña = mobjUnitOfWork.Repository<AcopioCampaña>().Queryable();
            var oProveedorCanalOperacion = mobjUnitOfWork.Repository<ProveedorCanalOperacion>().Queryable();
            var oProveedorComercial = mobjUnitOfWork.Repository<ProveedorComercial>().Queryable();
            var oProveedorCondicion = mobjUnitOfWork.Repository<ProveedorCondicion>().Queryable();
            var oProveedorDestinatario = mobjUnitOfWork.Repository<ProveedorDestinatario>().Queryable();
            var oCampañaMaterial = mobjUnitOfWork.Repository<Objetivo>().Queryable();
            var oComercials = mobjUnitOfWork.Repository<Comercial>().Queryable();

            
            var existe = oProveedores.AsNoTracking().Any(x => x.CUIT == oParam.basicos.cuit);
                

            if (existe)
            {
                oEntityErrors.errores = new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = "Ya existe un proveedor con ese CUIT" } } };
                return oEntityErrors;
            }

            var existeRG = oRg2300.AsNoTracking().Any(x => x.CUIT == oParam.basicos.cuit);

            if (!existeRG)
            {
                oEntityErrors.errores = new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = "No existe el CUIT" } } };
                return oEntityErrors;
            }

            var existeFA = oFACACOP.AsNoTracking().Any(x => x.CUIT == oParam.basicos.cuit);

            if (existeFA)
            {
                oEntityErrors.errores = new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = "El CUIT es Apocrifo" } } };
                return oEntityErrors;
            }

            int Id = oProveedores.AsNoTracking().Select(x => x.ProveedorId)
                         .DefaultIfEmpty(0)
                         .Max();


            


            Proveedor proveedor = new Proveedor();
            proveedor.ProveedorId = Id + 1;
            //proveedor.AlmacCapacidadPropia = oParam.produccion.capacidadAlmacenamientoProp;

            if (oParam.produccion.habilitaoSojaSust != null && oParam.produccion.habilitaoSojaSust != "null")
                proveedor.AlmacHabilitadoSojaSust = Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust));
            else
                proveedor.AlmacHabilitadoSojaSust = null;

            proveedor.AlmacHectSojaSust = oParam.produccion.hasAprobSojaSust;
            proveedor.AlmacTonsMaxSojaSust = oParam.produccion.TonsMaxAprobSojaSust;
            proveedor.AlmacVolAnualTotal = oParam.produccion.volumenAnualTotalTns;

            if (oParam.contacto.areaDeInfluencia != null && oParam.contacto.areaDeInfluencia != "null")
                proveedor.AreaInfluenciaId = Convert.ToInt32(oParam.contacto.areaDeInfluencia);
            else
                proveedor.AreaInfluenciaId = null;

            proveedor.Calificacion = oParam.basicos.calificacion;
            proveedor.CodigoPostal = oParam.contacto.codpost;
            proveedor.CUIT = oParam.basicos.cuit;
            proveedor.Direccion = oParam.contacto.direccion;
            /*proveedor.Email1 = oParam.basicos.emails[0];
            proveedor.Email2 = oParam.basicos.emails[1];
            proveedor.Email3 = oParam.basicos.emails[2];
            proveedor.Email4 = oParam.basicos.emails[3];*/
            proveedor.Intermediario = oParam.contacto.intermediario;
            proveedor.LocalidadId = oParam.contacto.localidad;
            //proveedor.NombreReferente = oParam.basicos.nomReferente;
            proveedor.ObjectState = Constants.Object_Added;
            proveedor.Observaciones = oParam.basicos.comentario;
            proveedor.RazonSocial = oParam.basicos.RazonSocial;
            proveedor.SegmentacionId = oParam.basicos.segmentacion;
            /*proveedor.Telefono1 = oParam.basicos.telefonos[0].telefono;
            proveedor.TipoTelefono1Id = oParam.basicos.telefonos[0].tipoTelefono;
            proveedor.Telefono2 = oParam.basicos.telefonos[1].telefono;
            proveedor.TipoTelefono2Id = oParam.basicos.telefonos[1].tipoTelefono;
            proveedor.Telefono3 = oParam.basicos.telefonos[2].telefono;
            proveedor.TipoTelefono3Id = oParam.basicos.telefonos[2].tipoTelefono;
            proveedor.TipoTelefono4Id = oParam.basicos.telefonos[3].tipoTelefono;
            proveedor.Telefono4 = oParam.basicos.telefonos[3].telefono;*/
            proveedor.EstadoId = 1;
            proveedor.FechaAlta = DateTime.Now;

            /*if (ConfigurationManager.AppSettings["SinConexionSap"].ToString() != "1")
            {
                var listaDeCuit = new List<Datos>();
                var oEstados = mobjUnitOfWork.Repository<Estado>().Queryable().AsNoTracking().ToList();
                var usuarioPrueba = idActiveDirectory;

                if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
                {
                    usuarioPrueba = ConfigurationManager.AppSettings["SapPruebaUser"];
                }

                listaDeCuit.Add(new Datos() { CUIT = oParam.basicos.cuit, UsuarioDirectory = usuarioPrueba });
                var list = new DatosProveedor().ObtenerDatosDeProveedor(listaDeCuit);
                if (list.Count > 0)
                {
                    proveedor.ClienteMOA = (!String.IsNullOrEmpty(list[0].CLIENTE_MOA) ? true : false);
                    var estado = oEstados.Where(x => x.Descripcion == list[0].STATUS).FirstOrDefault();
                    if (estado != null)
                    {
                        proveedor.EstadoId = estado.EstadoId;
                    }
                    else
                    {
                        if (oParam.basicos.nocliente == 1)
                            proveedor.EstadoId = int.Parse(ConfigurationManager.AppSettings["NoCliente"]);
                        else
                            proveedor.EstadoId = int.Parse(ConfigurationManager.AppSettings["PotencialCliente"]);
                    }
                }
                else
                {
                    if (oParam.basicos.nocliente == 1)
                        proveedor.EstadoId = int.Parse(ConfigurationManager.AppSettings["NoCliente"]);
                    else
                        proveedor.EstadoId = int.Parse(ConfigurationManager.AppSettings["PotencialCliente"]);
                }


            }
            else
            {
                if (oParam.basicos.nocliente == 1)
                    proveedor.EstadoId = int.Parse(ConfigurationManager.AppSettings["NoCliente"]);
                else
                    proveedor.EstadoId = int.Parse(ConfigurationManager.AppSettings["PotencialCliente"]);
            }*/





            var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
            if (ConfigurationManager.AppSettings["SinConexionSap"].ToString() != "1")
            {
                var listaDeCuit = new List<Datos>();
                var oEstados = mobjUnitOfWork.Repository<Estado>().Queryable().AsNoTracking().ToList();
                var usuarioPrueba = idActiveDirectory;

                if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
                {
                    usuarioPrueba = ConfigurationManager.AppSettings["SapPruebaUser"];
                }
                var com = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.IdActiveDirectory == idActiveDirectory);
                var listProve = mobjUnitOfWork.SelStore<Datos>("DataAgro_ActualizarComercialHome", com.ComercialId).ToList();

                foreach (var x in listProve)
                {
                    if (x.CUIT == oParam.basicos.cuit)
                    {
                        listaDeCuit.Add(new Datos() { CUIT = x.CUIT, UsuarioDirectory = x.UsuarioDirectory });
                    }
                }
                //listaDeCuit.Add(new Datos() { CUIT = oParam.basicos.cuit, UsuarioDirectory = usuarioPrueba });

                if (listaDeCuit.Count == 0)
                {
                    if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && idActiveDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                    {
                        listaDeCuit.Add(new Datos() { CUIT = oParam.basicos.cuit, UsuarioDirectory = ConfigurationManager.AppSettings["SapPruebaUser"].ToString() });
                    }
                    else
                    {
                        listaDeCuit.Add(new Datos() { CUIT = oParam.basicos.cuit, UsuarioDirectory = idActiveDirectory });
                    }
                }

                var list = new DatosProveedor().ObtenerDatosDeProveedor(listaDeCuit);

                if (list.Count > 0)
                {
                    foreach (var lista in list)
                    {

                        var comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());


                        if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && idActiveDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                        {
                            string aux = ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower();
                            comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == aux);
                        }

                        if (comercial != null)
                        {

                            var ProvEstados = mobjUnitOfWork.Repository<ProveedorEstado>().Queryable().Where(x => x.ProveedorId == proveedor.ProveedorId && x.ComercialId == comercial.ComercialId).ToList();
                            if (ProvEstados.Count == 0)
                            {
                                //Agregar
                                int id = mobjUnitOfWork.Repository<ProveedorEstado>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => x.ProveedorEstadoId)
                                    .DefaultIfEmpty(0)
                                    .Max();



                                ProveedorEstado pe = new ProveedorEstado();

                                pe.ComercialId = comercial.ComercialId;
                                var Est = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault();
                                pe.EstadoId = Est != null ? Est.EstadoId : 1;
                                pe.ObjectState = Constants.Object_Added;
                                pe.ProveedorEstadoId = id + 1;
                                pe.ProveedorId = proveedor.ProveedorId;

                                mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);
                            }
                            else
                            {
                                //modificar
                                foreach (var pe in ProvEstados)
                                {
                                    pe.ComercialId = comercial.ComercialId;
                                    var Est = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault();
                                    pe.EstadoId = Est != null ? Est.EstadoId : 1;
                                    pe.ObjectState = Constants.Object_Modified;
                                    pe.ProveedorEstadoId = pe.ProveedorEstadoId;
                                    pe.ProveedorId = proveedor.ProveedorId;
                                    mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);
                                }

                            }




                            proveedor.ClienteMOA = (!String.IsNullOrEmpty(lista.CLIENTE_MOA) ? true : false);

                        }

                    }

                    mobjUnitOfWork.SaveChanges();


                    /*oProveedorSave.ClienteMOA = (!String.IsNullOrEmpty(list[0].CLIENTE_MOA) ? true : false);
                    var estado = oEstados.Where(x => x.Descripcion == list[0].STATUS).FirstOrDefault();
                    if (estado != null)
                    {
                        oProveedorSave.EstadoId = estado.EstadoId;
                    }
                    else
                    {
                        if (oParam.basicos.nocliente == 1)
                            oProveedorSave.EstadoId = int.Parse(ConfigurationManager.AppSettings["NoCliente"]);
                        else
                            oProveedorSave.EstadoId = int.Parse(ConfigurationManager.AppSettings["PotencialCliente"]);
                    }*/
                }
                else
                {

                    int id = mobjUnitOfWork.Repository<ProveedorEstado>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => x.ProveedorEstadoId)
                                    .DefaultIfEmpty(0)
                                    .Max();


                    var comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == idActiveDirectory);
                    ProveedorEstado pe = new ProveedorEstado();
                    pe.ComercialId = comercial.ComercialId;
                    pe.ObjectState = Constants.Object_Added;
                    pe.ProveedorEstadoId = id + 1;
                    pe.ProveedorId = proveedor.ProveedorId;

                    if (oParam.basicos.nocliente == 1)
                    {
                        pe.EstadoId = int.Parse(ConfigurationManager.AppSettings["NoCliente"]);
                    }
                    else
                        pe.EstadoId = int.Parse(ConfigurationManager.AppSettings["PotencialCliente"]);

                    mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);


                    mobjUnitOfWork.SaveChanges();

                }
            }
            else
            {
                if (oParam.basicos.nocliente == 1)
                    proveedor.EstadoId = int.Parse(ConfigurationManager.AppSettings["NoCliente"]);
                else
                    proveedor.EstadoId = int.Parse(ConfigurationManager.AppSettings["PotencialCliente"]);
            }











            mobjUnitOfWork.Repository<Proveedor>().SaveEntity(proveedor);
            try
            {
                await mobjUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            var oComercial = new Comercial();
            oComercial = await mobjUnitOfWork.Repository<Comercial>()
                                .Queryable()
                                .AsNoTracking()
                                .Where(x => x.IdActiveDirectory == idActiveDirectory)
                                .SingleOrDefaultAsync();




            ProveedorComercial proveedorcomercial = new ProveedorComercial();
            int ProveedorComercialId = oProveedorComercial.AsNoTracking().Select(x => x.ProveedorComercialId)
                         .DefaultIfEmpty(0)
                         .Max();
            //TODO: buscar el ultimo item
            int ProveedorComercialItem = 0;
            //int ProveedorComercialItem = oProveedorComercial.Where(x => x.ComercialId == oComercial.ComercialId && x.ProveedorId == proveedor.ProveedorId).Max(x=> x.Item);


            proveedorcomercial.ComercialId = oComercial.ComercialId;
            proveedorcomercial.NroItem = ProveedorComercialItem + 1; ;
            proveedorcomercial.ObjectState = Constants.Object_Added;
            proveedorcomercial.ProveedorComercialId = ProveedorComercialId + 1;
            proveedorcomercial.ProveedorId = proveedor.ProveedorId;

            mobjUnitOfWork.Repository<ProveedorComercial>().SaveEntity(proveedorcomercial);
            try
            {
                await mobjUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            if (oParam.contactocomercial != null && oParam.contactocomercial.Count > 0)
            {
                foreach (var param in oParam.contactocomercial)
                {
                    int ContactoComercialId = oContactoComercial.AsNoTracking().Select(x => x.ContactoComercialId)
                         .DefaultIfEmpty(0)
                         .Max();
                    ContactoComercial contactocomercial = new ContactoComercial();
                    contactocomercial.ContactoComercialId = ContactoComercialId + 1;
                    contactocomercial.Apellido = param.apellido;
                    contactocomercial.Cargo = param.cargo;
                    contactocomercial.Email1 = param.emails[0];
                    contactocomercial.Email2 = param.emails[1];
                    contactocomercial.Email3 = param.emails[2];
                    contactocomercial.EsPrincipal = param.principal;
                    contactocomercial.FechaNacimiento = param.fechaNacimiento;
                    contactocomercial.Nombres = param.nombre;
                    contactocomercial.ObjectState = Constants.Object_Added;
                    contactocomercial.OtrosIntereses = param.otrosIntereses;
                    contactocomercial.ProveedorId = proveedor.ProveedorId;
                    contactocomercial.Puesto = param.puesto;
                    contactocomercial.Telefono1 = param.telefonos[0].telefono;
                    contactocomercial.Telefono2 = param.telefonos[1].telefono;
                    contactocomercial.Telefono3 = param.telefonos[2].telefono;
                    contactocomercial.TipoTelefono1Id = (param.telefonos[0].tipoTelefono.HasValue ? (int?)param.telefonos[0].tipoTelefono.Value : null);
                    contactocomercial.TipoTelefono2Id = (param.telefonos[1].tipoTelefono.HasValue ? (int?)param.telefonos[1].tipoTelefono.Value : null);
                    contactocomercial.TipoTelefono3Id = (param.telefonos[2].tipoTelefono.HasValue ? (int?)param.telefonos[2].tipoTelefono.Value : null);
                    mobjUnitOfWork.Repository<ContactoComercial>().SaveEntity(contactocomercial);


                    try
                    {
                        await mobjUnitOfWork.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }


                    int cant = 1;
                    int ContactoComercialInteresId = oContactoComercialInteres.AsNoTracking()
                            .Select(x => x.ContactoComercialInteresId)
                            .DefaultIfEmpty(0)
                            .Max();


                    foreach (var interes in param.intereses)
                    {
                        ContactoComercialInteres contactocomercialinteres = new ContactoComercialInteres();
                        contactocomercialinteres.ContactoComercialInteresId = ContactoComercialInteresId + cant;
                        contactocomercialinteres.ContactoComercialId = contactocomercial.ContactoComercialId;
                        contactocomercialinteres.InteresId = interes;
                        contactocomercialinteres.NroItem = cant;
                        contactocomercialinteres.ObjectState = Constants.Object_Added;
                        mobjUnitOfWork.Repository<ContactoComercialInteres>().SaveEntity(contactocomercialinteres);

                        try
                        {
                            await mobjUnitOfWork.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        cant++;

                    }
                }
            }

            if (oParam.produccion.CamposProduccion != null && oParam.produccion.CamposProduccion.Count > 0)
            {
                int item = 1;
                int CampoId = oCampo.AsNoTracking()
                            .Select(x => x.CampoId)
                            .DefaultIfEmpty(0)
                            .Max();
                foreach (var cmp in oParam.produccion.CamposProduccion)
                {

                    Campo campo = new Campo();
                    campo.CampoId = CampoId + item;
                    campo.ArrendaPropia = cmp.hectareas;
                    campo.Coordenadas = cmp.coordenadas;
                    campo.HabilitadoSojaSustentable = Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust));
                    campo.KMZfile = cmp.archivoFileResult;
                    campo.KMZnombre = cmp.archivo;
                    campo.LocalidadId = cmp.localidad;
                    campo.NroItem = item;
                    campo.ObjectState = Constants.Object_Added;
                    campo.ProveedorId = proveedor.ProveedorId;

                    mobjUnitOfWork.Repository<Campo>().SaveEntity(campo);
                    try
                    {
                        await mobjUnitOfWork.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                    item++;

                    int itemCM = 1;
                    int CampoMaterialId = oCampoMaterial.AsNoTracking()
                            .Select(x => x.CampoMaterialid)
                            .DefaultIfEmpty(0)
                            .Max();
                    foreach (var grn in cmp.granos)
                    {


                        CampoMaterial campomaterial = new CampoMaterial();
                        campomaterial.CampañaId = grn.campañaId;
                        campomaterial.CampoId = campo.CampoId;
                        campomaterial.CampoMaterialid = CampoMaterialId + itemCM;
                        campomaterial.Hectareas = grn.hectareas.HasValue ? grn.hectareas : null;
                        campomaterial.MaterialId = grn.granoId;
                        campomaterial.NroItem = itemCM;
                        campomaterial.ObjectState = Constants.Object_Added;
                        campomaterial.Toneladas = grn.toneladas.HasValue ? grn.toneladas : null;

                        mobjUnitOfWork.Repository<CampoMaterial>().SaveEntity(campomaterial);
                        try
                        {
                            await mobjUnitOfWork.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        itemCM++;
                    }

                }
            }

            if (oParam.almacenamiento != null && oParam.almacenamiento.CamposAlmacenamiento != null && oParam.almacenamiento.CamposAlmacenamiento.Count > 0)
            {
                int item = 1;
                int AcopioId = oAcopio.AsNoTracking()
                            .Select(x => x.AcopioId)
                            .DefaultIfEmpty(0)
                            .Max();
                foreach (var cmp in oParam.almacenamiento.CamposAlmacenamiento)
                {


                    Acopio acopio = new Acopio();
                    acopio.AcopioId = AcopioId + item;
                    //acopio.ArrendadoPropio = cmp.hectareasAlmacenamiento;
                    acopio.Coordenadas = cmp.coordenadasAlmacenamiento;
                    acopio.KMZfile = cmp.archivoFileResult;
                    acopio.KMZnombre = cmp.archivo;
                    acopio.LocalidadId = cmp.localidad;
                    acopio.NroItem = item;
                    acopio.ObjectState = Constants.Object_Added;
                    acopio.ProveedorId = proveedor.ProveedorId;

                    mobjUnitOfWork.Repository<Acopio>().SaveEntity(acopio);
                    try
                    {
                        await mobjUnitOfWork.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                    item++;

                    int itemCM = 1;
                    int AcopioCampañaId = oAcopioCampaña.AsNoTracking()
                             .Select(x => x.AcopioCampañaId)
                            .DefaultIfEmpty(0)
                            .Max();
                    foreach (var grn in cmp.granosAlmacenamiento)
                    {


                        AcopioCampaña acopiocampaña = new AcopioCampaña();
                        acopiocampaña.AcopioId = acopio.AcopioId;
                        acopiocampaña.AcopioCampañaId = AcopioCampañaId + itemCM;
                        acopiocampaña.CampañaId = grn.campañaId;
                        //acopiomaterial.MaterialId = grn.granoId;
                        acopiocampaña.HasArrendadas = grn.hasArrendadas;
                        acopiocampaña.NroItem = itemCM;
                        acopiocampaña.ObjectState = Constants.Object_Added;
                        //acopiomaterial.Porcentaje = grn.porcentajeAlmacenamiento;
                        acopiocampaña.Toneladas = grn.toneladasAlmacenamiento;

                        mobjUnitOfWork.Repository<AcopioCampaña>().SaveEntity(acopiocampaña);
                        try
                        {
                            await mobjUnitOfWork.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        itemCM++;
                    }

                    itemCM = 1;
                    int AcopioMaterialId = oAcopioMaterial.AsNoTracking()
                             .Select(x => x.AcopioMaterialId)
                            .DefaultIfEmpty(0)
                            .Max();
                    foreach (var grn in cmp.granosAlmacenamientoGrano)
                    {

                        AcopioMaterial acopiomaterial = new AcopioMaterial();
                        acopiomaterial.AcopioId = acopio.AcopioId;
                        acopiomaterial.AcopioMaterialId = AcopioMaterialId + itemCM;
                        acopiomaterial.CampañaId = grn.campañaId;
                        //acopiomaterial.MaterialId = grn.granoId;
                        acopiomaterial.MaterialId = grn.granoId;
                        acopiomaterial.NroItem = itemCM;
                        acopiomaterial.ObjectState = Constants.Object_Added;
                        //acopiomaterial.Porcentaje = grn.porcentajeAlmacenamiento;
                        acopiomaterial.Toneladas = grn.toneladasAlmacenamiento;

                        mobjUnitOfWork.Repository<AcopioMaterial>().SaveEntity(acopiomaterial);
                        try
                        {
                            await mobjUnitOfWork.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        itemCM++;
                    }






                }
            }

            if (oParam.contacto.canalesOperacion != null && oParam.contacto.canalesOperacion.Count > 0)
            {
                List<ProveedorCanalOperacion> proveedorcanaloperacion = new List<ProveedorCanalOperacion>();
                int itemPCO = 1;
                int ProveedorCanalOperacionId = oProveedorCanalOperacion.AsNoTracking()
                         .Select(x => x.ContactoCanalOperacionId)
                         .DefaultIfEmpty(0)
                         .Max();
                foreach (var param in oParam.contacto.canalesOperacion)
                {


                    ProveedorCanalOperacion pco = new ProveedorCanalOperacion();
                    pco.CanalOperacionId = param;
                    pco.ContactoCanalOperacionId = ProveedorCanalOperacionId + itemPCO;
                    pco.NroItem = itemPCO.ToString();
                    pco.ObjectState = Constants.Object_Added;
                    pco.ProveedorId = proveedor.ProveedorId;

                    proveedorcanaloperacion.Add(pco);

                    itemPCO++;

                }

                mobjUnitOfWork.Repository<ProveedorCanalOperacion>().SaveRange(proveedorcanaloperacion);
                try
                {
                    await mobjUnitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }





            if (oParam.contacto.condPreferentes != null && oParam.contacto.condPreferentes.Count > 0)
            {
                List<ProveedorCondicion> proveedorcondicion = new List<ProveedorCondicion>();
                int itemPCO = 1;
                int ProveedorCondicionId = oProveedorCondicion.AsNoTracking().Select(x => x.ContactoCondicionId)
                         .DefaultIfEmpty(0)
                         .Max();
                foreach (var param in oParam.contacto.condPreferentes)
                {


                    ProveedorCondicion pc = new ProveedorCondicion();
                    pc.CondicionId = param;
                    pc.ContactoCondicionId = ProveedorCondicionId + itemPCO;
                    pc.NroItem = itemPCO;
                    pc.ObjectState = Constants.Object_Added;
                    pc.ProveedorId = proveedor.ProveedorId;

                    proveedorcondicion.Add(pc);
                    itemPCO++;

                }
                mobjUnitOfWork.Repository<ProveedorCondicion>().SaveRange(proveedorcondicion);
                try
                {
                    await mobjUnitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            if (oParam.contacto.entregaA != null && oParam.contacto.entregaA.Count > 0)
            {
                List<ProveedorDestinatario> proveedordestinatario = new List<ProveedorDestinatario>();
                int itemPCO = 1;
                int ProveedorDestinatarioId = oProveedorDestinatario.AsNoTracking().Select(x => x.ContactoDestinatarioId)
                         .DefaultIfEmpty(0)
                         .Max();
                foreach (var param in oParam.contacto.entregaA)
                {


                    ProveedorDestinatario pd = new ProveedorDestinatario();
                    pd.ContactoDestinatarioId = ProveedorDestinatarioId + itemPCO;
                    pd.DestinatarioId = param;
                    pd.NroItem = itemPCO;
                    pd.ObjectState = Constants.Object_Added;
                    pd.ProveedorId = proveedor.ProveedorId;

                    proveedordestinatario.Add(pd);
                    itemPCO++;

                }
                mobjUnitOfWork.Repository<ProveedorDestinatario>().SaveRange(proveedordestinatario);
                try
                {
                    await mobjUnitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            if (oParam.produccion.objetivos != null && oParam.produccion.objetivos.Count > 0)
            {
                oParam.produccion.objetivos.OrderBy(z => z.campañaId);

                List<Objetivo> campañamaterial = new List<Objetivo>();

                int CampañaActualId = 0;
                int ItemCampañaActual = 0;
                int itemCampañaMaterialId = 0;

                foreach (var param in oParam.produccion.objetivos)
                {
                    int CampañaMaterialId = oCampañaMaterial.AsNoTracking().Select(x => x.ObjetivoId)
                         .DefaultIfEmpty(0)
                         .Max();

                    if (CampañaActualId != param.campañaId)
                    {
                        CampañaActualId = param.campañaId;
                        ItemCampañaActual = 0;
                    }

                    ItemCampañaActual++;
                    itemCampañaMaterialId++;

                    Objetivo cm = new Objetivo();
                    cm.CampañaId = param.campañaId;
                    cm.ObjetivoId = CampañaMaterialId + itemCampañaMaterialId;
                    cm.MaterialId = param.granoId;
                    cm.NroItem = ItemCampañaActual;
                    cm.ObjectState = Constants.Object_Added;
                    cm.ProveedorId = proveedor.ProveedorId;
                    cm.ToneladasObjetivos = Convert.ToDouble(param.toneladasObjetivo);

                    campañamaterial.Add(cm);

                }
                mobjUnitOfWork.Repository<Objetivo>().SaveRange(campañamaterial);
                try
                {
                    await mobjUnitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }

            }

            oEntityErrors.ProveedorId = proveedor.ProveedorId;
            oEntityErrors.errores = new EntityErrors();
            //await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<GrabarProveedorResult> UpdateProveedor(NuevoProveedor oParam, string idActiveDirectory)
        {
            
            var oEntityErrors = new GrabarProveedorResult();
            oEntityErrors.errores = new EntityErrors();
            /*
            EntityValid.ValidateAll(oParam, oEntityErrors.ListaErrores);

            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            var validacion = Validar(oParam);

            if (validacion != null)
            {
                oEntityErrors.ListaErrores.Add(validacion);
                return oEntityErrors;
            }
            */


            var Error = await UpdateDatosBasicosProveedor(oParam, idActiveDirectory);

            if (Error != null)
            {
                oEntityErrors.errores = Error;
                return oEntityErrors;
            }   

            var ErrorContacto = await UpdateDatosContacto(oParam);

            if (ErrorContacto != null)
            {
                oEntityErrors.errores = ErrorContacto;
                return oEntityErrors;
            }




            var ErrorComercial = await UpdateContactoComerciales(oParam);

            if (ErrorComercial != null)
            {
                oEntityErrors.errores = ErrorComercial;
                return oEntityErrors;
            }




            var ErrorProduccion = await UpdateProduccion(oParam);


            if (ErrorProduccion != null)
            {
                oEntityErrors.errores = ErrorProduccion;
                return oEntityErrors;
            }

            var ErrorAlmacenamiento = await UpdateAlmacenamiento(oParam);


            if (ErrorAlmacenamiento != null)
            {
                oEntityErrors.errores = ErrorAlmacenamiento;
                return oEntityErrors;
            }

            try
            {
                await mobjUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                oEntityErrors.errores = new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
                return oEntityErrors;
            }

            oEntityErrors.ProveedorId = oParam.ProveedorId;

            return oEntityErrors;

        }

        private ErrorMessage Validar(NuevoProveedor oParam)
        {
            throw new NotImplementedException();
        }

        public async Task<Proveedor> TraerProveedor(int? proveedorId)
        {
            var oProveedor = await mobjUnitOfWork.Repository<Proveedor>()
                                 .Queryable()
                                 .Where(x => x.ProveedorId == proveedorId)
                                 .SingleOrDefaultAsync();

            oProveedor.ObjectState = Constants.Object_Modified;

            return oProveedor;
        }

        public async Task<EntityErrors> UpdateDatosBasicosProveedor(NuevoProveedor oParam, string idActiveDirectory)
        {
            Proveedor oProveedorSave;


            try
            {
                oProveedorSave = await TraerProveedor(oParam.ProveedorId);

                /*
                oProveedorSave.AlmacCapacidadPropia = oParam.produccion.capacidadAlmacenamientoProp;

                if (oParam.produccion.habilitaoSojaSust != null && oParam.produccion.habilitaoSojaSust != "null")
                    oProveedorSave.AlmacHabilitadoSojaSust = Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust));
                else
                    oProveedorSave.AlmacHabilitadoSojaSust = null;

                oProveedorSave.AlmacHectSojaSust = oParam.produccion.hasAprobSojaSust;
                oProveedorSave.AlmacTonsMaxSojaSust = oParam.produccion.TonsMaxAprobSojaSust;
                oProveedorSave.AlmacVolAnualTotal = oParam.produccion.volumenAnualTotalTns;

                if (oParam.contacto.areaDeInfluencia != null && oParam.contacto.areaDeInfluencia != "null")
                    oProveedorSave.AreaInfluenciaId = Convert.ToInt32(oParam.contacto.areaDeInfluencia);
                else
                    oProveedorSave.AreaInfluenciaId = null;
                    */

                //oProveedorSave.CodigoPostal = oParam.contacto.codpost;
                //oProveedorSave.Direccion = oParam.contacto.direccion;
                //oProveedorSave.Intermediario = oParam.contacto.intermediario;
                //oProveedorSave.LocalidadId = oParam.contacto.localidad;

                if (oParam.produccion.habilitaoSojaSust != null && oParam.produccion.habilitaoSojaSust != "null")
                    oProveedorSave.AlmacHabilitadoSojaSust = Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust));
                else
                    oProveedorSave.AlmacHabilitadoSojaSust = null;

                oProveedorSave.AlmacVolAnualTotal = oParam.produccion.volumenAnualTotalTns;

                oProveedorSave.AlmacHectSojaSust = oParam.produccion.hasAprobSojaSust;
                oProveedorSave.AlmacTonsMaxSojaSust = oParam.produccion.TonsMaxAprobSojaSust;
                oProveedorSave.Calificacion = oParam.basicos.calificacion;
                /*oProveedorSave.Email1 = oParam.basicos.emails[0];
                oProveedorSave.Email2 = oParam.basicos.emails[1];
                oProveedorSave.Email3 = oParam.basicos.emails[2];
                oProveedorSave.Email4 = oParam.basicos.emails[3];
                oProveedorSave.NombreReferente = oParam.basicos.nomReferente;*/
                oProveedorSave.Observaciones = oParam.basicos.comentario;
                oProveedorSave.SegmentacionId = oParam.basicos.segmentacion;
                /*oProveedorSave.Telefono1 = oParam.basicos.telefonos[0].telefono;
                oProveedorSave.TipoTelefono1Id = oParam.basicos.telefonos[0].tipoTelefono;
                oProveedorSave.Telefono2 = oParam.basicos.telefonos[1].telefono;
                oProveedorSave.TipoTelefono2Id = oParam.basicos.telefonos[1].tipoTelefono;
                oProveedorSave.Telefono3 = oParam.basicos.telefonos[2].telefono;
                oProveedorSave.TipoTelefono3Id = oParam.basicos.telefonos[2].tipoTelefono;
                oProveedorSave.TipoTelefono4Id = oParam.basicos.telefonos[3].tipoTelefono;
                oProveedorSave.Telefono4 = oParam.basicos.telefonos[3].telefono;*/
                oProveedorSave.CodigoPostal = oParam.contacto.codpost;
                oProveedorSave.Direccion = oParam.contacto.direccion;
                oProveedorSave.Intermediario = oParam.contacto.intermediario;
                oProveedorSave.LocalidadId = oParam.contacto.localidad;
                oProveedorSave.ProvinciaId = oParam.contacto.provincia;

                if (oParam.contacto.areaDeInfluencia != null && oParam.contacto.areaDeInfluencia != "null")
                    oProveedorSave.AreaInfluenciaId = Convert.ToInt32(oParam.contacto.areaDeInfluencia);
                else
                    oProveedorSave.AreaInfluenciaId = null;

                
                var oProveedorEstados = mobjUnitOfWork.Repository<ProveedorEstado>().Queryable().AsNoTracking();
                var ProveedorId = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking().FirstOrDefault(x => x.CUIT == oParam.basicos.cuit).ProveedorId;
                var ComercialId = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.IdActiveDirectory == idActiveDirectory).ComercialId;
                var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
                
                

                if (ConfigurationManager.AppSettings["SinConexionSap"].ToString() != "1")
                {
                    var listaDeCuit = new List<Datos>();
                    var oEstados = mobjUnitOfWork.Repository<Estado>().Queryable().AsNoTracking().ToList();
                    var usuarioPrueba = idActiveDirectory;

                    if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
                    {
                        usuarioPrueba = ConfigurationManager.AppSettings["SapPruebaUser"];
                    }


                    var listProve = mobjUnitOfWork.SelStore<Datos>("DataAgro_ActualizarComercialHome", ComercialId).ToList();

                    foreach (var x in listProve)
                    {
                        if (x.CUIT == oParam.basicos.cuit)
                        {

                            if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && idActiveDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                            {
                                listaDeCuit.Add(new Datos() { CUIT = x.CUIT, UsuarioDirectory = ConfigurationManager.AppSettings["SapPruebaUser"].ToString() });
                            }
                            else
                            {
                                listaDeCuit.Add(new Datos() { CUIT = x.CUIT, UsuarioDirectory = x.UsuarioDirectory });
                            }
                        }
                    }



                    //listaDeCuit.Add(new Datos() { CUIT = oParam.basicos.cuit, UsuarioDirectory = usuarioPrueba });
                    

                    var list = new DatosProveedor().ObtenerDatosDeProveedor(listaDeCuit);

                    if (list.Count > 0)
                    {
                        foreach (var lista in list)
                        {

                            var comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());


                            if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && idActiveDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                            {
                                string aux = ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower();
                                comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == aux);
                            }

                            if (comercial != null)
                            {

                                var ProvEstados = mobjUnitOfWork.Repository<ProveedorEstado>().Queryable().Where(x => x.ProveedorId == ProveedorId && x.ComercialId == comercial.ComercialId).ToList();
                                if (ProvEstados.Count == 0)
                                {
                                    //Agregar
                                    int id = mobjUnitOfWork.Repository<ProveedorEstado>()
                                        .Queryable()
                                        .AsNoTracking()
                                        .Select(x => x.ProveedorEstadoId)
                                        .DefaultIfEmpty(0)
                                        .Max();



                                    ProveedorEstado pe = new ProveedorEstado();

                                    pe.ComercialId = comercial.ComercialId;
                                    var Est = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault();
                                    pe.EstadoId = Est != null ? Est.EstadoId : 1;
                                    pe.ObjectState = Constants.Object_Added;
                                    pe.ProveedorEstadoId = id + 1;
                                    pe.ProveedorId = ProveedorId;

                                    mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);
                                }
                                else
                                {
                                    //modificar
                                    foreach (var pe in ProvEstados)
                                    {
                                        pe.ComercialId = comercial.ComercialId;
                                        var Est = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault();
                                        pe.EstadoId = Est != null ? Est.EstadoId : 1;
                                        pe.ObjectState = Constants.Object_Modified;
                                        pe.ProveedorEstadoId = pe.ProveedorEstadoId;
                                        pe.ProveedorId = ProveedorId;
                                        mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);
                                    }

                                }




                                oProveedorSave.ClienteMOA = (!String.IsNullOrEmpty(lista.CLIENTE_MOA) ? true : false);

                            }

                        }

                        mobjUnitOfWork.SaveChanges();
                    }
                    else
                    {
                        if (oParam.basicos.nocliente == 1)
                            oProveedorSave.EstadoId = int.Parse(ConfigurationManager.AppSettings["NoCliente"]);
                        else
                            oProveedorSave.EstadoId = int.Parse(ConfigurationManager.AppSettings["PotencialCliente"]);



                        var comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory == idActiveDirectory);
                        if (comercial != null)
                        {

                            var ProvEstados = mobjUnitOfWork.Repository<ProveedorEstado>().Queryable().Where(x => x.ProveedorId == ProveedorId && x.ComercialId == comercial.ComercialId).ToList();
                            if (ProvEstados.Count == 0)
                            {
                                //Agregar
                                int id = mobjUnitOfWork.Repository<ProveedorEstado>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => x.ProveedorEstadoId)
                                    .DefaultIfEmpty(0)
                                    .Max();



                                ProveedorEstado pe = new ProveedorEstado();

                                pe.ComercialId = comercial.ComercialId;
                                pe.EstadoId = oProveedorSave.EstadoId.Value;
                                pe.ObjectState = Constants.Object_Added;
                                pe.ProveedorEstadoId = id + 1;
                                pe.ProveedorId = ProveedorId;

                                mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);
                            }
                            else
                            {
                                //modificar
                                foreach (var pe in ProvEstados)
                                {
                                    pe.ComercialId = comercial.ComercialId;
                                    pe.EstadoId = oProveedorSave.EstadoId.Value;
                                    pe.ObjectState = Constants.Object_Modified;
                                    pe.ProveedorEstadoId = pe.ProveedorEstadoId;
                                    pe.ProveedorId = ProveedorId;
                                    mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);
                                }

                            }
                        }
                    }
                }
                else
                {
                    if (oParam.basicos.nocliente == 1)
                        oProveedorSave.EstadoId = int.Parse(ConfigurationManager.AppSettings["NoCliente"]);
                    else
                        oProveedorSave.EstadoId = int.Parse(ConfigurationManager.AppSettings["PotencialCliente"]);


                    var comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory == idActiveDirectory);
                    if (comercial != null)
                    {

                        var ProvEstados = mobjUnitOfWork.Repository<ProveedorEstado>().Queryable().Where(x => x.ProveedorId == ProveedorId && x.ComercialId == comercial.ComercialId).ToList();
                        if (ProvEstados.Count == 0)
                        {
                            //Agregar
                            int id = mobjUnitOfWork.Repository<ProveedorEstado>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => x.ProveedorEstadoId)
                                .DefaultIfEmpty(0)
                                .Max();



                            ProveedorEstado pe = new ProveedorEstado();

                            pe.ComercialId = comercial.ComercialId;
                            pe.EstadoId = oProveedorSave.EstadoId.Value;
                            pe.ObjectState = Constants.Object_Added;
                            pe.ProveedorEstadoId = id + 1;
                            pe.ProveedorId = ProveedorId;

                            mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);
                        }
                        else
                        {
                            //modificar
                            foreach (var pe in ProvEstados)
                            {
                                pe.ComercialId = comercial.ComercialId;
                                pe.EstadoId = oProveedorSave.EstadoId.Value;
                                pe.ObjectState = Constants.Object_Modified;
                                pe.ProveedorEstadoId = pe.ProveedorEstadoId;
                                pe.ProveedorId = ProveedorId;
                                mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);
                            }

                        }
                    }


                }
                
                mobjUnitOfWork.Repository<Proveedor>().SaveEntity(oProveedorSave);
            }
            catch (Exception ex)
            {
                return new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
            }
            return null;
        }

        public async Task<EntityErrors> UpdateDatosContacto(NuevoProveedor oParam)
        {
            try
            {
                var ErrorCanal = await UpdateCanalOperacion(oParam);
                if (ErrorCanal != null)
                    return ErrorCanal;


                var ErrorDestinatario = await UpdateDestinatario(oParam);
                if (ErrorDestinatario != null)
                    return ErrorDestinatario;


                var ErrorCondicion = await UpdateCondicion(oParam);
                if (ErrorCondicion != null)
                    return ErrorCondicion;

                var ErrorObjetivos = await UpdateObjetivos(oParam);
                if (ErrorObjetivos != null)
                    return ErrorObjetivos;
            }
            catch (Exception ex)
            {
                return new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
            }
            return null;
        }

        public async Task<EntityErrors> UpdateCanalOperacion(NuevoProveedor oParam)
        {
            try
            {
                List<ProveedorCanalOperacion> oCanalSave;

                oCanalSave = await mobjUnitOfWork.Repository<ProveedorCanalOperacion>()
                                     .Queryable()
                                     .AsNoTracking()
                                     .Where(x => x.ProveedorId == oParam.ProveedorId)
                                     .ToListAsync();


                #region Eliminar

                foreach (var can in oCanalSave)
                {
                    if (!oParam.contacto.canalesOperacion.Contains(can.CanalOperacionId))
                    {
                        can.ObjectState = Constants.Object_Deleted;
                        mobjUnitOfWork.Repository<ProveedorCanalOperacion>().SaveEntity(can);
                    }
                }

                #endregion


                var id = mobjUnitOfWork.Repository<ProveedorCanalOperacion>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => x.ContactoCanalOperacionId)
                                    .DefaultIfEmpty(0)
                                    .Max();

                foreach (var can in oParam.contacto.canalesOperacion)
                {
                    if (!oCanalSave.Any(x => x.CanalOperacionId == can))
                    {
                        id = id + 1;
                        //crear nuevo objeto
                        var a = new ProveedorCanalOperacion() { ContactoCanalOperacionId = id, CanalOperacionId = can, NroItem = "1", ProveedorId = (int)oParam.ProveedorId, ObjectState = Constants.Object_Added };
                        mobjUnitOfWork.Repository<ProveedorCanalOperacion>().SaveEntity(a);
                    }
                }
            }
            catch (Exception ex)
            {
                return new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
            }
            return null;
        }

        public async Task<EntityErrors> UpdateDestinatario(NuevoProveedor oParam)
        {
            try
            {
                List<ProveedorDestinatario> oDestinatarioSave;

                oDestinatarioSave = await mobjUnitOfWork.Repository<ProveedorDestinatario>()
                                     .Queryable()
                                     .AsNoTracking()
                                     .Where(x => x.ProveedorId == oParam.ProveedorId)
                                     .ToListAsync();


                #region Eliminar

                foreach (var dest in oDestinatarioSave)
                {
                    if (!oParam.contacto.entregaA.Contains(dest.DestinatarioId))
                    {

                        dest.ObjectState = Constants.Object_Deleted;
                        mobjUnitOfWork.Repository<ProveedorDestinatario>().SaveEntity(dest);
                    }
                }

                #endregion
                var id = mobjUnitOfWork.Repository<ProveedorDestinatario>()
                                        .Queryable()
                                        .AsNoTracking()
                                        .Select(x => x.ContactoDestinatarioId)
                                        .DefaultIfEmpty(0)
                                        .Max();
                foreach (var can in oParam.contacto.entregaA)
                {
                    if (!oDestinatarioSave.Any(x => x.DestinatarioId == can))
                    {
                        //crear nuevo objeto
                        id = id + 1;
                        var a = new ProveedorDestinatario() { ContactoDestinatarioId = id, DestinatarioId = can, NroItem = 1, ProveedorId = (int)oParam.ProveedorId, ObjectState = Constants.Object_Added };
                        mobjUnitOfWork.Repository<ProveedorDestinatario>().SaveEntity(a);
                    }
                }
            }
            catch (Exception ex)
            {
                return new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
            }
            return null;
        }

        public async Task<EntityErrors> UpdateCondicion(NuevoProveedor oParam)
        {
            try
            {
                List<ProveedorCondicion> oCondicionSave;

                oCondicionSave = await mobjUnitOfWork.Repository<ProveedorCondicion>()
                                     .Queryable()
                                     .AsNoTracking()
                                     .Where(x => x.ProveedorId == oParam.ProveedorId)
                                     .ToListAsync();


                #region Eliminar

                foreach (var cond in oCondicionSave)
                {
                    if (!oParam.contacto.condPreferentes.Contains(cond.CondicionId))
                    {
                        cond.ObjectState = Constants.Object_Deleted;
                        mobjUnitOfWork.Repository<ProveedorCondicion>().SaveEntity(cond);
                    }
                }

                #endregion
                var id = mobjUnitOfWork.Repository<ProveedorCondicion>()
                                        .Queryable()
                                        .AsNoTracking()
                                        .Select(x => x.ContactoCondicionId)
                                        .DefaultIfEmpty(0)
                                        .Max();
                if (oParam.contacto != null)
                {
                    foreach (var cond in oParam.contacto.condPreferentes)
                    {
                        if (!oCondicionSave.Any(x => x.CondicionId == cond))
                        {
                            //crear nuevo objeto
                            id = id + 1;
                            var a = new ProveedorCondicion() { ContactoCondicionId = id, CondicionId = cond, NroItem = 1, ProveedorId = (int)oParam.ProveedorId, ObjectState = Constants.Object_Added };
                            mobjUnitOfWork.Repository<ProveedorCondicion>().SaveEntity(a);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
            }
            return null;
        }

        public async Task<EntityErrors> UpdateObjetivos(NuevoProveedor oParam)
        {
            List<Objetivo> oCampañaSave;
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable().AsNoTracking();

            oCampañaSave = await mobjUnitOfWork.Repository<Objetivo>()
                                     .Queryable()
                                     .Join(oMaterial, a => a.MaterialId, b => b.MaterialId, (a, b) => new { CAMP = a, MAT = b }) 
                                     .Where(x => x.CAMP.ProveedorId == oParam.ProveedorId && x.CAMP.CampañaId >= x.MAT.CampañaId)
                                     .Select(x => x.CAMP)
                                     .ToListAsync();



            #region Eliminar

            if (oParam.produccion.eliminarobjetivos.Count > 0)
            { 
                foreach (var camp in oCampañaSave)
                {
                    if (oParam.produccion.eliminarobjetivos.Any(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId))
                    {
                        camp.ObjectState = Constants.Object_Deleted;
                        mobjUnitOfWork.Repository<Objetivo>().SaveEntity(camp);
                    }
                }
            }

            #endregion

            #region Agregar
            var id = mobjUnitOfWork.Repository<Objetivo>()
                                        .Queryable()
                                        .AsNoTracking()
                                        .Select(x => x.ObjetivoId)
                                        .DefaultIfEmpty(0)
                                        .Max();

            if (oParam.produccion.objetivos != null && oParam.produccion.objetivos.Count > 0)
            {
                foreach (var obj in oParam.produccion.objetivos)
                {
                    if (!oCampañaSave.Any(x => x.CampañaId == obj.campañaId && x.MaterialId == obj.granoId))
                    {
                        id = id + 1;
                        var oCampañaMaterial = new Objetivo() { ObjetivoId = id, NroItem = 1, CampañaId = obj.campañaId, MaterialId = obj.granoId, ProveedorId = (int)oParam.ProveedorId, ToneladasObjetivos = Convert.ToDouble(obj.toneladasObjetivo),
                            ObjectState = Constants.Object_Added };
                        mobjUnitOfWork.Repository<Objetivo>().SaveEntity(oCampañaMaterial);
                    }
                }
            }
            #endregion

            #region Modificar

            foreach (var camp in oCampañaSave)
            {

                if (oParam.produccion.objetivos != null)
                { 
                    if (oParam.produccion.objetivos.Any(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId))
                    {
                        var mod = oParam.produccion.objetivos.Where(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId).First();
                        camp.CampañaId = mod.campañaId;
                        camp.MaterialId = mod.granoId;
                        camp.ToneladasObjetivos = Convert.ToDouble(mod.toneladasObjetivo);

                        camp.ObjectState = Constants.Object_Modified;


                        //var oCampañaMaterial = new CampañaMaterial() { CampañaId= camp., ProveedorId = (int)oParam.ProveedorId, ToneladasObjetivo = camp.ToneladasObjetivo, ObjectState = Constants.Object_Added };
                        mobjUnitOfWork.Repository<Objetivo>().SaveEntity(camp);
                    }
                }
            }



            #endregion
            return null;

        }

        public async Task<EntityErrors> UpdateContactoComerciales(NuevoProveedor oParam)
        {
            try
            {
                var idComercial = mobjUnitOfWork.Repository<ContactoComercial>()
                                        .Queryable()
                                        .AsNoTracking()
                                        .Select(x => x.ContactoComercialId)
                                        .DefaultIfEmpty(0)
                                        .Max();

                var idInteres = mobjUnitOfWork.Repository<ContactoComercialInteres>()
                                       .Queryable()
                                       .AsNoTracking()
                                       .Select(x => x.ContactoComercialInteresId)
                                       .DefaultIfEmpty(0)
                                       .Max();

                var oContactoSave = await mobjUnitOfWork.Repository<ContactoComercial>()
                                     .Queryable()
                                     .AsNoTracking()
                                     .Where(x => x.ProveedorId == oParam.ProveedorId)
                                     .ToListAsync();


                #region Eliminar

                foreach (var can in oContactoSave)
                {
                    if (oParam.contactocomercial != null)
                    {
                        if (!oParam.contactocomercial.Any(x => x.contactoComercialId == can.ContactoComercialId))
                        {
                            var oContactoComercialInteresEliminar = await mobjUnitOfWork.Repository<ContactoComercialInteres>()
                                         .Queryable()
                                         .Where(x => x.ContactoComercialId == can.ContactoComercialId)
                                         .ToListAsync();
                            foreach (var interes in oContactoComercialInteresEliminar)
                            {
                                interes.ObjectState = Constants.Object_Deleted;
                                mobjUnitOfWork.Repository<ContactoComercialInteres>().SaveEntity(interes);
                            }

                            can.ObjectState = Constants.Object_Deleted;
                            mobjUnitOfWork.Repository<ContactoComercial>().SaveEntity(can);
                        }
                    }
                    else
                    {
                        var oContactoComercialInteresEliminar = await mobjUnitOfWork.Repository<ContactoComercialInteres>()
                                        .Queryable()
                                        .Where(x => x.ContactoComercialId == can.ContactoComercialId)
                                        .ToListAsync();
                        foreach (var interes in oContactoComercialInteresEliminar)
                        {
                            interes.ObjectState = Constants.Object_Deleted;
                            mobjUnitOfWork.Repository<ContactoComercialInteres>().SaveEntity(interes);
                        }

                        can.ObjectState = Constants.Object_Deleted;
                        mobjUnitOfWork.Repository<ContactoComercial>().SaveEntity(can);
                    }
                }

                #endregion

                #region Agregar
                if (oParam.contactocomercial != null)
                {
                    foreach (var can in oParam.contactocomercial)
                    {
                        if (can.contactoComercialId == 0)
                        {
                            idComercial = idComercial + 1;

                            //crear nuevo objeto y agregar lo que falta
                            var contactoComercial = new ContactoComercial()
                            {
                                ContactoComercialId = idComercial,
                                ProveedorId = (int)oParam.ProveedorId,
                                Nombres = can.nombre,
                                Apellido = can.apellido,
                                Cargo = can.cargo,
                                FechaNacimiento = can.fechaNacimiento,
                                Puesto = can.puesto,
                                EsPrincipal = can.principal,
                                OtrosIntereses = can.otrosIntereses,
                                Telefono1 = can.telefonos[0].telefono,
                                Telefono2 = can.telefonos[1].telefono,
                                Telefono3 = can.telefonos[2].telefono,
                                TipoTelefono1Id = can.telefonos[0].tipoTelefono,
                                TipoTelefono2Id = can.telefonos[1].tipoTelefono,
                                TipoTelefono3Id = can.telefonos[2].tipoTelefono,
                                Email1 = can.emails[0],
                                Email2 = can.emails[1],
                                Email3 = can.emails[2],
                                ObjectState = Constants.Object_Added
                            };
                            mobjUnitOfWork.Repository<ContactoComercial>().SaveEntity(contactoComercial);

                            var nroItem = 1;

                            foreach (var valor in can.intereses)
                            {
                                idInteres = idInteres + 1;
                                var ContactoIntereses = new ContactoComercialInteres() { ContactoComercialInteresId = idInteres, NroItem = nroItem, InteresId = valor, ContactoComercialId = idComercial, ObjectState = Constants.Object_Added };
                                mobjUnitOfWork.Repository<ContactoComercialInteres>().SaveEntity(ContactoIntereses);
                                nroItem = nroItem + 1;
                            }
                        }
                    }
                }

                #endregion

                #region Modificar

                foreach (var con in oContactoSave)
                {
                    if (oParam.contactocomercial != null)
                    {


                        if (oParam.contactocomercial.Any(x => x.contactoComercialId == con.ContactoComercialId))
                        {
                            //Todo Agregar los objetos que faltan
                            var mod = oParam.contactocomercial.Where(x => x.contactoComercialId == con.ContactoComercialId).First();
                            con.Apellido = mod.apellido;
                            con.Nombres = mod.nombre;
                            con.ProveedorId = (int)oParam.ProveedorId;
                            con.OtrosIntereses = mod.otrosIntereses;
                            con.Puesto = mod.puesto;
                            con.Telefono1 = mod.telefonos[0].telefono;
                            con.Telefono2 = mod.telefonos[1].telefono;
                            con.Telefono3 = mod.telefonos[2].telefono;
                            con.TipoTelefono1Id = mod.telefonos[0].tipoTelefono;
                            con.TipoTelefono2Id = mod.telefonos[1].tipoTelefono;
                            con.TipoTelefono3Id = mod.telefonos[2].tipoTelefono;
                            con.Email1 = mod.emails[0];
                            con.Email2 = mod.emails[1];
                            con.Email3 = mod.emails[2];
                            con.EsPrincipal = mod.principal;
                            con.FechaNacimiento = mod.fechaNacimiento;
                            con.Cargo = mod.cargo;
                            con.ObjectState = Constants.Object_Modified;
                            mobjUnitOfWork.Repository<ContactoComercial>().SaveEntity(con);

                            var oContactosInteresesSave = await mobjUnitOfWork.Repository<ContactoComercialInteres>()
                                                         .Queryable()
                                                         .AsNoTracking()
                                                         .Where(x => x.ContactoComercialId == mod.contactoComercialId)
                                                         .ToListAsync();


                            #region Eliminar Intereses Contactos

                            foreach (var can in oContactosInteresesSave)
                            {
                                if (!mod.intereses.Contains(can.InteresId))
                                {
                                    can.ObjectState = Constants.Object_Deleted;
                                    mobjUnitOfWork.Repository<ContactoComercialInteres>().SaveEntity(can);
                                }
                            }

                            #endregion

                            #region Agregar Intereses Contactos
                            if (mod.intereses != null)
                            {
                                foreach (var can in mod.intereses)
                                {
                                    if (!oContactosInteresesSave.Any(x => x.InteresId == can))
                                    {
                                        idInteres = idInteres + 1;
                                        //crear nuevo objeto
                                        var a = new ContactoComercialInteres() { ContactoComercialInteresId = idInteres, ContactoComercialId = (int)mod.contactoComercialId, NroItem = 1, InteresId = can, ObjectState = Constants.Object_Added };
                                        mobjUnitOfWork.Repository<ContactoComercialInteres>().SaveEntity(a);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }



                #endregion



            }
            catch (Exception ex)
            {
                return new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
            }
            return null;
        }

        public async Task<EntityErrors> UpdateProduccion(NuevoProveedor oParam)
        {
            try
            {
                var idCampo = mobjUnitOfWork.Repository<Campo>()
                                        .Queryable()
                                        .AsNoTracking()
                                        .Select(x => x.CampoId)
                                        .DefaultIfEmpty(0)
                                        .Max();

                var idCampoMaterial = mobjUnitOfWork.Repository<CampoMaterial>()
                                       .Queryable()
                                       .AsNoTracking()
                                       .Select(x => x.CampoMaterialid)
                                       .DefaultIfEmpty(0)
                                       .Max();

                var oCampoSave = await mobjUnitOfWork.Repository<Campo>()
                                     .Queryable()
                                     .AsNoTracking()
                                     .Where(x => x.ProveedorId == oParam.ProveedorId)
                                     .ToListAsync();

                #region Eliminar

                foreach (var cam in oCampoSave)
                {
                    if (oParam.produccion.CamposProduccion != null)
                    {
                        if (!oParam.produccion.CamposProduccion.Any(x => x.CampoId == cam.CampoId))
                        {
                            var oCampoEliminar = await mobjUnitOfWork.Repository<CampoMaterial>()
                                         .Queryable()
                                         .AsNoTracking()
                                         .Where(x => x.CampoId == oParam.produccion.CampoId)
                                         .ToListAsync();

                            foreach (var campmat in oCampoEliminar)
                            {
                                if (!oParam.produccion.objetivos.Any(x => x.campañaId == campmat.CampañaId && x.granoId == campmat.MaterialId))
                                {
                                    campmat.ObjectState = Constants.Object_Deleted;
                                    mobjUnitOfWork.Repository<CampoMaterial>().SaveEntity(campmat);
                                }
                            }

                            cam.ObjectState = Constants.Object_Deleted;
                            mobjUnitOfWork.Repository<Campo>().SaveEntity(cam);
                        }
                    }
                    else
                    {
                        var oCampoEliminar = await mobjUnitOfWork.Repository<CampoMaterial>()
                                         .Queryable()
                                         .AsNoTracking()
                                         .Where(x => x.CampoId == cam.CampoId)
                                         .ToListAsync();




                        foreach (var campmat in oCampoEliminar)
                        {
                            campmat.ObjectState = Constants.Object_Deleted;
                            mobjUnitOfWork.Repository<CampoMaterial>().SaveEntity(campmat);
                        }

                        cam.ObjectState = Constants.Object_Deleted;
                        mobjUnitOfWork.Repository<Campo>().SaveEntity(cam);
                    }
                }
                #endregion

                #region Agregar
                if (oParam.produccion.CamposProduccion != null)
                {


                    foreach (var cam in oParam.produccion.CamposProduccion)
                    {
                        if (cam.CampoId.GetValueOrDefault(0) == 0)
                        {
                            idCampo = idCampo + 1;
                            var produccion = new Campo()
                            {
                                CampoId = idCampo,
                                Coordenadas = cam.coordenadas,
                                KMZfile = cam.archivoFileResult,
                                KMZnombre = cam.archivo,
                                LocalidadId = cam.localidad,
                                ArrendaPropia = cam.hectareas,

                                //HabilitadoSojaSustentable = Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust)),
                                NroItem = 1,
                                ProveedorId = (int)oParam.ProveedorId,

                                ObjectState = Constants.Object_Added

                            };
                            mobjUnitOfWork.Repository<Campo>().SaveEntity(produccion);

                            if (cam.granos != null)
                            {
                                foreach (var valor in cam.granos)
                                {
                                    if (valor.granoId != null && valor.campañaId != null)
                                    {
                                        idCampoMaterial = idCampoMaterial + 1;
                                        var ContactoIntereses = new CampoMaterial()
                                        {
                                            CampoMaterialid = idCampoMaterial,
                                            NroItem = 1,
                                            MaterialId = (int)valor.granoId,
                                            CampañaId = (int)valor.campañaId,
                                            CampoId = idCampo,
                                            Hectareas = valor.hectareas.HasValue ? valor.hectareas : null,
                                            Toneladas = valor.toneladas.HasValue ? valor.toneladas : null,
                                            ObjectState = Constants.Object_Added
                                        };
                                        mobjUnitOfWork.Repository<CampoMaterial>().SaveEntity(ContactoIntereses);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Modificar

                foreach (var campo in oCampoSave)
                {
                    if (oParam.produccion.CamposProduccion != null)
                    {
                        if (oParam.produccion.CamposProduccion.Any(x => x.CampoId == campo.CampoId))
                        {
                            var mod = oParam.produccion.CamposProduccion.Where(x => x.CampoId == campo.CampoId).First();

                            #region Set Campos
                            campo.CampoId = (int)mod.CampoId;
                            campo.Coordenadas = mod.coordenadas;
                            campo.KMZfile = mod.archivoFileResult;
                            campo.KMZnombre = mod.archivo;
                            campo.LocalidadId = mod.localidad;
                            campo.ArrendaPropia = mod.hectareas;
                            //cam.HabilitadoSojaSustentable = Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust)),
                            //cam.NroItem = 1,
                            campo.Coordenadas = mod.coordenadas;
                            campo.LocalidadId = mod.localidad;
                            campo.ProveedorId = (int)oParam.ProveedorId;
                            campo.ObjectState = Constants.Object_Modified;
                            mobjUnitOfWork.Repository<Campo>().SaveEntity(campo);
                            #endregion

                            var oCampoMaterialSave = await mobjUnitOfWork.Repository<CampoMaterial>()
                                                         .Queryable()
                                                         .AsNoTracking()
                                                         .Where(x => x.CampoId == mod.CampoId)
                                                         .ToListAsync();

                            #region Eliminar Campo Material
                            /*foreach (var can in oCampoMaterialSave)
                            {
                                if (mod.granos != null)
                                {
                                    if (!mod.granos.Any(x => x.granoId == can.MaterialId && x.campañaId == can.CampañaId))
                                    {
                                        can.ObjectState = Constants.Object_Deleted;
                                        mobjUnitOfWork.Repository<CampoMaterial>().SaveEntity(can);
                                    }
                                }
                            }*/

                            foreach (var can in oParam.produccion.CamposProduccion)
                            {
                                if (can.eliminarproduccion != null && can.eliminarproduccion.Count > 0)
                                {
                                    foreach (var camp in oCampoMaterialSave)
                                    {
                                        if (can.eliminarproduccion.Any(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId))
                                        {
                                            camp.ObjectState = Constants.Object_Deleted;
                                            mobjUnitOfWork.Repository<CampoMaterial>().SaveEntity(camp);
                                        }
                                    }
                                }

                            }



                            #endregion

                            #region Agregar Campo material
                            if (mod.granos != null)
                            {
                                foreach (var gra in mod.granos)
                                {
                                    if (!oCampoMaterialSave.Any(x => x.MaterialId == gra.granoId && x.CampañaId == gra.campañaId))
                                    {
                                        idCampoMaterial = idCampoMaterial + 1;
                                        var ContactoIntereses = new CampoMaterial()
                                        {
                                            CampoMaterialid = idCampoMaterial,
                                            NroItem = 1,
                                            MaterialId = gra.granoId,
                                            CampañaId = gra.campañaId,
                                            CampoId = (int)mod.CampoId,
                                            Hectareas = gra.hectareas,
                                            Toneladas = gra.toneladas,
                                            ObjectState = Constants.Object_Added
                                        };
                                        mobjUnitOfWork.Repository<CampoMaterial>().SaveEntity(ContactoIntereses);
                                    }
                                }
                            }
                            #endregion

                            #region Modificar Campo Material
                            //foreach (var campoModificar in oParam.produccion.CamposProduccion)
                            //{ 

                            foreach (var campmaterial in oCampoMaterialSave)
                            {
                                if (mod.granos != null)
                                {
                                    if (mod.granos.Any(x => x.granoId == campmaterial.MaterialId && x.campañaId == campmaterial.CampañaId))
                                    {
                                        var grano = mod.granos.Where(x => x.granoId == campmaterial.MaterialId && x.campañaId == campmaterial.CampañaId).First();
                                        campmaterial.CampoId = (int)mod.CampoId;
                                        campmaterial.CampañaId = grano.campañaId;
                                        campmaterial.MaterialId = grano.granoId;
                                        campmaterial.Hectareas = grano.hectareas;
                                        campmaterial.Toneladas = grano.toneladas;
                                        campmaterial.ObjectState = Constants.Object_Modified;
                                        mobjUnitOfWork.Repository<CampoMaterial>().SaveEntity(campmaterial);
                                    }
                                }
                            }
                            //}
                            #endregion
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                return new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
            }
            return null;
        }

        public async Task<EntityErrors> UpdateAlmacenamiento(NuevoProveedor oParam)
        {
            try
            {
                var idAcopio = mobjUnitOfWork.Repository<Acopio>()
                                        .Queryable()
                                        .AsNoTracking()
                                        .Select(x => x.AcopioId)
                                        .DefaultIfEmpty(0)
                                        .Max();

                var idAcopioMaterial = mobjUnitOfWork.Repository<AcopioMaterial>()
                                       .Queryable()
                                       .AsNoTracking()
                                       .Select(x => x.AcopioMaterialId)
                                       .DefaultIfEmpty(0)
                                       .Max();

                var idAcopioCampaña = mobjUnitOfWork.Repository<AcopioCampaña>()
                                       .Queryable()
                                       .AsNoTracking()
                                       .Select(x => x.AcopioCampañaId)
                                       .DefaultIfEmpty(0)
                                       .Max();

                var oAcopioSave = await mobjUnitOfWork.Repository<Acopio>()
                                 .Queryable()
                                 .AsNoTracking()
                                 .Where(x => x.ProveedorId == oParam.ProveedorId)
                                 .ToListAsync();

                #region Eliminar Almacenamiento

                foreach (var cam in oAcopioSave)
                {
                    if (oParam.almacenamiento != null)
                    {
                        if (!oParam.almacenamiento.CamposAlmacenamiento.Any(x => x.CampoId == cam.AcopioId))
                        {
                            var oCampoEliminar = await mobjUnitOfWork.Repository<AcopioMaterial>()
                                         .Queryable()
                                         .AsNoTracking()
                                         .Where(x => x.AcopioId == cam.AcopioId)
                                         .ToListAsync();

                            var oAcopioCampañaEliminar = await mobjUnitOfWork.Repository<AcopioCampaña>()
                                         .Queryable()
                                         .AsNoTracking()
                                         .Where(x => x.AcopioId == cam.AcopioId)
                                         .ToListAsync();


                            #region eliminar acopio material
                            foreach (var campmat in oCampoEliminar)
                            {
                                campmat.ObjectState = Constants.Object_Deleted;
                                mobjUnitOfWork.Repository<AcopioMaterial>().SaveEntity(campmat);
                            }
                            #endregion

                            #region Eliminar Acopio Campaña
                            foreach (var acopcamp in oAcopioCampañaEliminar)
                            {
                                acopcamp.ObjectState = Constants.Object_Deleted;
                                mobjUnitOfWork.Repository<AcopioCampaña>().SaveEntity(acopcamp);
                            }
                            #endregion

                            cam.ObjectState = Constants.Object_Deleted;
                            mobjUnitOfWork.Repository<Acopio>().SaveEntity(cam);



                        }
                    }
                    else
                    {
                        var oCampoEliminar = await mobjUnitOfWork.Repository<AcopioMaterial>()
                                         .Queryable()
                                         .AsNoTracking()
                                         .Where(x => x.AcopioId == cam.AcopioId)
                                         .ToListAsync();

                        var oAcopioCampañaEliminar = await mobjUnitOfWork.Repository<AcopioCampaña>()
                                         .Queryable()
                                         .AsNoTracking()
                                         .Where(x => x.AcopioId == cam.AcopioId)
                                         .ToListAsync();

                        foreach (var campmat in oCampoEliminar)
                        {
                            campmat.ObjectState = Constants.Object_Deleted;
                            mobjUnitOfWork.Repository<AcopioMaterial>().SaveEntity(campmat);
                        }

                        #region Eliminar Acopio Campaña
                        foreach (var acopcamp in oAcopioCampañaEliminar)
                        {
                            acopcamp.ObjectState = Constants.Object_Deleted;
                            mobjUnitOfWork.Repository<AcopioCampaña>().SaveEntity(acopcamp);
                        }
                        #endregion

                        cam.ObjectState = Constants.Object_Deleted;
                        mobjUnitOfWork.Repository<Acopio>().SaveEntity(cam);
                    }
                }
                #endregion

                #region Agregar
                if (oParam.almacenamiento != null)
                {


                    foreach (var cam in oParam.almacenamiento.CamposAlmacenamiento)
                    {
                        if (cam.CampoId.GetValueOrDefault(0) == 0)
                        {
                            idAcopio = idAcopio + 1;
                            var acopio = new Acopio()
                            {
                                AcopioId = idAcopio,
                                Coordenadas = cam.coordenadasAlmacenamiento,
                                KMZfile = cam.archivoFileResult,
                                KMZnombre = cam.archivo,
                                LocalidadId = cam.localidad,
                                NroItem = 1,
                                ProveedorId = (int)oParam.ProveedorId,

                                ObjectState = Constants.Object_Added

                            };

                            mobjUnitOfWork.Repository<Acopio>().SaveEntity(acopio);

                            if (cam.granosAlmacenamientoGrano != null)
                            {
                                foreach (var valor in cam.granosAlmacenamientoGrano)
                                {
                                    idAcopioMaterial = idAcopioMaterial + 1;
                                    var ContactoIntereses = new AcopioMaterial()
                                    {
                                        AcopioMaterialId = idAcopioMaterial,
                                        NroItem = 1,
                                        CampañaId = valor.campañaId,
                                        AcopioId = idAcopio,
                                        MaterialId = valor.granoId,
                                        Toneladas = valor.toneladasAlmacenamiento,
                                        ObjectState = Constants.Object_Added
                                    };
                                    mobjUnitOfWork.Repository<AcopioMaterial>().SaveEntity(ContactoIntereses);

                                }
                            }
                            if (cam.granosAlmacenamiento != null) { 
                                foreach (var acopiocamp in cam.granosAlmacenamiento)
                                {
                                    idAcopioCampaña = idAcopioCampaña + 1;
                                    var ContactoCampaña = new AcopioCampaña()
                                    {
                                        AcopioCampañaId = idAcopioCampaña + 1,
                                        NroItem = 1,
                                        CampañaId = acopiocamp.campañaId,
                                        AcopioId = idAcopio,
                                        HasArrendadas = acopiocamp.hasArrendadas,
                                        Toneladas = acopiocamp.toneladasAlmacenamiento,
                                        ObjectState = Constants.Object_Added
                                    };
                                    mobjUnitOfWork.Repository<AcopioCampaña>().SaveEntity(ContactoCampaña);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Modificar Almacenamiento

                foreach (var acopio in oAcopioSave)
                {
                    if (oParam.almacenamiento != null)
                    {
                        if (oParam.almacenamiento.CamposAlmacenamiento.Any(x => x.CampoId == acopio.AcopioId))
                        {
                            var mod = oParam.almacenamiento.CamposAlmacenamiento.Where(x => x.CampoId == acopio.AcopioId).First();

                            #region Set Campos
                            acopio.AcopioId = (int)mod.CampoId;
                            acopio.Coordenadas = mod.coordenadasAlmacenamiento;
                            acopio.KMZfile = mod.archivoFileResult;
                            acopio.KMZnombre = mod.archivo;
                            acopio.LocalidadId = mod.localidad;
                            acopio.LocalidadId = mod.localidad;
                            acopio.ProveedorId = (int)oParam.ProveedorId;
                            acopio.ObjectState = Constants.Object_Modified;
                            mobjUnitOfWork.Repository<Acopio>().SaveEntity(acopio);
                            #endregion

                            var oCampoMaterialSave = await mobjUnitOfWork.Repository<AcopioMaterial>()
                                                         .Queryable()
                                                         .AsNoTracking()
                                                         .Where(x => x.AcopioId == mod.CampoId)
                                                         .ToListAsync();

                            var oCampoCampañaSave = await mobjUnitOfWork.Repository<AcopioCampaña>()
                                                         .Queryable()
                                                         .AsNoTracking()
                                                         .Where(x => x.AcopioId == mod.CampoId)
                                                         .ToListAsync();

                            #region Eliminar Campo Material
                            /*foreach (var can in oCampoMaterialSave)
                            {
                                if (mod.granosAlmacenamientoGrano != null)
                                {
                                    if (!mod.granosAlmacenamientoGrano.Any(x => x.campañaId == can.CampañaId && x.granoId == can.MaterialId))
                                    {
                                        can.ObjectState = Constants.Object_Deleted;
                                        mobjUnitOfWork.Repository<AcopioMaterial>().SaveEntity(can);
                                    }
                                }
                            }*/

                            foreach (var can in oParam.almacenamiento.CamposAlmacenamiento)
                            {
                                if (can.eliminargranoalmacenamientograno != null && can.eliminargranoalmacenamientograno.Count > 0)
                                {
                                    foreach (var camp in oCampoMaterialSave)
                                    {
                                        if (can.eliminargranoalmacenamientograno.Any(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId))
                                        {
                                            camp.ObjectState = Constants.Object_Deleted;
                                            mobjUnitOfWork.Repository<AcopioMaterial>().SaveEntity(camp);
                                        }
                                    }
                                }

                            }



                            /*
                            foreach (var campacop in oCampoCampañaSave)
                            {
                                if (!mod.granosAlmacenamiento.Any(x => x.campañaId == campacop.CampañaId))
                                {
                                    campacop.ObjectState = Constants.Object_Deleted;
                                    mobjUnitOfWork.Repository<AcopioCampaña>().SaveEntity(campacop);
                                }
                            }*/

                            foreach (var can in oParam.almacenamiento.CamposAlmacenamiento)
                            {
                                if (can.eliminargranoalmacenamiento != null && can.eliminargranoalmacenamiento.Count > 0)
                                {
                                    foreach (var camp in oCampoCampañaSave)
                                    {
                                        if (can.eliminargranoalmacenamiento.Any(x => x.campañaId == camp.CampañaId && x.hasArrendadas == camp.HasArrendadas))
                                        {
                                            camp.ObjectState = Constants.Object_Deleted;
                                            mobjUnitOfWork.Repository<AcopioCampaña>().SaveEntity(camp);
                                        }
                                    }
                                }

                            }

                            #endregion

                            #region Agregar Campo material
                            if (mod.granosAlmacenamientoGrano != null)
                            {
                                foreach (var gra in mod.granosAlmacenamientoGrano)
                                {
                                    if (!oCampoMaterialSave.Any(x => x.CampañaId == gra.campañaId && x.MaterialId == gra.granoId))
                                    {
                                        idAcopioMaterial = idAcopioMaterial + 1;
                                        var ContactoIntereses = new AcopioMaterial()
                                        {
                                            AcopioMaterialId = idAcopioMaterial,
                                            NroItem = 1,
                                            CampañaId = gra.campañaId,
                                            MaterialId = gra.granoId,
                                            AcopioId = acopio.AcopioId,
                                            Toneladas = gra.toneladasAlmacenamiento,
                                            ObjectState = Constants.Object_Added
                                        };
                                        mobjUnitOfWork.Repository<AcopioMaterial>().SaveEntity(ContactoIntereses);
                                    }

                                }
                            }
                            #endregion

                            #region Agregar Campo Campaña
                            if (mod.granosAlmacenamiento != null)
                            {
                                foreach (var camp in mod.granosAlmacenamiento)
                                {
                                    if (!oCampoCampañaSave.Any(x => x.CampañaId == camp.campañaId))
                                    {
                                        idAcopioCampaña = idAcopioCampaña + 1;
                                        var ContactoCampaña = new AcopioCampaña()
                                        {
                                            AcopioCampañaId = idAcopioCampaña,
                                            NroItem = 1,
                                            CampañaId = camp.campañaId,
                                            HasArrendadas = camp.hasArrendadas,
                                            AcopioId = acopio.AcopioId,
                                            Toneladas = camp.toneladasAlmacenamiento,
                                            ObjectState = Constants.Object_Added
                                        };
                                        mobjUnitOfWork.Repository<AcopioCampaña>().SaveEntity(ContactoCampaña);
                                    }
                                }
                            }
                            #endregion

                            #region Modificar Campo Material
                            foreach (var campmaterial in oCampoMaterialSave)
                            {
                                if (mod.granosAlmacenamientoGrano != null)
                                {
                                    if (mod.granosAlmacenamientoGrano.Any(x => x.campañaId == campmaterial.CampañaId && x.granoId == campmaterial.MaterialId))
                                    {
                                        var grano = mod.granosAlmacenamientoGrano.Where(x => x.campañaId == campmaterial.CampañaId && x.granoId == campmaterial.MaterialId).First();
                                        campmaterial.AcopioId = (int)mod.CampoId;
                                        campmaterial.CampañaId = grano.campañaId;
                                        campmaterial.Toneladas = grano.toneladasAlmacenamiento;
                                        campmaterial.MaterialId = grano.granoId;
                                        campmaterial.ObjectState = Constants.Object_Modified;
                                        mobjUnitOfWork.Repository<AcopioMaterial>().SaveEntity(campmaterial);
                                    }
                                }
                            }
                            #endregion

                            #region Modificar Campo Campaña
                            foreach (var campcampaña in oCampoCampañaSave)
                            {
                                if (mod.granosAlmacenamiento != null)
                                {
                                    if (mod.granosAlmacenamiento.Any(x => x.campañaId == campcampaña.CampañaId))
                                    {
                                        var campaña = mod.granosAlmacenamiento.Where(x => x.campañaId == campcampaña.CampañaId).First();
                                        campcampaña.AcopioId = (int)mod.CampoId;
                                        campcampaña.CampañaId = campaña.campañaId;
                                        campcampaña.Toneladas = campaña.toneladasAlmacenamiento;
                                        campcampaña.HasArrendadas = campaña.hasArrendadas;
                                        campcampaña.ObjectState = Constants.Object_Modified;
                                        mobjUnitOfWork.Repository<AcopioCampaña>().SaveEntity(campcampaña);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                return new EntityErrors() { HayError = true, ListaErrores = new List<ErrorMessage>() { new ErrorMessage() { Message = ex.Message } } };
            }
            return null;
        }

        private Historial DevolverHistorial(int proveedorId, string Usuario)
        {
            var res = new Historial();

            res = Comprar(proveedorId, Usuario);
            

            return res;

        }

        public Historial Comprar(int proveedorId, string UsuarioDirectory)
        {
            Historial historial = new Historial();
            List<GranoHistorial> list = new List<GranoHistorial>();
            List<campañaTotal> listCampañaTotal = new List<campañaTotal>();

            GranoHistorial gr = null;
            CampañaHistorial cH = null;
            GranoTotales gt = null;
            campañaTotal ct = null;

            try
            {
                List<ZMPES5130> hist = new List<ZMPES5130>();
                ZMPES5130 zmp = null;

                var oMateriales = mobjUnitOfWork.Repository<Material>()
                    .Queryable()
                    .AsNoTracking()
                    .ToList();

                var oCampaña = mobjUnitOfWork.Repository<Campaña>()
                    .Queryable()
                    .AsNoTracking()
                    .ToList();

                var oProveedor = mobjUnitOfWork.Repository<Proveedor>()
                                .Queryable()
                                .AsNoTracking()
                                .Where(x => x.ProveedorId == proveedorId)
                                .SingleOrDefault();

                var oCampañaMaterial = mobjUnitOfWork.Repository<CampañaMaterial>()
                                       .Queryable()
                                       .AsNoTracking()
                                       .Where(x => x.ProveedorId == proveedorId)
                                       .ToList();

                var listMateriales = oCampañaMaterial.GroupBy(x => new { x.MaterialId }).Select(x => x.Key.MaterialId).ToList();


                //foreach (var campañamaterial in oCampañaMaterial)
                foreach (var MaterialId in listMateriales)
                {

                    int? CampañaId;

                    var oMaterial = oMateriales.FirstOrDefault(x => x.MaterialId == MaterialId);

                    if (oMaterial != null)
                    {
                        CampañaId = oMaterial.CampañaId;



                        var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
                        int? ComercialId = 0;

                        if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && UsuarioDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                        {
                            string aux = ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower();
                            ComercialId = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == aux).ComercialId;
                        }
                        else
                        {
                            ComercialId = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == UsuarioDirectory.ToLower()).ComercialId;
                        }




                        /*********************** Campaña Anterior *****************************************************/
                        var oCampañaMaterialAnt = oCampañaMaterial
                                                  .Where(x => x.ProveedorId == proveedorId && x.CampañaId == (CampañaId - 1) && x.MaterialId == MaterialId)
                                                  .ToList();

                        foreach (var cmaux in oCampañaMaterialAnt)
                        {
                            /*var oCampañaMaterialPorMes = mobjUnitOfWork.Repository<CampañaMaterialPorMes>()
                                                     .Queryable()
                                                     .AsNoTracking()
                                                     .Where(x => x.CampañaMaterialId == cmaux.CampañaMaterialId)
                                                     .ToList();*/

                            var oCampañaMaterialPorMes = mobjUnitOfWork.SelStore<CampañaMaterialPorMes>("DataAgro_ComprasPorComercialId", ComercialId, cmaux.CampañaMaterialId).ToList();


                            foreach (var cmpm in oCampañaMaterialPorMes)
                            {
                                zmp = new ZMPES5130();
                                zmp.ANIO = cmpm.Año.Value.ToString();
                                zmp.COSECHA = oCampaña.Where(x => x.CampañaId == cmaux.CampañaId).ToList()[0].Descripcion;
                                zmp.MATERIAL = MaterialId.ToString();
                                zmp.MES = cmpm.Mes.Value.ToString();
                                zmp.TN_COMPRADAS = decimal.Parse(cmpm.Toneladas.Value.ToString());
                                zmp.VENDEDOR = oProveedor.CUIT;
                                hist.Add(zmp);
                            }

                        }


                        /*********************** Campaña Actual  *****************************************************/
                        var oCampañaMaterialAct = oCampañaMaterial
                                                  .Where(x => x.ProveedorId == proveedorId && x.CampañaId == CampañaId && x.MaterialId == MaterialId)
                                                  .ToList();

                        foreach (var cmaux in oCampañaMaterialAct)
                        {
                            var oCampañaMaterialPorMes = mobjUnitOfWork.SelStore<CampañaMaterialPorMes>("DataAgro_ComprasPorComercialId", ComercialId, cmaux.CampañaMaterialId).ToList();

                            /*mobjUnitOfWork.Repository<CampañaMaterialPorMes>()
                                                 .Queryable()
                                                 .AsNoTracking()
                                                 .Where(x => x.CampañaMaterialId == cmaux.CampañaMaterialId)
                                                 .ToList();
                                                 */
                            foreach (var cmpm in oCampañaMaterialPorMes)
                            {
                                zmp = new ZMPES5130();
                                zmp.ANIO = cmpm.Año.Value.ToString();
                                zmp.COSECHA = oCampaña.Where(x => x.CampañaId== cmaux.CampañaId).ToList()[0].Descripcion;
                                zmp.MATERIAL = MaterialId.ToString();
                                zmp.MES = cmpm.Mes.Value.ToString();
                                zmp.TN_COMPRADAS = decimal.Parse(cmpm.Toneladas.Value.ToString());
                                zmp.VENDEDOR = oProveedor.CUIT;
                                hist.Add(zmp);
                            }


                        }



                            /*********************** Campaña Nueva *****************************************************/

                            var oCampañaMaterialSig = oCampañaMaterial
                                                  .Where(x => x.ProveedorId == proveedorId && x.CampañaId == (CampañaId + 1) && x.MaterialId == MaterialId)
                                                  .ToList();
                        
                        foreach (var cmaux in oCampañaMaterialSig)
                        {


                            var oCampañaMaterialPorMes = mobjUnitOfWork.SelStore<CampañaMaterialPorMes>("DataAgro_ComprasPorComercialId", ComercialId, cmaux.CampañaMaterialId).ToList();

                            /*mobjUnitOfWork.Repository<CampañaMaterialPorMes>()
                                                     .Queryable()
                                                     .AsNoTracking()
                                                     .Where(x => x.CampañaMaterialId == cmaux.CampañaMaterialId)
                                                     .ToList();*/
                            //aca arriba falta agregar la validacion por comercial.


                            foreach (var cmpm in oCampañaMaterialPorMes)
                            {
                                zmp = new ZMPES5130();
                                zmp.ANIO = cmpm.Año.Value.ToString();
                                zmp.COSECHA = oCampaña.Where(x => x.CampañaId == cmaux.CampañaId).ToList()[0].Descripcion;
                                zmp.MATERIAL = MaterialId.ToString();
                                zmp.MES = cmpm.Mes.Value.ToString();
                                zmp.TN_COMPRADAS = decimal.Parse(cmpm.Toneladas.Value.ToString());
                                zmp.VENDEDOR = oProveedor.CUIT;
                                hist.Add(zmp);
                            }
                        }
                    }
                }
                /*
                var SapCompras = new ComprasAgent();
             
                var hist = SapCompras.Comprar(oProveedor.CUIT, UsuarioDirectory);
                */
                var listMaterial = hist.GroupBy(z => z.MATERIAL).ToList();
                ct = new campañaTotal();
                ct.Nombre = "0";

                foreach (var mat in listMaterial)
                {
                    var Camp = hist.Where(x => x.MATERIAL == mat.Key).GroupBy(z => z.COSECHA);

                    gr = new GranoHistorial() { Nombre = oMateriales.Where(x => x.MaterialId.ToString() == mat.Key).First().Descripcion };

                    ct.grano.Add(new GranoTotales()
                    {
                        Grano = oMateriales.Where(x => x.MaterialId.ToString() == mat.Key).First().Descripcion,
                        Total = hist.Where(x => x.MATERIAL == mat.Key).Sum(x => x.TN_COMPRADAS).ToString()
                    });

                    foreach (var camp in Camp)
                    {
                        var GranosPorCampaña = hist.Where(x => x.MATERIAL == mat.Key && x.COSECHA == camp.Key)
                            .Select(x => new CampañaPorMes() { Mes = int.Parse(x.MES),Año= int.Parse(x.ANIO),  Total = (double)x.TN_COMPRADAS })
                            .OrderBy(x=> x.Año).ThenBy(x=> x.Mes).ToList();

                        gr.campañas.Add(new CampañaHistorial() { Nombre = camp.Key, Campañas = GranosPorCampaña });
                    }

                    list.Add(gr);
                }

                listCampañaTotal.Add(ct);

                historial.HistorialGrano = list;

                var listCampaña = hist.GroupBy(z => z.COSECHA).ToList();

                foreach (var ca in listCampaña)
                {
                    ct = new campañaTotal();
                    ct.Nombre = ca.Key;
                    var materiales = hist.Where(x => x.COSECHA == ca.Key).GroupBy(z => z.MATERIAL);

                    foreach (var m in materiales)
                    {
                        var total = hist.Where(x => x.MATERIAL == m.Key && x.COSECHA == ca.Key).Sum(x => x.TN_COMPRADAS);
                        ct.grano.Add(new GranoTotales() { Grano = oMateriales.Where(x => x.MaterialId.ToString() == m.Key).First().Descripcion, Total = total.ToString() });
                    }
                    listCampañaTotal.Add(ct);
                }

                historial.camp = listCampañaTotal;

            }
            catch (Exception ex)
            {
                throw;
            }
            return historial;
        }

        private int DevolverIdMes(string mES)
        {
            switch (mES.ToLower())
            {

                case "enero":
                    return 1;
                case "febrero":
                    return 2;
                case "marzo":
                    return 3;
                case "abril":
                    return 4;
                case "mayo":
                    return 5;
                case "junio":
                    return 6;
                case "julio":
                    return 7;
                case "agosto":
                    return 8;
                case "septiembre":
                    return 9;
                case "octubre":
                    return 10;
                case "noviembre":
                    return 11;
                case "diciembre":
                    return 12;
            }
            return 1;
        }

        public async Task<List<ReporteProveedor>> ObtenerReporteProveedor(string Valor, string idActiveDirectory)
        {

            var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.IdActiveDirectory.ToLower() == idActiveDirectory.ToLower());

            return mobjUnitOfWork.SelStore<ReporteProveedor>("DataAgro_ReporteProveedor", Valor, oComerciales.ComercialId).ToList();
        }


        public async Task<List<BusquedaHome>> DevolverProveedores(string filtro)
        {
            var query = mobjUnitOfWork.SelStore<BusquedaHome>("DataAgro_BusquedaProveedores", filtro);
            return query.ToList();
        }


    }

    public class ZMPES5130
    {
        public string VENDEDOR;

        public string MATERIAL;

        public string COSECHA;

        public string MES;

        public string ANIO;

        public decimal TN_COMPRADAS;
    }
}
