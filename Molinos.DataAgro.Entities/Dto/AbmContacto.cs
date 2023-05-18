using Molinos.DataAgro.Entities.Validations;
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

        public bool ValidateKey(Resultado oErrorMessages)
        {
            return oErrorMessages.HayErrores;
        }


        public bool Validate(Resultado oErrorMessages)
        {
            return oErrorMessages.HayErrores;
        }

    }

    public class ResultIniContacto
    {
        public List<ContactoIni> Contactos { get; set; }
        public int TotalContactos { get; set; }
        public int TotalPotencialContactos { get; set; }
        public int TotalOperandoContactos { get; set; }
        public int TotalNoOperandoContactos { get; set; }
        public int TotalBajaContactos { get; set; }
        public int TotalSinInteresContactos { get; set; }
        public int TotalHabilitadoContactos { get; set; }
        public int TotalLegajoIrregularContactos { get; set; }
        public int TotalNoHabilitadoContactos { get; set; }
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
        public bool Corredor { get; set; }
        public int? EstadoHomeId { get; set; }
        public string EstadoHomeMensaje { get; set; }
        public string EstadoHomeDescripcion { get; set; }
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