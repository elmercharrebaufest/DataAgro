using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodosContratosBoleto : IConsultaEscalar<IQueryable<BasicoContrato>>
    {
        private readonly List<string> contratos;
        private readonly List<int> equipo;

        public TraerTodosContratosBoleto(List<string> contratos, List<int> equipo)
        {
            this.contratos = contratos;
            this.equipo = equipo;
        }

        private static IQueryable<BasicoContrato> Query(DbContext contexto, List<string> contratos, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos = TraerTodosContratosSinFiltro.QueryBase(contexto, equipo);
            var basicosContratos = queryContratos.Where(x => contratos.Contains(x.Negocio) && x.Estado == 5);

            var direccionesSap = contexto.Set<MailProveedor>()
                .Where(x => basicosContratos.Select(c => c.ProveedorId).Contains((int)x.ProveedorId)).ToList();

            var precios = contexto.Set<PrecioPactado>()
                .Where(x => basicosContratos.Select(c => c.Id).Contains(x.ContratoId)).ToList();

            return basicosContratos.AsEnumerable().Select(contrato =>
            {
                // usar direcciones fiscales de SAP
                contrato.ProveedorDireccion = direccionesSap
                    .Where(x => x.ProveedorId == contrato.ProveedorId && !string.IsNullOrEmpty(x.DireccionSap))
                    .Select(x => x.DireccionSap).FirstOrDefault();

                contrato.ProveedorLocalidad = direccionesSap
                    .Where(x => x.ProveedorId == contrato.ProveedorId && !string.IsNullOrEmpty(x.DireccionSap))
                    .Select(x => x.LocalidadSap).FirstOrDefault();

                contrato.ProveedorProvincia = direccionesSap
                    .Where(x => x.ProveedorId == contrato.ProveedorId && !string.IsNullOrEmpty(x.DireccionSap))
                    .Select(x => x.ProvinciaSap).FirstOrDefault();

                contrato.ProveedorCP = direccionesSap
                    .Where(x => x.ProveedorId == contrato.ProveedorId && !string.IsNullOrEmpty(x.DireccionSap))
                    .Select(x => x.CodigoPostalSap).FirstOrDefault();

                // guardar precios pactados
                contrato.PreciosPactados = precios
                    .Where(x => x.ContratoId == contrato.Id)
                    .Select(e => new PrecioPactadosDto
                    {
                        ContratoId = e.ContratoId,
                        FechaDesde = e.FechaDesde.HasValue ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", e.FechaDesde), 2) + "/" +
                          DbFunctions.Right("0" + SqlFunctions.DatePart("month", e.FechaDesde), 2) + "/" +
                          SqlFunctions.DateName("year", e.FechaDesde) : "",
                        FechaHasta = e.FechaHasta.HasValue ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", e.FechaHasta), 2) + "/" +
                          DbFunctions.Right("0" + SqlFunctions.DatePart("month", e.FechaHasta), 2) + "/" +
                          SqlFunctions.DateName("year", e.FechaHasta) : "",
                        MonedaPactadoDesc = e.MonedaPactado.Descripcion ?? "",
                        MonedaPactadoId = e.MonedaPactadoId ?? "",
                        Precio = e.Precio,
                        Porcentaje = e.Porcentaje
                    }).ToList();

                return contrato;
            }).AsQueryable();
        }

        public virtual IQueryable<BasicoContrato> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, contratos, equipo);
            }
        }
    }
}