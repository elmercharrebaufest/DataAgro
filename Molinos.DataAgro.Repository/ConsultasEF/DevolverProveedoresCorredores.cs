using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class DevolverProveedoresCorredores : IConsulta<BusquedaHome>
    {
        private readonly string filtro;

        public DevolverProveedoresCorredores(string filtro)
        {
            this.filtro = filtro;
        }

        private static List<BusquedaHome> Query(DbContext contexto, string filtro)
        {
            var resultado = from Proveedor in contexto.Set<Proveedor>()
                            join p in contexto.Set<ProveedorComercial>() on Proveedor.ProveedorId equals p.ProveedorId into rgs
                            from p in rgs.DefaultIfEmpty()
                            join c in contexto.Set<ContactoComercial>() on Proveedor.ProveedorId equals c.ProveedorId into rg
                            from c in rg.DefaultIfEmpty()
                            where Proveedor.CUIT.Contains(filtro) || Proveedor.RazonSocial.Contains(filtro) ||
                            c.Nombres.Contains(filtro) || c.Apellido.Contains(filtro)
                            group c by Proveedor into provs
                            select new BusquedaHome
                            {
                                Id = provs.Key.ProveedorId,
                                Cuit = provs.Key.CUIT,
                                RazonSocial = provs.Key.SegmentacionId == 5 || provs.Key.SegmentacionId == 7 ? "COR - " + provs.Key.RazonSocial : provs.Key.RazonSocial,
                                Corredor = provs.Key.SegmentacionId == 5 || provs.Key.SegmentacionId == 7 ? "COR" : "",
                                Filtro = filtro + "|" + provs.Key.RazonSocial + " (" + provs.Key.CUIT + ")"
                            };

            return resultado.Distinct().Take(15).ToList();
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtro);
            }
        }
    }
}
