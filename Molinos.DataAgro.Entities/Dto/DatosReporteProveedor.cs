using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosProveedor
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
        public IQueryable<string> Comerciales { get; set; }
        public string Comercial { get; set; }
        public string ClienteMoa { get; set; }
        public string Zona { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string Clasificacion { get; set; }
        public string TipoBoleto { get; set; }
        public string Bolsa { get; set; }
        public string Consignatario { get; set; }

        public decimal? ComisionPorcentaje { get; set; }
        public string LocalidadCompraNet { get; set; }
        public string ProvinciaCompraNet { get; set; }
    }

    public class DatosContactoProveedor
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Profesion { get; set; }
        public string Puesto { get; set; }
        public string Interes { get; set; }
        public string OtrosIntereses { get; set; }
        public string Principal { get; set; }
        public string PrincipalCupo { get; set; }
    }

    public class DatosProduccionProveedor
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public int? Hectareas { get; set; }
        public int? Toneladas { get; set; }
        public string Alquiladas { get; set; }
        public string Propias { get; set; }
    }

    public class DatosAlmacenamientoProveedor 
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string CampañaPlanta { get; set; }
        public decimal? CapacidadPlantaTn { get; set; }
        public string Alquiladas { get; set; }
        public string Propias { get; set; }
        public string Campaña { get; set; }
        public string Material { get; set; }
        public double? Toneladas { get; set; }
        public double? VolumenAnualTn { get; set; }
        public string HabilitadoSojaSustentable { get; set; }
    }
}
