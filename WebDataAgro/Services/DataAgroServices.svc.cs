using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataAgroServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DataAgroServices.svc or DataAgroServices.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DataAgroServices : IDataAgroServices
    {
        private ILogger logger;
        private IRiesgoComercialManager riesgoComercial;
        private ICampaniaActualManager campanaActual;
        private ICampaniaMaterialManager campaniaMaterial;
        private IInformeComercialManager informeComercial;

        public DataAgroServices(ILogger logger, 
            IRiesgoComercialManager riesgoComercial,
            ICampaniaActualManager campanaActual, 
            ICampaniaMaterialManager campaniaMaterial, 
            IInformeComercialManager informeComercial)
        {
            this.logger = logger;
            this.riesgoComercial = riesgoComercial;
            this.campanaActual = campanaActual;
            this.campaniaMaterial = campaniaMaterial;
            this.informeComercial = informeComercial;
        }
        #region Servicios de DataAgro

        public ResultadoSap Ping()
        {
            return new ResultadoSap();
        }

        public ResultadoSap GrabarRiesgoComercial(RiesgoComercial oRiesgos)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("GrabarRiesgoComercial" + oRiesgos.ToXml());
                var resultado = riesgoComercial.ActualizacionDeRiesgoComercial(oRiesgos);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap GrabarCampaniaActual(CampaniaActual oCampania)
        {
            var oEntityErrors = new ResultadoSap();

            try
            {
                logger.Debug("GrabarCampaniaActual " + oCampania.ToXml());
                var resultado = campanaActual.ActualizacionCampaniaActual(oCampania);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap ActualizarCampaniaMaterial(List<CampaniaMaterialSAPDTO> oCampaniaMaterialSAP)
        {
            var oEntityErrors = new ResultadoSap();

            try
            {
                logger.Debug("ActualizarCampaniaMaterial " + oCampaniaMaterialSAP.ToXml());
                string aux = string.Empty;

                foreach (var sap in oCampaniaMaterialSAP)
                {
                    aux = aux + "//  CUIT: " + sap.CUIT.ToString() + "/  Campania: " + sap.Campania.ToString() + "/  Material: " + sap.Material.ToString() + "/  Mes: " + sap.Mes.ToString()
                        + "/  Anio: " + sap.Anio.ToString() + "/  Toneladas: " + sap.Toneladas.ToString() + "/  Comercial: " + sap.Comercial.ToString() + "\r\n";
                }
                logger.Debug(aux);

                oEntityErrors.ListaErrores.AddRange(campaniaMaterial.TraerCampañasPorGrano(oCampaniaMaterialSAP).Errores);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap ActualizarEstadoComercial(List<InformeComercialSAPDTO> LoInformeComercialSAP)
        {
            var oEntityErrors = new ResultadoSap();

            try
            {
                logger.Debug("ActualizarEstadoComercial " + LoInformeComercialSAP.ToXml());

                string aux = string.Empty;

                foreach (var oInformeComercialSAP in LoInformeComercialSAP)
                {
                    aux = aux + "//  CUIT: " + oInformeComercialSAP.CUIT.ToString() + "/  Material: " + oInformeComercialSAP.Material.ToString() + "/  MensajeSap: " + oInformeComercialSAP.RptSap.ToString() + "\\r\\n";
                    var rtaSap = String.Empty;
                    if (oInformeComercialSAP.RptSap.ToLower() != "ok")
                        rtaSap = oInformeComercialSAP.RptSap;

                    oEntityErrors.ListaErrores.AddRange(informeComercial.RespuestaDeSapCapacidadProductiva(oInformeComercialSAP.CUIT, oInformeComercialSAP.Material, rtaSap).ListaErrores);
                }

                logger.Debug(aux);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        #endregion
    }
}
