using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WebDataAgro.Seguridad
{
    public class AuthorizationManager : ServiceAuthorizationManager
    {
        public AuthorizationManager()
        {
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            // IP del cliente
            var remote = operationContext.IncomingMessageProperties[
                RemoteEndpointMessageProperty.Name
            ] as RemoteEndpointMessageProperty;

            string ip = remote?.Address ?? "0.0.0.0";

            // Host desde cabeceras HTTP
            var http = operationContext.IncomingMessageProperties[
                HttpRequestMessageProperty.Name
            ] as HttpRequestMessageProperty;

            string host = http?.Headers["Host"] ?? "(sin host)";

            // Nombre de la operación que están invocando
            string operacion = operationContext.IncomingMessageHeaders.Action ?? "(sin action)";

            // Registrar log
            AccessLog.Registrar(ip, host, operacion);

            // Leer whitelist
            var config = WhitelistLoader.GetConfig();

            // Validar IP
            bool ipValida = config.IpsPermitidas.Contains(ip);

            // Validar Dominio
            string hostLimpio = host.Split(':')[0]; // si viene con puerto
            bool dominioValido = config.DominiosPermitidos.Contains(hostLimpio, StringComparer.OrdinalIgnoreCase);

            // Resultado final
            return ipValida || dominioValido;
        }

    }

    public class WhitelistConfig
    {
        public List<string> IpsPermitidas { get; set; }
        public List<string> DominiosPermitidos { get; set; }
    }
    public static class WhitelistLoader
    {
        private static WhitelistConfig _cache;
        private static readonly object _lock = new object();

        public static WhitelistConfig GetConfig()
        {
            if (_cache != null)
                return _cache;

            lock (_lock)
            {
                if (_cache != null)
                    return _cache;

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "whitelist.json");

                if (!File.Exists(path))
                    throw new FileNotFoundException("No se encontró el whitelist.json", path);

                string json = File.ReadAllText(path);
                _cache = JsonConvert.DeserializeObject<WhitelistConfig>(json);

                return _cache;
            }
        }
    }
    public static class AccessLog
    {
        private static readonly object _lock = new object();

        public static void Registrar(string ip, string host, string operacion)
        {
            lock (_lock)
            {
                string hostPorIP = "";
                try
                {
                    IPHostEntry entry = Dns.GetHostEntry(ip);
                    hostPorIP = entry.HostName;
                }
                catch
                {
                    hostPorIP = "No resuelto";
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accesos.log");

                string linea = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | IP: {ip} | Host: {host} / {hostPorIP} | Operación: {operacion}";
                File.AppendAllLines(path, new[] { linea });
            }
        }
    }
}
