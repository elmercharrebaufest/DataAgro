using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CampañaDto
    {
        public int CampañaId { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSIO { get; set; }
    }

    public partial class CampaniaDto
    {
        public List<int> CampaniaId { get; set; }
        public List<string> CampaniaDesc { get; set; }
        public int? ComercialId { get; set; }
    }
}
   


