using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business.Managers
{
    public class RG2300Manager : IRG2300Manager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public RG2300Manager(ILogger logger, IMSContextProvider oMSContextProvider, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        public bool InsetarRG2300 (List<RG2300> oDatos, int cantProcesar)
        {

            try
            {
                RG2300 oRg2300;

                if(cantProcesar == 1000)
                 { 
                    mobjUnitOfWork.Repository<RG2300>().DeleteWhere( x => x != null);
                }

                int i = 0;
                i = cantProcesar - 1000;
                int j = 1;
                int cantRecorridos = 1;

                    while (i < cantProcesar)
                    {
                        if (i < oDatos.Count)
                        {
                            oRg2300 = oDatos[i];
                            oRg2300.Id = i + 1;
                            mobjUnitOfWork.Repository<RG2300>().SaveEntity(oRg2300);
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
