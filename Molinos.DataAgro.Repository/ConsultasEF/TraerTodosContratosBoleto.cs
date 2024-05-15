using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;
using System.Linq;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodosContratosBoleto : IConsultaEscalar<IQueryable<BasicoContrato>>
    {
        private readonly List<string> contratos;
        private readonly List<int> equipo;
        private readonly bool corredor;
        private readonly List<int> corredoresComercial;

        public TraerTodosContratosBoleto(List<string> contratos, bool corredor, List<int> equipo, List<int> corredoresComercial)
        {
            this.contratos = contratos;
            this.equipo = equipo;
            this.corredor = corredor;
            this.corredoresComercial = corredoresComercial;
        }

        private static IQueryable<BasicoContrato> Query(DbContext contexto, List<string> contratos, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos = TraerTodosContratosSinFiltro.QueryBase(contexto, equipo);
            var basicosContratos = queryContratos.Where(x => contratos.Contains(x.Negocio) && x.Estado == 5);

            List<MailProveedor> direccionesSap = contexto.Set<MailProveedor>()
                .Where(x => basicosContratos.Select(c => c.ProveedorId).Contains((int)x.ProveedorId)).ToList();
            List<PrecioPactado> precios = contexto.Set<PrecioPactado>()
                .Where(x => basicosContratos.Select(c => c.Id).Contains(x.ContratoId)).ToList();
            var bcList = basicosContratos.ToList();
            foreach (var contrato in bcList)
            {
                //usar direcciones fiscales de SAP
                var prov = direccionesSap.Find(x => x.ProveedorId == contrato.ProveedorId && !string.IsNullOrEmpty(x.DireccionSap));
                contrato.ProveedorDireccion = prov?.DireccionSap;
                contrato.ProveedorLocalidad = prov?.LocalidadSap;
                contrato.ProveedorProvincia = prov?.ProvinciaSap;
                contrato.ProveedorCP = prov?.CodigoPostalSap;
                //guardar precios pactados
                if (precios.Count > 0)
                {
                    contrato.PreciosPactados = precios.Where(x => x.ContratoId == contrato.Id)?.Select(e => new PrecioPactadosDto
                    {
                        ContratoId = e.ContratoId,
                        FechaDesde = e.FechaDesde?.ToString("dd/MM/yyyy"),
                        FechaHasta = e.FechaHasta?.ToString("dd/MM/yyyy"),
                        MonedaPactadoDesc = e.MonedaPactado.Descripcion ?? "",
                        MonedaPactadoId = e.MonedaPactadoId ?? "",
                        Precio = e.Precio,
                        Porcentaje = e.Porcentaje
                    }).ToList();
                }
            }

            //GridHelper.TruncateTime(request.Filter, ref queryContratos);
            //return queryContratos.Where(x => (!(string.IsNullOrEmpty( x.Negocio)) ? contratos.All(c => x.Negocio.Contains(c)) : contratos.All(c => x.ContratoSAP.Contains(c)))  && x.Estado == 5);
            return bcList.AsQueryable();
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