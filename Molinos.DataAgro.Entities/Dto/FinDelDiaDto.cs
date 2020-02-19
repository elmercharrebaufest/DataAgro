using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class FinDelDiaDto
    {
        public bool? Cerrado { get; set; }
        public double? Diferencial { get; set; }
    }
}

