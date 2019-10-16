using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class ObjetivoManager : IObjetivoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public ObjetivoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public ObjetivoHome TraerObjetivoHome(int idComercial, List<int> equipo)
        {
            var list = new ObjetivoHome();

            var lista = repositorio.Listar<ObjetivoComercial, MaterialObjetivo>(x=> new MaterialObjetivo
            {
               Material = x.Material.Descripcion,
               MaterialId = x.MaterialId,
               Campana = x.Campana.Descripcion,
               Toneladas = x.ToneladasObjetivos,
               Comercial = x.Comercial.Nombres + " "+x.Comercial.Apellido,
               ComercialId = x.ComercialId
            }, x=> equipo.Contains(x.ComercialId) && x.Material.CampañaId <= x.CampanaId,0,"ComercialId");
            var listaPorMaterial = lista.GroupBy(x => x.Material);
            foreach (var obj in listaPorMaterial)
            {
                var agregarObjetivo = obj.OrderByDescending(x => x.Campana).Where(x => x.ComercialId == idComercial).FirstOrDefault();
                if(agregarObjetivo != null)
                list.Objetivos.Add(agregarObjetivo);
            }
            var listaPorComercial = lista.GroupBy(x => new { x.ComercialId, x.Comercial});
            foreach(var comercial in listaPorComercial)
            {
                var listaMaterial = comercial.OrderBy(x=>x.MaterialId).ThenByDescending(x => x.Campana);
                list.Comerciales.Add(new DetalleObjetivo
                {
                    Comercial = comercial.Key.Comercial,
                    ComercialId = comercial.Key.ComercialId,
                    Objetivos = listaMaterial.ToList()                    
                });

            }
            return list;
        }
        public Resultado GuardarObjetivo(ObjetivoComercial objetivo)
        {
            var resultado = new Resultado();
            if (objetivo.MaterialId == 0)
                resultado.Error("Material", "Debe elegir Material");
            if (objetivo.CampanaId == 0)
                resultado.Error("Campaña", "Debe elegir Campaña");
            if (objetivo.ToneladasObjetivos == 0)
                resultado.Error("Toneladas", "Las tonealadas tienen que ser mayor a 0");
            if (objetivo.ComercialId == 0)
                resultado.Error("Comercial", "Comercial Inválido");
            if (repositorio.Existe<ObjetivoComercial>(x => x.CampanaId == objetivo.CampanaId && x.MaterialId == objetivo.MaterialId && x.ComercialId == objetivo.ComercialId))
                resultado.Error("Existe", "Ya existe Objetivo para el comercial con esos datos");
            if (resultado.HayError)
                return resultado;

            repositorio.Agregar(objetivo);
            repositorio.GuardarCambios();

            return resultado;
        }

    }
}
