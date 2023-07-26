using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class AcuerdoSap
    {
        public decimal Cantidad { get; set; }
        public string ClaseDoc { get; set; }
        public string Cliente { get; set; }
        public string Cosecha { get; set; }
        public string Estado { get; set; }
        public string FechaReg { get; set; }
        public string FechaPerval { get; set; }
        public string Inperval { get; set; }
        public string Material { get; set; }
        public string NroAcuerdo { get; set; }
        public decimal PrecioServ { get; set; }
        public string Unidad { get; set; }
        public int CampanaId { get; set; }
    }
}


