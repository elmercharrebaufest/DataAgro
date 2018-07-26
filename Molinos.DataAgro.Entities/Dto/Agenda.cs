using Mastersoft.Framework.Standard;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAgendaActividad
    {
        public List<TipoActividadCombo> TiposActividades { get; set; }
        public List<ProveedorCombo> Proveedores { get; set; }


    }
    public class DatosIniAgendaActividadModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniAgendaActividad Datos { get; set; }

        public DatosIniAgendaActividadModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniAgendaActividad();
        }
    }

    public class RptActividadAgenda
    {
        public string ActividadDetalle { get; set; }
        public string TipoActividad { get; set; }
        public string Proveedor { get; set; }
        public string ContactoComercial { get; set; }
        public DateTime? FechaRecordatorio { get; set; }
        public DateTime FechaActividad { get; set; }
        public string Comercial { get; set; }
    }

    public class AgendaStore
    {
        public string RazonSocial { get; set; }
        public string Detalle { get; set; }
        public string TipoDeAcividad { get; set; }
        public DateTime FechaHoraActividad { get; set; }
        public DateTime? FechaHoraRecordatorio { get; set; }
        public string Apellido { get; set; }
        public string NombreContacto { get; set; }
        public int ActividadId { get; set; }


    }

    public class RptActividadAgendaParam
    {
        public string ActividadDetalle { get; set; }
        public int? TipoActividad { get; set; }
        public int? ProveedorId { get; set; }
        public string ActiveDirectoryId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

    }

}
