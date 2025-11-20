using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    //---------------------------------------------------------
    // Clases para proyectar resultados de combos,
    // cuando la entidad original tiene muchos campos
    //---------------------------------------------------------

    public class LocalidadCombo 
    {
        public int LocalidadId { get; set; }
        public string Nombre { get; set; }
    }

    public class MaterialCombo 
    {
        public int MaterialId { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public string Campaña { get; set; }
        public decimal? IVA { get; set; }
    }

    public class MotivoCombo
    {
        public int MotivoId { get; set; }
        public string Descripcion { get; set; }
    }

    public class NivelTarifaCombo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSap { get; set; }
    }

    public class CampaniaCombo
    {
        public int CampaniaId { get; set; }
        public string Descripcion { get; set; }
    }

    public class CampaniaTableroCombo
    {
        public int CampaniaTableroId { get; set; }
        public string Descripcion { get; set; }
    }

    public class ComercialCombo 
    {
        public int ComercialId { get; set; }
        public string Apellido { get; set; }
    }

    public class TipoActividadCombo
    {
        public int TipoActividadId { get; set; }
        public string Descripcion { get; set; }
    }

    public class ProveedorCombo : IEquatable<ProveedorCombo>
    {
        public int ProveedorId { get; set; }
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }
        public string Alias { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProveedorCombo);
        }

        public bool Equals(ProveedorCombo other)
        {
            return other != null &&
                   Cuit == other.Cuit;
        }

        public override int GetHashCode()
        {
            return -833617328 + EqualityComparer<string>.Default.GetHashCode(Cuit);
        }
    }

    public class TipoTelefonoQry
    {
        public int TipoTelefonoId;
        public string Descripcion;
    }

    public class ProvinciaQry
    {
        public int Provinciaid;
        public string Nombre;
        public bool Inscripto { get; set; }
        public int Orden { get; set; }
    }

    public class MonedaQry
    {
        public string MonedaId;
        public string Descripcion;
    }

    public class TipoFasonQry
    {
        public int Id;
        public string Descripcion;
    }

    public class TipoAgenteCompraQry
    {
        public int Id;
        public string Descripcion;
    }

    public class OperadorQry
    {
        public int Id;
        public string Descripcion;
    }

    public class TipoNegocioQry
    {
        public int TipoNegocioId;
        public string Descripcion;
    }

    public class ClaseNegocioQry
    {
        public int ClaseNegocioId;
        public string Descripcion;
    }

    public class EstadoICQry
    {
        public int EstadoInformeId { get; set; }
        public string Descripcion { get; set; }
    }

    public class LocalidadQry
    {
        public int LocalidadId;
        public string CodLocalidad;
        public string Nombre;
        public int ProvinciaId;
    }

    public class CanalOperacionQry
    {
        public int CanalOperacionId;
        public string Descripcion;
        public bool Inhabilitado;
    }

    public class MaterialQry
    {
        public int MaterialId;
        public string Codigo;
        public string Descripcion;
        public int CampañaIdActual;
        public int CampaniaTableroId;
    }

    public class DestinatarioQry
    {
        public int DestinatarioId;
        public string Descripcion;
        public bool Inhabilitado;
    }

    public class CondicionQry
    {
        public int CondicionId;
        public string Descripcion;
        public bool Inhabilitado;
    }

    public class InteresQry
    {
        public int InteresId;
        public string Descripcion;
    }

    public class TipoActividadQry
    {
        public int TipoActividadId;
        public string Descripcion;
        //public int? CamposExtra { get; set; }
    }

    public class ContactoComercialQry
    {
        public int ContactoComercialId;
        public string Nombres;
    }

    public class ClasificacionCompraNetQry
    {
        public int Id;
        public string Descripcion;
    }

    public class BoletoCompraNetQry
    {
        public int Id;
        public string Descripcion;
    }

    public class BolsaCompraNetQry
    {
        public int Id;
        public string Descripcion;
    }

    public class CentroQry
    {
        public int Id;
        public int ProvinciaId;
        public string Descripcion;
        public string CodigoSap;
    }

    public class CondicionFijacionQry
    {
        public int Id;
        public string Descripcion;
        public string CodigoSap;
    }

    public class StandardDeCalidadQry
    {
        public int Id;
        public string Descripcion;
        public string CodigoSap;
    }

    public class CalidadesEspecialesQry
    {
        public int Id;
        public string Descripcion;
        public string CodigoSap;
        public string MaterialId;
    }

    public class TipoDBQry
    {
        public int Id;
        public string Descripcion;
        public string CodigoSap;        
    }

    public class TipoPeriodoDBQry
    {
        public int Id;
        public string Descripcion;
        public string CodigoSap;
    }

    public class EstadosContratos {
        public int EstadosContratosId { get; set; }
        public string Descripcion { get; set; }

        public EstadosContratos() { }
        public EstadosContratos(int EstadosContratosId, string Descripcion) {
            this.EstadosContratosId = EstadosContratosId;
            this.Descripcion = Descripcion;
        }
    }

    public class CentroCombo
    {
        public int Id;
        public string Descripcion;
        public string CodigoSap;
    }

    public class ZonaCupoCombo
    {
        public int Id;
        public string Descripcion;
        public string CodigoSap;
    }

    public class ZonaCombo
    {
        public int Id;
        public string Descripcion;
        public string CodigoSap;
    }

    public class OperadorCombo
    {
        public int Id;
        public string Descripcion;
    }

    public class ContratoAcuerdoCombo
    {
        public int Id;
    }

    public class CamaraQry
    {
        public int Id;
        public string Descripcion;
    }

    public class CondicionDePagoVentaQry
    {
        public int Id;
        public string Descripcion;
        public bool? CondicionFijacion;
        public bool? CondicionPesificado;
    }

    public class ComisionAFavorQry
    {
        public int Id;
        public string Descripcion;
    }

    public class FleteACargoQry
    {
        public string Descripcion;
    }

    public class KgBalanzaQry
    {
        public string Descripcion;
    }

    public class PagoQry
    {
        public string Descripcion;
    }

    public class CondicionPagoQry
    {
        public string Descripcion;
    }

    public class BoletoVentaQry
    {
        public int Id;
        public string Descripcion;
    }

    public class RangoCombo
    {
        public int Id { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public string Material { get; set; }
        public string MonedaId { get; set; }
        public DateTime FechaDesde { get; set; }
    }

    public class RolCombo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Disabled { get; set; }
    }
}