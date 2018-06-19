
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Reportes : Entity
    {
        public string Identificador { get; set; }
        public byte[] Contenido { get; set; }
        public string FileName { get; set; }

        public Reportes()
        {

        }
    }
}
