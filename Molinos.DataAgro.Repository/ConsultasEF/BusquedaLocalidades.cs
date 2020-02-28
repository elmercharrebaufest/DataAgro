using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class BusquedaLocalidades : IConsulta<BusquedaLocalidad>
    {
        private readonly string filtro;

        public BusquedaLocalidades(string filtro)
        {
            this.filtro = filtro;
        }

        private static List<BusquedaLocalidad> Query(DbContext contexto, string filtro)
        {
            var resultado = from Localidad in contexto.Set<Localidad>()
                            select new BusquedaLocalidad
                            {
                                Id = Localidad.LocalidadId,
                                Localidad = Localidad.Nombre,
                                Provincia= Localidad.Provincia.Nombre,
                                Partido = Localidad.Partido.Descripcion,
                                ProvinciaId = Localidad.ProvinciaId,
                                Filtro = Localidad.Nombre + " (" + Localidad.Provincia.Nombre + ")"
                            };

            return resultado.Where(x=>x.Filtro.StartsWith(filtro)).Take(20).ToList();
        }

        public virtual List<BusquedaLocalidad> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtro);
            }
        }
    }
}
