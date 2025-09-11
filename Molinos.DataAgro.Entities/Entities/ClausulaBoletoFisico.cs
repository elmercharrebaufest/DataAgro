using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Molinos.DataAgro.Entities.Entities
{
    public abstract class ClausulaBoletoFisico
    {
        [Key]
        public int Id { get; set; }

        public int Orden { get; set; }
        public bool Estado { get; set; }
        public string Clausula { get; set; }


        public string Descripcion
        {
            get
            {
                return this.GetType().BaseType.Name;
            }
        }
        
        public static Type[] TiposDeComandos()
        {
            var tipoComando = typeof(ClausulaBoletoFisico);
            return tipoComando.Assembly.GetTypes().Where(tipoComando.IsAssignableFrom).ToArray();
        }
        [NotMapped]
        public BasicoContrato Basico { get; set; }

        //[NotMapped]
        //public decimal Puntuacion { get; set; }

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
