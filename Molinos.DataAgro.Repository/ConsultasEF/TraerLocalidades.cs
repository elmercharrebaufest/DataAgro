using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerLocalidades : IConsulta<BusquedaLocalidad>
    {

        public TraerLocalidades()
        {

        }

        private static List<BusquedaLocalidad> Query(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado = from Localidad in contexto.Set<Localidad>()
                            select new BusquedaLocalidad
                            {
                                Id = Localidad.LocalidadId,
                                Localidad = Localidad.Nombre,
                                CodLocalidad = Localidad.CodLocalidad,
                                Provincia = Localidad.Provincia.Nombre,
                                Partido = Localidad.Partido.Descripcion,
                                PartidoId = Localidad.Partido != null ? Localidad.Partido.Id : 0,
                                ProvinciaId = Localidad.ProvinciaId
                            };

            return resultado.ToList();
        }

        public virtual List<BusquedaLocalidad> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto);
            }
        }
    }
}
