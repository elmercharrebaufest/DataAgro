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

    public class OperadorManager : IOperadorManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public OperadorManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public DatosIniAbmOperador TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);
            return new DatosIniAbmOperador()
            {
                Operador = qry.GetAbmOperadorCombo()
            };
        }

        public ResultIniOperador TraerTodoOperador()
        {
            return new ResultIniOperador
            {
                Operador = repositorio.Listar<Operador, OperadorIni>(x => new OperadorIni()
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion")
            };
        }

        public OperadorDto TraerOperador(int id)
        {
            return repositorio.Obtener<Operador, OperadorDto>(x => x.Id == id, x => new OperadorDto { Id = x.Id, Descripcion = x.Descripcion }) ?? new OperadorDto();
        }

        public Resultado GrabarOperador(Operador oOperador)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oOperador, oEntityErrors);
            if (oOperador.Descripcion == "" || oOperador.Descripcion == null)
            {
                oEntityErrors.Error("", "El nombre del operador no debe estar vacío");
            }
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oOperador.Id != 0)
            {
                var oOperadorSave = repositorio.Obtener<Operador>(oOperador.Id);
                oOperadorSave.Descripcion = oOperador.Descripcion;
            }
            else
            {
                repositorio.Agregar(oOperador);
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

            logger.Debug("Guardando el operador:" + oOperador.Descripcion);

            return oEntityErrors;
        }

        public Resultado EliminarOperador(int id)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Operador>(id);

            logger.Debug("Eliminando el operador:" + id);
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

        public List<OperadorIni> ListarOperador(string text)
        {
            return repositorio.Listar<Operador, OperadorIni>(x => new OperadorIni { Id = x.Id, Descripcion = x.Descripcion }, x => text == "" || x.Descripcion.Contains(text), 15);
        }
    }
}

