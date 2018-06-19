using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Business;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.DataRepository;
namespace InicializarCompras
{
    class Program
    {
        static void Main(string[] args)
        {
            ComprasManager cm = new ComprasManager();
            cm.Inicializar(Molinos.DataAgro.Process.Util.GetMSContext());
            cm.ArmarCargaInicial();
        }
        


    }
}
