using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{


    public class ContratoManager : IContratoManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;
        
        private IMaterialManager mobjMaterialManager;
        private ITipoNegocioManager mobjTipoNegocioManager;
        private ICampañaManager   mobjCampaniaManager;
        private IProvinciaManager mobjProvinciaManager;
        private ILocalidadManager mobjLocalidadManager;
        private IProveedorManager mobjProveedorManager;
        private IComercialManager mobjComercialManager;

        
        public ContratoManager(ILogger logger, IMSContextProvider oMSContextProvider, 
            IMaterialManager oMSMaterialManager, ITipoNegocioManager oMSTipoNegocioManager,
            ICampañaManager oMSCampaniaManager, IProvinciaManager oMSProvinciaManager, 
            ILocalidadManager oMSLocalidadManager, IProveedorManager oMSProveedorManager, 
            IComercialManager oMSComercialManager)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
            mobjMaterialManager = oMSMaterialManager;
            mobjCampaniaManager = oMSCampaniaManager;
            mobjProvinciaManager = oMSProvinciaManager;
            mobjLocalidadManager = oMSLocalidadManager;
            mobjProveedorManager = oMSProveedorManager;
            mobjComercialManager = oMSComercialManager;
            mobjTipoNegocioManager = oMSTipoNegocioManager;
        }

        public async Task<DatosIniContrato> TraerDatosCombo()
        {
            var datosCombo = new DatosIniContrato();
            
            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable().AsNoTracking();

            datosCombo.prov = await mobjUnitOfWork.Repository<Provincia>()
                                .Queryable()
                                .AsNoTracking()
                                .Join(oLocalidad, a => a.ProvinciaId, b => b.ProvinciaId, (a, b) => new { P = a, L = b })
                                .GroupBy(x => new { x.P.ProvinciaId, x.P.Nombre })
                                .Select(x => new ProvinciaQry() { Provinciaid = x.Key.ProvinciaId, Nombre = x.Key.Nombre }).ToListAsync();

            datosCombo.loc = new List<LocalidadQry>();

            datosCombo.campaña = await mobjUnitOfWork.Repository<Campaña>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion }).ToListAsync();

            datosCombo.material = await mobjUnitOfWork.Repository<Material>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion }).ToListAsync();

            datosCombo.moneda = await mobjUnitOfWork.Repository<Moneda>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion }).ToListAsync();

            datosCombo.comercial = await mobjUnitOfWork.Repository<Comercial>()
                             .Queryable()
                             .AsNoTracking()
                             .OrderBy(x => x.Nombres)
                             .ThenBy(x => x.Apellido)
                             .Where(x =>  x.PerfilId  == (int)EnumPerfil.Comercial || x.PerfilId == (int)EnumPerfil.Jefe || x.PerfilId == (int)EnumPerfil.Mesa)
                             .Select(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }).ToListAsync();

            datosCombo.monedaSustentable = await mobjUnitOfWork.Repository<Moneda>()
                                 .Queryable()
                                 .AsNoTracking()
                                 .Select(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion }).ToListAsync();

            datosCombo.proveedor = await mobjUnitOfWork.Repository<Proveedor>()
                               .Queryable()
                               .AsNoTracking()
                               .OrderBy(x => x.RazonSocial)
                               .Select(x => new ProveedorQry() { ProveedorId = x.ProveedorId, Descripcion = x.RazonSocial }).ToListAsync();
            
            datosCombo.tiponegocio = await mobjUnitOfWork.Repository<TipoNegocio>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new TipoNegocioQry() { TipoNegocioId = x.TipoNegocioId, Descripcion = x.Descripcion }).ToListAsync();

            datosCombo.Clasificacion = await mobjUnitOfWork.Repository<ClasificacionCompraNet>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new ClasificacionCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion }).ToListAsync();

            datosCombo.Bolsa = await mobjUnitOfWork.Repository<BolsaCompraNet>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new BolsaCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion }).ToListAsync();
            datosCombo.Destino = await mobjUnitOfWork.Repository<Centro>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion }).ToListAsync();
            datosCombo.Condicion = await mobjUnitOfWork.Repository<CondicionFijacion>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new CondicionFijacionQry() { Id = x.Id, Descripcion = x.Descripcion }).ToListAsync();
            datosCombo.Standard = await mobjUnitOfWork.Repository<StandardDeCalidad>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new StandardDeCalidadQry() { Id = x.Id, Descripcion = x.Descripcion }).ToListAsync();
            
            Array estadosValues = Enum.GetValues(typeof(EnumEstadoContrato));

            foreach (int estadoValue in estadosValues) {
                string estadoName = Enum.GetName(typeof(EnumEstadoContrato), estadoValue);

                EstadosContratos item = new EstadosContratos(estadoValue, estadoName);

                datosCombo.estadoContrato.Add(item);
            }

            return datosCombo;
        }

        private List<ErrorMessage> Validar(Contrato oParam, List<ErrorMessage> oErrorMessages)
        {

            if (oParam.ProveedorId == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Proveedor' no debe estar vacio", "ProveedorId"));
            }
            if (oParam.MaterialId == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Material' no debe estar vacio", "Material"));
            }

            if (oParam.Cantidad == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Cantidad' no debe estar vacio", "Cantidad"));
            }
            if (oParam.Precio == 0 && oParam.TipoNegocioId != 1)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Precio' no debe estar vacio", "Precio"));
            }
            if (oParam.LocalidadId == null && oParam.TipoNegocioId == 1 || oParam.LocalidadId == null && oParam.TipoNegocioId == 2) {
                oErrorMessages.Add(new ErrorMessage("El campo 'Localidad' no debe estar vacio", "LocalidadId"));
            }
            if (oParam.ProvinciaId == null && oParam.TipoNegocioId == 1 || oParam.ProvinciaId == null && oParam.TipoNegocioId == 2) {
                oErrorMessages.Add(new ErrorMessage("El campo 'Provincia' no debe estar vacio", "ProvinciaId"));
            }
            if (oParam.FechaEntrega.Year == 1)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Fecha de Entrega' no debe estar vacio", "FechaEntrega"));
            }
            if (oParam.FechaDesde.Year == 1)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Fecha Desde' no debe estar vacio", "FechaDesde"));
            }
            if (oParam.FechaHasta.Year == 1)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Fecha Hasta' no debe estar vacio", "FechaHasta"));
            }
            if (oParam.FechaHasta.Year == 1)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Fecha Hasta' no debe estar vacio", "FechaHasta"));
            }
            if (string.IsNullOrEmpty(oParam.MonedaId))
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Moneda' no debe estar vacio", "MonedaId"));
            }
            if (oParam.TipoNegocioId == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Tipo de Negocio' no debe estar vacio", "TipoNegocioId"));
            }
            if (oParam.CampanaId == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Campaña' no debe estar vacio", "CampañaId"));
            }
            if (!oParam.ComercialId.HasValue || oParam.ComercialId == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Comercial' no debe estar vacio", "ComercialId"));
            }
            if (oParam.ProvinciaId == 1 && oParam.EstablecimientoPropio == null)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Establecimiento' no debe estar vacio cuando Provincia es Buenos Aires", "EstablecimientoPropio"));
            }
            if (oParam.TipoNegocioId==1 && (oParam.CondicionFijacionId == null || oParam.DesdeFijacion == null || oParam.HastaFijacion == null))
            {
                oErrorMessages.Add(new ErrorMessage("El 'Plazos y Topes de Fijación' no debe estar vacio cuando el contrato es 'A FIJAR'", "EstablecimientoPropio"));
            }
            return oErrorMessages;

        }

        public async Task<Contrato> TraerContratoAsync(int ContratoId)
        {
            var oContrato = await mobjUnitOfWork.Repository<Contrato>()
                           .Queryable()
                           .Where(x => x.ContratoId == ContratoId)
                           .SingleOrDefaultAsync();

            oContrato.ObjectState = Constants.Object_Modified;

            return oContrato;
        }

        public async Task<GrabarContratoResult> GrabarAmpliacionContrato(Contrato oContrato) {

            Contrato oContratoSave;

            var oEntityErrors = new GrabarContratoResult();
            oEntityErrors.Errores = new List<ErrorMessage>();

            oContratoSave = await TraerContratoAsync(oContrato.ContratoId);

            if (oContratoSave != null && (oContratoSave.Estado == (int)EnumEstadoContrato.Confirmado))
            {

                oContratoSave.Ampliaciones = oContrato.Ampliaciones.Value;
                oContratoSave.Estado = oContratoSave.Base == true ? (int)EnumEstadoContrato.Oferta : (int)EnumEstadoContrato.Pendiente;

                mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

                await mobjUnitOfWork.SaveChangesAsync();
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("El contrato no se puede ampliar"));
            }

            return oEntityErrors;
        }

        public async Task<GrabarContratoResult> GrabarContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            Validar(oContrato, oEntityErrors.Errores);

            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }

            Contrato oContratoSave;

            if (oContrato.ContratoId == 0)
            {
                oContratoSave = new Contrato()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oContratoSave = await TraerContratoAsync(oContrato.ContratoId);
                if (oContratoSave.Estado > (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("El contrato no se puede modificar"));
                    return oEntityErrors;
                }
            }

            oContratoSave.MaterialId = oContrato.MaterialId;
            oContratoSave.TipoNegocioId = oContrato.TipoNegocioId;
            oContratoSave.Cantidad = oContrato.Cantidad;
            oContratoSave.Precio = oContrato.Precio;
            oContratoSave.FechaEntrega = oContrato.FechaEntrega;
            oContratoSave.CampanaId = oContrato.CampanaId;
            oContratoSave.FechaDesde = oContrato.FechaDesde;
            oContratoSave.FechaHasta = oContrato.FechaHasta;
            oContratoSave.ProveedorId = oContrato.ProveedorId;
            oContratoSave.MonedaId = oContrato.MonedaId;
            oContratoSave.GrupoCompra = oContrato.GrupoCompra;
            oContratoSave.ComercialId = oContrato.ComercialId;
            oContratoSave.LocalidadId = oContrato.LocalidadId;
            oContratoSave.UsuarioId = oContrato.UsuarioId;
            oContratoSave.ProvinciaId = oContrato.ProvinciaId;
            oContratoSave.Base = oContrato.Base;
            oContratoSave.ImporteSustentable = oContrato.ImporteSustentable;
            oContratoSave.MonedaIdSustentable = oContrato.MonedaIdSustentable;
            oContratoSave.FechaDolarizado = oContrato.FechaDolarizado;
            oContratoSave.DiasPesificado = oContrato.DiasPesificado;
            oContratoSave.NoInformaSio = oContrato.NoInformaSio;
            oContratoSave.TrigoEspecial = oContrato.TrigoEspecial;
            oContratoSave.Estado = oContrato.Estado;
            oContratoSave.UsuarioId = oContrato.UsuarioId;
            oContratoSave.Ampliaciones = oContrato.Ampliaciones;
            oContratoSave.Observacion = oContrato.Observacion;
            oContratoSave.DestinoId = oContrato.DestinoId;
            oContratoSave.CantidadCamiones = oContrato.CantidadCamiones;
            oContratoSave.Consignatario = oContrato.Consignatario;
            oContratoSave.PlanCanje = oContrato.PlanCanje;
            oContratoSave.CondicionFijacionId = oContrato.CondicionFijacionId;
            oContratoSave.CD = oContrato.CD;
            oContratoSave.Warrant = oContrato.Warrant;
            oContratoSave.PagoDirectoVendedor = oContrato.PagoDirectoVendedor;
            oContratoSave.StandardDeCalidadId = oContrato.StandardDeCalidadId;
            oContratoSave.CalidadEspecialId = oContrato.CalidadEspecialId;
            oContratoSave.ValorCalidadEspecial = oContrato.ValorCalidadEspecial;
            oContratoSave.EstablecimientoPropio = oContrato.EstablecimientoPropio;
            oContratoSave.ClasificacionId = oContrato.ClasificacionId;
            oContratoSave.CantidadCamiones = oContrato.CantidadCamiones;
            oContratoSave.BoletoId = oContrato.BoletoId;
            oContratoSave.BolsaId = oContrato.BolsaId;
            oContratoSave.DesdeFijacion = oContrato.DesdeFijacion;
            oContratoSave.HastaFijacion = oContrato.HastaFijacion;

            if (oContratoSave.Fecha.Date != oContrato.Fecha.Date)
            {
                oContratoSave.Fecha = oContrato.Fecha;
            }

            if (oContratoSave.ContratoSAP != null)
            {
                oContratoSave.ContratoSAP = oContrato.ContratoSAP;
            }

            mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public KendoGrid<BasicoContrato> TraerTodosContratos(KendoGridMvcRequest request, List<int> listComercialesId)
        {
            var oResult = new StoredPorContratoResult();

            var oContrato = mobjUnitOfWork.Repository<Contrato>().Queryable();

            var oFijacion = mobjUnitOfWork.Repository<FijacionDePrecioContrato>().Queryable();

            var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable();

            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable();

            var oMaterial = mobjUnitOfWork.Repository<Material>()
                            .Queryable();

            var oTipoNegocio = mobjUnitOfWork.Repository<TipoNegocio>()
                            .Queryable();

            var oMoneda = mobjUnitOfWork.Repository<Moneda>()
                            .Queryable();

            var oCampaña = mobjUnitOfWork.Repository<Campaña>()
                            .Queryable();

            var oLocalidad = mobjUnitOfWork.Repository<Localidad>()
                            .Queryable();

            var oProvincia = mobjUnitOfWork.Repository<Provincia>()
                            .Queryable();

            var oEstadoContrato = mobjUnitOfWork.Repository<EstadoContrato>()
                            .Queryable();

            var queryContratos =
                from cont in oContrato
                join prove in oProveedor on cont.ProveedorId equals prove.ProveedorId into proves
                from prove in proves.DefaultIfEmpty()
                join come in oComercial on cont.ComercialId equals come.ComercialId into comes
                from come in comes.DefaultIfEmpty()
                join mat in oMaterial on cont.MaterialId equals mat.MaterialId into mats
                from mat in mats.DefaultIfEmpty()
                join tine in oTipoNegocio on cont.TipoNegocioId equals tine.TipoNegocioId into tines
                from tine in tines.DefaultIfEmpty()
                join mone in oMoneda on cont.MonedaId equals mone.MonedaId into mones
                from mone in mones.DefaultIfEmpty()
                join moneSust in oMoneda on cont.MonedaIdSustentable equals moneSust.MonedaId into monesSust
                from moneSust in monesSust.DefaultIfEmpty()
                join camp in oCampaña on cont.CampanaId equals camp.CampañaId into camps
                from camp in camps.DefaultIfEmpty()
                join loc in oLocalidad on cont.LocalidadId equals loc.LocalidadId into locs
                from loc in locs.DefaultIfEmpty()
                join provi in oProvincia on cont.ProvinciaId equals provi.ProvinciaId into provis
                from provi in provis.DefaultIfEmpty()
                join estado in oEstadoContrato on cont.Estado equals estado.EstadoContratoId into estados
                from estado in estados.DefaultIfEmpty()
                where listComercialesId.Contains(cont.ComercialId != null ? cont.ComercialId.Value : 0)
                select new BasicoContrato()
                {
                    ContratoId = SqlFunctions.StringConvert((double)cont.ContratoId).Trim(),
                    
                    ProveedorId = cont.ProveedorId,
                    ComercialId = cont.ComercialId,
                    MaterialId = cont.MaterialId,
                    TipoNegocioId = cont.TipoNegocioId,
                    Cantidad = cont.Cantidad,
                    Precio = cont.Precio,
                    FechaEntrega = cont.FechaEntrega,
                    CampanaId = cont.CampanaId,
                    FechaDesde = DbFunctions.TruncateTime(cont.FechaDesde),
                    FechaHasta = DbFunctions.TruncateTime(cont.FechaHasta),
                    MonedaId = cont.MonedaId,
                    Moneda = mone == null ? "" : mone.Descripcion,
                    Fecha = DbFunctions.TruncateTime(cont.Fecha),
                    Fecha_Order = cont.Fecha,
                    GrupoCompra = cont.GrupoCompra,
                    ProvinciaId = cont.ProvinciaId,
                    LocalidadId = cont.LocalidadId,
                    Base = cont.Base,
                    Importe_Sustentable = ((decimal)cont.ImporteSustentable),
                    MonedaId_Sustentable = cont.MonedaIdSustentable,
                    Moneda_Sustentable = moneSust == null ? "" : moneSust.Descripcion,
                    Fecha_Dolarizado = DbFunctions.TruncateTime(cont.FechaDolarizado),
                    Dias_Pesificado = cont.DiasPesificado,
                    NoInformaSIO = cont.NoInformaSio,
                    TrigoEspecial = cont.TrigoEspecial,
                    Estado = cont.Estado,
                    Estado_Contrato = estado.Descripcion,
                    Estado_Order = estado.Orden,
                    UsuarioId = cont.UsuarioId,
                    ContratoSAP = cont.ContratoSAP,
                    Ampliaciones = cont.Ampliaciones,
                    Cuit = prove == null ? "" : prove.CUIT,
                    Proveedor = prove == null ? "" : prove.RazonSocial,                    
                    Comercial = come == null ? "" : come.Nombres + " " + come.Apellido,
                    Material = mat == null ? "" : mat.Descripcion,
                    Campania = camp == null ? "" : camp.Descripcion,
                    Provincia = provi == null ? "" : provi.Nombre,
                    TipoNegocio = tine == null ? "" : tine.Descripcion,
                    Localidad = loc == null ? "" : loc.Nombre,
                    Observacion = cont.Observacion != null ? cont.Observacion : "",
                    FijacionDePrecioContratoId = null,
                    Sustentable = ((decimal)cont.ImporteSustentable) != null && ((decimal)cont.ImporteSustentable) > 0,
                    Dolarizado = cont.FechaDolarizado!=null,
                    Pesificado = cont.DiasPesificado!=null,
                    Negocio = (cont.ContratoSAP == 0 || cont.ContratoSAP == null) ? cont.ContratoId : cont.ContratoSAP
                };

            var queryFijacion =
                from fijac in oFijacion
                join prove in oProveedor on fijac.ProveedorId equals prove.ProveedorId into proves
                from prove in proves.DefaultIfEmpty()
                join come in oComercial on fijac.ComercialId equals come.ComercialId into comes
                from come in comes.DefaultIfEmpty()
                join mat in oMaterial on fijac.MaterialId equals mat.MaterialId into mats
                from mat in mats.DefaultIfEmpty()
                join mone in oMoneda on fijac.MonedaId equals mone.MonedaId into mones
                from mone in mones.DefaultIfEmpty()
                join estado in oEstadoContrato on fijac.Estado equals estado.EstadoContratoId into estados
                from estado in estados.DefaultIfEmpty()
                where listComercialesId.Contains(fijac.ComercialId)
                select new BasicoContrato()
                {
                    ContratoId = SqlFunctions.StringConvert((double)fijac.ContratoId).Trim(),
                    ProveedorId = fijac.ProveedorId,
                    ComercialId = fijac.ComercialId,
                    MaterialId = fijac.MaterialId != null ? fijac.MaterialId.Value : 0,
                    TipoNegocioId = 3,
                    Cantidad = fijac.Cantidad,
                    Precio = fijac.Precio,
                    FechaEntrega = null,
                    CampanaId = 0,
                    FechaDesde = null,
                    FechaHasta = null,
                    MonedaId = fijac.MonedaId,
                    Moneda = mone == null ? "" : mone.Descripcion,
                    Fecha = DbFunctions.TruncateTime(fijac.Fecha),
                    Fecha_Order = fijac.Fecha,
                    GrupoCompra = 0,
                    ProvinciaId = null,
                    LocalidadId = null,
                    Base = null,
                    Importe_Sustentable = null,
                    MonedaId_Sustentable = "",
                    Moneda_Sustentable = "",
                    Fecha_Dolarizado = null,
                    Dias_Pesificado = null,
                    NoInformaSIO = null,
                    TrigoEspecial = null,
                    Estado = fijac.Estado,
                    Estado_Contrato = estado.Descripcion,
                    Estado_Order = estado.Orden,
                    UsuarioId = "",
                    ContratoSAP = null,
                    Ampliaciones = fijac.Ampliaciones,
                    Cuit = prove == null ? "" : prove.CUIT,
                    Proveedor = prove == null ? "" : prove.RazonSocial,
                    Comercial = come == null ? "" : come.Nombres + " " + come.Apellido,
                    Material = mat == null ? "" : mat.Descripcion,
                    Campania = "",
                    Provincia = "",
                    TipoNegocio = "FIJACION",
                    Localidad = "",
                    Observacion = fijac.Observacion != null ? fijac.Observacion : "",
                    FijacionDePrecioContratoId = fijac.FijacionDePrecioContratoId,
                    Sustentable = false,
                    Dolarizado = false,
                    Pesificado = false,
                    Negocio = fijac.ContratoId
                };

            queryContratos = queryContratos.Union(queryFijacion);
            
            return new KendoGrid<BasicoContrato>(request, queryContratos);

        }
        

        public async Task<GrabarContratoResult> ConfirmarContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();

            var  oContratoSave = await TraerContratoAsync(oContrato.ContratoId);

            if (oContratoSave != null && (oContratoSave.Estado == (int)EnumEstadoContrato.Pendiente || oContratoSave.Estado == (int)EnumEstadoContrato.Oferta))
            {
                try
                {
                    oContratoSave.Cantidad += oContratoSave.Ampliaciones.Value;
                    oContratoSave.Ampliaciones = 0;
                }
                catch { }

                oContratoSave.Estado = (int)EnumEstadoContrato.Confirmado;

                mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

                await mobjUnitOfWork.SaveChangesAsync();
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("El contrato no se puede confirmar"));
            }

            return oEntityErrors;
        }

        public async Task<GrabarContratoResult> FinalizarContrato(Contrato oContrato, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarContratoResult();
            var  oContratoSave = await TraerContratoAsync(oContrato.ContratoId);

            if (oContratoSave != null && (oContratoSave.Estado == (int)EnumEstadoContrato.Confirmado || oContratoSave.Estado == (int)EnumEstadoContrato.Con_Error))
            {
                var objCampania = await mobjCampaniaManager.TraerCampaniaAsync(oContratoSave.CampanaId);
                var objMaterial = await mobjMaterialManager.TraerMaterialAsync(oContratoSave.MaterialId);
                var objProvincia = await mobjProvinciaManager.TraerProvinciaAsync(oContratoSave.ProvinciaId != null ? oContratoSave.ProvinciaId.Value : 0);
                var objTiponegocio = await mobjTipoNegocioManager.TraerTipoNegociodAsync(oContratoSave.TipoNegocioId);
                var objLocalidad = await mobjLocalidadManager.TraerLocalidadAsync(oContratoSave.LocalidadId != null ? oContratoSave.LocalidadId.Value : 0);
                var objProveedor = await mobjProveedorManager.TraerProveedor(oContratoSave.ProveedorId);
                var objComercial = await mobjComercialManager.TraerComercialAsync(oContratoSave.ComercialId != null ? oContratoSave.ComercialId.Value : 0);

                oContratoSave.Estado = (int)EnumEstadoContrato.Con_Error;
                mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);
                await mobjUnitOfWork.SaveChangesAsync();

                try
                {
                    string nroContratoSAP = SAPFinalizarContrato(oContratoSave, objCampania.Descripcion, objMaterial.Codigo, objProvincia.ProvinciaId.ToString(), objTiponegocio.Descripcion, objLocalidad.CodLocalidad, objProveedor.CUIT, objComercial != null ? objComercial.IdActiveDirectory : "");

                    oContratoSave = await TraerContratoAsync(oContrato.ContratoId);

                    oContratoSave.Estado = (int)EnumEstadoContrato.Finalizado;
                    try
                    {
                        oContratoSave.ContratoSAP = Convert.ToInt32(nroContratoSAP);
                    }
                    catch
                    {
                        oContratoSave.ContratoSAP = 0;
                    }

                    try
                    {
                        //Envio de mail
                        mobjProveedorManager.EnviarEmail(oContratoSave, idActiveDirectory);
                    }
                    catch { }


                    mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

                    await mobjUnitOfWork.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    oEntityErrors.Errores = new List<ErrorMessage>() {
                        new ErrorMessage() { Message = e.Message}
                    };
                }
            }
            else
            {
                if (oContratoSave.Estado == (int)EnumEstadoContrato.Confirmado)
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("El contrato ya se encuentra Finalizado"));
                } else if (oContratoSave.Estado == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("El contrato ya ha sido Rechazado"));
                }
                else if (oContratoSave.Estado == (int)EnumEstadoContrato.Pendiente || oContratoSave.Estado == (int)EnumEstadoContrato.Oferta)
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("El contrato debe ser Confirmado"));
                }
            }

            return oEntityErrors;
        }

        public async Task<GrabarContratoResult> BorrarContrato(Contrato oContrato)
        {    
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = await TraerContratoAsync(oContrato.ContratoId);

            if (oContratoSave != null && (oContratoSave.Estado < (int)EnumEstadoContrato.Finalizado))
            {
                oContratoSave.Estado = (int)EnumEstadoContrato.Rechazado;

                mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

                await mobjUnitOfWork.SaveChangesAsync();
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("El contrato no se puede rechazar"));
            }
            return oEntityErrors;
        }

        public int ObtenerComercialId(string idActiveDirectory)
        {
            return mobjUnitOfWork.Repository<Comercial>().Queryable().Where(x => x.IdActiveDirectory == idActiveDirectory).SingleOrDefault().ComercialId;
        }

        public string SAPFinalizarContrato(Contrato contrato, string campaniaDescripcion, string materialCodigo, string provinciaId, string tiponegocioDescripcion, string localidadCod, string proveedorCUIT, string comercial)
        {
            var SapFinalizarContrato = new FinalizarContratoAgent(logger);
            return SapFinalizarContrato.Finalizar(contrato, campaniaDescripcion, materialCodigo, provinciaId, tiponegocioDescripcion, localidadCod, proveedorCUIT, comercial);
        }

    }

}
