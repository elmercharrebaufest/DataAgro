using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business
{
    public class ProvinciaManager : IProvinciaManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public ProvinciaManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public ResultIniProvincia TraerTodoProvincia()
        {
            var oResult = new ResultIniProvincia();

            oResult.Provincia = repositorio.Listar<Provincia, ProvinciaIni>(x => new ProvinciaIni()
            {
                ProvinciaId = x.ProvinciaId,
                Nombre = x.Nombre
            }, null, 0, "Nombre");

            return oResult;
        }


        public ProvinciaDto TraerProvincia(int intProvinciaId)
        {
            return repositorio.Obtener<Provincia, ProvinciaDto>(x=>x.ProvinciaId == intProvinciaId, x=> new ProvinciaDto {ProvinciaId=x.ProvinciaId, Nombre=x.Nombre }) ?? new ProvinciaDto();
        }


        public Resultado GrabarProvincia(Provincia oProvincia)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oProvincia, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oProvincia.ProvinciaId != 0)
            {
                var oProvinciaSave = repositorio.Obtener<Provincia>(oProvincia.ProvinciaId);
                oProvinciaSave.Nombre = oProvincia.Nombre;
            }
            else
            {
                repositorio.Agregar(oProvincia);
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


        public Resultado EliminarProvincia(int intProvinciaId)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Provincia>(intProvinciaId);
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

        public List<ProvinciaDto> ListarProvincia(string provincia)
        {
            return repositorio.Listar<Provincia, ProvinciaDto>(x => new ProvinciaDto { Nombre = x.Nombre, ProvinciaId = x.ProvinciaId },x => provincia == "" || x.Nombre.Contains(provincia));
        }
    }
}




