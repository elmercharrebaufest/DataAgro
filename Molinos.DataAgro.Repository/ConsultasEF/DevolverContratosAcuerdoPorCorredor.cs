using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class DevolverContratosAcuerdoPorCorredor : IConsulta<ContratoCopiar>
    {
        private readonly int filtro;
        private readonly DateTime dia;

        public DevolverContratosAcuerdoPorCorredor(int filtro, DateTime dia)
        {
            this.filtro = filtro;
            this.dia = dia;
        }

        private static List<ContratoCopiar> Query(DbContext contexto, int filtro, DateTime dia)
        {
            
          
            var resultado = (from c in contexto.Set<ContratoAcuerdo>()
                             where c.CorredorId == filtro 
                             && c.EstadoId.Equals(2)
                             && c.Fecha >= dia
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
                return Query(contexto, filtro, dia);
            }
        }
    }
}
