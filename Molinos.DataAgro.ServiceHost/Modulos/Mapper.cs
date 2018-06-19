using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Molinos.DataAgro.ServiceHost.Modulos
{
    public class Mapper
    {

        public CampañaActual DevolverCampañaActual(CampaniaActual cma)
        {
            CampañaActual ca = new CampañaActual();
            ca.Campaña = cma.Campania;
            ca.Material = cma.Material;
            return ca;
        }

        public List<CampañaMaterialSAPDTO> DevolverCampañaMaterial(List<CampaniaMaterialSAPDTO> campsap)
        {
            List<CampañaMaterialSAPDTO> cms = new List<CampañaMaterialSAPDTO>();
           
            foreach (var camp in campsap)
            {
                /*
                double? x = null;
                if (String.IsNullOrEmpty(camp.Toneladas))
                    x = 0;
                else if (!double.TryParse(camp.Toneladas.Trim(), out val)){
                    x = 0;
                }
                else
                {
                    x = double.Parse(camp.Toneladas);
                }
                
    */
                CampañaMaterialSAPDTO cm = new CampañaMaterialSAPDTO();
                cm.Año = camp.Anio;
                cm.Campaña = camp.Campania;
                cm.Comercial = camp.Comercial;
                cm.CUIT = camp.CUIT;
                cm.Material = camp.Material;
                cm.Mes = camp.Mes;
                cm.Toneladas = camp.Toneladas;
                cms.Add(cm);
            }
            return cms;
        }
    }
}