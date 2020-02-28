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
            },
                x => (oParam.Nombre.Trim() == "" || x.Nombre.Contains(oParam.Nombre.Trim())) &&
                (oParam.ProvinciaId == null || x.ProvinciaId == oParam.ProvinciaId)
            , 500).OrderBy(x => x.ProNombre).ThenBy(x => x.Nombre).ToList();

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
            return repositorio.Obtener<Localidad, LocalidadDto>(x => x.LocalidadId == intLocalidadId, x => new LocalidadDto { CodLocalidad = x.CodLocalidad, LocalidadId = x.LocalidadId, Nombre = x.Nombre});
        }
        public Resultado GrabarLocalidad(Localidad oLocalidad)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oLocalidad, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            
            if (oLocalidad.LocalidadId != 0)
            {
                var oLocalidadSave = repositorio.Obtener<Localidad>(oLocalidad.LocalidadId);
                oLocalidadSave.CodLocalidad = oLocalidad.CodLocalidad;
                oLocalidadSave.Nombre = oLocalidad.Nombre;
                oLocalidadSave.ProvinciaId = oLocalidad.ProvinciaId;
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
                logger.Error(ex);
                throw;
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
            return repositorio.Listar<Localidad,LocalidadDto>(x=>new LocalidadDto { LocalidadId = x.LocalidadId, Nombre= x.Nombre} ,x => localidad == "" || x.Nombre.Contains(localidad), 15);
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
        public  LocalidadQry TraerLocalidadProvincia(string localidad, string provincia)
        {
            var localidadDto = repositorio.Obtener<Localidad, LocalidadQry>(x => x.Nombre == localidad && x.Provincia.Nombre == provincia, x => new LocalidadQry() { LocalidadId = x.LocalidadId, Nombre = x.Nombre, ProvinciaId= x.ProvinciaId});
            return localidadDto;
        }
    }
}




