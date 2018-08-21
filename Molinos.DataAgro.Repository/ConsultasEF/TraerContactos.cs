using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerContactos : IConsulta<Contactos>
    {
        private readonly oParamBusqueda oParam;

        public TraerContactos(oParamBusqueda oParam)
        {
            this.oParam = oParam;
        }

        private static List<Contactos> Query(DbContext contexto, oParamBusqueda oParam)
        {
            var campanias = string.IsNullOrEmpty(oParam.Campaña) ? oParam.Campaña.Split('|') : null;
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var proveedorEstados = contexto.Set<ProveedorEstado>().Where(x => oParam.Equipo.Contains(x.Comercial.ComercialId)).GroupBy(x => x.Proveedor)
                .Select(x => x.FirstOrDefault(r => r.Estado.EstadoId == 4) ?? x.FirstOrDefault(r => r.Estado.EstadoId == 5) ?? x.FirstOrDefault(r => r.Estado.EstadoId == 1) ?? x.FirstOrDefault(r => r.Estado.EstadoId == 2) ?? x.FirstOrDefault(r => r.Estado.EstadoId == 3) ?? new ProveedorEstado { Proveedor = x.Key, Estado = contexto.Set<Estado>().FirstOrDefault(r => r.EstadoId == 1) });

            var resultado =
                from proveedorEstado in proveedorEstados
                join fac in contexto.Set<FACACOP>() on proveedorEstado.Proveedor.CUIT equals fac.CUIT into facs
                from fac in facs.DefaultIfEmpty()
                join rg in contexto.Set<RG2300>() on proveedorEstado.Proveedor.CUIT equals rg.CUIT into rgs
                from rg in rgs.DefaultIfEmpty()
                join contactoComercial in contexto.Set<ContactoComercial>() on proveedorEstado.Proveedor.ProveedorId equals contactoComercial.Proveedor.ProveedorId into cons
                from contactoComercial in cons.DefaultIfEmpty()
                where contactoComercial.EsPrincipal == true
                select new Contactos()
                {
                    Calificacion = proveedorEstado.Proveedor.Calificacion,
                    ComercialAcargo = proveedorEstado.Comercial.Apellido,
                    Email1 = contactoComercial.Email1,
                    Email2 = contactoComercial.Email2,
                    Email3 = contactoComercial.Email3,
                    Email4 = null,
                    CUIT = proveedorEstado.Proveedor.CUIT,
                    Telefono1 = contactoComercial.Telefono1,
                    Telefono2 = contactoComercial.Telefono2,
                    Telefono3 = contactoComercial.Telefono3,
                    Telefono4 = null,
                    RazonSocial = proveedorEstado.Proveedor.RazonSocial,
                    ProveedorId = proveedorEstado.Proveedor.ProveedorId,
                    FechaUltimoContacto = proveedorEstado.Proveedor.FechaUltimoContacto,
                    Estado = proveedorEstado.Estado.Descripcion,
                    Facacop = (fac.CUIT == null) ? 0 : 1,
                    RiesgoComercialSap = proveedorEstado.Proveedor.RiesgoComercialSap,
                    Situacion = rg.Situacion != null ? rg.Situacion : ""
                };

            return resultado.ToList();
        }

        public virtual List<Contactos> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, oParam);
            }
        }
    }
}
