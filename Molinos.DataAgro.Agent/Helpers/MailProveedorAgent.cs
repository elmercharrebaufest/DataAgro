using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ObtenerMailProveedor;
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

                //var listaa = new List<ZMPES6280>() { new ZMPES6280 { CUIT = "30711160163" } };

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
    }
}
