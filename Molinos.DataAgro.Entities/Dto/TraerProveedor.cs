using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{

    public class ActividadTraer
    {
        public int ActividadId { get; set; }
        public string Detalle { get; set; }
        public DateTime FechaHoraActividad { get; set; }
        public DateTime? FechaHoraRecordatorio { get; set; }
        public string TipoActividad { get; set; }
        public int ComercialId { get; set; }
        public int? ContactoComercialId { get; set; }
        public string ContactoComercial { get; set; }
        public int? ProveedorId { get; set; }
        public DateTime? FechaHoraRecordatorioFin { get; set; }
        public string asunto { get; set; }
    }

    public class HistorialTraer
    {
        public int ActividadId { get; set; }
        public string Detalle { get; set; }
        public DateTime FechaHoraActividad { get; set; }
        public DateTime? FechaHoraRecordatorio { get; set; }
        public DateTime? FechaHoraRecordatorioFin { get; set; }
        public string TipoActividad { get; set; }
        public string ContactoComercial { get; set; }
        public int? ContactoComercialId { get; set; }
        public int ComercialId { get; set; }
        public string Asunto { get; set; }
    }

    public class ActividadTraerPorProveedor
    {
        public ActividadTraer ActividadesTraer { get; set; }
    }


    public class BasicoProveedor
    {
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string Estado { get; set; }
        public int Facacop { get; set; }
        public string RiesgoComercialSap { get; set; }
        public int EstadoCuit { get; set; }
        public string Segmentacion { get; set; }
        public string GrupoSegmentacion { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Email4 { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string Telefono4 { get; set; }
        public string Observaciones { get; set; }
        public Nullable<DateTime> FechaUltimoContacto { get; set; }
        public string Nombres { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string CanalOperacion { get; set; }
        public string Destinatario { get; set; }
        public string Condicion { get; set; }
        public string AreaInfluencia { get; set; }
        public string Intermediario { get; set; }
        public Nullable<int> Calificacion { get; set; }
        public string GrupoDeCompras { get; set; }
        public string CodigoPostal { get; set; }
        public bool? NoOperable { get; set; }
        public bool? Operando { get; set; }
        public string TooltipNoOperable { get; set; }
        public Nullable<int> TipoTelefono1Id { get; set; }
        public Nullable<int> TipoTelefono2Id { get; set; }
        public Nullable<int> TipoTelefono3Id { get; set; }
        public Nullable<int> TipoTelefono4Id { get; set; }
        public string NombreReferente { get; set; }
        public Nullable<int> ProveedorId { get; set; }
        public Nullable<bool> ClienteMOA { get; set; }
        public string ProvinciaCompraNet { get; set; }
        public string LocalidadCompraNet { get; set; }
        public string ClasificacionCompraNet { get; set; }
        public string BoletoCompraNet { get; set; }
        public string BolsaCompraNet { get; set; }
        public bool? Consignatario { get; set; }
        public decimal? Comision { get; set; }
        public bool? PlanCanje { get; set; }
        public bool? Deshabilitado { get; set; }
        public string Alias { get; set; }
        public string RazonSocialComisionista { get; set; }
        public bool? Comisionista { get; set; }
        public int? ComisionistaId { get; set; }
        public bool? CuposConRiesgo { get; set; }
        public bool? OperaConMATBA { get; set; }

    }

    public class BasicoProveedorTraerPorProveedor
    {
        public BasicoProveedor BasicosProveedor { get; set; }
    }

    public class ContactosComerciales
    {
        public string Nombres { get; set; }
        public string Apellido { get; set; }
        public string Puesto { get; set; }
        public string Cargo { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string InteresId { get; set; }
        public string Interes { get; set; }
        public string OtrosIntereses { get; set; }
        public int? TipoTelefono1Id { get; set; }
        public Nullable<int> TipoTelefono2Id { get; set; }
        public Nullable<int> TipoTelefono3Id { get; set; }
        public int ContactoComercialId { get; set; }
        public bool? EsPrincipal { get; set; }
        public bool? CompraNet { get; set; }
        public bool? Cupo { get; set; }
        public bool? Boleto { get; set; }
        public Nullable<int> ProveedorId { get; set; }
    }

    public class ContactosComercialesTraerPorProveedor
    {
        public ContactosComerciales ContactoComerciales { get; set; }
    }

    public class AcopioMaterialPorProveedor
    {
        public Nullable<int> AcopioId { get; set; }
        public int AcopioMaterialId { get; set; }
        public int CampañaId { get; set; }
        public string Campaña { get; set; }
        public int MaterialId { get; set; }
        public int NroItem { get; set; }
        public Nullable<double> Toneladas { get; set; }
        public string Material { get; set; }
        public int LocalidadId { get; set; }
        public string Localidad { get; set; }
        public int ProvinciaId { get; set; }
        public string Provincia { get; set; }
    }

    public class CampoProduccionAcopio
    {

        public string Provincia { get; set; }
        public int ProvinciaId { get; set; }
        public string Localidad { get; set; }
        public int? LocalidadId { get; set; }
        public bool? ArrendadoPropio { get; set; }
        public int? MaterialId { get; set; }
        public string Material { get; set; }
        public double? HectareasPorcentaje { get; set; }
        public double? Toneladas { get; set; }
        public int? CampañaId { get; set; }
        public string Campaña { get; set; }
        public Nullable<int> ProveedorId { get; set; }
        //public string Coordenadas { get; set; }
        public string KMZnombre { get; set; }
        public string KMZfile { get; set; }

        public Nullable<double> AlmacVolAnualTotal { get; set; }
        //public Nullable<double> AlmacCapacidadPropia { get; set; }
        public Nullable<bool> AlmacHabilitadoSojaSust { get; set; }
        public Nullable<double> AlmacTonsMaxSojaSust { get; set; }
        public Nullable<double> AlmacHectSojaSust { get; set; }

        public bool? EsCampoProduccion { get; set; }
        public int Id { get; set; }
        public Nullable<bool> HasArrendadas { get; set; }

        //public string Latitud { get; set; }
        //public string Longitud { get; set; }
        //public string Nombre { get; set; }
        //public int? ComercialId { get; set; }
        //public string Comercial { get; set; }
        public string Partido { get; set; }
    }

    public class CampoProduccionAcopioPorProveedor
    {
        public CampoProduccionAcopio CampoProduccionAcopios { get; set; }
    }

    public class StoredHistorialResult
    {
        public List<ActividadTraer> ActividadHistoriaTraerPorProveedores { get; set; }
    }

    public class ObjetivosTraer
    {
        public int ObjetivoId { get; set; }
        public int? CampañaId { get; set; }
        public string Campaña { get; set; }
        public int? MaterialId { get; set; }
        public string Material { get; set; }
        public int? NroItem { get; set; }
        public int? ProveedorId { get; set; }
        public int? ComercialId { get; set; }
        public int? GrupoDeComprasId { get; set; }
        public Nullable<double> ToneladasObjetivos { get; set; }
    }

    public class StoredPorProveedorResult
    {
        public List<ActividadTraer> ActividadTraerPorProveedores { get; set; }
        public List<BasicoProveedor> BasicoProveedorTraerPorProveedores { get; set; }
        public List<ContactosComerciales> ContactosComercialesTraerPorProveedores { get; set; }
        public List<CampoProduccionAcopio> CampoProduccionAcopioPorProveedores { get; set; }
        public List<CampoProduccionAcopio> Acopio { get; set; }
        public List<AcopioMaterialPorProveedor> AcopioMaterialPorProveedores { get; set; }
        public List<ActividadTraer> ActividadHistoriaTraerPorProveedores { get; set; }
        public DatosContacto DatosContacto { get; set; }
        public Historial Historial { get; set; }
        public List<CanalOperacion> CanalesDeOperacion { get; set; }
        public List<Destinatario> ProveedorDestinatario { get; set; }
        public List<Condicion> ProveedorCondicion { get; set; }
        public List<CampoDetalleDto> ProveedorCampoDetalle { get; set; }
        public List<ObjetivosTraer> ObjetivosTraerPorProveedorId { get; set; }

        public List<ProveedorCorredorDto> ProveedorCorredor { get; set; }
        public StoredPorProveedorResult()
        {

        }
        public List<CompraDto> CompraDetalle { get; set; }

        public List<String> Material { get; set; }
        public List<String> Campanias { get; set; }
    }

    public class RptProveedorInfo
    {
        //Datos Contacto
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Intermediario { get; set; }

        //Basico Proveedor
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string Estado { get; set; }
        public string Segmentacion { get; set; }
        public string Email1P { get; set; }
        public string Telefono1P { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string CanalOperacion { get; set; }
        public string Destinatario { get; set; }
        public string Condicion { get; set; }
        public string AreaInfluencia { get; set; }
        public string Calificacion { get; set; }
        public string NombreReferente { get; set; }
        public string Comentarios { get; set; }

    }

    public class RptContactosInfo
    {
        //Contactos Comerciales
        public int EliminaDetalle { get; set; }
        public string TieneContactos { get; set; }
        public string Nombres { get; set; }
        public string Apellido { get; set; }
        public string Puesto { get; set; }
        public string Cargo { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string InteresId { get; set; }
        public string Interes { get; set; }
        public string OtrosIntereses { get; set; }
        public int TipoTelefono1Id { get; set; }
        public Nullable<int> TipoTelefono2Id { get; set; }
        public Nullable<int> TipoTelefono3Id { get; set; }
        public int ContactoComercialId { get; set; }
        public string EsPrincipal { get; set; }
        public Nullable<int> ProveedorId { get; set; }
    }

    public class RptProduccionInfo
    {
        //Produccion
        public string ProvinciaProd { get; set; }
        public string LocalidadProd { get; set; }
        public string ArrendadoPropio { get; set; }
        public string Material { get; set; }
        public double HectareasPorcentaje { get; set; }
        public double? Toneladas { get; set; }
        public string Campaña { get; set; }
        public Nullable<double> AlmacVolAnualTotal { get; set; }
        //public Nullable<double> AlmacCapacidadPropia { get; set; }
        public Nullable<bool> AlmacHabilitadoSojaSust { get; set; }
        public Nullable<double> AlmacTonsMaxSojaSust { get; set; }
        public Nullable<double> AlmacHectSojaSust { get; set; }
        public string EsCampoProduccion { get; set; }
        public Nullable<bool> HasArrendadas { get; set; }
        public string TieneProduccion { get; set; }
    }

    public class RptAlmacenamientoInfo
    {
        //Almacenamiento

        public string ProvinciaAlm { get; set; }
        public string LocalidadAlm { get; set; }
        public string ArrendadoPropioAlm { get; set; }
        public string MaterialAlm { get; set; }
        public double HectareasPorcentajeAlm { get; set; }
        public double? ToneladasAlm { get; set; }
        public int CampañaId { get; set; }
        public string CampañaAlm { get; set; }
        public Nullable<double> AlmacVolAnualTotalAlm { get; set; }
        //public Nullable<double> AlmacCapacidadPropia { get; set; }
        public Nullable<bool> AlmacHabilitadoSojaSustAlm { get; set; }
        public Nullable<double> AlmacTonsMaxSojaSustAlm { get; set; }
        public Nullable<double> AlmacHectSojaSustAlm { get; set; }
        public string EsCampoProduccionAlm { get; set; }
        public Nullable<bool> HasArrendadasAlm { get; set; }
        public string TieneAlmacenamiento { get; set; }
    }
    //Objetivos

    public class RptObjetivosInfo
    {
        public string MaterialObjetivo { get; set; }
        public double? ToneladaObjetivo { get; set; }
        public string TieneObjetivos { get; set; }
    }


    public class DatosContacto
    {
        //public string Provincia { get; set; }
        //public string Localidad { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        //public int CanalOperacionId { get; set; }
        //public int DestinatarioId { get; set; }
        //public List<int> CondicionesPreferentesId { get; set; }
        //public int AreaInfluenciaId { get; set; }
        public string Intermediario { get; set; }
        //public string Comentarios { get; set; }

        public DatosContacto()
        {

        }

    }
    public class DatosLocalidadProveedor
    {
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public int LocalidadId { get; set; }
        public int ProvinciaId { get; set; }
        public string CUIT { get; set; }
        public bool Consignatario { get; set; }
    }

    public class DatosLocalidadProvinciaFiltro
    {

        public string CUIT { get; set; }
        public int MaterialId { get; set; }
        public int CampanaId { get; set; }

        public DatosLocalidadProvinciaFiltro()
        {
            this.CUIT = "";
            this.MaterialId = 0;
            this.CampanaId = 0;
        }
    }

    public class DatosLocalidadProvincia
    {

        public int ProveedorId { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public int LocalidadId { get; set; }
        public int ProvinciaId { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public int ClasificacionId { get; set; }
        public bool Consignatario { get; set; }
        public int BoletoId { get; set; }
        public int BolsaId { get; set; }
        public bool Corredor { get; set; }
        public DatosLocalidadProvincia()
        {
            this.ProveedorId = 0;
            this.CUIT = "";
            this.RazonSocial = "";
            this.LocalidadId = 0;
            this.ProvinciaId = 0;
            this.Localidad = "";
            this.Provincia = "";
            this.ClasificacionId = 0;
            this.Consignatario = false;
            this.BoletoId = 0;
            this.BolsaId = 0;
        }
    }

    public class MsHistorial
    {
        public String Campaña { get; set; }
        public String Grano { get; set; }
        public int Mes { get; set; }
        public double Cantidad { get; set; }
    }


    public class CampañaHistorial
    {
        public String Nombre { get; set; }
        public List<CampañaPorMes> Campañas { get; set; }

        public CampañaHistorial()
        {
            Campañas = new List<CampañaPorMes>();
        }
    }

    public class GranoHistorial
    {
        public String Nombre { get; set; }
        public List<CampañaHistorial> campañas { get; set; }

        public GranoHistorial()
        {
            campañas = new List<CampañaHistorial>();
        }
    }

    public class CampañaPorMes
    {
        public int Mes { get; set; }
        public int Año { get; set; }
        public double Total { get; set; }
    }


    public class Historial
    {
        public List<GranoHistorial> HistorialGrano { get; set; }
        public List<campañaTotal> camp { get; set; }

        public Historial()
        {
            HistorialGrano = new List<GranoHistorial>();
            camp = new List<campañaTotal>();
        }
    }

    public class campañaTotal
    {
        public String Nombre { get; set; }
        public List<GranoTotales> grano { get; set; }
        public campañaTotal()
        {
            grano = new List<GranoTotales>();
        }

    }

    public class GranoTotales
    {
        public String Grano { get; set; }
        public String Total { get; set; }
    }
}
