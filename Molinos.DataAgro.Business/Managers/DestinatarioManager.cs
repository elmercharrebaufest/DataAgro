using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Diagnostics;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Mapping.Context;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business
{
    public class DestinatarioManager : IDestinatarioManager
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Inicialización
        //--------------------------------------------------

        public void Inicializar(MSContext oContexto)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = new UnitOfWork(oContexto, new DataAgroContext(oContexto));
        }


        public void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = oUnitOfWork;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<ResultIniDestinatario> TraerTodoDestinatarioAsync()
        {
            var oResult = new ResultIniDestinatario();
           
            var oDestinatario = mobjUnitOfWork.Repository<Destinatario>().Queryable();

            var query = oDestinatario
                        .OrderBy(x => x.Descripcion)
                        .Select(x => new DestinatarioIni()
                        {
                             DestinatarioId = x.DestinatarioId,
                             Descripcion = x.Descripcion
                        });

            oResult.Destinatario = await query.ToListAsync();

            return oResult;
        }


        public async Task<Destinatario> TraerDestinatarioAsync(int intDestinatarioId)
        {
            var oDestinatario = new Destinatario();

            oDestinatario = await mobjUnitOfWork.Repository<Destinatario>()
                                 .Queryable()
                                 .Where(x => x.DestinatarioId == intDestinatarioId)
                                 .SingleOrDefaultAsync();

            if (oDestinatario == null)
            {
                oDestinatario = new Destinatario()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oDestinatario.ObjectState = Constants.Object_Modified;
            }
            
            return oDestinatario;
        }


        public async Task<EntityErrors> GrabarDestinatarioAsync(Destinatario oDestinatario)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oDestinatario, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            Destinatario oDestinatarioSave;

            if (oDestinatario.ObjectState == 0)
            {
                oDestinatarioSave = new Destinatario()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oDestinatarioSave = await TraerDestinatarioAsync(oDestinatario.DestinatarioId);
            }

            var validacion = ValidarDescripcion(oDestinatario, oDestinatarioSave.ObjectState,
                (oDestinatarioSave.ObjectState == Constants.Object_Modified ? (int?)oDestinatario.DestinatarioId : null));

            if (validacion != null)
            {
                oEntityErrors.ListaErrores.Add(validacion);
                return oEntityErrors;
            }
         
            oDestinatarioSave.Descripcion = oDestinatario.Descripcion;
            oDestinatarioSave.Inhabilitado = oDestinatario.Inhabilitado;
            if (oDestinatarioSave.ObjectState == Constants.Object_Added)
            {
                oDestinatarioSave.DestinatarioId = ((mobjUnitOfWork.Repository<Destinatario>().Queryable().Max(x => (int?)x.DestinatarioId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<Destinatario>().SaveEntity(oDestinatarioSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarDestinatarioAsync(int intDestinatarioId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Destinatario>();

            var oDestinatario = await oRepository
                                 .Queryable()
                                 .Where(x => x.DestinatarioId == intDestinatarioId)
                                 .SingleOrDefaultAsync();

            if (oDestinatario != null)
            {
                oRepository.Delete(oDestinatario);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        #region Validar
        public ErrorMessage ValidarDescripcion(Destinatario oDestinatarios, int ObjectState, int? Id = null)
        {
            var oDestinatario = mobjUnitOfWork.Repository<Destinatario>().Queryable();

            Destinatario val = null;

            if (ObjectState == Constants.Object_Added)
            {
                val = oDestinatario
                    .Where(x => x.Descripcion == oDestinatarios.Descripcion).FirstOrDefault();
            }
            else if (ObjectState == Constants.Object_Modified)
            {
                val = oDestinatario
                    .Where(x=> x.Descripcion == oDestinatarios.Descripcion
                    && x.DestinatarioId != Id).FirstOrDefault();
            }

            if (val != null)
            {
                return new ErrorMessage() { Message = "Existe un registro de iguales carecteristicas." };
            }
            return null;
        } 
        #endregion

    }
}




