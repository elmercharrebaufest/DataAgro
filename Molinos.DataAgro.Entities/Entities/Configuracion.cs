using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Configuracion
    {
        [Key]
        public int Id { get; set; }
        public int CantidadDias { get; set; }
        public string ClaveStop { get; set; }
        public bool? ConexionABMStop { get; set; }
        public bool? ConexionConsultaStop { get; set; }
        public int TerminalStopId { get; set; }
        public string CuitDestinoStop { get; set; }
        public int CodigoLocalidadStop { get; set; }
        public decimal ImporteSustentable { get; set; }
        public decimal? ContratoAperturaPrecioPorcentajeDeComisionMaximo { get; set; }
        public int? DiasDiferimiento { get; set; }
        public int? CantidadAcuerdo { get; set; }
        public int CantidadDiasDolarizadoLimiteMaximo { get; set; }
        public int CantidadMaxima { get; set; }
        public int CantidadDiasPesificadoLimite { get; set; }
        public int RedespachoMaximoUSDM { get; set; }
        public int RedespachoMaximoARP { get; set; }
        public int ToleranciaPaseMin { get; set; }
        public int ToleranciaPaseMax { get; set; }
        public decimal ImporteSustentableEspecial { get; set; }



        public int AlgoritmoKilosMinimosParaSugerencia { get; set; }
    }
}
