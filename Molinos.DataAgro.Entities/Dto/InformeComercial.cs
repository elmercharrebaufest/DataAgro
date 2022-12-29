using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Entities.Dto
{
    public class RptInformeComercialInfo
    {
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string DomReal { get; set; }
        public string DomLegal { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public List<RptContactosInfo> Contactos { get; set; }
        public string CorrBsAs { get; set; }
        public string CorrRos { get; set; }
        public string OrIntNorte { get; set; }
        public string OrIntCentro { get; set; }
        public string OrIntSur { get; set; }
        public string Productores { get; set; }
        public string Canjeadores { get; set; }
        public string Corredores { get; set; }
        public string Acopiadores { get; set; }
        public string GrandesCuentas { get; set; }
        public string Exportadores { get; set; }
        public string Campaña { get; set; }
        public string ToneladasTodo { get; set; }
        public List<InformeComercialAcopiadores> CapProduccion { get; set; }
        public List<InformeComercialAcopiadores> CapAlmacenaje { get; set; }
        public string AntActividad { get; set; }
        public string RelacionDependenciaSi { get; set; }
        public string RelacionDependenciaNo { get; set; }
        public string RelacionDependenciaCant { get; set; }
        public string RodadosEquipamientoPropio { get; set; }
        public string RodadosAlquilado { get; set; }
        public string RodadosPropioYAlquilado { get; set; }
        public string RodadosOtros { get; set; }
        public string RodadosOtrosTexto { get; set; }
        public string ServicioPersonalPropio { get; set; }
        public string ServicioPersonalContratado { get; set; }
        public string ServicioPropioYContratado { get; set; }
        public string ServicioOtros { get; set; }
        public string ServicioOtrosTexto { get; set; }
        public string Antiguedad { get; set; }
        public string ActuacionProduccion { get; set; }
        public string ClientesAnteriores { get; set; }
        public string Comentarios { get; set; }
        public string ContactoNombre1 { get; set; }
        public string ContactoCargo1 { get; set; }
        public string ContactoTelefono1 { get; set; }
        public string ContactoMail1 { get; set; }
        public string ContactoNombre2 { get; set; }
        public string ContactoCargo2 { get; set; }
        public string ContactoTelefono2 { get; set; }
        public string ContactoMail2 { get; set; }


        public RptInformeComercialInfo()
        {
            Contactos = new List<RptContactosInfo>();
            CapProduccion = new List<InformeComercialAcopiadores>();
            CapAlmacenaje = new List<InformeComercialAcopiadores>();
        }
    }

    public class ParamInformeComercial
    {
        public int ProveedorId { get; set; }
        public List<ParamInformeComercialMaterial> Materiales { get; set; }
        public int CampañaId { get; set; }
        public string Campaña { get; set; }
        public bool EmplRelDep { get; set; }
        public string EmplRelDepCant { get; set; }
        public int? Rodados { get; set; }
        public string RodadosOtros { get; set; }
        public int? Chacra { get; set; }
        public string ChacraOtros { get; set; }
        public string AntigActividad { get; set; }
        public string ActuacionProd { get; set; }
        public string ClienteAnt { get; set; }
        public string Comentarios { get; set; }
        public string Domicilio { get; set; }
        public int? InformeComercialId { get; set; }
        public DateTime? FechaDescarga { get; set; }
        public bool? OrigenDA { get; set; }

        public ParamInformeComercial()
        {
            Materiales = new List<ParamInformeComercialMaterial>();
        }

    }

    public class ParamInformeComercialMaterial
    {
        public int MaterialId { get; set; }
        public float? Toneladas { get; set; }
    }

    public class oParamInforme
    {
        public int filtro { get; set; }
    }

    public class InformesModel
    {
        public List<InformeComercialMaterialDisponible> materiales { get; set; }
        public List<InformeGeneradoList> InformeGenerado { get; set; }

        public InformesModel()
        {
            materiales = new List<InformeComercialMaterialDisponible>();
            InformeGenerado = new List<InformeGeneradoList>();
        }
    }

    public class InformeComercialMaterialDisponible
    {
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public int CampañaId { get; set; }
        public string Campaña { get; set; }
        public string Material { get; set; }
    }

    public class InformeGeneradoList
    {
        public int InformeComercialId { get; set; }
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Campaña { get; set; }
        public string Materiales { get; set; }
        public string Comercial { get; set; }
        public int EstadoId { get; set; }
        public string EstadoInforme { get; set; }
        public DateTime FechaAlta { get; set; }



        public InformeGeneradoList()
        {
            InformeComercialId = 0;
        }

    }

    public class InformeComercialAcopiadores
    {
        public string Grano { get; set; }
        public float Superficie { get; set; }
        public int Toneladas { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Propio { get; set; }
        public string Alquilado { get; set; }
    }
    public class InformeComercialMateriales
    {
        public int MaterialId { get; set; }
        public string Descripcion { get; set; }
    }

    public class InformeResult : Resultado
    {
        public int? InformeId { get; set; }
    }

    public class InformeList
    {
        public int InformeComercialId { get; set; }
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string Campaña { get; set; }
        public IEnumerable<string> MaterialesList { get; set; }
        public string Comercial { get; set; }
        public bool Seleccionado { get; set; }
        public int ProveedorId { get; set; }
        public int? CampanaId { get; set; }
        public int? ComercialId { get; set; }
        public IEnumerable<int?> MaterialesIdList { get; set; }
        public string Materiales { get { return string.Join(", ", this.MaterialesList.Distinct()) ?? ""; } }
        public string MaterialId { get { return string.Join(", ", this.MaterialesIdList.Distinct()) ?? ""; } }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaDescarga { get; set; }
        public string OrigenDA { get; set; }

        public InformeList()
        {
            Seleccionado = false;
        }

    }

    public class ParamReportesIC
    {
        public int? ComercialID { get; set; }
        public int? ComercialIDGenerador { get; set; }
        public string Cuit { get; set; }
        public int? MaterialID { get; set; }
        public int? EstadoId { get; set; }
    }

    public class ReportesList
    {
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public DateTime? FechaDeGeneracion { get; set; }
        public string Comercial { get; set; }
        public string Estado { get; set; }
        public string Material { get; set; }
        public string Observaciones { get; set; }
        public int InformeComercialId { get; set; }
    }

    public class oParamExcel
    {
        public string Informes { get; set; }
    }

    public class ResultCapacidadProductiva
    {
        public string Proveedor { get; set; }
        public Decimal Soja { get; set; }
        public Decimal Maiz { get; set; }
        public Decimal Trigo { get; set; }
    }

    public class MaterialesModificacionInforme
    {
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public bool Seleccionado { get; set; }

        public MaterialesModificacionInforme()
        {
            Seleccionado = false;
        }
    }

    public class NuevoProduccion
    {
        public int MaterialId { get; set; }
        public int Hectareas { get; set; }
        public int Toneladas { get; set; }
        public int LocalidadId { get; set; }
        public bool ArrendaPropia { get; set; }
        public int CampañaId { get; set; }
    }
    public class NuevoAcopio
    {
        public int Toneladas { get; set; }
        public int LocalidadId { get; set; }
        public bool ArrendaPropia { get; set; }
        public int CampañaId { get; set; }
    }

}
