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
                    IM_CONTRATO = new ZMPES5270
                    {
                        CANTIDAD = Convert.ToDecimal(contrato.Cantidad),
                        CONTR_DATAAGRO = contrato.ContratoId.ToString(),
                        COSECHA = campaniaDescripcion,
                        DIAS_DIFERIM = contrato.DiasPesificado != null ? contrato.DiasPesificado.Value.ToString() : "0",
                        FECHA_DESDE = contrato.FechaDesde.ToString("yyyy-MM-dd"),
                        FECHA_ENTREGA = contrato.FechaEntrega.ToString("yyyy-MM-dd"),
                        FECHA_HASTA = contrato.FechaHasta.ToString("yyyy-MM-dd"),
                        FECHA_LIMITE = fechaDolarizadoString,
                        GRUPO_COMPRAS = "",
                        MONEDA = contrato.Moneda.MonedaId,
                        NO_INFORMAR_SIO = noInformaSioString,
                        PAGO_DIFERIDO = pagoDiferidoString,
                        MATERIAL = materialCodigo,
                        PAGO_DIF_ARP = pagoDifArpString,
                        PRECIO = contrato.Precio,
                        PROVEEDOR = proveedorCUIT,
                        PROVINCIA = provinciaId,
                        SUSTENTABLE = sustentableString,
                        TRIGO_ESPECIAL = trigoEspecialString,
                        FECHA = contrato.Fecha.ToString("yyyy-MM-dd"),
                        USUARIO = UsuarioComercial,
                        HORAACT = contrato.Fecha.ToString("HH:mm:ss"),
                        PROCEDENCIA = localidadString
                    },
                    IM_TOPES_FIJ = new ZMPES5280
                    {
                        
                    },
                    IM_DESC_BONIF = new ZMPES5290 [] 
                    {
                        new ZMPES5290
                        {
                            IMPORTE_DB = contrato.ImporteSustentable != null ? contrato.ImporteSustentable.Value : 0,
                            
                        }
                    },
                    IM_CALIDAD = new ZMPES5300[] {},
                    IM_TIPO_NEGOCIO = tiponegocioDescripcion,
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
