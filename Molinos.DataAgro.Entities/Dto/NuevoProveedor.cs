using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class NuevoProveedor
    {
        public Basico basicos { get; set; }
        public Contacto contacto { get; set; }
        public Produccion produccion { get; set; }
        public Almacenamiento almacenamiento { get; set; }
        public List<ContactosComercial> contactocomercial { get; set; }
        public List<CampoDetalleDto> establecimiento { get; set; }
        public int? ProveedorId { get; set; }
        public int? ProveedorCorredorId { get; set; }
        public int? CampañaId { get; set; }
    }

    public class Basico
    {
        public string RazonSocial { get; set; }
        public string cuit { get; set; }
        public int nocliente { get; set; }
        public int segmentacion { get; set; }
        public List<string> emails { get; set; }
        public List<Telefono> telefonos { get; set; }
        public string comentario { get; set; }
        public string nomReferente { get; set; }
        public int? calificacion { get; set; }
        public int? ProvinciaCompraNet { get; set; }
        public int? LocalidadCompraNet { get; set; }
        public int? ClasificacionCompraNet { get; set; }
        public int? BoletoCompraNet { get; set; }
        public int? BolsaCompraNet { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public decimal? Comision { get; set; }
        public bool? Deshabilitado { get; set; }
        public string Alias { get; set; }
        public bool? comisionista { get; set; }
        public int? comisionistaId { get; set; }
        public bool? CuposConRiesgo { get; set; }
    }

    public class Telefono
    {
        public int? tipoTelefono { get; set; }
        public string telefono { get; set; }
    }

    public class Contacto
    {
        public string areaDeInfluencia { get; set; }
        public List<int> canalesOperacion { get; set; }
        public string direccion { get; set; }
        public string codpost { get; set; }
        public string comentarioContacto { get; set; }
        public List<int> entregaA { get; set; }
        public string intermediario { get; set; }
        public int? localidad { get; set; }
        public int? provincia { get; set; }
        public List<int> condPreferentes { get; set; }
    }

    public class Produccion
    {
        public List<CamposProduccion> CamposProduccion { get; set; }
        public double? TonsMaxAprobSojaSust { get; set; }
        public string habilitaoSojaSust { get; set; }
        public double? hasAprobSojaSust { get; set; }
        public double? volumenAnualTotalTns { get; set; }
        public List<Objetivos> objetivos { get; set; }
        public List<Objetivos> eliminarobjetivos { get; set; }
        public int? CampoId { get; set; }

        public Produccion()
        {
            eliminarobjetivos = new List<Objetivos>();
        }
    }

    public class CamposProduccion
    {
        public string archivo { get; set; }
        public byte[] archivofile { get; set; }
        public string archivoFileResult { get; set; }
        public List<Granos> granos { get; set; }
        public List<Granos> eliminarproduccion { get; set; }
        public int item { get; set; }
        public int localidad { get; set; }
        public string localidadNom { get; set; }
        public int provincia { get; set; }
        public string provinciaNom { get; set; }
        public int? hectareas { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string nombre { get; set; }
        public int? comercialId { get; set; }
        public int? CampoId { get; set; }
    }

    public class Granos
    {
        public string grano { get; set; }
        public int granoId { get; set; }
        public int? hectareas { get; set; }
        public string hectareasNom { get; set; }
        public int? toneladas { get; set; }
        public int campañaId { get; set; }
    }

    public class Almacenamiento
    {
        public List<CamposAlmacenamiento> CamposAlmacenamiento { get; set; }
    }

    public class CamposAlmacenamiento
    {
        public int? CampoId { get; set; }
        public string archivo { get; set; }
        public byte[] archivofile { get; set; }
        public string archivoFileResult { get; set; }
        public List<GranosAlmacenamiento> granosAlmacenamiento { get; set; }
        public List<GranosAlmacenamiento> eliminargranoalmacenamiento { get; set; }
        public List<GranosAlmacenamientoGrano> granosAlmacenamientoGrano { get; set; }
        public List<GranosAlmacenamientoGrano> eliminargranoalmacenamientograno { get; set; }
        public int item { get; set; }
        public int localidad { get; set; }
        public string localidadNom { get; set; }
        public int provincia { get; set; }
        public string provinciaNom { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string nombre { get; set; }
        public int? comercialId { get; set; }
        //public int hectareasAlmacenamiento { get; set; }
    }

    public class GranosAlmacenamiento
    {

        public int campañaId { get; set; }
        public decimal? toneladasAlmacenamiento { get; set; }
        public bool? hasArrendadas { get; set; }
    }

    public class GranosAlmacenamientoGrano
    {
        public int granoId { get; set; }
        public int campañaId { get; set; }
        public double? toneladasAlmacenamiento { get; set; }
    }

    public class ContactosComercial
    {
        public int? contactoComercialId { get; set; }
        public string apellido { get; set; }
        public string cargo { get; set; }
        public List<string> emails { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public List<int> intereses { get; set; }
        public int? item { get; set; }
        public string nombre { get; set; }
        public string otrosIntereses { get; set; }
        public bool? principal { get; set; }
        public string puesto { get; set; }
        public List<Telefono> telefonos { get; set; }
        public bool CompraNet { get; set; }
        public bool Cupo { get; set; }
        public bool Boleto { get; set; }
    }

    public class Objetivos
    {
        public int granoId { get; set; }
        public string grano { get; set; }
        public int campañaId { get; set; }
        public string campaña { get; set; }
        public string toneladasObjetivo { get; set; }
    }


    public class CampoDetalleDto
    {
        public string partido;
        public int proveedorId;

        public string archivo { get; set; }
        public byte[] archivofile { get; set; }
        public string archivoFileResult { get; set; }
        public int item { get; set; }
        public int localidadId { get; set; }
        public string localidad { get; set; }
        public int provinciaId { get; set; }
        public string provincia { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string nombre { get; set; }
        public int? comercialId { get; set; }
        public string comercial { get; set; }
        public int? CampoId { get; set; }
        public decimal? rinde { get; set; }
        public decimal? htotales { get; set; }
        public decimal? hcultivables { get; set; }
        public int materialId { get; set; }
        public string material { get; set; }
        public int? ImportId  { get; set; }
        public string campaña { get; set; }
        public int? campañaId { get; set; }

    }


}
