using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class CampoDetalleTercero
    {
        [Key]
        public int Id { get; set; }
        public int IdMoa { get; set; }

        public int ProveedorId { get; set; }

        public int LocalidadId { get; set; }
        public int MaterialId { get; set; }

        public string Estado { get; set; }
        public string Latitud { get; set; }

        public string KMZnombre { get; set; }

        public string KMZfile { get; set; }

        public string Longitud { get; set; }

        public string Nombre { get; set; }

        public decimal? Rinde { get; set; }

        public decimal? HectareasTotales { get; set; }

        public decimal? HectareasCultivables { get; set; }

        public int CampañaId { get; set; }

        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }

}
