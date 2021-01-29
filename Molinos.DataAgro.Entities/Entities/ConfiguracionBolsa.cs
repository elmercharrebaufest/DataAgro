using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ConfiguracionBolsa
    {
        [Key]
        public int Id { get; set; }
      
        public int DestinoId { get; set; }
        public int BolsaId { get; set; }
        public int ProvinciaId { get; set; }

        [ForeignKey("DestinoId")]
        public virtual Centro Destino { get; set; }      
       
        [ForeignKey("BolsaId")]
        public virtual BolsaCompraNet Bolsa { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }

     
    }
}



