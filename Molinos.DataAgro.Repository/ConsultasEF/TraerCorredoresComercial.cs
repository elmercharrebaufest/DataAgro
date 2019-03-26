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
        private readonly List<int> idComerciales;
        private readonly int comercialOriginalId;

        public TraerCorredoresComercial(List<int> idComerciales, int comercialOriginalId)
        {
            this.idComerciales = idComerciales;
            this.comercialOriginalId = comercialOriginalId;
        }

        private static List<Contactos> Query(DbContext contexto, List<int> idComerciales, int comercialOriginalId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from pCom in contexto.Set<ProveedorComercial>()
                join prove in contexto.Set<Proveedor>() on pCom.ProveedorId equals prove.ProveedorId
                join contacto in contexto.Set<ContactoComercial>() on prove.ProveedorId equals contacto.ProveedorId into con
                from cont in con.DefaultIfEmpty()
                join comercial in contexto.Set<Comercial>() on pCom.ComercialId equals comercial.ComercialId
                join est in contexto.Set<Estado>() on prove.EstadoId equals est.EstadoId into estados
                from ests in estados.DefaultIfEmpty()
                where idComerciales.Contains(pCom.ComercialId)
                select new Contactos
                {
                    ProveedorId = pCom.ProveedorId,
                    CUIT = prove.CUIT,
                    Calificacion = prove.Calificacion,
                    ComercialAcargo = comercial.Apellido,
                    Email1 = cont.Email1,
                    Email2 = cont.Email2,
                    Email3 = cont.Email3,
                    Email4 = null,
                    RazonSocial = prove.RazonSocial,
                    Telefono1 = cont.Telefono1,
                    Telefono2 = cont.Telefono2,
                    Telefono3 = cont.Telefono3,
                    Telefono4 = null,
                    FechaUltimoContacto = prove.FechaUltimoContacto,
                    Estado = prove.Estado.Descripcion,
                    EstadoCuit = (contexto.Set<SISA>().Where(x => x.CUIT == prove.CUIT).FirstOrDefault() != null) ? contexto.Set<SISA>().Where(x => x.CUIT == prove.CUIT).FirstOrDefault().EstadoCuit : 0,
                    FechaAlta = prove.FechaAlta,
                    GrupoDeCompras = "",
                    Segmentacion = prove.SegmentacionId,
                    RiesgoComercialSap = prove.RiesgoComercialSap,
                    Facacop = contexto.Set<FACACOP>().Where(x => x.CUIT == prove.CUIT).Any() ? 1 : 0
                };

            return resultado.Distinct().ToList();
        }

        public virtual List<Contactos> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, idComerciales, comercialOriginalId);
            }
        }
    }
}
