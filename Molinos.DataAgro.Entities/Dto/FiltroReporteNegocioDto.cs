using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class FiltroReporteNegocioDto
    {
        public int ComercialId { get; set; }
        public string ContratoSAP { get; set; }
        public int TipoNegocioId { get; set; }
        public string FechaCarga { get; set; }
        public string FechaEntregaDesde { get; set; }
        public string FechaEntregaHasta { get; set; }
        public string FechaHastaFijacion { get; set; }
        public int CampaniaId { get; set; }
        public int MaterialId { get; set; }
        public int GrupoDeCompraId { get; set; }
        public int EstadoId { get; set; }
        public int CentroId { get; set; }
        public int ProveedorId { get; set; }
        public int CorredorId { get; set; }
        public int BoletoCompraNetId { get; set; }
        public decimal? ImporteSustentable { get; set; }
        public int DiasDiferimiento { get; set; }
        public string FechaLimiteDolarizado { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<OrdenarFiltros> Sort { get; set; }

        public override bool Equals(Object obj)
        {
            var result = false;
            Type type = typeof(FiltroReporteNegocioDto);
            var propiedades = type.GetProperties();
            propiedades = propiedades.Where(x => x.Name != "Page"&& x.Name != "PageSize" && x.Name != "sort").ToArray();
            
            foreach (PropertyInfo pi in propiedades)
            {
                if (!result)
                {
                    dynamic value;
                    if (pi.PropertyType == typeof(string))
                    {
                        value = (string)pi.GetValue(obj);
                        result = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? false : true ;
                    }
                    else if (pi.PropertyType == typeof(int))
                    {
                        value = (int)pi.GetValue(obj);
                        result = value == 0 || value == null ? false: true;

                    }else if (pi.PropertyType == typeof(decimal?))
                    {
                        value = (decimal?)pi.GetValue(obj);
                        result = value == 0 || value == null ? false : true;
                    }
                }
                else
                {
                    return result;
                }
            }
            return result;
        }
    }
    public class OrdenarFiltros
    {
        public string field { get; set; }
        public string dir { get; set; }
    }
}
