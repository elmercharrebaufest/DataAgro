using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class FijacionDePrecioContratoManager : IFijacionDePrecioContratoManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private IProveedorManager mobjProveedorManager;
        private ILogger logger;

        public FijacionDePrecioContratoManager(ILogger logger, IMSContextProvider oMSContextProvider, IProveedorManager oMSProveedorManager)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
            mobjProveedorManager = oMSProveedorManager;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<DatosIniAbmFijacionDePrecioContrato> TraerDatosInicialesAsync()
        {
            var datosCombo = new DatosIniAbmFijacionDePrecioContrato();

            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable().AsNoTracking();

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
                             .Select(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }).ToListAsync();

            datosCombo.proveedor = await mobjUnitOfWork.Repository<Proveedor>()
                               .Queryable()
                               .AsNoTracking()
                               .Select(x => new ProveedorQry() { ProveedorId = x.ProveedorId, Descripcion = x.RazonSocial }).ToListAsync();

            return datosCombo;
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

        public async Task<GrabarContratoResult> GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion)
        {
            var oFijacionDePrecioContratoSave = await TraerFijacionDePrecioAsync(oFijacion.FijacionDePrecioContratoId);
            var oEntityErrors = new GrabarContratoResult();
            
            if (oFijacionDePrecioContratoSave != null && oFijacionDePrecioContratoSave.Estado <= (int)EnumEstadoContrato.Con_Error)
            {
                oFijacionDePrecioContratoSave.Ampliaciones = oFijacion.Ampliaciones.Value;
                oFijacionDePrecioContratoSave.Estado = (int)EnumEstadoContrato.Pendiente;

                mobjUnitOfWork.Repository<FijacionDePrecioContrato>().SaveEntity(oFijacionDePrecioContratoSave);

                await mobjUnitOfWork.SaveChangesAsync();
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("La Fijación no se puede modificar"));
            }
            

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
            oEntityErrors.Errores = new List<ErrorMessage>();

            oEntityErrors.Errores = this.Validar(oFijacionDePrecio, oEntityErrors.Errores);


            if (oEntityErrors.Errores.Count > 0)
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
                if(oFijacionDePrecioSave.Estado > (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("La Fijación no se puede modificar"));
                    return oEntityErrors;
                }
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
            var oFijacionDePrecioSave = await TraerFijacionDePrecioAsync(oFijacionDePrecio.FijacionDePrecioContratoId);

            if ( oFijacionDePrecioSave != null && ( oFijacionDePrecioSave.Estado == (int)EnumEstadoContrato.Pendiente || oFijacionDePrecioSave.Estado == (int)EnumEstadoContrato.Oferta))
            {
                try
                {
                    oFijacionDePrecioSave.Cantidad += oFijacionDePrecioSave.Ampliaciones.Value;
                    oFijacionDePrecioSave.Ampliaciones = 0;
                }
                catch { }

                oFijacionDePrecioSave.Estado = (int)EnumEstadoContrato.Confirmado;

                mobjUnitOfWork.Repository<FijacionDePrecioContrato>().SaveEntity(oFijacionDePrecioSave);

                await mobjUnitOfWork.SaveChangesAsync();
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("La Fijación no se puede confirmar"));
            }

            return oEntityErrors;
        }

        public async Task<GrabarFijacionResult> FinalizarFijacion(FijacionDePrecioContrato oFijacionDePrecio, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarFijacionResult();
            var oFijacionDePrecioSave = await TraerFijacionDePrecioAsync(oFijacionDePrecio.FijacionDePrecioContratoId);

            if(oFijacionDePrecioSave != null && (oFijacionDePrecioSave.Estado == (int)EnumEstadoContrato.Confirmado || oFijacionDePrecioSave.Estado == (int)EnumEstadoContrato.Con_Error))
            {
                oFijacionDePrecioSave.Estado = (int)EnumEstadoContrato.Finalizado;

                //Envio de mail
                mobjProveedorManager.EnviarEmailFijacion(oFijacionDePrecioSave, idActiveDirectory);

                mobjUnitOfWork.Repository<FijacionDePrecioContrato>().SaveEntity(oFijacionDePrecioSave);

                await mobjUnitOfWork.SaveChangesAsync();
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage ("La Fijación ya se encuentra Finalizada"));
            }
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

        public async Task<GrabarContratoResult> BorrarFijacion(FijacionDePrecioContrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = await TraerFijacionDePrecioAsync(oContrato.FijacionDePrecioContratoId);

            if (oContratoSave != null && (oContratoSave.Estado < (int)EnumEstadoContrato.Finalizado))
            {
                oContratoSave.Estado = (int)EnumEstadoContrato.Rechazado;

                mobjUnitOfWork.Repository<FijacionDePrecioContrato>().SaveEntity(oContratoSave);

                await mobjUnitOfWork.SaveChangesAsync();
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("La Fijación no se puede rechazar"));
            }
            return oEntityErrors;
        }
    }
}




