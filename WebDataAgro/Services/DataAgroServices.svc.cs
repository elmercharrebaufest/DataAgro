using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;

namespace WebDataAgro.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataAgroServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DataAgroServices.svc or DataAgroServices.svc.cs at the Solution Explorer and start debugging.
    public class DataAgroServices : IDataAgroServices
    {
        private readonly ILogger logger;
        private readonly IRiesgoComercialManager riesgoComercial;
        private readonly ICampaniaActualManager campanaActual;
        private readonly ICampaniaMaterialManager campaniaMaterial;
        private readonly IInformeComercialManager informeComercial;

        //public DataAgroServices(ILogger logger, 
        //    IRiesgoComercialManager riesgoComercial,
        //    ICampaniaActualManager campanaActual, 
        //    ICampaniaMaterialManager campaniaMaterial, 
        //    IInformeComercialManager informeComercial)
        //{
        //    this.logger = logger;
        //    this.riesgoComercial = riesgoComercial;
        //    this.campanaActual = campanaActual;
        //    this.campaniaMaterial = campaniaMaterial;
        //    this.informeComercial = informeComercial;
        //}
        #region Servicios de DataAgro

        public Resultado GrabarRiesgoComercial(RiesgoComercial oRiesgos)
        {
            var oEntityErrors = new Resultado();

            try
            {
                logger.Debug("CUIT: " + oRiesgos.CUIT + " / Material: " + oRiesgos.RiesgoComercialDesc);
                var resultado = riesgoComercial.ActualizacionDeRiesgoComercial(oRiesgos);
            }
            catch (Exception ex)
            {
                oEntityErrors.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }

            return oEntityErrors;
        }

        public Resultado GrabarCampaniaActual(CampaniaActual oCampania)
        {
            try
            {
                logger.Debug("Campaña: " + oCampania.Campania + " / Material: " + oCampania.Material);
                return campanaActual.ActualizacionCampaniaActual(oCampania);
            }
            catch (Exception ex)
            {
                var resultado = new Resultado();
                resultado.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
                return resultado;
            }
        }

        public Resultado ActualizarCampaniaMaterial(List<CampaniaMaterialSAPDTO> oCampaniaMaterialSAP)
        {
            var oEntityErrors = new Resultado();

            try
            {
                string aux = string.Empty;

                foreach (var sap in oCampaniaMaterialSAP)
                {
                    aux = aux + "//  CUIT: " + sap.CUIT.ToString() + "/  Campania: " + sap.Campania.ToString() + "/  Material: " + sap.Material.ToString() + "/  Mes: " + sap.Mes.ToString()
                        + "/  Anio: " + sap.Anio.ToString() + "/  Toneladas: " + sap.Toneladas.ToString() + "/  Comercial: " + sap.Comercial.ToString() + "\r\n";
                }
                logger.Debug(aux);

                oEntityErrors = campaniaMaterial.TraerCampañasPorGrano(oCampaniaMaterialSAP);
            }
            catch (Exception ex)
            {
                oEntityErrors.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }

            return oEntityErrors;
        }

        public Resultado ActualizarEstadoComercial(List<InformeComercialSAPDTO> LoInformeComercialSAP)
        {
            var oEntityErrors = new Resultado();

            try
            {
                string aux = string.Empty;

                foreach (var oInformeComercialSAP in LoInformeComercialSAP)
                {
                    aux = aux + "//  CUIT: " + oInformeComercialSAP.CUIT.ToString() + "/  Material: " + oInformeComercialSAP.Material.ToString() + "/  MensajeSap: " + oInformeComercialSAP.RptSap.ToString() + "\\r\\n";
                    var rtaSap = String.Empty;
                    if (oInformeComercialSAP.RptSap.ToLower() != "ok")
                        rtaSap = oInformeComercialSAP.RptSap;

                    oEntityErrors = informeComercial.RespuestaDeSapCapacidadProductiva(oInformeComercialSAP.CUIT, oInformeComercialSAP.Material, rtaSap);
                }

                logger.Debug(aux);
            }
            catch (Exception ex)
            {
                oEntityErrors.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }

            return oEntityErrors;
        }

        #endregion
    }
}
