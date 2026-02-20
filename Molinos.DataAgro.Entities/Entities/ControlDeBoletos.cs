using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ControlDeBoletos
{
    [Key]
    public int Id { get; set; }

    public int NegocioId { get; set; }
    public int ControlDeBoletosEstadoId { get; set; }

    public int? EstadoConfirmaId { get; set; }
    public bool EsConfirma { get; set; }

    public int? AltaIdLoteConfirma { get; set; }
    public int? IdentificadorConfirma { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }

    public bool ControlIniciado { get; set; }
    public bool ControlFinalizado { get; set; }
    public bool CertificacionCompletada { get; set; }
    public bool RegistroDatosOblea { get; set; }

    public DateTime? FechaControlIniciado { get; set; }
    public DateTime? FechaControlFinalizado { get; set; }
    public DateTime? FechaCertificacionCompletada { get; set; }
    public DateTime? FechaRegistroDatosOblea { get; set; }

    // 🔹 Navigation Properties
    public virtual Negocio Negocio { get; set; }
    public virtual ControlDeBoletosEstado ControlDeBoletosEstado { get; set; }
}
