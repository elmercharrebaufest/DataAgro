using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
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
            // Cargar los contratos en memoria antes de hacer las siguientes consultas
            var basicosContratosList = queryContratos
                .Where(x => contratos.Contains(x.Negocio) && x.Estado == (int)EnumEstadoContrato.Finalizado)
                .ToList();

            // Cargar direccionesSap una vez que los contratos estén en memoria
            var proveedorIds = basicosContratosList.Select(c => c.ProveedorId).Distinct().ToList();
            var direccionesSap = contexto.Set<MailProveedor>()
                .Where(x => proveedorIds.Contains((int)x.ProveedorId) && !string.IsNullOrEmpty(x.DireccionSap))
                .ToList();

            // Cargar precios una vez que los contratos estén en memoria
            var contratoIds = basicosContratosList.Select(c => c.Id).ToList();
            var precios = contexto.Set<PrecioPactado>()
                .Where(x => contratoIds.Contains(x.ContratoId))
                .ToList();

            return basicosContratosList.Select(contrato =>
            {
                // usar direcciones fiscales de SAP
                contrato.ProveedorDireccion = direccionesSap
                    .Where(x => x.ProveedorId == contrato.ProveedorId)
                    .Select(x => x.DireccionSap).FirstOrDefault();

                contrato.ProveedorLocalidad = direccionesSap
                    .Where(x => x.ProveedorId == contrato.ProveedorId)
                    .Select(x => x.LocalidadSap).FirstOrDefault();

                contrato.ProveedorProvincia = direccionesSap
                    .Where(x => x.ProveedorId == contrato.ProveedorId)
                    .Select(x => x.ProvinciaSap).FirstOrDefault();

                contrato.ProveedorCP = direccionesSap
                    .Where(x => x.ProveedorId == contrato.ProveedorId)
                    .Select(x => x.CodigoPostalSap).FirstOrDefault();

                // guardar precios pactados
                contrato.PreciosPactados = precios
                    .Where(x => x.ContratoId == contrato.Id)
                    .Select(e => new PrecioPactadosDto
                    {
                        ContratoId = e.ContratoId,
                        FechaDesde = e.FechaDesde.HasValue ? e.FechaDesde.Value.ToString("dd/MM/yyyy") : null,
                        FechaHasta = e.FechaHasta.HasValue ? e.FechaHasta.Value.ToString("dd/MM/yyyy") : null,
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