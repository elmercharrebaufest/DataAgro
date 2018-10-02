using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class RiesgoComercial
    {
        public string CUIT { get; set; }
        public string RiesgoComercialDesc { get; set; }

        public RiesgoComercial()
        {
            CUIT = String.Empty;
            RiesgoComercialDesc = String.Empty;
        }
    }
}
