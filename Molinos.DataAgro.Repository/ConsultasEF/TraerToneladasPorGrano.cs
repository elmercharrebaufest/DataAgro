using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerToneladasPorGrano : IConsultaEscalar<ToneladasGranoTipoDto>
    {
        private readonly int materialId;
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly bool? calidad;
        private readonly int centroId;
        public TraerToneladasPorGrano(int materialId, DateTime fechaDesde, DateTime fechaHasta, bool? calidad = null, int centroId = 0)
        {
            this.materialId = materialId;
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.calidad = calidad;
            this.centroId = centroId;
        }

        public ToneladasGranoTipoDto Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;
            var fechaPosicion = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(+1).Month, 1);
            var toneladasPorGrano = new ToneladasGranoTipoDto();
            var posicion = contexto.Set<Contrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && (calidad == null || (calidad != null && x.TrigoEspecial == calidad)) && (0 == centroId || x.DestinoId == centroId) && x.ContratoAcuerdo == null)
                .Select(x => new NegocioToneladasPosicionDto { Id = x.ContratoId,Fecha= x.Fecha, FechaDesde = x.FechaDesde,FechaHasta= x.FechaHasta,TipoNegocioId=  x.TipoNegocioId, MaterialCampanaId = x.Material.CampañaId.Value, CampanaId= x.CampanaId,Cantidad = x.Cantidad }).ToList();
            foreach (var pos in posicion)
            {
                if ((DateTime.DaysInMonth(pos.FechaDesde.Year, pos.FechaDesde.Month) - pos.FechaDesde.Day) >= 10)
                {
                    pos.Posicion = new DateTime(pos.FechaDesde.Year, pos.FechaDesde.Month,1);
                }
                else if (pos.FechaDesde.AddMonths(1).Month <= pos.FechaHasta.Month)
                {
                    pos.Posicion = new DateTime(pos.FechaDesde.Year, pos.FechaDesde.AddMonths(+1).Month, 1);
                }
                else if (pos.FechaDesde.AddMonths(1).Month > pos.FechaHasta.Month)
                {
                    pos.Posicion = new DateTime(pos.FechaHasta.Year, pos.FechaHasta.Month, 1);
                }
            }


            toneladasPorGrano.DispAFijar = posicion.Where(x => x.TipoNegocioId == 1 && (x.MaterialCampanaId > x.CampanaId || (x.MaterialCampanaId == x.CampanaId && fechaPosicion >= x.Posicion))).Sum(y => Math.Ceiling(y.Cantidad / 1000));
            toneladasPorGrano.DispAPrecio = posicion.Where(x => x.TipoNegocioId == 2 && (x.MaterialCampanaId > x.CampanaId || (x.MaterialCampanaId == x.CampanaId && fechaPosicion >= x.Posicion))).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.FrwAFijar = posicion.Where(x => x.TipoNegocioId == 1 && x.MaterialCampanaId == x.CampanaId && fechaPosicion < x.Posicion).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.FrwAPrecio = posicion.Where(x => x.TipoNegocioId == 2 && x.MaterialCampanaId == x.CampanaId && fechaPosicion < x.Posicion).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.NewAFijar = posicion.Where(x => x.TipoNegocioId == 1 && x.MaterialCampanaId < x.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.NewAPrecio = posicion.Where(x => x.TipoNegocioId == 2 && x.MaterialCampanaId < x.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();


            var fijaciones = contexto.Set<FijacionDePrecioContrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && (calidad == null || (calidad != null && x.TrigoEspecial == calidad)) && (centroId == 0 || centroId == 1))
                    .Select(x => new NegocioToneladasPosicionDto { Id = x.FijacionDePrecioContratoId, Fecha = x.Fecha, FechaDesde = x.FechaDesde, FechaHasta = x.FechaHasta, TipoNegocioId = 3, MaterialCampanaId = x.Material.CampañaId.Value, CampanaId = x.CampanaId, Cantidad = x.Cantidad }).ToList();
            foreach (var pos in fijaciones)
            {
                if ((DateTime.DaysInMonth(pos.FechaDesde.Year, pos.FechaDesde.Month) - pos.FechaDesde.Day) >= 10)
                {
                    pos.Posicion = new DateTime(pos.FechaDesde.Year, pos.FechaDesde.Month, 1);
                }
                else if (pos.FechaDesde.AddMonths(1).Month <= pos.FechaHasta.Month)
                {
                    pos.Posicion = new DateTime(pos.FechaDesde.Year, pos.FechaDesde.AddMonths(+1).Month, 1);
                }
                else if (pos.FechaDesde.AddMonths(1).Month > pos.FechaHasta.Month)
                {
                    pos.Posicion = new DateTime(pos.FechaHasta.Year, pos.FechaHasta.Month, 1);
                }
            }
            toneladasPorGrano.DispFijac = fijaciones.Where(x => x.MaterialCampanaId > x.CampanaId || (x.MaterialCampanaId == x.CampanaId && fechaPosicion >= x.Posicion)).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.FrwFijac = fijaciones.Where(x => x.MaterialCampanaId == x.CampanaId && fechaPosicion < x.Posicion).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.NewFijac = fijaciones.Where(x => x.MaterialCampanaId < x.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();

            var fason = contexto.Set<Fason>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && (calidad == null || (calidad != null && x.Especial == calidad)) && (centroId == 0 || centroId == 1))
                    .Select(x => new NegocioToneladasPosicionDto { Id = x.Id, Fecha = x.Fecha, FechaDesde = x.FechaDesde, FechaHasta = x.FechaHasta, TipoNegocioId = 4, MaterialCampanaId = x.Material.CampañaId.Value, CampanaId = x.CampanaId, Cantidad = x.Cantidad, PosicionString= x.Posicion }).ToList();

            foreach (var pos in fason)
            {
                var mes = int.Parse(pos.PosicionString.Substring(0, 2));
                var anio = int.Parse(pos.PosicionString.Substring(3, 4));
                pos.Posicion = new DateTime(anio, mes, 1);
            }

            toneladasPorGrano.DispFason = fason.Where(x => x.MaterialCampanaId > x.CampanaId||( x.MaterialCampanaId == x.CampanaId && fechaPosicion >= x.Posicion)).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.FrwFason = fason.Where(x => x.MaterialCampanaId == x.CampanaId && fechaPosicion < x.Posicion).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.NewFason = fason.Where(x => x.MaterialCampanaId < x.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();

            var acuerdos = contexto.Set<ContratoAcuerdo>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2) && x.MaterialId == materialId && (calidad == null || !calidad.Value) && (0 == centroId || x.DestinoId == centroId))
                    .Select(x => new NegocioToneladasPosicionDto { Id = x.Id, Fecha = x.Fecha, FechaDesde = x.FechaDesde, FechaHasta = x.FechaHasta, TipoNegocioId = 4, Cantidad = x.Cantidad }).ToList();
            foreach (var pos in acuerdos)
            {
                if ((DateTime.DaysInMonth(pos.FechaDesde.Year, pos.FechaDesde.Month) - pos.FechaDesde.Day) >= 10)
                {
                    pos.Posicion = new DateTime(pos.FechaDesde.Year, pos.FechaDesde.Month, 1);
                }
                else if (pos.FechaDesde.AddMonths(1).Month <= pos.FechaHasta.Month)
                {
                    pos.Posicion = new DateTime(pos.FechaDesde.Year, pos.FechaDesde.AddMonths(+1).Month, 1);
                }
                else if (pos.FechaDesde.AddMonths(1).Month > pos.FechaHasta.Month)
                {
                    pos.Posicion = new DateTime(pos.FechaHasta.Year, pos.FechaHasta.Month, 1);
                }
            }
            toneladasPorGrano.DispFijac += acuerdos.Where(x => fechaPosicion >= x.Posicion).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            toneladasPorGrano.FrwFijac += acuerdos.Where(x => fechaPosicion < x.Posicion).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum();
            

            toneladasPorGrano.Total = toneladasPorGrano.DispAFijar + toneladasPorGrano.DispAPrecio + toneladasPorGrano.DispFijac + toneladasPorGrano.DispFason +
            toneladasPorGrano.FrwAFijar + toneladasPorGrano.FrwAPrecio + toneladasPorGrano.FrwFijac + toneladasPorGrano.FrwFason +
            toneladasPorGrano.NewAFijar + toneladasPorGrano.NewAPrecio + toneladasPorGrano.NewFijac + toneladasPorGrano.NewFason;

            var agente = contexto.Set<AgenteCompra>().Where(x => fechaHoy==fechaManana && DbFunctions.TruncateTime(x.Fecha) == fechaHoy && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && (centroId == 0 || centroId == 1) && (calidad == null || calidad == false))
                .Select(x => new NegocioToneladasPosicionDto { Id = x.Id, Fecha = x.Fecha, TipoNegocioId = 5, Cantidad = x.Cantidad, PosicionString = x.Posicion }).ToList();
            foreach(var age in agente)
            {
                var pos = age.PosicionString.Split('.');
                var fecha = new DateTime(int.Parse(pos[1]), int.Parse(pos[0]), 1);
                var fechaNewCrop = new DateTime(DateTime.Now.AddYears(1).Year, materialId == 3 ? 4 : materialId == 1 ? 3 : 11, 1);

                if (fecha >= fechaNewCrop)
                {
                    toneladasPorGrano.NewAgente += Math.Ceiling(age.Cantidad / 1000);
                }
                else if ((fecha < fechaNewCrop && (DateTime.Now.Month == fecha.Month || DateTime.Now.AddMonths(1).Month == fecha.Month)) || (fecha < fechaNewCrop && (DateTime.Now.Month == fecha.Month || DateTime.Now.AddMonths(1).Month == fecha.Month)) || (fecha < fechaNewCrop && (DateTime.Now.Month == fecha.Month || DateTime.Now.AddMonths(1).Month == fecha.Month)))
                {
                    toneladasPorGrano.DispAgente += Math.Ceiling(age.Cantidad / 1000);
                }
                else { toneladasPorGrano.FrwAgente += Math.Ceiling(age.Cantidad / 1000); }
            }

            toneladasPorGrano.Total += toneladasPorGrano.NewAgente + toneladasPorGrano.DispAgente + toneladasPorGrano.FrwAgente;

            return toneladasPorGrano;
        }
    }
}
