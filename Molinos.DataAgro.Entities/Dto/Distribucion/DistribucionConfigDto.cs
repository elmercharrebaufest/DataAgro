using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class DistribucionConfigDto
    {
        public bool AplicarCuotaOperador { get; set; }
        public Dictionary<string, int> CuotasPorOperador { get; set; }
        public bool AplicarCuotaClase { get; set; }
        public Dictionary<string, int> CuotasPorClase { get; set; }
        public bool ExcluirFason { get; set; }
        public bool ExcluirAgenteCompra { get; set; }
        public bool PriorizarSustentables { get; set; }
        public Dictionary<string, decimal> PreciosReferencia { get; set; }
        public bool HabilitarFiltroFechas { get; set; }
        public string FiltroFechaDesdeMin { get; set; }
        public string FiltroFechaDesdeMax { get; set; }
        public string FiltroFechaHastaMin { get; set; }
        public string FiltroFechaHastaMax { get; set; }
        public decimal? CuitMaxPct { get; set; }

        public DistribucionConfigDto()
        {
            CuotasPorOperador = new Dictionary<string, int>();
            CuotasPorClase = new Dictionary<string, int>();
            PreciosReferencia = new Dictionary<string, decimal>();
        }
    }
}
