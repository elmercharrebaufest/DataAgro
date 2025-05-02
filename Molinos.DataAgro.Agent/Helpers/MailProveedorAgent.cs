using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ObtenerMailProveedor;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class MailProveedorAgent : IMailProveedorAgent
    {
        public MailProveedorAgent(ILogger logger)
        {
            this.logger = logger;
        }
        readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public List<MailProveedorDto> Ejecutar(List<string> cuits)
        {
            try
            {
                logger.Debug("Obtener MailProveedor y direcciones SAP");

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var request = new ZMprfcObtenerMails
                    {
                        ImCuit = CrearListaSinPi(cuits).ToArray()
                    };

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var devolucion = agent.ZMprfcObtenerMails(request);
                    var lista = new List<MailProveedorDto>();
                    foreach (var item in devolucion.ExSalida)
                    {
                        lista.Add(new MailProveedorDto()
                        {
                            Cuit = item.Cuit,
                            Pesificado = (!(item.Remark.ToUpper().Contains("BOLETO") ||
                            item.Remark.ToUpper().Contains("CUPO") || item.Remark.ToUpper().Contains("NDNCDIFTC")) || item.Flgdefault == "X") ? item.Mail : "",
                            DireccionSap = item.Direccion.Direccion,
                            LocalidadSap = item.Direccion.Localidad,
                            ProvinciaSap = item.Direccion.Provincia,
                            CodigoPostalSap = item.Direccion.CodigoPostal
                        });
                    }
                    logger.Debug(devolucion.ToXml());

                    logger.Debug("Sin Error");
                    return lista;
                }
                else
                {
                    var request = new Z_MPRFC_OBTENER_MAILS
                    {
                        IM_CUIT = CrearLista(cuits).ToArray()
                    };
                    var agent = new SI_ZMPWS_DATAAGRO_OBTENER_MAILSClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    logger.Debug(request.ToXml());
                    var devolucion = agent.SI_ZMPWS_DATAAGRO_OBTENER_MAILS(request);
                    var lista = new List<MailProveedorDto>();
                    foreach (var item in devolucion.EX_SALIDA)
                    {
                        lista.Add(new MailProveedorDto()
                        {
                            Cuit = item.CUIT,
                            Pesificado = (!(item.REMARK.ToUpper().Contains("BOLETO") ||
                            item.REMARK.ToUpper().Contains("CUPO") || item.REMARK.ToUpper().Contains("NDNCDIFTC")) || item.FLGDEFAULT == "X") ? item.MAIL : "",
                            DireccionSap = item.DIRECCION.DIRECCION,
                            LocalidadSap = item.DIRECCION.LOCALIDAD,
                            ProvinciaSap = item.DIRECCION.PROVINCIA,
                            CodigoPostalSap = item.DIRECCION.CODIGO_POSTAL
                        });
                    }
                    logger.Debug(devolucion.ToXml());

                    logger.Debug("Sin Error");
                    return lista;
                }
            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP al obtener mails de proveedores.", e);
                throw;
            }
        }

        private List<ZMPES6280> CrearLista(List<string> cuits)
        {
            var cuit = new List<ZMPES6280>();
            foreach (var item in cuits)
            {
                cuit.Add(new ZMPES6280 { CUIT = item });
            }
            return cuit;
        }

        private List<Zmpes6280> CrearListaSinPi(List<string> cuits)
        {
            var cuit = new List<Zmpes6280>();
            foreach (var item in cuits)
            {
                cuit.Add(new Zmpes6280 { Cuit = item });
            }
            return cuit;
        }
    }
}
