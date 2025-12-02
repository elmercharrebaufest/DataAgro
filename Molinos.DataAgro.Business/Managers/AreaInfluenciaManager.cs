using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business
{
    public class AreaInfluenciaManager : IAreaInfluenciaManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public AreaInfluenciaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public ResultIniAreaInfluencia TraerTodoAreaInfluencia()
        {
            return new ResultIniAreaInfluencia
            {
                AreaInfluencia = repositorio.Listar<AreaInfluencia, AreaInfluenciaIni>(x => new AreaInfluenciaIni()
                {
                    AreaInfluenciaId = x.AreaInfluenciaId,
                    Descripcion = x.Descripcion
                }, null, 15, "Descripcion")
            };
        }

        public AreaInfluenciaDto TraerAreaInfluencia(int intAreaInfluenciaId)
        {
            return repositorio.Obtener<AreaInfluencia, AreaInfluenciaDto>(x => x.AreaInfluenciaId == intAreaInfluenciaId, x => new AreaInfluenciaDto { AreaInfluenciaId = x.AreaInfluenciaId, Descripcion = x.Descripcion }) ?? new AreaInfluenciaDto();
        }

        public Resultado GrabarAreaInfluencia(AreaInfluencia oAreaInfluencia)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oAreaInfluencia, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oAreaInfluencia.AreaInfluenciaId != 0)
            {
                var oAreaInfluenciaSave = repositorio.Obtener<AreaInfluencia>(oAreaInfluencia.AreaInfluenciaId);
                oAreaInfluenciaSave.Descripcion = oAreaInfluencia.Descripcion;
            }
            else
            {
                repositorio.Agregar(oAreaInfluencia);
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


        public Resultado EliminarAreaInfluencia(int intAreaInfluenciaId)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<AreaInfluencia>(intAreaInfluenciaId);
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




