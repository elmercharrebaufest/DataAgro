using Mastersoft.Framework.Standard;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class AbmContacto : IEntityKeyValid
    {
        //--------------------------------------------------------------------------------
        //   Implementacion de IEntityValid
        //--------------------------------------------------------------------------------

        public bool ValidateKey(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }


        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            //if (String.IsNullOrWhiteSpace(this.)
            //{
            //    oErrorMessages.Add(new ErrorMessage("El campo 'Descripción' no debe estar vacio", "Descripcion"));
            //}

            return oErrorMessages.Count == 0;
        }

    }

        public class ResultIniContacto
        {
            public List<ContactoIni> Contactos { get; set; }
        }


        public class ContactoIni
        {       
        public int ProveedorId { get; set; }
        public int? Calificacion { get; set; }
        [DisplayName("Razon Social")]
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }
        public string Mail { get; set; }
        public string Estado { get; set; }
        public bool Operando { get; set; }
        public string Telefono { get; set; }
        [DisplayName("Comercial a Cargo")]
        public string ComercialCargo { get; set; }
        [DisplayName("Ultimo Contacto")]
        public string UltimoContacto { get; set; }
        [DisplayName("No Operable")]
        public bool NoOperable { get; set; }
        [DisplayName("Condiciones")]
        public string TooltipNoOperable { get; set; }
        public string RptOpera { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string GrupoDeCompras { get; set; }

    }

    public class ContactoExcel
    {
        [DisplayName("Razon Social")]
        public string RazonSocial { get; set; }
        public int? Calificacion { get; set; }
        public string Cuit { get; set; }      
        public string Mail { get; set; }
        public string Estado { get; set; }       
        public string Telefono { get; set; }        
        [DisplayName("Ultimo Contacto")]
        public string UltimoContacto { get; set; }
        public string Operable { get; set; }
        [DisplayName("Condiciones")]
        public string Condicion { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string GrupoDeCompras { get; set; }
        public string ComercialCargo { get; set; }

    }





}

