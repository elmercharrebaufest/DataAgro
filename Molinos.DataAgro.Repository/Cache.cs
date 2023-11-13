using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Web.Caching;
using System.Web.UI.WebControls;

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
        public List<KeyValuePair<string,string>> ListAllCacheItems()
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            var cacheItems = cache.Select(kvp => kvp);

            foreach (var cacheItem in cacheItems)
            {
                list.Add(new KeyValuePair<string, string>(cacheItem.Key, cacheItem.Value.ToString()));
            }
            return list;
        }
    }
}