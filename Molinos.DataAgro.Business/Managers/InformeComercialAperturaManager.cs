using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class InformeComercialAperturaManager : IInformeComercialAperturaManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly int DIAS_MAXIMO_APERTURA_INFORME = Convert.ToInt32(ConfigurationManager.AppSettings["DiasMaximoAperturaInformeComercial"]);

        public InformeComercialAperturaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public Resultado GrabarInformeComercialApertura(InformeComercialApertura oInformeComercialApertura)
        {
            var oEntityErrors = new Resultado();
            EntityValid.ValidateAll(oInformeComercialApertura, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            try
            {
                repositorio.Agregar(oInformeComercialApertura);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return oEntityErrors;
        }

        public InformeComercialAperturaDto TraerInformeComercialApertura(int ComercialId)
        {
            var fechaHoy = DateTime.Now.Date;
            var informeComercialApertura = new InformeComercialAperturaDto();
            var listarAperturas = repositorio.Listar<InformeComercialApertura>(x => x.ComercialId == ComercialId).OrderByDescending(x=> x.FechaApertura).FirstOrDefault();
            if (listarAperturas != null)
            {
                var resultado = Convert.ToInt32((fechaHoy - listarAperturas.FechaApertura).TotalDays) == DIAS_MAXIMO_APERTURA_INFORME ? listarAperturas : null;
                if (resultado != null)
                {
                    informeComercialApertura.Id = resultado.Id;
                    informeComercialApertura.ComercialId = resultado.ComercialId;
                    informeComercialApertura.FechaApertura = resultado.FechaApertura;
                }
            }
            else
            {
                informeComercialApertura.Id = -1;
            }
            return informeComercialApertura;

        }
    }
}
