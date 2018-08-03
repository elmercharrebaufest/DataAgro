using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.FinalizarContrato;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class FinalizarContratoAgent
    {
        public FinalizarContratoAgent(ILogger logger)
        {
            this.logger = logger;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public string Finalizar(Contrato contrato, string campaniaDescripcion, string materialCodigo, string provinciaId, string tiponegocioDescripcion, string localidadCod, string proveedorCUIT, string UsuarioComercial)
        {
            try
            {
                SI_ZMPWS_DATAAGRO_PRE_SLIPClient agent = new SI_ZMPWS_DATAAGRO_PRE_SLIPClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                string fechaDolarizadoString = contrato.FechaDolarizado != null ? contrato.FechaDolarizado.Value.ToString("yyyy-MM-dd") : "";
                string pagoDiferidoString = contrato.FechaDolarizado != null ? "X" : "";
                string pagoDifArpString = contrato.DiasPesificado != null && contrato.DiasPesificado.Value != 0 ? "X" : "";
                string sustentableString = contrato.ImporteSustentable != null && contrato.ImporteSustentable.Value != 0 ? "X" : "";
                string noInformaSioString = contrato.NoInformaSio != null && contrato.NoInformaSio.Value ? "X" : "";
                string trigoEspecialString = contrato.TrigoEspecial != null && contrato.TrigoEspecial.Value ? "X" : "";

                string localidadString = rellenarEspaciosSAP(localidadCod, 5);

                var rq = new Z_MPRFC_PRE_SLIP() {
                    IM_CANTIDAD = Convert.ToDecimal(contrato.Cantidad),
                    IM_CONTRATO_DATAAGRO = contrato.ContratoId.ToString(),
                    IM_COSECHA = campaniaDescripcion,
                    IM_DIAS_DIFERIM = contrato.DiasPesificado != null ? contrato.DiasPesificado.Value.ToString() : "0",
                    IM_FECHA_DESDE = contrato.FechaDesde.ToString("yyyy-MM-dd"),
                    IM_FECHA_ENTREGA = contrato.FechaEntrega.ToString("yyyy-MM-dd"),
                    IM_FECHA_HASTA = contrato.FechaHasta.ToString("yyyy-MM-dd"),
                    IM_FECHA_LIMITE = fechaDolarizadoString,
                    IM_GRUPO_COMPRAS = "",
                    IM_IMPORTE_DB = contrato.ImporteSustentable != null ? contrato.ImporteSustentable.Value : 0,
                    IM_LOCALIDAD = localidadString,
                    IM_MATERIAL = materialCodigo,
                    IM_MONEDA = contrato.Moneda.MonedaId,
                    IM_NO_INFORMAR_SIO = noInformaSioString,
                    IM_PAGO_DIFERIDO = pagoDiferidoString,
                    IM_PAGO_DIF_ARP = pagoDifArpString,
                    IM_PRECIO = contrato.Precio,
                    IM_PRECIOSpecified = true,
                    IM_PROVEEDOR = proveedorCUIT,
                    IM_PROVINCIA = provinciaId,
                    IM_SUSTENTABLE = sustentableString,
                    IM_TIPO_NEGOCIO = tiponegocioDescripcion,
                    IM_TRIGO_ESPECIAL = trigoEspecialString,
                    IM_USUARIO = UsuarioComercial,
                    IM_FECHA = contrato.Fecha.ToString("yyyy-MM-dd"),
                    IM_HORA = contrato.Fecha.ToString("HH:mm:ss")
                };

                logger.Debug(rq.ToXml());
                var devolucion = agent.SI_ZMPWS_DATAAGRO_PRE_SLIP(rq);
                logger.Debug(devolucion.ToXml());

                if (devolucion.EX_MENSAJE_ERROR != null && devolucion.EX_MENSAJE_ERROR != "")
                {
                    throw new Exception(devolucion.EX_MENSAJE_ERROR);
                }
                
                return devolucion.EX_CONTRATO_SAP;
            }
            catch (Exception e) {
                logger.Error("Error comunicacion SAP",e);
                throw e;
            }

        }

        private string rellenarEspaciosSAP(string value, int stringLength)
        {
            if (value != null) {
                int cantCeros = stringLength - value.Length;
                for (int i = 0; i < cantCeros; i++) {
                    value = " " + value;
                }                
            }
            return value;
        }


    }
}
