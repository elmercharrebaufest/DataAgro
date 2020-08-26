using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultadoValidarProveedorComercial
    {
        public ResultadoValidarProveedorComercial()
        {
            ListaErrores = new List<ErrorMessage>();
        }

        public List<ErrorMessage> ListaErrores { get; set; }
        
        public bool HayError { get; set; }
        public int ComercialId { get; set; }
        public string ComercialApellido { get; set; }
        public string ComercialNombres { get; set; }
        public string ComercialMail { get; set; }
        public List<string> ProveedorMails { get; set; }
        public int? ProveedorId { get; set; }
        public string ProveedorRazonSocial { get; set; }
        public bool ProveedorOperable { get; set; }
        public string ProveedorCBU { get; set; }
        public string ProveedorClasificacion { get; set; }
        public string ProveedorSISAEstadoCuit { get; set; }       
        public string ProveedorSISASituacionCategoria { get; set; }
        public string ProveedorSISACodCategoria { get; set; }
        public bool ProveedorOperando { get; set; }
        public DateTime? ProveedorUltimaOperacion { get; set; }
    }
}
