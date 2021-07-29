using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerMonedaKilo : IConsulta<PrecioCantidadDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int centroId;
        private List<int> materialId;
        public TraerMonedaKilo(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0)
        {
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.centroId = centroId;
            this.materialId = materialId;
        }

        public List<PrecioCantidadDto> Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;
            if (materialId == null || materialId.Count() == 0) materialId = contexto.Set<Material>().Select(a=>a.MaterialId).ToList();

            var precioPizarraPorMaterial = contexto.Set<PrecioPizarra>().GroupBy(x => x.MaterialId).Select(x => new { MaterialId = x.Key, x.OrderByDescending(y => y.FechaHasta).FirstOrDefault().MonedaId, x.OrderByDescending(y => y.FechaHasta).FirstOrDefault().Precio });
                        
            var cont = contexto.Set<Contrato>()
                .Where(x =>materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && x.TipoNegocioId == 2 && 
                DbFunctions.TruncateTime(x.FechaOperacion) >= fechaHoy && 
                DbFunctions.TruncateTime(x.FechaOperacion) <= fechaManana && 
                (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && 
                (0 == centroId || x.DestinoId == centroId) 
                && x.ContratoAcuerdo == null
                && x.TipoAgenteCompraId == null
                && (x.Canje != true)
                && (x.PrestamoDevolucion != true)
                && (x.Venta != true)
                && x.Pizarra != true)
                .GroupBy(x => x.MonedaId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad = x.Sum(y => (y.PrecioNeto == null) ? (double)y.Precio * y.Cantidad / 1000 : (double)y.PrecioNeto.Value * y.Cantidad / 1000)
                }).ToList();

            var fij = contexto.Set<FijacionDePrecioContrato>()
                .Where(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) >= fechaHoy 
                && DbFunctions.TruncateTime(x.FechaOperacion) <= fechaManana && 
                (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && 
                (centroId == 0 || centroId == 1) && x.Canje != true                 
                && x.Pizarra != true)
                .GroupBy(x => x.MonedaId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad = x.Sum(y => (y.PrecioNeto == null) ? (double)y.Precio * y.Cantidad / 1000 : (double)y.PrecioNeto.Value * y.Cantidad / 1000)
                }).ToList();

            var contPizarra = contexto.Set<Contrato>()
                .Where(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && x.TipoNegocioId == 2 &&
                DbFunctions.TruncateTime(x.FechaOperacion) >= fechaHoy &&
                DbFunctions.TruncateTime(x.FechaOperacion) <= fechaManana &&
                (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) &&
                (0 == centroId || x.DestinoId == centroId)
                && x.ContratoAcuerdo == null
                && (x.Canje != true)
                && (x.PrestamoDevolucion != true)
                && (x.Venta != true)
                && x.Pizarra == true)
                .GroupBy(x => x.MaterialId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {                   
                    Moneda = precioPizarraPorMaterial.Any(y=>y.MaterialId==x.Key)? precioPizarraPorMaterial.FirstOrDefault(y => y.MaterialId == x.Key).MonedaId:"",
                    Cantidad = precioPizarraPorMaterial.Any(y => y.MaterialId == x.Key) ?
                     x.Sum(y => precioPizarraPorMaterial.FirstOrDefault(z => z.MaterialId == x.Key).Precio * y.Cantidad / 1000) : 0,
                }).ToList();

            var fijPizarra = contexto.Set<FijacionDePrecioContrato>()
                .Where(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) >= fechaHoy
                && DbFunctions.TruncateTime(x.FechaOperacion) <= fechaManana &&
                (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) &&
                (centroId == 0 || centroId == 1)
                && (x.Canje != true)
                && x.Pizarra == true)
                .GroupBy(x => x.MaterialId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = precioPizarraPorMaterial.Any(y => y.MaterialId == x.Key) ? precioPizarraPorMaterial.FirstOrDefault(y => y.MaterialId == x.Key).MonedaId : "",
                    Cantidad = precioPizarraPorMaterial.Any(y => y.MaterialId == x.Key) ?
                     x.Sum(y => precioPizarraPorMaterial.FirstOrDefault(z => z.MaterialId == x.Key).Precio * y.Cantidad / 1000) : 0,
                }).ToList();

            var fas = contexto.Set<Fason>().Where(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == 1))
                .GroupBy(x => x.MonedaId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad = x.Sum(y => (double)y.Precio * y.Cantidad / 1000)
                }).ToList();

            var contAcuerdo = contexto.Set<ContratoAcuerdo>().Where(x => x.TipoAgenteCompraId == null && materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && (x.PrecioNeto != null && x.PrecioNeto != 0) && DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (0 == centroId || x.DestinoId == centroId))
               .GroupBy(x => x.MonedaId).DefaultIfEmpty()
               .Select(x => new PrecioCantidadDto()
               {
                   Moneda = x.Key,
                   Cantidad = x.Sum(y => (double)y.Precio * y.Cantidad  / 1000)
               }).ToList();

            var res = cont.Union(fij).Union(fas).Union(contAcuerdo).Union(contPizarra).Union(fijPizarra).GroupBy(x => x.Moneda)
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad = x.Sum(y => y.Cantidad)
                }).ToList();
            return res;
        }
    }
}
