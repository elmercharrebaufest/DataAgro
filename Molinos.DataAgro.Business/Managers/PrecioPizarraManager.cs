using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class PrecioPizarraManager : IPrecioPizarraManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        public PrecioPizarraManager(IRepositorio repositorio, ILogger logger)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public Resultado GrabarPrecioPizarra(PrecioPizarra precioPizarra)
        {
            var oEntityErrors = ValidarPrecioPizarra(precioPizarra);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                repositorio.Agregar(precioPizarra);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }
        
        private Resultado ValidarPrecioPizarra(PrecioPizarra precioPizarra)
        {
            var precioMayorHasta = repositorio.ObtenerMayor<PrecioPizarra, DateTime>(x => x.MaterialId == precioPizarra.MaterialId, x => x.FechaHasta);
            var error = new Resultado();
            if (precioPizarra.MaterialId == 0) error.Errores.Add(new ErrorMessage(400, "El campo Cultivo no puede estar vacío"));
            if (string.IsNullOrEmpty(precioPizarra.MonedaId)) error.Errores.Add(new ErrorMessage(400, "El campo Moneda no puede estar vacío"));
            if (precioPizarra.FechaHasta.CompareTo(precioPizarra.FechaDesde) == -1) error.Errores.Add(new ErrorMessage(400, "El campo Fecha Hasta no puede ser menor que el campo Fecha Desde"));
            if (precioMayorHasta != null && precioMayorHasta.FechaHasta >= precioPizarra.FechaDesde) error.Errores.Add(new ErrorMessage(400, "Ya existe un rango de fechas asignado para este cultivo"));
            if (precioPizarra.Precio == 0) error.Errores.Add(new ErrorMessage(400, "El campo Precio no puede estar vacío"));
            return error;
        }

       
        public List<PrecioPizarraDto> TraerTodoPrecioPizarra()
        {
            return repositorio.Listar<PrecioPizarra, PrecioPizarraDto>(x => new PrecioPizarraDto
            {
                Id = x.Id,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion + "",
                PizarraId = x.PizarraId,
                Pizarra = x.Pizarra.Descripcion + "",
                FechaDesde = x.FechaDesde.ToString(),
                FechaHasta = x.FechaHasta.ToString(),
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion + "",
                UnidadMedida = x.UnidadMedida

            }) ;
        }

        public List<PrecioPizarraDto> TraerTodoPrecioPizarraPorMaterialYPizarra(int materialId, int pizarraId)
        {
            return repositorio.Listar<PrecioPizarra, PrecioPizarraDto>(x => new PrecioPizarraDto
            {
                Id = x.Id,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion + "",
                PizarraId = x.PizarraId,
                Pizarra = x.Pizarra.Descripcion + "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "/" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "/" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "/" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "/" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion + "",
                UnidadMedida = x.UnidadMedida + "TON"

            }, x => x.MaterialId == materialId && x.PizarraId == pizarraId);
        }

        public List<MonedaDto> TraerTodoMoneda()
        {
            return repositorio.Listar<Moneda, MonedaDto>(x => new MonedaDto
            {
                MonedaId = x.MonedaId,
                Descripcion = x.Descripcion

            });
        }
    }
}
