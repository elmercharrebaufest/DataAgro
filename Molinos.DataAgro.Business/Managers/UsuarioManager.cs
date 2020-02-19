using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business
{
    public class UsuarioManager : IUsuarioManager
    {
        private readonly IRepositorio repositorio;

        public UsuarioManager(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public bool AceptoTerminosYCondiciones(string nombre, string Cuit)
        {
            return repositorio.Existe<UsuarioExterno>(x => x.Nombre == nombre && x.Proveedor.CUIT == Cuit && x.AceptaTyC);
        }
        public void Aceptar(string nombre, string Cuit)
        {
            var proveedorId = repositorio.Obtener<Proveedor,int>(x=>x.CUIT == Cuit, x=>x.ProveedorId);
            repositorio.Agregar(new UsuarioExterno { Nombre = nombre, ProveedorId= proveedorId,AceptaTyC = true });
            repositorio.GuardarCambios();
        }
    }
}




