using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
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
            var listaZonas = repositorio.Listar<ZonaCupo, ZonaCupoIni>(x => new ZonaCupoIni()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                CodigoSap = x.CodigoSap
            }, null, 0, "Descripcion");
            if (!PermisosHelper.Is(PermisosDataAgro.AltaOrigenCentro))
            {
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "OIC"));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.AltaOrigenNorte))
            {
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "OIN"));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.AltaOrigenSur))
            {
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "OIS"));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.AltaCorredoresBsAs))
            {
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "CBA"));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.AltaCorredoresRosario))
            {
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "CRO"));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.AltaOtrasZonas))
            {
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "FAS"));
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "MAT"));
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "PPR"));
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "RED"));
                listaZonas.RemoveAt(listaZonas.FindIndex(x => x.CodigoSap == "SOL"));
            }
            return new ResultIniZonaCupo
            {
                ZonaCupo = listaZonas
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
