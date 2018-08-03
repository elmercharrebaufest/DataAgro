using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business
{
    public class CampañaActualManager : ICampañaActualManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public CampañaActualManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------
        public Resultado ActualizacionCampañaActual(CampañaActual oParam)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oParam, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            var material = repositorio.Obtener<Material>(x => x.Codigo == oParam.Material);
            if (material == null)
            {
                oEntityErrors.Error("Material", "No existe el material.");
                return oEntityErrors;
            }

            string[] arrAux = oParam.Campaña.Split('-');
            for (int i = 0; i < arrAux.Length; i++)
            {
                arrAux[i] = (Convert.ToInt32(arrAux[i]) + 1).ToString();
            }
            string campaniaSiguiente = string.Join("-", arrAux);
            if (!repositorio.Existe<Campaña>(x => x.Descripcion == campaniaSiguiente))
            {
                repositorio.Agregar(new Campaña()
                {
                    Descripcion = campaniaSiguiente
                });
            }

            var campania = repositorio.Obtener<Campaña>(x => x.Descripcion == oParam.Campaña);

            if (campania == null)
            {
                campania = repositorio.Agregar(new Campaña()
                {
                    Descripcion = oParam.Campaña
                });
            }

            repositorio.Agregar(new CampañaMaterialHistorico()
            {
                Campaña = campania,
                Fecha = DateTime.Now,
                Material = material
            });
            material.Campaña = campania;

            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }


            return oEntityErrors;
        }
    }
}
