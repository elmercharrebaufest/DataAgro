using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class MonedaManager : IMonedaManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public MonedaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------


        public ResultIniMoneda TraerTodo()
        {
            return new ResultIniMoneda
            {
                Moneda = repositorio.Listar<Moneda, MonedaIni>(x => new MonedaIni()
                {
                    MonedaId = x.MonedaId,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion")
            };
        }

        public Moneda TraerMonedad(string monedaId)
        {
            return repositorio.Obtener<Moneda>(monedaId) ?? new Moneda();
        }

        public Resultado GrabarMoneda(Moneda oMoneda)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oMoneda, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            var oMonedaSave = repositorio.Obtener<Moneda>(oMoneda.MonedaId);

            if (oMonedaSave == null)
            {
                repositorio.Agregar(oMoneda);
            }
            else
            {
                oMonedaSave.Descripcion = oMoneda.Descripcion;
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

        public Resultado EliminarMoneda(string monedaId)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Moneda>(monedaId);
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




