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
    public class FechaFeriadoManager : IFechaFeriadoManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public FechaFeriadoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------
               

        public FechaFeriadoDto Traer(int id)
        {
            return repositorio.Obtener<FechaFeriado, FechaFeriadoDto>(x => x.Id == id, x => new FechaFeriadoDto { Id = x.Id,Feriado = x.Feriado }) ?? new FechaFeriadoDto();
        }

        public Resultado Grabar(FechaFeriado feriado)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(feriado, oEntityErrors);
            if (repositorio.Existe<FechaFeriado>(x=>x.Feriado == feriado.Feriado && x.Id != feriado.Id))
            {
                oEntityErrors.Errores.Add(new ErrorMessage { Source = "Feriado", Message = "Ya esta cargado ese feriado." });
            }
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (feriado.Id != 0)
            {
                var oMaterialSave = repositorio.Obtener<FechaFeriado>(feriado.Id);
                oMaterialSave.Feriado = feriado.Feriado;
            }
            else
            {
                repositorio.Agregar(feriado);
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

        public Resultado Eliminar(int Id)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<FechaFeriado>(Id);
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
       
        public List<FechaFeriadoDto> TraerTodo()
        {
            var result = repositorio.Listar<FechaFeriado, FechaFeriadoDto>(x => new FechaFeriadoDto()
            {
                Id = x.Id,
                Feriado = x.Feriado
            }, null, 0, "Feriado")
            .OrderByDescending(x => x.Feriado).ToList();

            return result;
        }
    }
}




