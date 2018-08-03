using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmFijacionDePrecioModel : Resultado
    {
        public DatosIniAbmFijacionDePrecio Datos { get; set; }
        public FijacionDePrecio FijacionDePrecio { get; set; }

        public DatosIniAbmFijacionDePrecioModel()
        {
            this.Datos = new DatosIniAbmFijacionDePrecio();
        }
    }


    public class ResultIniFijacionDePrecioModel : Resultado
    {
        public List<FijacionDePrecioIni> Datos { get; set; }

        public ResultIniFijacionDePrecioModel()
        {
            this.Datos = new List<FijacionDePrecioIni>();
        }
    }


    public class AbmFijacionDePrecioParam
    {
        public int FijacionId { get; set; }
    }


    public class AbmFijacionDePrecioResult : Resultado
    {
        public FijacionDePrecio FijacionDePrecio { get; set; }

        public AbmFijacionDePrecioResult()
        {
            this.FijacionDePrecio = new FijacionDePrecio();
        }
    }


}

