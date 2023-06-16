using System;

namespace Molinos.DataAgro.Repository
{
    public interface ICache : IDisposable
    {
        bool Existe(string clave);
        TEntidad Obtener<TEntidad>(string clave) where TEntidad : class;
        void Remover(string clave);
        TEntidad Agregar<TEntidad>(string clave, TEntidad entidad, DateTimeOffset? tiempoDeExpiracion = null) where TEntidad : class;
    }
}