using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ConfiguracionBolsaDto
    {
        public int Id { get; set; }
        public int DestinoId { get; set; }
        public string Destino { get; set; }
        public int ProvinciaId { get; set; }
        public string Provincia { get; set; }
        public int BolsaId { get; set; }
        public string Bolsa { get; set; }

    }
}