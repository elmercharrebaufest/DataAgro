using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Interfaces.Managers;
using NLog;
using System;
using System.ServiceModel;

namespace WebDataAgro.Services
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true)]
    public class DataAgroCBServices : IDataAgroCBServices
    {
        private static class ErrorCatalogo
        {
            public const int RequestNulo = 52001;
            public const int RespuestaNula = 52002;
            public const int ErrorInterno = 52003;

            public const string MsgRequestNulo = "El request recibido es nulo.";
            public const string MsgRespuestaNula = "La operacion no devolvio respuesta.";
            public const string MsgErrorInterno = "Error interno al procesar la operacion SAP de Control de Boletos.";
        }

        private readonly ILogger logger;
        private readonly IControlDeBoletosSAPManager controlDeBoletosSapManager;

        public DataAgroCBServices(
            ILogger logger,
            IControlDeBoletosSAPManager controlDeBoletosSapManager)
        {
            this.logger = logger;
            this.controlDeBoletosSapManager = controlDeBoletosSapManager;
        }

        private static ErrorMessage CrearError(int codigo, string source, string mensaje)
        {
            return new ErrorMessage(codigo, mensaje)
            {
                Source = source
            };
        }

        private Resultado EjecutarOperacion(string nombreOperacion, Func<Resultado> operacion)
        {
            var resultado = new Resultado();

            try
            {
                var respuesta = operacion();

                if (respuesta == null)
                {
                    resultado.Errores.Add(CrearError(
                        ErrorCatalogo.RespuestaNula,
                        nombreOperacion,
                        string.Format("{0} Operacion: {1}", ErrorCatalogo.MsgRespuestaNula, nombreOperacion)));
                    logger.Error("{0} devolvio null.", nombreOperacion);
                    return resultado;
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                resultado.Errores.Add(CrearError(
                    ErrorCatalogo.ErrorInterno,
                    nombreOperacion,
                    string.Format("{0} Codigo: {1}", ErrorCatalogo.MsgErrorInterno, ErrorCatalogo.ErrorInterno)));
                logger.Error(ex, "Error en {0}.", nombreOperacion);
                return resultado;
            }
        }

        public Resultado RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionServiceDto controlDeBoletosPreCertificacion)
        {
            logger.Info("Inicio {0}.", nameof(RegistrarDatosPreCertificacion));

            if (controlDeBoletosPreCertificacion == null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add(CrearError(
                    ErrorCatalogo.RequestNulo,
                    nameof(RegistrarDatosPreCertificacion),
                    ErrorCatalogo.MsgRequestNulo));
                return resultado;
            }

            return EjecutarOperacion(
                nameof(RegistrarDatosPreCertificacion),
                () => controlDeBoletosSapManager.RegistrarDatosPreCertificacion(controlDeBoletosPreCertificacion));
        }

        public Resultado RegistrarDatosSeguimiento(ControlDeBoletosDatosSeguimientoServiceDto controlDeBoletosDatosSeguimiento)
        {
            logger.Info("Inicio {0}.", nameof(RegistrarDatosSeguimiento));

            if (controlDeBoletosDatosSeguimiento == null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add(CrearError(
                    ErrorCatalogo.RequestNulo,
                    nameof(RegistrarDatosSeguimiento),
                    ErrorCatalogo.MsgRequestNulo));
                return resultado;
            }

            return EjecutarOperacion(
                nameof(RegistrarDatosSeguimiento),
                () => controlDeBoletosSapManager.RegistrarDatosSeguimiento(controlDeBoletosDatosSeguimiento));
        }


    }
}
