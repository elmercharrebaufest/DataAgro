using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ReporteCompraNetModel
    {
        public List<ToneladasGranoTipoDto> ToneladasGranoTipo { get; set; }
        public List<PosicionComprasDto> PosicionCompras { get; set; }
        public List<PrecioCantidadDto> PrecioCantidad { get; set; }
        public List<PricingCampaniaDto> PricingCampania { get; set; }
        public List<HedgeMaterialModel> HedgeMaterial { get; set; }
        public HedgeCargaObjetivoDto HedgeObjetivo { get; set; }
        public HedgeTCPromedioDto TCPromedioDto { get; set; }
        public AgenteCompraModel AgenteCompras { get; set; }
        public ReporteSojaSustDto SojaSustentable { get; set; }
        public ReporteSojaEPAyEUDRDto SojaEPAyEUDR { get; set; }
    }
    public class HedgeMaterialModel
    {
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public decimal Disponible { get; set; }
        public decimal Forward { get; set; }
        public decimal NewCrop { get; set; }
    }
}