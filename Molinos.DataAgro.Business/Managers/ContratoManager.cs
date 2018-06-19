using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Agent.Compras;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
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
using static Mastersoft.Framework.Standard.Constantes;
using System.Web.UI.WebControls;
using Molinos.DataAgro.Agent.DatosDelComercial;
using KendoGridBinder.ModelBinder.Mvc;
using KendoGridBinder;
using System.Data.Entity.SqlServer;

namespace Molinos.DataAgro.Business.Managers
{


    public class ContratoManager : IContratoManager
    {
        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;
        
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

        public async Task<List<ComercialQry>> TraerComerciales() {

            List<ComercialQry> listComerciales = new List<ComercialQry>();

            listComerciales = await mobjUnitOfWork.Repository<Comercial>()
                             .Queryable()
                             .AsNoTracking()
                             .OrderBy(x => x.Nombres)
                             .ThenBy(x => x.Apellido)
                             .Select(x => new ComercialQry() { ComercialId = x.ComercialId, EmpleadorACargo = x.EmpleadorACargo }).ToListAsync();

            return listComerciales;
        }


        public async Task<DatosIniContrato> TraerDatosCombo()
        {
            var DatosCombo = new DatosIniContrato();
            
            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable().AsNoTracking();

            DatosCombo.prov = await mobjUnitOfWork.Repository<Provincia>()
                                .Queryable()
                                .AsNoTracking()
                                .Join(oLocalidad, a => a.ProvinciaId, b => b.ProvinciaId, (a, b) => new { P = a, L = b })
                                .GroupBy(x => new { x.P.ProvinciaId, x.P.Nombre })
                                .Select(x => new ProvinciaQry() { Provinciaid = x.Key.ProvinciaId, Nombre = x.Key.Nombre }).ToListAsync();

            DatosCombo.loc = new List<LocalidadQry>();

            DatosCombo.campaña = await mobjUnitOfWork.Repository<Campaña>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion }).ToListAsync();

            DatosCombo.material = await mobjUnitOfWork.Repository<Material>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion }).ToListAsync();

            DatosCombo.moneda = await mobjUnitOfWork.Repository<Moneda>()
                                    .Queryable()
                                    .AsNoTracking()
                                    .Select(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion }).ToListAsync();

            DatosCombo.comercial = await mobjUnitOfWork.Repository<Comercial>()
                             .Queryable()
                             .AsNoTracking()
                             .OrderBy(x => x.Nombres)
                             .ThenBy(x => x.Apellido)
                             .Where(x =>  x.PerfilId  == (int)EnumPerfil.Comercial || x.PerfilId == (int)EnumPerfil.Jefe || x.PerfilId == (int)EnumPerfil.Mesa )
                             .Select(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }).ToListAsync();

            DatosCombo.monedaSustentable = await mobjUnitOfWork.Repository<Moneda>()
                                 .Queryable()
                                 .AsNoTracking()
                                 .Select(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion }).ToListAsync();

            DatosCombo.proveedor = await mobjUnitOfWork.Repository<Proveedor>()
                               .Queryable()
                               .AsNoTracking()
                               .OrderBy(x => x.RazonSocial)
                               .Select(x => new ProveedorQry() { ProveedorId = x.ProveedorId, Descripcion = x.RazonSocial }).ToListAsync();
            
            DatosCombo.tiponegocio = await mobjUnitOfWork.Repository<TipoNegocio>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => new TipoNegocioQry() { TipoNegocioId = x.TipoNegocioId, Descripcion = x.Descripcion }).ToListAsync();

            Array estadosValues = System.Enum.GetValues(typeof(EnumEstadoContrato));

            foreach (int estadoValue in estadosValues) {
                string estadoName = Enum.GetName(typeof(EnumEstadoContrato), estadoValue);

                EstadosContratos item = new EstadosContratos(estadoValue, estadoName);

                DatosCombo.estadoContrato.Add(item);
            }

            return DatosCombo;
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

        public StoredPorContratoResult TraerTodosContratos()
        {
            var oResult = new StoredPorContratoResult();

            var oContrato = mobjUnitOfWork.Repository<Contrato>().Queryable();

            var query = oContrato
                        .Select(x => new BasicoContrato()
                        {
                            ContratoId = x.ContratoId.ToString(),
                            ProveedorId = x.ProveedorId,
                            ComercialId = x.ComercialId,
                            MaterialId = x.MaterialId,
                            TipoNegocioId = x.TipoNegocioId,
                            Cantidad = x.Cantidad,
                            Precio = x.Precio,
                            FechaEntrega = SqlFunctions.DateName("day", x.FechaEntrega).Trim() + "/" +
                                           SqlFunctions.StringConvert((double)x.FechaEntrega.Month).TrimStart() + "/" +
                                           SqlFunctions.DateName("year", x.FechaEntrega),
                            CampanaId = x.CampanaId,
                            FechaDesde = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "/" +
                                         SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "/" +
                                         SqlFunctions.DateName("year", x.FechaDesde),
                            FechaHasta = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "/" +
                                         SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "/" +
                                         SqlFunctions.DateName("year", x.FechaHasta),
                            MonedaId = x.MonedaId,
                            Fecha = Convert.ToString(x.Fecha),
                            GrupoCompra = x.GrupoCompra,
                            ProvinciaId = x.ProvinciaId,
                            LocalidadId = x.LocalidadId,
                            Base = x.Base,
                            Importe_Sustentable = ((decimal)x.ImporteSustentable),
                            MonedaId_Sustentable = x.MonedaIdSustentable,
                            Fecha_Dolarizado = SqlFunctions.DateName("day", x.FechaDolarizado).Trim() + "/" +
                                               SqlFunctions.StringConvert((double)x.FechaDolarizado.Value.Month).TrimStart() + "/" +
                                               SqlFunctions.DateName("year", x.FechaDolarizado),
                            Dias_Pesificado = x.DiasPesificado,
                            NoInformaSIO = x.NoInformaSio,
                            TrigoEspecial = x.TrigoEspecial,
                            Estado = x.Estado,
                            UsuarioId = x.UsuarioId,
                            ContratoSAP = x.ContratoSAP,
                            Ampliaciones = x.Ampliaciones,
                            Observacion = x.Observacion

                        });

            oResult.BasicoContratoTraerPorFltro = query.ToList();

            //oResult.BasicoContratoTraerPorFltro.FindAll

            return oResult;
        }

        public async Task<GrabarContratoResult> GrabarAmpliacionContrato(Contrato oContrato) {

            Contrato oContratoSave;

            var oEntityErrors = new GrabarContratoResult();
            oEntityErrors.errores = new List<ErrorMessage>();

            oContratoSave = await TraerContratoAsync(oContrato.ContratoId);

            oContratoSave.Ampliaciones = oContrato.Ampliaciones.Value;
            oContratoSave.Estado = oContratoSave.Base == true ? (int)EnumEstadoContrato.Oferta : (int)EnumEstadoContrato.Pendiente;

            mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;

        }

        public async Task<GrabarContratoResult> GrabarContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            oEntityErrors.errores = new List<ErrorMessage>();

            oEntityErrors.errores = this.Validar(oContrato, oEntityErrors.errores);

            if (oEntityErrors.errores.Count > 0)
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
            oContratoSave.Observacion = oContrato.Observacion;
            oContratoSave.Ampliaciones = oContrato.Ampliaciones;

            if (oContratoSave.Fecha != null)
            {
                oContratoSave.Fecha = oContrato.Fecha;
            }

            if (oContratoSave.ContratoSAP != null)
            {
                oContratoSave.ContratoSAP = oContrato.ContratoSAP;
            }

            if (oContratoSave.ContratoId == Constants.Object_Added)
            {
                oContratoSave.ContratoId = ((mobjUnitOfWork.Repository<Contrato>().Queryable().Max(x => (int?)x.ContratoId)) ?? 0) + 1;
            }


            mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }



        public KendoGrid<BasicoContrato> TraerTodosContratos(KendoGridMvcRequest request, List<ComercialQry> listComercial)
        {

            List<int> listComercialesId = listComercial.Select(x => x.ComercialId).ToList();

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
                    FechaEntrega = SqlFunctions.DateName("day", cont.FechaEntrega).Trim() + "/" +
                                   SqlFunctions.StringConvert((double)cont.FechaEntrega.Month).TrimStart() + "/" +
                                   SqlFunctions.DateName("year", cont.FechaEntrega),
                    CampanaId = cont.CampanaId,
                    FechaDesde = SqlFunctions.DateName("day", cont.FechaDesde).Trim() + "/" +
                                 SqlFunctions.StringConvert((double)cont.FechaDesde.Month).TrimStart() + "/" +
                                 SqlFunctions.DateName("year", cont.FechaDesde),
                    FechaHasta = SqlFunctions.DateName("day", cont.FechaHasta).Trim() + "/" +
                                 SqlFunctions.StringConvert((double)cont.FechaHasta.Month).TrimStart() + "/" +
                                 SqlFunctions.DateName("year", cont.FechaHasta),
                    MonedaId = cont.MonedaId,
                    Moneda = mone == null ? "" : mone.Descripcion,
                    Fecha = SqlFunctions.DateName("day", cont.Fecha).Trim() + "/" +
                            SqlFunctions.StringConvert((double)cont.Fecha.Month).TrimStart() + "/" +
                            SqlFunctions.DateName("year", cont.Fecha),
                    Fecha_Order = cont.Fecha,
                    GrupoCompra = cont.GrupoCompra,
                    ProvinciaId = cont.ProvinciaId,
                    LocalidadId = cont.LocalidadId,
                    Base = cont.Base,
                    Importe_Sustentable = ((decimal)cont.ImporteSustentable),
                    MonedaId_Sustentable = cont.MonedaIdSustentable,
                    Moneda_Sustentable = moneSust == null ? "" : moneSust.Descripcion,
                    Fecha_Dolarizado = cont.FechaDolarizado != null ? SqlFunctions.DateName("day", cont.FechaDolarizado).Trim() + "/" +
                                                                      SqlFunctions.StringConvert((double)cont.FechaDolarizado.Value.Month).TrimStart() + "/" +
                                                                      SqlFunctions.DateName("year", cont.FechaDolarizado) :"",
                    Dias_Pesificado = cont.DiasPesificado,
                    NoInformaSIO = cont.NoInformaSio,
                    TrigoEspecial = cont.TrigoEspecial,
                    Estado = cont.Estado,
                    Estado_Contrato = estado.Descripcion,
                    Estado_Order = estado.Orden,
                    UsuarioId = cont.UsuarioId,
                    ContratoSAP = cont.ContratoSAP,
                    Ampliaciones = cont.Ampliaciones,
                    Proveedor = prove == null ? "" : prove.RazonSocial,
                    Comercial = come == null ? "" : come.Nombres,
                    Material = mat == null ? "" : mat.Descripcion,
                    Campania = camp == null ? "" : camp.Descripcion,
                    Provincia = provi == null ? "" : provi.Nombre,
                    TipoNegocio = tine == null ? "" : tine.Descripcion,
                    Localidad = loc == null ? "" : loc.Nombre,
                    Observacion = cont.Observacion != null ? cont.Observacion : "",
                    FijacionDePrecioContratoId = ""
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
                    ContratoId = fijac.ContratoId,
                    ProveedorId = fijac.ProveedorId,
                    ComercialId = fijac.ComercialId,
                    MaterialId = fijac.MaterialId != null ? fijac.MaterialId.Value : 0,
                    TipoNegocioId = 3,
                    Cantidad = fijac.Cantidad,
                    Precio = fijac.Precio,
                    FechaEntrega = "",
                    CampanaId = 0,
                    FechaDesde = "",
                    FechaHasta = "",
                    MonedaId = fijac.MonedaId,
                    Moneda = mone == null ? "" : mone.Descripcion,
                    Fecha = SqlFunctions.DateName("day", fijac.Fecha).Trim() + "/" +
                    SqlFunctions.StringConvert((double)fijac.Fecha.Month).TrimStart() + "/" +
                    SqlFunctions.DateName("year", fijac.Fecha),
                    Fecha_Order = fijac.Fecha,
                    GrupoCompra = 0,
                    ProvinciaId = null,
                    LocalidadId = null,
                    Base = null,
                    Importe_Sustentable = null,
                    MonedaId_Sustentable = "",
                    Moneda_Sustentable = "",
                    Fecha_Dolarizado = "",
                    Dias_Pesificado = null,
                    NoInformaSIO = null,
                    TrigoEspecial = null,
                    Estado = fijac.Estado,
                    Estado_Contrato = estado.Descripcion,
                    Estado_Order = estado.Orden,
                    UsuarioId = "",
                    ContratoSAP = null,
                    Ampliaciones = fijac.Ampliaciones,
                    Proveedor = prove == null ? "" : prove.RazonSocial,
                    Comercial = come == null ? "" : come.Nombres,
                    Material = mat == null ? "" : mat.Descripcion,
                    Campania = "",
                    Provincia = "",
                    TipoNegocio = "FIJACION",
                    Localidad = "",
                    Observacion = fijac.Observacion != null ? fijac.Observacion : "",
                    FijacionDePrecioContratoId = SqlFunctions.StringConvert((double)fijac.FijacionDePrecioContratoId).Trim(),
                };

            queryContratos = queryContratos.Union(queryFijacion);
            
            return new KendoGrid<BasicoContrato>(request, queryContratos);

        }

        public async Task<GrabarContratoResult> ConfirmarContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();

            Contrato oContratoSave = new Contrato();

            if (oContrato.Estado != 0)
            {
                oContratoSave = await TraerContratoAsync(oContrato.ContratoId);
            }

            try {
                oContratoSave.Cantidad += oContratoSave.Ampliaciones.Value;
                oContratoSave.Ampliaciones = 0;
            } catch { }

            oContratoSave.Estado = (int)EnumEstadoContrato.Confirmado;

            mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<GrabarContratoResult> FinalizarContrato(Contrato oContrato, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarContratoResult();
            MaterialManager mobjMaterialManager = new MaterialManager();
            mobjMaterialManager.Inicializar(mobjContexto);
            TipoNegocioManager mobjTipoNegocioManager = new TipoNegocioManager();
            mobjTipoNegocioManager.Inicializar(mobjContexto);
            CampañaManager mobjCampaniaManager = new CampañaManager();
            mobjCampaniaManager.Inicializar(mobjContexto);
            ProvinciaManager mobjProvinciaManager = new ProvinciaManager();
            mobjProvinciaManager.Inicializar(mobjContexto);
            LocalidadManager mobjLocalidadManager = new LocalidadManager();
            mobjLocalidadManager.Inicializar(mobjContexto);
            ProveedorManager mobjProveedorManager = new ProveedorManager();
            mobjProveedorManager.Inicializar(mobjContexto);
            ComercialManager mobjComercialManager = new ComercialManager();
            mobjComercialManager.Inicializar(mobjContexto);

            Contrato oContratoSave = new Contrato();

            if (oContrato.Estado != 0)
            {
                oContratoSave = await TraerContratoAsync(oContrato.ContratoId);
            }


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
            catch (Exception e) {
                oEntityErrors.errores = new List<ErrorMessage>() {
                    new ErrorMessage() { Message = e.Message}
                };
            }

            return oEntityErrors;

        }

        //public async Task<GrabarContratoResult> BorrarContrato(Contrato oContrato, string idActiveDirectory)
        //{
        //    var oEntityErrors = new GrabarContratoResult();
        //    MaterialManager mobjMaterialManager = new MaterialManager();
        //    mobjMaterialManager.Inicializar(mobjContexto);
        //    TipoNegocioManager mobjTipoNegocioManager = new TipoNegocioManager();
        //    mobjTipoNegocioManager.Inicializar(mobjContexto);
        //    CampañaManager mobjCampaniaManager = new CampañaManager();
        //    mobjCampaniaManager.Inicializar(mobjContexto);
        //    ProvinciaManager mobjProvinciaManager = new ProvinciaManager();
        //    mobjProvinciaManager.Inicializar(mobjContexto);
        //    LocalidadManager mobjLocalidadManager = new LocalidadManager();
        //    mobjLocalidadManager.Inicializar(mobjContexto);
        //    ProveedorManager mobjProveedorManager = new ProveedorManager();
        //    mobjProveedorManager.Inicializar(mobjContexto);
        //    ComercialManager mobjComercialManager = new ComercialManager();
        //    mobjComercialManager.Inicializar(mobjContexto);

        //    Contrato oContratoSave = new Contrato();

        //    if (oContrato.Estado != 0)
        //    {
        //        oContratoSave = await TraerContratoAsync(oContrato.ContratoId);
        //    }


        //    var objCampania = await mobjCampaniaManager.TraerCampaniaAsync(oContratoSave.CampanaId);
        //    var objMaterial = await mobjMaterialManager.TraerMaterialAsync(oContratoSave.MaterialId);
        //    var objProvincia = await mobjProvinciaManager.TraerProvinciaAsync(oContratoSave.ProvinciaId != null ? oContratoSave.ProvinciaId.Value : 0);
        //    var objTiponegocio = await mobjTipoNegocioManager.TraerTipoNegociodAsync(oContratoSave.TipoNegocioId);
        //    var objLocalidad = await mobjLocalidadManager.TraerLocalidadAsync(oContratoSave.LocalidadId != null ? oContratoSave.LocalidadId.Value : 0);
        //    var objProveedor = await mobjProveedorManager.TraerProveedor(oContratoSave.ProveedorId);
        //    var objComercial = await mobjComercialManager.TraerComercialAsync(oContratoSave.ComercialId != null ? oContratoSave.ComercialId.Value : 0);

        //    oContratoSave.Estado = (int)EnumEstadoContrato.Con_Error;
        //    mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);
        //    await mobjUnitOfWork.SaveChangesAsync();

        //    try
        //    {
        //        string nroContratoSAP = SAPFinalizarContrato(oContratoSave, objCampania.Descripcion, objMaterial.Codigo, objProvincia.ProvinciaId.ToString(), objTiponegocio.Descripcion, objLocalidad.CodLocalidad, objProveedor.CUIT, objComercial != null ? objComercial.IdActiveDirectory : "");

        //        oContratoSave = await TraerContratoAsync(oContrato.ContratoId);

        //        oContratoSave.Estado = (int)EnumEstadoContrato.Finalizado;
        //        try
        //        {
        //            oContratoSave.ContratoSAP = Convert.ToInt32(nroContratoSAP);
        //        }
        //        catch
        //        {
        //            oContratoSave.ContratoSAP = 0;
        //        }

        //        try
        //        {
        //            //Envio de mail
        //            mobjProveedorManager.EnviarEmail(oContratoSave, idActiveDirectory);
        //        }
        //        catch { }


        //        mobjUnitOfWork.Repository<Contrato>().SaveEntity(oContratoSave);

        //        await mobjUnitOfWork.SaveChangesAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        oEntityErrors.errores = new List<ErrorMessage>() {
        //            new ErrorMessage() { Message = e.Message}
        //        };
        //    }

        //    return oEntityErrors;

        //}

        public async Task<int> ObtenerComercialId(string idActiveDirectory)
        {

            return mobjUnitOfWork.Repository<Comercial>().Queryable().Where(x => x.IdActiveDirectory == idActiveDirectory).SingleOrDefault().ComercialId;

        }

        public string SAPFinalizarContrato(Contrato contrato, string campaniaDescripcion, string materialCodigo, string provinciaId, string tiponegocioDescripcion, string localidadCod, string proveedorCUIT, string comercial)
        {
            var SapFinalizarContrato = new FinalizarContratoAgent();
            //var aux = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
            return SapFinalizarContrato.Finalizar(contrato, campaniaDescripcion, materialCodigo, provinciaId, tiponegocioDescripcion, localidadCod, proveedorCUIT, comercial);
        }

    }

}
