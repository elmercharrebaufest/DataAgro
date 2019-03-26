using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerExportarAllContactosComerciales : IConsulta<ContactosPrincipalesAll>
    {
        private readonly List<string> proveedores;

        public TraerExportarAllContactosComerciales(List<string> proveedores)
        {
            this.proveedores = proveedores;
        }

        private static List<ContactosPrincipalesAll> Query(DbContext contexto, List<string> proveedores)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from ContactoComercial in contexto.Set<ContactoComercial>()
                join Proveedor in contexto.Set<Proveedor>() on ContactoComercial.ProveedorId equals Proveedor.ProveedorId
                join ContactoComercialInteres in contexto.Set<ContactoComercialInteres>() on ContactoComercial.ContactoComercialId equals ContactoComercialInteres.ContactoComercialId into ComInteres
                from intereses in ComInteres.DefaultIfEmpty()
                where proveedores.Contains(ContactoComercial.Proveedor.CUIT)
                select new ContactosPrincipalesAll
                {
                    Apellido = ContactoComercial.Apellido,
                    Cuit = Proveedor.CUIT,
                    Email = ContactoComercial.Email1,
                    FechaNacimiento = ContactoComercial.FechaNacimiento.HasValue ? ContactoComercial.FechaNacimiento.ToString() : "",
                    Interes = intereses.Interes.Descripcion,
                    Nombre = ContactoComercial.Nombres,
                    OtrosIntereses = ContactoComercial.OtrosIntereses,
                    Principal = ContactoComercial.EsPrincipal.HasValue && ContactoComercial.EsPrincipal.Value ? "SI" : "NO",
                    Profesion = ContactoComercial.Cargo,
                    Puesto = ContactoComercial.Puesto,
                    RazonSocial = Proveedor.RazonSocial,
                    Telefono = ContactoComercial.Telefono1
                };

            return resultado.Distinct().ToList();
        }

        public virtual List<ContactosPrincipalesAll> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, proveedores);
            }
        }
    }
}
