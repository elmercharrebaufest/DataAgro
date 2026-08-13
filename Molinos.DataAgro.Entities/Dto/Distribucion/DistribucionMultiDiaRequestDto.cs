using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class DistribucionMultiDiaRequestDto
    {
        public List<ContratoSapImportadoDto> Contratos { get; set; }
        public Dictionary<string, decimal> CcppByCuitMat { get; set; }
        public List<string> Fechas { get; set; }
        public List<LimiteDiaDto> LimitesPorDia { get; set; }
        public bool DistribuirUniforme { get; set; }
        public DistribucionConfigDto Configuracion { get; set; }

        public DistribucionMultiDiaRequestDto()
        {
            Contratos = new List<ContratoSapImportadoDto>();
            CcppByCuitMat = new Dictionary<string, decimal>();
            Fechas = new List<string>();
            LimitesPorDia = new List<LimiteDiaDto>();
            Configuracion = new DistribucionConfigDto();
        }
    }
}
