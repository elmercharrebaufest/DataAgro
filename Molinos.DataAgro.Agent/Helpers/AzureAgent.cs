using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Molinos.DataAgro.Interfaces.Agent;
using System;
using System.Configuration;
using System.IO;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class AzureAgent: IAzureAgent
    {
        readonly String connectionStringAzure = ConfigurationManager.AppSettings["ConnectionStringAzure"];
        private const string NOMBRE_CONTENEDOR = "research";

        public string GenerarTokenSAS()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionStringAzure);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(NOMBRE_CONTENEDOR);
            // Definir los permisos del SAS token
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),// Tiempo de inicio del acceso (15 minutos antes del tiempo actual)
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),   // Tiempo de expiración del acceso (1 hora después del tiempo actual)
                Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
            };
            // Generar el SAS token
            string sasToken = container.GetSharedAccessSignature(sasConstraints, null);

            return sasToken;
        }

        public CloudBlobContainer GenerarBlobContainer(string sasToken)
        {
            // Combinar la cadena de conexión con el SAS token
            string containerUriWithSas2 = $"{GetContainerUri(connectionStringAzure, NOMBRE_CONTENEDOR)}{sasToken}";
            // Crear un CloudBlobContainer con la URL del contenedor y el SAS token
            CloudBlobContainer cloudBlobContainer = new CloudBlobContainer(new Uri(containerUriWithSas2));

            return cloudBlobContainer;
        }

        public string GuardarImagenEnAzure(CloudBlobContainer cloudBlobContainer, string rutaArchivo, byte[] imageBytes) {
            string blobUri = "";
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(rutaArchivo);

            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                blob.UploadFromStream(memoryStream);
                blobUri = blob.Uri.ToString();
            }

            return blobUri;
        }

        private string GetContainerUri(string connectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);

            return container.Uri.ToString();
        }
    }
}
