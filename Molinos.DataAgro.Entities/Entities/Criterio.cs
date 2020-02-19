using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Molinos.DataAgro.Entities.Entities
{
    public abstract class Criterio
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Padre")]
        public int? PadreId { get; set; }
        
        public Criterio Padre { get; set; }

        [InverseProperty("Padre")]
        public virtual ICollection<Criterio> Hijos { get; set; }

        public int Prioridad { get; set; }

        public string Descripcion
        {
            get
            {
                return this.GetType().BaseType.Name;
            }
        }
        [NotMapped]
        public virtual bool Concreta { get; set; }



        public static Type[] TiposDeComandos()
        {
            var tipoComando = typeof(Criterio);
            return tipoComando.Assembly.GetTypes().Where(tipoComando.IsAssignableFrom).ToArray();
        }
        [NotMapped]
        public SugerenciaCupoDto Dto { get; set; }

        [NotMapped]
        public decimal Puntuacion { get; set; }

        //public static string GetDisplayName()
        //{
        //    var displayName = typeof(CriterioRaiz).GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
        //    if (displayName != null)
        //        return displayName.DisplayName;
        //    return "";
        //}
        public string DisplayName
        {
            get
            {
                var displayName = this.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
                if (displayName != null)
                    return displayName.DisplayName;
                return "";
            }
        }
    }
}
