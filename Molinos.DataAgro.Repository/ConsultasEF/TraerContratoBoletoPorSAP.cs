using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    /// <summary>
    /// Consulta optimizada para obtener un contrato de boleto por SAP sin filtro de equipo.
    /// Similar a TraerTodosContratosBoleto pero busca un único contrato directamente.
    /// </summary>
    public class TraerContratoBoletoPorSAP : IConsultaEscalar<BasicoContrato>
    {
        private readonly string contratoSAP;

        public TraerContratoBoletoPorSAP(string contratoSAP)
        {
            this.contratoSAP = contratoSAP;
        }

        private static BasicoContrato Query(DbContext contexto, string contratoSAP)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos = TraerTodosContratosSinFiltroOptimizado.QueryBase(contexto, contratoSAP);

            var contrato = queryContratos
                .Where(x => x.Estado == (int)EnumEstadoContrato.Finalizado)
                .FirstOrDefault();

            if (contrato == null)
            {
                return null;
            }

            var direccionSap = contexto.Set<MailProveedor>()
                .AsNoTracking()
                .Where(x => x.ProveedorId == contrato.ProveedorId && !string.IsNullOrEmpty(x.DireccionSap))
                .Select(x => new
                {
                    x.DireccionSap,
                    x.LocalidadSap,
                    x.ProvinciaSap,
                    x.CodigoPostalSap
                })
                .FirstOrDefault();

            if (direccionSap != null)
            {
                contrato.ProveedorDireccion = direccionSap.DireccionSap;
                contrato.ProveedorLocalidad = direccionSap.LocalidadSap;
                contrato.ProveedorProvincia = direccionSap.ProvinciaSap;
                contrato.ProveedorCP = direccionSap.CodigoPostalSap;
            }
            var precios = contexto.Set<PrecioPactado>()
                .Where(x => x.ContratoId == contrato.Id)
                .ToList();
            if (precios != null && precios.Any())
            {
                contrato.PreciosPactados = precios
                    .Select(e => new PrecioPactadosDto
                    {
                        ContratoId = e.ContratoId,
                        FechaDesde = e.FechaDesde.HasValue ? e.FechaDesde.Value.ToString("dd/MM/yyyy") : null,
                        FechaHasta = e.FechaHasta.HasValue ? e.FechaHasta.Value.ToString("dd/MM/yyyy") : null,
                        MonedaPactadoDesc = e.MonedaPactado.Descripcion ?? "",
                        MonedaPactadoId = e.MonedaPactadoId ?? "",
                        Precio = e.Precio,
                        Porcentaje = e.Porcentaje
                    })
                    .ToList();
            }
            return contrato;
        }

        public virtual BasicoContrato Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                }))
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                return Query(contexto, contratoSAP);
            }
        }
    }
}
