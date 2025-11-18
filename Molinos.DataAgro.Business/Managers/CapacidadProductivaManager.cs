using System.Collections.Generic;
using System.Linq;
using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Managers
{
    public class CapacidadProductivaManager : ICapacidadProductivaManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IVisualizarCapacidadProductivaAgent capProdAgent;

        public CapacidadProductivaManager(ILogger logger, IRepositorio repositorio, IVisualizarCapacidadProductivaAgent capProdAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.capProdAgent = capProdAgent;
        }

        public List<CapacidadProductivaDto> ObtenerCapacidadProductiva(int proveedorId)
        {
            return capProdAgent.VisualizarCapacidadProductiva(proveedorId);
        }

        public void ActualizarCapacidadProductiva()
        {
            List<CapacidadProductivaDto> listaCP = new List<CapacidadProductivaDto>();
            List<ProveedorDto> proveedoresTodos = repositorio.Listar<Proveedor, ProveedorDto>(x => new ProveedorDto { CUIT = x.CUIT, ProveedorId = x.ProveedorId, RazonSocial = x.RazonSocial, SegmentacionId = x.SegmentacionId });
            List<Material> materiales = repositorio.Listar<Material>();
            List<Campaña> cosechas = repositorio.Listar<Campaña>();

            foreach (ProveedorDto p in proveedoresTodos)
            {
                listaCP.AddRange(capProdAgent.VisualizarCapacidadProductiva(p.ProveedorId, p, materiales, cosechas));
            }

            repositorio.RemoverTodosConReseedCero<CapacidadProductiva>(a => true);
            List<CapacidadProductiva> listaFinal = listaCP.Select(cp => new CapacidadProductiva
            {
                ProveedorId = cp.ProveedorId,
                MaterialId = cp.MaterialId,
                CampaniaId = cp.CampaniaId,
                Cantidad = cp.Cantidad,
                UnidadMedida = cp.UnidadMedida,
                Porcentaje = cp.Porcentaje,
                FechaActualizacion = cp.FechaActualizacion
            }).ToList();

            repositorio.AgregarTodos(listaFinal);
            repositorio.GuardarCambios();
        }
    }
}
