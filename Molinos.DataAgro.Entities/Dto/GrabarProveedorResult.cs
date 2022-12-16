using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class GrabarProveedorResult : Resultado
    {
        public int? ProveedorId { get; set; }
        public List<string> DownloadKey { get; set; } = new List<string>();

    }
}
