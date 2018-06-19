using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Data.Entity;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Mapping.Context;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business
{
    public class ErroresManager
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Constructor
        //--------------------------------------------------

        public ErroresManager(MSContext oContexto)
        {
            mobjUnitOfWork = new UnitOfWork(oContexto, new DataAgroContext(oContexto));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public void Grabar(Errores oErrores)
        {
            mobjUnitOfWork.Repository<Errores>().Insert(oErrores);

            mobjUnitOfWork.SaveChanges();
        }

    }
}
