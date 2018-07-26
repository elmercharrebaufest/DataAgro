using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Mapping.Context;

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
