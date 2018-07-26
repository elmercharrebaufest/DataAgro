
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Errores : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ErrorId { get; set; }

        public Nullable<System.DateTime> ErrorDateTime { get; set; }
        public string MachineName { get; set; }
        public string AppDomainName { get; set; }
        public string ThreadIdentity { get; set; }
        public string WindowsIdentity { get; set; }
        public string Message { get; set; }
        public string FullException { get; set; }

        public Errores()
        {

        }
    }
}
