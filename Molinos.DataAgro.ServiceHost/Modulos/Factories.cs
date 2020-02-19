
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;

namespace Molinos.DataAgro.ServiceHost
{
    public class Factories
    {
        public static IDestinatarioManager GetDestinatarioManager()
        {
            var result = new DestinatarioManager();
            
            result.Inicializar(Util.GetMSContext());

            return result;
        }

        
        public static IRiesgoComercialManager GetRiesgoComercialManager()
        {
            var result = new RiesgoComercialManager();

            result.Inicializar(Util.GetMSContext());

            return result;
        }

        public static ICampañaActualManager GetCampañaActualManager()
        {
            var result = new CampañaActualManager();

            result.Inicializar(Util.GetMSContext());

            return result;
        }

        public static ICampañaMaterial GetCampañaMaterial()
        {
            var result = new CampañaMaterialManager();

            result.Inicializar(Util.GetMSContext());

            return result;
        }

        public static IInformeComercialManager GetInformeComercial()
        {
            var result = new InformeComercialManager();

            result.Inicializar(Util.GetMSContext());

            return result;
        }

    }
}