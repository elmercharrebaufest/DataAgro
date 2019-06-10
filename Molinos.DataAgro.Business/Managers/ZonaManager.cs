using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class ZonaManager : IZonaManager
    { 
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public ZonaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public DatosIniAbmZona TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);
            return new DatosIniAbmZona()
            {
                Zona = qry.GetAbmZonaCombo()
            };
        }

        public ResultIniZona TraerTodoZona()
        {
            return new ResultIniZona
            {
                Zona = repositorio.Listar<Zona, ZonaIni>(x => new ZonaIni()
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    CodigoSap = x.CodigoSap
                }, null, 0, "Descripcion")
            };
        }

        public ZonaDto TraerZona(int id)
        {
            return repositorio.Obtener<Zona, ZonaDto>(x => x.Id == id, x => new ZonaDto { Id = x.Id, CodigoSap = x.CodigoSap, Descripcion = x.Descripcion}) ?? new ZonaDto();
        }

        public Resultado GrabarZona(Zona oZona)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oZona, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oZona.Id != 0)
            {
                var oZonaSave = repositorio.Obtener<Zona>(oZona.Id);
                oZonaSave.Descripcion = oZona.Descripcion;
                oZonaSave.CodigoSap = oZona.CodigoSap;
            }
            else
            {
                repositorio.Agregar(oZona);
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

            logger.Debug("Guardando el centro:" + oZona.Descripcion);

            return oEntityErrors;
        }

        public Resultado EliminarZona(int id)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Zona>(id);

            logger.Debug("Eliminando el centro:" + id);
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

