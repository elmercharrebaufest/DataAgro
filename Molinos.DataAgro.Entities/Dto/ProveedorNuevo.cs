using Mastersoft.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ProveedorNuevo : Entity
    {
        public string razonSocial { get; set; }
        public int Operable { get; set; }
        public string CUIT { get; set; }
        public string Condicion { get; set; }
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
