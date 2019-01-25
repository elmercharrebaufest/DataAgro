using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class ReporteCompraNetModel
    {
        public List<ToneladasGranoTipoDto> ToneladasGranoTipo { get; set; }
        public ReporteSojaSustDto SojaSustentable { get; set; }
        public List<PosicionComprasDto> PosicionCompras { get; set; }
        public List<PrecioCantidadDto> PrecioCantidad { get; set; }
        public List<HedgeMaterialModel> HedgeMaterial { get; set; }
        public HedgeCargaObjetivoDto HedgeObjetivo { get; set; }
        public HedgeTCPromedioDto TCPromedioDto { get; set; }
        public AgenteCompraModel AgenteCompras { get; set; }
    }
}