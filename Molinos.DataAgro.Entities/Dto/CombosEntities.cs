using System;
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

    public class ProveedorCombo
    {
        public int ProveedorId { get; set; }
        public string RazonSocial { get; set; }
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

    public class RangoCombo
    {
        public int Id { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public string Material { get; set; }
        public string MonedaId { get; set; }
        public DateTime FechaDesde { get; set; }
    }
}


