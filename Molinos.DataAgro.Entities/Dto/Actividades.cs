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

    public class ActividadInsetarIni
    {
        public int tipoactividad { get; set; }
        public string detalle { get; set; }
        public DateTime? fechaYHoraRecordatorio { get; set; }
        public DateTime? fechaYHoraRecordatorioFin { get; set; }
        public string asunto { get; set; }
        public DateTime fechaYHoraActividad { get; set; }
        public int? contacto { get; set; }
        public int enviar { get; set; }
        public int ActividadId { get; set; }
        public int ComercialId { get; set; }
        public int ProveedorId { get; set; }
        public string UserName { get; set; }

        public ActividadInsetarIni()
        {
            this.fechaYHoraActividad = DateTime.Now;
            this.fechaYHoraRecordatorio = null;
            this.tipoactividad = 0;
            this.detalle = "";
            this.contacto = null;
            this.enviar = 0;
            this.ActividadId = 0;
            this.ComercialId = 0;
            this.ProveedorId = 0;
            this.UserName = string.Empty;
        }
    }

    public class HistorialActiviad
    {
        public string tipoactividad { get; set; }
        public string detalle { get; set; }
        public DateTime? fechaYHoraRecordatorio { get; set; }
        public DateTime fechaYHoraActividad { get; set; }
        public int? contacto { get; set; }
        public int ActividadId { get; set; }
        public int ProveedorId { get; set; }
        public int cantidadRegistros { get; set; }

        public HistorialActiviad()
        {
            this.fechaYHoraActividad = DateTime.Now;
            this.fechaYHoraRecordatorio = null;
            this.tipoactividad = "";
            this.detalle = "";
            this.contacto = null;
            this.ActividadId = 0;
            this.ProveedorId = 0;
            this.cantidadRegistros = 0;
        }
    }

    public class ActividadExportar
    {
        public int ActividadId { get; set; }
        public int TipoActividadId { get; set; }
        public string tipoActividad { get; set; }
        public string Detalle { get; set; }
        public int ProveedorId { get; set; }
        public string proveedor { get; set; }
        public DateTime FechaHoraActividad { get; set; }
        public DateTime? FechaHoraRecordatorio { get; set; }
        public int? ComercialId { get; set; }
        public string comercial { get; set; }
        public int? ContactoComercialId { get; set; }
        public string contactoComercial { get; set; }
        public DateTime? FechaHoraRecordatorioFin { get; set; }
        public string asunto { get; set; }
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
