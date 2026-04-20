using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using NLog;
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
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                logger.Debug("MailProveedorAgent - ValorPruebaSap = 1, se devuelve valor de prueba.");
                return new List<MailProveedorDto>
                {
                    new MailProveedorDto{ Cuit = "30711276293", Pesificado = "dataagro@molinosagro.com.ar", DireccionSap = "ENTRE RIOS 1328", LocalidadSap = "CONCORDIA", ProvinciaSap = "ENTRE RIOS", CodigoPostalSap = "2451" },
                    new MailProveedorDto{ Cuit = "30714744956", Pesificado = "dataagro@molinosagro.com.ar", DireccionSap = "SAN MARTIN 232 DPTO L 1", LocalidadSap = "ARROYITO", ProvinciaSap = "CORDOBA", CodigoPostalSap = "2434" },
                    new MailProveedorDto{ Cuit = "30715255592", Pesificado = "dataagro@molinosagro.com.ar", DireccionSap = "INDEPENDENCIA 323", LocalidadSap = "ARTEAGA", ProvinciaSap = "SANTA FE", CodigoPostalSap = "2187" }
                };
            }
            else
            {
                try
                {
                    logger.Debug("Obtener MailProveedor y direcciones SAP");

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
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al obtener mails de proveedores.");
                    throw;
                }
            }
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
