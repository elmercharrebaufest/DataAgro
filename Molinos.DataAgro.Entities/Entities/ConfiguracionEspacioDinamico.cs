using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ConfiguracionEspacioDinamico
    {
        [Key]
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public DateTime Fecha { get; set; }
        public int ProveedorId { get; set; }
        public int CantidadDeCupo { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        public int ComercialId { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        public string Calidad { get; set; }
    }
}
