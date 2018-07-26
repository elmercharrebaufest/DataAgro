using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business
{
    public class FacacopManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public FacacopManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        public bool InsetarFacacop(List<FACACOP> oDatos, int cantProcesar)
        {
            try
            {
                FACACOP oFacacop;

                if (cantProcesar == 1000)
                {
                    mobjUnitOfWork.Repository<FACACOP>().DeleteWhere(x => x != null);
                }

                int i = 0;
                i = cantProcesar - 1000;
                int j = 1;
                int cantRecorridos = 1;

                while (i < cantProcesar)
                {
                    if (i < oDatos.Count)
                    {
                        oFacacop = oDatos[i];
                        oFacacop.Id = i + 1;
                        mobjUnitOfWork.Repository<FACACOP>().SaveEntity(oFacacop);
                        i++;
                        j++;
                    }
                    else
                    {
                        mobjUnitOfWork.SaveChanges();
                        return false;
                    }
                }
                mobjUnitOfWork.SaveChanges();
                Console.WriteLine("Ya procesamos  = " + i);

                cantRecorridos += 1;
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
