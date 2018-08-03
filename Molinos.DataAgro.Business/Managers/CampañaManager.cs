using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class CampañaManager : ICampañaManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public CampañaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public Campaña TraerCampania(int campaniaId)
        {
            return repositorio.Obtener<Campaña>(campaniaId);
        }

        public List<Campaña> TraerCampañasActivas()
        {
            return repositorio.Listar<Material, Campaña>(x => x.Campaña);
        }
        
        public List<Material> TraerMaterialPorCampaña(int campanaId)
        {
            return repositorio.Listar<Material>(x => x.CampañaId == campanaId);
        }

        public CampañaHome TraerCampañaHome(int idComercial, List<int> equipo)
        {
            var list = new CampañaHome();

            var lista = repositorio.ListarConsulta(new TraerComprasHome(idComercial, equipo));

            if (lista.Count() > 5)
            {
                list.Materiales = lista.Take(4).ToList();
                list.Materiales.Add(new MaterialCampaña()
                {
                    Nombre = "Otros",
                    Toneladas = lista.Where(x => !list.Materiales.Any(y => y.Nombre == x.Nombre)).Sum(x => x.Toneladas),
                    Campaña = string.Empty
                });
            }
            else
            {
                list.Materiales = lista.ToList();
            }
            
            return list;
        }

        public List<Campaña> TraerCampañasPorGrano(int materialId)
        {
            return repositorio.Listar<CampañaMaterialHistorico, Campaña>(x => x.Campaña, x => x.MaterialId == materialId, 0, "CampañaId", DirOrden.Desc);
        }

        public List<Campaña> TraerCampañaPorMaterial(int materialId)
        {
            return repositorio.Listar<Material, Campaña>(x => x.Campaña, x => x.MaterialId == materialId, 0, "CampañaId", DirOrden.Desc);
        }
        public List<CalidadEspecial> TraerCalidadPorMaterial(int materialId)
        {
            return repositorio.Listar<CalidadEspecial>(x => x.MaterialId == materialId);
        }

        public CampañaMaterial TraerCampañaMaterial(int campañaId, int proveedorId, int materialId)
        {
            return repositorio.Obtener<CampañaMaterial>(x => x.CampañaId == campañaId && x.ProveedorId == proveedorId && x.MaterialId == materialId) ?? new CampañaMaterial();
        }
    }
    
}
