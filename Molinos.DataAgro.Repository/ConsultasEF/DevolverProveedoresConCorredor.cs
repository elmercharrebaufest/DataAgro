using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class DevolverProveedoresConCorredor : IConsulta<BusquedaHome>
    {
        private readonly string filtro;
        private readonly string cuitCorredor;

        public DevolverProveedoresConCorredor(string filtro, string cuitCorredor)
        {
            this.filtro = filtro;
            this.cuitCorredor = cuitCorredor;
        }

        private static List<BusquedaHome> Query(DbContext contexto, string filtro, string cuitCorredor)
        {
            var resultado = (from corredorProveedor in contexto.Set<CorredorProveedor>()
                            join c in contexto.Set<ContactoComercial>() on corredorProveedor.ProveedorId equals c.ProveedorId into rgs
                            from c in rgs.DefaultIfEmpty()
                            where (corredorProveedor.Corredor.CUIT.Contains(cuitCorredor))
                            && (corredorProveedor.Proveedor.CUIT.Contains(filtro) || c.Nombres.Contains(filtro) || c.Apellido.Contains(filtro) || corredorProveedor.Proveedor.RazonSocial.Contains(filtro))
                            group c by corredorProveedor into provs
                            select new BusquedaHome
                            {
                                Id = provs.Key.ProveedorId,
                                Cuit = provs.Key.Proveedor.CUIT,
                                RazonSocial = provs.Key.Proveedor.RazonSocial,
                                Filtro = filtro + "|" + provs.Key.Proveedor.RazonSocial + " (" + provs.Key.Proveedor.CUIT + ")"
                            }).Distinct().Take(15);

            return resultado.ToList();
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtro, cuitCorredor);
            }
        }
    }
}
