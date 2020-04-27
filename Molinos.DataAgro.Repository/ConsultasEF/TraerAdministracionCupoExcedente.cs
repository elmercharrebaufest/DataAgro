using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerAdministracionCupoExcedente : IConsultaEscalar<KendoGrid<AdministracionCupoDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerAdministracionCupoExcedente(KendoGridMvcRequest request)
        {
            this.request = request;            
        }

        private static KendoGrid<AdministracionCupoDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query =
                from cupo in contexto.Set<AdministracionCupo>()    
                where cupo.Excedente == true
                select new AdministracionCupoDto()
                {
                    Id = cupo.Id,                    
                    Fecha = cupo.Fecha,
                    EstadoId = cupo.EstadoId,
                    CantidadFleteProcedencia = cupo.CantidadFleteProcedencia,
                    CantidadCupo = cupo.CantidadCupo,
                    Comercial  = cupo.Comercial.Nombres +" "+cupo.Comercial.Apellido,
                    Proveedor = cupo.Proveedor.RazonSocial,
                    Material = cupo.Material.Descripcion,
                    Centro = cupo.Centro.Descripcion,
                    Zona = cupo.Zona.Descripcion,
                    Excedente = cupo.Excedente
                };
            
            return new KendoGrid<AdministracionCupoDto>(request, query);
        }

        public virtual KendoGrid<AdministracionCupoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
