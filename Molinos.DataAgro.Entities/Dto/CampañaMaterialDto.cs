using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CampañaMaterialDto
    {
        public int CampañaMaterialId { get; set; }
        public int CampañaId { get; set; }
        public int NroItem { get; set; }
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public double ToneladasCompradas { get; set; }
    }
}

