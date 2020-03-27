using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class DevolverContratos : IConsulta<ContratoCopiar>
    {
        private readonly string nroSap;

        public DevolverContratos(string nroSap)
        {
            this.nroSap = nroSap;
        }

        private static List<ContratoCopiar> Query(DbContext contexto, string nroSap)
        {
            var resultado = (from c in contexto.Set<Contrato>()
                             where (c.ContratoSAP.Contains(nroSap))

                             select new ContratoCopiar
                             {
                                 Id = c.Id,
                                 Comercial = c.Comercial.Nombres + " " + c.Comercial.Apellido,
                                 RazonSocial = c.Proveedor.RazonSocial,
                                 Filtro = c.ContratoSAP,
                                 ContratoSap = c.ContratoSAP,
                                 CantidadD = c.Cantidad,
                                 Fecha = SqlFunctions.DateName("day", c.Fecha) != null ? SqlFunctions.DateName("day", c.Fecha) + "/" + SqlFunctions.DatePart("month", c.Fecha) + "/" + SqlFunctions.DateName("year", c.Fecha) : "",
                                 Material = c.Material.Descripcion,
                                 tipoNegocio = c.TipoNegocioId.ToString()
                             }
                             ).Take(15);

            return resultado.ToList();
        }

        public virtual List<ContratoCopiar> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, nroSap);
            }
        }
    }
}
