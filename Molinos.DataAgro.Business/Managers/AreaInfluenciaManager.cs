using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class AreaInfluenciaManager : IAreaInfluenciaManager
    {
        private ILogger logger;
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
        
        public AreaInfluencia TraerAreaInfluencia(int intAreaInfluenciaId)
        {
            return repositorio.Obtener<AreaInfluencia>(intAreaInfluenciaId) ?? new AreaInfluencia();
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
                var oAreaInfluenciaSave = TraerAreaInfluencia(oAreaInfluencia.AreaInfluenciaId);
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




