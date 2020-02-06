using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ConfiguracionEspacioDinamicoDto
    {
        public int Id { get; set; }
        public int CentroId { get; set; }
        public string Centro { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadDeCupo { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorCUIT { get; set; }
        public string ProveedorRazonSocial { get; set; }
        public string Calidad { get; set; }
        public int ComercialId { get; set; }
        public string ComercialNombre { get; set; }

    }
}