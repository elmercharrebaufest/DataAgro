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
    public class CanalOperacionManager : ICanalOperacionManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public CanalOperacionManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public ResultIniCanalOperacion TraerTodoCanalOperacion()
        {
            return new ResultIniCanalOperacion
            {
                CanalOperacion = repositorio.Listar<CanalOperacion, CanalOperacionIni>(x => new CanalOperacionIni()
                {
                    CanalOperacionId = x.CanalOperacionId,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion")
            };
        }


        public CanalOperacion TraerCanalOperacion(int intCanalOperacionId)
        {
            return repositorio.Obtener<CanalOperacion>(intCanalOperacionId) ?? new CanalOperacion();
        }


        public Resultado GrabarCanalOperacion(CanalOperacion oCanalOperacion)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oCanalOperacion, oEntityErrors);
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            var validacion = ValidarCanalOperacion(oCanalOperacion);
            if (validacion.HayErrores)
            {
                return validacion;
            }
            
            if (oCanalOperacion.CanalOperacionId != 0)
            {
                var oCanalOperacionSave = TraerCanalOperacion(oCanalOperacion.CanalOperacionId);
                oCanalOperacionSave.Descripcion = oCanalOperacion.Descripcion;
                oCanalOperacionSave.Inhabilitado = oCanalOperacion.Inhabilitado;
            }
            else
            {
                repositorio.Agregar(oCanalOperacion);
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


        public Resultado EliminarCanalOperacion(int intCanalOperacionId)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<CanalOperacion>(intCanalOperacionId);
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
        public Resultado ValidarCanalOperacion(CanalOperacion oCanalOperaciones)
        {
            var resultado = new Resultado();
            if (repositorio.Existe<CanalOperacion>(x => x.Descripcion == oCanalOperaciones.Descripcion && (oCanalOperaciones.CanalOperacionId != x.CanalOperacionId)))
            {
                resultado.Error("Descripcion", "Existe un registro de iguales carecteristicas.");
            }
            return resultado;
        } 
        #endregion

    }
}




