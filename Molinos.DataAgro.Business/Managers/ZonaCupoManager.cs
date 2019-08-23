using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class ZonaCupoManager: IZonaCupoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public ZonaCupoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public DatosIniAbmZonaCupo TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);
            return new DatosIniAbmZonaCupo()
            {
                ZonaCupo = qry.GetAbmZonaCupo()
            };
        }

        public ResultIniZonaCupo TraerTodoZonaCupo()
        {
            return new ResultIniZonaCupo
            {
                ZonaCupo = repositorio.Listar<ZonaCupo, ZonaCupoIni>(x => new ZonaCupoIni()
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    CodigoSap = x.CodigoSap
                }, null, 0, "Descripcion")
            };
        }

        public ZonaCupoDto TraerZonaCupo(int id)
        {
            return repositorio.Obtener<ZonaCupo, ZonaCupoDto>(x => x.Id == id, x => new ZonaCupoDto { Id = x.Id, CodigoSap = x.CodigoSap, Descripcion = x.Descripcion }) ?? new ZonaCupoDto();
        }

        public Resultado GrabarZonaCupo(ZonaCupo oZonaCupo)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oZonaCupo, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oZonaCupo.Id != 0)
            {
                var oZonaCupoSave = repositorio.Obtener<ZonaCupo>(oZonaCupo.Id);
                oZonaCupoSave.Descripcion = oZonaCupo.Descripcion;
                oZonaCupoSave.CodigoSap = oZonaCupo.CodigoSap;
            }
            else
            {
                repositorio.Agregar(oZonaCupo);
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

            logger.Debug("Guardando:" + oZonaCupo.Descripcion);

            return oEntityErrors;
        }

        public Resultado EliminarZonaCupo(int id)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<ZonaCupo>(id);

            logger.Debug("Eliminando:" + id);
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

        public static implicit operator ZonaCupoManager(ZonaManager v)
        {
            throw new NotImplementedException();
        }
    }
}
