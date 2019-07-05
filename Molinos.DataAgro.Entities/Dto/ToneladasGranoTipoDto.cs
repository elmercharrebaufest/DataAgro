using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ToneladasGranoTipoDto
    {
        public string Material { get; set; }
        public double DispAFijar { get; set; }
        public double DispAPrecio { get; set; }
        public double DispFijac { get; set; }
        public double DispFason { get; set; }
        public double FrwAFijar { get; set; }
        public double FrwAPrecio { get; set; }
        public double FrwFijac { get; set; }
        public double FrwFason { get; set; }
        public double NewAFijar { get; set; }
        public double NewAPrecio { get; set; }
        public double NewFijac { get; set; }
        public double NewFason { get; set; }
        public double Total { get; set; }
    }

    public class NegocioToneladasPosicionDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public DateTime Posicion { get; set; }
        public string PosicionString { get; set; }
        public int TipoNegocioId { get; set; }
        public int MaterialCampanaId { get; set; }
        public int CampanaId { get; set; }
        public double Cantidad { get; set; }
    }
}
