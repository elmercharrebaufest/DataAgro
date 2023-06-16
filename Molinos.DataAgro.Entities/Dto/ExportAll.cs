using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ExportAll
    {
        public List<ContactoAll> Contacto { get; set; }
        public List<ObjetivoAll> Objetivo { get; set; }
        public List<ContactosPrincipalesAll> ContactosPrincipales { get; set; }
        public List<ProduccionAll> Produccion { get; set; }
        public List<AlmacenamientoAll> Almacenamiento { get; set; }
        public List<CampoDetalleAll> Establecimiento { get; set; }
        public List<AgendaAll> Agenda { get; set; }
        public List<ComprasAll> Compras { get; set; }
        public List<CompraCampanaActualDto> CompraCampanaActual { get; set; }
        public List<CampanaMaterialDetallePorMeseExcelDto> Situacion { get; set; }
        public List<CapacidadProductivaAll> CapacidadProductiva { get; set; }
        public List<ActividadComercial> ActividadComercial { get; set; }
        public List<ObjetivoCampania> ObjetivoCampania { get; set; }

        public ExportAll()
        {
            Contacto = new List<ContactoAll>();
            Objetivo = new List<ObjetivoAll>();
            ContactosPrincipales = new List<ContactosPrincipalesAll>();
            Produccion = new List<ProduccionAll>();
            Almacenamiento = new List<AlmacenamientoAll>();
            Establecimiento = new List<CampoDetalleAll>();
            Compras = new List<ComprasAll>();
            CapacidadProductiva = new List<CapacidadProductivaAll>();
            ActividadComercial = new List<ActividadComercial>();
            ObjetivoCampania = new List<ObjetivoCampania>();
        }
    }

    public class ContactoAll
    {
       
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Estado { get; set; }
        public int? Calificacion { get; set; }
        public string Segmentacion { get; set; }
        public string Domicilio { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string CodPostal { get; set; }
        public string CanalDeOperacion { get; set; }
        public string Destinatario { get; set; }
        public string Condicion { get; set; }
        public string Intermediario { get; set; }
        public string AreaDeInfluencia { get; set; }
        public string Comentario { get; set; }
        public string ComercialACargo { get; set; }
        public string ClienteMoa { get; set; }
        public string Zona { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Clasificacion { get; set; }
        public string TipoBoleto { get; set; }
        public string Bolsa { get; set; }
        public string Consignatario { get; set; }

        public decimal? ComisionPorcentaje { get; set; }
        public string LocalidadCompraNet { get; set; }
        public string ProvinciaCompraNet { get; set; }
        public string Deshabilitado { get; set; }
        public string Comisionista { get; set; }
        public string CuposConRiesgo { get; set; }
        public string OperaConMATBA { get; set; }
        public string OperaAtravesDe { get; set; }

        public int ProveedorId { get; set; }
    }

    public class ObjetivoAll
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string Tonelada { get; set; }
    }

    public class ContactosPrincipalesAll
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string FechaNacimiento { get; set; }
        public string Profesion { get; set; }
        public string Puesto { get; set; }
        public string Interes { get; set; }
        public string OtrosIntereses { get; set; }
        public string Principal { get; set; }
        public string PrincipalCupo { get; set; }
        public string CompraNet { get; set; }
        public string Cupo { get; set; }
        public string Boleto { get; set; }
        //public string OperaATravesDe { get; set; }
        //public string Comisionista { get; set; }
        //public string HabilitadoCuposConRiesgo { get; set; }
        //public string OperaConMatba { get; set; }
    }

    public class ProduccionAll
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string Hectareas { get; set; }
        public string Toneladas { get; set; }
        public string Alquiladas { get; set; }
        public string Propias { get; set; }
    }

    public class AlmacenamientoAll
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string CampañaPlanta { get; set; }
        public string CapacidadPlantaTn { get; set; }
        public string Alquiladas { get; set; }
        public string Propias { get; set; }
        public string Campaña { get; set; }
        public string Material { get; set; }
        public string Toneladas { get; set; }
        public string VolumenAnualTn { get; set; }
        public string HabilitadoSojaSustentable { get; set; }
    }

    public class AgendaAll
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string TipoActividad { get; set; }
        public string DetalleContacto { get; set; }
        public string Asunto { get; set; }
        public string FechaDesde { get; set; }
        public string HoraDesde { get; set; }
        public string FechaHasta { get; set; }
        public string HoraHasta { get; set; }
    }

    public class ComprasAll
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Campaña { get; set; }
        public string Material { get; set; }
        public string Toneladas { get; set; }
        public string Mes { get; set; }
        public string Año { get; set; }

    }

    public class CampoDetalleAll
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Nombre { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Partido { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Material { get; set; }
        public decimal? HectareasCultivables { get; set; }
        public decimal? HectareasTotales { get; set; }
        public decimal? Rinde { get; set; }
        public string Comercial { get; set; }
        public int? ImportId { get; set; }
        public string Campaña { get; set; }
    }

    public class CapacidadProductivaAll
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Material { get; set; }
        public string Campania { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal Porcentaje { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }

    public class ObjetivoCampania
    {
        public string Comercial { get; set; }
        public string Material { get; set; }
        public string ToneladasObjetivo { get; set; }
        public string ToneladasCompradas { get; set; }
        public string PorcentajeDeCumplimiento { get; set; }
        public string Campania { get; set; }
    }
}
