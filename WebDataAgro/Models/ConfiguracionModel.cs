using Molinos.DataAgro.Entities.Dto;
using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;

namespace WebDataAgro.Models
{
    public class ConfiguracionModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_Dias")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_Dias")]
        public int CantidadDias { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ClaveStop")]
        public string ClaveStop { get; set; }
        public bool ConexionABMStop { get; set; }
        public bool ConexionConsultaStop { get; set; }
        public int TerminalStopId { get; set; }
        public string CuitDestinoStop { get; set; }
        public int CodigoLocalidadStop { get; set; }
        public decimal ImporteSustentable { get; set; }
        public Resultado Resultado { get; set; }
        public decimal? ContratoAperturaPrecioPorcentajeDeComisionMaximo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_Dias")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_Dias")]
        public int DiasDiferimiento { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_Cantidad")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_Cantidad")]
        public int CantidadAcuerdo { get; set; }
        public int CantidadDiasDolarizadoLimiteMaximo { get; set; }
        public int CantidadMaxima { get; set; }
        public int CantidadDiasPesificadoLimite { get; set; }
        public int RedespachoMaximoUSDM { get; set; }
        public int RedespachoMaximoARP { get; set; }
        public int ToleranciaPaseMax { get; set; }
        public int ToleranciaPaseMin { get; set; }
        public decimal ImporteSustentableEspecial { get; set; }
        public int AlgoritmoKilosMinimosParaSugerencia { get; set; }
        public int AlgoritmoProcMaxSugerenciasProveedorDia { get; set; }
        public int Actualizacion { get; set; }
        public string ApiKeyBolsaRosario { get; set; }
        public string SecretBolsaRosario { get; set; }
        public int MinutosCronometroConDescarga { get; set; }
        public int CantidadMaximaDiasNegocioConDescarga { get; set; }
        public int PorcentajeVolumenNegocioConDescarga { get; set; }
        public bool ExigirNegocioEnSolExt_Soja { get; set; }
        public bool ExigirNegocioEnSolExt_Maiz { get; set; }
        public bool ExigirNegocioEnSolExt_Trigo { get; set; }
        public bool ExigirNegocioEnSolExt_Girasol { get; set; }
        public string ApiKeyScoringCupos { get; set; }
    }
}