using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class ConsultaActividadHistoriaTraerPorProveedorId : IConsulta<ActividadTraer>
    {
        private readonly int proveedorId;
        private readonly string detalle;
        private readonly string tipoActividad;
        private readonly bool actual;

        public ConsultaActividadHistoriaTraerPorProveedorId(int proveedorId, bool actual, string detalle = null, string tipoActividad = null)
        {
            this.proveedorId = proveedorId;
            this.detalle = detalle;
            this.tipoActividad = tipoActividad;
            this.actual = actual;
        }

        private static List<ActividadTraer> Query(DbContext contexto, int proveedorId, string detalle, string tipoActividad, bool actual)
        {
            int[] actividad;
            
            var hoy = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(tipoActividad)&&tipoActividad!="0")
            {
                var idactividad = tipoActividad.Split('|');
                var ids = new List<int>();
                foreach (var id in idactividad)
                {
                    ids.Add(int.Parse(id));                    
                }
                actividad = ids.ToArray();
            }
            else
            {
                var ids = from t in contexto.Set<TipoActividad>() select t.TipoActividadId;
                actividad = ids.ToArray();
            }
            if (string.IsNullOrEmpty(detalle))
            {
                detalle = "";
            }
            var resultado = from a in contexto.Set<Actividad>()
                            join ta in contexto.Set<TipoActividad>() on a.TipoActividadId equals ta.TipoActividadId into tas
                            from ta in tas.DefaultIfEmpty()
                            join c in contexto.Set<Comercial>() on a.ComercialId equals c.ComercialId into cs
                            from c in cs.DefaultIfEmpty()
                            join cc in contexto.Set<ContactoComercial>() on a.ContactoComercialId equals cc.ContactoComercialId into ccs
                            from cc in ccs.DefaultIfEmpty()
                            where a.ProveedorId == proveedorId && (actividad.Contains(a.TipoActividadId) && a.Detalle.Contains(detalle)) && actual? a.FechaHoraRecordatorio >= hoy: a.ProveedorId == proveedorId
                            orderby a.FechaHoraActividad
                            select new ActividadTraer
                            {
                                ActividadId = a.ActividadId,
                                Detalle = a.Detalle,
                                FechaHoraRecordatorio = a.FechaHoraRecordatorio,
                                FechaHoraRecordatorioFin = a.FechaHoraRecordatorioFin,
                                asunto = a.asunto,
                                TipoActividad = ta.Descripcion,
                                ComercialId = c.ComercialId,
                                ContactoComercialId = cc.ContactoComercialId,
                                ContactoComercial = cc.Nombres + " " + cc.Apellido
                            };

            return resultado.ToList();
        }

        public virtual List<ActividadTraer> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, proveedorId, detalle, tipoActividad, actual);
            }
        }
    }
}
