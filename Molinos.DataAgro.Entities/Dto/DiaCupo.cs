using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DiaCupo
    {
        public DateTime Fecha { get; set; }
        public int? Cantidad { get; set; }

        public int? CantidadFleteProcedencia { get; set; }
        public int CantidadSugerencia { get; set; }
        public int DisponibilidadEnPlanta { get; set; }
        public decimal CantidadDeCupoSugerencias { get; set; }
        public int CantidadCuposLibres { get; set; }
        public int CantidadDisponibilidadDia { get; set; }
        public int CantidadSugerenciaAceptadaDia { get; set; }
        public int CantidadCuposDevueltos { get; set; }
        public int CantidadSugerenciaNoAceptadas { get; set; }
        public string NombreComercial { get; set; }
        public int CantidadSugerenciaPendiente { get; set; }
        public int CantidadSolicitudesAceptadas { get; set; }
    }
}


