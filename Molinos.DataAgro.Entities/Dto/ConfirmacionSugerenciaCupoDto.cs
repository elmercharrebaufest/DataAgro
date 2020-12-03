using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConfirmacionSugerenciaCupoDto
    {
        public List<DiaCupo> Detalles { get; set; }
        public List<DiaCupo> Devoluciones { get; set; }

        public int ProveedorId { get; set; }
        public int ComercialId { get; set; }
        public string ProveedorDesc { get; set; }

    }
}


