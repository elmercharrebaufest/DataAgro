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
        private readonly int idComercial;
        private readonly int comercialOriginalId;

        public TraerCorredoresComercial(int idComercial, int comercialOriginalId)
        {
            this.idComercial = idComercial;
            this.comercialOriginalId = comercialOriginalId;
        }

        private static List<Contactos> Query(DbContext contexto, int comercialId, int comercialOriginalId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                //from cProv in contexto.Set<CorredorProveedor>()
                from pCom in contexto.Set<ProveedorComercial>()
                join prove in contexto.Set<Proveedor>() on pCom.ProveedorId equals prove.ProveedorId
                join contacto in contexto.Set<ContactoComercial>() on prove.ProveedorId equals contacto.ProveedorId
                join comercial in contexto.Set<Comercial>() on pCom.ComercialId equals comercial.ComercialId
                join est in contexto.Set<Estado>() on prove.EstadoId equals est.EstadoId
                where pCom.ComercialId == comercialId && contacto.EsPrincipal == true
                select new Contactos
                {
                    ProveedorId = pCom.ProveedorId,
                    CUIT = prove.CUIT,
                    Calificacion = prove.Calificacion,
                    ComercialAcargo = comercial.Apellido,
                    Email1 = contacto.Email1,
                    Email2 = contacto.Email2,
                    Email3 = contacto.Email3,
                    Email4 = null,
                    RazonSocial = prove.RazonSocial,
                    Telefono1 = contacto.Telefono1,
                    Telefono2 = contacto.Telefono2,
                    Telefono3 = contacto.Telefono3,
                    Telefono4 = null,
                    FechaUltimoContacto = prove.FechaUltimoContacto,
                    Estado = est.Descripcion,
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
                return Query(contexto, idComercial, comercialOriginalId);
            }
        }
    }
}
