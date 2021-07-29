using Molinos.DataAgro.Entities.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto
{
    public class EstadoPrecioMOADto
    {
        public int Id { get; set; }      
        public int MaterialId { get; set; }
        public string Descripcion { get; set; }      
        public bool? Habilitado { get; set; }
        public int TipoNegocioId { get; set; }
    }
}
