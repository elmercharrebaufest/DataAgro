using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business
{
    public class TipoNegocioManager : ITipoNegocioManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public TipoNegocioManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------


        public ResultIniTipoNegocio TraerTodo()
        {
            var oResult = new ResultIniTipoNegocio();

            oResult.TipoNegocio = repositorio.Listar<TipoNegocio, TipoNegocioIni>(x => new TipoNegocioIni()
            {
                TipoNegocioId = x.TipoNegocioId,
                Descripcion = x.Descripcion
            }, null, 0, "Descripcion");

            return oResult;
        }

        public TipoNegocioDto TraerTipoNegociod(int tipoNegocioId)
        {
            return repositorio.Obtener<TipoNegocio, TipoNegocioDto>(x => x.TipoNegocioId == tipoNegocioId, x=> new TipoNegocioDto(){TipoNegocioId = x.TipoNegocioId,Descripcion =x.Descripcion}) ?? new TipoNegocioDto();
        }

        public Resultado GrabarTipoNegocio(TipoNegocio oTipoNegocio)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oTipoNegocio, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oTipoNegocio.TipoNegocioId != 0)
            {
                var oTipoNegocioSave = repositorio.Obtener<TipoNegocio>(oTipoNegocio.TipoNegocioId);
                oTipoNegocioSave.Descripcion = oTipoNegocio.Descripcion;
            }
            else
            {
                repositorio.Agregar(oTipoNegocio);
            }

            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return oEntityErrors;
        }

        public Resultado EliminarTipoNegocio(int id)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<TipoNegocio>(id);
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return oEntityErrors;
        }
    }
}




