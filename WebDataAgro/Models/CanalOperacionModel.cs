using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmCanalOperacionModel : Resultado
    {
        public CanalOperacion CanalOperacion { get; set; }
    }


    public class ResultIniCanalOperacionModel : Resultado
    {
        public List<CanalOperacionIni> Datos { get; set; }

        public ResultIniCanalOperacionModel()
        {
            this.Datos = new List<CanalOperacionIni>();
        }
    }


    public class AbmCanalOperacionParam
    {
        public int CanalOperacionId { get; set; }
    }


    public class AbmCanalOperacionResult : Resultado
    {
        public CanalOperacionDto CanalOperacion { get; set; }

        public AbmCanalOperacionResult()
        {
            this.CanalOperacion = new CanalOperacionDto();
        }
    }


}

