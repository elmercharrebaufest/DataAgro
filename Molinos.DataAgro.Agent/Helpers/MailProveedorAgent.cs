using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ObtenerMailProveedor;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class MailProveedorAgent : IMailProveedorAgent
    {
        public MailProveedorAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        public List<MailProveedorDto> Ejecutar(List<string> cuits)
        {
            try
            {
                logger.Debug("ObtenerMailProveedor ");

                //var listaa = new List<ZMPES6280>() { new ZMPES6280 { CUIT = "30711160163" } };

                var request = new Z_MPRFC_OBTENER_MAILS
                {
                    IM_CUIT = CrearLista(cuits).ToArray(),

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
                        Pesificado = (item.REMARK == "" || item.REMARK.ToUpper() == "PESIFICADOS"  || item.REMARK.ToUpper() == "PAGOS") ? item.MAIL : ""
                    });
                }
                logger.Debug(devolucion.ToXml());

                logger.Debug("Sin Error");
                return lista;
            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP", e);
                throw e;
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
