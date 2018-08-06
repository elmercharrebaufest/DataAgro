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
    public class CondicionManager : ICondicionManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public CondicionManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public ResultIniCondicion TraerTodoCondicion()
        {
            return new ResultIniCondicion
            {
                Condicion = repositorio.Listar<Condicion, CondicionIni>(x => new CondicionIni()
                {
                    CondicionId = x.CondicionId,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion")
            };
        }


        public CondicionDto TraerCondicion(int intCondicionId)
        {
            return repositorio.Obtener<Condicion, CondicionDto>(x => x.CondicionId == intCondicionId, 
                x => new CondicionDto { CondicionId = x.CondicionId, Descripcion = x.Descripcion, Inhabilitado = x.Inhabilitado}) ?? new CondicionDto();
        }

        public Resultado GrabarCondicion(Condicion oCondicion)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oCondicion, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            oEntityErrors = ValidarCondicion(oCondicion);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oCondicion.CondicionId != 0)
            {
                var oCondicionSave = repositorio.Obtener<Condicion>(oCondicion.CondicionId);

                oCondicionSave.Descripcion = oCondicion.Descripcion;
                oCondicionSave.Inhabilitado = oCondicion.Inhabilitado;
            }
            else
            {
                repositorio.Agregar(oCondicion);
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


        public Resultado EliminarCondicion(int intCondicionId)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Condicion>(intCondicionId);
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


        #region Validar
        public Resultado ValidarCondicion(Condicion oCondiciones)
        {
            var resultado = new Resultado();
            if (repositorio.Existe<Condicion>(x => x.Descripcion == oCondiciones.Descripcion && x.CondicionId != oCondiciones.CondicionId))
            {
                resultado.Error("Descripcion", "Existe un registro de iguales carecteristicas.");
            }
            return resultado;
        } 
        #endregion

    }
}




