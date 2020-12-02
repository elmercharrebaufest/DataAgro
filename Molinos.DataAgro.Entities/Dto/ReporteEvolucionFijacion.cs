using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{

    public class ReporteEvolucionFijacionModel
    {
        public List<ReporteEvolucionFijacion> tablero { get; set; } = new List<ReporteEvolucionFijacion>();
        public List<BasicoContrato> afijar { get; set; } = new List<BasicoContrato>();
        public List<BasicoContrato> fijaciones { get; set; } = new List<BasicoContrato>();

    }

    public class ReporteEvolucionFijacion
    {
        public int Anio { get; set; }
        public int MesId { get; set; }
        public string Mes { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }

        public double TotalAfijar { get; set; }
        public double TotalFijacion { get; set; }
        public double UltimaSemana { get; set; }
        public double Porcentaje { get; set; }
        public string MesCompleto { get; set; }
    }
   
}