using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class DevolverContratosAcuerdo : IConsulta<ContratoCopiar>
    {
        private readonly string filtro;

        public DevolverContratosAcuerdo(string filtro)
        {
            this.filtro = filtro;
        }

        private static List<ContratoCopiar> Query(DbContext contexto, string filtro)
        {
            
          
            var resultado = (from c in contexto.Set<ContratoAcuerdo>()
                             where (c.Comercial.Apellido.Contains(filtro) || c.Destino.Descripcion.Contains(filtro) ||
                             c.Proveedor.RazonSocial.Contains(filtro) || c.Material.Descripcion.Contains(filtro) ||
                             c.Id.ToString().Contains(filtro)) && c.EstadoId.Equals(2)

                             select new ContratoCopiar
                             {
                                 Id = c.Id,
                                 Comercial = c.Comercial.Nombres + " " + c.Comercial.Apellido,
                                 RazonSocial = c.ProveedorId != null && c.ProveedorId > 0 ? c.Proveedor.RazonSocial : c.Corredor.RazonSocial,
                                 Filtro = c.Id + " - " + c.Material.Descripcion + " - " + (c.ProveedorId != null && c.ProveedorId > 0 ? c.Proveedor.RazonSocial : c.Corredor.RazonSocial ) + " - " + (SqlFunctions.DateName("day", c.Fecha) != null ? SqlFunctions.DateName("day", c.Fecha) + "/" + SqlFunctions.DatePart("month", c.Fecha) + "/" + SqlFunctions.DateName("year", c.Fecha) : ""),
                                 CantidadD = c.Cantidad,
                                 Fecha = SqlFunctions.DateName("day", c.Fecha) != null ? SqlFunctions.DateName("day", c.Fecha) + "/" + SqlFunctions.DatePart("month", c.Fecha) + "/" + SqlFunctions.DateName("year", c.Fecha) : "",
                                 Material = c.Material.Descripcion,
                                 tipoNegocio = "2"
                             }
                             ).Take(15);

            return resultado.ToList();
        }

        public virtual List<ContratoCopiar> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtro);
            }
        }
    }
}
