using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerPosicionMaterial : IConsulta<PosicionKilos>
    {
        private readonly int materialId;
        private readonly bool? calidad;
        private readonly IRepositorio repositorio;
        private readonly DateTime fecha;
        
        public TraerPosicionMaterial(int materialId,DateTime fecha, IRepositorio repositorio, bool? calidad = null)
        {
            this.materialId = materialId;
            this.calidad = calidad;
            this.repositorio = repositorio;
            this.fecha = fecha;
        }
        
        private static List<PosicionKilos> Query(DbContext contexto, int materialId, DateTime fecha,IRepositorio repositorio, bool? calidad)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fecha.Date;
            var posicionKilos = new List<PosicionKilos>();
            var contratos = repositorio.Listar<Contrato>(x => DbFunctions.TruncateTime(x.Fecha) == fechaHoy 
            && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) 
            && x.MaterialId == materialId 
            && (calidad == null || (calidad != null && x.TrigoEspecial == calidad)));
            foreach (var cont in contratos)
            {
                var posKil = new PosicionKilos();

                if ((DateTime.DaysInMonth(cont.FechaDesde.Year, cont.FechaDesde.Month) - cont.FechaDesde.Day) >= 10)
                {
                    posKil.Kilos = cont.Cantidad / 1000;
                    posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
                }
                else if (cont.FechaDesde.Month + 1 < cont.FechaHasta.Month)
                {
                    cont.FechaDesde = cont.FechaDesde.AddMonths(1);
                    posKil.Kilos = cont.Cantidad / 1000;
                    posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
                }
                else
                {
                    posKil.Kilos = cont.Cantidad / 1000;
                    posKil.Mes = (EnumMeses)cont.FechaHasta.Month;
                }

                if (!posicionKilos.Exists(x => x.Mes == posKil.Mes))
                {
                    posicionKilos.Add(posKil);
                }
                else
                {
                    posicionKilos.Sum(x => x.Kilos + posKil.Kilos);
                }
            }

            var fijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => DbFunctions.TruncateTime(x.Fecha) == fechaHoy
            && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
            && x.MaterialId == materialId
            && (calidad == null || (calidad != null && x.TrigoEspecial == calidad)));
            foreach (var fij in fijaciones)
            {
                var posKil = new PosicionKilos();

                if ((DateTime.DaysInMonth(fij.FechaDesde.Year, fij.FechaDesde.Month)- fij.FechaDesde.Day) >= 10)
                {
                    posKil.Kilos = fij.Cantidad / 1000;
                    posKil.Mes = (EnumMeses)fij.FechaDesde.Month;
                }
                else if (fij.FechaDesde.Month + 1 < fij.FechaHasta.Month)
                {
                    fij.FechaDesde = fij.FechaDesde.AddMonths(1);
                    posKil.Kilos = fij.Cantidad / 1000;
                    posKil.Mes = (EnumMeses)fij.FechaDesde.Month;
                }
                else
                {
                    posKil.Kilos = fij.Cantidad / 1000;
                    posKil.Mes = (EnumMeses)fij.FechaHasta.Month;
                }

                if (!posicionKilos.Exists(x=>x.Mes == posKil.Mes))
                {
                    posicionKilos.Add(posKil);
                }
                else
                {
                    posicionKilos.Sum(x => x.Kilos + posKil.Kilos);
                }
            }
            return posicionKilos;
        }

        public virtual List<PosicionKilos> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, materialId, fecha,repositorio, calidad );
            }
        }
    }
}
