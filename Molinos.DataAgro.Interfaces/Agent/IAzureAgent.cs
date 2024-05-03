using Microsoft.WindowsAzure.Storage.Blob;

namespace Molinos.DataAgro.Interfaces.Agent
{
    public interface IAzureAgent
    {
        string GenerarTokenSAS();
        CloudBlobContainer GenerarBlobContainer(string sasToken);
        string GuardarImagenEnAzure(CloudBlobContainer cloudBlobContainer, string rutaArchivo, byte[] imageBytes);
    }
}
