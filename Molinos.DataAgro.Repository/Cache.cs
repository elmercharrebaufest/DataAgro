using System;
using System.Runtime.Caching;

namespace Molinos.DataAgro.Repository
{
    public class Cache : ICache
    {
        private readonly MemoryCache cache;

        public Cache()
        {
            cache = MemoryCache.Default;
        }

        public TEntidad Agregar<TEntidad>(string clave, TEntidad entidad, DateTimeOffset? tiempoDeExpiracion = null) where TEntidad : class
        {
            cache.Set(clave, entidad, tiempoDeExpiracion ?? DateTimeOffset.Now.AddSeconds(30));
            return entidad;
        }

        public void Dispose()
        {
            cache.Dispose();
        }

        public bool Existe(string clave)
        {
            return cache.Contains(clave);
        }

        public TEntidad Obtener<TEntidad>(string clave) where TEntidad : class
        {
            return (TEntidad)cache.Get(clave);
        }

        public void Remover(string clave)
        {
            if (Existe(clave)) cache.Remove(clave);
        }
    }
}