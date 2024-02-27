using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class LocalidadManager : ILocalidadManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public LocalidadManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public DatosIniAbmLocalidad TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);

            var oDatosIniciales = new DatosIniAbmLocalidad()
            {
                Provincia = qry.GetProvinciaCombo()
            };

            return oDatosIniciales;
        }


        public ResultIniLocalidad TraerFiltroLocalidad(ParamAbmLocalidad oParam)
        {
            var oResult = new ResultIniLocalidad();

            oResult.Localidad = repositorio.Listar<Localidad, LocalidadIni>(x => new LocalidadIni()
            {
                LocalidadId = x.LocalidadId,
                CodLocalidad = x.CodLocalidad,
                Nombre = x.Nombre,
                ProNombre = x.Provincia.Nombre,
                PartidoNombre = x.Partido.Descripcion
            },
                x => (oParam.Nombre.Trim() == "" || x.Nombre.Contains(oParam.Nombre.Trim())) &&
                (oParam.ProvinciaId == null || x.ProvinciaId == oParam.ProvinciaId) &&
                (oParam.PartidoId == null || x.PartidoId == oParam.PartidoId)
            , 500).OrderBy(x => x.ProNombre).ThenBy(x => x.PartidoNombre).ThenBy(x => x.Nombre).ToList();

            return oResult;
        }

        public ResultIniLocalidad TraerLocalidadPorProvincia(int provinciaId)
        {
            var oResult = new ResultIniLocalidad();

            oResult.Localidad = repositorio.Listar<Localidad, LocalidadIni>(x => new LocalidadIni()
            {
                LocalidadId = x.LocalidadId,
                CodLocalidad = x.CodLocalidad,
                Nombre = x.Nombre,
                ProNombre = x.Provincia.Nombre,
            }, x => x.ProvinciaId == provinciaId);

            return oResult;
        }

        public LocalidadDto TraerLocalidad(int intLocalidadId)
        {
            var localidad = repositorio.Obtener<Localidad, LocalidadDto>(x => x.LocalidadId == intLocalidadId, x => new LocalidadDto
            {
                CodLocalidad = x.CodLocalidad,
                LocalidadId = x.LocalidadId,
                ProvinciaId = x.ProvinciaId,
                Provincia_Nombre = x.Provincia.Nombre,
                Nombre = x.Nombre,
                PartidoId = x.PartidoId,
                Partido_Nombre = x.Partido.Descripcion
            });
            return localidad;
        }
        public Resultado GrabarLocalidad(Localidad oLocalidad)
        {
            var oEntityErrors = new Resultado();
            List<Localidad> listLocalidades;

            EntityValid.ValidateAll(oLocalidad, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oLocalidad.LocalidadId != 0)
                listLocalidades = repositorio.Listar<Localidad>(x => x.LocalidadId != oLocalidad.LocalidadId && x.CodLocalidad == oLocalidad.CodLocalidad).ToList();
            else
                listLocalidades = repositorio.Listar<Localidad>(x => x.CodLocalidad == oLocalidad.CodLocalidad).ToList();

            if (listLocalidades != null && listLocalidades.Count > 0)
            {
                oEntityErrors.Errores.Add(new ErrorMessage() { Message = "Ya existe el código de localidad" });
                return oEntityErrors;
            }

            if (oLocalidad.LocalidadId != 0)
            {
                var oLocalidadSave = repositorio.Obtener<Localidad>(oLocalidad.LocalidadId);
                oLocalidadSave.CodLocalidad = oLocalidad.CodLocalidad;
                oLocalidadSave.Nombre = oLocalidad.Nombre;
                oLocalidadSave.ProvinciaId = oLocalidad.ProvinciaId;
                oLocalidadSave.PartidoId = oLocalidad.PartidoId;
            }
            else
            {
                repositorio.Agregar(oLocalidad);
            }

            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                oEntityErrors.Error("", ex.Message);
                return oEntityErrors;
            }
            return oEntityErrors;
        }


        public Resultado EliminarLocalidad(int intLocalidadId)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Localidad>(intLocalidadId);
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

        public List<LocalidadDto> ListarLocalidad(string localidad)
        {
            return repositorio.Listar<Localidad, LocalidadDto>(x => new LocalidadDto { LocalidadId = x.LocalidadId, Nombre = x.Nombre }, x => localidad == "" || x.Nombre.Contains(localidad), 15);
        }

        public List<LocalidadDto> ListarLocalidadTodas()
        {
            return repositorio.Listar<Localidad, LocalidadDto>(x => new LocalidadDto { LocalidadId = x.LocalidadId, Nombre = x.Nombre, Provincia_Nombre = x.Provincia.Nombre, ProvinciaId = x.ProvinciaId, Partido_Nombre = x.Partido.Descripcion, PartidoId = x.PartidoId }, x => true);
        }

        public List<BusquedaLocalidad> DevolverLocalidades(string filtro)
        {
            //var resultado = repositorio.SelStore<BusquedaLocalidad>("DataAgro_BusquedaLocalidades", 20, filtro);
            var resultado = repositorio.ListarConsulta(new BusquedaLocalidades(filtro));
            foreach (var r in resultado)
            {
                r.Filtro = filtro + "|" + r.Localidad + " (" + r.Provincia + ")";
            }
            return resultado;
        }
        public LocalidadQry TraerLocalidadProvincia(string localidad, string provincia)
        {
            var localidadDto = repositorio.Obtener<Localidad, LocalidadQry>(x => x.Nombre == localidad && x.Provincia.Nombre == provincia, x => new LocalidadQry() { LocalidadId = x.LocalidadId, Nombre = x.Nombre });
            localidadDto.ProvinciaId = repositorio.Obtener<Provincia, int>(x => x.Nombre == provincia, x => x.ProvinciaId);
            return localidadDto;
        }
        public ResultIniPartido TraerPartidosPorProvincia(int provinciaId)
        {
            var qry = new CombosQueries(logger, repositorio);

            var oPartidosPorProvincia = new ResultIniPartido()
            {
                Partidos = qry.GetPartidoCombo(provinciaId)
            };

            return oPartidosPorProvincia;
        }

        public List<PartidoDto> ListarPartidos()
        {
            return repositorio.Listar<Partido, PartidoDto>(x => new PartidoDto { Id = x.Id, Descripcion = x.Descripcion, Provincia = x.Provincia.Nombre, ProvinciaId = x.ProvinciaId}, x => true);
        }
    }
}