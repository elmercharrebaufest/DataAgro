using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ExportAll
    {

        public List<ContactoAll> contacto { get; set; }
        public List<ObjetivoAll> objetivo { get; set; }
        public List<ContactosPrincipalesAll> ContactosPrincipales { get; set; }
        public List<ProduccionAll> produccion { get; set; }
        public List<AlmacenamientoAll> almacenamiento { get; set; }
        public List<AgendaAll> agenda { get; set; }
        public List<ComprasAll> compras { get; set; }

        public ExportAll()
        {
            contacto = new List<ContactoAll>();
            objetivo = new List<ObjetivoAll>();
            ContactosPrincipales = new List<ContactosPrincipalesAll>();
            produccion = new List<ProduccionAll>();
            almacenamiento = new List<AlmacenamientoAll>();
            compras = new List<ComprasAll>();
            
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
        public string Comercial { get; set; }
        public string ClienteMoa { get; set; }
        public string Zona { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Clasificacion { get; set; }
        public string TipoBoleto { get; set; }
        public string Bolsa { get; set; }
        public string Consignatario { get; set; }

        public decimal? ComisionPorcentaje { get; set; }

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
}
