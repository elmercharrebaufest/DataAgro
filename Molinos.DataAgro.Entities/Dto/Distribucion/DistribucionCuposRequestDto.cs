using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class DistribucionCuposRequestDto
    {
        public string Fecha { get; set; }
        public List<ContratoSapImportadoDto> Contratos { get; set; }
        public Dictionary<string, decimal> CcppByCuitMat { get; set; }
        public Dictionary<string, int> LimitesPorMaterial { get; set; }
        public DistribucionConfigDto Configuracion { get; set; }

        public DistribucionCuposRequestDto()
        {
            Contratos = new List<ContratoSapImportadoDto>();
            CcppByCuitMat = new Dictionary<string, decimal>();
            LimitesPorMaterial = new Dictionary<string, int>();
            Configuracion = new DistribucionConfigDto();
        }
    }
}
