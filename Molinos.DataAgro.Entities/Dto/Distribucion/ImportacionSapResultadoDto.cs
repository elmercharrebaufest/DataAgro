using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class ImportacionSapResultadoDto
    {
        public List<ContratoSapImportadoDto> Contratos { get; set; }
        public Dictionary<string, decimal> CcppByCuitMat { get; set; }
        public Dictionary<string, decimal> CcppByMat { get; set; }
        public List<string> Materiales { get; set; }
        public EstadisticasImportacionDto Estadisticas { get; set; }
        public List<string> Errores { get; set; }

        public ImportacionSapResultadoDto()
        {
            Contratos = new List<ContratoSapImportadoDto>();
            CcppByCuitMat = new Dictionary<string, decimal>();
            CcppByMat = new Dictionary<string, decimal>();
            Materiales = new List<string>();
            Estadisticas = new EstadisticasImportacionDto();
            Errores = new List<string>();
        }
    }
}
