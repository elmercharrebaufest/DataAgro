using Mastersoft.Framework.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Process
{
    public static class Util
    {
        //--------------------------------------------------
        //  Constantes Privadas
        //--------------------------------------------------

        private const string DefaultCNPrefix = "DataAgro";

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public static MSContext GetMSContext()
        {
            var oMSContext = new MSContext()
            {
                CN = "",
                CNPrefix = DefaultCNPrefix,
                DBProvider = "SQLServer",
                EmpresaId = 1,
                UsuarioId = 1,
                Usuario = "",
                PerfilId = 0
            };

            return oMSContext;
        }
    }
}
