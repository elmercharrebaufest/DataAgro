using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public class PostIt : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ComercialId { get; set; }
        public String Texto { get; set; }

        public PostIt()
        {
        }
    }
}
