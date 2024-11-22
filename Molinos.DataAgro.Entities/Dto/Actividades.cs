using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ActividadRecordatorio
    {
        public int ActividadId { get; set; }
        public string Tema { get; set; }
        public string Comentarios { get; set; }
        public string Dia { get; set; }
        public string Hora { get; set; }
        public string Contacto { get; set; }
        public int ProveedorId { get; set; }
    }

    public class ActividadRecordatorioGrid
    {
        public int ActividadId { get; set; }
        public string Tema { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaRecordatorio { get; set; }
        public string Contacto { get; set; }
        public int ProveedorId { get; set; }
    }

    public class ActividadInsertarIni
    {
        public int TipoActividad { get; set; }
        public string Detalle { get; set; }
        public DateTime? FechaYHoraRecordatorio { get; set; }
        public DateTime? FechaYHoraRecordatorioFin { get; set; }
        public string Asunto { get; set; }
        public DateTime FechaYHoraActividad { get; set; }
        public int? Contacto { get; set; }
        public int Enviar { get; set; }
        public int ActividadId { get; set; }
        public int ComercialId { get; set; }
        public int ProveedorId { get; set; }
        public string UserName { get; set; }

        public ActividadInsertarIni()
        {
            FechaYHoraActividad = DateTime.Now;
            FechaYHoraRecordatorio = null;
            TipoActividad = 0;
            Detalle = "";
            Contacto = null;
            Enviar = 0;
            ActividadId = 0;
            ComercialId = 0;
            ProveedorId = 0;
            UserName = string.Empty;
        }
    }

    public class HistorialActividad
    {
        public string TipoActividad { get; set; }
        public string Detalle { get; set; }
        public DateTime? FechaYHoraRecordatorio { get; set; }
        public DateTime FechaYHoraActividad { get; set; }
        public int? Contacto { get; set; }
        public int ActividadId { get; set; }
        public int ProveedorId { get; set; }
        public int CantidadRegistros { get; set; }

        public HistorialActividad()
        {
            FechaYHoraActividad = DateTime.Now;
            FechaYHoraRecordatorio = null;
            TipoActividad = "";
            Detalle = "";
            Contacto = null;
            ActividadId = 0;
            ProveedorId = 0;
            CantidadRegistros = 0;
        }
    }

    public class ActividadExportar
    {
        public int ActividadId { get; set; }
        public int TipoActividadId { get; set; }
        public string TipoActividad { get; set; }
        public string Detalle { get; set; }
        public int ProveedorId { get; set; }
        public string Proveedor { get; set; }
        public DateTime FechaHoraActividad { get; set; }
        public DateTime? FechaHoraRecordatorio { get; set; }
        public int? ComercialId { get; set; }
        public string Comercial { get; set; }
        public int? ContactoComercialId { get; set; }
        public string ContactoComercial { get; set; }
        public DateTime? FechaHoraRecordatorioFin { get; set; }
        public string Asunto { get; set; }
    }

    public class ActividadExportarExcel
    {
        public int ActividadId { get; set; }
        public string TipoActividad { get; set; }
        public string Detalle { get; set; }
        public DateTime FechaHoraActividad { get; set; }
        public DateTime? FechaHoraRecordatorio { get; set; }
        public string Comercial { get; set; }
        public string ContactoComercial { get; set; }
        public DateTime? FechaHoraRecordatorioFin { get; set; }
        public string Asunto { get; set; }
    }

}