using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class HabilitacionCupoManager : IHabilitacionCupoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IConfiguracionCupoManager configuracionManager;
        private readonly IMaterialManager materialManager;
        public HabilitacionCupoManager(ILogger logger, IRepositorio repositorio, IConfiguracionCupoManager configuracionManager, IMaterialManager materialManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.configuracionManager = configuracionManager;
            this.materialManager = materialManager;
    }

        public Resultado GrabarHabilitacionCupo(HabilitacionCupo cupo)
        {
            var oEntityErrors = Validar(cupo);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                if (cupo.Id == 0)
                {
                    repositorio.Agregar(cupo);
                }
                else
                {
                    var habilitacionCupoSave = repositorio.Obtener<HabilitacionCupo>(cupo.Id);
                    habilitacionCupoSave.FechaDesde = cupo.FechaDesde;
                    habilitacionCupoSave.FechaHasta = cupo.FechaHasta;
                    habilitacionCupoSave.ZonaCupoId = cupo.ZonaCupoId;
                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            return oEntityErrors;
        }
        private Resultado Validar(HabilitacionCupo cupo) {
            var errores = new Resultado();

            //if (cupo.ZonaCupoId == 0)
            //{
            //    errores.Error("habilitacion", "Seleccione una Zona");
            //}
            if (cupo.FechaDesde == new DateTime())
            {
                errores.Error("habilitacion", "La fecha desde no puede estar vacía");
            }
            if (cupo.FechaHasta == new DateTime())
            {
                errores.Error("habilitacion", "La fecha hasta no puede estar vacía");
            }
            if (cupo.FechaDesde > cupo.FechaHasta)
            {
                errores.Error("habilitacion", "La fecha hasta no puede ser menor que la fecha desde");
            }
            if (cupo.Id == 0)
            {
                if (repositorio.Existe<HabilitacionCupo>(x => x.ZonaCupoId == cupo.ZonaCupoId && x.FechaHasta <= cupo.FechaHasta && x.FechaDesde >= cupo.FechaDesde))
                {
                    errores.Error("cupo", "Ya existe una habilitación para esa Zona y rango de fechas");
                }
            }
            return errores;
        }
        public KendoGrid<HabilitacionCupoDto> TraerTodaHabilitacionCupo(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerHabilitacionesCupo(request));
        }

        public HabilitacionCupoDto TraerHabilitacionCupo(int id)
        {
            return repositorio.Obtener<HabilitacionCupo, HabilitacionCupoDto>(x => x.Id == id, x => new HabilitacionCupoDto
            {
                Id = x.Id,
                FechaHasta = x.FechaHasta,
                FechaDesde = x.FechaDesde,
                ZonaCupoId = x.ZonaCupoId,
                MaterialId = x.MaterialId
            });
        }

        public List<HabilitacionCupoDto> TraerTodasHabilitacionesActivas(int zona)
        {
          
            var hoy = DateTime.Now;
            return repositorio.Listar<HabilitacionCupo, HabilitacionCupoDto>(x => new HabilitacionCupoDto
            {
                Id = x.Id,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                ZonaCupoId = x.ZonaCupoId,
                MaterialId = x.MaterialId
            }, x => x.ZonaCupoId == zona || x.ZonaCupoId == null && x.FechaDesde <= hoy && x.FechaHasta >= hoy);
        }
        public List<MaterialHabilitadoDto> TraerTodoMaterialRetirado(int zona)
        {
            var materiales = repositorio.Listar<Material, MaterialDto>(x => new MaterialDto { MaterialId = x.MaterialId, Descripcion = x.Descripcion }, x => x.MaterialId != 5);
            var materialHabilitado = TraerTodoMaterialHabilitado(zona);
            var habilitacion = new List<MaterialHabilitadoDto>();
            var materialDisponible = configuracionManager.TraerTodaConfiguracionCupoPorDia(zona);
            foreach (var item in materiales)
            {
                if (materialHabilitado.Any(x=>x == item.MaterialId) && materialDisponible.Any(x => x.MaterialId == item.MaterialId))
                {
                    var habilitado = new MaterialHabilitadoDto { Id = item.MaterialId, DescripcionMaterial = item.Descripcion, Descripcion = "DISPONIBLE" };
                    habilitacion.Add(habilitado);
                }
                else if (!materialHabilitado.Any(x => x == item.MaterialId) && materialDisponible.Any(x => x.MaterialId == item.MaterialId))
                {
                    var habilitado = new MaterialHabilitadoDto { Id = item.MaterialId, DescripcionMaterial = item.Descripcion, Descripcion = "NO HABILITADO" };
                    habilitacion.Add(habilitado);
                }
                else if (!materialDisponible.Any(x => x.MaterialId == item.MaterialId))
                {
                    var habilitado = new MaterialHabilitadoDto { Id = item.MaterialId, DescripcionMaterial = item.Descripcion, Descripcion = "RETIRADO" };
                    habilitacion.Add(habilitado);
                }   
            }
            return habilitacion;
        }
        public bool HayMaterialDisponibleExterno(int zona)
        {
            var material = materialManager.TraerTodoMaterial();
            var materialExterno = TraerTodoMaterialRetirado(zona);
            material.Material = material.Material.Where(x => materialExterno.Where(y => y.Descripcion == "Disponible".ToUpper()).Any(y => y.Id == x.MaterialId)).ToList();
            return  material.Material.Count >= 1 ? true : false;
           
        }
        public List<int> TraerTodoMaterialHabilitado(int zona)
        {
           return TraerTodasHabilitacionesActivas(zona).Select(x => x.MaterialId).ToList();
        }
    }
}
