using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PosicionComprasDto
    {
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public List<PosicionKilos> PosicionKilos { get; set; }
        public double Total { get; set; }
    }

    public class PosicionKilos
    {
        public EnumMeses Mes { get; set; }
        public double KilosPesos { get; set; }
        public double KilosDolares { get; set; }
        public double DispAFijar { get; set; }
        public double DispAPrecio { get; set; }
        public double DispFijac { get; set; }
        public double FrwAFijar { get; set; }
        public double FrwAPrecio { get; set; }
        public double FrwFijac { get; set; }
        public double NewAFijar { get; set; }
        public double NewAPrecio { get; set; }
        public double NewFijac { get; set; }
        public int? Anio { get; set; }
        public decimal? PrecioPonderadoPesos { get; set; }
        public decimal? PrecioPonderadoDolares { get; set; }
        public double? CantidadPonderada { get; set; }
        public int TipoNegocioId { get; set; }
        public int NegocioId { get; set; }
        public IEnumerable<int> ListDispAFijar { get; set; }
        public IEnumerable<int> ListDispAPrecio { get; set; }
        public IEnumerable<int> ListDispFijac { get; set; }
        public IEnumerable<int> ListFrwAFijar { get; set; }
        public IEnumerable<int> ListFrwAPrecio { get; set; }
        public IEnumerable<int> ListFrwFijac { get; set; }
        public IEnumerable<int> ListNewAFijar { get; set; }
        public IEnumerable<int> ListNewAPrecio { get; set; }
        public IEnumerable<int> ListNewFijac { get; set; }
        public double DispAPrecioPesos { get; set; }
        public double DispAPrecioDolares { get; set; }
        public double DispFijacPesos { get; set; }
        public double DispFijacDolares { get; set; }
        public double FrwAPrecioPesos { get; set; }
        public double FrwAPrecioDolares { get; set; }
        public double FrwFijacPesos { get; set; }
        public double FrwFijacDolares { get; set; }
        public double NewAPrecioPesos { get; set; }
        public double NewAPrecioDolares { get; set; }
        public double NewFijacPesos { get; set; }
        public double NewFijacDolares { get; set; }
        public int? CampanaId { get; set; }
        public string Campana { get; set; }

    }

}
