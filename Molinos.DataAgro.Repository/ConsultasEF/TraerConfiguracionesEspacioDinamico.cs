using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerConfiguracionesEspacioDinamico : IConsultaEscalar<KendoGrid<ConfiguracionEspacioDinamicoDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerConfiguracionesEspacioDinamico(KendoGridMvcRequest request)
        {
            this.request = request;            
        }

        private static KendoGrid<ConfiguracionEspacioDinamicoDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query =
                from conf in contexto.Set<ConfiguracionEspacioDinamico>()                
                select new ConfiguracionEspacioDinamicoDto()
                {
                    Id = conf.Id,
                    Fecha = conf.Fecha,
                    Material = conf.Material.Descripcion,
                    Centro = conf.Centro.Descripcion,
                    CentroId = conf.CentroId,
                    MaterialId = conf.MaterialId,
                    ProveedorId = conf.ProveedorId,
                    ProveedorRazonSocial = conf.Proveedor.RazonSocial,
                    ProveedorCUIT = conf.Proveedor.CUIT,
                    CantidadDeCupo = conf.CantidadDeCupo,
                    ComercialId = conf.ComercialId,
                    ComercialNombre = conf.Comercial.Nombres + " " + conf.Comercial.Apellido,
                    Calidad = conf.Calidad
                };
            
            return new KendoGrid<ConfiguracionEspacioDinamicoDto>(request, query);
        }

        public virtual KendoGrid<ConfiguracionEspacioDinamicoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
