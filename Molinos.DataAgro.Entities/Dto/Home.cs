using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CampañaHome
    {
        public string Nombre { get; set; }
        public List<MaterialCampaña> Materiales { get; set; }
    }

    public class MaterialCampaña
    {
        public string Nombre { get; set; }
        public double Toneladas { get; set; }
        public string Campaña { get; set; }
    }
    public class ObjetivoHome
    {
        public List<DetalleObjetivo> Comerciales { get; set; }
        public List<MaterialObjetivo> Objetivos { get; set; }
        public ObjetivoHome()
        {
            Objetivos = new List<MaterialObjetivo>();
            Comerciales = new List<DetalleObjetivo>();
        }
    }
    public class DetalleObjetivo
    {
        public string Comercial { get; set; }
        public int ComercialId { get; set; }
        public List<MaterialObjetivo> Objetivos { get; set; }

        public DetalleObjetivo()
        {
            Objetivos = new List<MaterialObjetivo>();
        }
    }
    public class MaterialObjetivo
    {
        public int Id { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public double Toneladas { get; set; }
        public string Campana { get; set; }
        public string Comercial { get; set; }
        public int ComercialId { get; set; }
    }
    public class Contactos
    {

        public int? Calificacion { get; set; }
        public string ComercialAcargo { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Email4 { get; set; }
        public string CUIT { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string Telefono4 { get; set; }
        public string RazonSocial { get; set; }
        public int ProveedorId { get; set; }
        public DateTime? FechaUltimoContacto { get; set; }
        public int? Facacop { get; set; }
        public string RiesgoComercialSap { get; set; }
        public int EstadoCuit { get; set; }
        public string Situacion { get; set; }
        public string Estado { get; set; }
        public string Condicion { get; set;}
        public DateTime? FechaAlta { get; set; }
        public string GrupoDeCompras { get; set; }
        public int Segmentacion { get; set; }
    }

    public class oParamBusqueda
    {
        public string Calificacion { get; set; }
        public int ComercialId { get; set; }
        public string Campaña { get; set; }
        public string Segmentacion { get; set; }
        public string Estado { get; set; }
        public string Actividad { get; set; }
        public string Material { get; set; }
        public string Hectareas { get; set; }
        public string Toneladas { get; set; }
        public string Condicion { get; set; }
        //Este es la propiedad del filtro
        public string Comercial { get; set; }
        public string Zona { get; set; }
        public List<int> Equipo { get; set; }
    }

    public class DatosIniciales
    {
        public List<MaterialesQry> mat { get; set; }
        public List<CampañaQry> camp { get; set; }
        public List<SegmentacionQry> segm { get; set; }
        public List<TipoActividadQry> tipoact { get; set; }
        public List<EstadoQry> est { get; set; }
        public List<CondicionPreferenteQry> cond { get; set; }
        public List<ComercialQry> come { get; set; }
        public List<ZonaQry> zona { get; set; }

        public DatosIniciales()
        {
            mat = new List<MaterialesQry>();
            camp = new List<CampañaQry>();
            segm = new List<SegmentacionQry>();
            tipoact = new List<TipoActividadQry>();
            est = new List<EstadoQry>();
            cond = new List<CondicionPreferenteQry>();
            come = new List<ComercialQry>();
            zona = new List<ZonaQry>();
        }
    }

    public class DatosInicialesReportes
    {
        public List<MaterialesQry> mat { get; set; }
        public List<CampañaQry> camp { get; set; }
        public List<SegmentacionQry> segm { get; set; }
        public List<ComercialQry> come { get; set; }
        public List<ProvinciaQry> provs { get; set; }
        public List<EstadoICQry> estic { get; set; }

        public DatosInicialesReportes()
        {
            mat = new List<MaterialesQry>();
            camp = new List<CampañaQry>();
            segm = new List<SegmentacionQry>();
            come = new List<ComercialQry>();
            provs = new List<ProvinciaQry>();
            estic = new List<EstadoICQry>();
        }

    }


    public class MaterialesQry
    {
        public int MaterialId { get; set; }
        public string Descripcion { get; set; }
    }

    public class CampañaQry
    {
        public int CampañaId { get; set; }
        public string Descripcion { get; set; }
    }
    public class ProveedorQry
    {
        public int ProveedorId { get; set; }
        public string Descripcion { get; set; }
    }
    public class SegmentacionQry
    {
        public int SegmentacionId { get; set; }
        public string Descripcion { get; set; }
        public string Grupo { get; set; }
    }

    public class EstadoQry
    {
        public int EstadoId { get; set; }
        public string Descripcion { get; set; }
    }


    public class CondicionPreferenteQry
    {
        public int CondicionId { get; set; }
        public string Descripcion { get; set; }
    }

    public class ComercialQry
    {
        public int ComercialId { get; set; }
        public string IdActiveDirectory { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Comercial { get; set; }
        public int? EmpleadorACargo { get; set; }
    }

    public class ZonaQry
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class NivelTarifaQry
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSap { get; set; }
    }
}
