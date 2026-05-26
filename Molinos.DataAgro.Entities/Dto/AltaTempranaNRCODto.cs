using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class AltaTempranaNRCODto
    {
        public string AltaTemprana { get; set; }
        public string FechaActualizacion { get; set; }
        public string Nosis { get; set; }
        public string Bolsa { get; set; }
        public string PlanCanje { get; set; }
        public string Consignatario { get; set; }
        public Ruca Ruca { get; set; }
        public string SinOblea { get; set; }
        public string Carta { get; set; }
        public string Mensaje { get; set; }
        public string ProveedorGrano { get; set; }
        public string BoletoFisico { get; set; }

        public string PeticionBorradoGral { get; set; }
        public string PeticionBorradoSociedad { get; set; }
        public string BloqueoProveedorGral { get; set; }
        public string BloqueoProveedorSociedad { get; set; }
        public string RiesgoComercial { get; set; }
        public string AuthGralMP { get; set; }
        public string AuthSociedadMP { get; set; }
        public string BloqueoProveedor { get; set; }
        public string FechaActualizacionLegajo { get; set; }
        public List<HistoricoFechaActualizacionLegajo> HistoricoFechaActualizacionLegajo { get; set; }
    }
    public partial class Ruca
    {
        public ValoresRuca Otros { get; set; }
        public ValoresRuca Acopiador { get; set; }
        public string Corredor { get; set; }
        public string Fason { get; set; }
    }
    public partial class ValoresRuca
    {
        public string Consignatario { get; set; }
        public string PlanCanje { get; set; }
        public string Directo { get; set; }
    }

    public partial class HistoricoFechaActualizacionLegajo
    {
        public string Material { get; set; }
        public string Cosecha { get; set; }
        public string FechaAtualizacion { get; set; }
    }
}



