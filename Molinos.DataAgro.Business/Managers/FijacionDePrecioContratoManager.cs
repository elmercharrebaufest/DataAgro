using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Diagnostics;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Common.Enums;

using Molinos.DataAgro.Mapping.Context;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Managers {
    public class FijacionDePrecioContratoManager : IFijacionDePrecioContratoManager
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Inicializacion
        //--------------------------------------------------

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

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<DatosIniAbmFijacionDePrecioContrato> TraerDatosInicialesAsync()
        {
            var DatosCombo = new DatosIniAbmFijacionDePrecioContrato();

            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable().AsNoTracking();

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
                             .Select(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }).ToListAsync();

            DatosCombo.proveedor = await mobjUnitOfWork.Repository<Proveedor>()
                               .Queryable()
                               .AsNoTracking()
                               .Select(x => new ProveedorQry() { ProveedorId = x.ProveedorId, Descripcion = x.RazonSocial }).ToListAsync();

            return DatosCombo;
        }


        public async Task<ResultIniFijacionDePrecioContrato> TraerTodoFijacionDePrecioAsync()
        {
            var result = new ResultIniFijacionDePrecioContrato();
            result.FijacionDePrecioContrato = mobjUnitOfWork.SelStore<FijacionDePrecioContratoIni>("DataAgro_BasicoFijacionPrecioContratoTraerPorFiltro", 0).ToList();
            return  result;


        }
        public async Task<ResultIniFijacionDePrecioContrato> TraerFijacionDePrecioContratoAsync(int ContratoId)
        {
            var result = new ResultIniFijacionDePrecioContrato
            {
                FijacionDePrecioContrato = mobjUnitOfWork.SelStore<FijacionDePrecioContratoIni>("DataAgro_BasicoFijacionPrecioContratoTraerPorFiltro", ContratoId).ToList()
            };

            return result;
        }

        public async Task<FijacionDePrecioContrato> TraerFijacionDePrecioAsync(int intFijacionId)
        {
            var oFijacionDePrecio = new FijacionDePrecioContrato();

            oFijacionDePrecio = await mobjUnitOfWork.Repository<FijacionDePrecioContrato>()
                                 .Queryable()
                                 .Where(x => x.FijacionDePrecioContratoId == intFijacionId)
                                 .SingleOrDefaultAsync();

            if (oFijacionDePrecio == null)
            {
                oFijacionDePrecio = new FijacionDePrecioContrato()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oFijacionDePrecio.ObjectState = Constants.Object_Modified;
            }

            return oFijacionDePrecio;
        }

        public async Task<GrabarContratoResult> GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion) {

            FijacionDePrecioContrato oFijacionDePrecioContratoSave;

            var oEntityErrors = new GrabarContratoResult();
            oEntityErrors.errores = new List<ErrorMessage>();

            oFijacionDePrecioContratoSave = await TraerFijacionDePrecioAsync(oFijacion.FijacionDePrecioContratoId);

            oFijacionDePrecioContratoSave.Ampliaciones = oFijacion.Ampliaciones.Value;
            oFijacionDePrecioContratoSave.Estado = (int)EnumEstadoContrato.Pendiente;

            mobjUnitOfWork.Repository<FijacionDePrecioContrato>().SaveEntity(oFijacionDePrecioContratoSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;

        }

        private List<ErrorMessage> Validar(FijacionDePrecioContrato oParam, List<ErrorMessage> oErrorMessages) {

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
            if (oParam.Precio == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Precio' no debe estar vacio", "Precio"));
            }
            if (string.IsNullOrEmpty(oParam.MonedaId))
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Moneda' no debe estar vacio", "MonedaId"));
            }
            if (oParam.ComercialId == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Comercial' no debe estar vacio", "ComercialId"));
            }
          

            return oErrorMessages;
        }

        public async Task<GrabarFijacionResult> GrabarFijacionDePrecioAsync(FijacionDePrecioContrato oFijacionDePrecio)
        {
            //var oEntityErrors = new EntityErrors();

            //EntityValid.ValidateAll(oFijacionDePrecio, oEntityErrors.ListaErrores);
            var oEntityErrors = new GrabarFijacionResult();
            oEntityErrors.errores = new List<ErrorMessage>();

            oEntityErrors.errores = this.Validar(oFijacionDePrecio, oEntityErrors.errores);


            if (oEntityErrors.errores.Count > 0)
            {
                return oEntityErrors;
            }

            FijacionDePrecioContrato oFijacionDePrecioSave;

            if (oFijacionDePrecio.FijacionDePrecioContratoId == 0)
            {
                oFijacionDePrecioSave = new FijacionDePrecioContrato()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oFijacionDePrecioSave = await TraerFijacionDePrecioAsync(oFijacionDePrecio.FijacionDePrecioContratoId);
            }

            oFijacionDePrecioSave.MaterialId = oFijacionDePrecio.MaterialId;
            oFijacionDePrecioSave.Precio = oFijacionDePrecio.Precio;
            oFijacionDePrecioSave.Fecha = oFijacionDePrecio.Fecha;
            oFijacionDePrecioSave.ProveedorId = oFijacionDePrecio.ProveedorId;
            oFijacionDePrecioSave.ComercialId = oFijacionDePrecio.ComercialId;
            oFijacionDePrecioSave.Cantidad = oFijacionDePrecio.Cantidad;
            oFijacionDePrecioSave.ContratoId = oFijacionDePrecio.ContratoId;
            oFijacionDePrecioSave.MonedaId = oFijacionDePrecio.MonedaId;
            oFijacionDePrecioSave.Ampliaciones = oFijacionDePrecio.Ampliaciones;
            oFijacionDePrecioSave.Estado = oFijacionDePrecio.Estado;
            oFijacionDePrecioSave.Observacion = oFijacionDePrecio.Observacion;


            if (oFijacionDePrecioSave.FijacionDePrecioContratoId == Constants.Object_Added)
            {
                oFijacionDePrecioSave.FijacionDePrecioContratoId = ((mobjUnitOfWork.Repository<FijacionDePrecioContrato>().Queryable().Max(x => (int?)x.FijacionDePrecioContratoId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<FijacionDePrecioContrato>().SaveEntity(oFijacionDePrecioSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<GrabarFijacionResult> ConfirmarFijacion(FijacionDePrecioContrato oFijacionDePrecio) {
            var oEntityErrors = new GrabarFijacionResult();

            FijacionDePrecioContrato oFijacionDePrecioSave = new FijacionDePrecioContrato();

            if (oFijacionDePrecio.Estado != 0) {
                oFijacionDePrecioSave = await TraerFijacionDePrecioAsync(oFijacionDePrecio.FijacionDePrecioContratoId);
            }

            try {
                oFijacionDePrecioSave.Cantidad += oFijacionDePrecioSave.Ampliaciones.Value;
                oFijacionDePrecioSave.Ampliaciones = 0;
            }
            catch { }

            oFijacionDePrecioSave.Estado = (int)EnumEstadoContrato.Confirmado;

            mobjUnitOfWork.Repository<FijacionDePrecioContrato>().SaveEntity(oFijacionDePrecioSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<GrabarFijacionResult> FinalizarFijacion(FijacionDePrecioContrato oFijacionDePrecio, string idActiveDirectory) {
            var oEntityErrors = new GrabarFijacionResult();

            ProveedorManager mobjProveedorManager = new ProveedorManager();
            mobjProveedorManager.Inicializar(mobjContexto);

            FijacionDePrecioContrato oFijacionDePrecioSave = new FijacionDePrecioContrato();

            if (oFijacionDePrecio.Estado != 0) {
                oFijacionDePrecioSave = await TraerFijacionDePrecioAsync(oFijacionDePrecio.FijacionDePrecioContratoId);
            }
            
            oFijacionDePrecioSave.Estado = (int)EnumEstadoContrato.Finalizado;

            //Envio de mail
            mobjProveedorManager.EnviarEmailFijacion(oFijacionDePrecioSave, idActiveDirectory);

            mobjUnitOfWork.Repository<FijacionDePrecioContrato>().SaveEntity(oFijacionDePrecioSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<EntityErrors> EliminarFijacionDePrecioAsync(int intFijacionId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<FijacionDePrecioContrato>();

            var oFijacionDePrecio = await oRepository
                                 .Queryable()
                                 .Where(x => x.FijacionDePrecioContratoId == intFijacionId)
                                 .SingleOrDefaultAsync();

            if (oFijacionDePrecio != null)
            {
                oRepository.Delete(oFijacionDePrecio);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }
    }
}




