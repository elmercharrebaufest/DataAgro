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
    public class DestinatarioManager : IDestinatarioManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public DestinatarioManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public ResultIniDestinatario TraerTodoDestinatario()
        {
            return new ResultIniDestinatario
            {
                Destinatario = repositorio.Listar<Destinatario, DestinatarioIni>(x => new DestinatarioIni()
                {
                    DestinatarioId = x.DestinatarioId,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion")
            };
        }


        public DestinatarioDto TraerDestinatario(int intDestinatarioId)
        {
            return repositorio.Obtener<Destinatario, DestinatarioDto>(x => x.DestinatarioId == intDestinatarioId, 
                x => new DestinatarioDto
                {
                    DestinatarioId = x.DestinatarioId,
                    Descripcion = x.Descripcion,
                    Inhabilitado = x.Inhabilitado
                }) ?? new DestinatarioDto();
        }


        public Resultado GrabarDestinatario(Destinatario oDestinatario)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oDestinatario, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            oEntityErrors = ValidarDescripcion(oDestinatario);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oDestinatario.DestinatarioId != 0)
            {
                var oDestinatarioSave = repositorio.Obtener<Destinatario>(oDestinatario.DestinatarioId);
                oDestinatarioSave.Descripcion = oDestinatario.Descripcion;
                oDestinatarioSave.Inhabilitado = oDestinatario.Inhabilitado;
            }
            else
            {
                repositorio.Agregar(oDestinatario);
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


        public Resultado EliminarDestinatario(int intDestinatarioId)
        {
            var oEntityErrors = new Resultado();
            repositorio.Remover<Destinatario>(intDestinatarioId);
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
        public Resultado ValidarDescripcion(Destinatario oDestinatarios)
        {
            var resultado = new Resultado();

            if (repositorio.Existe<Destinatario>(x => x.Descripcion == oDestinatarios.Descripcion && x.DestinatarioId != oDestinatarios.DestinatarioId))
            {
                resultado.Error("Descripcion", "Existe un registro de iguales carecteristicas.");
            }
            return resultado;
        } 
        #endregion

    }
}




