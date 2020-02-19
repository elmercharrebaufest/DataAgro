using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ProveedorNuevo
    {
        public string razonSocial { get; set; }
        public int Operable { get; set; }
        public string CUIT { get; set; }
        public string Condicion { get; set; }
        public int EstadoCuit { get; set; }
        public DateTime FechaVigenciaEstado { get; set; }
        public string RiesgoComercial { get; set; }
        public int Existe { get; set; }

        public ProveedorNuevo()
        {
            this.razonSocial = "";
            this.Operable = 0;
            this.CUIT = "";
            Condicion = String.Empty;
            RiesgoComercial = String.Empty;
            Existe = 0;
        }
    }
}
