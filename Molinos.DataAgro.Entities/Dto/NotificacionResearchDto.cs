using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class NotificacionResearchDto
    {
        public int Id { get; set; }
        public int CampanaId { get; set; }
        public int MaterialId { get; set; }
        public int TipoResearchId { get; set; }
        public string Mensaje { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string CampanaDescripcion { get; set; }
        public string TipoResearchDescripcion { get; set; }
        public string MaterialDescripcion { get; set; }
    }
}
   


