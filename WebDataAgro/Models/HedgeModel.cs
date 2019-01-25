using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class HedgeModel
    {
        public List<HedgeMaterialModel> HedgeMaterial { get; set; }
        public List<HedgeObjetivoModel> HedgeObjetivo { get; set; }
        public HedgeTCModel HedgeTC { get; set; }
        public TcModel TCModel { get; set; }
        public Resultado Resultado { get; set; }
    }

    public class HedgeMaterialModel
    {
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public decimal Disponible { get; set; }
        public decimal Forward { get; set; }
        public decimal NewCrop { get; set; }
    }
    public class HedgeObjetivoModel
    {
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public decimal Pricing { get; set; }
        public decimal ARemitir { get; set; }
    }
    public class HedgeTCModel
    {
        public List<HedgeTCDto> HedgeTC { get; set; }
        public decimal TotalTC  { get; set; }
        public decimal TotalHedge { get; set; }
    }
    public class TcModel
    {
        public decimal TC { get; set; }
        public decimal HedgePesos { get; set; }
    }
}