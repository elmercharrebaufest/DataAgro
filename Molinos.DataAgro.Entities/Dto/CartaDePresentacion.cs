using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class RptCartaDePresentacionInfo
    {
        public string corredorCuit { get; set; }
        public string corredorRazonSocial { get; set; }
        public string corredorBolsa { get; set; }
        public string corredorNroRegistro { get; set; }
        public string vendedorCuit { get; set; }
        public string vendedorRazonSocial { get; set; }
        public string vendedorDomicilioFiscal { get; set; }
        public string vendedorActividad { get; set; }
        public string vendedorAntiguedadEnActividad { get; set; }
        public string vendedorAntecedentesComerciales { get; set; }
        public string vendedorMailContacto { get; set; }
        public string vendedorTelefonoContacto { get; set; }
        public string vendedorDomicilioReal { get; set; }
        public string vendedorCosecha { get; set; }
        //public string fimarAclaracion { get; set; }
        //public string nroDNI { get; set; }
        //public string cargo { get; set; }
        public List<CartaDePresentacionAcopiadores> CapProduccion { get; set; }
        public List<CartaDePresentacionAcopiadores> CapAlmacenaje { get; set; }
        public string ToneladasTodo { get; set; }

        public RptCartaDePresentacionInfo()
        {
            CapProduccion = new List<CartaDePresentacionAcopiadores>();
            CapAlmacenaje = new List<CartaDePresentacionAcopiadores>();
        }
    }
    public class CartaDePresentacionAcopiadores
    {
        public string Grano { get; set; }
        public float Superficie { get; set; }
        public int Toneladas { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Propio { get; set; }
        public string Alquilado { get; set; }
    }

    //public class ParamCartaDePresentacion
    //{
    //    public int ProveedorId { get; set; }
    //    public List<ParamCartaDePresentacionMaterial> Materiales { get; set; }
    //    public int CampañaId { get; set; }
    //    public string Campaña { get; set; }
    //    public bool EmplRelDep { get; set; }
    //    public string EmplRelDepCant { get; set; }
    //    public int? Rodados { get; set; }
    //    public string RodadosOtros { get; set; }
    //    public int? Chacra { get; set; }
    //    public string ChacraOtros { get; set; }
    //    public string AntigActividad { get; set; }
    //    public string ActuacionProd { get; set; }
    //    public string ClienteAnt { get; set; }
    //    public string Comentarios { get; set; }
    //    public string Domicilio { get; set; }
    //    public int? CartaDePresentacionId { get; set; }

    //    public ParamCartaDePresentacion()
    //    {
    //        Materiales = new List<ParamCartaDePresentacionMaterial>();
    //    }

    //}

    //public class ParamCartaDePresentacionMaterial
    //{
    //    public int MaterialId { get; set; }
    //    public float? Toneladas { get; set; }
    //}

    //public class oParamInforme
    //{
    //    public int filtro { get; set; }
    //}

    //public class InformesModel
    //{
    //    public List<CartaDePresentacionMaterialDisponible> materiales { get; set; }
    //    public List<InformeGeneradoList> InformeGenerado { get; set; }

    //    public InformesModel()
    //    {
    //        materiales= new List<CartaDePresentacionMaterialDisponible>();
    //        InformeGenerado = new List<InformeGeneradoList>();
    //    }
    //}

    //public class CartaDePresentacionMaterialDisponible
    //{
    //    public int ProveedorId { get; set; }
    //    public int MaterialId { get; set; }
    //    public int CampañaId { get; set; }
    //    public string Campaña { get; set; }
    //    public string Material { get; set; }
    //}

    //public class InformeGeneradoList
    //{
    //    public int CartaDePresentacionId { get; set; }
    //    public string Cuit { get; set; }
    //    public string RazonSocial { get; set; }
    //    public string Campaña { get; set; }
    //    public string Materiales { get; set; }
    //    public string Comercial { get; set; }
    //    public int EstadoId { get; set; }
    //    public string EstadoInforme { get; set; }

    //}



    //  public class CartaDePresentacionMateriales
    //  {
    //      public int MaterialId { get; set; }
    //      public string Descripcion { get; set; }
    //  }

    //  public class InformeResult : Resultado
    //  {
    //      public int? InformeId { get; set; }
    //  }

    //  public class InformeList
    //  {
    //      public int CartaDePresentacionId { get; set; }
    //      public string Cuit { get; set; }
    //      public string RazonSocial { get; set; }
    //      public string Campaña { get; set; }
    //      public string Materiales { get; set; }
    //      public string Comercial { get; set; }
    //      public bool Seleccionado { get; set; }


    //      public InformeList()
    //      {
    //          Seleccionado=false;
    //      }

    //  }

    //  public class ParamReportesIC
    //  {
    //      public int? ComercialID { get; set; }
    //      public int? ComercialIDGenerador { get; set; }
    //      public string Cuit { get; set; }
    //public int? MaterialID { get; set; }
    //      public int? EstadoId { get; set; }
    //  }

    //  public class ReportesList
    //  {
    //      public string Cuit { get; set; }
    //   public string RazonSocial { get; set; }
    //      public DateTime? FechaDeGeneracion { get; set; }
    //      public string Comercial { get; set; }
    //      public string Estado { get; set; }
    //      public string Material { get; set; }
    //      public string Observaciones { get; set; }
    //      public int CartaDePresentacionId { get; set; }
    //  }

    //  public class ResultCapacidadProductiva
    //  {
    //      public string Proveedor { get; set; }
    //      public Decimal Soja { get; set; }
    //      public Decimal Maiz { get; set; }
    //      public Decimal Trigo { get; set; }
    //  }

    //  public class MaterialesModificacionInforme
    //  {
    //      public int MaterialId { get; set; }
    //      public string Material { get; set; }
    //      public bool Seleccionado { get; set; }

    //      public MaterialesModificacionInforme()
    //      {
    //          Seleccionado = false;
    //      }
    //  }

    //  public class NuevoProduccion {
    //      public int MaterialId { get; set; }
    //      public int Hectareas { get; set; }
    //      public int Toneladas { get; set; }
    //      public int LocalidadId { get; set; }
    //      public bool ArrendaPropia { get; set; }
    //      public int CampañaId { get; set; }
    //  }
    //  public class NuevoAcopio
    //  {
    //      public int Toneladas { get; set; }
    //      public int LocalidadId { get; set; }
    //      public bool ArrendaPropia { get; set; }
    //      public int CampañaId { get; set; }
    //  }

}
