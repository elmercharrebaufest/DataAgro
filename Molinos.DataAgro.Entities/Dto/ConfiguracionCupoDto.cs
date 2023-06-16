using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ConfiguracionCupoDto
    {
        public int Id { get; set; }
        public int CentroId { get; set; }
        public string Centro { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public DateTime Fecha { get; set; }
        public int LimiteCupo { get; set; }
        public int LimiteAlgoritmo { get; set; }
        public bool? CierreCupera { get; set; }
        public List<LimiteCupoDto> CantidadCupo { get; set; }
        public string BloquearCupera { get; set; }
        public int LimiteCupoAnterior { get; set; }
        public string ZonaCupo { get; set; }
        public bool LiberarCupera { get; set; }
        public string LiberarCuperaDesc { get; set; }
        public int CuposConsumidos { get; set; }
        public string CentroCodigoSap { get; set; }
        public string MaterialCodigoSap { get; set; }
        public string Color { get; set; }
        public bool Bloquear { get; set; }
        public int CuposDisponibles { get; set; }
        public bool NoPropio { get; set; }
        public bool? Sustentable { get; set; }
        public int LimiteDescarga { get; set; }
        public int CuposDisponiblesConDescarga { get; set; }
        public int CuposConsumidosConDescarga { get; set; }
    }
}