using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDataAgro.Models
{
    public class NotificacionModel
    {
        public int Id { get; set; }
        public int CampanaId { get; set; }
        public int MaterialId { get; set; }
        public int TipoResearchId { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public Resultado Resultado { get; set; }
        public List<NotificacionResearchDto> Notificaciones { get; set; }
        public NotificacionModel()
        {
            FechaDesde = DateTime.Now.Date;
            FechaHasta = DateTime.Now.AddDays(7).Date;
        }
    }
    
}