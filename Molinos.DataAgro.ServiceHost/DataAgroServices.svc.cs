using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Interfaces;
using System.Threading.Tasks;
using Molinos.DataAgro.DataContracts;
using Molinos.DataAgro.ServiceHost.Modulos;
using System.Configuration;
using System.IO;

namespace Molinos.DataAgro.ServiceHost
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "DataAgroServices" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione DataAgroServices.svc o DataAgroServices.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class DataAgroServices : IDataAgroServices
    {

        #region Servicios de DataAgro

        public EntityErrors GrabarRiesgoComercial(RiesgoComercial oRiesgos)
        {
            var oEntityErrors = new EntityErrors();

            try
            {
                string aux = "CUIT: " + oRiesgos.CUIT + " / Material: " + oRiesgos.RiesgoComercialDesc;

                Util.GarbarArchivo("RiesgoComercial.txt", aux);

                var oManager = Factories.GetRiesgoComercialManager();

                var task = oManager.ActualizacionDeRiesgoComercialAsync(oRiesgos);

                task.Wait();

                if (task.Exception == null)
                {
                    oEntityErrors = task.Result;
                }
                else
                {
                    oEntityErrors.ListaErrores.Add(new ErrorMessage()
                    {
                        Message = task.Exception.Message
                    });
                }
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }

            return oEntityErrors;
        }

        public EntityErrors GrabarCampaniaActual(CampaniaActual oCampania)
        {
            var oEntityErrors = new EntityErrors();

            try
            {
                var oManager = Factories.GetCampañaActualManager();

                string aux = "Campaña: " + oCampania.Campania + " / Material: " + oCampania.Material;

                Util.GarbarArchivo("Campaña.txt", aux);

                var map = new Mapper();

                var task = oManager.ActualizacionCampañaActualAsync(map.DevolverCampañaActual(oCampania));

                task.Wait();

                if (task.Exception == null)
                {
                    oEntityErrors = task.Result;
                }
                else
                {
                    oEntityErrors.ListaErrores.Add(new ErrorMessage()
                    {
                        Message = task.Exception.Message
                    });
                }
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }

            return oEntityErrors;
        }

        public EntityErrors ActualizarCampaniaMaterial(List<CampaniaMaterialSAPDTO> oCampaniaMaterialSAP)
        {
            var oEntityErrors = new EntityErrors();
            
            try
            {
                var oManager = Factories.GetCampañaMaterial();

                string aux = string.Empty;

                foreach (var sap in oCampaniaMaterialSAP)
                {
                    aux = aux + "//  CUIT: " + sap.CUIT.ToString() + "/  Campania: " + sap.Campania.ToString() + "/  Material: " + sap.Material.ToString() + "/  Mes: " + sap.Mes.ToString()
                        + "/  Anio: " + sap.Anio.ToString() + "/  Toneladas: " + sap.Toneladas.ToString() + "/  Comercial: " + sap.Comercial.ToString() + "\r\n" ;
                }

                Util.GarbarArchivo("CampañaMaterial.txt", aux);

                var map = new Mapper();

                var task = oManager.TraerCampañasPorGrano(map.DevolverCampañaMaterial(oCampaniaMaterialSAP));

                task.Wait();

                if (task.Exception == null)
                {
                    oEntityErrors = task.Result;
                }
                else
                {
                    oEntityErrors.ListaErrores.Add(new ErrorMessage()
                    {
                        Message = task.Exception.Message
                    });
                }
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }

            return oEntityErrors;
        }

        public EntityErrors ActualizarEstadoComercial (List<InformeComercialSAPDTO> LoInformeComercialSAP)
        {
            var oEntityErrors = new EntityErrors();

            try
            {

                string aux = string.Empty;

                foreach (var oInformeComercialSAP in LoInformeComercialSAP) {

                    var oManager = Factories.GetInformeComercial();

                    
                    aux = aux + "//  CUIT: " + oInformeComercialSAP.CUIT.ToString() + "/  Material: " + oInformeComercialSAP.Material.ToString() + "/  MensajeSap: " + oInformeComercialSAP.RptSap.ToString() + "\\r\\n";
                    
                    var map = new Mapper();

                    var rtaSap = String.Empty;
                    if (oInformeComercialSAP.RptSap.ToLower()!="ok")
                        rtaSap = oInformeComercialSAP.RptSap;

                    var task = oManager.RespuestaDeSapCapacidadProductivaAsync(oInformeComercialSAP.CUIT, oInformeComercialSAP.Material, rtaSap);

                    task.Wait();

                    if (task.Exception == null)
                    {
                        oEntityErrors = task.Result;
                    }
                    else
                    {
                        oEntityErrors.ListaErrores.Add(new ErrorMessage()
                        {
                            Message = task.Exception.Message
                        });
                    }
                }

                Util.GarbarArchivo("InformeComercial.txt", aux);

            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }

            return oEntityErrors;
        }

        #endregion
    }
}
