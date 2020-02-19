using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerCorredoresComercial : IConsulta<Contactos>
    {
        public TraerCorredoresComercial()
        {
        }

        private static List<Contactos> Query(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from pCom in contexto.Set<ProveedorComercial>()
                join prove in contexto.Set<Proveedor>() on pCom.ProveedorId equals prove.ProveedorId
                join comercial in contexto.Set<Comercial>() on pCom.ComercialId equals comercial.ComercialId
                join est in contexto.Set<Estado>() on prove.EstadoId equals est.EstadoId into estados
                from ests in estados.DefaultIfEmpty()
                where prove.SegmentacionId==5 || prove.SegmentacionId == 7
                group prove by prove into provs
                select new Contactos
                {
                    ProveedorId = provs.Key.ProveedorId,
                    CUIT = provs.Key.CUIT,
                    Calificacion = provs.Key.Calificacion,
                    ComercialAcargo = contexto.Set<ProveedorComercial>().Where(x => x.ProveedorId == provs.Key.ProveedorId).FirstOrDefault().Comercial.Apellido,
                    Email1 = contexto.Set<ContactoComercial>().Where(x => x.ProveedorId == provs.Key.ProveedorId).FirstOrDefault().Email1,
                    Email2 = contexto.Set<ContactoComercial>().Where(x => x.ProveedorId == provs.Key.ProveedorId).FirstOrDefault().Email2,
                    Email3 = contexto.Set<ContactoComercial>().Where(x => x.ProveedorId == provs.Key.ProveedorId).FirstOrDefault().Email3,
                    Email4 = null,
                    RazonSocial = provs.Key.RazonSocial,
                    Telefono1 = contexto.Set<ContactoComercial>().Where(x => x.ProveedorId == provs.Key.ProveedorId).FirstOrDefault().Telefono1,
                    Telefono2 = contexto.Set<ContactoComercial>().Where(x => x.ProveedorId == provs.Key.ProveedorId).FirstOrDefault().Telefono2,
                    Telefono3 = contexto.Set<ContactoComercial>().Where(x => x.ProveedorId == provs.Key.ProveedorId).FirstOrDefault().Telefono3,
                    Telefono4 = null,
                    FechaUltimoContacto = provs.Key.FechaUltimoContacto,
                    Estado = provs.Key.Estado.Descripcion,
                    EstadoCuit = (contexto.Set<SISA>().Where(x => x.CUIT == provs.Key.CUIT).FirstOrDefault() != null) ? contexto.Set<SISA>().Where(x => x.CUIT == provs.Key.CUIT).FirstOrDefault().EstadoCuit : 0,
                    FechaAlta = provs.Key.FechaAlta,
                    GrupoDeCompras = "",
                    Segmentacion = provs.Key.SegmentacionId,
                    RiesgoComercialSap = provs.Key.RiesgoComercialSap,
                    Facacop = contexto.Set<FACACOP>().Where(x => x.CUIT == provs.Key.CUIT).Any() ? 1 : 0
                };

            return resultado.ToList();
        }

        public virtual List<Contactos> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto);
            }
        }
    }
}
